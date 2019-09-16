using System.Collections.Generic;

namespace DiskpartGUI.Models
{

    /// <summary>
    /// The volume type
    /// </summary>
    public enum VolumeType
    {
        None,
        Partition,
        Removable,
        DVDROM,
    }

    public static class VolumeTypeExtension
    {
        private static readonly Dictionary<string, VolumeType> type = new Dictionary<string, VolumeType>
        {
            {"None", VolumeType.None },
            {"Partition", VolumeType.Partition },
            {"Removable", VolumeType.Removable },
            {"DVD-ROM", VolumeType.DVDROM }
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
        UDF,
        RAW
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
            {"UDF", FileSystem.UDF },
            {FileSystem.RAW + "", FileSystem.RAW }
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

    public class Volume : BaseMedia
    {
        /// <summary>
        /// The maximum number of characters a label can be
        /// </summary>
        public const int Max_Label_Char_Len = 10;

        private char letter;
        private FileSystem filesystem;
        private VolumeType type;
        private string info;
        private MountState mounted;

        private int capacity;
        private SizePostfix capacitypostfix;
        private int freespace;
        private SizePostfix freespacepostfix;

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
                OnPropertyChanged(nameof(DriveLetter));
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
        public VolumeType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
                OnPropertyChanged(nameof(Type));
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
        /// The capacity of a volume
        /// </summary>
        public int Capacity
        {
            get
            {
                return capacity;
            }
            set
            {
                capacity = value;
                OnPropertyChanged(nameof(Capacity));
            }
        }

        /// <summary>
        /// The postfix of a volume's capacity
        /// </summary>
        public SizePostfix CapacityPostfix
        {
            get
            {
                return capacitypostfix;
            }
            set
            {
                capacitypostfix = value;
                OnPropertyChanged(nameof(CapacityPostfix));
            }
        }

        /// <summary>
        /// The string representation of a volume's capacity
        /// </summary>
        public string FullCapacity
        {
            get
            {
                return Capacity + " " + CapacityPostfix;
            }
        }

        /// <summary>
        /// The free space of a volume
        /// </summary>
        public int FreeSpace
        {
            get
            {
                return freespace;
            }
            set
            {
                freespace = value;
                OnPropertyChanged(nameof(FreeSpace));
            }
        }

        /// <summary>
        /// The postfix of a volume's free space
        /// </summary>
        public SizePostfix FreeSpacePostfix
        {
            get
            {
                return freespacepostfix;
            }
            set
            {
                freespacepostfix = value;
                OnPropertyChanged(nameof(FreeSpacePostfix));
            }
        }

        /// <summary>
        /// The string representation of a volume's free space
        /// </summary>
        public string FullFreeSpace
        {
            get
            {
                return FreeSpace + " " + FreeSpacePostfix;
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
            return Type == VolumeType.Removable;
        }

        /// <summary>
        /// Is the volume a Hidden device?
        /// </summary>
        /// <returns>Whether the volume is hidden</returns>
        public override bool IsHidden()
        {
            return Attributes.HasFlag(Attributes.Hidden);
        }

        /// <summary>
        /// Can a Volume be set to read only?
        /// </summary>
        /// <returns></returns>
        public override bool CanToggleReadOnly()
        {
            if (FileSystem == FileSystem.RAW)
                return false;
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
            if (FileSystem == FileSystem.RAW)
                return false;
            if (Type == VolumeType.Removable)
                return true;
            return false;
        }

        /// <summary>
        /// Can this media item be ejected?
        /// </summary>
        /// <returns></returns>
        public override bool CanBeEjected()
        {
            if (FileSystem == FileSystem.RAW)
                return false;
            if (Type == VolumeType.Removable)
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
            return "Volume " + DriveLetter + " " + Name;
        }
    }
}
