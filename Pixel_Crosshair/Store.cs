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
		private readonly Settings _settings = new Settings();
		public event PropertyChangedEventHandler PropertyChanged;

		// Cache for properties value
		private string _cachedCrosshairLayout = null;

		public Store(Page page)
		{
			// ref to page
			_page = page;

			// Listen for "Broadcasts" from other windows
			ApplicationData.Current.DataChanged += OnApplicationDataChanged;

			// Initialize the cached value of properties
			_cachedCrosshairLayout = CrosshairLayout;
		}

        public void Unregister() => ApplicationData.Current.DataChanged -= OnApplicationDataChanged;

		// property
		public Color CrosshairColor
		{
			get => _settings.Get("CrosshairColor", Colors.Cyan);
			set => _settings.Set("CrosshairColor", value);
		}

		// property
		public string CrosshairLayout
		{
			get => _settings.Get("CrosshairLayout", new string('0', 100));
			set => _settings.Set("CrosshairLayout", value);
		}

		private void OnApplicationDataChanged(ApplicationData sender, object args)
		{
			// Read and update everything possible variable
			OnPropertyChanged(nameof(CrosshairColor));

			// Check if currentLayout is different from cached layout
			if (CrosshairLayout != _cachedCrosshairLayout)
			{
				// Only fire event if layout has been updated
				OnPropertyChanged(nameof(CrosshairLayout));
				// Update the cached value
				_cachedCrosshairLayout = CrosshairLayout;
			}
		}

		protected void OnPropertyChanged(string name)
		{
			// Invoke PropertyChangedEvent on specific variable name
			_ = _page.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name))
			);
		}
	}

	public class Settings
	{
		// Local settings application data container
		private readonly ApplicationDataContainer _ctn = ApplicationData.Current.LocalSettings;

		public T Get<T>(string key, T defaultValue)
		{
			// Default value
			if (!_ctn.Values.ContainsKey(key))
				return defaultValue;
			// Get the dict as string
			string valueStr = _ctn.Values[key].ToString();
			// If type T is string already, return it
			if (typeof(T) == typeof(string))
				return (T)(object)valueStr;
			// otherwise convert to type T
			return (T)XamlBindingHelper.ConvertValue(typeof(T), valueStr);
		}

		public void Set(string key, object value)
		{
			// Set dict value as value to string
			_ctn.Values[key] = value.ToString();
			Debug.WriteLine($"[Store] {key} set to {value}");
			// Then signal data changed
			ApplicationData.Current.SignalDataChanged();
		}
	}

	public class ColorToBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is Color color)
				return new SolidColorBrush(color);
			return new SolidColorBrush(Colors.Cyan);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) => 
			throw new NotImplementedException();
	}

	public class LayoutToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is string layout && parameter is string indexStr)
				if (int.TryParse(indexStr, out int index) && index < layout.Length)
					return layout[index] == '1' ? Visibility.Visible : Visibility.Collapsed;
			return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) =>
			throw new NotImplementedException();
	}

	public class LayoutToBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is string layout && parameter is string indexStr)
				if (int.TryParse(indexStr, out int index) && index < layout.Length)
					return layout[index] == '1';
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
