using DiskpartGUI.Models;

namespace DiskpartGUI.Processes
{
    static class LabelProcess
    {
        /// <summary>
        /// The expected output of a failed label change
        /// </summary>
        private const string Error_Regex = @"Cannot change label.  This volume is write protected.";

        /// <summary>
        /// Renames a given volume
        /// </summary>
        /// <param name="v">The volume to rename</param>
        /// <param name="newlabel">The label to give the volume</param>
        /// <returns></returns>
        public static ProcessExitCode RenameVolume(ref Volume v, string newlabel)
        {
            CMDProcess label = new CMDProcess("label");
            label.AddArgument(v.DriveLetter + " " + newlabel);
            if (label.Run() == ProcessExitCode.Ok)
            {
                if (label.TestOutput(Error_Regex))
                    return ProcessExitCode.Error;
                v.Name = newlabel;
                return ProcessExitCode.Ok;
            }

            return ProcessExitCode.ErrorInvalidMediaType;
        }
    }
}
