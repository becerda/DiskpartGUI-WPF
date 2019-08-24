
using System;

namespace DiskpartGUI.ViewModels
{
    class AboutWindowViewModel : ApplyCancelViewModel
    {

        public string ProgramName
        {
            get
            {
                return "Diskpart GUI " + Properties.Settings.Default.Version;
            }
        }

        public string WebsiteAddress
        {
            get
            {
                return Properties.Settings.Default.Website;
            }
        }
        
        public string Information
        {
            get
            {
                return "This program is free software. This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE";
            }
        }

        public override void Apply()
        {
            throw new NotImplementedException();
        }
    }
}
