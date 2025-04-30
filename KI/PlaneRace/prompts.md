As you can see, this app displays a comic plane with thrust. Can you create a game from it with the following rules:

* if the user presses "d" 4 times per second, the plane keeps its x position.
* if the user presses "d" more than 4 times per second, the plane flies to the right (the quicker the user presses, the faster the plane flies)
* If the user presses "d" less than 4 times per second, the plane is flying back to the left.

---

Awesome. Can you slow down the move to the right a little bit so the game gets harder?

---

Amazing. Can you add a second plane (plane 2)? This will be a second player. He moves the plane with the l key.

---

I do not like that index.ts is so large now. Any idea how to encapsulate the game logic better?

---

Can you add unit tests for getFrames and drawFrame to @spritesheet.spec.ts ? Mock loadImage, fetch, and CanvasRenderingContext2D