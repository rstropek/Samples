export function random(min: number, max: number): number;
export function random<T>(arr: T[]): T;
export function random<T>(arg1: number | T[], arg2?: number): number | T {
	if (typeof arg1 === "number" && typeof arg2 === "number") {
		return Math.random() * (arg2 - arg1) + arg1;
	} else if (Array.isArray(arg1)) {
		const arr = arg1 as T[];
		return arr[Math.floor(Math.random() * arr.length)];
	}
	throw new Error("Invalid arguments");
}
