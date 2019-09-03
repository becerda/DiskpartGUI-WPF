
namespace DiskpartGUI.Models
{
    public class Volume : BaseMedia
    {
        /// <summary>
        /// The maximum number of characters a label can be
        /// </summary>
        public const int Max_Label_Char_Len = 10;

        private char letter;
        private string label;
        private FileSystem filesystem;
        private MediaType type;
        private string info;
        private MountState mounted;

        /// <summary>
        /// The dirve letter a volume is assigned
        /// </summary>
        public char Letter
        {
            get
            {
                return letter;
            }
            set
            {
                letter = value;
                OnPropertyChanged(nameof(Letter));
            }
        }

        /// <summary>
        /// The drive letter with ':' included
        /// </summary>
        public string DriveLetter
        {
            get
            {
                return Letter + ":";
            }
        }

        /// <summary>
        /// The label of the volume
        /// </summary>
        public string Label
        {
            get
            {
                return label;
            }
            set
            {
                label = value;
                OnPropertyChanged(nameof(Label));
            }
        }

        /// <summary>
        /// The file system used by a volume
        /// </summary>
        public FileSystem FileSystem
        {
            get
            {
                return filesystem;
            }
            set
            {
                filesystem = value;
                OnPropertyChanged(nameof(FileSystem));
            }
        }

        /// <summary>
        /// The type of a volume
        /// </summary>
        public MediaType MediaType
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
                OnPropertyChanged(nameof(MediaType));
            }
        }

        /// <summary>
        /// The information about a volume
        /// </summary>
        public string Info
        {
            get
            {
                return info;
            }
            set
            {
                info = value;
                OnPropertyChanged(nameof(Info));
            }
        }

        /// <summary>
        /// The mount state of a volume
        /// </summary>
        public MountState MountState
        {
            get
            {
                return mounted;
            }
            set
            {
                mounted = value;
                OnPropertyChanged(nameof(MountState));
            }
        }

        /// <summary>
        /// Is the volume mounted?
        /// </summary>
        /// <returns>Whether the volume is mounted</returns>
        public override bool IsMounted()
        {
            return MountState == MountState.Mounted;
        }

        /// <summary>
        /// Is the volume a Removable device?
        /// </summary>
        /// <returns>Whether the volume is removable</returns>
        public override bool IsRemovable()
        {
            return MediaType == MediaType.Removable;
        }

        /// <summary>
        /// Can a Volume be set to read only?
        /// </summary>
        /// <returns></returns>
        public override bool CanToggleReadOnly()
        {
            if (Info.Contains("Pagefile"))
                return false;
            if (Info.Contains("System"))
                return false;
            if (Info.Contains("Boot"))
                return false;
            return true;
        }

        /// <summary>
        /// A Volume can be renamed
        /// </summary>
        /// <returns></returns>
        public override bool CanBeRenamed()
        {
            if (MediaType == MediaType.Removable)
                return true;
            return false;
        }

        /// <summary>
        /// Can this media item be ejected?
        /// </summary>
        /// <returns></returns>
        public override bool CanBeEjected()
        {
            if (MediaType == MediaType.Removable)
                return true;
            return false;
        }

        /// <summary>
        /// Can this media item be formated
        /// </summary>
        /// <returns></returns>
        public override bool CanBeFormated()
        {
            if (Info.Contains("Pagefile"))
                return false;
            if (Info.Contains("System"))
                return false;
            if (Info.Contains("Boot"))
                return false;
            return true;
        }

        /// <summary>
        /// Gets a string representation of a Volume
        /// </summary>
        /// <returns>The DriveLetter and Label</returns>
        public override string ToString()
        {
            return "Volume " + DriveLetter + " " + Label;
        }
    }
}
