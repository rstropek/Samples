// Compiled using typings@0.6.10
// Source: https://raw.githubusercontent.com/typed-typings/npm-es6-promise/fb04188767acfec1defd054fc8024fafa5cd4de7/dist/es6-promise.d.ts
declare module 'gulp~orchestrator~es6-promise/dist/es6-promise' {
export interface Thenable <R> {
  then <U> (onFulfilled?: (value: R) => U | Thenable<U>, onRejected?: (error: any) => U | Thenable<U>): Thenable<U>;
  then <U> (onFulfilled?: (value: R) => U | Thenable<U>, onRejected?: (error: any) => void): Thenable<U>;
}

export class Promise <R> implements Thenable <R> {
  /**
   * If you call resolve in the body of the callback passed to the constructor,
   * your promise is fulfilled with result object passed to resolve.
   * If you call reject your promise is rejected with the object passed to resolve.
   * For consistency and debugging (eg stack traces), obj should be an instanceof Error.
   * Any errors thrown in the constructor callback will be implicitly passed to reject().
   */
  constructor (callback: (resolve : (value?: R | Thenable<R>) => void, reject: (error?: any) => void) => void);

  /**
   * onFulfilled is called when/if "promise" resolves. onRejected is called when/if "promise" rejects.
   * Both are optional, if either/both are omitted the next onFulfilled/onRejected in the chain is called.
   * Both callbacks have a single parameter , the fulfillment value or rejection reason.
   * "then" returns a new promise equivalent to the value you return from onFulfilled/onRejected after being passed through Promise.resolve.
   * If an error is thrown in the callback, the returned promise rejects with that error.
   *
   * @param onFulfilled called when/if "promise" resolves
   * @param onRejected called when/if "promise" rejects
   */
  then <U> (onFulfilled?: (value: R) => U | Thenable<U>, onRejected?: (error: any) => U | Thenable<U>): Promise<U>;
  then <U> (onFulfilled?: (value: R) => U | Thenable<U>, onRejected?: (error: any) => void): Promise<U>;

  /**
   * Sugar for promise.then(undefined, onRejected)
   *
   * @param onRejected called when/if "promise" rejects
   */
  catch <U> (onRejected?: (error: any) => U | Thenable<U>): Promise<U>;

  /**
   * Make a new promise from the thenable.
   * A thenable is promise-like in as far as it has a "then" method.
   */
  static resolve (): Promise<void>;
  static resolve <R> (value: R | Thenable<R>): Promise<R>;

  /**
   * Make a promise that rejects to obj. For consistency and debugging (eg stack traces), obj should be an instanceof Error
   */
  static reject <R> (error: any): Promise<R>;

  /**
   * Make a promise that fulfills when every item in the array fulfills, and rejects if (and when) any item rejects.
   * the array passed to all can be a mixture of promise-like objects and other objects.
   * The fulfillment value is an array (in order) of fulfillment values. The rejection value is the first rejection value.
   */
  static all<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(values: [T1 | Thenable<T1>, T2 | Thenable<T2>, T3 | Thenable<T3>, T4 | Thenable <T4>, T5 | Thenable<T5>, T6 | Thenable<T6>, T7 | Thenable<T7>, T8 | Thenable<T8>, T9 | Thenable<T9>, T10 | Thenable<T10>]): Promise<[T1, T2, T3, T4, T5, T6, T7, T8, T9, T10]>;
  static all<T1, T2, T3, T4, T5, T6, T7, T8, T9>(values: [T1 | Thenable<T1>, T2 | Thenable<T2>, T3 | Thenable<T3>, T4 | Thenable <T4>, T5 | Thenable<T5>, T6 | Thenable<T6>, T7 | Thenable<T7>, T8 | Thenable<T8>, T9 | Thenable<T9>]): Promise<[T1, T2, T3, T4, T5, T6, T7, T8, T9]>;
  static all<T1, T2, T3, T4, T5, T6, T7, T8>(values: [T1 | Thenable<T1>, T2 | Thenable<T2>, T3 | Thenable<T3>, T4 | Thenable <T4>, T5 | Thenable<T5>, T6 | Thenable<T6>, T7 | Thenable<T7>, T8 | Thenable<T8>]): Promise<[T1, T2, T3, T4, T5, T6, T7, T8]>;
  static all<T1, T2, T3, T4, T5, T6, T7>(values: [T1 | Thenable<T1>, T2 | Thenable<T2>, T3 | Thenable<T3>, T4 | Thenable <T4>, T5 | Thenable<T5>, T6 | Thenable<T6>, T7 | Thenable<T7>]): Promise<[T1, T2, T3, T4, T5, T6, T7]>;
  static all<T1, T2, T3, T4, T5, T6>(values: [T1 | Thenable<T1>, T2 | Thenable<T2>, T3 | Thenable<T3>, T4 | Thenable <T4>, T5 | Thenable<T5>, T6 | Thenable<T6>]): Promise<[T1, T2, T3, T4, T5, T6]>;
  static all<T1, T2, T3, T4, T5>(values: [T1 | Thenable<T1>, T2 | Thenable<T2>, T3 | Thenable<T3>, T4 | Thenable <T4>, T5 | Thenable<T5>]): Promise<[T1, T2, T3, T4, T5]>;
  static all<T1, T2, T3, T4>(values: [T1 | Thenable<T1>, T2 | Thenable<T2>, T3 | Thenable<T3>, T4 | Thenable <T4>]): Promise<[T1, T2, T3, T4]>;
  static all<T1, T2, T3>(values: [T1 | Thenable<T1>, T2 | Thenable<T2>, T3 | Thenable<T3>]): Promise<[T1, T2, T3]>;
  static all<T1, T2>(values: [T1 | Thenable<T1>, T2 | Thenable<T2>]): Promise<[T1, T2]>;
  static all<T1>(values: [T1 | Thenable<T1>]): Promise<[T1]>;
  static all<TAll>(values: Array<TAll | Thenable<TAll>>): Promise<TAll[]>;

  /**
   * Make a Promise that fulfills when any item fulfills, and rejects if any item rejects.
   */
  static race <R> (promises: (R | Thenable<R>)[]): Promise<R>;
}

/**
 * The polyfill method will patch the global environment (in this case to the Promise name) when called.
 */
export function polyfill (): void;
}
declare module 'gulp~orchestrator~es6-promise' {
export * from 'gulp~orchestrator~es6-promise/dist/es6-promise';
}

// Compiled using typings@0.6.10
// Source: https://raw.githubusercontent.com/typed-typings/npm-orchestrator/bd916c96d6d8e946bc73772fc3854accd5860aa4/orchestrator.d.ts
declare module 'gulp~orchestrator/orchestrator' {
// Type definitions for Orchestrator
// Project: https://github.com/orchestrator/orchestrator
// Definitions by: Qubo <https://github.com/tkQubo>
// Definitions: https://github.com/borisyankov/DefinitelyTyped

import { Promise } from 'gulp~orchestrator~es6-promise';

type Strings = string | string[];

class Orchestrator {
  add: Orchestrator.AddMethod;
  /**
   * Have you defined a task with this name?
   * @param name The task name to query
   */
  hasTask(name: string): boolean;
  start: Orchestrator.StartMethod;
  stop(): void;

  /**
   * Listen to orchestrator internals
   * @param event Event name to listen to:
   * <ul>
   *   <li>start: from start() method, shows you the task sequence
   *   <li>stop: from stop() method, the queue finished successfully
   *   <li>err: from stop() method, the queue was aborted due to a task error
   *   <li>task_start: from _runTask() method, task was started
   *   <li>task_stop: from _runTask() method, task completed successfully
   *   <li>task_err: from _runTask() method, task errored
   *   <li>task_not_found: from start() method, you're trying to start a task that doesn't exist
   *   <li>task_recursion: from start() method, there are recursive dependencies in your task list
   * </ul>
   * @param cb Passes single argument: e: event details
   */
  on(event: string, cb: (e: Orchestrator.OnCallbackEvent) => any): Orchestrator;

  /**
   * Listen to all orchestrator events from one callback
   * @param cb Passes single argument: e: event details
   */
  onAll(cb: (e: Orchestrator.OnAllCallbackEvent) => any): void;
}

namespace Orchestrator {
  interface AddMethodCallback {
    /**
     * Accept a callback
     * @param callback
     */
    (callback?: Function): any;
    /**
     * Return a promise
     */
    (): Promise<any>;
    /**
     * Return a stream: (task is marked complete when stream ends)
     */
    (): any; //TODO: stream type should be here e.g. map-stream
  }

  /**
   * Define a task
   */
  interface AddMethod {
    /**
     * Define a task
     * @param name The name of the task.
     * @param deps An array of task names to be executed and completed before your task will run.
     * @param fn The function that performs the task's operations. For asynchronous tasks, you need to provide a hint when the task is complete:
     * <ul>
     *     <li>Take in a callback</li>
     *     <li>Return a stream or a promise</li>
     * </ul>
     */
    (name: string, deps?: string[], fn?: AddMethodCallback | Function): Orchestrator;
    /**
     * Define a task
     * @param name The name of the task.
     * @param fn The function that performs the task's operations. For asynchronous tasks, you need to provide a hint when the task is complete:
     * <ul>
     *     <li>Take in a callback</li>
     *     <li>Return a stream or a promise</li>
     * </ul>
     */
    (name: string, fn?: AddMethodCallback | Function): Orchestrator;
  }

  /**
   * Start running the tasks
   */
  interface StartMethod {
    /**
     * Start running the tasks
     * @param tasks Tasks to be executed. You may pass any number of tasks as individual arguments.
     * @param cb Callback to call after run completed.
     */
    (tasks: Strings, cb?: (error?: any) => any): Orchestrator;
    /**
     * Start running the tasks
     * @param tasks Tasks to be executed. You may pass any number of tasks as individual arguments.
     * @param cb Callback to call after run completed.
     */
    (...tasks: Strings[]/*, cb?: (error: any) => any */): Orchestrator;
    //TODO: TypeScript 1.5.3 cannot express varargs followed by callback as a last argument...
    (task1: Strings, task2: Strings, cb?: (error?: any) => any): Orchestrator;
    (task1: Strings, task2: Strings, task3: Strings, cb?: (error?: any) => any): Orchestrator;
    (task1: Strings, task2: Strings, task3: Strings, task4: Strings, cb?: (error?: any) => any): Orchestrator;
    (task1: Strings, task2: Strings, task3: Strings, task4: Strings, task5: Strings, cb?: (error?: any) => any): Orchestrator;
    (task1: Strings, task2: Strings, task3: Strings, task4: Strings, task5: Strings, task6: Strings, cb?: (error?: any) => any): Orchestrator;
  }

  interface OnCallbackEvent {
    message: string;
    task: string;
    err: any;
    duration?: number;
  }

  interface OnAllCallbackEvent extends OnCallbackEvent {
    src: string;
  }

}

export = Orchestrator;
}
declare module 'gulp~orchestrator' {
import main = require('gulp~orchestrator/orchestrator');
export = main;
}

// Compiled using typings@0.6.10
// Source: https://raw.githubusercontent.com/typed-typings/npm-gulp/504da08753acdfe9dd2c415ab8fe36151e9645c1/gulp.d.ts
declare module 'gulp/gulp' {
// Type definitions for Gulp v3.8.x
// Project: http://gulpjs.com
// Original Definitions by: Drew Noakes <https://drewnoakes.com>

import Orchestrator = require('gulp~orchestrator');
import { EventEmitter } from 'events';
import { Duplex } from 'stream';

module gulp {
  interface Gulp extends Orchestrator {
    /**
     * Define a task
     * @param name The name of the task.
     * @param deps An array of task names to be executed and completed before your task will run.
     * @param fn The function that performs the task's operations. For asynchronous tasks, you need to provide a hint when the task is complete:
     * <ul>
     *     <li>Take in a callback</li>
     *     <li>Return a stream or a promise</li>
     * </ul>
     */
    task: Orchestrator.AddMethod;
    /**
     * Emits files matching provided glob or an array of globs. Returns a stream of Vinyl files that can be piped to plugins.
     * @param glob Glob or array of globs to read.
     * @param opt Options to pass to node-glob through glob-stream.
     */
    src: SrcMethod;
    /**
     * Can be piped to and it will write files. Re-emits all data passed to it so you can pipe to multiple folders.
     * Folders that don't exist will be created.
     *
     * @param outFolder The path (output folder) to write files to. Or a function that returns it, the function will be provided a vinyl File instance.
     * @param opt
     */
    dest: DestMethod;
    /**
     * Watch files and do something when a file changes. This always returns an EventEmitter that emits change events.
     *
     * @param glob a single glob or array of globs that indicate which files to watch for changes.
     * @param opt options, that are passed to the gaze library.
     * @param fn a callback or array of callbacks to be called on each change, or names of task(s) to run when a file changes, added with task().
     */
    watch: WatchMethod;
  }

  interface GulpPlugin {
    (...args: any[]): Duplex;
  }

  interface WatchMethod {
    /**
     * Watch files and do something when a file changes. This always returns an EventEmitter that emits change events.
     *
     * @param glob a single glob or array of globs that indicate which files to watch for changes.
     * @param fn a callback or array of callbacks to be called on each change, or names of task(s) to run when a file changes, added with task().
     */
    (glob: string | string[], fn: (WatchCallback | string)): EventEmitter;
    /**
     * Watch files and do something when a file changes. This always returns an EventEmitter that emits change events.
     *
     * @param glob a single glob or array of globs that indicate which files to watch for changes.
     * @param fn a callback or array of callbacks to be called on each change, or names of task(s) to run when a file changes, added with task().
     */
    (glob: string | string[], fn: (WatchCallback | string)[]): EventEmitter;
    /**
     * Watch files and do something when a file changes. This always returns an EventEmitter that emits change events.
     *
     * @param glob a single glob or array of globs that indicate which files to watch for changes.
     * @param opt options, that are passed to the gaze library.
     * @param fn a callback or array of callbacks to be called on each change, or names of task(s) to run when a file changes, added with task().
     */
    (glob: string | string[], opt: WatchOptions, fn: (WatchCallback | string)): EventEmitter;
    /**
     * Watch files and do something when a file changes. This always returns an EventEmitter that emits change events.
     *
     * @param glob a single glob or array of globs that indicate which files to watch for changes.
     * @param opt options, that are passed to the gaze library.
     * @param fn a callback or array of callbacks to be called on each change, or names of task(s) to run when a file changes, added with task().
     */
    (glob: string | string[], opt: WatchOptions, fn: (WatchCallback | string)[]): EventEmitter;

  }

  interface DestMethod {
    /**
     * Can be piped to and it will write files. Re-emits all data passed to it so you can pipe to multiple folders.
     * Folders that don't exist will be created.
     *
     * @param outFolder The path (output folder) to write files to. Or a function that returns it, the function will be provided a vinyl File instance.
     * @param opt
     */
    (outFolder: string | ((file: string) => string), opt?: DestOptions): Duplex;
  }

  interface SrcMethod {
    /**
     * Emits files matching provided glob or an array of globs. Returns a stream of Vinyl files that can be piped to plugins.
     * @param glob Glob or array of globs to read.
     * @param opt Options to pass to node-glob through glob-stream.
     */
    (glob: string | string[], opt?: SrcOptions): Duplex;
  }

  /**
   * Options to pass to node-glob through glob-stream.
   * Specifies two options in addition to those used by node-glob:
   * https://github.com/isaacs/node-glob#options
   */
  interface SrcOptions {
    /**
     * Setting this to <code>false</code> will return <code>file.contents</code> as <code>null</code>
     * and not read the file at all.
     * Default: <code>true</code>.
     */
    read?: boolean;

    /**
     * Setting this to false will return <code>file.contents</code> as a stream and not buffer files.
     * This is useful when working with large files.
     * Note: Plugins might not implement support for streams.
     * Default: <code>true</code>.
     */
    buffer?: boolean;

    /**
     * The base path of a glob.
     *
     * Default is everything before a glob starts.
     */
    base?: string;

    /**
     * The current working directory in which to search.
     * Defaults to process.cwd().
     */
    cwd?: string;

    /**
     * The place where patterns starting with / will be mounted onto.
     * Defaults to path.resolve(options.cwd, "/") (/ on Unix systems, and C:\ or some such on Windows.)
     */
    root?: string;

    /**
     * Include .dot files in normal matches and globstar matches.
     * Note that an explicit dot in a portion of the pattern will always match dot files.
     */
    dot?: boolean;

    /**
     * By default, a pattern starting with a forward-slash will be "mounted" onto the root setting, so that a valid
     * filesystem path is returned. Set this flag to disable that behavior.
     */
    nomount?: boolean;

    /**
     * Add a / character to directory matches. Note that this requires additional stat calls.
     */
    mark?: boolean;

    /**
     * Don't sort the results.
     */
    nosort?: boolean;

    /**
     * Set to true to stat all results. This reduces performance somewhat, and is completely unnecessary, unless
     * readdir is presumed to be an untrustworthy indicator of file existence. It will cause ELOOP to be triggered one
     * level sooner in the case of cyclical symbolic links.
     */
    stat?: boolean;

    /**
     * When an unusual error is encountered when attempting to read a directory, a warning will be printed to stderr.
     * Set the silent option to true to suppress these warnings.
     */
    silent?: boolean;

    /**
     * When an unusual error is encountered when attempting to read a directory, the process will just continue on in
     * search of other matches. Set the strict option to raise an error in these cases.
     */
    strict?: boolean;

    /**
     * See cache property above. Pass in a previously generated cache object to save some fs calls.
     */
    cache?: boolean;

    /**
     * A cache of results of filesystem information, to prevent unnecessary stat calls.
     * While it should not normally be necessary to set this, you may pass the statCache from one glob() call to the
     * options object of another, if you know that the filesystem will not change between calls.
     */
    statCache?: boolean;

    /**
     * Perform a synchronous glob search.
     */
    sync?: boolean;

    /**
     * In some cases, brace-expanded patterns can result in the same file showing up multiple times in the result set.
     * By default, this implementation prevents duplicates in the result set. Set this flag to disable that behavior.
     */
    nounique?: boolean;

    /**
     * Set to never return an empty set, instead returning a set containing the pattern itself.
     * This is the default in glob(3).
     */
    nonull?: boolean;

    /**
     * Perform a case-insensitive match. Note that case-insensitive filesystems will sometimes result in glob returning
     * results that are case-insensitively matched anyway, since readdir and stat will not raise an error.
     */
    nocase?: boolean;

    /**
     * Set to enable debug logging in minimatch and glob.
     */
    debug?: boolean;

    /**
     * Set to enable debug logging in glob, but not minimatch.
     */
    globDebug?: boolean;
  }

  interface DestOptions {
    /**
     * The output folder. Only has an effect if provided output folder is relative.
     * Default: process.cwd()
     */
    cwd?: string;

    /**
     * Octal permission string specifying mode for any folders that need to be created for output folder.
     * Default: 0777.
     */
    mode?: string;
  }

  /**
   * Options that are passed to <code>gaze</code>.
   * https://github.com/shama/gaze
   */
  interface WatchOptions {
    /** Interval to pass to fs.watchFile. */
    interval?: number;
    /** Delay for events called in succession for the same file/event. */
    debounceDelay?: number;
    /** Force the watch mode. Either 'auto' (default), 'watch' (force native events), or 'poll' (force stat polling). */
    mode?: string;
    /** The current working directory to base file patterns from. Default is process.cwd().. */
    cwd?: string;
  }

  interface WatchEvent {
    /** The type of change that occurred, either added, changed or deleted. */
    type: string;
    /** The path to the file that triggered the event. */
    path: string;
  }

  /**
   * Callback to be called on each watched file change.
   */
  interface WatchCallback {
    (event: WatchEvent): void;
  }

  interface TaskCallback {
    /**
     * Defines a task.
     * Tasks may be made asynchronous if they are passing a callback or return a promise or a stream.
     * @param cb callback used to signal asynchronous completion. Caller includes <code>err</code> in case of error.
     */
    (cb?: (err?: any) => void): any;
  }
}

var gulp: gulp.Gulp;

export = gulp;
}
declare module 'gulp' {
import main = require('gulp/gulp');
export = main;
}