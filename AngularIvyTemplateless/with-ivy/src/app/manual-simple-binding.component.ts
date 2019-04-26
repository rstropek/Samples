import * as core from '@angular/core';

export class ManualSimpleBindingComponent {
  greeting = "Hello";
  target = "World";

  static ngComponentDef = core.ɵdefineComponent({
    type: ManualSimpleBindingComponent,
    selectors: [["app-manual-simple-binding"]],
    factory: () => new ManualSimpleBindingComponent(),
    consts: 2,
    vars: 2,
    template: (rf: core.ɵRenderFlags, ctx: ManualSimpleBindingComponent) => {
      if (rf & core.ɵRenderFlags.Create) {
        core.ɵelementStart(0, "h1");
        core.ɵtext(1);
        core.ɵelementEnd();
      }
      if (rf & core.ɵRenderFlags.Update) {
        core.ɵtextBinding(1, core.ɵinterpolation2("", ctx.greeting, " ", ctx.target, ""));
      }
    },
    encapsulation: core.ViewEncapsulation.Native
  });
}