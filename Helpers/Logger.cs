using DiskpartGUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiskpartGUI.Helpers
{
    class Logger
    {
        public enum LoggerType
        {
            Method, // method
            Info, // info
            Error, // error
        }

        private const string filename = @"\DiskpartLogger.txt";

        public static void Log(LoggerType type, string callingfrom, string info, bool split = false)
        {
            if (Properties.Settings.Default.EnableLogging)
            {
                File.AppendAllText(Directory.GetCurrentDirectory() + filename, DateTime.Now + " <" + type + ">(" + callingfrom + "): ");
                File.AppendAllText(Directory.GetCurrentDirectory() + filename, info + "\n");
            }
        }

        public static void LogAppend(string info, bool split = false)
        {
            File.AppendAllText(Directory.GetCurrentDirectory() + filename, "Appended:\n");
            if (split)
            {
                string[] sa = Regex.Split(info, "\r\n");
                foreach (string s in sa)
                {
                    File.AppendAllText(Directory.GetCurrentDirectory() + filename, "\t>" + s + "\n");
                }
            }
            else
                File.AppendAllText(Directory.GetCurrentDirectory() + filename, "\t>" + info + "\n");
        }

        public static void Log(LoggerType type, string callingfrom, string[] info)
        {
            foreach (string s in info)
            {
                Log(type, callingfrom, s);
            }
        }

        public static void LogDisk(Disk disk)
        {
            LogAppend("<Volume>\r\n" +
                $"<Name>:\"{disk.Name}\"\r\n" +
                $"<Number>:{disk.Number}\r\n" +
                $"<Status>:\"{disk.Status}\"\r\n" +
                $"<Size>:\"{disk.FullSize}\"\r\n" +
                $"<Free Space>:\"{disk.FullFreeSpace}\"\r\n" +
                $"<Dynamic>:\"{disk.Dynamic}\"\r\n" +
                $"<GPT>:\"{disk.GPTType}\"\r\n" +
                $"<DiskID>:\"{disk.DiskID}\"\r\n" +
                $"<Type>:\"{disk.Type}\"\r\n" +
                $"<Status>:\"{disk.Status}\"\r\n" +
                $"<Path>:\"{disk.Path}\"\r\n" +
                $"<Target>:\"{disk.Target}\"\r\n" +
                $"<LUN ID>:\"{disk.LunID}\"\r\n" +
                $"<Location Path>:\"{disk.LocationPath}\"\r\n" +
                $"<Current Read-Only>:\"{disk.Attributes.HasFlag(Attributes.CurrentReadOnlyState)}\"\r\n" +
                $"<Read-Only>:\"{disk.IsReadOnly()}\"\r\n" +
                $"<Boot Disk>:\"{disk.Attributes.HasFlag(Attributes.Boot)}\"\r\n" +
                $"<Pagefile Disk>:\"{disk.Attributes.HasFlag(Attributes.Pagefile)}\"\r\n" +
                $"<Hibernation File Disk>:\"{disk.Attributes.HasFlag(Attributes.HibernationFile)}\"\r\n" +
                $"<Crashdump Disk>:\"{disk.Attributes.HasFlag(Attributes.Crashdump)}\"\r\n" +
                $"<Clustered Disk>:\"{disk.Attributes.HasFlag(Attributes.Cluster)}\"", true);
        }

        public static void LogVolume(Volume volume)
        {
            LogAppend("<Volume>\r\n" +
                $"<Name>:\"{volume.Name}\"\r\n" +
                $"<Number>:{volume.Number}\r\n" +
                $"<Letter>:\"{volume.Letter}\"\r\n" +
                $"<File System>:\"{volume.FileSystem}\"\r\n" +
                $"<Type>:\"{volume.Type}\"\r\n" +
                $"<Size>:\"{volume.FullSize}\"\r\n" +
                $"<Status>:\"{volume.Status}\"\r\n" +
                $"<Info>:\"{volume.Info}\"\r\n" +
                $"<Mount State>:\"{volume.MountState}\"\r\n" +
                $"<Read-Only>:\"{volume.IsReadOnly()}\"\r\n" +
                $"<Hidden>:\"{volume.IsHidden()}\"\r\n" + 
                $"<No Default Drive Letter>:\"{volume.Attributes.HasFlag(Attributes.NoDefaultDriveLetter)}\"\r\n" +
                $"<Shadow Copy>:\"{volume.Attributes.HasFlag(Attributes.ShadowCopy)}\"\r\n" +
                $"<Offline>:\"{volume.Attributes.HasFlag(Attributes.Offline)}\"\r\n" +
                $"<BitLock Enabled>:\"{volume.Attributes.HasFlag(Attributes.BitLocker)}\"\r\n" +
                $"<Installable>:\"{volume.Attributes.HasFlag(Attributes.Installable)}\"\r\n" +
                $"<Capacity>:\"{volume.FullCapacity}\"\r\n" +
                $"<Free Space>:\"{volume.FullFreeSpace}\"\r\n" +
                $"<Parent>:\"{volume.Parent}\"", true);
        }

        public static void LogPartition(Partition part)
        {
            LogAppend("<Partition>\r\n" +
                $"<Number>:{part.Number}\r\n" +
                $"<Partition Type>:\"{part.PartitionType}\"\r\n" +
                $"<Size>:\"{part.FullSize}\"\r\n" +
                $"<Offset>:\"{part.FullOffset}\"\r\n" +
                $"<Type>:\"{part.Type}\"\r\n" +
                $"<Hidden>:\"{part.IsHidden()}\"\r\n" +
                $"<Active>:\"{part.Attributes.HasFlag(Attributes.Active)}\"\r\n" +
                $"<Offset In Bytes>:\"{part.OffsetInBytes}\"\r\n" +
                $"<Parent>:\"{part.Parent}\"", true);
        }


    }
}
