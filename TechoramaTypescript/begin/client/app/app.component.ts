import {Component} from '@angular/core';

@Component({
    selector: 'my-app',
    templateUrl: 'app/app.html'
})
export class AppComponent { 
    public events: any[];
    
    constructor() {
		this.events = [
            { location: "Mechelen", date: new Date() }
        ];
    }
    
    getDetails(item: any) : void {
        console.log(item.location);
    }
}
