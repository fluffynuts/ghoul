const
    gulp = requireModule("gulp-with-help"),
    windowsShortcut = require("node-win-shortcut")
    path = require("path"),
    installFolder = path.join(process.env.LOCALAPPDATA, "Ghoul");

gulp.task("install", `Installs the binaries to ${installFolder}`, ["build"], () => {
    gulp.src(path.join("src", "Ghoul", "bin", "Debug", "*"))
    .pipe(gulp.dest(installFolder));
    windowsShortcut.createShortcut(
        path.join(installFolder, "Ghoul.exe"), "Ghoul", "Ghoul");
});