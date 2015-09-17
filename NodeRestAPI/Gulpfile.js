var gulp = require("gulp");
var del = require("del");
var ts = require("gulp-typescript");
var sourcemaps = require("gulp-sourcemaps");
var changed = require("gulp-changed");
var newer = require('gulp-newer');
   
gulp.task("clean", function () {
	del.sync(["*.js", "!Gulpfile.js", "*.js.map"]);
});

var tsProject = ts.createProject('tsconfig.json');
gulp.task("typescript", [], function () {
	var tsResult = tsProject.src() 
        .pipe(ts(tsProject));
    
    return tsResult.js.pipe(gulp.dest('./'));
});
   
gulp.task("typescript:watch", function () {
	gulp.watch(["*.ts"], ["typescript"]);
});
   
gulp.task("default", ["clean", "typescript"], function () { });