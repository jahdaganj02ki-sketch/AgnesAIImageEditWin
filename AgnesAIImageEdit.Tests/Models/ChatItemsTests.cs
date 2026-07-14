using System;
using System.Windows.Media.Imaging;
using AgnesAIImageEdit.Models;
using AgnesAIImageEdit.ViewModels;
using FluentAssertions;
using Xunit;

namespace AgnesAIImageEdit.Tests.Models
{
    public class ChatItemsTests
    {
        [Fact]
        public void UserPromptItem_PropertiesWork()
        {
            var item = new UserPromptItem
            {
                Prompt = "test prompt",
                PreviewImage = null
            };

            item.Prompt.Should().Be("test prompt");
            item.PreviewImage.Should().BeNull();
            item.HasImage.Should().BeFalse();
        }

        [Fact]
        public void UserPromptItem_HasImage_NullImageReturnsFalse()
        {
            var item = new UserPromptItem { Prompt = "test", PreviewImage = null };
            item.HasImage.Should().BeFalse();
        }

        [Fact]
        public void StatusItem_TextPropertyWorks()
        {
            var item = new StatusItem { Text = "status text" };
            item.Text.Should().Be("status text");
        }

        [Fact]
        public void ResultItem_PropertiesWork()
        {
            var item = new ResultItem
            {
                OutputImage = null,
                Prompt = "test prompt",
                Model = "agnes-image-2.1-flash",
                OutputPath = "/path/output.png",
                RevisedPrompt = "revised prompt",
                Rating = 1
            };

            item.OutputImage.Should().BeNull();
            item.Prompt.Should().Be("test prompt");
            item.Model.Should().Be("agnes-image-2.1-flash");
            item.OutputPath.Should().Be("/path/output.png");
            item.RevisedPrompt.Should().Be("revised prompt");
            item.Rating.Should().Be(1);
        }

        [Fact]
        public void ResultItem_Rating_ToggleBehavior_ThroughRateMethod()
        {
            var vm = new MainViewModel();
            var item = new ResultItem { Rating = null };

            // First up-vote
            vm.Rate(item, 1);
            item.Rating.Should().Be(1);

            // Same vote again should clear
            vm.Rate(item, 1);
            item.Rating.Should().BeNull();

            // Down-vote
            vm.Rate(item, -1);
            item.Rating.Should().Be(-1);

            // Same down-vote should clear
            vm.Rate(item, -1);
            item.Rating.Should().BeNull();
        }
    }
}