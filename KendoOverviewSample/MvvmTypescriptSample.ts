/// <reference path="scripts/typings/kendouicore/kendo.web.d.ts" />
/// <reference path="scripts/typings/jquery/jquery.d.ts" />

// Note that our viewmodel extends kendo's ObservableObject.
// Because this samples uses Typescript, we can use strongly
// typed fields and function declarations.
class MvvmSampleViewModel extends kendo.data.ObservableObject {
	// List of available salutations
	public availableSalutations = ["Mr.", "Ms."];

	// List of fields as entered by the user
	public salutation = "Mr.";
	public name = "Max Muster";
	public birthday = new Date(1970, 4, 1);

	// Calculated fields
	public age: () => number;
	public isValid: () => boolean;
	public register: () => void;

	constructor() {
		super();

		// Setup calculated fields.
		// Note that this is done in the constructor for correct
		// "this" handling.
		this.age = () => {
			// Note that it is extremely important to use the "get" method in 
			// calculated properties. If you access e.g. "this.birthday" 
			// without "get", Kendo would not recognize that "age" depends 
			// on the "birthday" field.
			var ageDifMs = Date.now() - (<Date>this.get("birthday")).getTime();
			var ageDate = new Date(ageDifMs);
			return Math.abs(ageDate.getUTCFullYear() - 1970);
		};

		this.isValid = () => {
			return (<number>this.get("age")()) >= 18;
		};

		this.register = () => {
			if (<boolean>this.get("isValid")()) {
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
kendo.bind(
