const
    gulp = requireModule("gulp-with-help"),
    //windowsShortcut = require("node-win-shortcut")
    path = require("path"),
    installFolder = path.join(process.env.LOCALAPPDATA, "Ghoul");

gulp.task("install", `Installs the binaries to ${installFolder}`, ["build"], () => {
    throw new Error("FIXME: node-win-shortcut is broken (gyp install), so this task needs some <3");
    var binFolder = path.join("src", "Ghoul", "bin", "Debug");
    gulp.src([path.join(binFolder, "*"), `!${path.join(binFolder, "ghoul.ini")}`])
    .pipe(gulp.dest(installFolder));
    windowsShortcut.createShortcut(
        path.join(installFolder, "Ghoul.exe"), "Ghoul", "Ghoul");
});
