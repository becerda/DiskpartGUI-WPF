using DiskpartGUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiskpartGUI.Processes
{
    /// <summary>
    /// The function call type of diskpart attributes
    /// </summary>
    public enum ReadOnlyFunction
    {
        SET,
        CLEAR
    }

    public enum MediaType
    {
        VOLUME,
        DISK
    }

    public class DiskpartProcess : CMDProcess
    {

        private static readonly string Disk_Parse_RX = "Disk (?<disknum>[0-9]){1,2}( ){3,4}(?<diskstat>Online| )?( ){0,15}(?<disksize>[0-9]{1,4})?( )(?<diskgk>K|G|M)?B( ){2,6}(?<diskfree>[0-9]{1,4})?( )(?<diskfreegk>K|G|M)?B( ){2}( ){2}(?<diskdyn>[ a-zA-Z]{3})?( ){2}(?<diskgpt>[ *a-zA-Z]{3})?";
        private static readonly string Volume_Parse_RX = "Volume (?<volnum>[0-9]){1,2}( ){4,5}(?<vollet>[A-Z ])( ){0,3}(?<vollab>[a-zA-Z ]{0,11})( ){2,3}(?<volfs>NTFS|FAT32|exFAT|CDFS|UDF)?( ){2,7}(?<voltype>Partition|Removable|DVD-ROM|Simple)?( ){3,14}(?<volsize>[0-9]{1,4})?( )(?<volgk>K|G|M)?B( ){2}(?<volstat>Healthy|No Media)?( ){0,11}(?<volinfo>[a-zA-Z]+)?";
        private static readonly string ReadOnly_Parse_RX = "Read-only( )+: (?<set>Yes|No)";

        private static readonly string Disk_ReadOnly_Test_RX = "Disk attributes (set|cleared) successfully.";
        private static readonly string Volume_ReadOnly_Test_RX = "Volume attributes (set|cleared) successfully.";


        /// <summary>
        /// The Diskpart script lines
        /// </summary>
        private List<string> script;

        /// <summary>
        /// The temp file to write and read file from
        /// </summary>
        private string file = String.Empty;

        /// <summary>
        /// Initializes a new DiskpartProcess Instance
        /// </summary>
        public DiskpartProcess() : base("diskpart")
        {
            script = new List<string>();
        }

        /// <summary>
        /// Adds a command line to the script
        /// </summary>
        /// <param name="line"></param>
        public void AddScriptCommand(string line)
        {
            script.Add(line);
        }

        /// <summary>
        /// Writes the script out to a temp file
        /// </summary>
        private void WriteScript()
        {
            try
            {
                if (!File.Exists(file))
                {
                    file = Path.GetTempPath() + "diskpart_script.txt";
                }

                File.WriteAllLines(file, script);

                AddArgument("/s");
                AddArgument(file);
                script.Clear();
            }
            catch (IOException e)
            {
                StdError = e.StackTrace;
            }
        }

        /// <summary>
        /// Sets the Volumes to a List from diskpart
        /// </summary>
        /// <param name="volumes">The list to set too</param>
        /// <returns>The process exit code</returns>
        public ProcessExitCode RunListCommand(ref List<BaseMedia> list, MediaType type)
        {
            CurrentProcess = nameof(RunListCommand);
            AddScriptCommand("LIST " + type);
            WriteScript();
            if (Run() == ProcessExitCode.Ok)
            {
                return ParseListCommand(ref list, type);
            }
            else
                ExitCode = ProcessExitCode.ErrorRun;
            return ExitCode;
        }

        /// <summary>
        /// Parses a string to search for Media information
        /// </summary>
        /// <param name="volumes">The List to add to</param>
        /// <returns>The process exit code</returns>
        private ProcessExitCode ParseListCommand(ref List<BaseMedia> list, MediaType type)
        {
            CurrentProcess = nameof(ParseListCommand);
            Regex rx;
            switch (type)
            {
                case MediaType.DISK:
                    rx = new Regex(Disk_Parse_RX);
                    break;
                case MediaType.VOLUME:
                    rx = new Regex(Volume_Parse_RX);
                    break;
                default:
                    rx = new Regex(string.Empty);
                    break;
            }

            MatchCollection matches = rx.Matches(StdOutput);
            if (matches.Count > 0)
            {
                list = new List<BaseMedia>();
                foreach (Match m in matches)
                {
                    GroupCollection gc = m.Groups;
                    BaseMedia b;
                    switch (type)
                    {
                        case MediaType.DISK:
                            b = new Disk
                            {
                                Number = Int32.Parse(gc["disknum"].Value),
                                Status = VolumeStatusExtension.Parse(gc["diskstat"].Value),
                                Size = Int32.Parse(gc["disksize"].Value),
                                SizePostfix = VolumeSizePostfixExtension.Parse(gc["diskgk"].Value),
                                FreeSpace = Int32.Parse(gc["diskfree"].Value),
                                FreeSpacePostfix = VolumeSizePostfixExtension.Parse(gc["diskfreegk"].Value),
                                Dynamic = gc["diskdyn"].Value.Trim(),
                                GPTType = gc["diskgpt"].Value.Trim()

                            };
                            break;
                        case MediaType.VOLUME:
                            b = new Volume
                            {
                                Number = Int32.Parse(gc["volnum"].Value),
                                Letter = gc["vollet"].Value.ElementAt<char>(0),
                                Label = gc["vollab"].Value,
                                FileSystem = FileSystemExtension.Parse(gc["volfs"].Value),
                                VolumeType = VolumeTypeExtension.Parse(gc["voltype"].Value),
                                Size = Int32.Parse(gc["volsize"].Value),
                                SizePostfix = VolumeSizePostfixExtension.Parse(gc["volgk"].Value),
                                Status = VolumeStatusExtension.Parse(gc["volstat"].Value),
                                Info = gc["volinfo"].Value,
                                MountState = MountStateExtension.Parse(gc["volinfo"].Value)
                            };
                            break;
                        default:
                            b = new BaseMedia();
                            break;
                    }
                    list.Add(b);
                }
                ExitCode = ProcessExitCode.Ok;

            }
            return ExitCode;
        }

        /// <summary>
        /// Gets the Read-Only flag of each Media item
        /// </summary>
        /// <param name="list">The Volumes to get the read-only flag</param>
        /// <returns>The process exit code</returns>
        public ProcessExitCode GetReadOnlyState(ref List<BaseMedia> list, MediaType type)
        {
            CurrentProcess = nameof(GetReadOnlyState);

            foreach (BaseMedia m in list)
            {
                AddScriptCommand("SELECT " + type + " " + m.Number);
                AddScriptCommand("ATTRIBUTE " + type);
            }
            WriteScript();
            if (Run() == ProcessExitCode.Ok)
            {
                return ParseReadOnlyState(ref list);
            }
            else
            {
                ExitCode = ProcessExitCode.ErrorRun;
            }
            return ExitCode;
        }

        /// <summary>
        /// Parses a string to search for Read-Only state
        /// </summary>
        /// <param name="list">The List to set Read-Only state to</param>
        /// <returns>The process exit code</returns>
        private ProcessExitCode ParseReadOnlyState(ref List<BaseMedia> list)
        {
            CurrentProcess = nameof(ParseReadOnlyState);
            if (list != null)
            {
                Regex rx = new Regex(ReadOnly_Parse_RX);
                MatchCollection matches = rx.Matches(StdOutput);
                if (matches.Count > 0)
                {
                    int i = 0;
                    foreach (Match match in matches)
                    {
                        list[i++].ReadOnlyState = match.Groups["set"].Value == "Yes" ? ReadOnlyState.Set : ReadOnlyState.Cleared;

                    }
                    ExitCode = ProcessExitCode.Ok;
                }
                else
                {
                    ExitCode = ProcessExitCode.ErrorParse;
                }
            }
            else
            {
                ExitCode = ProcessExitCode.ErrorNullVolumes;
            }
            return ExitCode;
        }

        /// <summary>
        /// Ejects a Volume
        /// </summary>
        /// <param name="b">The Volume to eject</param>
        /// <returns>The process exit code</returns>
        public ProcessExitCode EjectVolume(BaseMedia b)
        {
            CurrentProcess = nameof(EjectVolume);
            if (b.GetType() == typeof(Disk))
                ExitCode = ProcessExitCode.ErrorInvalidMediaType;
            else
            {
                if (b.IsValid())
                {
                    AddScriptCommand("SELECT VOLUME " + b.Number);
                    AddScriptCommand("REMOVE ALL DISMOUNT");
                    WriteScript();
                    if (Run() == ProcessExitCode.Ok)
                    {
                        if (TestOutput(@"DiskPart successfully dismounted and offlined the volume."))
                            ExitCode = ProcessExitCode.Ok;
                        else
                        {
                            ExitCode = ProcessExitCode.ErrorTestOutput;
                        }
                    }
                    else
                    {
                        ExitCode = ProcessExitCode.Error;
                    }
                }
                else
                {
                    ExitCode = ProcessExitCode.ErrorInvalidMediaType;
                }
            }
            return ExitCode;
        }

        /// <summary>
        /// Assigns a letter to an unmounted drive
        /// </summary>
        /// <param name="b">The Volume to assing to</param>
        /// <returns>The process exit code</returns>
        public ProcessExitCode AssignVolumeLetter(BaseMedia b)
        {
            CurrentProcess = nameof(AssignVolumeLetter);
            if (b.GetType() == typeof(Disk))
                ExitCode = ProcessExitCode.ErrorInvalidMediaType;
            else
            {
                if (!b.IsMounted())
                {
                    AddScriptCommand("SELECT VOLUME " + b.Number);
                    AddScriptCommand("ASSIGN");
                    WriteScript();
                    if (Run() == ProcessExitCode.Ok)
                    {
                        if (TestOutput("DiskPart successfully assigned the drive letter or mount point."))
                            ExitCode = ProcessExitCode.Ok;
                        else
                        {
                            ExitCode = ProcessExitCode.ErrorTestOutput;
                        }
                    }
                    else
                    {
                        ExitCode = ProcessExitCode.Error;
                    }
                }
                else
                {
                    ExitCode = ProcessExitCode.ErrorVolumeNotMounted;
                }
            }
            return ExitCode;
        }

        /// <summary>
        /// Sets or Clears the read-only flag of a Media item
        /// </summary>
        /// <param name="b">The volume to adjust read-only flag</param>
        /// <param name="function">The type of adjustment</param>
        /// <returns></returns>
        public ProcessExitCode SetReadOnly(BaseMedia b, ReadOnlyFunction function, MediaType type)
        {
            CurrentProcess = nameof(SetReadOnly);

            if (!b.CanToggleReadOnly())
            {
                ExitCode = ProcessExitCode.ErrorInvalidMediaType;
            }
            else
            {
                AddScriptCommand("SELECT " + type + " " + b.Number);
                AddScriptCommand("ATTRIBUTE " + type + " " + function + " READONLY");
                WriteScript();
                if (Run() == ProcessExitCode.Ok)
                {
                    string rx;
                    switch (type)
                    {
                        case MediaType.DISK:
                            rx = Disk_ReadOnly_Test_RX;
                            break;
                        case MediaType.VOLUME:
                            rx = Volume_ReadOnly_Test_RX;
                            break;
                        default:
                            rx = string.Empty;
                            break;
                    }
                    if (TestOutput(rx))
                        ExitCode = ProcessExitCode.Ok;
                    else
                    {
                        ExitCode = ProcessExitCode.ErrorTestOutput;
                    }
                }
                else
                {
                    ExitCode = ProcessExitCode.Error;
                }
            }
            return ExitCode;
        }

        /// <summary>
        /// Gets the FileSystem information for selected Volume
        /// </summary>
        /// <param name="v">The Volume to get information for</param>
        /// <param name="fs">List to store the available file systems</param>
        /// <param name="us">Lists to store the available sizes</param>
        /// <returns>The process exit code</returns>
        public ProcessExitCode GetFileSystemInfo(Volume v, ref List<FileSystem> fs, ref Dictionary<FileSystem, List<string>> us)
        {
            CurrentProcess = nameof(GetFileSystemInfo);
            if (!v.IsValid())
                ExitCode = ProcessExitCode.ErrorInvalidMediaType;
            else
            {
                AddScriptCommand("SELECT VOLUME " + v.Number);
                AddScriptCommand("FILESYSTEM");
                WriteScript();
                if (Run() == ProcessExitCode.Ok)
                {
                    return ParseFileSystemInfo(ref fs, ref us);
                }
                else
                    ExitCode = ProcessExitCode.ErrorRun;
            }
            return ExitCode;
        }

        /// <summary>
        /// Parses a string to look for file systems and their sizes
        /// </summary>
        /// <param name="fs">List to store the available file systems</param>
        /// <param name="us">Lists to store the available sizes</param>
        /// <returns>The process exit code</returns>
        private ProcessExitCode ParseFileSystemInfo(ref List<FileSystem> fs, ref Dictionary<FileSystem, List<string>> us)
        {
            CurrentProcess = nameof(ParseFileSystemInfo);

            if (fs == null)
            {
                ExitCode = ProcessExitCode.ErrorFileSystemNull;
                return ExitCode;
            }

            if (us == null)
            {
                ExitCode = ProcessExitCode.ErrorUnitSizeNull;
                return ExitCode;
            }

            Regex rx = new Regex(@"Type {17}: (?<fs>NTFS|FAT32|exFAT|CDFS|UDF)( \(Default\))?(\n|\r|\r\n)  Allocation Unit Sizes: (?<sizes>([0-9]+K?( \(Default\))?,? ?)*)", RegexOptions.Multiline);
            MatchCollection matches = rx.Matches(StdOutput);
            if (matches.Count > 0)
            {
                fs = new List<FileSystem>();
                fs.Add(FileSystem.Default);
                us = new Dictionary<FileSystem, List<string>>();
                us.Add(FileSystem.Default, new List<string> { "Default" });
                int i = 1;
                foreach (Match match in matches)
                {
                    string filesys = match.Groups["fs"].Value.Replace("(Default)", string.Empty);
                    fs.Add(FileSystemExtension.Parse(filesys));
                    string sizes = match.Groups["sizes"].Value.Replace("(Default)", string.Empty);
                    string[] list = sizes.Split(',');

                    List<string> temp = new List<string>();
                    temp.Add("Default");

                    foreach (string size in list)
                    {
                        temp.Add(size.Trim());
                    }
                    us.Add(fs[i++], temp);
                    ExitCode = ProcessExitCode.Ok;
                }
            }
            else
                ExitCode = ProcessExitCode.ErrorParse;

            return ExitCode;

        }

        /// <summary>
        /// Formats a Volume with given FormatArguments
        /// </summary>
        /// <param name="v">The Volume to format</param>
        /// <param name="fa">The arguments of the format</param>
        /// <returns></returns>
        public ProcessExitCode Format(Volume v, FormatArguments fa)
        {
            CurrentProcess = nameof(Format);

            if (v == null)
            {
                ExitCode = ProcessExitCode.ErrorNullVolumes;
                return ExitCode;
            }

            if (!v.IsValid())
            {
                ExitCode = ProcessExitCode.ErrorInvalidMediaType;
                return ExitCode;
            }

            AddScriptCommand("SELECT VOLUME " + v.Number);
            AddScriptCommand("FORMAT " + fa.GetArguments());
            WriteScript();
            if (Run() == ProcessExitCode.Ok)
            {
                if (TestOutput("DiskPart successfully formatted the volume"))
                    ExitCode = ProcessExitCode.Ok;
                else
                    ExitCode = ProcessExitCode.ErrorTestOutput;
            }
            else
                ExitCode = ProcessExitCode.ErrorRun;

            return ExitCode;
        }

    }
}
