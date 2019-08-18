using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskpartGUI.Models
{
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
