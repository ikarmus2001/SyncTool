# Usage
```powershell
> .\SyncTool.CLI.exe sync
Usage: sync [options...] [-h|--help] [--version]

Start a sync operation on a schedule

Options:
  --source <string>              Path to the directory, which will be the basepoint for syncing (Required)
  --replica <string>             Target directory, where files will be synced to (Required)
  --cron <string>                Cron expression for scheduling the sync operation
         Every two minutes by default (Default: @"*/2 * * * *")
  --create-replica-dir <bool>    Set to false to disable auto-creation of the replica directory (Default: true)
```

 This software synchronizes two folders: `source` and `replica`.
# Features
## 1. Identical
Maintains a full, identical copy of source folder at replica folder.
## 2. One-way sync
content of the `replica` folder should be modified to exactly match content of the `source` folder
## 3. Periodical sync support

## 4. Operations logging 
File creation/copying/removal operations should be logged to a file and to the console output
## 5. CLI-based
Folder paths, synchronization interval and log file path should be provided using
the command line arguments

