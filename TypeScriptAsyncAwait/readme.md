# TypeScript `async/await`

## Introduction

This sample demonstrates various aspects of TypeScript's `async/await` feature. It
contains three implementations of the same, very simple algorithm:

* Open Database
* Read all persons with first name "John"
* For each person:
	* If Person is customer:
        * Read customer details for person
        * Print person and customer details
    * Else:
        * Read supplier details for person
        * Print person and supplier details

## Samples

The first implementation in [plain-js-callbacks](plain-js-callbacks/app.ts) uses 
plain ECMAScript without frameworks for async programming or `async/await`. As you can see,
the resulting code is hard to understand and does not reflect the pseudocode shown above.

The second implementation in [async-js](async-js/app.ts) uses 
[async.js](http://caolan.github.io/async/) to simplify the async code.
As a result, our code is much more similar to the algorithm we try to express.

The third implementation in [async-await](async-await/app.ts) uses 
TypeScript's `async/await`. See how nice this code looks? The pseudocode translates
nearly 1:1 into TypeScript. It is easy to write and easy to read. Tip: Compile
the TypeScript code into JavaScript and make yourself familiar with the
generated code. 

Last but not least, [async-await-basics](async-await-basics/app.ts) covers some
async/await fundamentals like *Promises* async parallel execution of async.
operations.

## Running the Code

The examples use MongoDB as their underlying data store. If you do not
have MongoDB installed on your machine, you can use MongoDB's ready-made 
[Docker image](https://hub.docker.com/_/mongo/). Here is the command you have to run to start a MongoDB
container locally: `docker run -d -p 27017:27017 mongo`. To interactively work
with Mongo, I usually use [Robomongo](https://robomongo.org/). However, you
can use any client you want.

To compile and run the code, execute the following statements:

        npm install
        typings install
        tsc
        node app.js
