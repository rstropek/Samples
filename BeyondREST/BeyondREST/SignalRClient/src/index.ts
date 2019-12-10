import "./index.css";
import * as signalR from "@aspnet/signalr";
import { IPoint } from './point';
import { DrawingCanvas } from './canvas';

window.onload = async () => {
    const canvas = new DrawingCanvas(
        'drawingCanvas',
        (prev, next) => connection.invoke("draw", prev, next));

    // Setting up connection to SignalR hub
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:5001/hub")
        .configureLogging(signalR.LogLevel.Debug)
        .build()

    // Handle setColor message
    connection.on('setColor', (message: string) => {
        canvas.drawingColor = message;
    });

    // Handle a draw request (i.e. line drawn by another user)
    connection.on('draw', (previousPoint: IPoint, nextPoint: IPoint, color: string) => {
        canvas.drawLine(previousPoint, nextPoint, color);
    });

    // Start SignalR connection
    await connection.start();
    console.log('We are connected :-)');
};
