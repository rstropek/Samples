// Let's create two functions simulating async math
const add = (x: number, y: number) => new Promise<number>((res, rej) => setTimeout(() => res(x + y), 250));
const mult = (x: number, y: number) => new Promise<number>((res, rej) => setTimeout(() => res(x * y), 250));

// This is what we want to calculate: (x + y) * z

(async function () {
    const x = 10.5, y = 10.5, z = 2;

    try {
        console.log(await mult(await add(x, y), z));
    } catch (ex) {
        console.log("Something happened...");
    }
})();
