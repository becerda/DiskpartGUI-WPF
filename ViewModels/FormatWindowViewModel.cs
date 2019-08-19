using DiskpartGUI.Helpers;
using DiskpartGUI.Models;
using DiskpartGUI.Processes;
using DiskpartGUI.Views;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace DiskpartGUI.ViewModels
{
    class FormatWindowViewModel : ApplyCancelViewModel
    {

        private string title;
        private readonly Volume volume;
        private string newlabeltext;
        private List<FileSystem> filesystemlist;
        private FileSystem fs;
        private Dictionary<FileSystem, List<string>> unitsizelist;
        private List<string> selectedunitsizelist;
        private string us;
        private string revisiontext;
        private bool quick;
        private bool comp;
        private bool over;
        private bool dup;
        private bool comboboxunitsizeenabled;
        private bool textboxrevisionenabled;
        private Lazy<DiskpartProcess> ldpp = new Lazy<DiskpartProcess>(() => new DiskpartProcess());
        private FormatArguments fa;

        /// <summary>
        /// The title of the window
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
        /// The label of the volume after formating
        /// </summary>
        public string NewLabelText
        {
            get
            {
                return newlabeltext;
            }
            set
            {
                if (value.Length > Volume.Max_Label_Char_Len)
                    newlabeltext = value.Substring(0, Volume.Max_Label_Char_Len);
                else
                    newlabeltext = value;
                OnPropertyChanged(nameof(NewLabelText));
            }
        }

        /// <summary>
        /// The list of file systems the volume can use
        /// </summary>
        public List<FileSystem> FileSystemList
        {
            get
            {
                return filesystemlist;
            }
            set
            {
                filesystemlist = value;
                OnPropertyChanged(nameof(FileSystemList));
            }
        }

        /// <summary>
        /// The selected file system to format to
        /// </summary>
        public FileSystem SelectedFileSystem
        {
            get
            {
                return fs;
            }
            set
            {
                fs = value;
                if (value == FileSystem.Default)
                {
                    ComboBoxUnitSizeIsEnabled = false;
                    TextBoxRevisionIsEnabled = false;
                }
                else
                {
                    ComboBoxUnitSizeIsEnabled = true;
                    TextBoxRevisionIsEnabled = true;
                    SelectedUnitSizeList = unitsizelist[value];

                }

                RevisionText = string.Empty;
                SelectedUnitSize = unitsizelist[value][0];
                OnPropertyChanged(nameof(SelectedFileSystem));
            }
        }

        /// <summary>
        /// The list of unit sizes that can work with the selected file system
        /// </summary>
        public List<string> SelectedUnitSizeList
        {
            get
            {
                return selectedunitsizelist;
            }
            set
            {
                selectedunitsizelist = value;
                OnPropertyChanged(nameof(SelectedUnitSizeList));
            }
        }

        /// <summary>
        /// The selected unit size to format to
        /// </summary>
        public string SelectedUnitSize
        {
            get
            {
                return us;
            }
            set
            {
                us = value;
                OnPropertyChanged(nameof(SelectedUnitSize));
            }
        }

        /// <summary>
        /// The Revision of the file system to be used
        /// </summary>
        public string RevisionText
        {
            get
            {
                return revisiontext;
            }
            set
            {
                if (value.Length == 0 || (Regex.Match(value.ToCharArray()[value.Length - 1].ToString(), @"[0-9]|\.").Success))
                {
                    revisiontext = value;
                    OnPropertyChanged(nameof(RevisionText));
                }
            }
        }

        /// <summary>
        /// The Quick format option
        /// </summary>
        public bool QuickFormat
        {
            get
            {
                return quick;
            }
            set
            {
                quick = value;
                OnPropertyChanged(nameof(QuickFormat));
            }
        }

        /// <summary>
        /// The Compress format option
        /// </summary>
        public bool Compress
        {
            get
            {
                return comp;
            }
            set
            {
                comp = value;
                OnPropertyChanged(nameof(Compress));
            }
        }

        /// <summary>
        /// The Override format option
        /// </summary>
        public bool Override
        {
            get
            {
                return over;
            }
            set
            {
                over = value;
                OnPropertyChanged(nameof(Override));
            }
        }

        /// <summary>
        /// The Duplicate format option
        /// </summary>
        public bool Duplicate
        {
            get
            {
                return dup;
            }
            set
            {
                dup = value;
                OnPropertyChanged(nameof(Duplicate));
            }
        }

        /// <summary>
        /// Enable status of the UnitSizeComboBox
        /// </summary>
        public bool ComboBoxUnitSizeIsEnabled
        {
            get
            {
                return comboboxunitsizeenabled;
            }
            set
            {
                comboboxunitsizeenabled = value;
                OnPropertyChanged(nameof(ComboBoxUnitSizeIsEnabled));
            }
        }

        /// <summary>
        /// EnableStatus of the TextBoxRevision
        /// </summary>
        public bool TextBoxRevisionIsEnabled
        {
            get
            {
                return textboxrevisionenabled;
            }
            set
            {
                textboxrevisionenabled = value;
                OnPropertyChanged(nameof(TextBoxRevisionIsEnabled));
            }
        }

        /// <summary>
        /// The diskpart process
        /// </summary>
        private DiskpartProcess DiskpartProcess
        {
            get
            {
                return ldpp.Value;
            }
        }

        /// <summary>
        /// Creates a new instance of FormatWindowViewModel
        /// </summary>
        /// <param name="v">The volume to preform a format on</param>
        public FormatWindowViewModel(ref Volume v) : base()
        {
            volume = v;
            Title = "Format - " + v.ToString();
            filesystemlist = new List<FileSystem>();
            unitsizelist = new Dictionary<FileSystem, List<string>>();
            QuickFormat = true;
            if (DiskpartProcess.GetFileSystemInfo(volume, ref filesystemlist, ref unitsizelist) != ProcessExitCode.Ok)
            {
                MessageHelper.ShowFailure("Failed to get volume file information");
                RequestWindowClose();
            }
            SelectedFileSystem = FileSystem.Default;
            OnPropertyChanged(nameof(FileSystemList));
        }

        /// <summary>
        /// Runs the format process on the passed in volume
        /// </summary>
        public async override void Apply()
        {
            if (MessageHelper.ShowConfirm("Are you sure you want to format " + volume.ToString() + "?") == MessageBoxResult.Yes)
            {
                applymasterenabled = false;
                WaitWindow window = new WaitWindow();
                WaitWindowViewModel wwvm = new WaitWindowViewModel("Formating, please wait...");
                window.DataContext = wwvm;
                window.Show();
                ProcessExitCode result = await CallFormatProcess();
                window.Close();

                if (result == ProcessExitCode.Ok)
                {
                    MessageHelper.ShowSuccess("Successfully formated " + volume.DriveLetter + "!");
                    RequestWindowClose();
                }
                else
                {
                    MessageHelper.ShowFailure("Failed to format " + volume.DriveLetter + "!");
                }
            }
        }

        /// <summary>
        /// Can the Apply button be pressed?
        /// </summary>
        /// <param name="o">Not used</param>
        /// <returns></returns>
        public override bool CanApply(object o)
        {
            if (base.CanApply(o))
            {
                if (RevisionText == null || RevisionText == string.Empty)
                    return true;

                if (Regex.Match(RevisionText, @"(^[0-9]\.[0-9][0-9]$)").Success)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Closes the format window
        /// </summary>
        public override void Cancel()
        {
            RequestWindowClose();
        }

        /// <summary>
        /// Async task to format in background
        /// </summary>
        /// <returns>The ProcessExitCode of Format</returns>
        private async Task<ProcessExitCode> CallFormatProcess()
        {
            fa = new FormatArguments(SelectedFileSystem, RevisionText, NewLabelText, UnitSizeExtension.Parse(SelectedUnitSize), QuickFormat, Compress, Override, Duplicate);
            var format = Task.Run(() => DiskpartProcess.Format(volume, fa));
            ProcessExitCode result = await format;
            return result;
        }

    }
}
