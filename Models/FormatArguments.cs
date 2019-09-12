using System;
using System.Collections.Generic;

namespace DiskpartGUI.Models
{
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

    public class FormatArguments
    {
        /// <summary>
        /// Flags for NoErr and NoWait
        /// </summary>
        [Flags]
        public enum FormatOption
        {
            None,
            NoWait,
            NoError
        }

        /// <summary>
        /// The FileSystem argument
        /// </summary>
        public FileSystem FileSystem { get; set; }

        /// <summary>
        /// The Revision argument
        /// </summary>
        public string Revision { get; set; }

        /// <summary>
        /// The label argument
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The unit size argument
        /// </summary>
        public UnitSize Unit { get; set; }

        /// <summary>
        /// The Quick format argument
        /// </summary>
        public bool Quick { get; set; }

        /// <summary>
        /// The Compress format argument
        /// </summary>
        public bool Compress { get; set; }

        /// <summary>
        /// The Override format argument
        /// </summary>
        public bool Override { get; set; }

        /// <summary>
        /// The Duplicate format argument
        /// </summary>
        public bool Duplicate { get; set; }

        /// <summary>
        /// The NoWait and/or NoErr format argument
        /// </summary>
        public FormatOption Options { get; private set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FormatArguments() { }

        /// <summary>
        /// Initializes new FormatArguments with no Options flags
        /// </summary>
        /// <param name="fs">The FileSystem to use</param>
        /// <param name="revision">The Revision of the Filesystem</param>
        /// <param name="label">The new label of the Volume</param>
        /// <param name="us">The UnitSize of the Volume</param>
        /// <param name="quick">Use quick format</param>
        /// <param name="compress">Use compression</param>
        /// <param name="over">Use override</param>
        /// <param name="dup">Should duplicate</param>
        public FormatArguments(FileSystem fs, string revision, string label, UnitSize us, bool quick, bool compress, bool over, bool dup)
            : this(fs, revision, label, us, quick, compress, over, dup, FormatOption.None) { }

        /// <summary>
        /// Initializes new FormatArguments
        /// </summary>
        /// <param name="fs">The FileSystem to use</param>
        /// <param name="revision">The Revision of the Filesystem</param>
        /// <param name="label">The new label of the Volume</param>
        /// <param name="us">The UnitSize of the Volume</param>
        /// <param name="quick">Use quick format</param>
        /// <param name="compress">Use compression</param>
        /// <param name="over">Use override</param>
        /// <param name="dup">Should duplicate</param>
        /// <param name="options">Use NoWait and/or NoErr</param>
        public FormatArguments(FileSystem fs, string revision, string label, UnitSize us, bool quick, bool compress, bool over, bool dup, FormatOption options)
        {
            FileSystem = fs;
            Revision = revision;
            Label = label;
            Unit = us;
            Quick = quick;
            Compress = compress;
            Override = over;
            Duplicate = dup;
            Options = options;
        }

        /// <summary>
        /// The string arguments for formatting
        /// </summary>
        /// <returns>The string arguments to use with DiskpartProcess.Format</returns>
        public string GetArguments()
        {
            string args = string.Empty;

            if (FileSystem == FileSystem.Default)
            {
                args += "RECOMMENDED ";
            }
            else
            {
                args += "FS=" + FileSystem + " ";
                if(Revision != null && Revision != string.Empty)
                    args += "REVISION=" + Revision + " ";
            }

            if (Label != null && Label != string.Empty)
                args += "LABEL=" + Label + " ";

            if(Unit != UnitSize.Default)
                args += "UNIT=" + UnitSizeExtension.ToString(Unit) + " ";

            if (Quick)
                args += "QUICK ";

            if (Compress)
                args += "COMPRESS ";

            if (Override)
                args += "OVERRIDE ";

            if (Duplicate)
                args += "DUPLICATE ";

            if (Options.HasFlag(FormatOption.NoWait))
                args += "NOWAIT ";

            if (Options.HasFlag(FormatOption.NoError))
                args += "NOERR";

            return args.Trim();
        }
    }
}
