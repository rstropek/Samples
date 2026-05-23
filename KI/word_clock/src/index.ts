import { roundToNearestFive, time, toTime } from "./time";
import { setActiveSegments, WordIDs } from "./wordIds";

let currentDateTime = new Date(2024, 7, 18, 15, 25);
let currentTime: time = roundToNearestFive(toTime(currentDateTime));

const activeSegments = setActiveSegments(currentTime);

(Object.keys(activeSegments) as (keyof WordIDs)[]).forEach((key) => {
  if (activeSegments[key]) {
    document.getElementById(key)?.classList.add('highlight');
  }
});

