# What's New in RxJS 6

## Pipes

* Change introduced in RxJS 5.5
* Recommended syntax
* Patching vs. imported functions:
  * Patched `Observable` in 5.4; note side effects
  * Set breakpoint in new version ([RxJS 5.5](rxjs-5-5/src/app.ts)) and show cleaner `Observable`
  * Note: Renamings were necessary (e.g. `do` became `tap`)

### Demo

* View old syntax in [RxJS 5.4](rxjs-5-4/src/app.ts)
* View new syntax in [RxJS 5.5](rxjs-5-5/src/app.ts)
* Patching vs. imported functions:
  * Set breakpoint in old version ([RxJS 5.4](rxjs-5-4/src/app.ts)) and show patched `Observable`; note side effects
  * Set breakpoint in new version ([RxJS 5.5](rxjs-5-5/src/app.ts)) and show cleaner `Observable`
* Walkthrough [pipeable operators](https://github.com/ReactiveX/rxjs/blob/master/doc/pipeable-operators.md) ([RxJS 5.5](rxjs-5-5/src/app.ts))

## Imports

* Much clearer syntax in RxJS 6
* Easier to learn, easier to remember

### Demo

* View old syntax in [RxJS 5.4](rxjs-5-4/src/app.ts)
* View new syntax in [RxJS 5.5](rxjs-5-5/src/app.ts)

## *rxjs-compat*

* Installing [*rxjs-compat*](https://www.npmjs.com/package/rxjs-compat) makes RxJS 6 compatible to 5.4
* Intermediate step for large projects

### Demo in [RxJS 5.4](rxjs-5-4/src/app.ts)

```bash
npm uninstall rxjs
npm install rxjs
npm run build
npm install rxjs-compat
npm run build
npm start
```

## Migrate from RxJS 5 to 6 with [*rxjs-tslint*](https://github.com/ReactiveX/rxjs-tslint)

*tslint* rule rewriting your code for RxJS 6

### Demo in [RxJS 5.4](rxjs-5-4/src/app.ts)

Note: Open [*app.ts*](rxjs-5-4/src/app.ts) while running migration

```bash
rxjs-5-to-6-migrate -p tsconfig.json
npm uninstall rxjs
npm install rxjs
npm run build
npm start
```

## Synchronous Error Handling

`try/catch` can no longer catch synchronously thrown errors. You need to specify e.g. an error callback.

### Demo

* Show thrown error in [RxJS 5.5](rxjs-5-5/src/app.ts)
* Show thrown error in [RxJS 6](rxjs-6/src/app.ts)

## Further Readings

* Video: [*Introducing RxJS6!* by Ben Lesh](https://youtu.be/JCXZhe6KsxQ)
* [RxJS Change Log](https://github.com/ReactiveX/rxjs/blob/master/CHANGELOG.md)
* Blog: [*RxJS 6: What's new and what has changed?* by Auth0](https://auth0.com/blog/whats-new-in-rxjs-6/)
* Docs: [RxJS v5.x to v6 Update Guide](https://rxjs-dev.firebaseapp.com/guide/v6/migration)
