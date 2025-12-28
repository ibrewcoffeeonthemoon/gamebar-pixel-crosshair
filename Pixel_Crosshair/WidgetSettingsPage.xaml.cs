using System.Linq;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Pixel_Crosshair
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WidgetSettingsPage : Page
    {
        public WidgetSettingsPage()
        {
			// Initialize the XAML components
			this.InitializeComponent();
			InitializePixelLayoutEditerGrid();

			// Load settings from storage when the page is initialized
			LoadSettings();
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
				// Subscribe to Checked and Unchecked events
				cb.Checked += OnPixelLayoutEditorCheckboxToggled;
				cb.Unchecked += OnPixelLayoutEditorCheckboxToggled;
				// Position the CheckBox in the grid
				PixelLayoutEditorGrid.Children.Add(cb);
			}
		}

		void LoadSettings()
		{
			// Get settings from storage
			var settings = ApplicationData.Current.LocalSettings;

			// Restore ColorPicker State
			if (settings.Values.ContainsKey("CrosshairColor"))
			{
				// Get saved color string, convert to Color and set ColorPicker
				string colorStr = settings.Values["CrosshairColor"].ToString();
				var color = (Color)Windows.UI.Xaml.Markup.XamlBindingHelper.ConvertValue(typeof(Color), colorStr);
				PixelColorPicker.Color = color;
			}

			// Restore Matrix/Layout State
			if (settings.Values.ContainsKey("CrosshairLayout"))
			{
				// Get saved state string
				string savedState = settings.Values["CrosshairLayout"].ToString();
				// Iterate through each CheckBox and restore its state
				for (int i = 0; i < PixelLayoutEditorGrid.Children.Count; i++)
				{
					if (PixelLayoutEditorGrid.Children[i] is CheckBox cb && i < savedState.Length)
					{
						// Unsubscribe to prevent triggering Save during Load
						cb.Checked -= OnPixelLayoutEditorCheckboxToggled;
						cb.Unchecked -= OnPixelLayoutEditorCheckboxToggled;
						// Restore state
						cb.IsChecked = savedState[i] == '1';
						// Resubscribe
						cb.Checked += OnPixelLayoutEditorCheckboxToggled;
						cb.Unchecked += OnPixelLayoutEditorCheckboxToggled;
					}
				}
			}
		}

		private async void OnColorPickerColorChanged(object sender, ColorChangedEventArgs e)
        {
			// Save user selected color to storage
			var settings = ApplicationData.Current.LocalSettings;
            settings.Values["CrosshairColor"] = PixelColorPicker.Color.ToString();

			// Signal that application data has changed
			ApplicationData.Current.SignalDataChanged();
		}

		private void OnPixelLayoutEditorCheckboxToggled(object sender, RoutedEventArgs e)
		{
			// Save the current layout state to storage, as a string of '1's and '0's
			var stateString = string.Join("", PixelLayoutEditorGrid.Children
				.OfType<CheckBox>()
				.OrderBy(cb => (int)cb.Tag)
				.Select(cb => cb.IsChecked == true ? "1" : "0"));

			// Store in application data
			var settings = ApplicationData.Current.LocalSettings;
			settings.Values["CrosshairLayout"] = stateString;

			// Signal the other window to update!
			ApplicationData.Current.SignalDataChanged();
		}
	}
}
