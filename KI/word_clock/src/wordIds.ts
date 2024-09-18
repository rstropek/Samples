import { time } from "./time";

export type WordIDs = {
  it?: boolean; // Always active
  is?: boolean; // Always active
  quarter?: boolean; // 15 or 45 minutes
  twenty?: boolean; // 20 or 40 minutes
  five?: boolean; // 5, 25, 35 or 55 minutes
  ten?: boolean; // 10 or 50 minutes
  half?: boolean; // 30 minutes
  minutes?: boolean; // 5, 10, 20, 25, 35, 40, 50 or 55 minutes
  past?: boolean; // Active if minutes <= 30
  to?: boolean; // Active if minutes > 30
  one?: boolean;
  two?: boolean;
  three?: boolean;
  four?: boolean;
  five_hour?: boolean;
  six?: boolean;
  seven?: boolean;
  eight?: boolean;
  nine?: boolean;
  ten_hour?: boolean;
  eleven?: boolean;
  twelve?: boolean;
  oclock?: boolean; // 0 minutes
};

export function setActiveSegments(currentTime: time): WordIDs {
  const activeSegments: WordIDs = {};

  // Always active segments
  activeSegments.it = true;
  activeSegments.is = true;

  let hours = currentTime.h;
  let minutes = currentTime.m;

  // Determine if it's past or to the next hour
  if (minutes > 30) {
    activeSegments.to = true;
    hours = (hours % 12) + 1; // Adjust hour for 'to'
  } else if (minutes > 0) {
    activeSegments.past = true;
  }

  // Activate segments based on minutes
  switch (true) {
    case minutes === 0:
      activeSegments.oclock = true;
      break;
    case minutes === 5 || minutes === 55:
      activeSegments.five = true;
      activeSegments.minutes = true;
      break;
    case minutes === 10 || minutes === 50:
      activeSegments.ten = true;
      activeSegments.minutes = true;
      break;
    case minutes === 15 || minutes === 45:
      activeSegments.quarter = true;
      break;
    case minutes === 20 || minutes === 40:
      activeSegments.twenty = true;
      activeSegments.minutes = true;
      break;
    case minutes === 25 || minutes === 35:
      activeSegments.twenty = true;
      activeSegments.five = true;
      activeSegments.minutes = true;
      break;
    case minutes === 30:
      activeSegments.half = true;
      break;
  }

  // Activate segment for the current hour
  switch (hours) {
    case 1:
      activeSegments.one = true;
      break;
    case 2:
      activeSegments.two = true;
      break;
    case 3:
      activeSegments.three = true;
      break;
    case 4:
      activeSegments.four = true;
      break;
    case 5:
      activeSegments.five_hour = true;
      break;
    case 6:
      activeSegments.six = true;
      break;
    case 7:
      activeSegments.seven = true;
      break;
    case 8:
      activeSegments.eight = true;
      break;
    case 9:
      activeSegments.nine = true;
      break;
    case 10:
      activeSegments.ten_hour = true;
      break;
    case 11:
      activeSegments.eleven = true;
      break;
    case 12:
      activeSegments.twelve = true;
      break;
  }

  return activeSegments;
}
