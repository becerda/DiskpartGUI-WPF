﻿using DiskpartGUI.Commands;
using DiskpartGUI.Helpers;
using DiskpartGUI.Models;
using DiskpartGUI.Processes;
using DiskpartGUI.Views;
using System;
using System.Collections.Generic;
using System.Windows;

namespace DiskpartGUI.ViewModels
{
    class MainWindowViewModel : ClosablePropertyChangedViewModel
    {
        private string embstate;
        private string scrostate;
        private Volume selected;
        private string selectedinfo;
        private List<Volume> volumes;

        /// <summary>
        /// Lazy Instantiation of a DiskpartProcess
        /// </summary>
        private Lazy<DiskpartProcess> ldpp = new Lazy<DiskpartProcess>(() => new DiskpartProcess());

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
        /// The Set/Clear button content state
        /// </summary>
        public string SetClearReadOnlyButtonContent
        {
            get
            {
                return scrostate;
            }
            set
            {
                scrostate = value;
                OnPropertyChanged(nameof(SetClearReadOnlyButtonContent));
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
                    if (value.IsReadOnly())
                        SetClearReadOnlyButtonContent = "Clear Read-Only";
                    else
                        SetClearReadOnlyButtonContent = "Set Read-Only";
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
        public RelayCommand ChangeMountStateCommand { get; private set; }

        /// <summary>
        /// Refresh command for ButtonRefresh
        /// </summary>
        public RelayCommand RefreshCommand { get; private set; }

        /// <summary>
        /// Rename command for ButtonRename
        /// </summary>
        public RelayCommand RenameCommand { get; private set; }

        /// <summary>
        /// BitLock command for ButtonBitLock
        /// </summary>
        public RelayCommand BitLockCommand { get; private set; }

        /// <summary>
        /// Read-Only command for ButtonReadOnly
        /// </summary>
        public RelayCommand ReadOnlyCommand { get; private set; }

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
        /// Initializes a new MainWindowViewModel
        /// </summary>
        public MainWindowViewModel()
        {
            EjectMountButtonContent = "Eject";
            SetClearReadOnlyButtonContent = "Set Read-Only";

            ChangeMountStateCommand = new RelayCommand(ChangeMountState, IsSelectedVolumeRemovable);
            RefreshCommand = new RelayCommand(Refresh);
            RenameCommand = new RelayCommand(RenameVolume, IsSelectedVolumeValid);
            BitLockCommand = new RelayCommand(LaunchBitLock);
            ReadOnlyCommand = new RelayCommand(SetReadOnly, IsSelectedVolumeValid);

            Refresh();
        }

        /// <summary>
        /// Calls appropriate method to change the mount state of a volume
        /// </summary>
        public void ChangeMountState()
        {
            if (SelectedVolume.IsMounted())
                EjectVolume();
            else
                MountVolume();
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
        /// Refreshes Volumes
        /// </summary>
        public void Refresh()
        {
            if (DiskpartProcess.GetVolumes(ref volumes) != ProcessExitCode.Ok)
                ShowError("Refresh - GetVolumes");
            else
                if (DiskpartProcess.GetReadOnlyState(ref volumes) != ProcessExitCode.Ok)
                ShowError("Refresh - GetReadOnlyState");
            OnPropertyChanged(nameof(Volumes));
        }

        /// <summary>
        /// Renames SelectedVolume
        /// </summary>
        public void RenameVolume()
        {
            RenameWindowViewModel rwvm = new RenameWindowViewModel(ref selected);
            RenameWindow window = new RenameWindow();
            window.DataContext = rwvm;
            window.ShowDialog();
            Refresh();
        }

        /// <summary>
        /// Launches BitLock Window
        /// </summary>
        public void LaunchBitLock()
        {
            BitLockProcess.LaunchBitLock();
        }

        /// <summary>
        /// Calls appropriate method to change the read-only state of a volume
        /// </summary>
        public void SetReadOnly()
        {
            if (SelectedVolume.IsReadOnly())
                SetReadOnly(ReadOnlyFunction.CLEAR);
            else
                SetReadOnly(ReadOnlyFunction.SET);

            Refresh();
        }

        /// <summary>
        /// Shows appropriate confirmation dialog then calls DiskpartProcess.SetReadOnly
        /// </summary>
        /// <param name="function"></param>
        private void SetReadOnly(ReadOnlyFunction function)
        {
            if (MessageHelper.ShowConfirm("Are you sure you want to " + function + " the read-only flag on " + SelectedVolume.Info + "?") == MessageBoxResult.Yes)
            {
                if (function == ReadOnlyFunction.SET)
                {
                    if (DiskpartProcess.SetReadOnly(SelectedVolume) != ProcessExitCode.Ok)
                        ShowError(nameof(SetReadOnly));
                }
                else
                {
                    if (DiskpartProcess.ClearReadOnly(SelectedVolume) != ProcessExitCode.Ok)
                        ShowError(nameof(SetReadOnly));
                }
            }

        }

        /// <summary>
        /// Checks SelectedVolume.IsRemovalbe
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public bool IsSelectedVolumeRemovable(object o)
        {
            if (SelectedVolume == null)
                return false;
            return SelectedVolume.IsRemovable();
        }

        /// <summary>
        /// Checks SelectedVolume.IsValid
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public bool IsSelectedVolumeValid(object o)
        {
            if (SelectedVolume == null)
                return false;
            return SelectedVolume.IsValid();
        }

        /// <summary>
        /// Shows a MessageBox with an error
        /// </summary>
        /// <param name="callingfrom">Where the error happend</param>
        private void ShowError(string callingfrom)
        {
            MessageHelper.ShowError(callingfrom, DiskpartProcess.ExitCode, DiskpartProcess.StdError, DiskpartProcess.StdOutput);
        }

    }
}
