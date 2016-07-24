/// <binding ProjectOpened='typescript:watch' />

import { join } from 'path';
import * as gulp from 'gulp';
import * as del from 'del';
import * as ts from 'gulp-typescript';
import * as sourcemaps from 'gulp-sourcemaps';
import * as concat from 'gulp-concat';
import * as config from './config';

// Create TypeScript project from tsconfig.json
var tsClientProject = ts.createProject("tsconfig.json", {
    typescript: require("typescript")
});

// Cleanup by deleting target directory
gulp.task("clean", () => {
    del.sync(join(config.APP_DIST, "**/*"));
});

// Compile the typescript files of the client app
gulp.task("clientApp", [], () => {
    var tsResult = gulp.src(config.TS_SOURCES)
    	.pipe(ts(tsClientProject))
  		.pipe(gulp.dest(config.APP_DIST));
});

gulp.task("index", [], () => {
    // Combine external scripts into single file
    gulp.src(config.SCRIPT_DEPENDENCIES)
        .pipe(sourcemaps.init())
        .pipe(concat(config.SCRIPT_COMBINED))
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(config.APP_DIST));

    // Combine external styles into single file
    gulp.src(config.STYLES_DEPENDENCIES)
        .pipe(sourcemaps.init())
        .pipe(concat(config.STYLES_COMBINED))
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest(config.APP_DIST));

    // Copy all HTML files
    gulp.src(join(config.APP_SRC, "**/*.html"))
        .pipe(gulp.dest(config.APP_DIST));
});

//Set a default tasks
gulp.task("default", ["clean", "clientApp", "index"], () => { });

// Define a watch task
gulp.task("watch", () => {
    gulp.watch(join(config.APP_SRC, "**/*"), ["default"]);
});