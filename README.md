# ghoul
Attempts to resurrect window layouts and restart apps which derp on screen layout changes

# Installation
Head on over to the releases page for an installer or be brave and roll-your-own 
(instructions below)

# How to use?
Ghoul adds a systray icon which you can right-click to:
- Save a layout
    - Select a name for the layout
    - Select the windows you would like to be restored
    - Click "Ok"
- Restore a layout
    - Click the menu item with the name of the layout you'd like to restore
- Quick-restore
    - Double-click the tray icon to restore the last-restored layout, if any.

# Application doesn't manage windows with elevated privileges
Try using WinAero Tweaker to create an elevated shortcut. If you want this to run at startup, find the `shell:startup` folder (Start -> Run -> "shell:startup") and ensure you have a shortcut there.

# Some of my windows don't go back where they should
Are these windows running on a display with a different scaling from the display on which Ghoul is running? For example, is the display you're experiencing the issue on one with high pixel 
densitiy, such as a high-def laptop display? Ghoul currently can't deal with different scaling
between screens -- but I hope to fix that some day (: If this is important to you, raise and/or
vote for it on GitHub. Personally, I've worked around this by dropping my laptop display
resolution down to 1900x1200, where I can still run at 100% scale and have no issues with
apps which can't deal with scaling changes well (like Ghoul!)

# Application has bugs
Probably. Try reporting an issue (:

# UI sucks
Yep, probably. It's literally the simplest UI I could come up with that was functional.

# Bonus
I've found that some apps don't like having monitors added -- especially if there are different resolution scalings. So if you save a layout and are prepared to edit the ini file (`%LOCALAPPDATA%/Ghoul/ghoul.ini`), then you can add a section like so:

```
[restore: My Fancy Layout]
C:\path\to\derping\program.exe
C:\path\to\another\derping\program.exe
```

# Installing from source

1. Get the source (git clone somewhere on your local computer)
2. Ensure you have all the build dependencies: run `npm install` in the source folder
    - note that you will need at least the following:
        - NodeJS
        - Python (in your path, required to build the node module which creates shortcuts)
        - MSBuild -- may work with just the build tools, but you could just install VS Community
3. Install with `npm run install`

This should result in a shortcut in your start menu that you can run to use Ghoul. Note that the shortcut does not run elevated, so Ghoul may not be able to manage windows with elevated privileges (such as VS running as admin or snapins like `services.msc`). More on this later.
