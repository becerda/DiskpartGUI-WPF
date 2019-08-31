# DiskpartGUI
GUI for Windows Diskpart
Version:

## Installation
To Do

## How To Run
To Do

## Write-Up
This README will go over the commands used to preform the background tasks of various processes.

## Features
1) List Of All Drives
2) Quickly Change Attributes
3) Safely Eject Drives
4) Format Removable Drives
4) Rename Volume
6) Activate BitLock

## Listing Of Found Drive
Diskpart command line interpreter by C#-WPF.NET framework will be used to display information.

### Diskpart
```bash
LIST VOLUME
```
	LIST - Command to list all disks, partitions, volume, or virtual disks
	VOLUME - Command used in conjunction with List to display all volumes

### Returns
	Volume Number
	Volume Letter
	Volume Label
	File System
	Type
	Size
	Status
	Info

### Displayed Info On GUI
Populate ListView with volumes in main window with:

	Volume Number
	Volume Letter
	Label
	File System
	Type
	Size
	Status
	Info
	Read-Only Flag

## Quickly Change Attributes
Diskpart command line executed by C# will be used to change attributes associated with selected volume.

### Diskpart
#### Command 1:
```bash
ATTRIBUTES DISK [SET | CLEAR] [READONLY] [NOERR] 
```
	ATTRIBUTES - Runs attribute command
	DISK - Supplies the selected disk.
	SET | CLEAR - Sets or clears the supplied attribute.
	READONLY - The ReadOnly attribute.
	NOERR - Flag for setting no error output

### Returns
Success or error message

### Displayed Info On GUI
A pop-up window will appear upon completion showing either success or error.

#### Command 2:
```bash
ATTRIBUTES DISK
```
	ATTRIBUTES - Runs attribute command
	DISK - Specific disk to get attributes of

### Returns
Attribute for selected disk.

### Displayed Info On GUI
A column in the previously mentioned ListView marked as "Set" or "Cleared".

## Safely Eject Drives
Diskpart command executed through C#, the selected drive will be unmounted and safe to remove.

### Diskpart
```bash
REMOVE ALL DISMOUNT
```
	REMOVE - command used to unmount a drive
	ALL - tells Diskpart to unmount all mounting points
	DISMOUNT - tells Diskpart to dismount the selected volume

### Returns
Diskpart returns a success or error message

### Displayed Info On GUI
Button below displayed drive with "Eject" or "Mount" on it.
A pop-up window will appear upon completion showing either success or error.

## Format Removable Drives
Diskpart command line interpreter by C# will be used to format removable drives.

### Diskpart
```bash
FORMAT [[FS=<FS> [REVISION=<X.XX>]] | RECOMMENDED]  [LABEL=<LABEL>] [UINT=<N>] [QUICK] [COMPRESS] [OVERRIDE] [DUPLICATE]
```
	FORMAT - Command to format volume
	FS=<FS> - Specifies the type of file system. If no file system is given, the default file system displayed by the FILESYSTEMS command is used.
	REVISION - Specifies the file system revision (if applicable).
	RECOMMENDED - The System recommended version file system to use
	LABEL=<LABEL> - Specifies the volume label.
	UINT=<N> - Overrides the default allocation unit size. Default settings are strongly recommended for general use. The default allocation unit size for a particular file system is displayed by the FILESYSTEMS command.
	QUICK - Performs a quick format.
	COMPRESS - NTFS only: Files created on the new volume will be compressed by default
	OVERRIDE - Forces the file system to dismount first if necessary. All open handles to the volume would no longer be valid.
	DUPLICATE - UDF Only: This flag applies to UDF format, version 2.5 or higher. This flag instructs the format operation to duplicate the file system meta-data to a second set of sectors on the disk. The duplicate meta-data is used by applications, for example repair or recovery applications. If the primary meta-data sectors are found to be corrupted, the file system meta-data will be read from the duplicate sectors.

### Returns
Diskpart will return either success or error message.

### Displayed Info On GUI
Button below displayed drive with “Format” on it.
A pop-up window will appear upon completion showing either successful or error.

## Rename Volume
Label command line interpreted by C# will be used to rename removable drives.

### Label
```bash
LABEL [DRIVE:\] [LABEL]
```
	LABEL - Runs Label command
	DRIVE:\ - The specified drive to rename
	LABEL - The new label name

### Returns
Label does not return anything

### Displayed Info On GUI
Button below displayed drive with “Rename” on it.
A pop-up window will appear upon completion showing either success or error.

## Activate BitLock
Control command executed through C# to open BitLock window.

### Control
```bash
CONTROL /NAME Microsoft.BitLockerDriveEncryption
```
	CONTROL - Tells window to open specific application
	/NAME - Name flag of control
	Microsoft.BitLockerDriveEncryption - BitLocker application to open

### Returns
Nothing is returned

### Displayed Info On GUI
Button below displayed drive with “BitLocker” on it.