// Import express with body parsers (for handling JSON)
import express = require('express');
var bodyParser = require('body-parser');


// Business logic and data structures
interface IRegistration {
	salutation: string;
	name: string;
	age: number;
}

class Registration implements IRegistration {
	public salutation: string;
	public name: string;
	public age: number;

	constructor(registration: IRegistration) {
		this.salutation = registration.salutation;
		this.name = registration.name;
		this.age = registration.age;
	}

	public isValid() {
		return this.age >= 18;
	}
}

// Sample repository of registrations (for demo purposes just in memory
var registrations = new Array<IRegistration>();
registrations.push(
	{ salutation: "Mr.", name: "Tom Tailor", age: 20 },
	{ salutation: "Mr.", name: "Max Muster", age: 19 });


// Setup express
var app = express();
app.use(bodyParser());

// Uncommend this line to demo basic auth
// app.use(express.basicAuth((user, password) => user == "user2" && password == "password"));

// Allow CORS for debugging purposes
app.all("/*", function (req, res, next) {
	res.header("Access-Control-Allow-Origin", "*");
	res.header("Access-Control-Allow-Headers", "Cache-Control, Pragma, Origin, Authorization, Content-Type, X-Requested-With");
	res.header("Access-Control-Allow-Methods", "GET, PUT, POST");
	return next();
});
app.all("/api/*", function (req, res, next) {
	if (req.method.toLowerCase() !== "options") {
		return next();
	}
	return res.send(204);
});


// Implement web API
app.get("/registrations", (req, res) => {
	// Get all registrations
	res.send(registrations);
});

// Register
app.post("/register", (req, res) => {
	var registration = new Registration(<IRegistration>req.body);
	if (registration.isValid()) {
		registrations.push(registration);
		res.send(201);
	}
	else {
		res.send(400);
	}
});

// Listen for HTTP traffic
app.listen(process.env.PORT || 3000);
