import * as core from '@angular/core';

export class ManualChildComponent {
  name = "Bar";

  static ngComponentDef = core.ɵdefineComponent({
    type: ManualChildComponent,
    selectors: [["app-manual-child"]],
    factory: () => new ManualChildComponent(),
    consts: 2,
    vars: 1,
    template: (rf: core.ɵRenderFlags, ctx: ManualChildComponent) => {
      if (rf & core.ɵRenderFlags.Create) {
        core.ɵelementStart(0, "p");
        core.ɵtext(1);
        core.ɵelementEnd();
      } if (rf & core.ɵRenderFlags.Update) {
        core.ɵtextBinding(1, core.ɵinterpolation1("I am the child ", ctx.name, ""));
      }
    },
    encapsulation: core.ViewEncapsulation.Native
  });
}

export class ManualParentComponent {
  childName = "Foo";

  static ngComponentDef = core.ɵdefineComponent({
    type: ManualParentComponent,
    selectors: [["app-manual-parent"]],
    factory: () => new ManualParentComponent(),
    consts: 3,
    vars: 1,
    template: (rf: core.ɵRenderFlags, ctx: ManualParentComponent) => {
      if (rf & core.ɵRenderFlags.Create) {
        core.ɵelementStart(0, "p");
        core.ɵtext(1, "I am the parent, here is my child:");
        core.ɵelementEnd();
        core.ɵelement(2, "app-manual-child", [core.ɵAttributeMarker.SelectOnly, "name"]);
      } if (rf & core.ɵRenderFlags.Update) {
        core.ɵelementProperty(2, "name", core.ɵbind(ctx.childName));
      }
    },
    directives: [ManualChildComponent],
    encapsulation: core.ViewEncapsulation.Native
  });
}