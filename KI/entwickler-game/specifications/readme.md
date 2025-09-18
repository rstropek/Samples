# Casual Game - Make Devs Happy

## Introduction

This is a casual browser game. The idea is that the user can control the movements of a sprite depicting a developer with ASDW. Conference and magazine logos are falling down from above. If the sprite collides with a logo, the player gets points. The more points the player collects, the happier the developer will be (we have different sprites for different levels of happiness).

## Playing Area

Assumption: The screen is in landscape format. On the left, we see a rectangular game area in which the game happens. On the right, we see the current points and the happiness status of the developer. Additionally, we see comments about what is currently happening:

## Point Logic

For each touched logo, the player gets one point. If the user does not touch a logo for >= 10 seconds, one point every 5 seconds is reduced until the player touches a logo again. The points cannot go below zero.

Happiness states:

* 0..3 points: /src/images/developer/frustrated.png
* 4..7 points: /src/images/developer/happy.png
* >= 8 points: /src/images/developer/crown.png

## Logos

The logos are in SVG format in the folder `/src/images/logos`. The logos should be randomly selected and fall down from the top of the playing area. The speed of the falling logos should increase with the points collected by the player.

The logos contain paths and rectangles. Use CSS to set the fill color of those elements.

The size of the logo should be random, not too large and not too small.

## Collision detection

It is fine to have a rough collision detection using rectangles.

## Technology

Do not use a Canvas. Use HTML elements and CSS for the rendering.