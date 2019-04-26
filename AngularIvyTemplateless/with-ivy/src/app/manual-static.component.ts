import * as core from '@angular/core';

export class ManualStaticComponent {
  static numberOfItems = 5;

  static ngComponentDef = core.ɵdefineComponent({
    type: ManualStaticComponent,
    selectors: [["app-manual-static"]],
    factory: () => new ManualStaticComponent(),
    consts: ManualStaticComponent.numberOfItems * 2 + 5,
    vars: 0,
    template: (rf: core.ɵRenderFlags, ctx: ManualStaticComponent) => {
      if (rf & core.ɵRenderFlags.Create) {
        let index = 0;
        core.ɵelementStart(index++, "h1");
        core.ɵtext(index++, "Hello World");
        core.ɵelementEnd();
        core.ɵelementStart(index++, "ul");
        for(let i = 0; i < ManualStaticComponent.numberOfItems; i++) {
          core.ɵelementStart(index++, "li");
          core.ɵtext(index++, "Item");
          core.ɵelementEnd();
        }
        core.ɵelementStart(index++, "li");
        core.ɵtext(index++, "Last");
        core.ɵelementEnd();
        core.ɵelementEnd();
      }
    },
    encapsulation: core.ViewEncapsulation.Native
  });
}