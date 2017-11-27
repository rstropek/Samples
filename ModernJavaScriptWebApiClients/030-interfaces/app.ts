// Learn more about interfaces in http://www.typescriptlang.org/docs/handbook/interfaces.html
// Learn more about classes in http://www.typescriptlang.org/docs/handbook/classes.html

// Note that some code lines are commented in this sample. They
// would lead to compiler errors.

interface IPerson {
    firstName: string;
    lastName: string;
    age?: number;               // Note optional member
}

interface IPersonWithDescription extends IPerson {
    getDescription(): string;
}

class Person implements IPerson {
    public firstName: string;
    public lastName: string;
    public age: number;

    // Note that 'Person' does not explicity say that it is
    // compatible with 'IPersonWithDescription', but it implicitly is
    // because all necessary members are implemented. This concept is called
    // 'structural subtyping' (details in http://www.typescriptlang.org/docs/handbook/type-compatibility.html)
    public getDescription(): string {
        return `${this.firstName} ${this.lastName} is ${this.age} years old`;
    }
}

class SimplePerson {
    // Note that 'SimplePerson' does not explicity say that it is
    // compatible with 'IPerson', but it still is.
    constructor(public firstName: string, public lastName: string) { }

    // Note "contextual typing" in the following function
    public getDescription() {
        return `I am ${this.firstName} ${this.lastName}`;
    }

    // Note accessor here ('set' accessor would also be possible)
    get fullName() {
        return `${this.firstName} ${this.lastName}`;
    }
}

let p: IPerson;
p = new Person();
p = new SimplePerson('Foo', 'Bar');
console.log((<SimplePerson>p).fullName);
p = { firstName: 'Foo', lastName: 'Bar' };
p = { firstName: 'Foo', lastName: 'Bar', age: 99 };
//p = { firstNme: 'Foo', lastName: 'Bar', age: 99 };

const pWithDescription: IPersonWithDescription = new Person();

// Interface with 'readonly' members
interface IAccount {
    readonly accountNumber: number;
    balance: number;
}

var account: IAccount = { accountNumber: 1, balance: 100 };
account.balance += 10;
//account.accountNumber = 2;

// Generic interfaces and classes
interface ICursor<T> {
    readonly current: T;
    moveNext(): boolean;
};
class Cursor<T> implements ICursor<T> {
    private index = -1;

    constructor(private list: ReadonlyArray<T>) { }

    get current(): T {
        if (this.index < 0) {
            throw new Error("moveNext never called");
        }

        return this.list[this.index];
    }

    moveNext(): boolean {
        if (this.list.length == 0 || this.index >= (this.list.length - 1)) {
            return false;
        }

        this.index++;
        return true;
    }
}
let c = new Cursor([1, 2, 3, 4]);
while (c.moveNext()) {
    console.log(c.current);
}

interface ICommented {
    readonly comment: string;
}
class CommentedItems<T extends ICommented> {
    constructor(private items: T[]) { }
    dump(): void {
        this.items.forEach(item => console.log(item.comment));
    }
}
(new CommentedItems([
    { id: 1, comment: "First" },
    { id: 2, comment: "Second" }]))
    .dump();
//(new CommentedItems([1, 2, 3])).dump();

