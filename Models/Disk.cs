﻿
namespace DiskpartGUI.Models
{
    public class Disk : BaseMedia
    {
        private int freespace;
        private SizePostfix freesizepostfix;
        private DynamicType dynamic;
        private GPTType gpttype;

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
                return dynamic+"";
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
