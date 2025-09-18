This is a sample casual browser game for a training.

Tech Stack:
* TypeScript
* Vite
* Vanilla.js (no frameworks)
* Uses plain HTML+SVG+CSS, no Canvas, SASS or similar preprocessors
* jest and ts-jest for testing (node)

Use the latest version of TypeScript. Prefer types over interfaces. This project uses strict TypeScript typing rules. So make sure to add proper typings everywhere.

Introduce modules and/or classes where appropriate. Each file should focus on a single responsibility.

Here is a sample test (`dummy.spec.ts`) that shows how to setup test files:

```ts
import {describe, expect, test} from '@jest/globals';

describe('dummy', () => {
  test('adds 1 + 2 to equal 3', () => {
    expect(1 + 2).toBe(3);
  });
});
```

Do not write CSS in HTML files. Use separate CSS files instead.

You can always assume that I ran `npm install` in the project folder. You do not need to do that again. You can also assume that TypeScript, Jest, and Vite are configured properly.
