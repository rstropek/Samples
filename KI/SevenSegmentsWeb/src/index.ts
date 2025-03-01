import "./index.css";
import { createDigitDiv, setDigit } from "./segmentDigit";

const input = document.getElementById("input") as HTMLInputElement;
const sevenSegments = document.getElementById("seven-segments") as HTMLDivElement;

let lastValidValue = "";
input.addEventListener("input", () => {
    const value = parseInt(input.value);
    if (isNaN(value) || value < 0 || value > 9999) {
        input.value = lastValidValue;
        return;
    }

    lastValidValue = input.value;

    const valueStr = value.toString();
    const digits = valueStr.padStart(4, " ").split("").map(c => c === " " ? undefined : Number(c));
    for (let i = 0; i < 4; i++) {
        const segmentDigit = sevenSegments.childNodes[i] as HTMLDivElement;
        setDigit(segmentDigit, digits[i]);
    }
});

for (let i = 0; i < 4; i++) {
    const digit = createDigitDiv();
    sevenSegments.appendChild(digit);
}
