import "./index.css";
import * as signalR from "@microsoft/signalr";
import { Point } from './point';
import { DrawingCanvas } from './canvas';

window.onload = async () => {
    const canvas = new DrawingCanvas(
        'drawingCanvas',
        (prev, next) => connection.invoke("draw", prev, next));

    // Setting up connection to SignalR hub
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5000/hub")
        .configureLogging(signalR.LogLevel.Debug)
        .build()

    // Handle setColor message
    connection.on('setColor', (message: string) => {
        canvas.drawingColor = message;
    });

    // Handle a draw request (i.e. line drawn by another user)
    connection.on('draw', (previousPoint: Point, nextPoint: Point, color: string) => {
        canvas.drawLine(previousPoint, nextPoint, color);
    });

    // Start SignalR connection
    await connection.start();
    console.log('We are connected :-)');
};
