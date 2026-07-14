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

        protected override void OnStartup(StartupEventArgs e)
        {
            Directory.CreateDirectory(DataDir);
            Directory.CreateDirectory(Path.Combine(DataDir, "Outputs"));

            AppSettings.Current = AppSettings.Load();
            GpuProbe.ApplyRenderingMode();

            base.OnStartup(e);

            if (!KeyVault.HasKey())
            {
                var w = new SettingsWindow { WindowStartupLocation = WindowStartupLocation.CenterScreen };
                w.ShowDialog();
            }
        }
    }
}
