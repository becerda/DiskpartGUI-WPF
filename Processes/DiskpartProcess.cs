using DiskpartGUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiskpartGUI.Processes
{
    class DiskpartProcess : CMDProcess
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
            AddScriptCommand("LIST VOLUME");
            WriteScript();
            if (Run() == ProcessExitCode.Ok)
            {
                return ParseVolumes(ref volumes);
            }
            else
            {
                ExitCode = ProcessExitCode.Error;
                StdError = "GetVolumes - Run Failed";
            }
            return ExitCode;
        }

        /// <summary>
        /// Parses a string to search for Volume information
        /// </summary>
        /// <param name="volumes">The List to add to</param>
        /// <returns>The process exit code</returns>
        private ProcessExitCode ParseVolumes(ref List<Volume> volumes)
        {
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
                        Info = gc["volinfo"].Value
                    };
                    volumes.Add(v);
                }
                ExitCode = ProcessExitCode.Ok;
            }
            else
            {
                ExitCode = ProcessExitCode.ErrorParse;
                StdError = "ParseVolumes - No Matches Found";
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
                ExitCode = ProcessExitCode.Error;
                StdError = "GetReadOnlyState - Run Failed";
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
            if (volumes != null)
            {
                Regex rx = new Regex("Read-only  : (?<set>Yes|No)");
                MatchCollection matches = rx.Matches(StdOutput);
                int i = 0;
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        volumes[i].IsReadOnly = match.Groups["set"].Value == "Yes" ? true : false;
                    }
                    ExitCode = ProcessExitCode.Ok;
                }
                else
                {
                    ExitCode = ProcessExitCode.ErrorParse;
                    StdError = "ParseAttributes - No Matches Found";
                }
            }
            else
            {
                ExitCode = ProcessExitCode.ErrorNullVolumes;
                StdError = "ParseAttributes - Null Volumes";
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
            if (v.IsValid())
            {
                AddScriptCommand("SELECT " + v.Number);
                AddScriptCommand("REMOVE ALL DISMOUNT");
                WriteScript();
                if (Run() == ProcessExitCode.Ok)
                {
                    if (TestOutput(@"DiskPart successfully dismounted and offlined the volume."))
                        ExitCode = ProcessExitCode.Ok;
                    else
                    {
                        ExitCode = ProcessExitCode.ErrorTestOutput;
                        StdError = "EjectVolume - Test Output Failed";
                    }
                }
                else
                {
                    ExitCode = ProcessExitCode.Error;
                    StdError = "EjectVolume - Run Failed";
                }
            }
            else
            {
                ExitCode = ProcessExitCode.ErrorInvalidVolume;
                StdError = "EjectVolume - Invalid Volume";
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
            if (!v.IsMounted())
            {
                AddScriptCommand("SELECT " + v.Number);
                AddScriptCommand("ASSIGN");
                WriteScript();
                if (Run() == ProcessExitCode.Ok)
                {
                    if (TestOutput("DiskPart successfully assigned the drive letter or mount point."))
                        ExitCode = ProcessExitCode.Ok;
                    else
                    {
                        ExitCode = ProcessExitCode.ErrorTestOutput;
                        StdError = "AssignVolumeLetter - Test Output Failed";
                    }
                }
                else
                {
                    ExitCode = ProcessExitCode.Error;
                    StdError = "AssignVolumeLetter - Run Failed";
                }
            }
            else
            {
                ExitCode = ProcessExitCode.ErrorVolumeMounted;
                StdError = "AssignVolumeLetter - Volume Not Mounted";
            }
            return ExitCode;
        }
    }
}
