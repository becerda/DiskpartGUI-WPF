
using DiskpartGUI.Commands;
using DiskpartGUI.Helpers;
using DiskpartGUI.Interfaces;
using DiskpartGUI.Models;
using DiskpartGUI.Processes;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;

namespace DiskpartGUI.ViewModels
{
    class RenameWindowViewModel : INotifyPropertyChanged, IClosable
    {
        private string title;
        private string newlabeltext;
        private Volume volume;
        private string textboxtext;
        private bool canapply;

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

        public string TextBoxText {
            get
            {
                return textboxtext;
            }
            set
            {
                if (value.Length > 11)
                    textboxtext = value.Substring(0, 11);
                else
                    textboxtext = value;
                UpdateCanApply();
                OnPropertyChanged(nameof(TextBoxText));
            }
        }

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

        public CommandApply ApplyCommand { get; set; }

        public CommandCancel CancelCommand { get; set; }

        public RenameWindowViewModel(ref Volume volume)
        {
            this.volume = volume;
            Title = "Rename - " + volume.ToString();
            NewLabelText = "New label for volume " + volume.ToString();

            ApplyCommand = new CommandApply(this);
            CancelCommand = new CommandCancel(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<EventArgs> RequestClose;

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void RequestWindowClose()
        {
            RequestClose?.Invoke(this, new EventArgs());
        }

        private void UpdateCanApply()
        {
            if (TextBoxText.Length > 0 && TextBoxText.Length <= 11)
            {
                Regex r = new Regex("^[a-zA-z0-9 ]*$");
                if (r.IsMatch(TextBoxText))
                {
                    CanApply = true;
                    return;
                }
            }
            CanApply = false;
        }

        public void Apply()
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

        public void Cancel()
        {
            RequestWindowClose();
        }
    }
}
