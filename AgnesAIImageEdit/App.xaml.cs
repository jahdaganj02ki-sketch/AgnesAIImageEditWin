using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using AgnesAIImageEdit.Models;
using AgnesAIImageEdit.Services;
using AgnesAIImageEdit.Views;

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
			GpuProbe.ApplyRenderingMode();

			base.OnStartup(e);

			if (!KeyVault.HasKey())
			{
				try
				{
					// Show MainWindow first (hidden) so the native HWND exists.
					// WPF PasswordBox crashes silently when the modal dialog has
					// no Owner HWND because it cannot process input internally.
					MainWindow?.Show();

					var w = new SettingsWindow
					{
						Owner = MainWindow,
						WindowStartupLocation = WindowStartupLocation.CenterOwner
					};
					w.ShowDialog();
				}
				catch (Exception ex)
				{
					MessageBox.Show(
						$"Settings window error:\n\n{ex.GetType().Name}: {ex.Message}\n\n{ex.StackTrace}",
						"Startup Error",
						MessageBoxButton.OK,
						MessageBoxImage.Error);
				}
			}
		}
	}
}
