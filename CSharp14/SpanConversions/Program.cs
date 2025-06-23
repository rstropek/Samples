#pragma warning disable CS0219 // Variable is assigned but its value is never used

Span<int> myIntSpan;
Span<Person> myPersonSpan;
Span<Employee> myEmployeeSpan;
ReadOnlySpan<int> myReadOnlyIntSpan;
ReadOnlySpan<Person> myReadOnlyPersonSpan;
ReadOnlySpan<Employee> myReadOnlyEmployeeSpan;

// Implicit conversion from single-dimensional array
myIntSpan = new int[10];
myReadOnlyIntSpan = new int[10];
myPersonSpan = new Person[10];
myEmployeeSpan = new Employee[10];
myReadOnlyPersonSpan = new Employee[10]; // Possible because of covariance 
                                         // conversion from Employee to Person 
                                         // is possible

// Span to ReadOnlySpan
myReadOnlyIntSpan = myIntSpan;
myReadOnlyPersonSpan = myPersonSpan;
myReadOnlyPersonSpan = myEmployeeSpan; // Covariance
myReadOnlyEmployeeSpan = myEmployeeSpan;

// ReadOnlySpan to ReadOnlySpan
myReadOnlyPersonSpan = myReadOnlyEmployeeSpan; // Covariance

// string to ReadOnlySpan<char>
ReadOnlySpan<char> myReadOnlyCharSpan = "Hello, World!";

abstract class Person { }    

class Employee : Person { }
