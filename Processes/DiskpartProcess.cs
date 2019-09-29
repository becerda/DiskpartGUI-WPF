using DiskpartGUI.Helpers;
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
        // Diskpart Script File Name
        private const string Diskpart_Script_Filename = "diskpart_script.txt";
        // Diskpart Command Line Arguments
        private const string Diskpart_Script_Command_Arg = "/s";

        // Diskpart Script Command Lines
        private const string Script_Line_Select = "SELECT ";
        private const string Script_Line_Select_Disk = Script_Line_Select + "DISK ";
        private const string Script_Line_Select_Volume = Script_Line_Select + "VOLUME ";
        private const string Script_Line_Remove_All_Dismount = "REMOVE ALL DISMOUNT";
        private const string Script_Line_List = "LIST ";
        private const string Script_Line_Attribute = "ATTRIBUTE ";
        private const string Script_Line_ReadOnly = " READONLY";
        private const string Script_Line_Detail = "DETAIL ";
        private const string Script_Line_Assign = "ASSIGN";
        private const string Script_Line_FileSystem = "FILESYSTEM";
        private const string Script_Line_Format = "FORMAT ";

        // BaseMedia Regex Identifiers
        private const string RX_BaseMedia_Name = "basemedianame";
        private const string RX_BaseMedia_Number = "basemedianumber";
        private const string RX_BaseMedia_Status = "basemediastatus";
        private const string RX_BaseMedia_Size = "basemediasize";
        private const string RX_BaseMedia_SizePostfix = "basemediapostfix";

        // Multi Media Regex Identifiers
        private const string RX_Media_FreeSpace = "mediafreespace";
        private const string RX_Media_FreeSpacePostfix = "mediafreespacepostfix";
        private const string RX_Media_Hidden = "hiddenflag";
        private const string RX_Media_ReadOnly = "readonlyflag";
        private const string RX_Media_ParentNumber = "parentnumber";

        // Disk Regex Identifiers
        private const string RX_Disk_BootDisk = "bootdiskflag";
        private const string RX_Disk_ClusteredDisk = "clustereddiskflag";
        private const string RX_Disk_CrashDumpDisk = "crashdumpdiskflag";
        private const string RX_Disk_CurrentReadOnly = "currentreadonlyflag";
        private const string RX_Disk_Dynamic = "diskdyn";
        private const string RX_Disk_GPT = "diskgpt";
        private const string RX_Disk_HibernationFileDisk = "hibernationfilediskflag";
        private const string RX_Disk_DiskID = "diskid";
        private const string RX_Disk_PagefileDisk = "pagefilediskflag";
        private const string RX_Disk_Type = "disktype";
        private const string RX_Disk_Path = "diskpath";
        private const string RX_Disk_Target = "disktarget";
        private const string RX_Disk_LunID = "disklunid";
        private const string RX_Disk_LocationPath = "disklocationpath";

        // Volume Regex Identifiers
        private const string RX_Volume_BitLocker = "volumebitlocker";
        private const string RX_Volume_Letter = "volumeletter";
        private const string RX_Volume_FileSystem = "volumefilesystem";
        private const string RX_Volume_Type = "volumetype";
        private const string RX_Volume_Info = "volumeinfo";
        private const string RX_Volume_Installable = "volumeinstallable";
        private const string RX_Volume_Offline = "volumeoffline";
        private const string RX_Volume_NoDefaultDriveLetter = "nodefaultdriveletterflag";
        private const string RX_Volume_ShadowCopy = "shadowcopyflag";

        // Partition Regex Identifiers
        private const string RX_Partition_Type = "partitiontype";
        private const string RX_Partition_Offset = "partitionoffset";
        private const string RX_Partition_OffsetInBytes = "partitionoffsetinbytes";
        private const string RX_Partition_OffsetPostfix = "partitionoffsetpostfix";
        private const string RX_Partition_Active = "partitionoactiveflag";


        //Parsing Regex
        //language=regex
        private const string RX_Disk_List = "Disk (?<basemedianumber>[0-9]+){1,2}( ){3,4}(?<basemediastatus>Online| )?( ){0,15}(?<basemediasize>[0-9]{1,4})?( )(?<basemediapostfix>K|G|M)?B( ){2,6}(?<mediafreespace>[0-9]{1,4})?( )(?<mediafreespacepostfix>K|G|M)?B( ){2}( ){2}(?<diskdyn>[ a-zA-Z]{3})?( ){2}(?<diskgpt>[ *a-zA-Z]{3})?";
        //language=regex
        private const string RX_Volume_List = "Volume (?<basemedianumber>[0-9]+){1,2}( ){4,5}(?<volumeletter>[A-Z ])( ){0,3}(?<basemedianame>[a-zA-Z0-9_ ]{0,11})( ){2,3}(?<volumefilesystem>NTFS|FAT32|exFAT|CDFS|UDF|RAW)?( ){2,7}(?<volumetype>Partition|Removable|DVD-ROM|Simple)?( ){3,14}(?<basemediasize>[0-9]{1,4})?( )(?<basemediapostfix>K|G|M)?B( ){2}(?<basemediastatus>Healthy|No Media)?( ){0,11}(?<volumeinfo>[a-zA-Z]+)?";
        //language=regex
        private const string RX_Partition_List = "(There are no partitions on this disk to show\\.|Partition (?<basemedianumber>[0-9]+)( ){3,4}(?<partitiontype>Primary|Extended|Logical)( ){10,19}(?<basemediasize>[0-9]+)( )(?<basemediapostfix>K|G|M)?B( ){2,4}(?<partitionoffset>[0-9]+)( )(?<partitionoffsetpostfix>K|G|M)?B)";

        //language=regex
        private const string Disk_Attribute_Parse_RX = "Disk (?<basemedianumber>[0-9]+) is now the selected disk\\.\\r\\nCurrent Read-only State : (?<currentreadonlyflag>Yes|No)\\r\\nRead-only( )+: (?<readonlyflag>Yes|No)\\r\\nBoot Disk ( )+: (?<bootdiskflag>Yes|No)\\r\\nPagefile Disk( )+: (?<pagefilediskflag>Yes|No)\\r\\nHibernation File Disk( )+: (?<hibernationfilediskflag>Yes|No)\\r\\nCrashdump Disk( )+: (?<crashdumpdiskflag>Yes|No)\\r\\nClustered Disk( )+: (?<clustereddiskflag>Yes|No)";
        //language=regex
        private const string RX_Volume_Attribute_Parse = "Volume (?<basemedianumber>[0-9]+) is the selected volume\\.\\r\\nRead-only( )+: (?<readonlyflag>Yes|No)\\r\\nHidden( )+: (?<hiddenflag>Yes|No)\\r\\nNo Default Drive Letter: (?<nodefaultdriveletterflag>Yes|No)\\r\\nShadow Copy( )+: (?<shadowcopyflag>Yes|No)";

        //language=regex
        private const string RX_Disk_Detail = "\\r\\n(?<basemedianame>[a-zA-Z0-9_ ]+)\\r\\nDisk ID: (?<diskid>[A-F0-9]+)\\r\\nType( )+: (?<disktype>USB|RAID)\\r\\nStatus : (?<basemediastatus>Online|Offline)\\r\\nPath( )+: (?<diskpath>[a-zA-Z0-9]+)\\r\\nTarget : (?<disktarget>[a-zA-Z0-9]+)\\r\\nLUN ID : (?<disklunid>[a-zA-Z0-9]+)\\r\\nLocation Path : (?<disklocationpath>[()#a-zA-Z0-9]+)\\r\\nCurrent Read-only State : (?<currentreadonlyflag>Yes|No)\\r\\nRead-only( )+: (?<readonlyflag>Yes|No)\\r\\nBoot Disk( )+: (?<bootdiskflag>Yes|No)\\r\\nPagefile Disk( )+: (?<pagefilediskflag>Yes|No)\\r\\nHibernation File Disk( )+: (?<hibernationfilediskflag>Yes|No)\\r\\nCrashdump Disk( )+: (?<crashdumpdiskflag>Yes|No)\\r\\nClustered Disk( )+: (?<clustereddiskflag>Yes|No)";
        //language=regex
        private const string RX_Volume_Detail = "(There are no disks attached to this volume.|Disk (?<parentnumber>[0-9]+)( )+(Online|)?( )+([0-9]+)( )(K|G|M)?B( ){2,}([0-9]+)( )(M|G|K)?B( |\\*)+)(\\r\\n\\r\\n)?(Read-only( )+: (?<readonlyflag>Yes|No)\\r\\n)?(Hidden( )+: (?<hiddenflag>Yes|No)\\r\\n)?(No Default Drive Letter: (?<nodefaultdriveletterflag>Yes|No)\\r\\n)?(Shadow Copy( )+: (?<shadowcopyflag>Yes|No)\\r\\n)?(Offline( )+: (?<volumeoffline>Yes|No)\\r\\n)?(BitLocker Encrypted( )+: (?<volumebitlocker>Yes|No)\\r\\n)?(Installable( )+: (?<volumeinstallable>Yes|No)(\\r\\n\\r\\n)?(Volume Capacity( )+:( )+(?<basemediasize>[0-9]+) (?<basemediapostfix>M|K|G)?B\\r\\nVolume Free Space( )+:( )+(?<mediafreespace>[0-9]+) (?<mediafreespacepostfix>M|K|G)?B)?)?";
        //language=regex
        private const string RX_Partition_Detail = "\\r\\nPartition (?<basemedianumber>[0-9]+)\\r\\nType  : (?<partitiontype>[A-F0-9]+)\\r\\nHidden: (?<hiddenflag>Yes|No)\\r\\nActive: (?<partitionoactiveflag>Yes|No)\\r\\nOffset in Bytes: (?<partitionoffsetinbytes>[0-9]+)\\r\\n\\r\\n(There is no volume associated with this partition.|  Volume ###  Ltr  Label( ){8}Fs( ){5}Type( ){8}Size( ){5}Status( ){5}Info\\r\\n  -{10}  -{3}  -{11}  -{5}  -{10}  -{7}  -{9}  -{8}\\r\\n. Volume (?<volnum>[0-9]+){1,2}( ){4,5}(?<vollet>[A-Z ])( ){0,3}(?<vollab>[a-zA-Z0-9 ]{0,11})( ){2,3}(?<volfs>NTFS|FAT32|exFAT|CDFS|UDF|RAW)?( ){2,7}(?<voltype>Partition|Removable|DVD-ROM|Simple)?( ){3,14}(?<volsize>[0-9]{1,4})?( )(?<volgk>K|G|M)?B( ){2}(?<volstat>Healthy|No Media)?( ){0,11}(?<volinfo>[a-zA-Z]+)?)";

        // Log Method Names
        private const string Nameof_DiskpartProcess = nameof(DiskpartProcess);
        private const string WriteScript_Log_MethodName = Nameof_DiskpartProcess + " - " + nameof(WriteScript);
        private const string GetDiskInfo_Log_MethodName = Nameof_DiskpartProcess + " - " + nameof(GetDiskInfo) + "(List)";
        private const string GetVolumeInfo_Log_MethodName = Nameof_DiskpartProcess + " - " + nameof(GetVolumeInfo) + "(List)";
        private const string GetInfo_Log_MethodName = Nameof_DiskpartProcess + " - " + nameof(GetInfo);
        private const string GetPartitionInfo_Log_MethodName = Nameof_DiskpartProcess + " - " + nameof(GetPartitionInfo);
        private const string ListCommand_Log_MethodName = Nameof_DiskpartProcess + " - " + nameof(ListCommand);
        private const string ParseListCommand_Log_MethodName = Nameof_DiskpartProcess + " - " + nameof(ParseListCommand);
        private const string GetAttributes_Log_MethodName = Nameof_DiskpartProcess + " - " + nameof(GetAttributes);
        private const string ParseAttributes_Log_MethodName = Nameof_DiskpartProcess + " - " + nameof(ParseAttributes);
        private const string DetailCommand_Log_MethodName = Nameof_DiskpartProcess + " - " + nameof(DetailCommand);
        private const string EjectVolume_Log_MethodName = Nameof_DiskpartProcess + " - " + nameof(EjectVolume);
        private const string AssignVolumeLetter_Log_MethodName = Nameof_DiskpartProcess + " - " + nameof(AssignVolumeLetter);
        private const string SetReadOnly_Log_MethodName = Nameof_DiskpartProcess + " - " + nameof(SetReadOnly);
        private const string GetFileSystemInfo_Log_MethodName = Nameof_DiskpartProcess + " - " + nameof(GetFileSystemInfo);
        private const string ParseFileSystemInfo_Log_MethodName = Nameof_DiskpartProcess + " - " + nameof(ParseFileSystemInfo) + "(List, List)";
        private const string Format_Log_MethodName = Nameof_DiskpartProcess + " - " + nameof(Format);

        // Other Log Strings
        private const string ParseListCommand_Log_ListDetailsAdded = "List Details Added To ";
        private const string DetailCommand_Log_DetailsAdded = "Details Added To ";

        // Test Output Regex
        //language=regex
        private const string Disk_ReadOnly_Test_RX = "Disk attributes (set|cleared) successfully.";
        //language=regex
        private const string Volume_ReadOnly_Test_RX = "Volume attributes (set|cleared) successfully.";
        private const string EjectVolume_TestOutput = "DiskPart successfully dismounted and offlined the volume.";
        private const string AssignVolumeLetter_TestOutput = "DiskPart successfully assigned the drive letter or mount point.";
        private const string Format_TestOutput = "DiskPart successfully formatted the volume";
        private const string Partition_List_TestOuput = "There are no partitions on this disk to show";
        private const string FLAG_ENABLED = "Yes";


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
            Log.Info("DiskpartProcess - AddScriptCommand(string)", "\"" + line + "\" added to script");
            script.Add(line);
        }

        /// <summary>
        /// Writes the script out to a temp file
        /// </summary>
        private void WriteScript()
        {
            Log.MethodCall(WriteScript_Log_MethodName);
            try
            {
                if (!File.Exists(file))
                {
                    file = Path.GetTempPath() + Diskpart_Script_Filename;
                }

                File.WriteAllLines(file, script);

                AddArgument(Diskpart_Script_Command_Arg);
                AddArgument(file);
                script.Clear();
            }
            catch (IOException e)
            {
                Log.Error(WriteScript_Log_MethodName, StdError);
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
            Log.MethodCall(GetDiskInfo_Log_MethodName);
            return GetInfo(ref disks, StorageType.DISK);
        }

        /// <summary>
        /// Gets the list and details of all volumes present on a machine
        /// </summary>
        /// <param name="volumes">The list of volumes to populate</param>
        /// <returns>The process exit code</returns>
        public ProcessExitCode GetVolumeInfo(ref List<BaseMedia> volumes)
        {
            Log.MethodCall(GetVolumeInfo_Log_MethodName);
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
            Log.MethodCall(GetInfo_Log_MethodName + "(List, " + type + ")");
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
                        Log.Error(GetInfo_Log_MethodName + "(List, " + type + ")", nameof(ListCommand) + " - " + ExitCode + ": ");
                        Log.Append(StdOutput, true);
                        break;
                    }
                }
            }
            else
            {
                Log.Error(GetInfo_Log_MethodName + "(List, " + type + ")", nameof(DetailCommand) + " - " + ExitCode + ": ");
                Log.Append(StdOutput, true);
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
            Log.MethodCall(GetPartitionInfo_Log_MethodName + "(" + disknumber + ", List)");
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
                        partition.Parent = disknumber + "";
                    else
                    {
                        Log.Error(GetPartitionInfo_Log_MethodName + "(" + disknumber + ", List)", nameof(DetailCommand) + " - " + ExitCode + ": ");
                        Log.Append(StdOutput, true);
                        break;
                    }
                }
            }
            else if (ExitCode == ProcessExitCode.NoPartitions)
            {
                ExitCode = ProcessExitCode.Ok;
            }
            else
            {
                Log.Error(GetPartitionInfo_Log_MethodName + "(" + disknumber + ", List)", nameof(ListCommand) + " - " + ExitCode + ": ");
                Log.Append(StdOutput, true);
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
            Log.MethodCall(ListCommand_Log_MethodName + "(List, " + type + ", " + selectedDisk + ")");
            CurrentProcess = nameof(ListCommand);
            if (type == StorageType.PARTITION)
                AddScriptCommand(Script_Line_Select_Disk + selectedDisk);

            AddScriptCommand(Script_Line_List + type);
            WriteScript();
            ExitCode = Run();
            if (ExitCode == ProcessExitCode.Ok)
            {

                Log.Info(ListCommand_Log_MethodName + "(List, " + type + ", " + selectedDisk + ")", nameof(StdOutput) + ": ");
                Log.Append(StdOutput, true);
                return ParseListCommand(ref list, type);
            }
            else
            {
                Log.Error(ListCommand_Log_MethodName + "(List, " + type + ", " + selectedDisk + ")", nameof(Run) + " - " + ExitCode + ": ");
                Log.Append(StdOutput, true);
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
            Log.MethodCall(ParseListCommand_Log_MethodName + "(List, " + type + ")");
            CurrentProcess = nameof(ParseListCommand);
            Regex rx;
            switch (type)
            {
                case StorageType.DISK:
                    rx = new Regex(RX_Disk_List);
                    break;
                case StorageType.VOLUME:
                    rx = new Regex(RX_Volume_List);
                    break;
                case StorageType.PARTITION:
                    if (TestOutput(Partition_List_TestOuput))
                    {
                        Log.Info(ParseListCommand_Log_MethodName + "(List, " + type + ")", Partition_List_TestOuput);
                        ExitCode = ProcessExitCode.NoPartitions;
                        return ExitCode;
                    }
                    rx = new Regex(RX_Partition_List);
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
                            b = new Disk();
                            if (gc[RX_BaseMedia_Number].Success)
                                b.Number = Int32.Parse(gc[RX_BaseMedia_Number].Value);
                            if (gc[RX_BaseMedia_Status].Success)
                                b.Status = StatusExtension.Parse(gc[RX_BaseMedia_Status].Value);
                            if (gc[RX_BaseMedia_Size].Success)
                                b.Size = Int32.Parse(gc[RX_BaseMedia_Size].Value);
                            if (gc[RX_BaseMedia_SizePostfix].Success)
                                b.SizePostfix = SizePostfixExtension.Parse(gc[RX_BaseMedia_SizePostfix].Value);
                            if (gc[RX_BaseMedia_Number].Success)
                                ((Disk)b).FreeSpace = Int32.Parse(gc[RX_Media_FreeSpace].Value);
                            if (gc[RX_BaseMedia_Number].Success)
                                ((Disk)b).FreeSpacePostfix = SizePostfixExtension.Parse(gc[RX_Media_FreeSpacePostfix].Value);
                            if (gc[RX_BaseMedia_Number].Success)
                                ((Disk)b).Dynamic = gc[RX_Disk_Dynamic].Value.Trim();
                            if (gc[RX_BaseMedia_Number].Success)
                                ((Disk)b).GPTType = gc[RX_Disk_GPT].Value.Trim();
                            break;
                        case StorageType.VOLUME:
                            b = new Volume();
                            if (gc[RX_BaseMedia_Name].Success)
                                b.Name = gc[RX_BaseMedia_Name].Value;
                            if (gc[RX_BaseMedia_Number].Success)
                                b.Number = Int32.Parse(gc[RX_BaseMedia_Number].Value);
                            if (gc[RX_BaseMedia_Size].Success)
                                b.Size = Int32.Parse(gc[RX_BaseMedia_Size].Value);
                            if (gc[RX_BaseMedia_SizePostfix].Success)
                                b.SizePostfix = SizePostfixExtension.Parse(gc[RX_BaseMedia_SizePostfix].Value);
                            if (gc[RX_BaseMedia_Status].Success)
                                b.Status = StatusExtension.Parse(gc[RX_BaseMedia_Status].Value);
                            if (gc[RX_Volume_Letter].Success)
                                ((Volume)b).Letter = gc[RX_Volume_Letter].Value.ElementAt<char>(0);
                            if (gc[RX_Volume_FileSystem].Success)
                                ((Volume)b).FileSystem = FileSystemExtension.Parse(gc[RX_Volume_FileSystem].Value);
                            if (gc[RX_Volume_Type].Success)
                                ((Volume)b).Type = VolumeTypeExtension.Parse(gc[RX_Volume_Type].Value);
                            if (gc[RX_Volume_Info].Success)
                            {
                                ((Volume)b).Info = gc[RX_Volume_Info].Value;
                                ((Volume)b).MountState = MountStateExtension.Parse(gc[RX_Volume_Info].Value);
                            }
                            break;
                        case StorageType.PARTITION:
                            b = new Partition();
                            try
                            {
                                if (gc[RX_BaseMedia_Number].Success)
                                    b.Number = Int32.Parse(gc[RX_BaseMedia_Number].Value);
                                if (gc[RX_BaseMedia_Size].Success)
                                    b.Size = Int32.Parse(gc[RX_BaseMedia_Size].Value);
                                if (gc[RX_BaseMedia_SizePostfix].Success)
                                    b.SizePostfix = SizePostfixExtension.Parse(gc[RX_BaseMedia_SizePostfix].Value);
                                if (gc[RX_Partition_Type].Success)
                                    ((Partition)b).PartitionType = PartitionTypeExtension.Parse(gc[RX_Partition_Type].Value);
                                if (gc[RX_Partition_Offset].Success)
                                    ((Partition)b).Offset = Int32.Parse(gc[RX_Partition_Offset].Value);
                                if (gc[RX_Partition_OffsetPostfix].Success)
                                    ((Partition)b).OffsetPostfix = SizePostfixExtension.Parse(gc[RX_Partition_OffsetPostfix].Value);

                            }
                            catch (FormatException fe)
                            {
                                Log.Error(ParseListCommand_Log_MethodName + "(List, " + type + ")", fe.StackTrace);
                                ExitCode = ProcessExitCode.ErrorMatchFormat;
                            }
                            break;
                        default:
                            b = new BaseMedia();
                            break;
                    }
                    Log.Info(ParseListCommand_Log_MethodName + "(List, " + type + ")", ParseListCommand_Log_ListDetailsAdded + type + " " + b.Number);
                    list.Add(b);
                }
                ExitCode = ProcessExitCode.Ok;
            }
            else
            {
                ExitCode = ProcessExitCode.ErrorNoMatchesFound;
                Log.Error(ParseListCommand_Log_MethodName + "(List, " + type + ")", "No Matches - " + ExitCode + ": ");
                Log.Append(StdOutput, true);
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
            Log.MethodCall(GetAttributes_Log_MethodName + "(List, " + type + ")");
            CurrentProcess = nameof(GetAttributes);

            foreach (BaseMedia m in list)
            {
                AddScriptCommand(Script_Line_Select + type + " " + m.Number);
                AddScriptCommand(Script_Line_Attribute + type);
            }
            WriteScript();
            ExitCode = Run();
            if (ExitCode == ProcessExitCode.Ok)
            {
                Log.Info(GetAttributes_Log_MethodName + "(List, " + type + ")", nameof(StdOutput) + ": ");
                Log.Append(StdOutput, true);
                return ParseAttributes(ref list, type);
            }
            else
            {
                ExitCode = ProcessExitCode.ErrorRun;
                Log.Error(GetAttributes_Log_MethodName + "(List, " + type + ")", nameof(Run) + " - " + ExitCode + ": ");
                Log.Append(StdOutput, true);
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
            Log.MethodCall(ParseAttributes_Log_MethodName + "(List, " + type + ")");
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
                        rx = new Regex(RX_Volume_Attribute_Parse);
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
                                medianum = Int32.Parse(gc[RX_BaseMedia_Number].Value);

                                if (gc[RX_Disk_CurrentReadOnly].Success)
                                    if (gc[RX_Disk_CurrentReadOnly].Value == FLAG_ENABLED)
                                        list[medianum].Attributes |= Attributes.CurrentReadOnlyState;
                                if (gc[RX_Media_ReadOnly].Success)
                                    if (gc[RX_Media_ReadOnly].Value == FLAG_ENABLED)
                                        list[medianum].Attributes |= Attributes.ReadOnly;
                                if (gc[RX_Disk_BootDisk].Success)
                                    if (gc[RX_Disk_BootDisk].Value == FLAG_ENABLED)
                                        list[medianum].Attributes |= Attributes.Boot;
                                if (gc[RX_Disk_PagefileDisk].Success)
                                    if (gc[RX_Disk_PagefileDisk].Value == FLAG_ENABLED)
                                        list[medianum].Attributes |= Attributes.Pagefile;
                                if (gc[RX_Disk_HibernationFileDisk].Success)
                                    if (gc[RX_Disk_HibernationFileDisk].Value == FLAG_ENABLED)
                                        list[medianum].Attributes |= Attributes.HibernationFile;
                                if (gc[RX_Disk_CrashDumpDisk].Success)
                                    if (gc[RX_Disk_CrashDumpDisk].Value == FLAG_ENABLED)
                                        list[medianum].Attributes |= Attributes.Crashdump;
                                if (gc[RX_Disk_ClusteredDisk].Success)
                                    if (gc[RX_Disk_ClusteredDisk].Value == FLAG_ENABLED)
                                        list[medianum].Attributes |= Attributes.Cluster;
                                break;
                            case StorageType.VOLUME:
                                medianum = Int32.Parse(gc[RX_BaseMedia_Number].Value);

                                if (gc[RX_Media_ReadOnly].Success)
                                    if (gc[RX_Media_ReadOnly].Value == FLAG_ENABLED)
                                        list[medianum].Attributes |= Attributes.ReadOnly;
                                if (gc[RX_Media_Hidden].Success)
                                    if (gc[RX_Media_Hidden].Value == FLAG_ENABLED)
                                        list[medianum].Attributes |= Attributes.Hidden;
                                if (gc[RX_Volume_NoDefaultDriveLetter].Success)
                                    if (gc[RX_Volume_NoDefaultDriveLetter].Value == FLAG_ENABLED)
                                        list[medianum].Attributes |= Attributes.NoDefaultDriveLetter;
                                if (gc[RX_Volume_ShadowCopy].Success)
                                    if (gc[RX_Volume_ShadowCopy].Value == FLAG_ENABLED)
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
                    Log.Error(ParseAttributes_Log_MethodName + "(List, " + type + ")", "No Matches - " + ExitCode + ": ");
                    Log.Append(StdOutput, true);
                }
            }
            else
            {
                ExitCode = ProcessExitCode.ErrorNullVolumes;
                Log.Error(ParseAttributes_Log_MethodName + "(List, " + type + ")", "Null List - " + ExitCode + ": ");
                Log.Append(StdOutput, true);
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
            Log.MethodCall(DetailCommand_Log_MethodName + "(" + media.Number + ", " + type + ", " + selectedDisk + ")");
            CurrentProcess = nameof(DetailCommand);

            if (type == StorageType.PARTITION)
                AddScriptCommand(Script_Line_Select_Disk + selectedDisk);

            AddScriptCommand(Script_Line_Select + type + " " + media.Number);
            AddScriptCommand(Script_Line_Detail + type);
            WriteScript();
            ExitCode = Run();
            if (ExitCode == ProcessExitCode.Ok)
            {
                Log.Info(DetailCommand_Log_MethodName + "(" + media.Name + ", " + type + ", " + selectedDisk + ")", nameof(StdOutput) + ": ");
                Log.Append(StdOutput, true);
                return ParseDetailCommand(ref media, type);
            }
            else
            {
                ExitCode = ProcessExitCode.ErrorRun;
                Log.Error(DetailCommand_Log_MethodName + "(" + media.Name + ", " + type + ", " + selectedDisk + ")", nameof(Run) + " - " + ExitCode + ": ");
                Log.Append(StdOutput, true);
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
            Log.MethodCall(DetailCommand_Log_MethodName + "(" + media.Number + ", " + type + ")");
            CurrentProcess = nameof(ParseDetailCommand);

            Regex rx;
            switch (type)
            {
                case StorageType.DISK:
                    rx = new Regex(RX_Disk_Detail);
                    break;
                case StorageType.PARTITION:
                    rx = new Regex(RX_Partition_Detail);
                    break;
                case StorageType.VOLUME:
                    rx = new Regex(RX_Volume_Detail);
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

                    if (gc[RX_Media_ReadOnly].Success)
                        if (gc[RX_Media_ReadOnly].Value == FLAG_ENABLED)
                            media.SetFlag(Attributes.ReadOnly);
                    if (gc[RX_Media_Hidden].Success)
                        if (gc[RX_Media_Hidden].Value == FLAG_ENABLED)
                            media.SetFlag(Attributes.Hidden);

                    switch (type)
                    {
                        case StorageType.DISK:
                            if (gc[RX_BaseMedia_Name].Success)
                                media.Name = gc[RX_BaseMedia_Name].Value.Trim();
                            if (gc[RX_Disk_DiskID].Success)
                                ((Disk)media).DiskID = gc[RX_Disk_DiskID].Value;
                            if (gc[RX_Disk_Type].Success)
                                ((Disk)media).Type = DiskTypeExtension.Parse(gc[RX_Disk_Type].Value);
                            if (gc[RX_BaseMedia_Status].Success)
                                media.Status = StatusExtension.Parse(gc[RX_BaseMedia_Status].Value);
                            if (gc[RX_Disk_Path].Success)
                                ((Disk)media).Path = gc[RX_Disk_Path].Value;
                            if (gc[RX_Disk_Target].Success)
                                ((Disk)media).Target = gc[RX_Disk_Target].Value;
                            if (gc[RX_Disk_LunID].Success)
                                ((Disk)media).LunID = gc[RX_Disk_LunID].Value;
                            if (gc[RX_Disk_LocationPath].Success)
                                ((Disk)media).LocationPath = gc[RX_Disk_LocationPath].Value;
                            if (gc[RX_Disk_CurrentReadOnly].Success)
                                if (gc[RX_Disk_CurrentReadOnly].Value == FLAG_ENABLED)
                                    media.SetFlag(Attributes.CurrentReadOnlyState);
                            if (gc[RX_Disk_BootDisk].Success)
                                if (gc[RX_Disk_BootDisk].Value == FLAG_ENABLED)
                                    media.SetFlag(Attributes.Boot);
                            if (gc[RX_Disk_PagefileDisk].Success)
                                if (gc[RX_Disk_PagefileDisk].Value == FLAG_ENABLED)
                                    media.SetFlag(Attributes.Pagefile);
                            if (gc[RX_Disk_HibernationFileDisk].Success)
                                if (gc[RX_Disk_HibernationFileDisk].Value == FLAG_ENABLED)
                                    media.SetFlag(Attributes.HibernationFile);
                            if (gc[RX_Disk_CrashDumpDisk].Success)
                                if (gc[RX_Disk_CrashDumpDisk].Value == FLAG_ENABLED)
                                    media.SetFlag(Attributes.Crashdump);
                            if (gc[RX_Disk_ClusteredDisk].Success)
                                if (gc[RX_Disk_ClusteredDisk].Value == FLAG_ENABLED)
                                    media.SetFlag(Attributes.Cluster);
                            break;

                        case StorageType.PARTITION:
                            if (gc[RX_Partition_Type].Success)
                                ((Partition)media).Type = gc[RX_Partition_Type].Value;
                            if (gc[RX_Partition_Active].Success)
                                if (gc[RX_Partition_Active].Value == FLAG_ENABLED)
                                    media.SetFlag(Attributes.Active);
                            if (gc[RX_Partition_OffsetInBytes].Success)
                                ((Partition)media).OffsetInBytes = gc[RX_Partition_OffsetInBytes].Value;
                            break;

                        case StorageType.VOLUME:
                            if (gc[RX_Volume_NoDefaultDriveLetter].Success)
                                if (gc[RX_Volume_NoDefaultDriveLetter].Value == FLAG_ENABLED)
                                    media.SetFlag(Attributes.NoDefaultDriveLetter);
                            if (gc[RX_Volume_ShadowCopy].Success)
                                if (gc[RX_Volume_ShadowCopy].Value == FLAG_ENABLED)
                                    media.SetFlag(Attributes.ShadowCopy);
                            if (gc[RX_Volume_Offline].Success)
                                if (gc[RX_Volume_Offline].Value == FLAG_ENABLED)
                                    media.SetFlag(Attributes.Offline);
                            if (gc[RX_Volume_BitLocker].Success)
                                if (gc[RX_Volume_BitLocker].Value == FLAG_ENABLED)
                                    media.SetFlag(Attributes.BitLocker);
                            if (gc[RX_Volume_Installable].Success)
                                if (gc[RX_Volume_Installable].Value == FLAG_ENABLED)
                                    media.SetFlag(Attributes.Installable);
                            if (gc[RX_Media_ParentNumber].Success)
                                media.Parent = gc[RX_Media_ParentNumber].Value;
                            else
                                media.Parent = string.Empty;
                            if (gc[RX_BaseMedia_Size].Success)
                                ((Volume)media).Capacity = Int32.Parse(gc[RX_BaseMedia_Size].Value);
                            if (gc[RX_BaseMedia_SizePostfix].Success)
                                ((Volume)media).CapacityPostfix = SizePostfixExtension.Parse(gc[RX_BaseMedia_SizePostfix].Value);
                            if (gc[RX_Media_FreeSpace].Success)
                                ((Volume)media).FreeSpace = Int32.Parse(gc[RX_Media_FreeSpace].Value);
                            if (gc[RX_Media_FreeSpacePostfix].Success)
                                ((Volume)media).FreeSpacePostfix = SizePostfixExtension.Parse(gc[RX_Media_FreeSpacePostfix].Value);
                            break;
                        default:
                            break;
                    }
                    Log.Info(DetailCommand_Log_MethodName + "(" + media.Number + ", " + type + ")", DetailCommand_Log_DetailsAdded + type + " " + media.Number);
                    Log.Append(media);
                }
            }
            else
            {
                ExitCode = ProcessExitCode.ErrorNoMatchesFound;
                Log.Error(DetailCommand_Log_MethodName + "(" + media.Name + ", " + type + ")", "No Matches - " + ExitCode + ": ");
                Log.Append(StdOutput, true);
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
            Log.MethodCall(EjectVolume_Log_MethodName + "(" + b.Name + ")");
            CurrentProcess = nameof(EjectVolume);
            if (b is Disk)
            { // || b is Partition
                ExitCode = ProcessExitCode.ErrorInvalidMediaType;
                Log.Error(EjectVolume_Log_MethodName + "(" + b.Name + ")", "Invalid Media - " + ExitCode);
            }
            else
            {
                AddScriptCommand(Script_Line_Select_Volume + b.Number);
                AddScriptCommand(Script_Line_Remove_All_Dismount);
                WriteScript();
                ExitCode = Run();
                if (ExitCode == ProcessExitCode.Ok)
                {
                    Log.Info(EjectVolume_Log_MethodName + "(" + b.Name + ")", nameof(StdOutput) + ": ");
                    Log.Append(StdOutput, true);
                    if (TestOutput(EjectVolume_TestOutput))
                    {
                        Log.Info(EjectVolume_Log_MethodName + "(" + b.Name + ")", nameof(TestOutput) + " - " + nameof(StdOutput) + ": ");
                        Log.Append(StdOutput, true);
                        ExitCode = ProcessExitCode.Ok;
                    }
                    else
                    {
                        ExitCode = ProcessExitCode.ErrorTestOutput;
                        Log.Error(EjectVolume_Log_MethodName + "(" + b.Name + ")", nameof(TestOutput) + " - " + ExitCode + ": ");
                        Log.Append(StdOutput, true);
                    }
                }
                else
                {
                    ExitCode = ProcessExitCode.ErrorRun;
                    Log.Error(EjectVolume_Log_MethodName + "(" + b.Name + ")", nameof(Run) + " - " + ExitCode + ": ");
                    Log.Append(StdOutput, true);
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
            Log.MethodCall(AssignVolumeLetter_Log_MethodName + "(" + b.Name + ")");
            CurrentProcess = nameof(AssignVolumeLetter);
            if (b is Disk) // || b is Partition
            {
                ExitCode = ProcessExitCode.ErrorInvalidMediaType;
                Log.Error(AssignVolumeLetter_Log_MethodName + "(" + b.Name + ")", "Invalid Media - " + ExitCode);
            }
            else
            {
                if (!b.IsMounted())
                {
                    AddScriptCommand(Script_Line_Select_Volume + b.Number);
                    AddScriptCommand(Script_Line_Assign);
                    WriteScript();
                    ExitCode = Run();
                    if (ExitCode == ProcessExitCode.Ok)
                    {
                        Log.Info(AssignVolumeLetter_Log_MethodName + "(" + b.Name + ")", nameof(StdOutput) + ": ");
                        Log.Append(StdOutput, true);
                        if (TestOutput(AssignVolumeLetter_TestOutput))
                        {
                            ExitCode = ProcessExitCode.Ok;
                            Log.Info(AssignVolumeLetter_Log_MethodName + "(" + b.Name + ")", nameof(TestOutput) + " - " + nameof(StdOutput) + ": ");
                            Log.Append(StdOutput, true);
                        }
                        else
                        {
                            ExitCode = ProcessExitCode.ErrorTestOutput;
                            Log.Error(AssignVolumeLetter_Log_MethodName + "(" + b.Name + ")", nameof(TestOutput) + " - " + ExitCode + ": ");
                            Log.Append(StdOutput, true);
                        }
                    }
                    else
                    {
                        ExitCode = ProcessExitCode.ErrorRun;
                        Log.Error(AssignVolumeLetter_Log_MethodName + "(" + b.Name + ")", nameof(Run) + " - " + ExitCode + ": ");
                        Log.Append(StdOutput, true);
                    }
                }
                else
                {
                    ExitCode = ProcessExitCode.ErrorVolumeNotMounted;
                    Log.Error(AssignVolumeLetter_Log_MethodName + "(" + b.Name + ")", "Volume Not Mounted - " + ExitCode);
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
            Log.MethodCall(SetReadOnly_Log_MethodName + "(" + b.Name + ", " + function + ", " + type + ")");
            CurrentProcess = nameof(SetReadOnly);

            // Check for Partition type

            if (!b.CanToggleReadOnly())
            {
                ExitCode = ProcessExitCode.ErrorInvalidMediaType;
                Log.Error(SetReadOnly_Log_MethodName + "(" + b.Name + ", " + function + ", " + type + ")", "Invalid Media Type - " + ExitCode);
            }
            else
            {
                AddScriptCommand(Script_Line_Select + type + " " + b.Number);
                AddScriptCommand(Script_Line_Attribute + type + " " + function + Script_Line_ReadOnly);
                WriteScript();
                ExitCode = Run();
                if (ExitCode == ProcessExitCode.Ok)
                {
                    Log.Info(SetReadOnly_Log_MethodName + "(" + b.Name + ", " + function + ", " + type + ")", nameof(StdOutput) + ": ");
                    Log.Append(StdOutput, true);
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
                        Log.Info(SetReadOnly_Log_MethodName + "(" + b.Name + ", " + function + ", " + type + ")", nameof(TestOutput) + " - " + nameof(StdOutput) + ": ");
                        Log.Append(StdOutput, true);
                    }
                    else
                    {
                        ExitCode = ProcessExitCode.ErrorTestOutput;
                        Log.Error(SetReadOnly_Log_MethodName + "(" + b.Name + ", " + function + ", " + type + ")", nameof(TestOutput) + " - " + ExitCode + ": ");
                        Log.Append(StdOutput, true);
                    }
                }
                else
                {
                    ExitCode = ProcessExitCode.ErrorRun;
                    Log.Error(SetReadOnly_Log_MethodName + "(" + b.Name + ", " + function + ", " + type + ")", nameof(Run) + " - " + ExitCode + ": ");
                    Log.Append(StdOutput, true);
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
            Log.MethodCall(GetFileSystemInfo_Log_MethodName + "(" + v.Name + ", List, List)");
            CurrentProcess = nameof(GetFileSystemInfo);

            // Check for Partition type

            AddScriptCommand(Script_Line_Select_Volume + v.Number);
            AddScriptCommand(Script_Line_FileSystem);
            WriteScript();
            ExitCode = Run();
            if (ExitCode == ProcessExitCode.Ok)
            {
                Log.Info(GetFileSystemInfo_Log_MethodName + "(" + v.Name + ", List, List)", nameof(StdOutput) + ": ");
                Log.Append(StdOutput, true);
                return ParseFileSystemInfo(ref fs, ref us);
            }
            else
            {
                ExitCode = ProcessExitCode.ErrorRun;
                Log.Error(GetFileSystemInfo_Log_MethodName + "(" + v.Name + ", List, List)", nameof(Run) + " - " + ExitCode + ": ");
                Log.Append(StdOutput, true);
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
            Log.MethodCall(ParseFileSystemInfo_Log_MethodName);
            CurrentProcess = nameof(ParseFileSystemInfo);

            if (fs == null)
            {
                ExitCode = ProcessExitCode.ErrorFileSystemNull;
                Log.Error(ParseFileSystemInfo_Log_MethodName, "File System List Null - " + ExitCode);
                return ExitCode;
            }

            if (us == null)
            {
                ExitCode = ProcessExitCode.ErrorUnitSizeNull;
                Log.Error(ParseFileSystemInfo_Log_MethodName, "Unit Size List Null - " + ExitCode);
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
                Log.Error(ParseFileSystemInfo_Log_MethodName, "No Matches Found - " + ExitCode + ": ");
                Log.Append(StdOutput, true);
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
            Log.MethodCall(Format_Log_MethodName + "(" + b.Name + ", FormatArguments)");
            CurrentProcess = nameof(Format);

            if (b == null)
            {
                ExitCode = ProcessExitCode.ErrorNullVolumes;
                Log.Error(Format_Log_MethodName + "(" + b.Name + ", FormatArguments)", "Invalid Volumes - " + ExitCode);
                return ExitCode;
            }

            if (b is Disk)
            {
                ExitCode = ProcessExitCode.ErrorInvalidMediaType;
                Log.Error(Format_Log_MethodName + "(" + b.Name + ", FormatArguments)", "Invalid Media Type - " + ExitCode);
                return ExitCode;
            }

            AddScriptCommand(Script_Line_Select_Volume + b.Number);
            AddScriptCommand(Script_Line_Format + fa.GetArguments());
            WriteScript();
            ExitCode = Run();
            if (ExitCode == ProcessExitCode.Ok)
            {
                Log.Info(Format_Log_MethodName + "(" + b.Name + ", FormatArguments)", nameof(StdOutput) + ": ");
                Log.Append(StdOutput, true);
                if (TestOutput(Format_TestOutput))
                {
                    ExitCode = ProcessExitCode.Ok;
                    Log.Info(Format_Log_MethodName + "(" + b.Name + ", FormatArguments)", nameof(TestOutput) + " - " + nameof(StdOutput) + ": ");
                    Log.Append(StdOutput, true);
                }
                else
                {
                    ExitCode = ProcessExitCode.ErrorTestOutput;
                    Log.Error(Format_Log_MethodName + "(" + b.Name + ", FormatArguments)", nameof(TestOutput) + " - " + ExitCode + ": ");
                    Log.Append(StdOutput, true);
                }
            }
            else
            {
                ExitCode = ProcessExitCode.ErrorRun;
                Log.Error(Format_Log_MethodName + "(" + b.Name + ", FormatArguments)", nameof(Run) + " - " + ExitCode + ": ");
                Log.Append(StdOutput, true);
            }

            return ExitCode;
        }

    }
}
