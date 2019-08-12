using DiskpartGUI.Models;
using System.Collections.Generic;

namespace DiskpartGUI.ViewModels
{
    class MainWindowViewModel
    {

        public List<Volume> Volumes { get; set; }

        public MainWindowViewModel()
        {
            Volumes = new List<Volume>()
            {
                new Volume
                    {
                        Number = 0,
                        Letter = 'A',
                        Label = "New Volume",
                        FileSystem = FileSystem.FAT32,
                        VolumeType = VolumeType.Removable,
                        Size = 45,
                        VolumeSizePrefix = VolumeSizePrefix.GB,
                        Status = VolumeStatus.Healthy
                    },

                new Volume
                    {
                        Number = 1,
                        Letter = 'B',
                        Label = "Test",
                        FileSystem = FileSystem.FAT32,
                        VolumeType = VolumeType.Removable,
                        Size = 45,
                        VolumeSizePrefix = VolumeSizePrefix.GB,
                        Status = VolumeStatus.Healthy
                    },
            };
        }

    }
}
