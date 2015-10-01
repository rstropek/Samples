/// <reference path="../../typings/tsd.d.ts" />
/// <reference path="register.ts" />
/// <reference path="mobileServicesTableService.ts" />

module Registration {
	export class ListController {
		public static $inject = ['registrationTable'];
				
		constructor(private registrationTable: MobileServicesDataAccess.Table<IRegistration>) {
			this.refreshList();
		}
		
		public registrations: IRegistration[];
		
		public refreshList() {
			this.registrationTable.query()
				.success(r => this.registrations = r);
		}
	}
}