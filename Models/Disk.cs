using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskpartGUI.Models
{
    public class Disk : BaseMedia
    {
        private int freespace;
        private SizePostfix freesizepostfix;
        private DynamicType dynamic;
        private GPTType gpttype;

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

        public string FullFreeSpace
        {
            get
            {
                return FreeSpace + " " + (FreeSpacePostfix == SizePostfix.None ? "B" : FreeSpacePostfix + "");
            }
        }

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

        public override string ToString()
        {
            return "Disk " + Number;
        }
    }
}
