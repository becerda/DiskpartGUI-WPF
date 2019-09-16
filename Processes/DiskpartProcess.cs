using DiskpartGUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static DiskpartGUI.Helpers.Logger;

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

    /// <summary>
    /// The storage type
    /// </summary>
    public enum StorageType
    {
        VOLUME,
        DISK,
        PARTITION
    }

    public class DiskpartProcess : CMDProcess
    {

        //language=regex
        private const string Disk_Parse_RX = "Disk (?<disknum>[0-9]+){1,2}( ){3,4}(?<diskstat>Online| )?( ){0,15}(?<disksize>[0-9]{1,4})?( )(?<diskgk>K|G|M)?B( ){2,6}(?<diskfree>[0-9]{1,4})?( )(?<diskfreegk>K|G|M)?B( ){2}( ){2}(?<diskdyn>[ a-zA-Z]{3})?( ){2}(?<diskgpt>[ *a-zA-Z]{3})?";
        //language=regex
        private const string Volume_Parse_RX = "Volume (?<volnum>[0-9]+){1,2}( ){4,5}(?<vollet>[A-Z ])( ){0,3}(?<vollab>[a-zA-Z0-9 ]{0,11})( ){2,3}(?<volfs>NTFS|FAT32|exFAT|CDFS|UDF|RAW)?( ){2,7}(?<voltype>Partition|Removable|DVD-ROM|Simple)?( ){3,14}(?<volsize>[0-9]{1,4})?( )(?<volgk>K|G|M)?B( ){2}(?<volstat>Healthy|No Media)?( ){0,11}(?<volinfo>[a-zA-Z]+)?";
        //language=regex
        private const string Partition_Parse_RX = "Partition (?<partnum>[0-9]+)( ){3,4}(?<parttype>Primary|Extended|Logical)( ){10,19}(?<partsize>[0-9]+)( )(?<partsizegk>K|G|M)?B( ){2,4}(?<partoff>[0-9]+)( )(?<partoffgk>K|G|M)?B";

        //language=regex
        private const string Disk_Attribute_Parse_RX = "Disk (?<disknum>[0-9]+) is now the selected disk\\.\\r\\nCurrent Read-only State : (?<croflag>Yes|No)\\r\\nRead-only( )+: (?<roflag>Yes|No)\\r\\nBoot Disk ( )+: (?<bdflag>Yes|No)\\r\\nPagefile Disk( )+: (?<pfflag>Yes|No)\\r\\nHibernation File Disk( )+: (?<hibflag>Yes|No)\\r\\nCrashdump Disk( )+: (?<cdflag>Yes|No)\\r\\nClustered Disk( )+: (?<clustflag>Yes|No)";
        //language=regex
        private const string Volume_Attribute_Parse_RX = "Volume (?<volnum>[0-9]+) is the selected volume\\.\\r\\nRead-only( )+: (?<roflag>Yes|No)\\r\\nHidden( )+: (?<hidflag>Yes|No)\\r\\nNo Default Drive Letter: (?<noddflag>Yes|No)\\r\\nShadow Copy( )+: (<shadFlagYes|No)";

        //language=regex
        private const string Disk_ReadOnly_Test_RX = "Disk attributes (set|cleared) successfully.";
        //language=regex
        private const string Volume_ReadOnly_Test_RX = "Volume attributes (set|cleared) successfully.";

        //language=regex
        private const string Disk_Detail_RX = "\\r\\n(?<diskname>[a-zA-Z0-9_ ]+)\\r\\nDisk ID: (?<diskid>[A-F0-9]+)\\r\\nType( )+: (?<disktype>USB|RAID)\\r\\nStatus : (?<diskstatus>Online|Offline)\\r\\nPath( )+: (?<diskpath>[a-zA-Z0-9]+)\\r\\nTarget : (?<disktarget>[a-zA-Z0-9]+)\\r\\nLUN ID : (?<disklunid>[a-zA-Z0-9]+)\\r\\nLocation Path : (?<disklocpath>[()#a-zA-Z0-9]+)\\r\\nCurrent Read-only State : (?<diskcurreadonly>Yes|No)\\r\\nRead-only( )+: (?<diskreadonly>Yes|No)\\r\\nBoot Disk( )+: (?<diskboot>Yes|No)\\r\\nPagefile Disk( )+: (?<diskpage>Yes|No)\\r\\nHibernation File Disk( )+: (?<diskhibernation>Yes|No)\\r\\nCrashdump Disk( )+: (?<diskcrashdump>Yes|No)\\r\\nClustered Disk( )+: (?<diskcluster>Yes|No)";
        //language=regex
        private const string Volume_Detail_RX = "Disk (?<parentnumber>[0-9]+)( )+(Online|)?( )+([0-9]+)( )(K|G|M)?B( ){2,}([0-9]+)( )(M|G|K)?B( )+\\r\\n\\r\\nRead-only( )+: (?<volreadonly>Yes|No)\\r\\nHidden( )+: (?<volhidden>Yes|No)\\r\\nNo Default Drive Letter: (?<volnodefletter>Yes|No)\\r\\nShadow Copy( )+: (?<volshadow>Yes|No)\\r\\nOffline( )+: (?<voloffline>Yes|No)\\r\\nBitLocker Encrypted( )+: (?<volbitlock>Yes|No)\\r\\nInstallable( )+: (?<volinstall>Yes|No)\\r\\n\\r\\nVolume Capacity( )+:( )+(?<volcap>[0-9]+) (?<volcapgk>M|K|G)?B\\r\\nVolume Free Space( )+:( )+(?<volfree>[0-9]+) (?<volfreegk>M|K|G)?B";
        //language=regex
        private const string Partition_Detail_RX = "\\r\\nPartition (?<partnumber>[0-9]+)\\r\\nType  : (?<parttype>[A-F0-9]+)\\r\\nHidden: (?<parthidden>Yes|No)\\r\\nActive: (?<partactive>Yes|No)\\r\\nOffset in Bytes: (?<partoffsetbyte>[0-9]+)\\r\\n\\r\\n  Volume ###  Ltr  Label( ){8}Fs( ){5}Type( ){8}Size( ){5}Status( ){5}Info\\r\\n  -{10}  -{3}  -{11}  -{5}  -{10}  -{7}  -{9}  -{8}\\r\\n. Volume (?<volnum>[0-9]+){1,2}( ){4,5}(?<vollet>[A-Z ])( ){0,3}(?<vollab>[a-zA-Z0-9 ]{0,11})( ){2,3}(?<volfs>NTFS|FAT32|exFAT|CDFS|UDF|RAW)?( ){2,7}(?<voltype>Partition|Removable|DVD-ROM|Simple)?( ){3,14}(?<volsize>[0-9]{1,4})?( )(?<volgk>K|G|M)?B( ){2}(?<volstat>Healthy|No Media)?( ){0,11}(?<volinfo>[a-zA-Z]+)?";

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
        /// <param name="line">The command to add</param>
        public void AddScriptCommand(string line)
        {
            Log(LoggerType.Info, "DiskpartProcess - AddScriptCommand(string)", "\"" + line + "\" added to script");
            script.Add(line);
        }

        /// <summary>
        /// Writes the script out to a temp file
        /// </summary>
        private void WriteScript()
        {
            Log(LoggerType.Info, "DiskpartProcess - WriteScript()", "Called");
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
                Log(LoggerType.Error, "DiskpartProcess - WriteScript()", StdError);
                StdError = e.StackTrace;
            }
        }

        /// <summary>
        /// Gets the list and details of all disks present on a machine
        /// </summary>
        /// <param name="disks">The list of disks to populate</param>
        /// <returns>The process exit code</returns>
        public ProcessExitCode GetDiskInfo(ref List<BaseMedia> disks)
        {
            Log(LoggerType.Info, "DiskpartProcess - GetDiskInfo(List)", "Called");
            return GetInfo(ref disks, StorageType.DISK);
        }

        /// <summary>
        /// Gets the list and details of all volumes present on a machine
        /// </summary>
        /// <param name="volumes">The list of volumes to populate</param>
        /// <returns>The process exit code</returns>
        public ProcessExitCode GetVolumeInfo(ref List<BaseMedia> volumes)
        {
            Log(LoggerType.Info, "DiskpartProcess - GetVolumeInfo(List)", "Called");
            return GetInfo(ref volumes, StorageType.VOLUME);
        }

        /// <summary>
        /// Gets the list and details of either a list of disks or volumes
        /// </summary>
        /// <param name="medias">The list of media to populate</param>
        /// <param name="type">The type of media</param>
        /// <returns>The process exit code</returns>
        private ProcessExitCode GetInfo(ref List<BaseMedia> medias, StorageType type)
        {
            Log(LoggerType.Info, "DiskpartProcess - GetInfo(List, " + type + ")", "Called");
            CurrentProcess = nameof(GetInfo) + type;

            if (medias != null)
                medias.Clear();

            medias = new List<BaseMedia>();

            ExitCode = ListCommand(ref medias, type);

            if (ExitCode == ProcessExitCode.Ok)
            {
                for (int i = 0; i < medias.Count; i++)
                {
                    BaseMedia media;
                    if (type == StorageType.DISK)
                        media = (Disk)medias[i];
                    else
                        media = (Volume)medias[i];

                    ExitCode = DetailCommand(ref media, type);
                    if (ExitCode != ProcessExitCode.Ok)
                    {
                        Log(LoggerType.Error, "DiskpartProcess - GetInfo(List, " + type + ")", "ListCommand - " + ExitCode + ": ");
                        LogAppend(StdOutput, true);
                        break;
                    }
                }
            }
            else
            {
                Log(LoggerType.Error, "DiskpartProcess - GetInfo(List, " + type + ")", "DetailCommand - " + ExitCode + ": ");
                LogAppend(StdOutput, true);
            }


            return ExitCode;
        }

        /// <summary>
        /// Gets the list and details of all partitions present on a disk
        /// </summary>
        /// <param name="disknumber">The disk to look for partitions on</param>
        /// /// <param name="partitions">The list of partitions to populate</param>
        /// <returns>The process exit code</returns>
        public ProcessExitCode GetPartitionInfo(int disknumber, ref List<BaseMedia> partitions)
        {
            Log(LoggerType.Info, "DiskpartProcess - GetPartitionInfo(" + disknumber + ", List)", "Called");
            CurrentProcess = nameof(GetPartitionInfo);

            if (partitions != null)
                partitions.Clear();

            partitions = new List<BaseMedia>();

            ExitCode = ListCommand(ref partitions, StorageType.PARTITION, disknumber);
            if (ExitCode == ProcessExitCode.Ok)
            {
                for (int i = 0; i < partitions.Count; i++)
                {
                    BaseMedia partition = (Partition)partitions[i];
                    ExitCode = DetailCommand(ref partition, StorageType.PARTITION, disknumber);
                    if (ExitCode == ProcessExitCode.Ok)
                        partition.Parent = disknumber;
                    else
                    {
                        Log(LoggerType.Error, "DiskpartProcess - GetPartitionInfo(" + disknumber + ", List)", "DetailCommand - " + ExitCode + ": ");
                        LogAppend(StdOutput, true);
                        break;
                    }
                }
            }
            else
            {
                Log(LoggerType.Error, "DiskpartProcess - GetPartitionInfo(" + disknumber + ", List)", "ListCommand - " + ExitCode + ": ");
                LogAppend(StdOutput, true);
            }


            return ExitCode;
        }

        /// <summary>
        /// Sets the Volumes to a List from diskpart
        /// </summary>
        /// <param name="volumes">The list to set too</param>
        /// <returns>The process exit code</returns>
        public ProcessExitCode ListCommand(ref List<BaseMedia> list, StorageType type, int selectedDisk = -1)
        {
            Log(LoggerType.Info, "DiskpartProcess - ListCommand(List, " + type + ", " + selectedDisk + ")", "Called");
            CurrentProcess = nameof(ListCommand);
            if (type == StorageType.PARTITION)
                AddScriptCommand("SELECT DISK " + selectedDisk);

            AddScriptCommand("LIST " + type);
            WriteScript();
            ExitCode = Run();
            if (ExitCode == ProcessExitCode.Ok)
            {

                Log(LoggerType.Info, "DiskpartProcess - ListCommand(List, " + type + ", " + selectedDisk + ")", "StdOutput: ");
                LogAppend(StdOutput, true);
                return ParseListCommand(ref list, type);
            }
            else
            {
                Log(LoggerType.Error, "DiskpartProcess - ListCommand(List, " + type + ", " + selectedDisk + ")", "Run - " + ExitCode + ": ");
                LogAppend(StdOutput, true);
            }
            return ExitCode;
        }

        /// <summary>
        /// Parses a string to search for Media information
        /// </summary>
        /// <param name="volumes">The List to add to</param>
        /// <returns>The process exit code</returns>
        private ProcessExitCode ParseListCommand(ref List<BaseMedia> list, StorageType type)
        {
            Log(LoggerType.Info, "DiskpartProcess - ParseListCommand(List, " + type + ")", "Called");
            CurrentProcess = nameof(ParseListCommand);
            Regex rx;
            switch (type)
            {
                case StorageType.DISK:
                    rx = new Regex(Disk_Parse_RX);
                    break;
                case StorageType.VOLUME:
                    rx = new Regex(Volume_Parse_RX);
                    break;
                case StorageType.PARTITION:
                    rx = new Regex(Partition_Parse_RX);
                    break;
                default:
                    rx = new Regex(string.Empty);
                    break;
            }

            list = new List<BaseMedia>();

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
                        case StorageType.DISK:
                            b = new Disk
                            {
                                Number = Int32.Parse(gc["disknum"].Value),
                                Status = StatusExtension.Parse(gc["diskstat"].Value),
                                Size = Int32.Parse(gc["disksize"].Value),
                                SizePostfix = SizePostfixExtension.Parse(gc["diskgk"].Value),
                                FreeSpace = Int32.Parse(gc["diskfree"].Value),
                                FreeSpacePostfix = SizePostfixExtension.Parse(gc["diskfreegk"].Value),
                                Dynamic = gc["diskdyn"].Value.Trim(),
                                GPTType = gc["diskgpt"].Value.Trim()
                            };
                            Log(LoggerType.Info, "DiskpartProcess - ParseListCommand(List, " + type + ")", "List Details Added To Disk");
                            break;
                        case StorageType.VOLUME:
                            b = new Volume
                            {
                                Number = Int32.Parse(gc["volnum"].Value),
                                Letter = gc["vollet"].Value.ElementAt<char>(0),
                                Name = gc["vollab"].Value,
                                FileSystem = FileSystemExtension.Parse(gc["volfs"].Value),
                                Type = VolumeTypeExtension.Parse(gc["voltype"].Value),
                                Size = Int32.Parse(gc["volsize"].Value),
                                SizePostfix = SizePostfixExtension.Parse(gc["volgk"].Value),
                                Status = StatusExtension.Parse(gc["volstat"].Value),
                                Info = gc["volinfo"].Value,
                                MountState = MountStateExtension.Parse(gc["volinfo"].Value)
                            };
                            Log(LoggerType.Info, "DiskpartProcess - ParseListCommand(List, " + type + ")", "List Details Added To Volume");
                            break;
                        case StorageType.PARTITION:
                            b = new Partition
                            {
                                Number = Int32.Parse(gc["partnum"].Value),
                                PartitionType = PartitionTypeExtension.Parse(gc["parttype"].Value),
                                Size = Int32.Parse(gc["partsize"].Value),
                                SizePostfix = SizePostfixExtension.Parse(gc["partsizegk"].Value),
                                Offset = Int32.Parse(gc["partoff"].Value),
                                OffsetPostfix = SizePostfixExtension.Parse(gc["partoffgk"].Value)
                            };
                            Log(LoggerType.Info, "DiskpartProcess - ParseListCommand(List, " + type + ")", "List Details Added To Partition");
                            break;
                        default:
                            b = new BaseMedia();
                            break;
                    }
                    list.Add(b);
                }
                ExitCode = ProcessExitCode.Ok;
            }
            else
            {
                ExitCode = ProcessExitCode.ErrorNoMatchesFound;
                Log(LoggerType.Error, "DiskpartProcess - ParseListCommand(List, " + type + ")", "No Matches - " + ExitCode + ": ");
                LogAppend(StdOutput, true);
            }
            return ExitCode;
        }

        /// <summary>
        /// Gets the Read-Only flag of each Media item
        /// </summary>
        /// <param name="list">The Volumes to get the read-only flag</param>
        /// <returns>The process exit code</returns>
        public ProcessExitCode GetAttributes(ref List<BaseMedia> list, StorageType type)
        {
            Log(LoggerType.Info, "DiskpartProcess - GetAttributes(List, " + type + ")", "Called");
            CurrentProcess = nameof(GetAttributes);

            foreach (BaseMedia m in list)
            {
                AddScriptCommand("SELECT " + type + " " + m.Number);
                AddScriptCommand("ATTRIBUTE " + type);
            }
            WriteScript();
            ExitCode = Run();
            if (ExitCode == ProcessExitCode.Ok)
            {
                Log(LoggerType.Info, "DiskpartProcess - GetAttributes(List, " + type + ")", "StdOutput: ");
                LogAppend(StdOutput, true);
                return ParseAttributes(ref list, type);
            }
            else
            {
                ExitCode = ProcessExitCode.ErrorRun;
                Log(LoggerType.Error, "DiskpartProcess - GetAttributes(List, " + type + ")", "Run - " + ExitCode + ": ");
                LogAppend(StdOutput, true);
            }
            return ExitCode;
        }

        /// <summary>
        /// Parses a string to search for Read-Only state
        /// </summary>
        /// <param name="list">The List to set Read-Only state to</param>
        /// <returns>The process exit code</returns>
        private ProcessExitCode ParseAttributes(ref List<BaseMedia> list, StorageType type)
        {
            Log(LoggerType.Info, "DiskpartProcess - ParseAttributes(List, " + type + ")", "Called");
            CurrentProcess = nameof(ParseAttributes);
            if (list != null)
            {
                Regex rx;
                switch (type)
                {
                    case StorageType.DISK:
                        rx = new Regex(Disk_Attribute_Parse_RX);
                        break;
                    case StorageType.VOLUME:
                        rx = new Regex(Volume_Attribute_Parse_RX);
                        break;
                    default:
                        rx = new Regex(string.Empty);
                        break;
                }

                MatchCollection matches = rx.Matches(StdOutput);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        GroupCollection gc = match.Groups;
                        int medianum;
                        switch (type)
                        {
                            case StorageType.DISK:
                                medianum = Int32.Parse(gc["disknum"].Value);

                                if (gc["croflag"].Value == "Yes")
                                    list[medianum].Attributes |= Attributes.CurrentReadOnlyState;
                                if (gc["roflag"].Value == "Yes")
                                    list[medianum].Attributes |= Attributes.ReadOnly;
                                if (gc["bdflag"].Value == "Yes")
                                    list[medianum].Attributes |= Attributes.Boot;
                                if (gc["pfflag"].Value == "Yes")
                                    list[medianum].Attributes |= Attributes.Pagefile;
                                if (gc["hibflag"].Value == "Yes")
                                    list[medianum].Attributes |= Attributes.HibernationFile;
                                if (gc["cdflag"].Value == "Yes")
                                    list[medianum].Attributes |= Attributes.Crashdump;
                                if (gc["clustflag"].Value == "Yes")
                                    list[medianum].Attributes |= Attributes.Cluster;
                                break;
                            case StorageType.VOLUME:
                                medianum = Int32.Parse(gc["volnum"].Value);

                                if (gc["roflag"].Value == "Yes")
                                    list[medianum].Attributes |= Attributes.ReadOnly;

                                if (gc["hidflag"].Value == "Yes")
                                    list[medianum].Attributes |= Attributes.Hidden;

                                if (gc["noddflag"].Value == "Yes")
                                    list[medianum].Attributes |= Attributes.NoDefaultDriveLetter;

                                if (gc["shadflag"].Value == "Yes")
                                    list[medianum].Attributes |= Attributes.ShadowCopy;
                                break;
                            default:

                                break;
                        }

                    }
                    ExitCode = ProcessExitCode.Ok;
                }
                else
                {
                    ExitCode = ProcessExitCode.ErrorNoMatchesFound;
                    Log(LoggerType.Error, "DiskpartProcess - ParseAttributes(List, " + type + ")", "No Matches - " + ExitCode + ": ");
                    LogAppend(StdOutput, true);
                }
            }
            else
            {
                ExitCode = ProcessExitCode.ErrorNullVolumes;
                Log(LoggerType.Error, "DiskpartProcess - ParseAttributes(List, " + type + ")", "Null List - " + ExitCode + ": ");
                LogAppend(StdOutput, true);
            }
            return ExitCode;
        }

        /// <summary>
        /// Gets the Details of a media item
        /// </summary>
        /// <param name="media">The media item to get the details of</param>
        /// <param name="type">The type of media item</param>
        /// <param name="selectedDisk">Used only if getting the details of a partition. The disk the partition is on</param>
        /// <returns>The process exit code</returns>
        public ProcessExitCode DetailCommand(ref BaseMedia media, StorageType type, int selectedDisk = -1)
        {
            Log(LoggerType.Info, "DiskpartProcess - DetailCommand(" + media.Name + ", " + type + ", " + selectedDisk + ")", "Called");
            CurrentProcess = nameof(DetailCommand);

            if (type == StorageType.PARTITION)
                AddScriptCommand("SELECT DISK " + selectedDisk);

            AddScriptCommand("SELECT " + type + " " + media.Number);
            AddScriptCommand("DETAIL " + type);
            WriteScript();
            ExitCode = Run();
            if (ExitCode == ProcessExitCode.Ok)
            {
                Log(LoggerType.Info, "DiskpartProcess - DetailCommand(" + media.Name + ", " + type + ", " + selectedDisk + ")", "StdOutput: ");
                LogAppend(StdOutput, true);
                return ParseDetailCommand(ref media, type);
            }
            else
            {
                ExitCode = ProcessExitCode.ErrorRun;
                Log(LoggerType.Error, "DiskpartProcess - DetailCommand(" + media.Name + ", " + type + ", " + selectedDisk + ")", "Run - " + ExitCode + ": ");
                LogAppend(StdOutput, true);
            }

            return ExitCode;
        }

        /// <summary>
        /// Parses a string to search for details of a media item
        /// </summary>
        /// <param name="media">The media item to populate the details of</param>
        /// <param name="type">The type of media item</param>
        /// <returns>The process exit code</returns>
        public ProcessExitCode ParseDetailCommand(ref BaseMedia media, StorageType type)
        {
            Log(LoggerType.Info, "DiskpartProcess - ParseDetailCommand(" + media.Number + ", " + type + ")", "Called");
            CurrentProcess = nameof(ParseDetailCommand);

            Regex rx;
            switch (type)
            {
                case StorageType.DISK:
                    rx = new Regex(Disk_Detail_RX);
                    break;
                case StorageType.PARTITION:
                    rx = new Regex(Partition_Detail_RX);
                    break;
                case StorageType.VOLUME:
                    rx = new Regex(Volume_Detail_RX);
                    break;
                default:
                    rx = new Regex(string.Empty);
                    ExitCode = ProcessExitCode.ErrorNullRegex;
                    break;
            }

            MatchCollection matches = rx.Matches(StdOutput);
            if (matches.Count > 0)
            {
                foreach (Match m in matches)
                {
                    GroupCollection gc = m.Groups;
                    switch (type)
                    {
                        case StorageType.DISK:
                            media.Name = gc["diskname"].Value.Trim();
                            ((Disk)media).DiskID = gc["diskid"].Value;
                            ((Disk)media).Type = DiskTypeExtension.Parse(gc["disktype"].Value);
                            media.Status = StatusExtension.Parse(gc["diskstatus"].Value);
                            ((Disk)media).Path = gc["diskpath"].Value;
                            ((Disk)media).Target = gc["disktarget"].Value;
                            ((Disk)media).LunID = gc["disklunid"].Value;
                            ((Disk)media).LocationPath = gc["disklocpath"].Value;
                            if (gc["diskcurreadonly"].Value == "Yes")
                                media.SetFlag(Attributes.CurrentReadOnlyState);
                            if (gc["diskreadonly"].Value == "Yes")
                                media.SetFlag(Attributes.ReadOnly);
                            if (gc["diskboot"].Value == "Yes")
                                media.SetFlag(Attributes.Boot);
                            if (gc["diskpage"].Value == "Yes")
                                media.SetFlag(Attributes.Pagefile);
                            if (gc["diskhibernation"].Value == "Yes")
                                media.SetFlag(Attributes.HibernationFile);
                            if (gc["diskcrashdump"].Value == "Yes")
                                media.SetFlag(Attributes.Crashdump);
                            if (gc["diskcluster"].Value == "Yes")
                                media.SetFlag(Attributes.Cluster);
                            Log(LoggerType.Info, "DiskpartProcess - ParseDetailCommand(" + media.Number + ", " + type + ")", "Details Added To Disk");
                            LogDisk((Disk)media);
                            break;
                        case StorageType.PARTITION:
                            ((Partition)media).Type = gc["parttype"].Value;
                            if (gc["parthidden"].Value == "Yes")
                                media.SetFlag(Attributes.Hidden);
                            if (gc["partactive"].Value == "Yes")
                                media.SetFlag(Attributes.Active);
                            ((Partition)media).OffsetInBytes = gc["partoffsetbyte"].Value;
                            Log(LoggerType.Info, "DiskpartProcess - ParseDetailCommand(" + media.Number + ", " + type + ")", "Details Added To Partition");
                            LogPartition((Partition)media);
                            break;
                        case StorageType.VOLUME:
                            if (gc["volreadonly"].Value == "Yes")
                                media.SetFlag(Attributes.ReadOnly);
                            if (gc["volhidden"].Value == "Yes")
                                media.SetFlag(Attributes.Hidden);
                            if (gc["volnodefletter"].Value == "Yes")
                                media.SetFlag(Attributes.NoDefaultDriveLetter);
                            if (gc["volshadow"].Value == "Yes")
                                media.SetFlag(Attributes.ShadowCopy);
                            if (gc["voloffline"].Value == "Yes")
                                media.SetFlag(Attributes.Offline);
                            if (gc["volbitlock"].Value == "Yes")
                                media.SetFlag(Attributes.BitLocker);
                            if (gc["volinstall"].Value == "Yes")
                                media.SetFlag(Attributes.Installable);
                            media.Parent = Int32.Parse(gc["parentnumber"].Value);
                            ((Volume)media).Capacity = Int32.Parse(gc["volcap"].Value);
                            ((Volume)media).CapacityPostfix = SizePostfixExtension.Parse(gc["volcapgk"].Value);
                            ((Volume)media).FreeSpace = Int32.Parse(gc["volfree"].Value);
                            ((Volume)media).FreeSpacePostfix = SizePostfixExtension.Parse(gc["volfreegk"].Value);
                            Log(LoggerType.Info, "DiskpartProcess - ParseDetailCommand(" + media.Number + ", " + type + ")", "Details Added To Volume");
                            LogVolume((Volume)media);
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                ExitCode = ProcessExitCode.ErrorNoMatchesFound;
                Log(LoggerType.Error, "DiskpartProcess - ParseDetailCommand(" + media.Name + ", " + type + ")", "No Matches - " + ExitCode + ": ");
                LogAppend(StdOutput, true);
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
            Log(LoggerType.Info, "DiskpartProcess - EjectVolume(" + b.Name + ")", "Called");
            CurrentProcess = nameof(EjectVolume);
            if (b is Disk)
            { // || b is Partition
                ExitCode = ProcessExitCode.ErrorInvalidMediaType;
                Log(LoggerType.Error, "DiskpartProcess - EjectVolume(" + b.Name + ")", "Invalid Media - " + ExitCode);
            }
            else
            {
                AddScriptCommand("SELECT VOLUME " + b.Number);
                AddScriptCommand("REMOVE ALL DISMOUNT");
                WriteScript();
                ExitCode = Run();
                if (ExitCode == ProcessExitCode.Ok)
                {
                    Log(LoggerType.Info, "DiskpartProcess - EjectVolume(" + b.Name + ")", "StdOutput: ");
                    LogAppend(StdOutput, true);
                    if (TestOutput(@"DiskPart successfully dismounted and offlined the volume."))
                    {
                        Log(LoggerType.Info, "DiskpartProcess - EjectVolume(" + b.Name + ")", "TestOutput - StdOutput: ");
                        LogAppend(StdOutput, true);
                        ExitCode = ProcessExitCode.Ok;
                    }
                    else
                    {
                        ExitCode = ProcessExitCode.ErrorTestOutput;
                        Log(LoggerType.Error, "DiskpartProcess - EjectVolume(" + b.Name + ")", "TestOutput - " + ExitCode + ": ");
                        LogAppend(StdOutput, true);
                    }
                }
                else
                {
                    ExitCode = ProcessExitCode.ErrorRun;
                    Log(LoggerType.Error, "DiskpartProcess - EjectVolume(" + b.Name + ")", "Run - " + ExitCode + ": ");
                    LogAppend(StdOutput, true);
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
            Log(LoggerType.Info, "DiskpartProcess - AssignVolumeLetter(" + b.Name + ")", "Called");
            CurrentProcess = nameof(AssignVolumeLetter);
            if (b is Disk) // || b is Partition
            {
                ExitCode = ProcessExitCode.ErrorInvalidMediaType;
                Log(LoggerType.Error, "DiskpartProcess - AssignVolumeLetter(" + b.Name + ")", "Invalid Media - " + ExitCode);
            }
            else
            {
                if (!b.IsMounted())
                {
                    AddScriptCommand("SELECT VOLUME " + b.Number);
                    AddScriptCommand("ASSIGN");
                    WriteScript();
                    ExitCode = Run();
                    if (ExitCode == ProcessExitCode.Ok)
                    {
                        Log(LoggerType.Info, "DiskpartProcess - AssignVolumeLetter(" + b.Name + ")", "StdOutput: ");
                        LogAppend(StdOutput, true);
                        if (TestOutput("DiskPart successfully assigned the drive letter or mount point."))
                        {
                            ExitCode = ProcessExitCode.Ok;
                            Log(LoggerType.Info, "DiskpartProcess - AssignVolumeLetter(" + b.Name + ")", "TestOutput - StdOutput: ");
                            LogAppend(StdOutput, true);
                        }
                        else
                        {
                            ExitCode = ProcessExitCode.ErrorTestOutput;
                            Log(LoggerType.Error, "DiskpartProcess - AssignVolumeLetter(" + b.Name + ")", "TestOutput - " + ExitCode + ": ");
                            LogAppend(StdOutput, true);
                        }
                    }
                    else
                    {
                        ExitCode = ProcessExitCode.ErrorRun;
                        Log(LoggerType.Error, "DiskpartProcess - AssignVolumeLetter(" + b.Name + ")", "Run - " + ExitCode + ": ");
                        LogAppend(StdOutput, true);
                    }
                }
                else
                {
                    ExitCode = ProcessExitCode.ErrorVolumeNotMounted;
                    Log(LoggerType.Error, "DiskpartProcess - AssignVolumeLetter(" + b.Name + ")", "Volume Not Mounted - " + ExitCode);
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
        public ProcessExitCode SetReadOnly(BaseMedia b, ReadOnlyFunction function, StorageType type)
        {
            Log(LoggerType.Info, "DiskpartProcess - SetReadOnly(" + b.Name + ", " + function + ", " + type + ")", "Called");
            CurrentProcess = nameof(SetReadOnly);

            // Check for Partition type

            if (!b.CanToggleReadOnly())
            {
                ExitCode = ProcessExitCode.ErrorInvalidMediaType;
                Log(LoggerType.Error, "DiskpartProcess - SetReadOnly(" + b.Name + ", " + function + ", " + type + ")", "Invalid Media Type - " + ExitCode);
            }
            else
            {
                AddScriptCommand("SELECT " + type + " " + b.Number);
                AddScriptCommand("ATTRIBUTE " + type + " " + function + " READONLY");
                WriteScript();
                ExitCode = Run();
                if (ExitCode == ProcessExitCode.Ok)
                {
                    Log(LoggerType.Info, "DiskpartProcess - SetReadOnly(" + b.Name + ", " + function + ", " + type + ")", "StdOutput: ");
                    LogAppend(StdOutput, true);
                    string rx;
                    switch (type)
                    {
                        case StorageType.DISK:
                            rx = Disk_ReadOnly_Test_RX;
                            break;
                        case StorageType.VOLUME:
                            rx = Volume_ReadOnly_Test_RX;
                            break;
                        default:
                            rx = string.Empty;
                            break;
                    }
                    if (TestOutput(rx))
                    {
                        ExitCode = ProcessExitCode.Ok;
                        Log(LoggerType.Info, "DiskpartProcess - SetReadOnly(" + b.Name + ", " + function + ", " + type + ")", "TestOutput - StdOutput: ");
                        LogAppend(StdOutput, true);
                    }
                    else
                    {
                        ExitCode = ProcessExitCode.ErrorTestOutput;
                        Log(LoggerType.Error, "DiskpartProcess - SetReadOnly(" + b.Name + ", " + function + ", " + type + ")", "TestOutput - " + ExitCode + ": ");
                        LogAppend(StdOutput, true);
                    }
                }
                else
                {
                    ExitCode = ProcessExitCode.ErrorRun;
                    Log(LoggerType.Error, "DiskpartProcess - SetReadOnly(" + b.Name + ", " + function + ", " + type + ")", "Run - " + ExitCode + ": ");
                    LogAppend(StdOutput, true);
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
            Log(LoggerType.Info, "DiskpartProcess - GetFileSystemInfo(" + v.Name + ", List, List)", "Called");
            CurrentProcess = nameof(GetFileSystemInfo);

            // Check for Partition type

            AddScriptCommand("SELECT VOLUME " + v.Number);
            AddScriptCommand("FILESYSTEM");
            WriteScript();
            ExitCode = Run();
            if (ExitCode == ProcessExitCode.Ok)
            {
                Log(LoggerType.Info, "DiskpartProcess - GetFileSystemInfo(" + v.Name + ", List, List)", "StdOutput: ");
                LogAppend(StdOutput, true);
                return ParseFileSystemInfo(ref fs, ref us);
            }
            else
            {
                ExitCode = ProcessExitCode.ErrorRun;
                Log(LoggerType.Error, "DiskpartProcess - GetFileSystemInfo(" + v.Name + ", List, List)", "Run - " + ExitCode + ": ");
                LogAppend(StdOutput, true);
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
            Log(LoggerType.Info, "DiskpartProcess - ParseFileSystemInfo(List, List)", "Called");
            CurrentProcess = nameof(ParseFileSystemInfo);

            if (fs == null)
            {
                ExitCode = ProcessExitCode.ErrorFileSystemNull;
                Log(LoggerType.Error, "DiskpartProcess - ParseFileSystemInfo(List, List)", "File System List Null - " + ExitCode);
                return ExitCode;
            }

            if (us == null)
            {
                ExitCode = ProcessExitCode.ErrorUnitSizeNull;
                Log(LoggerType.Error, "DiskpartProcess - ParseFileSystemInfo(List, List)", "Unit Size List Null - " + ExitCode);
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
            {
                ExitCode = ProcessExitCode.ErrorNoMatchesFound;
                Log(LoggerType.Error, "DiskpartProcess - ParseFileSystemInfo(List, List)", "No Matches Found - " + ExitCode + ": ");
                LogAppend(StdOutput, true);
            }

            return ExitCode;

        }

        /// <summary>
        /// Formats a Volume with given FormatArguments
        /// </summary>
        /// <param name="b">The Volume to format</param>
        /// <param name="fa">The arguments of the format</param>
        /// <returns></returns>
        public ProcessExitCode Format(BaseMedia b, FormatArguments fa)
        {
            Log(LoggerType.Info, "DiskpartProcess - Format(" + b.Name + ", FormatArguments)", "Called");
            CurrentProcess = nameof(Format);

            if (b == null)
            {
                ExitCode = ProcessExitCode.ErrorNullVolumes;
                Log(LoggerType.Error, "DiskpartProcess - Format(" + b.Name + ", FormatArguments)", "Invalid Volumes - " + ExitCode);
                return ExitCode;
            }

            if (b is Disk)
            {
                ExitCode = ProcessExitCode.ErrorInvalidMediaType;
                Log(LoggerType.Error, "DiskpartProcess - Format(" + b.Name + ", FormatArguments)", "Invalid Media Type - " + ExitCode);
                return ExitCode;
            }

            AddScriptCommand("SELECT VOLUME " + b.Number);
            AddScriptCommand("FORMAT " + fa.GetArguments());
            WriteScript();
            ExitCode = Run();
            if (ExitCode == ProcessExitCode.Ok)
            {
                Log(LoggerType.Info, "DiskpartProcess - Format(" + b.Name + ", FormatArguments)", "StdOutput: ");
                LogAppend(StdOutput, true);
                if (TestOutput("DiskPart successfully formatted the volume"))
                {
                    ExitCode = ProcessExitCode.Ok;
                    Log(LoggerType.Info, "DiskpartProcess - Format(" + b.Name + ", FormatArguments)", "TestOutput - StdOutput: ");
                    LogAppend(StdOutput, true);
                }
                else
                {
                    ExitCode = ProcessExitCode.ErrorTestOutput;
                    Log(LoggerType.Error, "DiskpartProcess - Format(" + b.Name + ", FormatArguments)", "Testoutput - " + ExitCode + ": ");
                    LogAppend(StdOutput, true);
                }
            }
            else
            {
                ExitCode = ProcessExitCode.ErrorRun;
                Log(LoggerType.Error, "DiskpartProcess - Format(" + b.Name + ", FormatArguments)", "Run - " + ExitCode + ": " );
                LogAppend(StdOutput, true);
            }

            return ExitCode;
        }

    }
}
