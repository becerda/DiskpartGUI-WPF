using System;
using System.Collections.Generic;

namespace DiskpartGUI.Models
{

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

    public static class StatusExtension
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
    /// The Read-Only flag state of a volume
    /// </summary>
    public enum ReadOnlyState
    {
        Set,
        Cleared
    }

    /// <summary>
    /// Attributes that a media item can have
    /// </summary>
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
        Cluster = 512,
        Offline = 1024,
        BitLocker = 2048,
        Installable = 4096,
        Active = 8192
    }

    public class BaseMedia : BaseModel
    {
        protected string name;
        private int number;
        private Status status;
        private int size;
        private SizePostfix sizepostfix;
        private Attributes attribs;
        private int parent;

        /// <summary>
        /// The Name/Label of a media item
        /// </summary>
        public virtual string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value.ToUpper();
                OnPropertyChanged(nameof(Name));
            }
        }

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
        /// The number of the parent of a media item
        /// </summary>
        public int Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
                OnPropertyChanged(nameof(Parent));
            }
        }

        /// <summary>
        /// Sets or Clears the passed in flag
        /// </summary>
        /// <param name="attribute">The attribute to set or clear</param>
        /// <param name="state">Set or Clear</param>
        public void SetFlag(Attributes attribute, bool state = true)
        {
            if (state)
                Attributes |= attribute;
            else
                Attributes &= ~attribute;
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
