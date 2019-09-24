using DiskpartGUI.Models;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace DiskpartGUI.Helpers
{
    static class Log
    {

        public enum LoggerType
        {
            Method,
            Info,
            Error,
        }

        private const string filename = @"\DiskpartLogger.txt";
        private const string Method_Called = "Called";

        /// <summary>
        /// Writes contents to the log file
        /// </summary>
        /// <param name="type">The type of log information</param>
        /// <param name="callingfrom">Where this log is called from</param>
        /// <param name="info">The information to be logged</param>
        private static void Write(LoggerType type, string callingfrom, string info)
        {
            if (Properties.Settings.Default.EnableLogging)
            {
                File.AppendAllText(Directory.GetCurrentDirectory() + filename, DateTime.Now + " <" + type + ">(" + callingfrom + "): ");
                File.AppendAllText(Directory.GetCurrentDirectory() + filename, info + "\n");
            }
        }

        /// <summary>
        /// Called for method specific info
        /// </summary>
        /// <param name="method">The method this log was called from</param>
        /// <param name="info">The information to be logged</param>
        public static void Method(string method, string info)
        {
            Write(LoggerType.Method, method, info);
        }

        /// <summary>
        /// Method specific call for when a method is called
        /// </summary>
        /// <param name="method">The method this log was called from</param>
        public static void MethodCall(string method)
        {
            Method(method, Method_Called);
        }

        /// <summary>
        /// Called for general info
        /// </summary>
        /// <param name="method">The method this log was called from</param>
        /// <param name="info">The information to be logged</param>
        public static void Info(string method, string info)
        {
            Write(LoggerType.Info, method, info);
        }

        /// <summary>
        /// Called for logging errors
        /// </summary>
        /// <param name="method">The method this log was called from</param>
        /// <param name="info">The information to be logged</param>
        public static void Error(string method, string info)
        {
            Write(LoggerType.Error, method, info);
        }

        /// <summary>
        /// Called to append additional information not in original log call
        /// </summary>
        /// <param name="info">The information to be logged</param>
        /// <param name="split">Whether the information needs to be split by line</param>
        public static void Append(string info, bool split = false)
        {
            if (Properties.Settings.Default.EnableAdvancedLogging)
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
            else
            {
                Info("Logger - Append(string, string)", "Advanced Logging Disabled");
            }
        }

        /// <summary>
        /// Called to append additional information about a Disk
        /// </summary>
        /// <param name="disk">The Disk to append to the log file</param>
        public static void Append(Disk disk)
        {
            Append("<Volume>\r\n" +
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

        /// <summary>
        /// Called to append additional information about a Volume
        /// </summary>
        /// <param name="disk">The Volume to append to the log file</param>
        public static void Append(Volume volume)
        {
            Append("<Volume>\r\n" +
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

        /// <summary>
        /// Called to append additional information about a Partition
        /// </summary>
        /// <param name="disk">The Partition to append to the log file</param>
        public static void Append(Partition part)
        {
            Append("<Partition>\r\n" +
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
