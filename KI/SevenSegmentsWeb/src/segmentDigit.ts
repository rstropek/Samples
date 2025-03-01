import { getSegmentBit, getSegmentBitsForDigit, SegmentBits, Segments } from "./segmentBits";

const digitSvg = `<svg width="48" height="80" viewBox="0 0 10 18">
  <g>
    <!-- See also https://en.wikipedia.org/wiki/Seven-segment_display#/media/File:7_Segment_Display_with_Labeled_Segments.svg -->
    <polygon points="1, 1  2, 0  8, 0  9, 1  8, 2  2, 2" /><!-- a -->
    <polygon points="9, 1 10, 2 10, 8  9, 9  8, 8  8, 2" /><!-- b -->
    <polygon points="9, 9 10,10 10,16  9,17  8,16  8,10" /><!-- c -->
    <polygon points="9,17  8,18  2,18  1,17  2,16  8,16" /><!-- d -->
    <polygon points="1,17  0,16  0,10  1, 9  2,10  2,16" /><!-- e -->
    <polygon points="1, 9  0, 8  0, 2  1, 1  2, 2  2, 8" /><!-- f -->
    <polygon points="1, 9  2, 8  8, 8  9, 9  8,10  2,10" /><!-- g -->
  </g>
</svg>`;

export function createDigitDiv(): HTMLDivElement {
    const digit = document.createElement("div");
    digit.innerHTML = digitSvg;
    return digit;
}

export function setDigit(div: HTMLDivElement, digit?: number) {
    const polygons = div.querySelectorAll("polygon");
    let bits: number;
    if (!digit) {
        bits = 0;
    } else {
        bits = getSegmentBitsForDigit(digit);
    }

    for (let i = 0; i < SegmentBits.length; i++) {
        const polygon = polygons[i];
        const bit = SegmentBits[i];
        if (getSegmentBit(bits, bit)) {
            polygon.classList.add("on");
        } else {
            polygon.classList.remove("on");
        }
    }
}