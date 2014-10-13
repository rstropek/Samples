export module customer {
    /**
      * Represents a customer
      * @param firstName First name of customer
      * @param lastName Last name of customer
      */
    export interface ICustomer {
        firstName: string;
        lastName: string;
    }

    /**
      * Represents a customer
      */
    export class Customer implements ICustomer {
        public firstName: string;
        public lastName: string;

        constructor (arg: ICustomer = { firstName: "", lastName: "" }) {
            this.firstName = arg.firstName;
            this.lastName = arg.lastName;
        }

        /**
          * Returns the full name of the customer
          */
        public fullName() {
            return this.lastName + ", " + this.firstName;
        }
    }
}