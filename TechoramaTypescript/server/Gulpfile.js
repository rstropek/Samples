var gulp = require('gulp');
var del = require('del');
var ts = require('gulp-typescript');
var config = require('./build/config');
var sourcemaps = require('gulp-sourcemaps');

// Create TypeScript project from tsconfig.json
var tsClientProject = ts.createProject("tsconfig.json", {
    typescript: require("typescript")
});

// Cleanup by deleting target directory
gulp.task("clean", () => {
    del.sync(config.CLEAN);
});

// Compile Typescript files
gulp.task("app", [], () => {
    gulp.src(config.TS_SOURCES)
        .pipe(sourcemaps.init())
        .pipe(ts(tsClientProject))
        .pipe(sourcemaps.write('.'))
        .pipe(gulp.dest(config.APP_DIST));
        
    gulp.src(["package.json"])
        .pipe(gulp.dest(config.APP_DIST));
});

gulp.task("default", ["clean", "app"], () => { });
