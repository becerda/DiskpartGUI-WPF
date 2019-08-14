using System.ComponentModel;

namespace DiskpartGUI.Models
{
    /// <summary>
    /// Common types of File Systems
    /// </summary>
    enum FileSystem
    {
        None,
        NTFS,
        FAT32,
        exFAT,
        CDFS,
        UDF
    }

    static class FileSystemExtension
    {
        /// <summary>
        /// Parses a string into a FileSystem enum
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <returns>The FileSystem enum</returns>
        public static FileSystem Parse(string s)
        {
            if (s == "NTFS")
                return FileSystem.NTFS;
            if (s == "FAT32")
                return FileSystem.FAT32;
            if (s == "exFAT")
                return FileSystem.exFAT;
            if (s == "CDFS")
                return FileSystem.CDFS;
            if (s == "UDF")
                return FileSystem.UDF;
            return FileSystem.None;
        }
        
    }

    /// <summary>
    /// The volume type
    /// </summary>
    enum VolumeType
    {
        None,
        Partition,
        Removable,
        DVDROM,
        Simple
    }

    static class VolumeTypeExtension
    {
        /// <summary>
        /// Parses a string into a VolumeType enum
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <returns>The VolumeType enum</returns>
        public static VolumeType Parse(string s)
        {
            if (s == "Partition")
                return VolumeType.Partition;
            if (s == "Removable")
                return VolumeType.Removable;
            if (s == "DVD-ROM")
                return VolumeType.DVDROM;
            if (s == "Simple")
                return VolumeType.Simple;
            return VolumeType.None;
        }
    }

    /// <summary>
    /// The prefix of the size of a volume
    /// </summary>
    enum VolumeSizePostfix
    {
        None,
        KB,
        MB,
        GB
    }

    static class VolumeSizePostfixExtension
    {
        /// <summary>
        /// Parses a string to a VolumeSizePostfix enum
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <returns>The VolumeSizePostfix enum</returns>
        public static VolumeSizePostfix Parse(string s)
        {
            if (s == "K")
                return VolumeSizePostfix.KB;
            if (s == "M")
                return VolumeSizePostfix.MB;
            if (s == "G")
                return VolumeSizePostfix.GB;
            return VolumeSizePostfix.None;
        }
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

    static class VolumeStatusExtension
    {
        /// <summary>
        /// Parses a string to a VolumeStatus enum
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <returns>The VolumeStatus enum</returns>
        public static VolumeStatus Parse(string s)
        {
            if (s == "Healthy")
                return VolumeStatus.Healthy;
            if (s == "No Media")
                return VolumeStatus.NoMedia;
            return VolumeStatus.Blank;
        }
    }

    /// <summary>
    /// The mount state of a volume
    /// </summary>
    enum MountState
    {
        Mounted,
        Unmounted
    }

    static class MountStateExtension
    {
        /// <summary>
        /// Parses a string to a MountState enum
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <returns>The MountState enum</returns>
        public static MountState Parse(string s)
        {
            if (s == "Offline")
                return MountState.Unmounted;
            return MountState.Mounted;
        }
    }

    class Volume : BaseModel
    {
        private int number;
        private char letter;
        private string label;
        private FileSystem filesystem;
        private VolumeType type;
        private int size;
        private VolumeSizePostfix postfix;
        private VolumeStatus status;
        private string info;
        private bool read_only;
        private MountState mounted;

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
        public VolumeSizePostfix SizePostfix
        {
            get
            {
                return postfix;
            }
            set
            {
                postfix = value;
                NotifyPropertyChanged(nameof(SizePostfix));
            }
        }

        /// <summary>
        /// The full string size of a volume
        /// </summary>
        public string FullSize
        {
            get
            {
                return "" + Size + " " + SizePostfix;
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
                return read_only;
            }
            set
            {
                read_only = value;
                NotifyPropertyChanged(nameof(IsReadOnly));
            }
        }

        public MountState MountState
        {
            get
            {
                return mounted;
            }
            set
            {
                mounted = value;
                NotifyPropertyChanged(nameof(MountState));
            }
        }

        /// <summary>
        /// Is the volume mounted?
        /// </summary>
        /// <returns>Whether the volume is mounted</returns>
        public bool IsMounted()
        {
            return MountState == MountState.Mounted;
        }

        /// <summary>
        /// Is the volume a Removable device?
        /// </summary>
        /// <returns>Whether the volume is removable</returns>
        public bool IsRemovable()
        {
            return VolumeType == VolumeType.Removable;
        }

        /// <summary>
        /// Is the volume valid for operations?
        /// </summary>
        /// <returns>Whether the volume is valid</returns>
        public bool IsValid()
        {
            return IsMounted() && VolumeType == VolumeType.Removable && this != null;
        }

        /// <summary>
        /// Gets a string representation of a Volume
        /// </summary>
        /// <returns>The DriveLetter and Label</returns>
        public override string ToString()
        {
            return DriveLetter + " " + Label;
        }
    }
}
