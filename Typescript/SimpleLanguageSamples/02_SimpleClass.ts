class Person {
    public lastName: string;

    constructor (public firstName: string, lastName: string) {
        this.lastName = lastName;
    }

    public fullName() {
        return this.firstName + " " + this.lastName;
    }
}

var p = new Person("Mad", "Max");
