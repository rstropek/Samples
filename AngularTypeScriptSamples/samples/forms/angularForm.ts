class Person {
	public firstName: string = "Max";
	public lastName: string = "Muster";
	public shortBio: string = "I am a funny guy!";
	public email: string = "test@trash-mail.com";
	public firstJob: boolean = false;
	public driverLicense: string = "None";
	public sex: string = "male";
	public languages: Language[] = [
		{ language: "en", knowledge: "fluent" },
		{ language: "de", knowledge: "native" }
	];
}

class Language {
	public language: string;
	public knowledge: string;
}

class Application {
	public person: Person;

	constructor() {
		this.person = new Person();
	}
}

interface ISexOptions {
	code: string;
	description: string;
}

interface IBootstrapValidationClasses {
	"has-success": boolean;
	"has-error": boolean;
}

interface IApplicationScope extends ng.IScope {
	application: Application;
	sexOptions: ISexOptions[];
	getCssClasses: (modelController: ng.INgModelController) => IBootstrapValidationClasses;
	canSave: (modelController: ng.INgModelController) => boolean;
	removeLanguage: (index: number) => void;
	applicationForm: ng.INgModelController;
}

class ApplicationController {
	constructor($scope: IApplicationScope) {
		$scope.application = new Application();
		$scope.sexOptions = [
			{ code: "male", description: "Male" },
			{ code: "female", description: "Female" }
		];
		$scope.getCssClasses = (ngModelController) => {
			return {
				"has-success": ngModelController.$dirty && ngModelController.$valid,
				"has-error": ngModelController.$dirty && ngModelController.$invalid,
			};
		};
		$scope.canSave = (ngModelController) => ngModelController.$dirty && ngModelController.$valid;
		$scope.removeLanguage = (index) => {
			$scope.application.person.languages.splice(index, 1);
			$scope.applicationForm.$dirty = true;
		};
	}
}

angular.module("applicationApp", [])
	.controller("applicationForm", ['$scope', ApplicationController]);