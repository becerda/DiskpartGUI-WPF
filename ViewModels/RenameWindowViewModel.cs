
using DiskpartGUI.Commands;
using DiskpartGUI.Helpers;
using DiskpartGUI.Models;
using DiskpartGUI.Processes;
using System.Text.RegularExpressions;
using System.Windows;

namespace DiskpartGUI.ViewModels
{
    class RenameWindowViewModel : BaseViewModel
    {
        private readonly int MaxCharLen = 10;
        private string title;
        private string newlabeltext;
        private Volume volume;
        private string textboxtext;
        private bool canapply;

        /// <summary>
        /// The title of RenameWindow
        /// </summary>
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        /// <summary>
        /// Label text showing Volume's DriveLetter and Label
        /// </summary>
        public string NewLabelText
        {
            get
            {
                return newlabeltext;
            }
            set
            {
                newlabeltext = value;
                OnPropertyChanged(nameof(NewLabelText));
            }
        }

        /// <summary>
        /// The text typed in the TextBox
        /// </summary>
        public string TextBoxText {
            get
            {
                return textboxtext;
            }
            set
            {
                if (value.Length > MaxCharLen)
                    textboxtext = value.Substring(0, MaxCharLen);
                else
                    textboxtext = value;
                UpdateCanApply();
                OnPropertyChanged(nameof(TextBoxText));
            }
        }

        /// <summary>
        /// Check for ButtonApply CanExecute
        /// </summary>
        public bool CanApply
        {
            get
            {
                return canapply;
            }
            set
            {
                canapply = value;
                OnPropertyChanged(nameof(CanApply));
            }
        }

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="volume"></param>
        public RenameWindowViewModel(ref Volume volume)
        {
            this.volume = volume;
            Title = "Rename - " + volume.ToString();
            NewLabelText = "New label for volume " + volume.ToString();

            ApplyCommand = new CommandApply(this);
            CancelCommand = new CommandCancel(this);
        }

        /// <summary>
        /// Update CanApply when typing in TextBoxLabel
        /// </summary>
        private void UpdateCanApply()
        {
            if (TextBoxText.Length > 0 && TextBoxText.Length <= MaxCharLen)
            {
                Regex r = new Regex("^[a-zA-z0-9 !@#$%^&()_\\-{}]*$");
                if (r.IsMatch(TextBoxText))
                {
                    CanApply = true;
                    return;
                }
            }
            CanApply = false;
        }

        /// <summary>
        /// Action taken when ButtonApply is clicked, renaming a Volume
        /// </summary>
        public override void Apply()
        {
            if (MessageHelper.ShowConfirm("Are you sure you want to rename " + volume.ToString() + " to " + TextBoxText + "?") == MessageBoxResult.Yes)
            {
                if (LabelProcess.RenameVolume(ref volume, TextBoxText) == ProcessExitCode.Ok)
                {
                    MessageHelper.ShowSuccess("Successfully renamed " + volume.DriveLetter + "!");
                    RequestWindowClose();
                }
                else
                {
                    MessageHelper.ShowFailure("Failed to renamed " + volume.DriveLetter + "!");
                }
            }
        }

        /// <summary>
        /// Action take when ButtonCancel is clicked, closes the window
        /// </summary>
        public override void Cancel()
        {
            RequestWindowClose();
        }
    }
}
