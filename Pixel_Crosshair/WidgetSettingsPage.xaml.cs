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
		private readonly Store _store = null;

		public WidgetSettingsPage()
		{
			// Initialize Store and bind to DataContext
			_store = new Store(this);
			DataContext = _store;

			// Initialize the XAML components
			this.InitializeComponent();
			InitializePixelLayoutEditerGrid();

			// Clean up when setting window closed 
            Window.Current.Closed += Current_Closed;
		}

        private void Current_Closed(object sender, Windows.UI.Core.CoreWindowEventArgs e)
        {
			// Unsubscribe previous store 
			_store.Unregister();
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
				// Binding to CrosshairLayout, OneWay
				cb.SetBinding(CheckBox.IsCheckedProperty, new Binding
				{
					Source = _store,
					Path = new PropertyPath("CrosshairLayout"),
					Converter = new LayoutToBoolConverter(),
					ConverterParameter = i.ToString(),
					Mode = BindingMode.OneWay,
				});
				// 
				cb.Click += OnPixelCheckboxClick;
				// Position the CheckBox in the grid
				PixelLayoutEditorGrid.Children.Add(cb);
			}
		}

		private void OnPixelCheckboxClick(object sender, RoutedEventArgs e)
		{
			var cb = sender as CheckBox;
			int index = (int)cb.Tag; // Which pixel was clicked?

			// Get the current state from the store instance
			char[] layout = _store.CrosshairLayout.ToCharArray();

			// Update the specific bit (The "Reducer" step)
			layout[index] = (cb.IsChecked == true) ? '1' : '0';

			// Push the new state back to the store
			_store.CrosshairLayout = new string(layout);
		}

		private void OnPixelLayoutEditorClearButtonClicked(object sender, RoutedEventArgs e)
		{
			// Push the new state back to the store
			_store.CrosshairLayout = new string('0', 100);
			// Manually clear the checkbox (dirty hack)
			string layout = _store.CrosshairLayout;
			for (int i = 0; i < PixelLayoutEditorGrid.Children.Count; i++)
			{
				if (PixelLayoutEditorGrid.Children[i] is CheckBox cb)
				{
					cb.IsChecked = layout[i] == '1';
				}
			}
		}
	}
}
