# Angular and SVG

## Introduction

This sample has originally been developed for [*Magdeburger Dev Days*](https://md-devdays.de/) 2018. I refreshed and extended the material for [*Techorama Netherlands 2018*](https://techorama.nl). The goal is to show how to use SVG + WebSockets in Angular projects.

## Demo Checklist

### SVG Recap

The goal of this project is *not* to do a general SVG introduction. However, some people might not be that familiar with SVG. Therefore, use [*whats-svg.component.html*](src/app/whats-svg/whats-svg.component.html) to recap the SVG concepts that one needs to understand to be able to follow the Angular-related part of that talk.

* External vs. internal SVG sources - internal sources are important for Angular
* Style SVG with CSS
* How to work with [transforms](https://developer.mozilla.org/en-US/docs/Web/SVG/Attribute/transform)
* Animate SVG with CSS, discuss other options for animations
* [Dashed lines](https://developer.mozilla.org/en-US/docs/Web/SVG/Attribute/stroke-dasharray) in SVG
* [Paths](https://developer.mozilla.org/en-US/docs/Web/SVG/Tutorial/Paths) and arcs

### Binding

Use [*attr-binding.component.html*](src/app/attr-binding/attr-binding.component.html) to discuss binding topics.

* [Attribute binding](https://angular.io/guide/template-syntax#attribute-binding) instead of property binding
* Show that event binding just works
* Demonstrate the combination of CSS styling of SVG, [class binding](https://angular.io/guide/template-syntax#class-binding), and Angular to build a simple SVG-based control with clear view/logic separation

### Building SVG-based Components

The component in [*budget-chart.component.ts*](src/app/attr-selector/budget-chart/budget-chart.component.ts) shows how to build an SVG-based Angular directive.

* Use of attribute selector in component
* `svg:` namespace for identifying SVG elements in Angular templates
* `@Input` decorator for inpupt parameters
* `:host ::ng-deep` in CSS (see [*project-list.component.css*](src/app/attr-selector/project-list/project-list.component.css))
* Use component in parent SVG (see [*project-list.component.html*](src/app/attr-selector/project-list/project-list.component.html)) including data binding in a structural directive

### Animations

The next sample brings everything together. It implements a silo element in a ficticious IoT visualization library. The element [*silo*](src/app/iot/silo) is a reusable component, uses Angular animations and supports context menus.

* Context menu with [*ngx-contextmenu*](https://github.com/isaacplmann/ngx-contextmenu/)
* Use of [Angular Animations](https://angular.io/guide/animations) with SVG
* Using the component in a parent view (see [*animation.component.ts*](src/app/animation/animation.component.ts))

### WebSockets

The last sample demonstrates the integration of SVG+Angular+Websockets. A demo server is regularly sending the fill state of a silo using WebSockets.

* Run [demo WebSockets server](https://github.com/rstropek/Samples/tree/master/AngularSvgServer) using `npx ts-node timer-broadcast.ts`
* See how silo fill in [*silo-with-websocket.component.ts*](src/app/silo-with-websocket.component/silo-with-websocket.component.ts) changes based on data binding and incoming WebSocket messages
* Describe how websocket messages are distributed using RxJS (see [*backend.service.ts*](src/app/iot/backend.service.ts))
