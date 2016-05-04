var gulp = require('gulp');
var del = require('del');
var config = require('./build/config');
var sourcemaps = require('gulp-sourcemaps');

// Cleanup by deleting target directory
gulp.task("clean", () => {
    del.sync(config.CLEAN);
});

// Compile Typescript files
gulp.task("app", [], () => {
	// Add code to compile TypeScript here

    gulp.src(["package.json"])
        .pipe(gulp.dest(config.APP_DIST));
});

gulp.task("default", ["clean", "app"], () => { });
