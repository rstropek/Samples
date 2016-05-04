# TypeScript Intro Session from [Techorama 2016](http://www.techorama.be/agenda-2016/)

## Abstract

JavaScript has a big advantage: Reach. You can run JavaScript on the client and the server, on your phone, your PC and even on your smart watch. Unfortunately, JavaScript has disadvantages when it comes to larger project implemented by multiple developers. TypeScript was built to solve this problem by adding a language layer on top of JavaScript. With TypeScript, you can create and add types where you need them. Compiler and IDE use them for compile-time type checking, IntelliSense, refactoring support, etc. In this demo-heavy session, Rainer Stropek (Microsoft Azure MVP, Microsoft Regional Director) introduces you to the TypeScript language. Rainer will start with the basics and work his way up to real-world examples using Node.js on the server and AngularJS on the client. This session requires basic JavaScript knowledge but no existing TypeScript experience. 


## Server Project Setup Storyboard

1. Walkthrough startup code
    * `package.json`
    * `Gulpfile`

1. Add TypeScript compiler: `npm install typescript --save-dev`

1. Create `src/server.ts` and demo some basic TypeScript concepts
    * Compile with `node node_modules\typescript\bin\tsc`
    * Watch with `node node_modules\typescript\bin\tsc -w`

1. Create `tsconfig.json`
    ```
    {
        "compilerOptions": {
            "module": "commonjs",
            "target": "es2015",
            "noImplicitAny": true,
            "sourceMap": true,
            "moduleResolution": "node"
        },
        "exclude": [
            "node_modules",
            "typings/browser.d.ts",
            "typings/browser"
        ]
    }
    ```

1. Add `gulp-typescript`: `npm install gulp-typescript --save-dev`

1. Add TypeScript compile to Gulpfile
    ```
    ...
    // Create TypeScript project from tsconfig.json
    var tsClientProject = ts.createProject("tsconfig.json", {
        typescript: require("typescript")
    });
    ...
    // Compile Typescript files
    gulp.task("app", [], () => {
        gulp.src(config.TS_SOURCES)
            .pipe(sourcemaps.init())
            .pipe(ts(tsClientProject))
            .pipe(sourcemaps.write('.'))
            .pipe(gulp.dest(config.APP_DIST));
        ...
    });
    ...
    ```

1. Talk about the need for type definitions for existing JS libraries

1. Add `typings`: `npm install typings --save-dev`

1. Init typings: `typings init`

1. Install typings for mongodb: `typings install mongodb --save-dev`
    * Speak about [public typings registry](https://github.com/typings/registry)
    * Speak about [Definitly typed](https://github.com/borisyankov/DefinitelyTyped)

1. Install other typings: `typings install body-parser cors express express-serve-static-core jasmine mime node serve-static --save-dev --ambient`

1. Add `postinstall` to `package.json`
    ```
    {
        ...
        "scripts": {
            ...
            "postinstall": "typings install"
        },
        ...
    }
    ```

1. Add basic `src/server.ts` code (demo IntelliSense for *Express*)
    ```
    /// <reference path="../typings/main.d.ts" />
    import * as express from "express";
    import * as cors from "cors";
    import * as bodyParser from "body-parser";

    var app = express();

    // Create express server
    app.use(cors());
    var bodyParserOptions = { };
    app.use(bodyParser.json(bodyParserOptions));

    // Start express server
    var port: number = process.env.port || 1337;
    app.listen(port, () => {
        console.log(`Server is listening on port ${port}`);
    });
    ```


## Server Project Implementation Storyboard

### Reviver

1. Add/implement `src/middleware/reviver.ts`
    * Show RegEx IntelliSense
    * Show import of MongoDB
    * Show MongoDB IntelliSense
    * Talk about default export

1. Add/implement `src/spec/reviver.spec.ts`
    * Show Jasmine IntelliSense

1. Run tests: `npm run test`

1. Add reviver in `src/server.ts`
    ```
    ...
    import reviver from "./middleware/reviver";
    ...
    var bodyParserOptions = { reviver: reviver };
    ...
    ```

1. Setup tasks in Visual Studio Code and show building and testing
    ```
    {
        // See http://go.microsoft.com/fwlink/?LinkId=733558
        // for the documentation about the tasks.json format
        "version": "0.1.0",
        "command": "npm",
        "isShellCommand": true,
        "showOutput": "silent",
        "suppressTaskName": true,
        "tasks": [
            {
                "taskName": "build",
                "args": ["run", "build"],
                "problemMatcher": "$tsc"
            },
            {
                "taskName": "test",
                "args": ["run", "test"],
                "showOutput": "always"
            }
        ]
    }
    ```

### Interfaces

1. Add/implement `src/model.ts`
    * Speak about the role of interfaces in TypeScript

1. Add/implement `src/config.ts`

1. Add/implement `src/dataAccess/contracts.ts`
    * Speak about generics in TypeScript

### Data Access Implementation

1. Add/implement `src/dataAccess/store-base.ts`
    * Speak about Promises and `async/await`

1. Add/implement `src/dataAccess/event-store.ts`
    * Demo advanced MongoDB IntelliSense

1. Add/implement `src/dataAccess/participant-store.ts`
    * Speak about why `any` is still relevant (opt-out from typings)

1. Add/implement `src/spec/config.ts` and `src/spec/dataAccess.spec.ts`
    * Speak about async tests with Jasmine
    
1. Run tests: `npm run test`

### Middleware Implementation

1. Add/implement `src/middleware/add-data-context.ts` and `src/middleware/get-data-context.ts`
    * Speak about typecast to `any`

1. Add/implement `src/middleware/events-api.ts`
    * Demo how we program against interfaces of our data layer
    * Demo IntelliSense for Express

1. Add/implement full code for `src/server.ts`

### Run and test

1. Run server: `npm run start`

1. Demo web api with *Postman*


## Client Project

1. Walkthroug startup code
    * Show that no typings are necessary for Angular (come with NPM)
    * Show `package.json` (focus on `scripts`)
    * Demo how TypeScript detects file changes in watch mode

1. Add/implement `app/app.component.ts`
    * Speak about server/client-side code reuse (`IEvent`)
    * Demo Angular IntelliSense (`http`)

1. Run app: `npm run start`

