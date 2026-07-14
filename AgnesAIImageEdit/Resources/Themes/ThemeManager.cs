using System;
using System.Windows;

namespace AgnesAIImageEdit.Resources.Themes
{
	public static class ThemeManager
	{
		private const string LightDict = "/Resources/Themes/Light.xaml";
		private const string DarkDict = "/Resources/Themes/Dark.xaml";

		public static void ApplyTheme(Application app, bool isDark)
		{
			if (app == null) return;

			// Remove existing theme dictionaries
			for (int i = app.Resources.MergedDictionaries.Count - 1; i >= 0; i--)
			{
				var md = app.Resources.MergedDictionaries[i];
				var uri = md.Source?.AbsolutePath ?? "";
				if (uri.EndsWith("/Light.xaml", StringComparison.OrdinalIgnoreCase) ||
					uri.EndsWith("/Dark.xaml", StringComparison.OrdinalIgnoreCase))
				{
					app.Resources.MergedDictionaries.RemoveAt(i);
				}
			}

			// Apply new theme
			var dict = new ResourceDictionary
			{
				Source = new Uri(isDark ? DarkDict : LightDict, UriKind.Relative)
			};
			app.Resources.MergedDictionaries.Add(dict);
		}
	}
}
