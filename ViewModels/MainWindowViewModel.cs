using DiskpartGUI.Commands;
using DiskpartGUI.Models;
using DiskpartGUI.Processes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace DiskpartGUI.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private string embstate;
        private Volume selected;

        private Lazy<DiskpartProcess> ldpp = new Lazy<DiskpartProcess>(() => new DiskpartProcess());

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
                    if (value.IsMounted())
                        EjectMountButtonContent = "Eject";
                    else
                        EjectMountButtonContent = "Mount";
                }
                OnPropertyChanged(nameof(SelectedVolume));
            }
        }

        public EjectCommand EjectCommand { get; set; }


        public DiskpartProcess DiskpartProcess
        {
            get
            {
                return ldpp.Value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new MainWindowViewModel
        /// </summary>
        public MainWindowViewModel()
        {
            EjectMountButtonContent = "Eject";

            EjectCommand = new EjectCommand(this);

            Refresh();
        }

        public void EjectVolume()
        {

            if(DiskpartProcess.EjectVolume(SelectedVolume) != ProcessExitCode.Ok)
                ShowError("EjectVolume - EjectVolume");
            Refresh();
        }

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void Refresh()
        {
            
            if (DiskpartProcess.GetVolumes(ref volumes) != ProcessExitCode.Ok)
                ShowError("Refresh - GetVolumes");
            else
                if(DiskpartProcess.GetReadOnlyState(ref volumes) != ProcessExitCode.Ok)
                    ShowError("Refresh - GetReadOnlyState");
        }

        private void ShowError(string callingfrom)
        {
            MessageBox.Show("Error: " + DiskpartProcess.ExitCode + " - " + DiskpartProcess.StdError + 
                "\n" + DiskpartProcess.StdOutput, callingfrom);
        }

    }
}
