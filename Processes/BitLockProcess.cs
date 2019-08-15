using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskpartGUI.Processes
{
    class BitLockProcess
    {
        public static ProcessExitCode LaunchBitLock()
        {
            CMDProcess bl = new CMDProcess("control");
            bl.AddArgument("/name Microsoft.BitLockerDriveEncryption");
            return bl.Run();
        }
    }
}
