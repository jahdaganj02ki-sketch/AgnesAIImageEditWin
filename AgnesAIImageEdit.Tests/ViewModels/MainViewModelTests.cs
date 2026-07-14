using System.ComponentModel;
using System.IO;
using AgnesAIImageEdit.Models;
using AgnesAIImageEdit.ViewModels;
using Xunit;

namespace AgnesAIImageEdit.Tests.ViewModels
{
    public class MainViewModelTests
    {
        [Fact]
        public void Constructor_InitializesProperties()
        {
            var vm = new MainViewModel();

            Assert.NotNull(vm.Messages);
            Assert.Empty(vm.Messages);
            Assert.Equal("", vm.CurrentPrompt);
            Assert.Null(vm.SelectedImage);
            Assert.Equal("", vm.SelectedImagePath);
            Assert.Equal("agnes-image-2.1-flash", vm.SelectedModel);
            Assert.Equal(new[] { "agnes-image-2.1-flash", "agnes-image-2.0-flash" }, vm.AvailableModels);
            Assert.True(vm.IsTextToImage);
            Assert.False(vm.IsEditMode);
            Assert.Equal("", vm.StatusText);
            Assert.True(vm.Enhance); // Defaults to true from AppSettings
        }

        [Fact]
        public void CurrentPrompt_Set_RaisesPropertyChanged()
        {
            var vm = new MainViewModel();
            bool raised = false;
            vm.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(vm.CurrentPrompt)) raised = true; };

            vm.CurrentPrompt = "test prompt";

            Assert.True(raised);
            Assert.Equal("test prompt", vm.CurrentPrompt);
        }

        [Fact]
        public void SelectedImage_Set_RaisesPropertyChanged()
        {
            var vm = new MainViewModel();
            bool raised = false;
            vm.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(vm.SelectedImage)) raised = true; };

            vm.SelectedImage = new System.Windows.Media.Imaging.BitmapImage();

            Assert.True(raised);
            Assert.NotNull(vm.SelectedImage);
        }

        [Fact]
        public void IsTextToImage_Toggle_UpdatesIsEditModeAndModeButtonLabel()
        {
            var vm = new MainViewModel();

            Assert.True(vm.IsTextToImage);
            Assert.False(vm.IsEditMode);
            Assert.Equal("Edit Image", vm.ModeButtonLabel);

            vm.IsTextToImage = false;

            Assert.False(vm.IsTextToImage);
            Assert.True(vm.IsEditMode);
            Assert.Equal("Text-to-Image", vm.ModeButtonLabel);

            vm.IsTextToImage = true;

            Assert.True(vm.IsTextToImage);
            Assert.False(vm.IsEditMode);
            Assert.Equal("Edit Image", vm.ModeButtonLabel);
        }

        [Fact]
        public void ToggleModeCommand_SwitchesMode()
        {
            var vm = new MainViewModel();

            vm.ToggleModeCommand.Execute(null);

            Assert.False(vm.IsTextToImage);
            Assert.True(vm.IsEditMode);

            vm.ToggleModeCommand.Execute(null);

            Assert.True(vm.IsTextToImage);
            Assert.False(vm.IsEditMode);
        }

        [Fact]
        public void ContinueEditing_WithNullOutputImage_DoesNothing()
        {
            var vm = new MainViewModel();
            var item = new ResultItem { OutputImage = null };

            vm.ContinueEditing(item);

            Assert.Equal("", vm.SelectedImagePath);
            Assert.Null(vm.SelectedImage);
            Assert.True(vm.IsTextToImage);
        }

        [Fact]
        public void CleanupTempFiles_DeletesExistingTempFile()
        {
            var vm = new MainViewModel();
            var tempFile = Path.Combine(Path.GetTempPath(), $"test_cleanup_{Guid.NewGuid()}.png");
            File.WriteAllText(tempFile, "test");

            var field = typeof(MainViewModel).GetField("_lastTempEditFile", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field!.SetValue(vm, tempFile);

            vm.CleanupTempFiles();

            Assert.False(File.Exists(tempFile));
        }
    }
}