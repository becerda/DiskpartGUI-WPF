using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DiskpartGUI.Processes
{

    /// <summary>
    /// Different exit codes a process can have
    /// </summary>
    enum ProcessExitCode
    {
        Ok,
        Error,
        ErrorVolumeNotMounted,
        ErrorInvalidVolume,
        ErrorVolumeMounted,
        ErrorParse
    }

    class ProcessRunner
    {
        /// <summary>
        /// The process to run
        /// </summary>
        public Process Process { get; set; }

        /// <summary>
        /// the standard output of the process
        /// </summary>
        public string StdOutput { get; private set; }

        /// <summary>
        /// The standard error output of the process
        /// </summary>
        public string StdError { get; protected set; }

        /// <summary>
        /// The exit code of the process
        /// </summary>
        public ProcessExitCode ExitCode { get; set; }

        /// <summary>
        /// Initializes new ProcessRunner with a process
        /// </summary>
        /// <param name="processname">The process to run</param>
        public ProcessRunner(string processname)
        {
            Process = new Process();
            Process.StartInfo.FileName = processname;
            Process.StartInfo.RedirectStandardInput = true;
            Process.StartInfo.RedirectStandardOutput = true;
            Process.StartInfo.RedirectStandardError = true;
            Process.StartInfo.CreateNoWindow = true;
            Process.StartInfo.UseShellExecute = false;
            Process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process.StartInfo.Arguments = @"/C ";
        }

        /// <summary>
        /// Initializes a new ProcessRunner with a process and arguments
        /// </summary>
        /// <param name="processname">The process to run</param>
        /// <param name="arguments">The arguments for that process</param>
        public ProcessRunner(string processname, string arguments) : this(processname)
        {
            AddArgument(arguments);
        }

        /// <summary>
        /// Adds arguments to the process
        /// </summary>
        /// <param name="argument">The arguments to add</param>
        public void AddArgument(string argument)
        {
            Process.StartInfo.Arguments += @argument + " ";
        }

        /// <summary>
        /// Clears the arguments from the process
        /// </summary>
        public void ClearArguments()
        {
            Process.StartInfo.Arguments = @"/C";
        }

        /// <summary>
        /// Runns the process
        /// </summary>
        /// <returns>The exit code of the process</returns>
        public ProcessExitCode Run()
        {
            Process.StartInfo.Arguments.TrimEnd(' ');
            if (Process.Start())
            {
                StdOutput = Process.StandardOutput.ReadToEnd();
                StdError = Process.StandardError.ReadToEnd();
                Process.WaitForExit();
                return ProcessExitCode.Ok;
            }

            return ProcessExitCode.Error;
        }

        /// <summary>
        /// Tests the output of the process against expected input
        /// </summary>
        /// <param name="regex">The expected output</param>
        /// <returns></returns>
        public bool TestOutput(string regex)
        {
            Match result = Regex.Match(StdOutput, regex);
            return result.Success;
        }
    }
}
