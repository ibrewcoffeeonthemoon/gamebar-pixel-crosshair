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
        private XboxGameBarWidget widget = null;

        public WidgetPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            widget = e.Parameter as XboxGameBarWidget;

            widget.GameBarDisplayModeChanged += OnGameBarDisplayModeChanged;

			widget.SettingsClicked += OnWidgetSettingsButtonClicked;

            widget.CloseRequested += OnWidgetCloseRequested;

            ApplicationData.Current.DataChanged += OnApplicationDataChanged;

			ApplicationData.Current.SignalDataChanged();
		}

        private void OnWidgetCloseRequested(XboxGameBarWidget sender, XboxGameBarWidgetCloseRequestedEventArgs args)
        {
			ApplicationData.Current.DataChanged -= OnApplicationDataChanged;
        }

        private void OnGameBarDisplayModeChanged(XboxGameBarWidget sender, object args)
        {
            var isPinned = sender.GameBarDisplayMode == XboxGameBarDisplayMode.PinnedOnly;
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                CenterAppButton.Visibility = isPinned ? Visibility.Collapsed : Visibility.Visible;
            });
        }

        private async void OnCenterAppButtonClick(object sender, RoutedEventArgs e)
        {
            await widget.CenterWindowAsync();
        }

		private async void OnWidgetSettingsButtonClicked(XboxGameBarWidget sender, object args)
		{
            // if necessary pre-configure any required data needed by the settings widget prior to activation
            // ...
            await sender.ActivateSettingsAsync();
		}

        private void OnApplicationDataChanged(ApplicationData sender, object args)
        {
			_ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				var settings = ApplicationData.Current.LocalSettings;
				string colorStr = settings.Values["CrosshairColor"].ToString();
				var color = (Color)XamlBindingHelper.ConvertValue(typeof(Color), colorStr);
				// Update all Rectangles inside the CrosshairContainer
				foreach (var child in CrosshairPreviewGrid.Children)
				{
					if (child is Rectangle rect)
						rect.Fill = new SolidColorBrush(color);
				}
			});
        }
    }
}
