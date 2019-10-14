import { IPoint } from './point';

export class DrawingCanvas {
    private canvas: HTMLCanvasElement;
    private bb: ( ClientRect | DOMRect );
    private isMouseDown = false;
    private previousPoint: IPoint;
    private color = "rgb(0,0,0)";
    private ctx: CanvasRenderingContext2D;
    private cb: (prev: IPoint, next: IPoint) => void;

    constructor(canvasId: string, drawnCallback: (prev: IPoint, next: IPoint) => void) {
        this.canvas = document.getElementById(canvasId) as HTMLCanvasElement;
        this.ctx = this.canvas.getContext("2d");
        this.ctx.lineWidth = 3;
        this.bb = this.canvas.getBoundingClientRect();
        
        this.canvas.addEventListener("mousedown", () => this.isMouseDown = true, false);
        this.canvas.addEventListener("mouseup", () => this.mouseUp(), false);
        this.canvas.addEventListener("mousemove", (ev: MouseEvent) => this.move(ev), false);

        this.cb = drawnCallback;
    }

    public set drawingColor(drawColor: string) {
        this.color = drawColor;
    }
    
    private mouseUp() {
        this.isMouseDown = false;
        this.previousPoint = undefined;
    }
    
    private move(ev: MouseEvent) {
        if (!this.isMouseDown) return;
    
        const nextPoint = { x: ev.pageX - this.bb.left, y: ev.pageY - this.bb.top };
        if (this.previousPoint) {
            this.drawLine(this.previousPoint, nextPoint, this.color);
            this.cb(this.previousPoint, nextPoint);
        }
    
        this.previousPoint = nextPoint;
    }
    
    public drawLine(prev: IPoint, next: IPoint, color: string) {
        this.ctx.strokeStyle = color;
        this.ctx.strokeStyle = color;
        this.ctx.beginPath();
        this.ctx.moveTo(prev.x, prev.y);
        this.ctx.lineTo(next.x, next.y);
        this.ctx.stroke();
    }
}

