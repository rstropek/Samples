// Just some types and variables used during this demo...
interface IFoo {
    bar: string;
}
let promises: Promise<IFoo>[];

// Let's define an async function returning a Promise<T>
function doSomethingAsync(name: string = null): Promise<IFoo> {
    return new Promise<IFoo>((res, rej) => {
        // simulate some long running process (not CPU-bound)
        setTimeout(() =>
            // Resolve promise with a dummy result
            res({ bar: `Hello ${name || "Mr. X"}!` }),
            3000);
    });
}

// The basics...
(function () {
    // Let's call the async function and print the result when it is resolved
    doSomethingAsync()
        .then(foo => console.log(JSON.stringify(foo)));

    // Let's chain multiple promises
    doSomethingAsync()
        .then(foo =>
            // Note that we return another promise here
            doSomethingAsync(foo.bar))
        .then(result =>
            // Note that this result is not a promise, we can still continue with `then`
            result.bar.toUpperCase())
        .then(result => console.log(result));

    // Let's do something in parallel to our async function
    let counter = 0;
    const f = () => {
        console.log(`Heartbeat ${++counter}`);
        if (counter < 10) { setTimeout(f, 1000); }
    };
    f();
}); //();

// Execute multiple promises in parallel
(function () {
    promises = [doSomethingAsync(), doSomethingAsync(), doSomethingAsync()];
    Promise.all(promises)
        .then(foos => console.log(JSON.stringify(foos)));
}); //();

// Error handling
(function () {
    // This function returns a promise that will be rejected
    let counter = 1;
    const doSomethingBadAsync = () => new Promise<IFoo>((res, rej) => setTimeout(() => {
        if (counter++ % 3 == 0) {
            rej("I failed :-(");
        } else {
            res({ bar: "Dummy" });
        }
    }, 1000));

    // Handle success and failure in the `then` method
    for (let i = 0; i < 6; i++) {
        doSomethingBadAsync()
            .then(() => console.log("Everything is awesome!"), () => console.log("Shit happens"));
    }

    // See how catch stops a chain of `then` calls in case of an error
    doSomethingBadAsync()
        .then(() => { console.log("Good"); return doSomethingBadAsync(); })
        .then(() => { console.log("Good"); return doSomethingBadAsync(); })
        .then(() => { console.log("Good"); return doSomethingBadAsync(); })
        .then(() => { console.log("Good"); return doSomethingBadAsync(); })
        .then(() => { console.log("Good"); return doSomethingBadAsync(); })
        .catch(() => console.log("I handled the error properly..."));
}); //();

// Enter async/await
(function () {
    // Let's create two functions simulating async math
    const add = (x: number, y: number) => new Promise<number>((res, rej) => setTimeout(() => res(x + y), 250));
    const mult = (x: number, y: number) => new Promise<number>((res, rej) => setTimeout(() => res(x * y), 250));

    // This is what we want to calculate: (x + y) * z

    (function () {
        const x = 10.5, y = 10.5, z = 2;

        return add(x, y)
            .then(sum => mult(sum, z))
            .then(result => console.log(result))
            .catch(() => console.log("Something happened..."));
    }); //();

    (async function () {
        const x = 10.5, y = 10.5, z = 2;

        try {
            console.log(await mult(await add(x, y), z));
        } catch (ex) {
            console.log("Something happened...");
        }
    })();
}); //();