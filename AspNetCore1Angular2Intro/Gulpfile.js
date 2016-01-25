/// <binding ProjectOpened='typescript:watch' />
var gulp = require("gulp");
var del = require("del");
var ts = require("gulp-typescript");
var sourcemaps = require("gulp-sourcemaps");

var tsClientProject = ts.createProject({
	module: "commonjs",
	noImplicitAny: true,
	sourceMap: true,
	target: "ES5",
	experimentalDecorators: true
});

// Cleanup by deleting target directory
gulp.task("clean", function () {
    del.sync(["wwwroot/**/*.js", "wwwroot/**/*.js.map"]);
});

// Compile the typescript files of the client app
gulp.task("clientApp", [], function() {
	var tsResult = gulp.src("wwwroot/**/*.ts")
		.pipe(sourcemaps.init())
    	.pipe(ts(tsClientProject))
		.pipe(sourcemaps.write("./"))
  		.pipe(gulp.dest("wwwroot"));
});

// Define a watch task
gulp.task("typescript:watch", function () {
	gulp.watch(["wwwroot/**/*.ts"]);
});

//Set a default tasks
gulp.task("default", ["clean", "clientApp"], function () { });