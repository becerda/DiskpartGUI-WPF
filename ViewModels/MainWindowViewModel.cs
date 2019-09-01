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
        private string embstate;
        private string scrostate;
        private BaseMedia selecteditem;
        private string selectedinfo;
        private List<BaseMedia> listviewsource;

        private bool masterbuttonsenabled = true;

        private bool showvolumes;

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
        /// Is ListViewVolumes currently showing?
        /// </summary>
        public bool ShowVolumes
        {
            get
            {
                return showvolumes;
            }
            set
            {
                showvolumes = value;
                OnPropertyChanged(nameof(ShowVolumes));
                OnPropertyChanged(nameof(VolumeListVisibility));
                OnPropertyChanged(nameof(DiskListVisibility));
                OnPropertyChanged(nameof(ShowOpositeViewMenuItemText));
            }
        }

        /// <summary>
        /// The Visibility of ListViewVolumes
        /// </summary>
        public Visibility VolumeListVisibility
        {
            get
            {
                if (ShowVolumes)
                    return Visibility.Visible;
                return Visibility.Hidden;
            }
        }

        /// <summary>
        /// The Visibility of ListViewDisks
        /// </summary>
        public Visibility DiskListVisibility
        {
            get
            {
                if (ShowVolumes)
                    return Visibility.Hidden;
                return Visibility.Visible;
            }
        }

        /// <summary>
        /// Menu Item text to switch between ListViewVolumes and ListViewDisks
        /// </summary>
        public string ShowOpositeViewMenuItemText
        {
            get
            {
                if (ShowVolumes)
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
                    if (ShowVolumes)
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
                if (ShowVolumes)
                    selectedinfo = "Selected Volume: " + value;
                else
                    selectedinfo = "Selected Disk: " + value;
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

        public RelayCommand ToggleListViewCommand { get; private set; }

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
            ShowVolumes = Properties.Settings.Default.ShowVolumes;

            ListViewSource = new List<BaseMedia>();

            ChangeMountStateCommand = new RelayCommand(ChangeMountState, CanBeEjected, Key.E, ModifierKeys.Control);
            RefreshCommand = new RelayCommand(Refresh, Key.R, ModifierKeys.Control);
            RenameCommand = new RelayCommand(RenameVolume, CanBeRenamed, Key.F2);
            BitLockCommand = new RelayCommand(LaunchBitLock, Key.B, ModifierKeys.Control);
            ReadOnlyCommand = new RelayCommand(SetReadOnly, CanToggleReadOnlyFlag, Key.R, ModifierKeys.Control | ModifierKeys.Shift);
            FormatCommand = new RelayCommand(FormatVolume, IsSelectedItemValid, Key.F, ModifierKeys.Control);
            CloseWindowCommand = new RelayCommand(RequestWindowClose, Key.Q, ModifierKeys.Control);
            ShowAllVolumesCommand = new RelayCommand(ToggleShowAllVolumes, CanToggleShowAllVolumes, Key.S, ModifierKeys.Control | ModifierKeys.Shift);
            ToggleListViewCommand = new RelayCommand(ToggleListViews);
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

            if (ShowVolumes)
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
            MediaType type;
            if (SelectedItem.IsReadOnly())
                function = ReadOnlyFunction.CLEAR;
            else
                function = ReadOnlyFunction.SET;

            if (ShowVolumes)
                type = MediaType.VOLUME;
            else
                type = MediaType.DISK;

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
            return ShowVolumes;
        }

        /// <summary>
        /// Toggles between ListViewVolumes and ListViewDisks
        /// </summary>
        private void ToggleListViews()
        {
            ShowVolumes = !ShowVolumes;
            Refresh();
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
        /// Checks SelectedItem.IsValid
        /// </summary>
        /// <param name="o">The SelectedItem</param>
        /// <returns>Whether the SelectedItem is valid</returns>
        public bool IsSelectedItemValid(object o)
        {

            if (masterbuttonsenabled == false)
                return false;

            if (o == null)
                return false;
            return ((BaseMedia)o).IsValid();
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
            MediaType type;
            if (ShowVolumes)
                type = MediaType.VOLUME;
            else
                type = MediaType.DISK;

            task = Task.Run(() => DiskpartProcess.RunListCommand(ref listviewsource, type));
            ProcessExitCode result = await task;

            if (result != ProcessExitCode.Ok)
            {
                if (ShowVolumes)
                    ShowError("Refresh - GetVolumes");
                else
                    ShowError("Refresh - GetDisks");
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
