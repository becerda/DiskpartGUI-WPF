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
        /// <summary>
        /// The expected output of a failed label change
        /// </summary>
        private static readonly string Error_Regex = @"Cannot change label.  This volume is write protected.";

        /// <summary>
        /// Renames a given volume
        /// </summary>
        /// <param name="v">The volume to rename</param>
        /// <param name="newlabel">The label to give the volume</param>
        /// <returns></returns>
        public static ProcessExitCode RenameVolume(ref Volume v, string newlabel)
        {
            if (v.IsValid())
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
            return ProcessExitCode.ErrorInvalidVolume;
        }
    }
}
