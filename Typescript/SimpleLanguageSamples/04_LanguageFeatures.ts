/// <reference path="../../../tsd/DefinitelyTyped/node/node-0.11.d.ts" />

// Define a top-level module
module CrmModule {
  // Define an interface that specifies what a person must consist of.
  export interface IPerson {
    firstName: string;
    lastName: string;
  }
  
  // Note that Person would not need to specify "implements IPerson" 
  // explicitely. Even if the "implements" clause would not be there, 
  // Person would be compatible with IPerson because of structural subtyping.
  export class Person implements IPerson {
    private isNew: boolean;    // a private member only accessible inside Person
    public firstName: string;  // a public member accessible from outside
    
    // Here you see how to define a constructor
    // Note the keyword "public" used for parameter "lastName". It 
    // makes "lastName" a public property. "firstName" is assigned manually.
    constructor(firstName: string, public lastName: string) {
      this.firstName = firstName;
    }
    
    // A public method...
    public toString() {
      return this.lastName + ", " + this.firstName;
    }
    
    // A public get accessor...
    public get isValid() {
      return this.isNew || 
        (this.firstName.length > 0 && this.lastName.length > 0);
    }
    
    // Note the function type literal used for the "completeCallback" parameter.
    // "repository" has no type. Therefore it is of type "Any".
    public savePerson(repository, completedCallback: (bool) => void) {
      var code = repository.saveViaRestService(this);
      completedCallback(code === 200);
    }
  }
  
  // Create derived classes using the "extends" keyword
  export class VipPerson extends Person {
    // Note that "VipPerson" does not define a constructor. It gets a
    // constructor with appropriate parameters from its base class
    // automatically.
    
    // Note how we override "toString" here. Use "super" to access 
    // the base class.
    public toString() {
      return super.toString() + " (VIP)";
    }
  }
  
  // Define a nested module inside of CrmModule
  export module Sales {
    export class Opportunity {
      public potentialRevenueEur: number;
      public contacts: IPerson[];      // Array type
      
      // Note that we use the "IPerson" interface here.
      public addContact(p: IPerson) {
        this.contacts.push(p);
      }
      
      // A static member...
      static convertToUsd(amountInEur: number): number {
        return amountInEur * 1.3;
      }
    }
  }
}

// Note how we instanciate the Person class here.
var p: CrmModule.Person;
p = new CrmModule.Person("Max", "Muster");

// Change the HTML DOM via TypeScript. Try to play around with this code
// in the TypeScript Playground and you will see that you have IntelliSense
// when working with the DOM. Accessing the DOM is type safe.
var button = document.createElement('button')
button.innerText = p.toString()
button.onclick = function() {
  alert("Hello" + p.firstName)
}
document.body.appendChild(button)

// Call a method and pass a callback function.
var r = { 
  saveViaRestService: function (p: CrmModule.Person) {
    alert("Saving " + p.toString());
    return 200;
  }
};
p.savePerson(r, function(success: string) { alert("Saved"); });

// Create an instance of the derived class.
var v: CrmModule.VipPerson;
v = new CrmModule.VipPerson("Tom", "Turbo");
// Note how we access the get accessor here.
if (!v.isValid) {
  alert("Person is invalid");
}
else {
  // Not that "toString" calls the overridden version from the derived class
  // VipPerson.
  alert(v.toString());
}

// Note how we import a module here and assign it the alias "S".
import S = CrmModule.Sales;
var s: S.Opportunity;
s = new S.Opportunity();
s.potentialRevenueEur = 1000;
// Note structural subtyping here. You can call "addContact" with 
// any object type compatible with IPerson.
s.addContact(v);
s.addContact({ firstName: "Rainer", lastName: "Stropek" });
s.addContact(<CrmModule.IPerson> { firstName: "Rainer", lastName: "Stropek" });
var val = S.Opportunity.convertToUsd(s.potentialRevenueEur);
