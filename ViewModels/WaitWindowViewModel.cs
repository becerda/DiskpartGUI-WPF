using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskpartGUI.ViewModels
{
    class WaitWindowViewModel
    {
        private string message;

        public string Message
        {
            get
            {
                return message;
            }
            private set
            {
                message = value;
            }
        }

        public WaitWindowViewModel(string message)
        {
            Message = message;
        }

    }
}
