
using DiskpartGUI.Commands;
using DiskpartGUI.Helpers;
using DiskpartGUI.Models;
using DiskpartGUI.Processes;
using DiskpartGUI.Views;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace DiskpartGUI.ViewModels
{
    class RenameWindowViewModel : ApplyCancelViewModel
    {

        private string title;
        private string newlabeltext;
        private Volume volume;
        private string textboxtext;

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
        public string TextBoxText
        {
            get
            {
                return textboxtext;
            }
            set
            {
                if (value.Length > Volume.Max_Label_Char_Len)
                    textboxtext = value.Substring(0, Volume.Max_Label_Char_Len);
                else
                    textboxtext = value;
                OnPropertyChanged(nameof(TextBoxText));
            }
        }

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="volume"></param>
        public RenameWindowViewModel(ref Volume volume) : base()
        {
            this.volume = volume;
            Title = "Rename - " + volume.ToString();
            NewLabelText = "New label for volume " + volume.ToString();

        }

        /// <summary>
        /// Action taken when ButtonApply is clicked, renaming a Volume
        /// </summary>
        public async override void Apply()
        {
            if (MessageHelper.ShowConfirm("Are you sure you want to rename " + volume.ToString() + " to " + TextBoxText.ToUpper() + "?") == MessageBoxResult.Yes)
            {
                WaitWindow window = new WaitWindow
                {
                    DataContext = new WaitWindowViewModel("Renaming")
                };
                window.Show();
                ProcessExitCode result = await CallLabelProcess();
                window.Close();

                if (result == ProcessExitCode.Ok)
                {
                    MessageHelper.ShowSuccess("Successfully renamed " + volume.DriveLetter + "!");
                    ExitStatus = ExitStatus.Applied;
                    RequestWindowClose();
                }
                else
                {
                    MessageHelper.ShowFailure("Failed to renamed " + volume.DriveLetter + "!");
                }
            }
        }

        /// <summary>
        /// Is the Apply button enabled?
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override bool CanApply(object o)
        {
            if (base.CanApply(o))
            {
                if (TextBoxText == null)
                    return false;

                if (TextBoxText.Length > 0 && TextBoxText.Length <= Volume.Max_Label_Char_Len)
                {
                    Regex r = new Regex("^[a-zA-z0-9 !@#$%^&()_\\-{}]*$");
                    if (r.IsMatch(TextBoxText))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Async Task to call LabelProcess.RenameVolume
        /// </summary>
        /// <returns></returns>
        private async Task<ProcessExitCode> CallLabelProcess()
        {
            var rename = Task.Run(() => LabelProcess.RenameVolume(ref volume, TextBoxText));
            ProcessExitCode result = await rename;
            return result;
        }

    }
}
