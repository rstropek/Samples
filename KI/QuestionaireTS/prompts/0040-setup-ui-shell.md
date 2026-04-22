Set up the basic UI shell in the Angular frontend project. Read the `frontend-design` skill for guidelines.

## Requirements

* **Desktop only** — no responsive design, no hamburger menus, no mobile breakpoints.
* **Content area** max width `1280px`, centered horizontally.
* **Top navigation bar** — full width, single level (no dropdowns, no nested menus). For now, add placeholder links: "Questionnaires" and "About".
* Use FontAwesome icons in the navigation bar (e.g. `faClipboardList` for Questionnaires, `faCircleInfo` for About).

## Visual Design

* Pick a professional, modern color theme — not the Next.js default colors. Use CSS custom properties (variables) in global styles for the palette so it is easy to adjust later.
* Choose a clean sans-serif Google Font. Import it in `index.html` or `styles.css`.
* The nav bar should have a subtle bottom border or shadow to separate it from the content area.
* Active route link should be visually distinct (e.g. different color, underline, or background).

## After making changes

Follow the Quality Assurance steps in AGENTS.md. Additionally: Smoke test with Playwright CLI to ensure that everything renders properly.

