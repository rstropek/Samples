import { Observable } from 'rxjs/Observable';
import { timer } from 'rxjs/observable/timer';
import { concat } from 'rxjs/observable/concat';
import { of } from 'rxjs/observable/of';
import { map } from 'rxjs/operators/map';
import { filter } from 'rxjs/operators/filter';
import { take } from 'rxjs/operators/take';
import { tap } from 'rxjs/operators/tap';
import { _throw } from 'rxjs/observable/throw';

// Alternative:
// import { filter, map} from 'rxjs/operators';

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
        .pipe(tap(n => console.log(`  Looking into ${n}`)))
        .subscribe(n => console.log(n));
}); //();

(() => {
    of(1, 2, 3, 4)
        .pipe(filter(n => n % 2 == 0), map(n => n * n))
        .subscribe(n => console.log(n));

    const removeEven = () => (source: Observable<number>) => source.pipe(filter(n => n % 2 !== 0));

    of(1, 2, 3, 4)
        .pipe(removeEven(), map(n => n * n))
        .subscribe(n => console.log(n));
}); //();

(() => {
    console.log("Starting count down:");
    timer(0, 1000).pipe(map(n => 5 - n), take(6)).subscribe(n => console.log(n));
}); //();


(() => {
    try {
        concat(of(1), _throw('Something went wrong')).subscribe(n => console.log(n));
    } catch (ex) {
        console.error(ex);
    }
}); //();