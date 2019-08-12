using System.ComponentModel;

namespace DiskpartGUI.Models
{
    /// <summary>
    /// Common types of File Systems
    /// </summary>
    enum FileSystem
    {
        NTFS,
        FAT32,
        exFAT,
        CDFS,
        UDF
    }

    /// <summary>
    /// The volume type
    /// </summary>
    enum VolumeType
    {
        Partition,
        Removable,
        DVDROM,
        Simple
    }

    /// <summary>
    /// The prefix of the size of a volume
    /// </summary>
    enum VolumeSizePrefix
    {
        KB,
        MB,
        GB
    }

    /// <summary>
    /// The status of a volume
    /// </summary>
    enum VolumeStatus
    {
        Blank,
        Healthy,
        NoMedia
    }

    class Volume : BaseModel
    {
        private int number;
        private char letter;
        private string label;
        private FileSystem filesystem;
        private VolumeType type;
        private int size;
        private VolumeSizePrefix prefix;
        private VolumeStatus status;
        private string info;
        private bool read;

        /// <summary>
        /// The number a volume is assigned
        /// </summary>
        public int Number
        {
            get
            {
                return number;
            }
            set
            {
                number = value;
                NotifyPropertyChanged(nameof(Number));
            }
        }

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
                NotifyPropertyChanged(nameof(Letter));
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
                NotifyPropertyChanged(nameof(Label));
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
                NotifyPropertyChanged(nameof(FileSystem));
            }
        }

        /// <summary>
        /// The type of a volume
        /// </summary>
        public VolumeType VolumeType
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
                NotifyPropertyChanged(nameof(VolumeType));
            }
        }

        /// <summary>
        /// The size of a volume
        /// </summary>
        public int Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                NotifyPropertyChanged(nameof(Size));
            }
        }

        /// <summary>
        /// The prefix to a volumes size
        /// </summary>
        public VolumeSizePrefix VolumeSizePrefix
        {
            get
            {
                return prefix;
            }
            set
            {
                prefix = value;
                NotifyPropertyChanged(nameof(VolumeSizePrefix));
            }
        }

        /// <summary>
        /// The full string size of a volume
        /// </summary>
        public string FullSize
        {
            get
            {
                return "" + Size + " " + VolumeSizePrefix;
            }
        }

        /// <summary>
        /// The status of a volume
        /// </summary>
        public VolumeStatus Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                NotifyPropertyChanged(nameof(Status));
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
                NotifyPropertyChanged(nameof(Info));
            }
        }

        /// <summary>
        /// The Read-Only flag of the a volume
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return read;
            }
            set
            {
                read = value;
                NotifyPropertyChanged(nameof(IsReadOnly));
            }
        }
    }
}
