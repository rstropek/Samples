/// <reference path="../../typings/tsd.d.ts" />
/// <reference path="mobileServicesTableService.ts" />

module Registration {
	interface IValidationState {
		'has-error': boolean;
	}

	export interface IRegisterControllerScope extends ng.IScope {
		registerForm: ng.IFormController;
	}
	
	export interface IRegistration extends MobileServicesDataAccess.ITableRow {
        firstName: string;
		lastName: string;
		email: string;
    }
	
	export class RegisterController {
		public static $inject = ['$scope', 'reservationTable'];

		constructor($scope: IRegisterControllerScope, private reservationTable: MobileServicesDataAccess.Table<IRegistration>) {
			$scope.$watch("registerForm.$valid",
				() => this.showIncompleteDataError = this.showIncompleteDataError && $scope.registerForm.$invalid);
		}

		public firstName: string;
		public lastName: string;
		public email: string;
		public showIncompleteDataError = false;
		public showSuccess = false;

		public getValidationState(formController: ng.IFormController): IValidationState {
			return {
				'has-error': formController.$invalid
			};
		}

		public saveRegistration() {
			if (!this.firstName || !this.lastName || !this.email) {
				this.showIncompleteDataError = true;
			}
			else {
				this.reservationTable.insert({ firstName: this.firstName, lastName: this.lastName, email: this.email})
					.success(() => this.showSuccess = true);
			}
		}
	}
}