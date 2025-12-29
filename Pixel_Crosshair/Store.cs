using System;
using System.ComponentModel;
using System.Diagnostics;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

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
			ApplicationData.Current.DataChanged += OnApplicationDataChanged;
		}

        public void Unregister()
		{
			ApplicationData.Current.DataChanged -= OnApplicationDataChanged;
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

				// Trigger SignalDataChanged event
				ApplicationData.Current.SignalDataChanged();
			}
		}

		public string CrosshairLayout
		{
			get
			{
				// Default value if not set
				if (!_settings.Values.ContainsKey("CrosshairLayout"))
				{
					return new string('1', 100);
				}
				// Read the zeros and ones string and return
				return _settings.Values["CrosshairLayout"].ToString();
			}
			set
			{
				// Store the value as a string into storage
				_settings.Values["CrosshairLayout"] = value;
				Debug.WriteLine($"[Store] CrosshairLayout set to {value}");

				// Trigger SignalDataChanged event
				ApplicationData.Current.SignalDataChanged();
			}
		}

		private void OnApplicationDataChanged(ApplicationData sender, object args)
		{
			// Read and update everything possible variable
			OnPropertyChanged(string.Empty);
		}

		protected void OnPropertyChanged(string name)
		{
			// Invoke PropertyChangedEvent on specific variable name
			_ = _page.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
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

	public class LayoutToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is string layout && parameter is string indexStr)
			{
				if (int.TryParse(indexStr, out int index) && index < layout.Length)
				{
					return layout[index] == '1' ? Visibility.Visible : Visibility.Collapsed;
				}
			}
			return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}

	public class LayoutToBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is string layout && parameter is string indexStr)
			{
				if (int.TryParse(indexStr, out int index) && index < layout.Length)
				{
					return layout[index] == '1';
				}
			}
			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			if (value is bool isChecked && parameter is string indexStr)
			{
				int index = int.Parse(indexStr);
				var settings = ApplicationData.Current.LocalSettings;

				// Get the latest string directly from storage
				string currentLayout = settings.Values["CrosshairLayout"]?.ToString() ?? new string('0', 100);

				// Modify the bit
				char[] chars = currentLayout.ToCharArray();
				if (index < chars.Length)
				{
					chars[index] = isChecked ? '1' : '0';
					string nextLayout = new string(chars);

					// 3. Return the WHOLE string. 
					// The Binding engine will push this into Store.CrosshairLayout
					return nextLayout;
				}
			}
			return null;
		}
	}
}
