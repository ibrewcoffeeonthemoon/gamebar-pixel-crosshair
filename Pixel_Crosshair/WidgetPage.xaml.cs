using System;
using Microsoft.Gaming.XboxGameBar;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

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
		}

        private void OnGameBarDisplayModeChanged(XboxGameBarWidget sender, object args)
        {
            var isPinned = sender.GameBarDisplayMode == XboxGameBarDisplayMode.PinnedOnly;
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                CenterAppButton.Visibility = isPinned ? Visibility.Collapsed : Visibility.Visible;
                LeftStackPanel.Visibility = isPinned ? Visibility.Collapsed : Visibility.Visible;
                RightStackPanel.Visibility = isPinned ? Visibility.Collapsed : Visibility.Visible;
            });
        }

        private async void OnCenterAppButtonClick(object sender, RoutedEventArgs e)
		{
			await widget.CenterWindowAsync();
		}
	}
}
