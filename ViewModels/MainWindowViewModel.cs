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
        public List<Volume> Volumes { get; set; }

        /// <summary>
        /// Initializes a new MainWindowViewModel
        /// </summary>
        public MainWindowViewModel()
        {
            DiskpartProcess dpp = new DiskpartProcess();

            Volumes = dpp.GetVolumes();
        }

    }
}
