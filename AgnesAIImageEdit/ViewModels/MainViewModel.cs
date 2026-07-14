using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using AgnesAIImageEdit.Models;
using AgnesAIImageEdit.Services;
using AgnesAIImageEdit.Views;
using AgnesAIImageEdit.Resources.Themes;

namespace AgnesAIImageEdit.ViewModels
{
	public class MainViewModel : INotifyPropertyChanged
	{
		public ObservableCollection<IChatItem> Messages { get; } = new();

		private string _currentPrompt = "";
		public string CurrentPrompt
		{
			get => _currentPrompt;
			set { _currentPrompt = value; OnProp(nameof(CurrentPrompt)); }
		}

		private System.Windows.Media.Imaging.BitmapImage? _selectedImage;
		public System.Windows.Media.Imaging.BitmapImage? SelectedImage
		{
			get => _selectedImage;
			set { _selectedImage = value; OnProp(nameof(SelectedImage)); }
		}

		private string _selectedImagePath = "";
		public string SelectedImagePath
		{
			get => _selectedImagePath;
			set { _selectedImagePath = value; OnProp(nameof(SelectedImagePath)); }
		}

		private string _selectedModel = AppSettings.Current.DefaultImageModel;
		public string SelectedModel
		{
			get => _selectedModel;
			set { _selectedModel = value; OnProp(nameof(SelectedModel)); }
		}

		public string[] AvailableModels { get; } = { AppSettings.ModelImage21, AppSettings.ModelImage20 };

		private bool _isTextToImage = true;
		public bool IsTextToImage
		{
			get => _isTextToImage;
			set
			{
				_isTextToImage = value;
				OnProp(nameof(IsTextToImage));
				OnProp(nameof(IsEditMode));
				OnProp(nameof(ModeButtonLabel));
			}
		}

		public bool IsEditMode => !IsTextToImage;
		public string ModeButtonLabel => IsTextToImage ? "Edit Image" : "Text-to-Image";

		private bool _isBusy;
		public bool IsBusy
		{
			get => _isBusy;
			set { _isBusy = value; OnProp(nameof(IsBusy)); }
		}

		private string _statusText = "";
		public string StatusText
		{
			get => _statusText;
			set { _statusText = value; OnProp(nameof(StatusText)); }
		}

		private bool _enhance;
		public bool Enhance
		{
			get => _enhance;
			set { _enhance = value; OnProp(nameof(Enhance)); }
		}

		public string ThemeButtonLabel => AppSettings.Current.IsDarkMode ? "Light Mode" : "Dark Mode";

		public RelayCommand EditImageCommand { get; }
		public RelayCommand RemoveImageCommand { get; }
		public RelayCommand SubmitCommand { get; }
		public RelayCommand OpenSettingsCommand { get; }
		public RelayCommand OpenHistoryCommand { get; }
		public RelayCommand ToggleModeCommand { get; }
		public RelayCommand ToggleThemeCommand { get; }

		public MainViewModel()
		{
			Enhance = AppSettings.Current.EnhancePrompt;
			SelectedModel = AppSettings.Current.DefaultImageModel;

			EditImageCommand = new RelayCommand(_ => PickImage());
			RemoveImageCommand = new RelayCommand(_ => ClearImage());
			SubmitCommand = new RelayCommand(async _ => await SubmitAsync());
			OpenSettingsCommand = new RelayCommand(_ => OpenSettings());
			OpenHistoryCommand = new RelayCommand(_ => OpenHistory());
			ToggleModeCommand = new RelayCommand(_ => IsTextToImage = !IsTextToImage);
			ToggleThemeCommand = new RelayCommand(_ => ToggleTheme());
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		private void OnProp(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

		private void PickImage()
		{
			var dlg = new Microsoft.Win32.OpenFileDialog
			{
				Filter = "Images|*.png;*.jpg;*.jpeg;*.webp;*.bmp",
				Title = "Select image to edit"
			};
			if (dlg.ShowDialog() == true)
			{
				SelectedImagePath = dlg.FileName;
				SelectedImage = ImageHelper.LoadBitmapImage(dlg.FileName);
			}
		}

		private void ClearImage()
		{
			SelectedImage = null;
			SelectedImagePath = "";
		}

		private (string size, string? ratio) ResolveSize()
		{
			if (SelectedModel == AppSettings.ModelImage21)
				return (AppSettings.Current.OutputTier, AppSettings.Current.OutputRatio);
			return (AppSettings.Current.OutputSizeExact, null);
		}

		private async Task SubmitAsync()
		{
			if (IsBusy) return;
			IsBusy = true;
			try
			{
				if (!KeyVault.HasKey())
				{
					StatusText = "No API key set. Open Settings to add one.";
					OpenSettings();
					return;
				}

				var prompt = CurrentPrompt.Trim();
				if (string.IsNullOrEmpty(prompt)) return;

				Messages.Add(new UserPromptItem { Prompt = prompt, PreviewImage = IsTextToImage ? null : SelectedImage });

				string finalPrompt = prompt;

				if (Enhance && !IsTextToImage)
				{
					Messages.Add(new StatusItem { Text = "Enhancing prompt…" });
					var (enh, err) = await PromptEnhancer.EnhanceAsync(prompt);
					if (err == null && !string.IsNullOrWhiteSpace(enh))
					{
						finalPrompt = enh.Trim();
						Messages.Add(new StatusItem { Text = "Enhanced: " + finalPrompt });
					}
					else if (err != null)
					{
						Messages.Add(new StatusItem { Text = $"Enhance failed ({err}); using original prompt." });
					}
				}

				Messages.Add(new StatusItem { Text = "Preparing model… this may take 30–60 seconds." });

				List<string>? images = null;
				if (!IsTextToImage && !string.IsNullOrEmpty(SelectedImagePath))
				{
					var dataUri = ImageHelper.FileToDataUri(SelectedImagePath, 2048);
					images = new List<string> { dataUri };
				}

				var (size, ratio) = ResolveSize();
				var (bytes, revised, error) = await AgnesClient.GenerateImageAsync(SelectedModel, finalPrompt, images, size, ratio, "b64_json");

				if (error != null)
				{
					Messages.Add(new StatusItem { Text = "Error: " + error });
					return;
				}

				var bmp = ImageHelper.BytesToBitmap(bytes!);
				var outDir = AppSettings.Current.ResolveSaveFolder();
				Directory.CreateDirectory(outDir);
				var fileName = $"agnes_{DateTime.Now:yyyyMMdd_HHmmss}.png";
				var outPath = Path.Combine(outDir, fileName);
				File.WriteAllBytes(outPath, bytes!);

				Messages.Add(new ResultItem
				{
					OutputImage = bmp,
					Prompt = finalPrompt,
					Model = SelectedModel,
					OutputPath = outPath,
					RevisedPrompt = revised
				});

				var hist = new HistoryItem
				{
					Mode = IsTextToImage ? "text" : "edit",
					Model = SelectedModel,
					Prompt = finalPrompt,
					OutputPath = outPath
				};
				if (!IsTextToImage && !string.IsNullOrEmpty(SelectedImagePath) && File.Exists(SelectedImagePath))
					hist.InputThumbBase64 = ImageHelper.MakeThumbBase64(File.ReadAllBytes(SelectedImagePath));
				HistoryStore.Add(hist);

				CurrentPrompt = "";
				ClearImage();
			}
			finally
			{
				IsBusy = false;
			}
		}

		private void OpenSettings()
		{
			var w = new SettingsWindow { Owner = Application.Current.MainWindow, WindowStartupLocation = WindowStartupLocation.CenterOwner };
			if (w.ShowDialog() == true)
			{
				AppSettings.Current = AppSettings.Load();
				SelectedModel = AppSettings.Current.DefaultImageModel;
				Enhance = AppSettings.Current.EnhancePrompt;
				OnProp(nameof(ThemeButtonLabel));
			}
		}

		private void OpenHistory()
		{
			var w = new HistoryWindow { Owner = Application.Current.MainWindow, WindowStartupLocation = WindowStartupLocation.CenterOwner };
			w.ShowDialog();
		}

		private void ToggleTheme()
		{
			AppSettings.Current.IsDarkMode = !AppSettings.Current.IsDarkMode;
			AppSettings.Current.Save();
			ThemeManager.ApplyTheme(Application.Current, AppSettings.Current.IsDarkMode);
			OnProp(nameof(ThemeButtonLabel));
		}

		public void ApplyThemeAtStartup(Application app)
		{
			ThemeManager.ApplyTheme(app, AppSettings.Current.IsDarkMode);
			OnProp(nameof(ThemeButtonLabel));
		}
	}
}
