var gulp = require("gulp");
var concat = require("gulp-concat");
var del = require("del");
var ts = require("gulp-typescript");
var sourcemaps = require("gulp-sourcemaps");

var appDependencyScripts = ["node_modules/jquery/dist/jquery.min.js", "node_modules/angular/angular.min.js", "node_modules/angular-route/angular-route.min.js", "node_modules/bootstrap/dist/js/bootstrap.min.js"];
var appDependencyStylesheets = ["node_modules/bootstrap/dist/css/bootstrap.min.css"];

var testsDependencyScripts = [
	// Remove the following line if tests should run with Karma as
	// Karma includes Jasmine by default 
	"node_modules/jasmine-core/lib/jasmine-core/jasmine.js", "node_modules/jasmine-core/lib/jasmine-core/jasmine-html.js", "node_modules/jasmine-core/lib/jasmine-core/boot.js",
	"node_modules/jquery/dist/jquery.min.js", 
	"node_modules/angular/angular.min.js", "node_modules/angular-mocks/angular-mocks.js"]
var testsDepencencyStylesheets = ["node_modules/jasmine-core/lib/jasmine-core/jasmine.css"]

var appPath = "app/";
var distPath = "dist/";
var clientPath = "client/";
var scriptsPath = "scripts/";
var stylesPath = "styles/";
var testsPath = "tests/";
var appScriptDependenciesName = "appDependencies.min.js";
var appStylesDependenciesName = "appDependencies.min.css";
var testsScriptDependenciesName = "testsDependencies.min.js";
var testsStylesDependenciesName = "testsDependencies.min.css";
var appScriptName = "app.js";
var testsScriptName = "app.spec.js";

var tsServerProject = ts.createProject({
	module: "commonjs",
	sourceMap: false,
	target: "ES5"
});
var tsClientProject = ts.createProject({
	module: "commonjs",
	noImplicitAny: true,
	sourceMap: true,
	target: "ES5",
	out: appScriptName
});
var tsTestProject = ts.createProject({
	module: "commonjs",
	noImplicitAny: true,
	sourceMap: true,
	target: "ES5",
	out: testsScriptName
});

// Cleanup by deleting target directory
gulp.task("clean", function () {
	del.sync([distPath]);
});

// Build dev host (node.js)
gulp.task("typescriptDevHost", [], function () {
	var tsResult = gulp.src(appPath + "server.ts")
		.pipe(sourcemaps.init())
    	.pipe(ts(tsServerProject));
	return tsResult.js
		.pipe(sourcemaps.write("./"))
  		.pipe(gulp.dest(distPath));
});

// Build dependencies for web client (e.g. jquery, angular, etc.)
gulp.task("appDependencies", [], function () {
	// Scripts
	var scriptsTargetPath = distPath + clientPath + scriptsPath;
	gulp.src(appDependencyScripts)
		.pipe(concat(appScriptDependenciesName))
		.pipe(gulp.dest(scriptsTargetPath));

	// Styles
	var stylesTargetPath = distPath + clientPath + stylesPath;
	gulp.src(appDependencyStylesheets)
		.pipe(concat(appStylesDependenciesName))
		.pipe(gulp.dest(stylesTargetPath));
});

gulp.task("clientApp", [], function() {
	// Copy html files
	gulp.src([appPath + clientPath + "**/*.html"])
		.pipe(gulp.dest(distPath + clientPath));
	
	// Build client-side typescript files
	var tsResult = gulp.src(appPath + clientPath + "**/*.ts")
		.pipe(sourcemaps.init())
    	.pipe(ts(tsClientProject));
	return tsResult.js
		.pipe(sourcemaps.write("./"))
  		.pipe(gulp.dest(distPath + clientPath + scriptsPath));
});

// Build dependencies for tests (e.g. jasmine, etc.)
gulp.task("testDependencies", [], function () {
	// Scripts
	var scriptsTargetPath = distPath + testsPath + clientPath + scriptsPath;
	gulp.src(testsDependencyScripts)
		.pipe(concat(testsScriptDependenciesName))
		.pipe(gulp.dest(scriptsTargetPath));

	// Styles
	var stylesTargetPath = distPath + testsPath + clientPath + stylesPath;
	gulp.src(testsDepencencyStylesheets)
		.pipe(concat(testsStylesDependenciesName))
		.pipe(gulp.dest(stylesTargetPath));
});

gulp.task("tests", [], function() {
	// Copy html files
	gulp.src([appPath + testsPath + clientPath + "*.html"])
		.pipe(gulp.dest(distPath + testsPath + clientPath));
	
	// Build client-side typescript files
	var tsResult = gulp.src([appPath + clientPath + "**/*.ts", appPath + testsPath + clientPath + "**/*.ts"])
		.pipe(sourcemaps.init())
    	.pipe(ts(tsTestProject));
	return tsResult.js
		.pipe(sourcemaps.write("./"))
  		.pipe(gulp.dest(distPath + testsPath + clientPath + scriptsPath));
});

gulp.task("typescript:watch", function () {
	gulp.watch(["app/**/*.ts", "app/**/*.html"], ["typescriptDevHost", "clientApp", "tests"]);
});

//Set a default tasks
gulp.task("default", ["clean", "typescriptDevHost", "appDependencies", "clientApp", "testDependencies", "tests"], function () { });