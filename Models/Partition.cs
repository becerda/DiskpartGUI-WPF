using System.Collections.Generic;

namespace DiskpartGUI.Models
{
    /// <summary>
    /// The types of a partition
    /// </summary>
    public enum PartitionType
    {
        Extended,
        Simple,
        Primary,
        Logical
    }

    static class PartitionTypeExtension
    {
        private static readonly Dictionary<string, PartitionType> partition = new Dictionary<string, PartitionType>
        {
            {PartitionType.Extended + "", PartitionType.Extended },
            {PartitionType.Simple + "", PartitionType.Simple },
            {PartitionType.Primary + "", PartitionType.Primary },
            {PartitionType.Logical + "", PartitionType.Logical }
        };

        public static PartitionType Parse(string type)
        {
            return partition[type];
        }
    }

    public class Partition : BaseMedia
    {
        private PartitionType partitiontype;
        private int offset;
        private SizePostfix offsetpostfix;
        private string offsetinbytes;
        private string type;

        /// <summary>
        /// The type of a partition
        /// </summary>
        public PartitionType PartitionType
        {
            get
            {
                return partitiontype;
            }
            set
            {
                partitiontype = value;
                OnPropertyChanged(nameof(Type));
            }
        }

        /// <summary>
        /// The Offset of a partition
        /// </summary>
        public int Offset
        {
            get
            {
                return offset;
            }
            set
            {
                offset = value;
                OnPropertyChanged(nameof(Offset));
            }
        }

        /// <summary>
        /// The offset in bytes of a partition
        /// </summary>
        public string OffsetInBytes
        {
            get
            {
                return offsetinbytes;
            }
            set
            {
                offsetinbytes = value;
                OnPropertyChanged(nameof(OffsetInBytes));
            }
        }

        /// <summary>
        /// The postfix of the offset of a partition
        /// </summary>
        public SizePostfix OffsetPostfix
        {
            get
            {
                return offsetpostfix;
            }
            set
            {
                offsetpostfix = value;
                OnPropertyChanged(nameof(OffsetPostfix));
            }
        }

        /// <summary>
        /// The string representation of the offset
        /// </summary>
        public string FullOffset
        {
            get
            {
                return Offset + " " + (OffsetPostfix == SizePostfix.None ? "B" : OffsetPostfix + "");
            }
        }

        /// <summary>
        /// The type of media a partition is
        /// </summary>
        public string Type
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
        /// The string representation of a partition
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Partition " + Number;
        }
    }
}
