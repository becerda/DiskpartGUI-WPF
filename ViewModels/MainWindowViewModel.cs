using DiskpartGUI.Commands;
using DiskpartGUI.Helpers;
using DiskpartGUI.Models;
using DiskpartGUI.Processes;
using DiskpartGUI.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DiskpartGUI.ViewModels
{
    class MainWindowViewModel : ClosablePropertyChangedViewModel
    {
        private enum ListViewHeaderState
        {
            ShowingVolumes,
            ShowingPartitions,
            ShowingDisks
        }

        private string embstate;
        private string scrostate;
        private BaseMedia selecteditem;
        private string selectedinfo;
        private List<BaseMedia> listviewsource;
        private bool masterbuttonsenabled = true;
        private ListViewHeaderState lvstate;

        /// <summary>
        /// Lazy Instantiation of a DiskpartProcess
        /// </summary>
        private readonly Lazy<DiskpartProcess> ldpp = new Lazy<DiskpartProcess>(() => new DiskpartProcess());

        /// <summary>
        /// The source of ListViewVolumes and ListViewDisks
        /// </summary>
        public List<BaseMedia> ListViewSource
        {
            get
            {
                return listviewsource;
            }
            set
            {
                listviewsource = value;
                OnPropertyChanged(nameof(ListViewSource));
            }
        }

        /// <summary>
        /// Menu Item text to switch between ListViewVolumes and ListViewDisks
        /// </summary>
        public string ShowOpositeViewMenuItemText
        {
            get
            {
                if (lvstate == ListViewHeaderState.ShowingVolumes)
                    return "Show Disks";
                return "Show Volumes";
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
        public BaseMedia SelectedItem
        {
            get
            {
                return selecteditem;
            }
            set
            {
                selecteditem = value;
                if (value != null)
                {
                    SelectedItemInfo = value.ToString();
                    if (lvstate == ListViewHeaderState.ShowingVolumes)
                    {
                        if (((Volume)value).IsMounted())
                            EjectMountButtonContent = "Eject";
                        else
                            EjectMountButtonContent = "Mount";
                    }
                    if (value.IsReadOnly())
                        SetClearReadOnlyButtonContent = "Clear Read-Only";
                    else
                        SetClearReadOnlyButtonContent = "Set Read-Only";
                }
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        /// <summary>
        /// The ToString of SelectedVolume
        /// </summary>
        public string SelectedItemInfo
        {
            get
            {
                return selectedinfo;
            }
            set
            {
                if (lvstate == ListViewHeaderState.ShowingVolumes)
                    selectedinfo = "Selected Volume: " + value;
                else if (lvstate == ListViewHeaderState.ShowingDisks)
                    selectedinfo = "Selected Disk: " + value;
                else
                    selectedinfo = "Selected Partition: " + value;

                OnPropertyChanged(nameof(SelectedItemInfo));
            }
        }

        /// <summary>
        /// The option to show all volumes
        /// </summary>
        public bool ShowAllVolumes
        {
            get
            {
                return Properties.Settings.Default.ShowAllVolumes;
            }
            set
            {
                Properties.Settings.Default.ShowAllVolumes = value;
                OnPropertyChanged(nameof(ShowAllVolumes));
            }
        }

        /// <summary>
        /// The boolean to show the headers associated with Volumes
        /// </summary>
        public bool VolumeVisibility
        {
            get
            {
                if (lvstate == ListViewHeaderState.ShowingVolumes)
                    return true;
                return false;
            }
        }

        /// <summary>
        /// The boolean to show the headers associated with Partitions
        /// </summary>
        public bool PartitionVisibility
        {
            get
            {
                if (lvstate == ListViewHeaderState.ShowingPartitions)
                    return true;
                return false;
            }
        }

        /// <summary>
        /// The boolean to show the headers associated with Disks
        /// </summary>
        public bool DiskVisibility
        {
            get
            {
                if (lvstate == ListViewHeaderState.ShowingDisks)
                    return true;
                return false;
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
        /// Format Command for ButtonFormat
        /// </summary>
        public RelayCommand FormatCommand { get; private set; }

        /// <summary>
        /// Show All Volumes Command for View menu
        /// </summary>
        public RelayCommand ShowAllVolumesCommand { get; private set; }

        /// <summary>
        /// The Command to show the headers associated with Volumes
        /// </summary>
        public RelayCommand ShowVolumesListViewCommand { get; private set; }

        /// <summary>
        /// The Command to show the headers associated with Disks
        /// </summary>
        public RelayCommand ShowDisksListViewCommand { get; private set; }

        /// <summary>
        /// The Command to show the headers associated with Partitions
        /// </summary>
        public RelayCommand ShowPartitionCommand { get; private set; }

        /// <summary>
        /// Shows the About window
        /// </summary>
        public RelayCommand AboutCommand { get; private set; }

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
            SelectedItemInfo = "";
            ShowAllVolumes = Properties.Settings.Default.ShowAllVolumes;

            // TO-DO: Load Last ListView From Settings
            lvstate = ListViewHeaderState.ShowingVolumes;

            ListViewSource = new List<BaseMedia>();

            ChangeMountStateCommand = new RelayCommand(ChangeMountState, CanBeEjected, Key.E, ModifierKeys.Control);
            RefreshCommand = new RelayCommand(Refresh, Key.R, ModifierKeys.Control);
            RenameCommand = new RelayCommand(RenameVolume, CanBeRenamed, Key.F2);
            BitLockCommand = new RelayCommand(LaunchBitLock, Key.B, ModifierKeys.Control);
            ReadOnlyCommand = new RelayCommand(SetReadOnly, CanToggleReadOnlyFlag, Key.R, ModifierKeys.Control | ModifierKeys.Shift);
            FormatCommand = new RelayCommand(FormatVolume, CanBeFormated, Key.F, ModifierKeys.Control);
            CloseWindowCommand = new RelayCommand(RequestWindowClose, Key.Q, ModifierKeys.Control);

            ShowAllVolumesCommand = new RelayCommand(ToggleShowAllVolumes, CanToggleShowAllVolumes, Key.S, ModifierKeys.Control | ModifierKeys.Shift);

            ShowVolumesListViewCommand = new RelayCommand(ChangeListView, CanShowVolumesListView);
            ShowDisksListViewCommand = new RelayCommand(ChangeListView, CanShowDisksListView);
            ShowPartitionCommand = new RelayCommand(ChangeListView, CanShowPartitionListView);

            AboutCommand = new RelayCommand(ShowAboutWindow, Key.F1);

            Refresh();
        }

        /// <summary>
        /// Calls appropriate method to change the mount state of a volume
        /// </summary>
        public void ChangeMountState()
        {
            masterbuttonsenabled = false;
            if (SelectedItem.IsMounted())
                EjectVolume();
            else
                MountVolume();
            masterbuttonsenabled = true;
        }

        /// <summary>
        /// Ejects the SelectedVolume
        /// </summary>
        public void EjectVolume()
        {
            if (MessageBox.Show("Are you sure you want to eject " + SelectedItem.ToString() + "?",
            "Eject Volume " + ((Volume)SelectedItem).DriveLetter, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (DiskpartProcess.EjectVolume(SelectedItem) != ProcessExitCode.Ok)
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
            if (DiskpartProcess.AssignVolumeLetter(SelectedItem) != ProcessExitCode.Ok)
                ShowError("MountVolume - AssignVolumeLetter");
            else
            {
                Refresh();
            }
        }

        /// <summary>
        /// Refreshes Volumes
        /// </summary>
        public async void Refresh()
        {
            masterbuttonsenabled = false;
            WaitWindow window = new WaitWindow { DataContext = new WaitWindowViewModel("Refreshing") };
            window.Show();
            await CallRefresh();
            masterbuttonsenabled = true;
            window.Close();

            if (lvstate == ListViewHeaderState.ShowingVolumes)
            {
                if (!ShowAllVolumes)
                    FilterRemovableVolumes();
            }

            SelectedItemInfo = "";
            OnPropertyChanged(nameof(ListViewSource));
        }

        /// <summary>
        /// Renames SelectedVolume
        /// </summary>
        public void RenameVolume()
        {
            RenameWindowViewModel rwvm = new RenameWindowViewModel(ref selecteditem);
            RenameWindow window = new RenameWindow(rwvm);
            window.ShowDialog();
            if (rwvm.ExitStatus == ExitStatus.Applied)
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
            masterbuttonsenabled = false;
            ReadOnlyFunction function;
            Processes.StorageType type;
            if (SelectedItem.IsReadOnly())
                function = ReadOnlyFunction.CLEAR;
            else
                function = ReadOnlyFunction.SET;

            if (lvstate == ListViewHeaderState.ShowingVolumes)
                type = Processes.StorageType.VOLUME;
            else
                type = Processes.StorageType.DISK;

            if (MessageHelper.ShowConfirm("Are you sure you want to " + function + " the read-only flag on " + SelectedItem.ToString() + "?") == MessageBoxResult.Yes)
            {

                if (DiskpartProcess.SetReadOnly(SelectedItem, function, type) != ProcessExitCode.Ok)
                    ShowError(nameof(SetReadOnly));
                Refresh();
            }
            masterbuttonsenabled = true;
        }

        /// <summary>
        /// Formats SelectedVolume 
        /// </summary>
        private void FormatVolume()
        {
            FormatWindowViewModel fwvm = new FormatWindowViewModel(ref selecteditem);
            FormatWindow window = new FormatWindow(fwvm);
            window.ShowDialog();
            if (fwvm.ExitStatus == ExitStatus.Applied)
                Refresh();
        }

        /// <summary>
        /// Toggles the filter for showing all volumes
        /// </summary>
        private void ToggleShowAllVolumes()
        {
            ShowAllVolumes = !ShowAllVolumes;
            Refresh();
        }

        /// <summary>
        /// Checks SelectedItem.CanToggleShowAllVolumes
        /// </summary>
        /// <param name="o">The SelectedItem</param>
        /// <returns>Whether the SelectedItem can be renamed</returns>
        private bool CanToggleShowAllVolumes(object o)
        {
            return lvstate == ListViewHeaderState.ShowingVolumes;
        }

        /// <summary>
        /// Changes the headers to the appropriate list
        /// </summary>
        private void ChangeListView(object o)
        {
            string s = (string)o;

            if (s == "Volume")
                lvstate = ListViewHeaderState.ShowingVolumes;
            else if (s == "Disk")
                lvstate = ListViewHeaderState.ShowingDisks;
            else
                lvstate = ListViewHeaderState.ShowingPartitions;

            OnPropertyChanged(nameof(VolumeVisibility));
            OnPropertyChanged(nameof(DiskVisibility));
            OnPropertyChanged(nameof(PartitionVisibility));

            Refresh();
        }

        /// <summary>
        /// Can the Volumes headers be shown?
        /// </summary>
        /// <param name="o"></param>
        /// <returns>Whether the Volume headers can be shown</returns>
        private bool CanShowVolumesListView(object o)
        {
            if (VolumeVisibility)
                return false;
            return true;
        }

        /// <summary>
        /// Can the Disk headers be shown?
        /// </summary>
        /// <param name="o"></param>
        /// <returns>Whether the Disk headers can be shown</returns>
        private bool CanShowDisksListView(object o)
        {
            if (DiskVisibility)
                return false;
            return true;
        }

        /// <summary>
        /// Can the Partition headers be shown?
        /// </summary>
        /// <param name="o"></param>
        /// <returns>Whether the Partition headers can be shown</returns>
        private bool CanShowPartitionListView(object o)
        {
            if (DiskVisibility == true && SelectedItem != null)
                return true;
            return false;
        }

        /// <summary>
        /// Filters out the non-removable volumes
        /// </summary>
        private void FilterRemovableVolumes()
        {
            Volume[] temp = new Volume[ListViewSource.Count];
            ListViewSource.CopyTo(temp);
            foreach (Volume v in temp)
            {
                if (!v.IsRemovable())
                    ListViewSource.Remove(v);
            }
        }

        /// <summary>
        /// Shows the About window
        /// </summary>
        private void ShowAboutWindow()
        {
            new AboutWindow().ShowDialog();
        }

        /// <summary>
        /// Checks SelectedItem.IsRemovalbe
        /// </summary>
        /// <param name="o">The SelectedItem</param>
        /// <returns>Whether the SelectedItem is removable</returns>
        public bool IsSelectedItemRemovable(object o)
        {
            if (masterbuttonsenabled == false)
                return false;

            if (o == null)
                return false;
            return ((BaseMedia)o).IsRemovable();
        }

        /// <summary>
        /// Checks SelectedItem.CanToggleReadOnlyFlag
        /// </summary>
        /// <param name="o">The SelectedItem</param>
        /// <returns>Whether the SelectedItem can toggle a read only flag</returns>
        public bool CanToggleReadOnlyFlag(object o)
        {
            if (masterbuttonsenabled == false)
                return false;

            if (o == null)
                return false;
            return ((BaseMedia)o).CanToggleReadOnly();
        }

        /// <summary>
        /// Checks SelectedItem.CanBeRenamed
        /// </summary>
        /// <param name="o">The SelectedItem</param>
        /// <returns>Whether the SelectedItem can be renamed</returns>
        public bool CanBeRenamed(object o)
        {
            if (masterbuttonsenabled == false)
                return false;

            if (o == null)
                return false;
            return ((BaseMedia)o).CanBeRenamed();
        }

        /// <summary>
        /// Checks SelectedItem.CanBeEjected
        /// </summary>
        /// <param name="o">The SelectedItem</param>
        /// <returns>Whether the SelectedItem can be ejected</returns>
        public bool CanBeEjected(object o)
        {
            if (masterbuttonsenabled == false)
                return false;

            if (o == null)
                return false;
            return ((BaseMedia)o).CanBeEjected();
        }

        /// <summary>
        /// Checks SelectedItem.CanBeFormated
        /// </summary>
        /// <param name="o">The SelectedItem</param>
        /// <returns>Whether the SelectedItem can be formated</returns>
        public bool CanBeFormated(object o)
        {
            if (masterbuttonsenabled == false)
                return false;
            if (o == null)
                return false;
            return ((BaseMedia)o).CanBeFormated();
        }

        /// <summary>
        /// Shows a MessageBox with an error
        /// </summary>
        /// <param name="callingfrom">Where the error happend</param>
        private void ShowError(string callingfrom)
        {
            MessageHelper.ShowError(callingfrom, DiskpartProcess.ExitCode, DiskpartProcess.StdError, DiskpartProcess.StdOutput);
        }

        /// <summary>
        /// AsyncTask to call Refresh method
        /// </summary>
        /// <returns></returns>
        private async Task CallRefresh()
        {
            Task<ProcessExitCode> task;
            Processes.StorageType type;
            if (lvstate == ListViewHeaderState.ShowingVolumes)
                type = Processes.StorageType.VOLUME;
            else if (lvstate == ListViewHeaderState.ShowingDisks)
                type = Processes.StorageType.DISK;
            else
                type = Processes.StorageType.PARTITION;

            task = Task.Run(() => DiskpartProcess.RunListCommand(ref listviewsource, type, (SelectedItem == null ? -1 : SelectedItem.Number)));
            ProcessExitCode result = await task;

            if (result != ProcessExitCode.Ok)
            {
                if (lvstate == ListViewHeaderState.ShowingVolumes)
                    ShowError("Refresh - GetVolumes");
                else if (lvstate == ListViewHeaderState.ShowingDisks)
                    ShowError("Refresh - GetDisks");
                else
                    ShowError("Refresh - GetPartitions");
            }
            else
            {
                task = Task.Run(() => DiskpartProcess.GetAttributes(ref listviewsource, type));
                result = await task;
                if (result != ProcessExitCode.Ok)
                {
                    ShowError("Refresh - GetReadOnlyState");
                }
            }
        }

    }
}
