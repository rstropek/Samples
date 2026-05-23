import express from 'express';
import { calculateFractalTreePoints, LinePoint } from './calculate_tree';

const app = express();
const port = 3000;

function generateHtml(svg: string): string {
    return `<!DOCTYPE html>
    <html>
    <head>
        <title>Fractal Tree</title>
        <style>
            .basic {
                stroke: black;
                stroke-width: 1px;
            }
        </style>
    </head>
    <body>
        ${svg}
    </body>
    </html>`;
}

function linePointToSVG(point: LinePoint): string {
    return `<line class="basic" x1="${point.x1}" y1="${point.y1}" x2="${point.x2}" y2="${point.y2}" />`;
}

function generateFractalTreeSVG(): string {
    const treePoints = calculateFractalTreePoints({ x1: 300, y1: 500, angle: -90, length: 100, level: 0 });
    const svgLines = treePoints.map(linePointToSVG).join('');
    return `<svg width="600" height="600" xmlns="http://www.w3.org/2000/svg">${svgLines}</svg>`;
}

app.get('/simple', (_, res) => {
    const svg = generateFractalTreeSVG();
    res.send(generateHtml(svg));
});

app.listen(port, () => {
    console.log(`Server is running at http://localhost:${port}`);
});