using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public enum MediaType
    {
        None,
        Partition,
        Removable,
        DVDROM,
        Simple,
        Primary
    }

    public static class MediaTypeExtension
    {
        private static readonly Dictionary<string, MediaType> type = new Dictionary<string, MediaType>
        {
            {"None", MediaType.None },
            {"Partition", MediaType.Partition },
            {"Removable", MediaType.Removable },
            {"DVD-ROM", MediaType.DVDROM },
            {"Simple", MediaType.Simple },
            {"Primary", MediaType.Primary }
        };

        /// <summary>
        /// Parses a string into a VolumeType enum
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <returns>The VolumeType enum</returns>
        public static MediaType Parse(string s)
        {
            return type[s];
        }
    }

    /// <summary>
    /// The prefix of the size of a volume
    /// </summary>
    public enum SizePostfix
    {
        None,
        KB,
        MB,
        GB
    }

    public static class SizePostfixExtension
    {
        private static readonly Dictionary<string, SizePostfix> post = new Dictionary<string, SizePostfix>
        {
            {"", SizePostfix.None },
            {"None", SizePostfix.None },
            {"K", SizePostfix.KB },
            {"M", SizePostfix.MB },
            {"G", SizePostfix.GB }
        };
        /// <summary>
        /// Parses a string to a VolumeSizePostfix enum
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <returns>The VolumeSizePostfix enum</returns>
        public static SizePostfix Parse(string s)
        {
            return post[s];
        }
    }

    /// <summary>
    /// The status of a volume
    /// </summary>
    public enum Status
    {
        Blank,
        Healthy,
        NoMedia,
        Online,
        Offline
    }

    public static class VolumeStatusExtension
    {
        private static readonly Dictionary<string, Status> status = new Dictionary<string, Status>
        {
            {"Blank", Status.Blank },
            {"Healthy", Status.Healthy },
            {"No Media", Status.NoMedia },
            {"Online", Status.Online }
        };

        /// <summary>
        /// Parses a string to a VolumeStatus enum
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <returns>The VolumeStatus enum</returns>
        public static Status Parse(string s)
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

    [Flags]
    public enum Attributes
    {
        None = 0,
        ReadOnly = 1,
        Hidden = 2,
        NoDefaultDriveLetter = 4,
        ShadowCopy = 8,
        CurrentReadOnlyState = 16,
        Boot = 32,
        Pagefile = 64,
        HibernationFile = 128,
        Crashdump = 256,
        Cluster = 512
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

    /// <summary>
    /// The DynamicType of a disk
    /// Not Complete
    /// </summary>
    public enum DynamicType
    {
        Blank,
    }

    static class DynamicTypeExtension
    {
        private static readonly Dictionary<string, DynamicType> dynamic = new Dictionary<string, DynamicType>
        {
            {"", DynamicType.Blank }
        };


        public static DynamicType Parse(string type)
        {
            return dynamic[type];
        }
    }

    /// <summary>
    /// The GPT type of a disk
    /// Not Complete
    /// </summary>
    public enum GPTType
    {
        Blank,
        Set
    }

    static class GPTTypeExtension
    {
        private static readonly Dictionary<string, GPTType> gpt = new Dictionary<string, GPTType>
        {
            {"", GPTType.Blank },
            {"*", GPTType.Set }
        };

        public static GPTType Parse(string type)
        {
            return gpt[type];
        }
    }

    public class BaseMedia : BaseModel
    {

        private int number;
        private Status status;
        private int size;
        private SizePostfix sizepostfix;
        private Attributes attribs;

        /// <summary>
        /// The number a media item is assigned
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
                OnPropertyChanged(nameof(Number));
            }
        }

        /// <summary>
        /// The status of a media item
        /// </summary>
        public Status Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        /// <summary>
        /// The size of a media item
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
                OnPropertyChanged(nameof(Size));
            }
        }

        /// <summary>
        /// The prefix to a media item size
        /// </summary>
        public SizePostfix SizePostfix
        {
            get
            {
                return sizepostfix;
            }
            set
            {
                sizepostfix = value;
                OnPropertyChanged(nameof(SizePostfix));
            }
        }

        /// <summary>
        /// The Read-Only flag of a media item
        /// </summary>
        public Attributes Attributes
        {
            get
            {
                return attribs;
            }
            set
            {
                attribs = value;
                OnPropertyChanged(nameof(ReadOnlyState));
            }
        }

        /// <summary>
        /// The full string size of a media item
        /// </summary>
        public string FullSize
        {
            get
            {
                return Size + " " + (SizePostfix == SizePostfix.None ? "B" : SizePostfix + "");
            }
        }

        /// <summary>
        /// Is the media item's Read-Only flag set?
        /// </summary>
        /// <returns></returns>
        public bool IsReadOnly()
        {
            return Attributes.HasFlag(Attributes.ReadOnly);
        }

        /// <summary>
        /// Can this media item be set to read only?
        /// </summary>
        /// <returns></returns>
        public virtual bool CanToggleReadOnly()
        {
            return false;
        }

        /// <summary>
        /// Can this media item be renamed?
        /// </summary>
        /// <returns></returns>
        public virtual bool CanBeRenamed()
        {
            return false;
        }

        /// <summary>
        /// Can this media item be ejected?
        /// </summary>
        /// <returns></returns>
        public virtual bool CanBeEjected()
        {
            return false;
        }

        /// <summary>
        /// Can this media item be formated?
        /// </summary>
        /// <returns></returns>
        public virtual bool CanBeFormated()
        {
            return false;
        }

        /// <summary>
        /// Is the media item Removable device?
        /// </summary>
        /// <returns>Whether the volume is removable</returns>
        public virtual bool IsRemovable()
        {
            return false;
        }

        /// <summary>
        /// Is the media item mounted?
        /// </summary>
        /// <returns>Whether the volume is mounted</returns>
        public virtual bool IsMounted()
        {
            return false;
        }

        /// <summary>
        /// Gets a string representation of a media item
        /// </summary>
        /// <returns>The DriveLetter and Label</returns>
        public override string ToString()
        {
            return "Media Item: " + Number;
        }
    }
}
