using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using AgnesAIImageEdit.Models;
using AgnesAIImageEdit.ViewModels;
using AgnesAIImageEdit.Views;

namespace AgnesAIImageEdit.Views
{
    public partial class ResultCard : UserControl
    {
        public ResultCard()
        {
            InitializeComponent();
        }

        private MainViewModel VM => (MainViewModel)((MainWindow)Application.Current.MainWindow).DataContext;

        private void Up_Click(object sender, RoutedEventArgs e) => VM.Rate((ResultItem)DataContext, 1);
        private void Down_Click(object sender, RoutedEventArgs e) => VM.Rate((ResultItem)DataContext, -1);
        private void Copy_Click(object sender, RoutedEventArgs e) => VM.CopyResult((ResultItem)DataContext);
        private void Share_Click(object sender, RoutedEventArgs e) => VM.ShareResult((ResultItem)DataContext);
        private void Save_Click(object sender, RoutedEventArgs e) => VM.SaveResult((ResultItem)DataContext);

        private void ContinueEdit_Click(object sender, RoutedEventArgs e)
        {
            var item = (ResultItem)DataContext;
            if (item.OutputImage != null)
            {
                VM.ContinueEditing(item);
            }
        }

        private void FullScreen_Click(object sender, RoutedEventArgs e)
        {
            var item = (ResultItem)DataContext;
            if (item.OutputImage != null)
            {
                var window = new FullScreenWindow(item.OutputImage);
                window.Owner = Application.Current.MainWindow;
                window.ShowDialog();
            }
        }
    }
}