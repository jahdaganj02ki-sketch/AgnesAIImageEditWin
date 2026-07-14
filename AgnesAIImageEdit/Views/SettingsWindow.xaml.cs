using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using AgnesAIImageEdit.Models;
using AgnesAIImageEdit.Services;

namespace AgnesAIImageEdit.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            LoadFromSettings();
        }

        private void LoadFromSettings()
        {
            var s = AppSettings.Current;
            txtBaseUrl.Text = s.ApiBaseUrl;
            cmbModel.Text = s.DefaultImageModel;
            chkEnhance.IsChecked = s.EnhancePrompt;
            chkSoftware.IsChecked = s.AlwaysSoftwareRender;
            cmbTier.Text = s.OutputTier;
            cmbRatio.Text = s.OutputRatio;
            cmbSizeExact.Text = s.OutputSizeExact;
            txtSaveFolder.Text = s.SaveFolder;
        }

        private void GetKey_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://apihub.agnes-ai.com/") { UseShellExecute = true });
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFolderDialog { Title = "Select save folder" };
            if (dlg.ShowDialog() == true)
                txtSaveFolder.Text = dlg.FolderName;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var s = AppSettings.Current;
            s.ApiBaseUrl = txtBaseUrl.Text.Trim();
            s.DefaultImageModel = cmbModel.Text;
            s.EnhancePrompt = chkEnhance.IsChecked == true;
            s.AlwaysSoftwareRender = chkSoftware.IsChecked == true;
            s.OutputTier = cmbTier.Text;
            s.OutputRatio = cmbRatio.Text;
            s.OutputSizeExact = cmbSizeExact.Text;
            s.SaveFolder = txtSaveFolder.Text.Trim();
            s.Save();

            if (!string.IsNullOrEmpty(pwdKey.Password))
                KeyVault.SaveKey(pwdKey.Password);

            DialogResult = true;
            Close();
        }
    }
}
