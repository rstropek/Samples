import * as core from '@angular/core';

export class ManualEventComponent {
  public say(message: string) {
    console.log(message);
  }

  static ngComponentDef = core.ɵdefineComponent({
    type: ManualEventComponent,
    selectors: [["app-manual-event"]],
    factory: () => new ManualEventComponent(),
    consts: 2,
    vars: 0,
    template: (rf: core.ɵRenderFlags, ctx: ManualEventComponent) => {
      if (rf & core.ɵRenderFlags.Create) {
        core.ɵelementStart(0, "button", [core.ɵAttributeMarker.SelectOnly, "click"]);
        core.ɵlistener("click", () => ctx.say("Hi"));
        core.ɵtext(1, "Say Hi");
        core.ɵelementEnd();
      }
    },
    encapsulation: core.ViewEncapsulation.Native
  });
}