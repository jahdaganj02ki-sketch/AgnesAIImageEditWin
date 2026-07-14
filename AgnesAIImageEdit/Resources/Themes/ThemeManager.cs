using System;
using System.Windows;

namespace AgnesAIImageEdit.Resources.Themes
{
	public static class ThemeManager
	{
		private const string ApplicationRoot = "pack://application:,,,/";

		private static readonly Uri LightUri = new Uri(ApplicationRoot + "Resources/Themes/Light.xaml", UriKind.Absolute);
		private static readonly Uri DarkUri  = new Uri(ApplicationRoot + "Resources/Themes/Dark.xaml", UriKind.Absolute);

		public static void ApplyTheme(Application app, bool isDark)
		{
			if (app == null) return;

			// Remove existing theme dictionaries by matching the known absolute URI.
			for (int i = app.Resources.MergedDictionaries.Count - 1; i >= 0; i--)
			{
				var md = app.Resources.MergedDictionaries[i];
				if (md.Source == LightUri || md.Source == DarkUri)
					app.Resources.MergedDictionaries.RemoveAt(i);
			}

			var dict = new ResourceDictionary { Source = isDark ? DarkUri : LightUri };
			app.Resources.MergedDictionaries.Add(dict);
		}
	}
}
