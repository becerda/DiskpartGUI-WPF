using DiskpartGUI.Models;
using DiskpartGUI.Processes;
using System.Collections.Generic;
using System.Windows;

namespace DiskpartGUI.ViewModels
{
    class MainWindowViewModel
    {
        /// <summary>
        /// The ItemSorce for the list view
        /// </summary>
        private List<Volume> volumes;

        public List<Volume> Volumes
        {
            get
            {
                return volumes;
            }
            set
            {
                volumes = value;
            }
        }

        /// <summary>
        /// Initializes a new MainWindowViewModel
        /// </summary>
        public MainWindowViewModel()
        {
            DiskpartProcess dpp = new DiskpartProcess();

            dpp.GetVolumes(ref volumes);
            dpp.GetReadOnlyState(ref volumes);
        }

    }
}
