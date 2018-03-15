# ghoul
Attempts to resurrect window layouts and restart apps which derp on screen layout changes

# Where's the installer?
There isn't one -- yet. This is very, very alpha.

# Ok, so how *do* I install this?

1. Get the source (git clone somewhere on your local computer)
2. Ensure you have all the build dependencies: run `npm install` in the source folder
    - note that you will need at least the following:
        - NodeJS
        - Python (in your path, required to build the node module which creates shortcuts)
        - MSBuild -- may work with just the build tools, but you could just install VS Community
3. Install with `npm run install`

This should result in a shortcut in your start menu that you can run to use Ghoul. Note that the shortcut does not run elevated, so Ghoul may not be able to manage windows with elevated privileges (such as VS running as admin or snapins like `services.msc`). More on this later.

# What a mission!
Yeah, an installer will come at some point, if there's enough interest or I have nothing better to do (:
For now, things are changing fast enough that the `npm run install` script is more useful (for me at least) and the technical hurdles of getting going mean that people using this (for now) will at least be technical enough to give me useful feedback.

# UI sucks
Yep, probably. It's literally the simplest UI I could come up with that was functional.

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

# Application has bugs
Probably. Try reporting an issue (:

# Application doesn't manage windows with elevated privileges
Try using WinAero Tweaker to create an elevated shortcut. If you want this to run at startup, find the `shell:startup` folder (Start -> Run -> "shell:startup") and ensure you have a shortcut there.

# Bonus
I've found that some apps don't like having monitors added -- especially if there are different resolution scalings. So if you save a layout and are prepared to edit the ini file (`%LOCALAPPDATA%/Ghoul/ghoul.ini`), then you can add a section like so:

```
[restore: My Fancy Layout]
C:\path\to\derping\program.exe
C:\path\to\another\derping\program.exe
```