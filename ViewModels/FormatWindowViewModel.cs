using DiskpartGUI.Helpers;
using DiskpartGUI.Models;
using DiskpartGUI.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiskpartGUI.ViewModels
{
    class FormatWindowViewModel : ApplyCancelViewModel
    {

        private string title;
        private Volume volume;
        private string newlabeltext;
        private List<FileSystem> filesystemlist;
        private FileSystem fs;
        private Dictionary<FileSystem, List<string>> unitsizelist;
        private List<string> selectedunitsizelist;
        private string us;
        private string revisiontext;
        private bool comboboxunitsizeenabled;
        private bool textboxrevisionenabled;
        private Lazy<DiskpartProcess> ldpp = new Lazy<DiskpartProcess>(() => new DiskpartProcess());

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
        /// Creates a new instance of FormatWindowViewModel
        /// </summary>
        /// <param name="v">The volume to preform a format on</param>
        public FormatWindowViewModel(ref Volume v) : base()
        {
            volume = v;
            Title = "Format - " + v.ToString();
            filesystemlist = new List<FileSystem>();
            unitsizelist = new Dictionary<FileSystem, List<string>>();
            if (DiskpartProcess.GetFileSystemInfo(volume, ref filesystemlist, ref unitsizelist) != ProcessExitCode.Ok)
            {
                MessageHelper.ShowFailure("Failed to get volume file information");
                RequestWindowClose();
            }
            SelectedFileSystem = FileSystem.Default;
            OnPropertyChanged(nameof(FileSystemList));
        }

        /// <summary>
        /// Not Implemented Yet. Runs the format process on the passed in volume
        /// </summary>
        public override void Apply()
        {

        }

        /// <summary>
        /// Can the Apply button be pressed?
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override bool CanApply(object o)
        {
            if (SelectedFileSystem != FileSystem.Default)
            {
                if (RevisionText != null)
                {
                    if (RevisionText == string.Empty)
                        return true;
                    if (Regex.Match(RevisionText, @"(^[0-9]\.[0-9][0-9]$)").Success)
                        return true;
                }
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

    }
}
