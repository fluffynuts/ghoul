const
    gulp = requireModule("gulp-with-help"),
    spawn = requireModule("spawn"),
    which = require("which"),
    fs = require("fs");

gulp.task("release", "Builds the release installer", ["increment-release-version"], () => {
    return promisify(() => {
        var iscc = which.sync("iscc");
        if (!iscc) {
            throw new Error("ISCC.exe must be in your path to build");
        }
        return spawn(iscc, [ "ghoul.iss" ]);
    });
});

gulp.task("increment-release-version", ["test-dotnet"], () => {
    return promisify(() => {
        const lines = fs.readFileSync("ghoul.iss", { encoding: "utf8" })
            .split("\r\n")
            .map(l => {
                if (l.indexOf("#define MyAppVersion") > -1) {
                    const
                        parts = l.split(" "),
                        version = parts[parts.length - 1].replace(/"/g, ""),
                        versionParts = version.split("."),
                        newVersion = `${versionParts[0]}.${parseInt(versionParts[1]) + 1}`;
                    return `#define MyAppVersion "${newVersion}"`;
                }
                return l;
            });
        fs.writeFileSync("ghoul.iss", lines.join("\r\n"), { encoding: "utf8" });
    });
})

function promisify(func) {
    return new Promise((resolve, reject) => {
        try {
            resolve(func());
        } catch (e) {
            reject(e);
        }
    });
}