using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskpartGUI.Processes
{
    public class CMDProcess : ProcessRunner
    {
        /// <summary>
        /// The name of the cmd process
        /// </summary>
        private string CMDName { get; set; }

        /// <summary>
        /// Initializes a new cmd process
        /// </summary>
        /// <param name="cmdprocess">The name of the cmd</param>
        public CMDProcess(string cmdprocess) : this("cmd.exe", string.Empty)
        {
            this.CMDName = cmdprocess;
            ClearArguments();
        }

        /// <summary>
        /// Initializes a new cmd process with arguments
        /// </summary>
        /// <param name="cmdprocess">The name of the cmd process</param>
        /// <param name="arguments">The arguments for a cmd process</param>
        public CMDProcess(string cmdprocess, string arguments) : base("cmd.exe")
        {
            CMDName = cmdprocess;
            ClearArguments();
            AddArgument(arguments);
        }

        /// <summary>
        /// Clears and resets the arguments of the cmd process
        /// </summary>
        public override void ClearArguments()
        {
            Info.Arguments = "/C " + CMDName + " ";
        }
    }
}
