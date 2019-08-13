using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskpartGUI.Processes
{
    class CMDProcess : ProcessRunner
    {
        public CMDProcess(string cmdprocess, string arguments) : base("cmd.exe")
        {
            AddArgument(cmdprocess + " " + arguments);
        }

        public CMDProcess(string cmdprocess) : base("cmd.exe")
        {
            AddArgument(cmdprocess);
        }
    }
}
