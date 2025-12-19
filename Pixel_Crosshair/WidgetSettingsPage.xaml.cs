using Windows.Storage;
using Windows.UI;
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
			// Load settings from storage when the page is initialized
			LoadSettings();
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

        private async void OnColorPickerColorChanged(object sender, ColorChangedEventArgs e)
        {
			// save user selected color to storage
			var settings = ApplicationData.Current.LocalSettings;
            settings.Values["CrosshairColor"] = ColorPicker.Color.ToString();
			// signal that application data has changed
			ApplicationData.Current.SignalDataChanged();
		}
	}
}
