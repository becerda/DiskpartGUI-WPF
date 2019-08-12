using DiskpartGUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskpartGUI.Processes
{
    static class LabelProcess
    {

        private static readonly string Error_Regex = @"Cannot change label.  This volume is write protected.";

        public static ProcessExitCode RenameVolume(ref Volume v, string newlabel)
        {
            CMDProcess label = new CMDProcess("label");
            label.AddArgument(v.DriveLetter + " " + newlabel);
            if (label.Run() == ProcessExitCode.Ok)
            {
                if (label.TestOutput(Error_Regex))
                    return ProcessExitCode.Error;
                v.Label = newlabel;
                return ProcessExitCode.Ok;
            }
            return ProcessExitCode.Error;
        }
    }
}
