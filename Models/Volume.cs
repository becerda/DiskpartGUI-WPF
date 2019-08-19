
using System.Collections;
using System.Collections.Generic;

namespace DiskpartGUI.Models
{
    /// <summary>
    /// Common types of File Systems
    /// </summary>
    public enum FileSystem
    {
        Default,
        Blank,
        NTFS,
        FAT32,
        exFAT,
        CDFS,
        UDF
    }

    public static class FileSystemExtension
    {

        private static readonly Dictionary<string, FileSystem> fs = new Dictionary<string, FileSystem>
        {
            {"Default", FileSystem.Default },
            {"", FileSystem.Blank },
            {"NTFS", FileSystem.NTFS },
            {"FAT32", FileSystem.FAT32 },
            {"exFAT", FileSystem.exFAT },
            {"CDFS", FileSystem.CDFS },
            {"UDF", FileSystem.UDF }
        };

        /// <summary>
        /// Parses a string into a FileSystem enum
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <returns>The FileSystem enum</returns>
        public static FileSystem Parse(string s)
        {
            return fs[s];
        }

    }

    /// <summary>
    /// The volume type
    /// </summary>
    public enum VolumeType
    {
        None,
        Partition,
        Removable,
        DVDROM,
        Simple
    }

    public static class VolumeTypeExtension
    {
        private static readonly Dictionary<string, VolumeType> type = new Dictionary<string, VolumeType>
        {
            {"None", VolumeType.None },
            {"Partition", VolumeType.Partition },
            {"Removable", VolumeType.Removable },
            {"DVD-ROM", VolumeType.DVDROM },
            {"Simple", VolumeType.Simple }
        };

        /// <summary>
        /// Parses a string into a VolumeType enum
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <returns>The VolumeType enum</returns>
        public static VolumeType Parse(string s)
        {
            return type[s];
        }
    }

    /// <summary>
    /// The prefix of the size of a volume
    /// </summary>
    public enum VolumeSizePostfix
    {
        None,
        KB,
        MB,
        GB
    }

    public static class VolumeSizePostfixExtension
    {
        private static readonly Dictionary<string, VolumeSizePostfix> post = new Dictionary<string, VolumeSizePostfix>
        {
            {"None", VolumeSizePostfix.None },
            {"K", VolumeSizePostfix.KB },
            {"M", VolumeSizePostfix.MB },
            {"G", VolumeSizePostfix.GB }
        };
        /// <summary>
        /// Parses a string to a VolumeSizePostfix enum
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <returns>The VolumeSizePostfix enum</returns>
        public static VolumeSizePostfix Parse(string s)
        {
            return post[s];
        }
    }

    /// <summary>
    /// The status of a volume
    /// </summary>
    public enum VolumeStatus
    {
        Blank,
        Healthy,
        NoMedia
    }

    public static class VolumeStatusExtension
    {
        private static readonly Dictionary<string, VolumeStatus> status = new Dictionary<string, VolumeStatus>
        {
            {"Blank", VolumeStatus.Blank },
            {"Healthy", VolumeStatus.Healthy },
            {"No Media", VolumeStatus.NoMedia }
        };

        /// <summary>
        /// Parses a string to a VolumeStatus enum
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <returns>The VolumeStatus enum</returns>
        public static VolumeStatus Parse(string s)
        {
            return status[s];
        }
    }

    /// <summary>
    /// The mount state of a volume
    /// </summary>
    public enum MountState
    {
        Mounted,
        Unmounted
    }

    public static class MountStateExtension
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

    /// <summary>
    /// The Read-Only flag state of a volume
    /// </summary>
    public enum ReadOnlyState
    {
        Set,
        Cleared
    }

    /// <summary>
    /// The Unit Size of a volume
    /// </summary>
    public enum UnitSize
    {
        Default,
        _512,
        _1024,
        _2048,
        _4096,
        _8192,
        _16K,
        _32K,
        _64K,
        _128K,
        _256K,
        _512K,
        _1024K,
        _2048K,
        _4096K,
        _8192K,
        _16384K,
        _32768K
    }

    static class UnitSizeExtension
    {
        private static readonly Dictionary<string, UnitSize> sizes = new Dictionary<string, UnitSize>
            {
                {"512", UnitSize._512 },
                { "1024", UnitSize._1024 },
                { "2048", UnitSize._2048 },
                { "4096", UnitSize._4096 },
                { "8192", UnitSize._8192 },
                { "16K", UnitSize._16K },
                { "32K", UnitSize._32K },
                { "64K", UnitSize._64K },
                { "128K", UnitSize._128K },
                { "256K", UnitSize._256K },
                { "512K", UnitSize._512K },
                { "1024K", UnitSize._1024K },
                { "2048K", UnitSize._2048K },
                { "4096K", UnitSize._4096K },
                { "8192K", UnitSize._8192K },
                { "16384K", UnitSize._16384K },
                { "32768K", UnitSize._32768K },
                { "Default", UnitSize.Default }
            };

        /// <summary>
        /// Convert UnitSize enum to string
        /// </summary>
        /// <param name="size">The enum to convert</param>
        /// <returns>The string representation</returns>
        public static string ToString(UnitSize size)
        {
            return size.ToString().Substring(1);
        }

        /// <summary>
        /// Convert string to UnitSize enum
        /// </summary>
        /// <param name="size">The string to convert</param>
        /// <returns>The enum representation</returns>
        public static UnitSize Parse(string size)
        {
            return sizes[size];
        }
    }

    public class Volume : BaseModel
    {
        /// <summary>
        /// The maximum number of characters a label can be
        /// </summary>
        public static readonly int Max_Label_Char_Len = 10;

        private int number;
        private char letter;
        private string label;
        private FileSystem filesystem;
        private VolumeType type;
        private int size;
        private VolumeSizePostfix postfix;
        private VolumeStatus status;
        private string info;
        private ReadOnlyState read_only;
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
        public ReadOnlyState ReadOnlyState
        {
            get
            {
                return read_only;
            }
            set
            {
                read_only = value;
                NotifyPropertyChanged(nameof(ReadOnlyState));
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
        /// Is the volume's Read-Only flag set?
        /// </summary>
        /// <returns></returns>
        public bool IsReadOnly()
        {
            return ReadOnlyState == ReadOnlyState.Set;
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
