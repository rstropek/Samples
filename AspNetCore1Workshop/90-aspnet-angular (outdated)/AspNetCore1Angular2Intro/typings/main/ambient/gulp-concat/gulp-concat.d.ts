// Compiled using typings@0.6.10
// Source: https://raw.githubusercontent.com/DefinitelyTyped/DefinitelyTyped/750b43c5b25887b6dc14ed3be1ccca5aa74d60b5/gulp-concat/gulp-concat.d.ts
// Type definitions for gulp-concat
// Project: http://github.com/wearefractal/gulp-concat
// Definitions by: Keita Kagurazaka <https://github.com/k-kagurazaka>
// Definitions: https://github.com/borisyankov/DefinitelyTyped


declare module "gulp-concat" {

    interface IOptions {
        newLine: string;
    }

    interface IFsStats {
        dev?: number;
        ino?: number;
        mode?: number;
        nlink?: number;
        uid?: number;
        gid?: number;
        rdev?: number;
        size?: number;
        blksize?: number;
        blocks?: number;
        atime?: Date;
        mtime?: Date;
        ctime?: Date;
    }

    interface IVinylOptions {
        cwd?: string;
        base?: string;
        path?: string;
        stat?: IFsStats;
        contents?: NodeJS.ReadableStream | Buffer;
    }

    interface IConcat {
        (filename: string, options?: IOptions): NodeJS.ReadWriteStream;
        (options: IVinylOptions): NodeJS.ReadWriteStream;
    }

    var _tmp: IConcat;
    export = _tmp;
}