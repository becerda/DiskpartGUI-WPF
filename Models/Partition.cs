using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskpartGUI.Models
{
    class Partition : BaseMedia
    {
        private int offset;
        private SizePostfix offsetpostfix;
        private MediaType mediatype;

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
        public MediaType MediaType
        {
            get
            {
                return mediatype;
            }
            set
            {
                mediatype = value;
                OnPropertyChanged(nameof(Type));
            }
        }

        public override string ToString()
        {
            return "Partition " + Number;
        }
    }
}
