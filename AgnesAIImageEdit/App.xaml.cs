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

			// Fix: WPF throws "Cannot find non-neutral culture related to 'en-us'"
			// when MainWindow load-lokalisierten Content bindet, bevor eine
			// neutrale CurrentCulture gesetzt ist. Die CurrentCulture explizit
			// auf die OS-Kultur setzen, bevor base.OnStartup MainWindow erstellt.
			try
			{
				var osCulture = System.Globalization.CultureInfo.InstalledUICulture ??
					System.Globalization.CultureInfo.CurrentUICulture;
				System.Globalization.CultureInfo.DefaultThreadCurrentCulture = osCulture;
				System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = osCulture;
			}
			catch { /* best-effort */ }

			GpuProbe.ApplyRenderingMode();

			base.OnStartup(e);

			if (!KeyVault.HasKey())
			{
				try
				{
					// ── Passwortbox-Bugfix ──────────────────────────────────────
					// 1. SettingsWindow zuerst öffnen (ShowDialog) – MainWindow bleibt
					//    erstellt (base.OnStartup hat sie gebaut) aber NICHT sichtbar.
					// 2. Owner auf MainWindow setzen, ABER über Win32 SetWindowLong,
					//    damit WPF nicht "Cannot set Owner property to itself" wirft.
					// 3. MainWindow danach ausblenden, damit nur das Settings-Fenster
					//    sichtbar ist. Bei Close/Cancel wird die App sauber beendet.
					// ─────────────────────────────────────────────────────────────

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

					// MainWindow ggf. zeigen, falls Settings mit OK geschlossen wurde
					if (result == true)
					{
						MainWindow?.Show();
					}
					else
					{
						// Benutzer hat Settings abgebrochen → App beenden
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
		}

		// ── Win32 Helpers für Owner-Fix ────────────────────────────────────────

		private const int GWLP_HWNDPARENT = -8;

		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
	}
}
