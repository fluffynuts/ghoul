const
    gulp = requireModule("gulp-with-help"),
    windowsShortcut = require("node-win-shortcut")
    path = require("path"),
    installFolder = path.join(process.env.LOCALAPPDATA, "Ghoul");

gulp.task("install", `Installs the binaries to ${installFolder}`, ["build"], () => {
    var binFolder = path.join("src", "Ghoul", "bin", "Debug");
    gulp.src([path.join(binFolder, "*"), `!${path.join(binFolder, "ghoul.ini")}`])
    .pipe(gulp.dest(installFolder));
    windowsShortcut.createShortcut(
        path.join(installFolder, "Ghoul.exe"), "Ghoul", "Ghoul");
});