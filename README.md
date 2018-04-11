# Git Basic
A Git client for Windows desktop. You can download the installer for version 1.0.0 [here](Released/v1.0.0/GitBasic.msi).

![Git Basic Screenshot](Documentation/Screenshots/GitBasicScreenshot.png?raw=true "Git Basic")

## Feature Guide
Git Basic is easy to use. It’s not bloated with features. In fact, that’s why it’s called Git Basic. Here is a brief overview of the primary features of Git Basic.

### Integrated Terminal
<img align="left" src="https://github.com/MattTheMan/git-basic/blob/develop/Documentation/Screenshots/Terminal.png">
Type Git commands here. This is a cmd.exe terminal control, so you can enter more than just Git commands here.

### Status Bar
![Status Bar](Documentation/Screenshots/StatusBar.png?raw=true "Status Bar")
Click the repository name (on the left side of the status bar) to reveal a directory selector. You can navigate to a different repository from here.
Click the branch name (on the right side of the status bar) to reveal a list of all local branches in this repository. You can switch branches by selecting one.

### Command Buttons
![Command Buttons](Documentation/Screenshots/CommandButtons.png?raw=true "Command Buttons")
These are command buttons to quickly execute Git commands. When you click one, you will see the output in the terminal. Some execute the command for you. Others just paste the command text in the input box, so that you can complete the command. Hold down the Ctrl key to reveal hotkeys for these buttons.

### File Status Control
![File Status Control](Documentation/Screenshots/FileStatus.png?raw=true "File Status Control")
The file status control shows you which files are staged or unstaged. If you select an unstaged file from the tree view, you will see its diff in the diff viewer. You can drag and drop files or directories to and from Staged Changes and Unstaged Changes in order to stage or unstage.

### Diff Viewer
![Diff Viewer](Documentation/Screenshots/DiffViewer.png?raw=true "Diff Viewer")
This is the diff viewer for the file which is currently selected in the file status control. The left side shows you the old state of the file and the right side shows you the new state.
