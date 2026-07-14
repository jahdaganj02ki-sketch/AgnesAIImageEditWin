using System;
using System.IO;
using System.Windows;
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
                // MainWindow zuerst zeigen, damit SettingsWindow einen Owner hat.
                // WPF PasswordBox benötigt einen Owner-HWND, sonst kann sie bei
                // Fokus/Eingabe einen stillen Crash verursachen.
                MainWindow?.Show();

                try
                {
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
