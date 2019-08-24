using System;

namespace DiskpartGUI.ViewModels
{
    class AboutWindowViewModel : ApplyCancelViewModel
    {

        /// <summary>
        /// The name of the program
        /// </summary>
        public string ProgramName
        {
            get
            {
                return "Diskpart GUI " + Properties.Settings.Default.Version;
            }
        }

        /// <summary>
        /// The website this program can be found at
        /// </summary>
        public string WebsiteAddress
        {
            get
            {
                return Properties.Settings.Default.Website;
            }
        }

        /// <summary>
        /// licensing informationg
        /// </summary>
        public string Information
        {
            get
            {
                return "This program is free software. This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE";
            }
        }

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public AboutWindowViewModel() : base() { }

        /// <summary>
        /// Will check for updates
        /// </summary>
        public override void Apply()
        {
            throw new NotImplementedException();
        }
    }
}
