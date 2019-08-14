using DiskpartGUI.Commands;
using DiskpartGUI.Models;
using DiskpartGUI.Processes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace DiskpartGUI.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private string embstate;
        private Volume selected;
        private string selectedinfo;

        /// <summary>
        /// Lazy Instantiation of a DiskpartProcess
        /// </summary>
        private Lazy<DiskpartProcess> ldpp = new Lazy<DiskpartProcess>(() => new DiskpartProcess());

        private List<Volume> volumes;

        /// <summary>
        /// The ItemSorce for the list view
        /// </summary>
        public List<Volume> Volumes
        {
            get
            {
                return volumes;
            }
            set
            {
                volumes = value;
                OnPropertyChanged(nameof(Volumes));
            }
        }
        
        /// <summary>
        /// The Eject/Mount button content state
        /// </summary>
        public string EjectMountButtonContent
        {
            get
            {
                return embstate;
            }
            set
            {
                embstate = value;
                OnPropertyChanged(nameof(EjectMountButtonContent));
            }
        }

        /// <summary>
        /// The selected Volume in ListViewVolumes
        /// </summary>
        public Volume SelectedVolume
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                if (value != null)
                {
                    SelectedVolumeInfo = value.ToString();
                    if (value.IsMounted())
                        EjectMountButtonContent = "Eject";
                    else
                        EjectMountButtonContent = "Mount";
                }
                OnPropertyChanged(nameof(SelectedVolume));
            }
        }

        /// <summary>
        /// The ToString of SelectedVolume
        /// </summary>
        public string SelectedVolumeInfo
        {
            get
            {
                return selectedinfo;
            }
            set
            {
                selectedinfo = value;
                OnPropertyChanged(nameof(SelectedVolumeInfo));
            }
        }

        /// <summary>
        /// Eject command for ButtonEject
        /// </summary>
        public CommandEject EjectCommand { get; set; }

        /// <summary>
        /// Refresh command for ButtonRefresh
        /// </summary>
        public CommandRefresh RefreshCommand { get; set; }

        /// <summary>
        /// Reference to a DiskpartProces
        /// </summary>
        public DiskpartProcess DiskpartProcess
        {
            get
            {
                return ldpp.Value;
            }
        }

        /// <summary>
        /// Property changed event handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new MainWindowViewModel
        /// </summary>
        public MainWindowViewModel()
        {
            EjectMountButtonContent = "Eject";

            EjectCommand = new CommandEject(this);
            RefreshCommand = new CommandRefresh(this);

            Refresh();
        }

        /// <summary>
        /// Ejects the SelectedVolume
        /// </summary>
        public void EjectVolume()
        {
            if (MessageBox.Show("Are you sure you want to eject " + SelectedVolume.ToString() + "?",
                "Eject Volume " + SelectedVolume.DriveLetter, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (DiskpartProcess.EjectVolume(SelectedVolume) != ProcessExitCode.Ok)
                    ShowError("EjectVolume - EjectVolume");
                else
                {
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Mounts the SelectedVolume
        /// </summary>
        public void MountVolume()
        {
            if (DiskpartProcess.AssignVolumeLetter(SelectedVolume) != ProcessExitCode.Ok)
                ShowError("MountVolume - AssignVolumeLetter");
            else
            {
                Refresh();
            }
        }

        /// <summary>
        /// Notifies the property changed event
        /// </summary>
        /// <param name="property"></param>
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        /// <summary>
        /// Refreshes Volumes
        /// </summary>
        public void Refresh()
        {
            if (DiskpartProcess.GetVolumes(ref volumes) != ProcessExitCode.Ok)
                ShowError("Refresh - GetVolumes");
            else
                if(DiskpartProcess.GetReadOnlyState(ref volumes) != ProcessExitCode.Ok)
                    ShowError("Refresh - GetReadOnlyState");
            OnPropertyChanged(nameof(Volumes));
        }

        /// <summary>
        /// Shows a MessageBox with an error
        /// </summary>
        /// <param name="callingfrom">Where the error happend</param>
        private void ShowError(string callingfrom)
        {
            MessageBox.Show("Error: " + DiskpartProcess.ExitCode + " - " + DiskpartProcess.StdError + 
                "\n" + DiskpartProcess.StdOutput, callingfrom);
        }

    }
}
