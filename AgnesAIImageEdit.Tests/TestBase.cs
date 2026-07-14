using System.ComponentModel;
using System.Windows;
using AgnesAIImageEdit.Resources.Themes;
using AgnesAIImageEdit.ViewModels;
using Xunit;

namespace AgnesAIImageEdit.Tests
{
    public class TestBase
    {
        static TestBase()
        {
            // Initialize WPF application for tests
            if (Application.Current == null)
            {
                var app = new Application();
                app.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/AgnesAIImageEdit;component/Resources/Styles.xaml", UriKind.Absolute) });
                app.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/AgnesAIImageEdit;component/Resources/Themes/Light.xaml", UriKind.Absolute) });
            }
        }

        public static MainViewModel CreateViewModel()
        {
            var vm = new MainViewModel();
            return vm;
        }

        public static void AssertPropertyChanged(INotifyPropertyChanged vm, string propertyName, Action act)
        {
            bool raised = false;
            vm.PropertyChanged += (s, e) => { if (e.PropertyName == propertyName) raised = true; };
            act();
            if (!raised)
                throw new Xunit.Sdk.XunitException($"PropertyChanged not raised for {propertyName}");
        }
    }
}