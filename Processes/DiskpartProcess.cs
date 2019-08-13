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
        private List<string> script;

        private string file = String.Empty;

        public DiskpartProcess() : base("diskpart")
        {
            script = new List<string>();
            AddArgument("/s");
        }

        public void AddScriptCommand(string line)
        {
            script.Add(line);
        }

        private void WriteScript()
        {
            try
            {
                if (file == String.Empty)
                    file = Path.GetTempPath() + Guid.NewGuid() + ".txt";

                File.WriteAllLines(file, script);

                AddArgument(file);
                script.Clear();
            } catch(IOException e)
            {
                StdError = e.StackTrace;
            }
        }

        public List<Volume> GetVolumes()
        {
            AddScriptCommand("LIST VOLUME");
            WriteScript();
            if(Run() == ProcessExitCode.Ok)
            {
                return ParseVolumes();
            }
            return null;
        }

        private List<Volume> ParseVolumes()
        {
            Regex rx = new Regex("Volume (?<volnum>[0-9]){1,2}( ){4,5}(?<vollet>[A-Z ])( ){0,3}(?<vollab>[a-zA-Z ]{0,11})( ){2,3}(?<volfs>NTFS|FAT32|exFAT|CDFS|UDF)?( ){2,7}(?<voltype>Partition|Removable|DVD-ROM|Simple)?( ){3,14}(?<volsize>[1-9]{1,4})?( )(?<volgk>K|G|M)?B( ){2}(?<volstat>Healthy|No Media)?( ){0,11}(?<volinfo>[a-zA-Z]+)?");
            MatchCollection matches = rx.Matches(StdOutput);
            if (matches.Count > 0)
            {
                List<Volume> volumes = new List<Volume>();
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
                return volumes;
            }
            return null;
        }

        public ProcessExitCode EjectVolume(Volume v)
        {
            if (v.IsValid())
            {
                AddScriptCommand("SELECT " + v.Number);
                AddScriptCommand("REMOVE ALL DISMOUNT");
                WriteScript();
                if(Run() == ProcessExitCode.Ok)
                {
                    if (TestOutput(@"DiskPart successfully dismounted and offlined the volume."))
                        return ProcessExitCode.Ok;
                    return ProcessExitCode.Error;
                }
            }
            return ProcessExitCode.ErrorInvalidVolume;
        }

        public ProcessExitCode AssignVolumeLetter(Volume v)
        {
            if (!v.IsMounted())
            {
                AddScriptCommand("SELECT " + v.Number);
                AddScriptCommand("ASSIGN");
                WriteScript();
                if(Run() == ProcessExitCode.Ok)
                {
                    if (TestOutput("DiskPart successfully assigned the drive letter or mount point."))
                        return ProcessExitCode.Ok;
                    return ProcessExitCode.Error;
                }
            }
            return ProcessExitCode.ErrorVolumeMounted;
        }
    }
}
