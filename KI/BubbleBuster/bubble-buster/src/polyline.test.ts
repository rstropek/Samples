import { expect, test } from 'vitest';
import { Vector } from './vector';
import { Polyline } from './polyline';

test('Polyline: constructor', () => {
  const polyline = new Polyline([new Vector(0, 0), new Vector(1, 1)]);
  expect(polyline.points.length).toBe(2);
});

test('Polyline: clear', () => {
  const polyline = new Polyline([new Vector(0, 0), new Vector(1, 1)]);
  polyline.clear();
  expect(polyline.points.length).toBe(0);
});

test('Polyline: push', () => {
  const polyline = new Polyline();
  polyline.push(new Vector(0, 0));
  expect(polyline.points.length).toBe(1);
});

test('Polyline: doCross', () => {
  const polyline1 = new Polyline([new Vector(0, 0), new Vector(1, 1)]);
  const polyline2 = new Polyline([new Vector(1, 0), new Vector(0, 1)]);
  expect(polyline1.doCross(polyline2)).toBe(true);
});

test('Polyline: doCross returns false', () => {
  const polyline1 = new Polyline([new Vector(0, 0), new Vector(1, 1)]);
  const polyline2 = new Polyline([new Vector(1, 1), new Vector(2, 2)]);
  expect(polyline1.doCross(polyline2)).toBe(false);
});

test('Polyline: isClosed (same point)', () => {
  const polyline = new Polyline([new Vector(0, 0), new Vector(1, 1), new Vector(1, 0), new Vector(0, 0)]);
  expect(polyline.isClosed()).toBe(true);
});

test('Polyline: isClosed (crosses)', () => {
  const polyline = new Polyline([new Vector(0, 1), new Vector(3, 3), new Vector(2, 5), new Vector(0.5, 0.5)]);
  expect(polyline.isClosed()).toBe(true);
});

test('Polyline: isPointInside', () => {
  const polyline = new Polyline([new Vector(0, 0), new Vector(1, 0), new Vector(1, 1), new Vector(0, 1), new Vector(0, 0)]);
  expect(polyline.isPointInside(new Vector(0.5, 0.5))).toBe(true);
});

test('Polyline: isPointInside (not closed)', () => {
  const polyline = new Polyline([new Vector(0, 0), new Vector(1, 0), new Vector(1, 1), new Vector(0, 1), new Vector(0.1, 0.1)]);
  expect(polyline.isPointInside(new Vector(0.5, 0.5))).toBe(false);
});

test('Polyline: isPointInside returns false', () => {
  const polyline = new Polyline([new Vector(0, 0), new Vector(1, 0), new Vector(1, 1), new Vector(0, 1), new Vector(0, 0)]);
  expect(polyline.isPointInside(new Vector(2, 2))).toBe(false);
});

test('Polyline: isFullyInside', () => {
  const polyline1 = new Polyline([new Vector(0, 0), new Vector(2, 0), new Vector(2, 2), new Vector(0, 2), new Vector(0, 0)]);
  const polyline2 = new Polyline([new Vector(1, 1), new Vector(1.5, 1), new Vector(1.5, 1.5), new Vector(1, 1.5), new Vector(1, 1)]);
  expect(polyline1.isFullyInside(polyline2)).toBe(true);
});

test('Polyline: isFullyInside', () => {
  const polyline1 = new Polyline([new Vector(0, 0), new Vector(2, 0), new Vector(2, 2), new Vector(0, 2), new Vector(0, 0)]);
  const polyline2 = new Polyline([new Vector(1, 1), new Vector(3, 1), new Vector(1.5, 1.5), new Vector(1, 1.5), new Vector(1, 1)]);
  expect(polyline1.isFullyInside(polyline2)).toBe(false);
});

test('Polyline: doIntersect', () => {
  const p1 = new Vector(0, 0);
  const q1 = new Vector(1, 1);
  const p2 = new Vector(1, 0);
  const q2 = new Vector(0, 1);
  expect(Polyline.doIntersect(p1, q1, p2, q2)).toBe(true);
});

test('Polyline: orientation', () => {
  const p = new Vector(0, 0);
  const q = new Vector(1, 0);
  const r = new Vector(0, 1);
  expect(Polyline.orientation(p, q, r)).toBe(2);
});
