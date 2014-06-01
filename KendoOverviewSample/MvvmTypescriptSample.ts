/// <reference path="scripts/typings/kendouicore/kendo.web.d.ts" />
/// <reference path="scripts/typings/jquery/jquery.d.ts" />

class MvvmSampleViewModel extends kendo.data.ObservableObject {
	public availableSalutations = ["Mr.", "Ms."];
	public salutation = "Mr.";
	public name = "Max Muster";
	public birthday = new Date(1970, 4, 1);
	public age: () => number;
	public isValid: () => boolean;
	public register: () => void;

	constructor() {
		super();

		this.age = () => {
			// Note that it is extremely important to use the "get" method here. If
			// you access "viewModel.birthday" without "get", Kendo would not recognize
			// that "age" depends on the "birthday" field.
			var ageDifMs = Date.now() - this.get("birthday").getTime();
			var ageDate = new Date(ageDifMs);
			return Math.abs(ageDate.getUTCFullYear() - 1970);
		};

		this.isValid = () => {
			return (<() => number>this.get("age"))() >= 18;
		};

		this.register = () => {
			if (<() => boolean>this.get("isValid")) {
				alert("Registering ...");
			}
		};
	}
}


// Setup Kendo controls
$("#salutation").kendoComboBox();
$("#birthday").kendoDatePicker();
$("#register").kendoButton();

// Bind form to the view model
kendo.bind($("#registrationForm"), new MvvmSampleViewModel());
