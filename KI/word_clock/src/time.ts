export type time = {
  /**
   * The hour of the day (1-12)
   */
  h: number;

  /**
   * The minute of the hour (0-59)
   */
  m: number;
};

/**
 * Converts a Date object to a time object
 *
 * Converts 24-hour time to 12-hour time and adjusts for midnight and noon
 * (0 should be 12)
 */
export function toTime(date: Date): time {
  const t = { h: date.getHours(), m: date.getMinutes() };

  // Convert 24-hour time to 12-hour time
  t.h = t.h % 12;
  t.h = t.h ? t.h : 12; // the hour '0' should be '12'

  return t;
}

/**
 * Rounds a time to the nearest 5 minutes
 *
 * If rounded minutes go over 59, the hour is adjusted accordingly.
 */
export function roundToNearestFive(date: time): time {
  const roundedMinutes = Math.round(date.m / 5) * 5;

  if (roundedMinutes >= 60) {
    date.h += Math.floor(roundedMinutes / 60);
    date.m = roundedMinutes % 60;
  } else {
    date.m = roundedMinutes;
  }

  // Adjust for going over 23 hours
  if (date.h >= 24) {
    date.h = date.h % 24;
  }

  return { h: date.h, m: date.m };
}

