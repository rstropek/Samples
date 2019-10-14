import "./index.css";
import { IPoint } from './point';
import { DrawingCanvas } from './canvas';

window.onload = async () => {
    const canvas = new DrawingCanvas(
        'drawingCanvas',
        (prev, next) => { });
};
