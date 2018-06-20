import {HttpClient} from '@angular/common/http';
import {Component, OnInit} from '@angular/core';
import { environment } from '../environments/environment';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  public queryResult: Observable<any>;
  public query = '';
  public queries = [
    {
      description: 'Help',
      options: [
        'How can you help me',
        'What can you tell me',
        'What kind of questions can I ask you'
      ]
    },
    {
      description: 'Venue Location',
      options: [
        'I need help finding the conference venue'
      ]
    },
    {
      description: 'Feedback',
      options: [
        'I have feedback about the program',
        'Open the feedback form',
        'Let me give feedback',
        'Lead me to the feedback form',
        'I want to tell you about the quality of the tea',
        'I think the dinner was excellent',
        'The coffee tastes great',
        'The lunch tasted horrible'
      ]
    },
    {
      description: 'Food and Drinks',
      options: [
        'I want coffee',
        'I need food',
        'I am starving',
        'Where can I get snacks',
        'Please tell me where to find tea',
        'Where is coffee served'
      ]
    },
    {
      description: 'Next Sessions',
      options: [
        'what\'s up next in A09',
        'Sessions in A01 between 11am and 1pm'
      ]
    }
  ];

  constructor(private httpClient: HttpClient) {}

  ngOnInit(): void {
    if (this.query) {
      this.search();
    }
  }
  
  public search(): void {
    this.queryResult = this.httpClient.get(`${environment.luisUrl}${this.query}`);
  }
}
