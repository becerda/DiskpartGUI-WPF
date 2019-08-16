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
    enum ReadOnlyFunction
    {
        SET,
        CLEAR
    }

    public class DiskpartProcess : CMDProcess
    {
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
        public ProcessExitCode GetVolumes(ref List<Volume> volumes)
        {
            CurrentProcess = nameof(GetVolumes);

            AddScriptCommand("LIST VOLUME");
            WriteScript();
            if (Run() == ProcessExitCode.Ok)
                return ParseVolumes(ref volumes);
            else
                ExitCode = ProcessExitCode.ErrorRun;
            return ExitCode;
        }

        /// <summary>
        /// Parses a string to search for Volume information
        /// </summary>
        /// <param name="volumes">The List to add to</param>
        /// <returns>The process exit code</returns>
        private ProcessExitCode ParseVolumes(ref List<Volume> volumes)
        {
            CurrentProcess = nameof(ParseAttributes);

            Regex rx = new Regex("Volume (?<volnum>[0-9]){1,2}( ){4,5}(?<vollet>[A-Z ])( ){0,3}(?<vollab>[a-zA-Z ]{0,11})( ){2,3}(?<volfs>NTFS|FAT32|exFAT|CDFS|UDF)?( ){2,7}(?<voltype>Partition|Removable|DVD-ROM|Simple)?( ){3,14}(?<volsize>[1-9]{1,4})?( )(?<volgk>K|G|M)?B( ){2}(?<volstat>Healthy|No Media)?( ){0,11}(?<volinfo>[a-zA-Z]+)?");
            MatchCollection matches = rx.Matches(StdOutput);
            if (matches.Count > 0)
            {
                volumes = new List<Volume>();
                foreach (Match m in matches)
                {
                    GroupCollection gc = m.Groups;
                    Volume v = new Volume
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
                    volumes.Add(v);
                }
                ExitCode = ProcessExitCode.Ok;
            }
            else
            {
                ExitCode = ProcessExitCode.ErrorParse;
            }
            return ExitCode;
        }

        /// <summary>
        /// Gets the Read-Only flag of each Volume
        /// </summary>
        /// <param name="volumes">The Volumes to get the read-only flag</param>
        /// <returns>The process exit code</returns>
        public ProcessExitCode GetReadOnlyState(ref List<Volume> volumes)
        {
            CurrentProcess = nameof(GetReadOnlyState);

            foreach (Volume v in volumes)
            {
                AddScriptCommand("SELECT DISK " + v.Number);
                AddScriptCommand("ATTRIBUTE DISK");
            }
            WriteScript();
            if (Run() == ProcessExitCode.Ok)
            {
                return ParseAttributes(ref volumes);
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
        /// <param name="volumes">The List to set Read-Only state to</param>
        /// <returns>The process exit code</returns>
        private ProcessExitCode ParseAttributes(ref List<Volume> volumes)
        {
            CurrentProcess = nameof(ParseAttributes);
            if (volumes != null)
            {
                Regex rx = new Regex("Read-only  : (?<set>Yes|No)");
                MatchCollection matches = rx.Matches(StdOutput);
                if (matches.Count > 0)
                {
                    int i = 0;
                    foreach (Match match in matches)
                    {
                        volumes[i++].ReadOnlyState = match.Groups["set"].Value == "Yes" ? ReadOnlyState.Set : ReadOnlyState.Cleared;

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
        /// <param name="v">The Volume to eject</param>
        /// <returns>The process exit code</returns>
        public ProcessExitCode EjectVolume(Volume v)
        {
            CurrentProcess = nameof(EjectVolume);
            if (v.IsValid())
            {
                AddScriptCommand("SELECT VOLUME " + v.Number);
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
                ExitCode = ProcessExitCode.ErrorInvalidVolume;
            }
            return ExitCode;
        }

        /// <summary>
        /// Assigns a letter to an unmounted drive
        /// </summary>
        /// <param name="v">The Volume to assing to</param>
        /// <returns>The process exit code</returns>
        public ProcessExitCode AssignVolumeLetter(Volume v)
        {
            CurrentProcess = nameof(AssignVolumeLetter);
            if (!v.IsMounted())
            {
                AddScriptCommand("SELECT VOLUME " + v.Number);
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
            return ExitCode;
        }

        /// <summary>
        /// Calls SetReadOnly to set the flag
        /// </summary>
        /// <param name="v">The Volume to set read-only flag</param>
        /// <returns>The process exit code</returns>
        public ProcessExitCode SetReadOnly(Volume v)
        {
            return SetReadOnly(v, ReadOnlyFunction.SET);
        }

        /// <summary>
        /// Calls SetReadOnly to clear the flag
        /// </summary>
        /// <param name="v">The Volume to clear read-only flag</param>
        /// <returns>The process exit code</returns>
        public ProcessExitCode ClearReadOnly(Volume v)
        {
            return SetReadOnly(v, ReadOnlyFunction.CLEAR);
        }

        /// <summary>
        /// Sets or Clears the read-only flag of a Volume
        /// </summary>
        /// <param name="v">The volume to adjust read-only flag</param>
        /// <param name="function">The type of adjustment</param>
        /// <returns></returns>
        private ProcessExitCode SetReadOnly(Volume v, ReadOnlyFunction function)
        {
            CurrentProcess = nameof(SetReadOnly);

            if (!v.IsValid())
            {
                ExitCode = ProcessExitCode.ErrorInvalidVolume;
            }
            else
            {
                AddScriptCommand("SELECT DISK " + v.Number);
                AddScriptCommand("ATTRIBUTE DISK " + function + " READONLY");
                WriteScript();
                if (Run() == ProcessExitCode.Ok)
                {
                    if (TestOutput("Disk attributes (set|cleared) successfully."))
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
        /// Gets the FileSystem information for selected volume
        /// </summary>
        /// <param name="v">The Volume to get information for</param>
        /// <param name="fs">List to store the available file systems</param>
        /// <param name="us">Lists to store the available sizes</param>
        /// <returns>The process exit code</returns>
        public ProcessExitCode GetFileSystemInfo(Volume v, ref List<FileSystem> fs, ref List<List<UnitSize>> us)
        {
            CurrentProcess = nameof(GetFileSystemInfo);
            if (!v.IsValid())
                ExitCode = ProcessExitCode.ErrorInvalidVolume;
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
        private ProcessExitCode ParseFileSystemInfo(ref List<FileSystem> fs, ref List<List<UnitSize>> us)
        {
            CurrentProcess = nameof(ParseFileSystemInfo);

            if(fs == null)
            {
                ExitCode = ProcessExitCode.ErrorFileSystemNull;
                return ExitCode;
            }

            if(us == null)
            {
                ExitCode = ProcessExitCode.ErrorUnitSizeNull;
                return ExitCode;
            }

            Regex rx = new Regex(@"Type {17}: (?<fs>NTFS|FAT32|exFAT|CDFS|UDF)( \(Default\))?(\n|\r|\r\n)  Allocation Unit Sizes: (?<sizes>([0-9]+K?( \(Default\))?,? ?)*)", RegexOptions.Multiline);
            MatchCollection matches = rx.Matches(StdOutput);
            if (matches.Count > 0)
            {
                fs = new List<FileSystem>();
                us = new List<List<UnitSize>>();
                int i = 0;
                foreach (Match match in matches)
                {
                    string filesys = match.Groups["fs"].Value.Replace("(Default)", string.Empty);
                    fs.Add(FileSystemExtension.Parse(filesys));
                    string sizes = match.Groups["sizes"].Value.Replace("(Default)", string.Empty);
                    string[] list = sizes.Split(',');
                    us.Add(new List<UnitSize>());
                    foreach (string size in list)
                    {
                        us[i].Add(UnitSizeExtension.Parse(size.Trim()));
                    }
                    i++;
                    ExitCode = ProcessExitCode.Ok;
                }
            }
            else
                ExitCode = ProcessExitCode.ErrorParse;

            return ExitCode;

        }

    }
}
