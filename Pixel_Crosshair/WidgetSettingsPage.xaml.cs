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
            this.InitializeComponent();
            LoadSettings();
        }

		void LoadSettings()
		{
			var settings = ApplicationData.Current.LocalSettings;
			string colorStr = settings.Values["CrosshairColor"].ToString();
			ColorPicker.Color = (Color)Windows.UI.Xaml.Markup.XamlBindingHelper.ConvertValue(typeof(Color), colorStr);
		}

        private async void OnColorPickerColorChanged(object sender, ColorChangedEventArgs e)
        {
			var settings = ApplicationData.Current.LocalSettings;
            settings.Values["CrosshairColor"] = ColorPicker.Color.ToString();
            ApplicationData.Current.SignalDataChanged();
		}
	}
}
