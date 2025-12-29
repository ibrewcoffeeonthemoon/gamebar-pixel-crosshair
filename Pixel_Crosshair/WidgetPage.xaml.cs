using System;
using Microsoft.Gaming.XboxGameBar;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Pixel_Crosshair
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WidgetPage : Page
    {
		// Ref to store
		private readonly Store _store;
		// Ref to the Xbox Game Bar widget instance
		private XboxGameBarWidget _widget;

        public WidgetPage()
        {
			// Initialize Store and bind to DataContext
			_store = new Store(this);
			DataContext = _store;

			// Initialize the XAML components
			this.InitializeComponent();
			InitializeCrosshairGrid();
        }

		private void InitializeCrosshairGrid()
		{
			// Clear any existing definitions
			CrosshairGrid.Children.Clear();
			CrosshairGrid.RowDefinitions.Clear();
			CrosshairGrid.ColumnDefinitions.Clear();
			// Create the 10x10 coordinate system
			for (int i = 0; i < 10; i++)
			{
				// Add Row and Column definitions, each 1 pixel in size
				CrosshairGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Pixel) });
				CrosshairGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Pixel) });
			}

			// Populate with 100 hidden pixel rectangles
			for (int i = 0; i < 100; i++)
			{
				// Create a Rectangle for each pixel
				var rect = new Rectangle
				{
					Width = 1,
					Height = 1,
					Visibility = Visibility.Collapsed,
				};
				// Binding to CrosshairColor of store
				rect.SetBinding(Rectangle.FillProperty, new Binding 
				{ 
					Source = _store,
					Path = new PropertyPath("CrosshairColor"),
					Converter = new ColorToBrushConverter(),
				});
				// Binding to CrosshairLayout of store
				rect.SetBinding(Rectangle.VisibilityProperty, new Binding
				{
					Source = _store,
					Path = new PropertyPath("CrosshairLayout"),
					Converter = new LayoutToVisibilityConverter(),
					ConverterParameter = i.ToString()
				});
				// Position the Rectangle in the grid
				Grid.SetRow(rect, i / 10);
				Grid.SetColumn(rect, i % 10);
				// Add to the grid
				CrosshairGrid.Children.Add(rect);
			}
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
        {
			// Get the widget instance
			_widget = e.Parameter as XboxGameBarWidget;

			// Initial update of UI based on current display mode
			_widget.SettingsClicked += OnWidgetSettingsButtonClicked;

			// listen to close request to clean up old event handlers from previous instances
			_widget.CloseRequested += OnWidgetCloseRequested;
		}

		// On widget closed, unregister previous store
        private void OnWidgetCloseRequested(XboxGameBarWidget sender, XboxGameBarWidgetCloseRequestedEventArgs args) => _store.Unregister();

		// On center app button clicked, center the widget window on screen
        private async void OnCenterAppButtonClick(object sender, RoutedEventArgs e) => await _widget.CenterWindowAsync();

		// On settings button clicked, launch the settings page
		private async void OnWidgetSettingsButtonClicked(XboxGameBarWidget sender, object args) => await sender.ActivateSettingsAsync();
	}
}
