using System;
using System.ComponentModel;
using System.Diagnostics;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Pixel_Crosshair
{
	public class Store : INotifyPropertyChanged
	{
		private readonly Page _page = null;
		public event PropertyChangedEventHandler PropertyChanged;
		private readonly ApplicationDataContainer _settings = ApplicationData.Current.LocalSettings;

		public Store(Page page)
		{
			_page = page;
			// Listen for "Broadcasts" from other windows
			ApplicationData.Current.DataChanged += (s, e) => SyncFromStorage();
		}

		public Color CrosshairColor
		{
			get
			{
				// Default to Cyan if not set
				if (!_settings.Values.ContainsKey("CrosshairColor"))
					return Colors.Cyan;
				// Read the color from storage, convert from string to Color then return
				string colorStr = _settings.Values["CrosshairColor"].ToString();
				Color color = (Color)XamlBindingHelper.ConvertValue(typeof(Color), colorStr);
				return color;
			}
			set
			{
				// Store the color as a string into storage
				_settings.Values["CrosshairColor"] = value.ToString();
				Debug.WriteLine($"[Store] CrosshairColor set to {value}");
				// Notify THIS window immediately
				OnPropertyChanged(nameof(CrosshairColor));
				// Notify OTHER windows via the system broadcast
				ApplicationData.Current.SignalDataChanged();
			}
		}

		private async void SyncFromStorage()
		{
			// When the "Other" window signals a change, 
			// we force this window to re-read the values.
			await _page.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				OnPropertyChanged(nameof(CrosshairColor));
			});
		}

		protected void OnPropertyChanged(string name)
		{

			_ = _page.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
			});
		}
	}
}
