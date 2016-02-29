import { join } from 'path';

// Folder names for client code (sources, distribution)
export const APP_SRC = "src";
export const APP_DIST = "wwwroot";

// TypeScript sources
export const TS_SOURCES: string[] =
    ["typings/browser.d.ts", join(APP_SRC, "**/*.ts")]

// External script dependencies (combined into single file)
export const SCRIPT_DEPENDENCIES: string[] = [
    "node_modules/systemjs/dist/system.src.js",
    "node_modules/angular2/bundles/angular2-polyfills.js",
    "node_modules/rxjs/bundles/Rx.js",
    "node_modules/angular2/bundles/angular2.dev.js",
    "node_modules/angular2/bundles/http.dev.js"
];
export const SCRIPT_COMBINED = "scripts.js";

export const STYLES_DEPENDENCIES: string[] = [
    "node_modules/bootstrap/dist/css/bootstrap.css"
];
export const STYLES_COMBINED = "styles.css";
