using System;
using Microsoft.Gaming.XboxGameBar;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
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
		// reference to the Xbox Game Bar widget instance
		private XboxGameBarWidget widget = null;

        public WidgetPage()
        {
			// Initialize the XAML components
			this.InitializeComponent();
			InitializeCrosshairGrid();
        }

		private void InitializeCrosshairGrid()
		{
			// Clear any existing definitions
			CrosshairPreviewGrid.Children.Clear();
			CrosshairPreviewGrid.RowDefinitions.Clear();
			CrosshairPreviewGrid.ColumnDefinitions.Clear();
			// Create the 10x10 coordinate system
			for (int i = 0; i < 10; i++)
			{
				CrosshairPreviewGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Pixel) });
				CrosshairPreviewGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Pixel) });
			}

			// Populate with 100 hidden pixel rectangles
			for (int i = 0; i < 100; i++)
			{
				var rect = new Rectangle
				{
					Width = 1,
					Height = 1,
					Visibility = Visibility.Collapsed
				};
				Grid.SetRow(rect, i / 10);
				Grid.SetColumn(rect, i % 10);
				CrosshairPreviewGrid.Children.Add(rect);
			}
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
        {
			// get the widget instance
			widget = e.Parameter as XboxGameBarWidget;
			// initial update of UI based on current display mode
			widget.SettingsClicked += OnWidgetSettingsButtonClicked;

			// listen to application data changes
			ApplicationData.Current.DataChanged += OnApplicationDataChanged;
			// simulate a data change to load initial settings
			ApplicationData.Current.SignalDataChanged();
			// listen to close request to clean up old event handlers from previous instances
			widget.CloseRequested += OnWidgetCloseRequested;
		}

        private void OnWidgetCloseRequested(XboxGameBarWidget sender, XboxGameBarWidgetCloseRequestedEventArgs args)
        {
			// must clean up old event handlers to avoid multiple subscriptions when the widget is reopened
			ApplicationData.Current.DataChanged -= OnApplicationDataChanged;
        }

        private async void OnCenterAppButtonClick(object sender, RoutedEventArgs e)
        {
			// center the widget window on screen
			await widget.CenterWindowAsync();
        }

		private async void OnWidgetSettingsButtonClicked(XboxGameBarWidget sender, object args)
		{
			// launch the settings page
			// if necessary pre-configure any required data needed by the settings widget prior to activation
			// ...
			await sender.ActivateSettingsAsync();
		}

        private void OnApplicationDataChanged(ApplicationData sender, object args)
        {
			// let the UI thread handle the update and also fetch the settings from application data
			_ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				// fetch data from application data and convert to SolidColorBrush object
				var settings = ApplicationData.Current.LocalSettings;
				string colorStr = settings.Values["CrosshairColor"].ToString();
				var color = (Color)XamlBindingHelper.ConvertValue(typeof(Color), colorStr);
				var brush = new SolidColorBrush(color);
				// fetch matrix state from application data (or default to all '0's)
				string matrixStr = settings.Values.ContainsKey("CrosshairMatrix")
					? settings.Values["CrosshairMatrix"].ToString()
					: new string('0', 100);
				// update UI, set all rectangles in the preview grid to the new brush and visibility based on matrix state
				for (int i = 0; i < 100; i++)
				{
					if (CrosshairPreviewGrid.Children[i] is Rectangle rect)
					{
						// Update Color
						rect.Fill = brush;
						// Update Visibility (Shape)
						rect.Visibility = (matrixStr[i] == '1') ? Visibility.Visible : Visibility.Collapsed;
					}
				}
			});
        }
    }
}
