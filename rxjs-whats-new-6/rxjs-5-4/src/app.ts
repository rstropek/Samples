import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/from';
import 'rxjs/add/observable/timer';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/filter';
import 'rxjs/add/operator/take';
import 'rxjs/add/operator/do';

// Clear screen
console.log('\x1Bc');

(() => {
    const observable = new Observable((observer) => {
        observer.next(1);
        observer.next(2);
        setTimeout(() => {
        observer.next(3);
        observer.complete();
        }, 1000);
    });
    observable
        .do(n => console.log(`  Looking to ${n}`))
        .subscribe(n => console.log(n));
})();

(() => {
    Observable.from([1, 2, 3, 4])
        .filter(n => n % 2 == 0)
        .map(n => n * n)
        .subscribe(n => console.log(n));
})();

(() => {
    console.log("Starting count down:");
    Observable.timer(0, 1000)
        .map(n => 5 - n)
        .take(6)
        .subscribe(n => console.log(n));
})();
