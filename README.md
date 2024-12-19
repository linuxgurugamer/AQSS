A.Q.S.S. - AutoQuickSave System

This is a small mod to do quicksaves on an automatic, scheduled basis.  It can also do a quicksave when you launch a vessel.

Features

Selectable interval for quicksaves
Quicksave on Launch
Templated names for the quicksave files
Automatic expiration of old quicksaves based on specified criteria
Audio file playable when a quicksave is done

Usage

Click the button [buttonImage]

There are two buttons, one for the quicksave options, and one for the Sound Options

The filenames are created using a template.  The template for both Launch and Quicksave are shown, and the line right below each is showing the final result.  Click the Template Info button to read about the available tokens which can be used in the templates

Automatic Purging of old files

The bottom three lines are the criteria to determine when to delete files.  Two things to note:

1. Launch quicksaves are never purged.
2. The mod will only delete files which match the prefix AutoQSave_ and which fit the rest of the criteria


Sound Options

An audio file can be played automatically whenever a quicksave is done.  The available sounds are listed in a list, and you can add more.  To add your own sounds, copy the audio file into the folder:  AutoQuickSaveSystem/Audio, close and open the window and the new files will be shown in the list.
The currently selected audio file is highlighted with green letters.  Click the little triangle at the right of each line to hear the sound the audio file plays


Special note regarding Quicksave on launch

This depends on the initial staging event of a newly launched vessel.  What this means is that if you launch a rover and drive it away, no quicksave will be done of that launch

Note regarding the voices


Special note regarding the S.A.V.E mod
The S.A.V.E. mod renames save files and quicksave files.  You need to disable the autosaves to prevent it from interfering with this mod

Availability

Github: https://github.com/linuxgurugamer/AutoQuickSaveSystem
Download: https://spacedock.info/mod/2475/AQSS - Auto-Quick-Save-System
License: GPLv3
Now available in CKAN

Questions & Answers

How is this different from S.A.V.E.  ?
S.A.V.E does full backups of the save, including craft files, and whatever else is in the save directory.  This does a quicksave, meaning that it quicksaves the game into a new persistent sfs & meta file.  Quicksaves can be loaded immediately inside the game, S.A.V.E. backups can't, you need to restore a backup, which restores everything from the backup for the selected save
Dependencies

Click Through Blocker
ToolbarController
SpaceTuxLibrary
 

Still in beta, and already reviewed by Kottabos Games:

https://youtu.be/1tO10-HU-UA


Main config window

https://i.imgur.com/kxbJ3RF.png

 

Audio configuration window

https://i.imgur.com/Uus7Alm.png

 

Template Info Window

https://i.imgur.com/dJ086Q6.png

 

Acknowledgements

This was originally going to be part of the S.A.V.E. mod, made by @Nereid.  After I had written it, I found that @Nereid had updated his Github repo, which seemed to indicate that he was about to  update the mod.  So I pulled all the code related to backups out, and made this a new mod.  The SAVE mod has a GPL license, so I am using that to avoid any possible license issues