# Discussion

## Compiler

* See generated in-memory files running Angular compiler `npx ngc`
* Enabling Ivy in *tsconfig.app.json*: `{ ..., "angularCompilerOptions": { "enableIvy": "ngtsc" } }`
* [Ivy feature status](https://github.com/angular/angular/blob/master/packages/core/src/render3/STATUS.md)

## Pre-Ivy

### `ngfactory`

* *Private* members start with ɵ (Greek Theta)
* Use *Goto Definition* to see full name of these methods

#### Static

```ts
@Component({
  selector: 'app-static',
  template: '<h1>Hello World</h1><ul><li>First</li><li>Second</li></ul>',
  styles: []
})
export class StaticComponent ... { ... }
```

View Creation:

```js
export function View_StaticComponent_0(_l) {
    return i0.ɵvid(0, [
        (_l()(), i0.ɵeld(0, 0, null, null, 1, "h1", [], null, null, null, null, null)),
        (_l()(), i0.ɵted(-1, null, ["Hello World"])),
        (_l()(), i0.ɵeld(2, 0, null, null, 4, "ul", [], null, null, null, null, null)),
        (_l()(), i0.ɵeld(3, 0, null, null, 1, "li", [], null, null, null, null, null)),
        (_l()(), i0.ɵted(-1, null, ["First"])),
        (_l()(), i0.ɵeld(5, 0, null, null, 1, "li", [], null, null, null, null, null)),
        (_l()(), i0.ɵted(-1, null, ["Second"]))
    ], null, null);
}
```

#### Simple Binding

```ts
@Component({
  selector: 'app-simple-binding',
  template: '<h1>{{ greeting }} {{ target }}</h1><ul><li>First</li><li>Second</li></ul>',
  styles: []
})
export class SimpleBindingComponent ... { ... }
```

View Creation and Change Detection:

```js
export function View_SimpleBindingComponent_0(_l) {
    return i0.ɵvid(0, [
        (_l()(), i0.ɵeld(0, 0, null, null, 1, "h1", [], null, null, null, null, null)),
        (_l()(), i0.ɵted(1, null, ["", " ", ""])),
        (_l()(), i0.ɵeld(2, 0, null, null, 4, "ul", [], null, null, null, null, null)),
        (_l()(), i0.ɵeld(3, 0, null, null, 1, "li", [], null, null, null, null, null)),
        (_l()(), i0.ɵted(-1, null, ["First"])),
        (_l()(), i0.ɵeld(5, 0, null, null, 1, "li", [], null, null, null, null, null)),
        (_l()(), i0.ɵted(-1, null, ["Second"]))
    ], null, function (_ck, _v) {
        var _co = _v.component;
        var currVal_0 = _co.greeting;
        var currVal_1 = _co.target;
        _ck(_v, 1, 0, currVal_0, currVal_1);
    });
}
```

## Ivy

* Generate code with `npx ng build --aot`, check *main.js*
* See manually created components (*template-less*) in Ivy sample
