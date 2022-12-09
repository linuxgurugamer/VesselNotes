Vessel Notes
=========

Notes for each vessel, available both in the VAB and in flight

Description

This mod is designed to let you enter notes and keep logs for each vessel.  Notes can be entered in the editor (VAB/SPH) and will carry over into flight.  

When in flight, you can keep notes and a log of the vessel.  When a vessel is recovered, the logs will be written out to files in the save directory of the game, in a directory called VesselLogs

This mod is added to every part with a ModuleCommand (ie:  pods, etc), but NOT the external seat.

Notes
Multiple notes can be added.  Notes added to one part will be shared/copied to all the other parts with the ModuleCommand module.  To keep a note private to a specific part, enable the Private flag.  Be warned, that once a note is marked Private it cannot be re-shared again; if you want to do that you will need to delete the note and add it again.  Notes can be deleted

Activation
The mod is available in the PAW (Part Action Window), and is called "Vessel Notes & Logs"

Logs
Log entries are generated based on the current vessel situation.  Beyond that, the log entries cannot be deleted, although they can be edited at will.  Log entries are shared with all parts on the vessel with ModuleCommand.

Docking
When two vessels dock, their individual notes and log entries will be preserved.  New entries will be shared, but the older ones will not.  When undocking, the same goes

This first picture shows the basic note screen, along with some of the specialized controls.  At the top there are the following:

* X - Close button, if in Vessel Log, don't return to the Notes
* Toggle Skin
* Change Font Size

Notes are listed in the left column.  The selected note will be in green, when selected the note title and text will be copied to the right

At the top are two buttons:

* New Note
* Vessel Log

The Vessel Log screen is similiar as you can see in further images below.  Functionality is the same, although more limited

Pre/Post game notes
		usage:
			Enable in the stock settings page in any save game.  Note:  This is an install-wide setting, will affect all saves
			When enabled the following is available
				When exiting a save to the MainMenu:
					A window will open up with the following controls:
						Button to toggle the window skin
						A main text input area
						Three buttons at the bottom to either cancel, clear or save & close

				When entering a save and there are notes previously saved:
					A window with the following controls:
						At the top:
							Button to toggle the window skin
							Button to toggle showing all notes or just the last
							If all notes being shown, then a button to toggle whether to display them in ascending order or descending order (using the game date as the sort)
							A button to copy the displayed notes to the clipboard, in the order displayed
						In the center, a scroll list showing either a single note or all the notes.  Each note will have a toggle which will hide that note
						At the bottom, a button to close the window
		Once saved, notes cannot be edited directly.  The pre/post release notes are saved in a file: 
			VesselNotes.cfg in the game save directory