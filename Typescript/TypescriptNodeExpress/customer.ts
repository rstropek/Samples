export module customer {
    export interface ICustomer {
        firstName: string;
        lastName: string;
    }

    export class Customer implements ICustomer {
        public firstName: string;
        public lastName: string;

        constructor (arg: ICustomer = { firstName: "", lastName: "" }) {
            this.firstName = arg.firstName;
            this.lastName = arg.lastName;
        }

        public fullName() {
            return this.lastName + ", " + this.firstName;
        }
    }
}