import {Component} from 'angular2/core';
import {Http, HTTP_PROVIDERS} from 'angular2/http';

interface IBook {
    id: number;
    title: string;
    description: string;
    price: number;
}

interface IShoppingCartItem {
    book: IBook;
    quantity: number;
}

@Component({
    selector: 'my-app',
    templateUrl: 'app/app.html'
})
export class AppComponent {
    public books: IBook[];
    public cart: IShoppingCartItem[] = [];

    public addToShoppingCart(book: IBook) {
        var existing = this.cart.filter(b => b.book.id == book.id);
        if (existing && existing.length) {
            existing[0].quantity++;
        } else {
            this.cart.push({ book: book, quantity: 1 });
        }
    }

    constructor(http: Http) {
        http.get("/api/books")
            .subscribe(b => this.books = <IBook[]>b.json());
    }
}
