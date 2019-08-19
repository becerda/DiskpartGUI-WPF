using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskpartGUI.ViewModels
{
    class WaitWindowViewModel
    {
        private string action;

        /// <summary>
        /// The action this window is waiting for
        /// </summary>
        public string Action
        {
            get
            {
                return action;
            }
            private set
            {
                action = value;
            }
        }

        public WaitWindowViewModel(string action)
        {
            Action = action + ", please wait...";
        }

    }
}
