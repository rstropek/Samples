Set up the basic UI shell in the Angular frontend project. Read the `angular-frontend` and `frontend-design` skills for coding guidelines.

## Requirements

* **Desktop only** — no responsive design, no hamburger menus, no mobile breakpoints.
* **Content area** max width `1280px`, centered horizontally.
* **Top navigation bar** — full width, single level (no dropdowns, no nested menus). For now, add placeholder links: "Questionnaires" and "About".
* Use **Angular Router** with lazy-loaded routes for each menu item. Create stub components for each route.

## Icons

Install Angular FontAwesome following the instructions at `https://raw.githubusercontent.com/FortAwesome/angular-fontawesome/refs/heads/main/README.md`. Use FontAwesome icons in the navigation bar (e.g. `faClipboardList` for Questionnaires, `faCircleInfo` for About).

## Visual Design

* Pick a professional, modern color theme — not the Angular default colors. Use CSS custom properties (variables) in `styles.css` for the palette so it is easy to adjust later.
* Choose a clean sans-serif Google Font (e.g. Inter, Source Sans 3, or similar). Import it in `index.html` or `styles.css`.
* The nav bar should have a subtle bottom border or shadow to separate it from the content area.
* Active route link should be visually distinct (e.g. different color, underline, or background).

## After making changes

* Build the Angular project (`pnpm run build` in the Frontend folder)
* Start the Aspire application and verify the frontend loads correctly using the Aspire CLI (read the `aspire` skill for CLI details)
* Smoke test with Playwright CLI to ensure that everything renders properly

