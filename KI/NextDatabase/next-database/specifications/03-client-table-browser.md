# Add Client-Side Browser

There is already a simple table browser in [page.tsx](../src/app/browse/[table]/page.tsx) and [page.module.css](../src/app/browse/[table]/page.module.css). It is a server component.

Because of technical reasons, I consider switching to a client-side table browser. So please:

* Implement a corresponding web API in the [api](../src/app/api) folder.
* Add a client-side table browser `browse-client` in the [app](../src/app) folder.
* The functionality should be identical to the existing server component mentioned above.

Implementation notes:

* No need for authentication for the web api yet.
* Keep formating to a minimum.
* Do not remove the server component yet.

