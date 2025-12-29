using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Pixel_Crosshair
{
	public class Store : INotifyPropertyChanged
	{
		// Singleton Instance
		private static Store _instance;
		public static Store Instance => _instance ?? (_instance = new Store());
		private Store()
		{
			LoadFromSettings();
		}


		private Color _crosshairColor;
		public Color CrosshairColor
		{
			get => _crosshairColor;
			set
			{
				if (_crosshairColor != value)
				{
					_crosshairColor = value;
					OnPropertyChanged();
					SaveToSettings();
					Debug.WriteLine($"CrosshairColor changed to: {_crosshairColor}");
				}
			}
		}

		private void SaveToSettings()
		{
			var settings = ApplicationData.Current.LocalSettings;
			settings.Values["CrosshairColor"] = CrosshairColor.ToString();

			// Notify other windows (Widget <-> Settings)
			//ApplicationData.Current.SignalDataChanged();
		}

		private void LoadFromSettings()
		{
			var settings = ApplicationData.Current.LocalSettings;
			if (settings.Values.ContainsKey("CrosshairColor"))
			{
				string colorStr = settings.Values["CrosshairColor"].ToString();
				_crosshairColor = (Color)XamlBindingHelper.ConvertValue(typeof(Color), colorStr);
			}
			else
			{
				_crosshairColor = Colors.Cyan; // Default
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected async void OnPropertyChanged([CallerMemberName] string name = null)
		{
			var dispatcher = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().Dispatcher;
			await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
			});
		}
	}

	public class ColorToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is Color color)
			{
				return new SolidColorBrush(color);
			}
			return new SolidColorBrush(Colors.Cyan); // Fallback
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
