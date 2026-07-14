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

		// Remove existing theme dictionaries (use OriginalPath to avoid
		// InvalidOperationException on relative URIs).
		for (int i = app.Resources.MergedDictionaries.Count - 1; i >= 0; i--)
		{
			var md = app.Resources.MergedDictionaries[i];
			var path = md.Source?.OriginalPath ?? "";
			if (path.EndsWith("/Light.xaml", StringComparison.OrdinalIgnoreCase) ||
				path.EndsWith("/Dark.xaml", StringComparison.OrdinalIgnoreCase))
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
