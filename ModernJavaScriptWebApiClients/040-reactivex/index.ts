import * as Rx from 'rxjs';
import 'rxjs/add/observable/dom/ajax';
import { Observable } from 'rxjs/Observable';

// The basics
(function () {
    var observable = Rx.Observable.create(function (observer) {
        observer.next(1);
        observer.next(2);
        observer.next(3);
        setTimeout(() => {
            observer.next(4);
            //observer.error("ERROR");
            observer.next(5);
            observer.complete();
        }, 1000);
    });

    console.log('just before subscribe');
    observable.subscribe({
        next: x => console.log('got value ' + x),
        error: err => console.error('something wrong occurred: ' + err),
        complete: () => console.log('done'),
    });
    console.log('just after subscribe');
}); //();

// DOM events
(function () {
    let counter = 0;
    const button = document.querySelector("button");
    Rx.Observable.fromEvent(button, "click")
        .debounceTime(250)
        .filter(() => counter < 5)
        .subscribe(() => console.log(`Clicked button ${++counter} times...`));
});//();

// Samples for creating observables
(function () {
    // Read more at http://reactivex.io/documentation/operators.html
    Rx.Observable.from([1, 2, 3]).forEach(v => console.log(v));
    Rx.Observable.range(1, 3).forEach(v => console.log(v));

    // Note that the following two lines do not block the program. They
    // execute the observer code asynchronously.
    Rx.Observable.interval(1000).take(3).forEach(v => console.log(v + 1));
    Rx.Observable.timer(1000, 0).take(3).forEach(v => console.log(v + 1))
        // Note how we use the returned promise to execute code once 
        // observing is finished.
        .then(_ => console.log("Done!"));
}); //();

// Subscribing and unsubscribing
(function () {
    // Note that we create an "endless" interval here -> to stop the subscription,
    // we have to explicitly unsubscribe.
    const subscription = Rx.Observable.interval(250).subscribe(v => console.log(v));
    console.log(subscription.closed);
    setTimeout(() => {
        subscription.unsubscribe();
        console.log(subscription.closed);
    }, 1000);
}); //();

// Combining observables
(function () {
    const o1 = Rx.Observable.from([1, 2]);
    const o2 = Rx.Observable.from([3, 4]);
    const o3 = o1.concat(o2);
    o3.forEach(v => console.log(v));

    // Try `concat` and `merge` with async code...
    const asyncO1 = Rx.Observable.interval(1000).map(v => v + 1).take(3);
    const asyncO2 = Rx.Observable.interval(2000).map(v => (v + 1) * 10).take(3);
    const asyncO3 = asyncO1.concat(asyncO2);
    // const asyncO3 = asyncO1.merge(asyncO2);
    asyncO3.forEach(v => console.log(v));
}); //();

// Error handling
(function () {
    const o = Rx.Observable.from([1, 2]).concat(
        Rx.Observable.throw(new Error("Something happened...")));
    o.subscribe(
        v => console.log(v),
        err => console.log(`ERROR: ${err.message}`));
}); //();

// Having fun with some more operators
(function () {
    // Try with and without debounce
    Rx.Observable.interval(10).map(v => 1).take(5)
        .merge(Rx.Observable.timer(1000, 10).map(v => 2).take(5))
        .debounceTime(100)
        .forEach(v => console.log(v));
}); //();

interface IPokemon {
    url: string;
    name: string;
}

(function () {
    Rx.Observable.ajax('https://pokeapi.co/api/v2/pokemon/')
        .map(e => e.response)
        .flatMap(e => <IPokemon[]>e.results)
        .map(e => e.name)
        .forEach(e => console.log(e));
})();