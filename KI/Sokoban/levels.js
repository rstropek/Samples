// Get original Sokoban level from e.g. https://www.mathsisfun.com/games/sokoban.html
// Get Junior levels from http://borgar.net/programs/sokoban/#Sokoban%20Jr.%201

// _....... Nothing
// X....... Wall
// Blank... Floor
// @....... Player
// Dot..... Target
// b....... Box not on target
// B....... Box on target

export const levels = [
  [
    "XXXXX__",
    "X   X__",
    "X@XbXXX",
    "X b ..X",
    "XXXXXXX"
  ],
  [
    "XXXXXXX",
    "X.   .X",
    "X  b  X",
    "X b@b X",
    "X  b  X",
    "X.   .X",
    "XXXXXXX",
  ],
  [
    "XXXXXXX",
    "X.   .X",
    "X.bbb.X",
    "XXb@bXX",
    "X.bbb.X",
    "X.   .X",
    "XXXXXXX",
  ],
  [
    "__XXXXX_",
    "XXX   X_",
    "X.@b  X_",
    "XXX b.X_",
    "X.XXb X_",
    "X X . XX",
    "Xb Bbb.X",
    "X   .  X",
    "XXXXXXXX"
  ],
  [
    "____XXXXX_____________",
    "____X   X_____________",
    "____Xb  X_____________",
    "__XXX  bXXX___________",
    "__X  b  b X___________",
    "XXX X XXX X     XXXXXX",
    "X   X XXX XXXXXXX  ..X",
    "X b  b             ..X",
    "XXXXX XXXX X@XXXX  ..X",
    "____X      XXX__XXXXXX",
    "____XXXXXXXX__________"
  ]
];

export function getNumberOfLevels() {
  return levels.length;
}
