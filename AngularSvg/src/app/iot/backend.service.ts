import { Injectable, OnInit } from '@angular/core';
import { Observable, Subject } from 'rxjs';

export interface Message {
  messageType: number;
}

export interface SiloFillMessage extends Message {
  deviceId: number;
  value: number;
}

export function isSiloFillMessage(arg: any): arg is SiloFillMessage {
  return arg.messageType === 1;
}

@Injectable({
  providedIn: 'root'
})
export class BackendService {
  private messages = new Subject<Message>();

  public get messages$(): Observable<Message> {
    return this.messages;
  }

  constructor() {
    // Create WebSocket connection.
    const socket = new WebSocket('ws://localhost:3000');

    // Listen for messages
    const that = this;
    socket.addEventListener('message', function (event) {
      const message: Message = JSON.parse(event.data);
      that.messages.next(message);
    });
  }
}
