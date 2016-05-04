import {Component} from '@angular/core';

@Component({
    selector: 'my-app',
    templateUrl: 'app/app.html'
})
export class AppComponent { 
    public events: any[];
    
    constructor(private http: Http) {
		this.events = [];
    }
    
    getDetails(item: any) : void {
        console.log(item.location);
    }
}
