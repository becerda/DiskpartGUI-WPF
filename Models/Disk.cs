using System.Collections.Generic;

namespace DiskpartGUI.Models
{

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

    /// <summary>
    /// The type of a disk
    /// </summary>
    public enum DiskType
    {
        RAID,
        USB
    }

    static class DiskTypeExtension
    {
        private static Dictionary<string, DiskType> disk = new Dictionary<string, DiskType>
        {
            {DiskType.RAID + "", DiskType.RAID },
            {DiskType.USB + "", DiskType.USB }
        };

        public static DiskType Parse(string type)
        {
            return disk[type];
        }
    }

    public class Disk : BaseMedia
    {
        private string diskid;
        private DiskType type;
        private string path;
        private string target;
        private string lunid;
        private string locationpath;

        private int freespace;
        private SizePostfix freesizepostfix;
        private DynamicType dynamic;
        private GPTType gpttype;
        public List<BaseMedia> partitions;

        /// <summary>
        /// The ID of a disk
        /// </summary>
        public string DiskID
        {
            get
            {
                return diskid;
            }
            set
            {
                diskid = value;
                OnPropertyChanged(nameof(DiskID));
            }
        }

        /// <summary>
        /// The type of a disk
        /// </summary>
        public DiskType Type
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
        /// The path of a disk
        /// </summary>
        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
                OnPropertyChanged(nameof(Path));
            }
        }

        /// <summary>
        /// The target of a disk
        /// </summary>
        public string Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
                OnPropertyChanged(nameof(Target));
            }
        }

        /// <summary>
        /// The LUN ID of a disk
        /// </summary>
        public string LunID
        {
            get
            {
                return lunid;
            }
            set
            {
                lunid = value;
                OnPropertyChanged(nameof(LunID));
            }
        }

        /// <summary>
        /// The location path of a disk
        /// </summary>
        public string LocationPath
        {
            get { return locationpath; }
            set { locationpath = value; OnPropertyChanged(nameof(LocationPath)); }
        }

        /// <summary>
        /// The free space of a disk
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
        /// The postfix of the free space size
        /// </summary>
        public SizePostfix FreeSpacePostfix
        {
            get
            {
                return freesizepostfix;
            }
            set
            {
                freesizepostfix = value;
                OnPropertyChanged(nameof(freesizepostfix));
            }
        }

        /// <summary>
        /// The full string of the free space
        /// </summary>
        public string FullFreeSpace
        {
            get
            {
                return FreeSpace + " " + (FreeSpacePostfix == SizePostfix.None ? "B" : FreeSpacePostfix + "");
            }
        }

        /// <summary>
        /// The Dynamic property of a disk
        /// </summary>
        public string Dynamic
        {
            get
            {
                if (dynamic == DynamicType.Blank)
                    return string.Empty;
                return dynamic + "";
            }
            set
            {
                dynamic = DynamicTypeExtension.Parse(value);
                OnPropertyChanged(nameof(Dynamic));
            }
        }

        /// <summary>
        /// The GPT property of a disk
        /// </summary>
        public string GPTType
        {
            get
            {
                if (gpttype == Models.GPTType.Blank)
                    return string.Empty;
                return gpttype + "";
            }
            set
            {
                gpttype = GPTTypeExtension.Parse(value);
                OnPropertyChanged(nameof(GPTType));
            }
        }

        /// <summary>
        /// The partitions of a disk
        /// </summary>
        public List<BaseMedia> Partitions
        {
            get
            {
                return partitions;
            }
            set
            {
                partitions = value;
                OnPropertyChanged(nameof(Partitions));
            }
        }

        /// <summary>
        /// Can this media item be set to read only?
        /// </summary>
        /// <returns></returns>
        public override bool CanToggleReadOnly()
        {
            return true;
        }

        /// <summary>
        /// The string representation of a disk
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Disk " + Number;
        }
    }
}
