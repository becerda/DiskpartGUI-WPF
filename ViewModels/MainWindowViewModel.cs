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
        /// <summary>
        /// The states of which view is being show in the ListView
        /// </summary>
        private enum ListViewHeaderState
        {
            ShowingVolumes,
            ShowingPartitions,
            ShowingDisks
        }

        private const string Disks_Header_Param = "Disk";
        private const string Partitions_Header_Param = "Partition";
        private const string Volumes_Header_Param = "Volume";

        private string embstate;
        private string scrostate;
        private BaseMedia selecteditem;
        private int selectedindex;
        private int lastselectedindex;
        private string selectedinfo;
        private List<BaseMedia> listviewsource;
        private bool masterbuttonsenabled = true;
        private ListViewHeaderState lvstate;

        private List<BaseMedia> disks;
        private List<BaseMedia> partitions;
        private List<BaseMedia> volumes;

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
        /// The currently selected media item in the ListViewVolumes
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
                        if (value.IsMounted())
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
        /// The currently selected index of the ListView
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return selectedindex;
            }
            set
            {
                selectedindex = value;

                if (value > 0)
                    lastselectedindex = value;

                OnPropertyChanged(nameof(SelectedIndex));
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
        public RelayCommand ShowVolumesCommand { get; private set; }

        /// <summary>
        /// The Command to show the headers associated with Disks
        /// </summary>
        public RelayCommand ShowDisksCommand { get; private set; }

        /// <summary>
        /// The Command to show the headers associated with Partitions
        /// </summary>
        public RelayCommand ShowPartitionsCommand { get; private set; }

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

            ChangeMountStateCommand = new RelayCommand(ChangeMountState, CanBeEjected, Key.E, ModifierKeys.Control);
            RenameCommand = new RelayCommand(RenameVolume, CanBeRenamed, Key.F2);
            ReadOnlyCommand = new RelayCommand(SetReadOnly, CanToggleReadOnlyFlag, Key.R, ModifierKeys.Control | ModifierKeys.Shift);
            FormatCommand = new RelayCommand(FormatVolume, CanBeFormated, Key.F, ModifierKeys.Control);

            RefreshCommand = new RelayCommand(Refresh, Key.R, ModifierKeys.Control);
            BitLockCommand = new RelayCommand(LaunchBitLock, Key.B, ModifierKeys.Control);

            ShowAllVolumesCommand = new RelayCommand(ToggleShowAllVolumes, CanToggleShowAllVolumes, Key.S, ModifierKeys.Control | ModifierKeys.Shift);

            ShowVolumesCommand = new RelayCommand(ChangeListView, CanShowVolumesListView, Key.D1);
            ShowDisksCommand = new RelayCommand(ChangeListView, CanShowDisksListView, Key.D2);
            ShowPartitionsCommand = new RelayCommand(ChangeListView, CanShowPartitionListView, Key.D3);

            AboutCommand = new RelayCommand(ShowAboutWindow, Key.F1);

            CloseWindowCommand = new RelayCommand(RequestWindowClose, Key.Q, ModifierKeys.Control);

        }

        /// <summary>
        /// Loads the contents of the ListView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnWindowLoaded(object sender, EventArgs e)
        {
            // TO-DO: Load last ListView from settings
            ChangeListView(Disks_Header_Param);
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
        /// Calls the appropriate type of refresh based on the ListViewHeaderState
        /// </summary>
        public async void Refresh()
        {
            ListViewSource = null;
            switch (lvstate)
            {
                case ListViewHeaderState.ShowingDisks:
                    await Refresh(StorageType.DISK);
                    ListViewSource = disks;
                    break;
                case ListViewHeaderState.ShowingPartitions:
                    await Refresh(StorageType.PARTITION);
                    ListViewSource = partitions;
                    break;
                case ListViewHeaderState.ShowingVolumes:
                    await Refresh(StorageType.VOLUME);
                    ListViewSource = volumes;
                    break;
            }
            OnPropertyChanged(nameof(ListViewSource));
        }

        /// <summary>
        /// Refreshes appropriate list of media items
        /// </summary>
        public async Task Refresh(StorageType type)
        {
            masterbuttonsenabled = false;

            if (type == StorageType.DISK)
            {
                if (disks != null)
                {
                    disks.Clear();
                    disks = null;
                }
            }
            else if (type == StorageType.PARTITION)
            {
                if (partitions != null)
                {
                    partitions.Clear();
                    partitions = null;
                }
            }
            else
            {
                if (volumes != null)
                {
                    volumes.Clear();
                    volumes = null;
                }
            }

            WaitWindow window = new WaitWindow { DataContext = new WaitWindowViewModel("Refreshing " + type.ToString().ToCharArray()[0] + type.ToString().Substring(1).ToLower() + "s") };
            window.Show();

            await CallRefresh(type);

            masterbuttonsenabled = true;
            window.Close();
            if (lvstate == ListViewHeaderState.ShowingVolumes)
            {
                ListViewSource = volumes;
                if (!ShowAllVolumes)
                    FilterRemovableVolumes();
            }

            SelectedItemInfo = "";
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
            {
                Refresh();
            }
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
            StorageType type;
            if (SelectedItem.IsReadOnly())
                function = ReadOnlyFunction.CLEAR;
            else
                function = ReadOnlyFunction.SET;

            if (lvstate == ListViewHeaderState.ShowingVolumes)
                type = StorageType.VOLUME;
            else
                type = StorageType.DISK;

            if (MessageHelper.ShowConfirm("Are you sure you want to " + function + " the read-only flag on " + SelectedItem.ToString() + "?") == MessageBoxResult.Yes)
            {

                if (DiskpartProcess.SetReadOnly(SelectedItem, function, type) != ProcessExitCode.Ok)
                {
                    ShowError(nameof(SetReadOnly));
                }
                else
                {
                    Refresh();
                }
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
            {
                Refresh();
            }
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
            bool refresh = false;
            if (s == Volumes_Header_Param)
            {
                lvstate = ListViewHeaderState.ShowingVolumes;
                ListViewSource = volumes;
                if (volumes == null)
                    refresh = true;
            }
            else if (s == Disks_Header_Param)
            {
                lvstate = ListViewHeaderState.ShowingDisks;
                ListViewSource = disks;
                if (disks == null)
                    refresh = true;
            }
            else if (s == Partitions_Header_Param)
            {
                lvstate = ListViewHeaderState.ShowingPartitions;
                ListViewSource = partitions;
                refresh = true;
            }
            else
            {
                MessageHelper.ShowError(nameof(ChangeListView), ProcessExitCode.Error, "No ListViewHeaderState selected.");
                return;
            }
            SelectedItemInfo = string.Empty;
            if (refresh)
                Refresh();

            OnPropertyChanged(nameof(VolumeVisibility));
            OnPropertyChanged(nameof(DiskVisibility));
            OnPropertyChanged(nameof(PartitionVisibility));
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
        private async Task CallRefresh(StorageType type)
        {
            Task<ProcessExitCode> task;
            if (type == StorageType.DISK)
                task = Task.Run(() => DiskpartProcess.GetDiskInfo(ref disks));
            else if (type == StorageType.PARTITION)
                task = Task.Run(() => DiskpartProcess.GetPartitionInfo(disks[lastselectedindex].Number, ref partitions));
            else
                task = Task.Run(() => DiskpartProcess.GetVolumeInfo(ref volumes));

            ProcessExitCode result = await task;

            if (result != ProcessExitCode.Ok)
            {
                ShowError("Refresh - Refresh - " + type);
            }
        }

    }
}
