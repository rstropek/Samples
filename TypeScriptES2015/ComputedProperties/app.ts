/// <reference path="typings/tsd.d.ts" />

class ValueController {
	public value: number;

	// Name of CSS classes for error and warning indication is
	// injected by Angular.	
	constructor(private errorCssName: string, 
		private warningCssName: string) {
	}
	
	// Method returning an object setting CSS classes for input
	// validation warnings. Is used in conjunction with 
	// Angular's ngClass directive.
	public getClass() {
		// Note the use of the new calculated property syntax.
		return {
			[this.errorCssName]: this.value >= 20,
			[this.warningCssName]: 
				this.value < 20 && this.value >= 10
		};
	}
}

angular.module("demo", [])
	.constant("errorCssName", "has-error")
	.constant("warningCssName", "has-warning")
	.controller("demo", ValueController);