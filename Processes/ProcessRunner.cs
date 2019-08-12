using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiskpartGUI.Processes
{


    enum ProcessExitCode
    {
        Ok,
        Error
    }

    class ProcessRunner
    {

        public Process Process { get; set; }

        public string StdOutput { get; private set; }

        public string StdError { get; private set; }

        public ProcessExitCode ExitCode { get; set; }

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

        public ProcessRunner(string processname, string arguments) : this(processname)
        {
            AddArgument(arguments);
        }

        public void AddArgument(string argument)
        {
            Process.StartInfo.Arguments += @argument + " ";
        }

        public void ClearArguments()
        {
            Process.StartInfo.Arguments = @"/C";
        }

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

        public bool TestOutput(string regex)
        {
            Match result = Regex.Match(StdOutput, regex);
            return result.Success;
        }
    }
}
