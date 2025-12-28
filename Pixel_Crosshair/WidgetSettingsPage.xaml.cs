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
			GenerateMatrix();
			// Load settings from storage when the page is initialized
			LoadSettings();
			LoadMatrixSettings();
        }

		private void GenerateMatrix()
		{
			MatrixGrid.Children.Clear();

			for (int i = 0; i < 100; i++)
			{
				CheckBox cb = new CheckBox
				{
					MinWidth = 0,
					MinHeight = 0,
					Padding = new Thickness(0),
					Margin = new Thickness(0),
					Tag = i // Store the index (0-99) to identify the pixel later
				};

				cb.Checked += OnPixelToggled;
				cb.Unchecked += OnPixelToggled;

				MatrixGrid.Children.Add(cb);
			}
		}

		void LoadSettings()
		{
			// get saved color from storage
			var settings = ApplicationData.Current.LocalSettings;
			string colorStr = settings.Values["CrosshairColor"].ToString();
			var color = (Color)Windows.UI.Xaml.Markup.XamlBindingHelper.ConvertValue(typeof(Color), colorStr);
			// set the ColorPicker's color retrieved from storage
			ColorPicker.Color = color;
		}

		private void LoadMatrixSettings()
		{
			var settings = ApplicationData.Current.LocalSettings;
			if (settings.Values.ContainsKey("CrosshairMatrix"))
			{
				string savedState = settings.Values["CrosshairMatrix"].ToString();

				for (int i = 0; i < MatrixGrid.Children.Count; i++)
				{
					if (MatrixGrid.Children[i] is CheckBox cb && i < savedState.Length)
					{
						// Temporarily remove the event handler so we don't 
						// trigger a 'Save' while we are 'Loading'
						cb.Checked -= OnPixelToggled;
						cb.Unchecked -= OnPixelToggled;

						cb.IsChecked = savedState[i] == '1';

						cb.Checked += OnPixelToggled;
						cb.Unchecked += OnPixelToggled;
					}
				}
			}
		}

		private async void OnColorPickerColorChanged(object sender, ColorChangedEventArgs e)
        {
			// save user selected color to storage
			var settings = ApplicationData.Current.LocalSettings;
            settings.Values["CrosshairColor"] = ColorPicker.Color.ToString();
			// signal that application data has changed
			ApplicationData.Current.SignalDataChanged();
		}

		private void OnPixelToggled(object sender, RoutedEventArgs e)
		{
			// Get all checkboxes, order them by their index (Tag), 
			// and turn 'Checked' into '1' and 'Unchecked' into '0'
			var stateString = string.Join("", MatrixGrid.Children
				.OfType<CheckBox>()
				.OrderBy(cb => (int)cb.Tag)
				.Select(cb => cb.IsChecked == true ? "1" : "0"));

			var settings = ApplicationData.Current.LocalSettings;
			settings.Values["CrosshairMatrix"] = stateString;

			// Signal the other window to update!
			ApplicationData.Current.SignalDataChanged();
		}
	}
}
