using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Pixel_Crosshair
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class WidgetSettingsPage : Page
	{
		// Ref to store
		private readonly Store _store;

		public WidgetSettingsPage()
		{
			// Initialize Store and bind to DataContext
			_store = new Store(this);
			DataContext = _store;

			// Initialize the XAML components
			this.InitializeComponent();
			InitializePixelLayoutEditerGrid();

			// Clean up when setting window closed 
            Window.Current.Closed += OnWidgetSettingsPageWindowClosed;
		}

        private void InitializePixelLayoutEditerGrid()
		{
			// Clear any existing definitions
			PixelLayoutEditorGrid.Children.Clear();

			// Create the 10x10 coordinate system
			for (int i = 0; i < 100; i++)
			{
				// Create a CheckBox for each pixel
				CheckBox cb = new CheckBox
				{
					MinWidth = 0,
					MinHeight = 0,
					Padding = new Thickness(0),
					Margin = new Thickness(0),
					Tag = i // Store the index (0-99) to identify the pixel later
				};
				// Binding to CrosshairLayout
				cb.SetBinding(CheckBox.IsCheckedProperty, new Binding
				{
					Source = _store,
					Path = new PropertyPath("CrosshairLayout"),
					Converter = new LayoutToBoolConverter(),
					ConverterParameter = i.ToString(),
					// Must use TwoWay binding, otherwise the checkbox will auto unregister binding upon user click
					Mode = BindingMode.TwoWay,
				});
				// Position the CheckBox in the grid
				PixelLayoutEditorGrid.Children.Add(cb);
			}
		}

		// On windows close, unsubscribe previous store 
        private void OnWidgetSettingsPageWindowClosed(object sender, CoreWindowEventArgs e) => _store.Unregister();

		// On clear button clicked, push the default layout state back to the store
		private void OnPixelLayoutEditorClearButtonClicked(object sender, RoutedEventArgs e) => _store.CrosshairLayout = new string('0', 100);
	}
}
