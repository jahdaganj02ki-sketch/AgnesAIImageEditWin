using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using AgnesAIImageEdit.Models;
using AgnesAIImageEdit.Services;

namespace AgnesAIImageEdit.Views
{
    public partial class HistoryWindow : Window
    {
        public HistoryWindow()
        {
            InitializeComponent();
            lst.ItemsSource = HistoryStore.Load();
        }

        private void Lst_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lst.SelectedItem is HistoryItem h && File.Exists(h.OutputPath))
                Process.Start("explorer.exe", $"/select,\"{h.OutputPath}\"");
        }
    }
}
