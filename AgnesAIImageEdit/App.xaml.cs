using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using AgnesAIImageEdit.Models;
using AgnesAIImageEdit.Services;
using AgnesAIImageEdit.Views;
using AgnesAIImageEdit.ViewModels;
using AgnesAIImageEdit.Resources.Themes;

namespace AgnesAIImageEdit
{
	public partial class App : Application
	{
		internal static string DataDir { get; } =
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AgnesAIImageEdit");

		public App()
		{
			DispatcherUnhandledException += (_, args) =>
			{
				MessageBox.Show(
					$"Unhandled exception:\n\n{args.Exception.GetType().Name}: {args.Exception.Message}\n\n{args.Exception.StackTrace}",
					"Fatal Error",
					MessageBoxButton.OK,
					MessageBoxImage.Error);
				args.Handled = true;
			};
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			Directory.CreateDirectory(DataDir);
			Directory.CreateDirectory(Path.Combine(DataDir, "Outputs"));

			AppSettings.Current = AppSettings.Load();

			// Set OS culture so WPF can load de-DE resources (fixes 0x0407 crash)
			try
			{
				System.Globalization.CultureInfo.DefaultThreadCurrentCulture =
					System.Globalization.CultureInfo.InstalledUICulture ??
					System.Globalization.CultureInfo.CurrentUICulture;
				System.Globalization.CultureInfo.DefaultThreadCurrentUICulture =
					System.Globalization.CultureInfo.InstalledUICulture ??
					System.Globalization.CultureInfo.CurrentUICulture;
			}
			catch { /* best-effort */ }

			GpuProbe.ApplyRenderingMode();

			base.OnStartup(e);

			// Apply saved theme before MainWindow renders
			try
			{
				bool dark = AppSettings.Current.IsDarkMode;
				ThemeManager.ApplyTheme(Application.Current, dark);

				// Push theme into PillButton and other base templates that may
				// have cached DynamicResource bindings before theme swap.
				// We do this by replacing the Style resource entry at runtime,
				// which forces re-evaluation by key.
				if (MainWindow?.Resources.Contains("PillButtonLight") == false && MainWindow != null)
				{
					MainWindow.Resources["PillButtonLight"] = MainWindow.Resources["PillButton"];
				}
			}
			catch { /* non-fatal */ }

			if (!KeyVault.HasKey())
			{
				try
				{
					var settings = new SettingsWindow();
					settings.Loaded += (_, __) =>
					{
						try
						{
							var helper = new WindowInteropHelper(settings);
							var mainHwnd = new WindowInteropHelper(MainWindow).Handle;
							if (mainHwnd != IntPtr.Zero && helper.Handle != mainHwnd)
							{
								SetWindowLong(helper.Handle, GWLP_HWNDPARENT, mainHwnd);
							}
						}
						catch { /* Win32-Owner-Set best-effort */ }
					};

					bool? result = settings.ShowDialog();

					if (result == true)
					{
						MainWindow?.Show();
						ApplyThemeToMainWindow();
					}
					else
					{
						Shutdown();
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(
						$"Settings window error:\n\n{ex.GetType().Name}: {ex.Message}\n\n{ex.StackTrace}",
						"Startup Error",
						MessageBoxButton.OK,
						MessageBoxImage.Error);
					MainWindow?.Show();
				}
			}
			else
			{
				ApplyThemeToMainWindow();
			}
		}

		private void ApplyThemeToMainWindow()
		{
			if (MainWindow?.DataContext is MainViewModel vm)
			{
				vm.ApplyThemeAtStartup(Application.Current);
			}
		}

		protected override void OnExit(ExitEventArgs e)
		{
			if (MainWindow?.DataContext is MainViewModel vm)
			{
				vm.CleanupTempFiles();
			}
			base.OnExit(e);
		}

		private const int GWLP_HWNDPARENT = -8;

		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
	}
}
