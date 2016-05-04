import {Component} from '@angular/core';
import {Http, HTTP_PROVIDERS} from '@angular/http';

interface IEvent {
    _id: string;
    location: string;
    date: string | Date;
}

@Component({
    selector: 'my-app',
    templateUrl: 'app/app.html'
})
export class AppComponent { 
    public events: IEvent[];
    
    constructor(private http: Http) {
        http.get("http://localhost:1337/api/events")
            .subscribe(e => {
                var data = <IEvent[]>e.json();
                this.events = data.map(item => { return { 
                    _id: item._id,
                    location: item.location,
                    date: new Date(<string>item.date) } })
            });
    }
    
    getDetails(item: IEvent) : void {
        console.log(item.location);
    }
}
