using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DiskpartGUI.Processes
{

    /// <summary>
    /// Different exit codes a process can have
    /// </summary>
    public enum ProcessExitCode
    {
        Ok,
        Error,
        ErrorRun,
        ErrorVolumeNotMounted,
        ErrorInvalidMediaType,
        ErrorVolumeMounted,
        ErrorParse,
        ErrorNullVolumes,
        ErrorTestOutput,
        ErrorFileSystemNull,
        ErrorUnitSizeNull,
        ErrorNoMatchesFound,
        ErrorNullRegex,
    }

    public abstract class ProcessRunner
    {
        private ProcessExitCode exit;

        /// <summary>
        /// Information about the process to run
        /// </summary>
        protected ProcessStartInfo Info { get; set; }

        /// <summary>
        /// The name of the process
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the standard output of the process
        /// </summary>
        public string StdOutput { get; private set; }

        /// <summary>
        /// The standard error output of the process
        /// </summary>
        public string StdError { get; protected set; }

        public string CurrentProcess { get; set; }

        /// <summary>
        /// The exit code of the process, also sets error mesage
        /// </summary>
        public ProcessExitCode ExitCode
        {
            get
            {
                return exit;
            }
            set
            {
                exit = value;
                if (CurrentProcess != null)
                {
                    string err = string.Empty;
                    switch (value)
                    {
                        case ProcessExitCode.Error:
                            err = "General Error";
                            break;
                        case ProcessExitCode.ErrorInvalidMediaType:
                            err = "Invalid Volume";
                            break;
                        case ProcessExitCode.ErrorNullVolumes:
                            err = "Volume Is Null";
                            break;
                        case ProcessExitCode.ErrorParse:
                            err = "No Matches Found";
                            break;
                        case ProcessExitCode.ErrorRun:
                            err = "Run Failed";
                            break;
                        case ProcessExitCode.ErrorTestOutput:
                            err = "Test Output Failed";
                            break;
                        case ProcessExitCode.ErrorVolumeMounted:
                            err = "Volume Is Mounted";
                            break;
                        case ProcessExitCode.ErrorVolumeNotMounted:
                            err = "Volume Is Not Mounted";
                            break;
                        default:
                            break;
                    }
                    StdError = CurrentProcess + " - " + err;
                }
            }
        }

        /// <summary>
        /// Initializes new ProcessRunner with a process
        /// </summary>
        /// <param name="processname">The process to run</param>
        public ProcessRunner(string processname)
        {
            Name = processname;
            Info = new ProcessStartInfo
            {
                FileName = Name,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };
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
            Info.Arguments += argument + " ";
        }

        /// <summary>
        /// Clears the arguments from the process
        /// </summary>
        public abstract void ClearArguments();

        /// <summary>
        /// Runns the process
        /// </summary>
        /// <returns>The exit code of the process</returns>
        public ProcessExitCode Run()
        {
            Info.Arguments.TrimEnd(' ');
            Process process = new Process();
            process.StartInfo = Info;
            if (process.Start())
            {
                StdOutput = process.StandardOutput.ReadToEnd();
                StdError = process.StandardError.ReadToEnd();
                process.WaitForExit();
                ClearArguments();
                return ProcessExitCode.Ok;

            }
            ClearArguments();
            return ProcessExitCode.Error;
        }

        /// <summary>
        /// Tests the output of the process against expected output
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
