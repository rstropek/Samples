using System.Globalization;
using System.Windows.Input;
using SkiaSharp;

#region JSON
// Note raw string literals.
// See also https://slides.com/rainerstropek/csharp-11/fullscreen#/6/1

// Also note UTF8 string literals.
// See also https://slides.com/rainerstropek/csharp-11/fullscreen#/8/1
ReadOnlySpan<byte> framesJson = """
    {
        "frames": [
            {
                "filename": "Arctic (10).png",
                "frame": {
                    "x": 0,
                    "y": 0,
                    "w": 264,
                    "h": 335
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 264,
                    "h": 335
                },
                "sourceSize": {
                    "w": 264,
                    "h": 335
                }
            },
            {
                "filename": "Arctic (11).png",
                "frame": {
                    "x": 264,
                    "y": 0,
                    "w": 202,
                    "h": 344
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 202,
                    "h": 344
                },
                "sourceSize": {
                    "w": 202,
                    "h": 344
                }
            },
            {
                "filename": "Arctic (12).png",
                "frame": {
                    "x": 466,
                    "y": 0,
                    "w": 213,
                    "h": 362
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 213,
                    "h": 362
                },
                "sourceSize": {
                    "w": 213,
                    "h": 362
                }
            },
            {
                "filename": "Arctic (13).png",
                "frame": {
                    "x": 679,
                    "y": 0,
                    "w": 189,
                    "h": 318
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 318
                },
                "sourceSize": {
                    "w": 189,
                    "h": 318
                }
            },
            {
                "filename": "Arctic (14).png",
                "frame": {
                    "x": 868,
                    "y": 0,
                    "w": 436,
                    "h": 286
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 436,
                    "h": 286
                },
                "sourceSize": {
                    "w": 436,
                    "h": 286
                }
            },
            {
                "filename": "Arctic (15).png",
                "frame": {
                    "x": 1304,
                    "y": 0,
                    "w": 418,
                    "h": 287
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 418,
                    "h": 287
                },
                "sourceSize": {
                    "w": 418,
                    "h": 287
                }
            },
            {
                "filename": "Arctic (16).png",
                "frame": {
                    "x": 1722,
                    "y": 0,
                    "w": 396,
                    "h": 269
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 396,
                    "h": 269
                },
                "sourceSize": {
                    "w": 396,
                    "h": 269
                }
            },
            {
                "filename": "Arctic (17).png",
                "frame": {
                    "x": 2118,
                    "y": 0,
                    "w": 135,
                    "h": 97
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 135,
                    "h": 97
                },
                "sourceSize": {
                    "w": 135,
                    "h": 97
                }
            },
            {
                "filename": "Arctic (18).png",
                "frame": {
                    "x": 2253,
                    "y": 0,
                    "w": 199,
                    "h": 151
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 199,
                    "h": 151
                },
                "sourceSize": {
                    "w": 199,
                    "h": 151
                }
            },
            {
                "filename": "Arctic (19).png",
                "frame": {
                    "x": 2452,
                    "y": 0,
                    "w": 199,
                    "h": 150
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 199,
                    "h": 150
                },
                "sourceSize": {
                    "w": 199,
                    "h": 150
                }
            },
            {
                "filename": "Arctic (1).png",
                "frame": {
                    "x": 2452,
                    "y": 150,
                    "w": 198,
                    "h": 114
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 198,
                    "h": 114
                },
                "sourceSize": {
                    "w": 198,
                    "h": 114
                }
            },
            {
                "filename": "Arctic (20).png",
                "frame": {
                    "x": 2118,
                    "y": 264,
                    "w": 199,
                    "h": 151
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 199,
                    "h": 151
                },
                "sourceSize": {
                    "w": 199,
                    "h": 151
                }
            },
            {
                "filename": "Arctic (21).png",
                "frame": {
                    "x": 2317,
                    "y": 264,
                    "w": 199,
                    "h": 151
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 199,
                    "h": 151
                },
                "sourceSize": {
                    "w": 199,
                    "h": 151
                }
            },
            {
                "filename": "Arctic (22).png",
                "frame": {
                    "x": 0,
                    "y": 415,
                    "w": 198,
                    "h": 114
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 198,
                    "h": 114
                },
                "sourceSize": {
                    "w": 198,
                    "h": 114
                }
            },
            {
                "filename": "Arctic (23).png",
                "frame": {
                    "x": 198,
                    "y": 415,
                    "w": 199,
                    "h": 159
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 199,
                    "h": 159
                },
                "sourceSize": {
                    "w": 199,
                    "h": 159
                }
            },
            {
                "filename": "Arctic (24).png",
                "frame": {
                    "x": 397,
                    "y": 415,
                    "w": 199,
                    "h": 157
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 199,
                    "h": 157
                },
                "sourceSize": {
                    "w": 199,
                    "h": 157
                }
            },
            {
                "filename": "Arctic (25).png",
                "frame": {
                    "x": 596,
                    "y": 415,
                    "w": 199,
                    "h": 114
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 199,
                    "h": 114
                },
                "sourceSize": {
                    "w": 199,
                    "h": 114
                }
            },
            {
                "filename": "Arctic (2).png",
                "frame": {
                    "x": 795,
                    "y": 415,
                    "w": 198,
                    "h": 159
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 198,
                    "h": 159
                },
                "sourceSize": {
                    "w": 198,
                    "h": 159
                }
            },
            {
                "filename": "Arctic (3).png",
                "frame": {
                    "x": 993,
                    "y": 415,
                    "w": 198,
                    "h": 160
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 198,
                    "h": 160
                },
                "sourceSize": {
                    "w": 198,
                    "h": 160
                }
            },
            {
                "filename": "Arctic (4).png",
                "frame": {
                    "x": 1191,
                    "y": 415,
                    "w": 195,
                    "h": 145
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 195,
                    "h": 145
                },
                "sourceSize": {
                    "w": 195,
                    "h": 145
                }
            },
            {
                "filename": "Arctic (5).png",
                "frame": {
                    "x": 1386,
                    "y": 415,
                    "w": 195,
                    "h": 145
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 195,
                    "h": 145
                },
                "sourceSize": {
                    "w": 195,
                    "h": 145
                }
            },
            {
                "filename": "Arctic (6).png",
                "frame": {
                    "x": 2118,
                    "y": 150,
                    "w": 111,
                    "h": 86
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 111,
                    "h": 86
                },
                "sourceSize": {
                    "w": 111,
                    "h": 86
                }
            },
            {
                "filename": "Arctic (7).png",
                "frame": {
                    "x": 2516,
                    "y": 264,
                    "w": 164,
                    "h": 114
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 164,
                    "h": 114
                },
                "sourceSize": {
                    "w": 164,
                    "h": 114
                }
            },
            {
                "filename": "Arctic (8).png",
                "frame": {
                    "x": 1581,
                    "y": 378,
                    "w": 164,
                    "h": 114
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 164,
                    "h": 114
                },
                "sourceSize": {
                    "w": 164,
                    "h": 114
                }
            },
            {
                "filename": "Arctic (9).png",
                "frame": {
                    "x": 1745,
                    "y": 378,
                    "w": 321,
                    "h": 356
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 321,
                    "h": 356
                },
                "sourceSize": {
                    "w": 321,
                    "h": 356
                }
            },
            {
                "filename": "Chip (1).png",
                "frame": {
                    "x": 2516,
                    "y": 378,
                    "w": 136,
                    "h": 97
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 136,
                    "h": 97
                },
                "sourceSize": {
                    "w": 136,
                    "h": 97
                }
            },
            {
                "filename": "Chip (2).png",
                "frame": {
                    "x": 2066,
                    "y": 475,
                    "w": 136,
                    "h": 97
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 136,
                    "h": 97
                },
                "sourceSize": {
                    "w": 136,
                    "h": 97
                }
            },
            {
                "filename": "Chip (3).png",
                "frame": {
                    "x": 2202,
                    "y": 475,
                    "w": 136,
                    "h": 97
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 136,
                    "h": 97
                },
                "sourceSize": {
                    "w": 136,
                    "h": 97
                }
            },
            {
                "filename": "Chip (4).png",
                "frame": {
                    "x": 2338,
                    "y": 475,
                    "w": 136,
                    "h": 97
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 136,
                    "h": 97
                },
                "sourceSize": {
                    "w": 136,
                    "h": 97
                }
            },
            {
                "filename": "Chip (5).png",
                "frame": {
                    "x": 2474,
                    "y": 475,
                    "w": 136,
                    "h": 97
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 136,
                    "h": 97
                },
                "sourceSize": {
                    "w": 136,
                    "h": 97
                }
            },
            {
                "filename": "Chip (6).png",
                "frame": {
                    "x": 0,
                    "y": 572,
                    "w": 136,
                    "h": 97
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 136,
                    "h": 97
                },
                "sourceSize": {
                    "w": 136,
                    "h": 97
                }
            },
            {
                "filename": "Chip (7).png",
                "frame": {
                    "x": 397,
                    "y": 572,
                    "w": 136,
                    "h": 97
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 136,
                    "h": 97
                },
                "sourceSize": {
                    "w": 136,
                    "h": 97
                }
            },
            {
                "filename": "Ghost (10).png",
                "frame": {
                    "x": 2610,
                    "y": 475,
                    "w": 65,
                    "h": 168
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 65,
                    "h": 168
                },
                "sourceSize": {
                    "w": 65,
                    "h": 168
                }
            },
            {
                "filename": "Ghost (11).png",
                "frame": {
                    "x": 533,
                    "y": 572,
                    "w": 89,
                    "h": 99
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 89,
                    "h": 99
                },
                "sourceSize": {
                    "w": 89,
                    "h": 99
                }
            },
            {
                "filename": "Ghost (12).png",
                "frame": {
                    "x": 622,
                    "y": 572,
                    "w": 89,
                    "h": 110
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 89,
                    "h": 110
                },
                "sourceSize": {
                    "w": 89,
                    "h": 110
                }
            },
            {
                "filename": "Ghost (13).png",
                "frame": {
                    "x": 1191,
                    "y": 572,
                    "w": 89,
                    "h": 99
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 89,
                    "h": 99
                },
                "sourceSize": {
                    "w": 89,
                    "h": 99
                }
            },
            {
                "filename": "Ghost (14).png",
                "frame": {
                    "x": 1280,
                    "y": 572,
                    "w": 124,
                    "h": 207
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 124,
                    "h": 207
                },
                "sourceSize": {
                    "w": 124,
                    "h": 207
                }
            },
            {
                "filename": "Ghost (15).png",
                "frame": {
                    "x": 1404,
                    "y": 572,
                    "w": 148,
                    "h": 150
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 148,
                    "h": 150
                },
                "sourceSize": {
                    "w": 148,
                    "h": 150
                }
            },
            {
                "filename": "Ghost (16).png",
                "frame": {
                    "x": 1552,
                    "y": 572,
                    "w": 101,
                    "h": 177
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 101,
                    "h": 177
                },
                "sourceSize": {
                    "w": 101,
                    "h": 177
                }
            },
            {
                "filename": "Ghost (17).png",
                "frame": {
                    "x": 2066,
                    "y": 572,
                    "w": 138,
                    "h": 251
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 138,
                    "h": 251
                },
                "sourceSize": {
                    "w": 138,
                    "h": 251
                }
            },
            {
                "filename": "Ghost (18).png",
                "frame": {
                    "x": 136,
                    "y": 643,
                    "w": 188,
                    "h": 329
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 329
                },
                "sourceSize": {
                    "w": 188,
                    "h": 329
                }
            },
            {
                "filename": "Ghost (19).png",
                "frame": {
                    "x": 2066,
                    "y": 378,
                    "w": 42,
                    "h": 69
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 42,
                    "h": 69
                },
                "sourceSize": {
                    "w": 42,
                    "h": 69
                }
            },
            {
                "filename": "Ghost (1).png",
                "frame": {
                    "x": 2204,
                    "y": 572,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (20).png",
                "frame": {
                    "x": 324,
                    "y": 643,
                    "w": 55,
                    "h": 96
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 55,
                    "h": 96
                },
                "sourceSize": {
                    "w": 55,
                    "h": 96
                }
            },
            {
                "filename": "Ghost (21).png",
                "frame": {
                    "x": 711,
                    "y": 572,
                    "w": 77,
                    "h": 68
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 77,
                    "h": 68
                },
                "sourceSize": {
                    "w": 77,
                    "h": 68
                }
            },
            {
                "filename": "Ghost (22).png",
                "frame": {
                    "x": 2393,
                    "y": 572,
                    "w": 193,
                    "h": 368
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 193,
                    "h": 368
                },
                "sourceSize": {
                    "w": 193,
                    "h": 368
                }
            },
            {
                "filename": "Ghost (23).png",
                "frame": {
                    "x": 324,
                    "y": 940,
                    "w": 339,
                    "h": 242
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 339,
                    "h": 242
                },
                "sourceSize": {
                    "w": 339,
                    "h": 242
                }
            },
            {
                "filename": "Ghost (24).png",
                "frame": {
                    "x": 711,
                    "y": 643,
                    "w": 87,
                    "h": 408
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 87,
                    "h": 408
                },
                "sourceSize": {
                    "w": 87,
                    "h": 408
                }
            },
            {
                "filename": "Ghost (25).png",
                "frame": {
                    "x": 798,
                    "y": 940,
                    "w": 270,
                    "h": 242
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 270,
                    "h": 242
                },
                "sourceSize": {
                    "w": 270,
                    "h": 242
                }
            },
            {
                "filename": "Ghost (26).png",
                "frame": {
                    "x": 1068,
                    "y": 940,
                    "w": 270,
                    "h": 243
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 270,
                    "h": 243
                },
                "sourceSize": {
                    "w": 270,
                    "h": 243
                }
            },
            {
                "filename": "Ghost (27).png",
                "frame": {
                    "x": 1338,
                    "y": 940,
                    "w": 270,
                    "h": 201
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 270,
                    "h": 201
                },
                "sourceSize": {
                    "w": 270,
                    "h": 201
                }
            },
            {
                "filename": "Ghost (28).png",
                "frame": {
                    "x": 1608,
                    "y": 940,
                    "w": 269,
                    "h": 201
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 269,
                    "h": 201
                },
                "sourceSize": {
                    "w": 269,
                    "h": 201
                }
            },
            {
                "filename": "Ghost (29).png",
                "frame": {
                    "x": 1877,
                    "y": 940,
                    "w": 270,
                    "h": 242
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 270,
                    "h": 242
                },
                "sourceSize": {
                    "w": 270,
                    "h": 242
                }
            },
            {
                "filename": "Ghost (2).png",
                "frame": {
                    "x": 798,
                    "y": 643,
                    "w": 94,
                    "h": 113
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 94,
                    "h": 113
                },
                "sourceSize": {
                    "w": 94,
                    "h": 113
                }
            },
            {
                "filename": "Ghost (30).png",
                "frame": {
                    "x": 2147,
                    "y": 940,
                    "w": 270,
                    "h": 243
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 270,
                    "h": 243
                },
                "sourceSize": {
                    "w": 270,
                    "h": 243
                }
            },
            {
                "filename": "Ghost (31).png",
                "frame": {
                    "x": 0,
                    "y": 1183,
                    "w": 270,
                    "h": 201
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 270,
                    "h": 201
                },
                "sourceSize": {
                    "w": 270,
                    "h": 201
                }
            },
            {
                "filename": "Ghost (32).png",
                "frame": {
                    "x": 270,
                    "y": 1183,
                    "w": 270,
                    "h": 201
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 270,
                    "h": 201
                },
                "sourceSize": {
                    "w": 270,
                    "h": 201
                }
            },
            {
                "filename": "Ghost (33).png",
                "frame": {
                    "x": 2417,
                    "y": 940,
                    "w": 269,
                    "h": 201
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 269,
                    "h": 201
                },
                "sourceSize": {
                    "w": 269,
                    "h": 201
                }
            },
            {
                "filename": "Ghost (34).png",
                "frame": {
                    "x": 1338,
                    "y": 1141,
                    "w": 269,
                    "h": 201
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 269,
                    "h": 201
                },
                "sourceSize": {
                    "w": 269,
                    "h": 201
                }
            },
            {
                "filename": "Ghost (35).png",
                "frame": {
                    "x": 540,
                    "y": 1183,
                    "w": 270,
                    "h": 201
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 270,
                    "h": 201
                },
                "sourceSize": {
                    "w": 270,
                    "h": 201
                }
            },
            {
                "filename": "Ghost (36).png",
                "frame": {
                    "x": 810,
                    "y": 1183,
                    "w": 270,
                    "h": 201
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 270,
                    "h": 201
                },
                "sourceSize": {
                    "w": 270,
                    "h": 201
                }
            },
            {
                "filename": "Ghost (37).png",
                "frame": {
                    "x": 1607,
                    "y": 1183,
                    "w": 284,
                    "h": 178
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 284,
                    "h": 178
                },
                "sourceSize": {
                    "w": 284,
                    "h": 178
                }
            },
            {
                "filename": "Ghost (38).png",
                "frame": {
                    "x": 2417,
                    "y": 1141,
                    "w": 259,
                    "h": 164
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 259,
                    "h": 164
                },
                "sourceSize": {
                    "w": 259,
                    "h": 164
                }
            },
            {
                "filename": "Ghost (39).png",
                "frame": {
                    "x": 1080,
                    "y": 1305,
                    "w": 188,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 141
                },
                "sourceSize": {
                    "w": 188,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (3).png",
                "frame": {
                    "x": 892,
                    "y": 643,
                    "w": 87,
                    "h": 165
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 87,
                    "h": 165
                },
                "sourceSize": {
                    "w": 87,
                    "h": 165
                }
            },
            {
                "filename": "Ghost (40).png",
                "frame": {
                    "x": 1891,
                    "y": 1305,
                    "w": 188,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 141
                },
                "sourceSize": {
                    "w": 188,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (41).png",
                "frame": {
                    "x": 2079,
                    "y": 1305,
                    "w": 188,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 141
                },
                "sourceSize": {
                    "w": 188,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (42).png",
                "frame": {
                    "x": 2267,
                    "y": 1305,
                    "w": 188,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 141
                },
                "sourceSize": {
                    "w": 188,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (43).png",
                "frame": {
                    "x": 2455,
                    "y": 1305,
                    "w": 188,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 141
                },
                "sourceSize": {
                    "w": 188,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (44).png",
                "frame": {
                    "x": 0,
                    "y": 1446,
                    "w": 188,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 141
                },
                "sourceSize": {
                    "w": 188,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (45).png",
                "frame": {
                    "x": 188,
                    "y": 1446,
                    "w": 188,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 141
                },
                "sourceSize": {
                    "w": 188,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (46).png",
                "frame": {
                    "x": 376,
                    "y": 1446,
                    "w": 188,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 141
                },
                "sourceSize": {
                    "w": 188,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (47).png",
                "frame": {
                    "x": 564,
                    "y": 1446,
                    "w": 188,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 141
                },
                "sourceSize": {
                    "w": 188,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (48).png",
                "frame": {
                    "x": 752,
                    "y": 1446,
                    "w": 188,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 141
                },
                "sourceSize": {
                    "w": 188,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (49).png",
                "frame": {
                    "x": 940,
                    "y": 1446,
                    "w": 188,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 141
                },
                "sourceSize": {
                    "w": 188,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (4).png",
                "frame": {
                    "x": 979,
                    "y": 643,
                    "w": 87,
                    "h": 165
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 87,
                    "h": 165
                },
                "sourceSize": {
                    "w": 87,
                    "h": 165
                }
            },
            {
                "filename": "Ghost (50).png",
                "frame": {
                    "x": 1128,
                    "y": 1446,
                    "w": 188,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 141
                },
                "sourceSize": {
                    "w": 188,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (51).png",
                "frame": {
                    "x": 1316,
                    "y": 1446,
                    "w": 188,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 141
                },
                "sourceSize": {
                    "w": 188,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (52).png",
                "frame": {
                    "x": 1504,
                    "y": 1446,
                    "w": 188,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 141
                },
                "sourceSize": {
                    "w": 188,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (53).png",
                "frame": {
                    "x": 1692,
                    "y": 1446,
                    "w": 188,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 141
                },
                "sourceSize": {
                    "w": 188,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (54).png",
                "frame": {
                    "x": 1880,
                    "y": 1446,
                    "w": 188,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 141
                },
                "sourceSize": {
                    "w": 188,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (55).png",
                "frame": {
                    "x": 2068,
                    "y": 1446,
                    "w": 188,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 141
                },
                "sourceSize": {
                    "w": 188,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (56).png",
                "frame": {
                    "x": 2256,
                    "y": 1446,
                    "w": 188,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 141
                },
                "sourceSize": {
                    "w": 188,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (57).png",
                "frame": {
                    "x": 2444,
                    "y": 1446,
                    "w": 188,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 141
                },
                "sourceSize": {
                    "w": 188,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (58).png",
                "frame": {
                    "x": 0,
                    "y": 1587,
                    "w": 188,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 141
                },
                "sourceSize": {
                    "w": 188,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (59).png",
                "frame": {
                    "x": 188,
                    "y": 1587,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (5).png",
                "frame": {
                    "x": 1066,
                    "y": 643,
                    "w": 87,
                    "h": 165
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 87,
                    "h": 165
                },
                "sourceSize": {
                    "w": 87,
                    "h": 165
                }
            },
            {
                "filename": "Ghost (60).png",
                "frame": {
                    "x": 377,
                    "y": 1587,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (61).png",
                "frame": {
                    "x": 566,
                    "y": 1587,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (62).png",
                "frame": {
                    "x": 755,
                    "y": 1587,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (63).png",
                "frame": {
                    "x": 944,
                    "y": 1587,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (64).png",
                "frame": {
                    "x": 1133,
                    "y": 1587,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (65).png",
                "frame": {
                    "x": 1322,
                    "y": 1587,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (66).png",
                "frame": {
                    "x": 1511,
                    "y": 1587,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (67).png",
                "frame": {
                    "x": 1700,
                    "y": 1587,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Ghost (6).png",
                "frame": {
                    "x": 1653,
                    "y": 643,
                    "w": 87,
                    "h": 166
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 87,
                    "h": 166
                },
                "sourceSize": {
                    "w": 87,
                    "h": 166
                }
            },
            {
                "filename": "Ghost (7).png",
                "frame": {
                    "x": 1889,
                    "y": 1587,
                    "w": 153,
                    "h": 138
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 153,
                    "h": 138
                },
                "sourceSize": {
                    "w": 153,
                    "h": 138
                }
            },
            {
                "filename": "Ghost (8).png",
                "frame": {
                    "x": 2042,
                    "y": 1587,
                    "w": 160,
                    "h": 129
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 160,
                    "h": 129
                },
                "sourceSize": {
                    "w": 160,
                    "h": 129
                }
            },
            {
                "filename": "Ghost (9).png",
                "frame": {
                    "x": 0,
                    "y": 940,
                    "w": 126,
                    "h": 176
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 126,
                    "h": 176
                },
                "sourceSize": {
                    "w": 126,
                    "h": 176
                }
            },
            {
                "filename": "Snow (10).png",
                "frame": {
                    "x": 2202,
                    "y": 1587,
                    "w": 138,
                    "h": 107
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 138,
                    "h": 107
                },
                "sourceSize": {
                    "w": 138,
                    "h": 107
                }
            },
            {
                "filename": "Snow (11).png",
                "frame": {
                    "x": 2586,
                    "y": 643,
                    "w": 99,
                    "h": 79
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 99,
                    "h": 79
                },
                "sourceSize": {
                    "w": 99,
                    "h": 79
                }
            },
            {
                "filename": "Snow (12).png",
                "frame": {
                    "x": 0,
                    "y": 722,
                    "w": 65,
                    "h": 53
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 65,
                    "h": 53
                },
                "sourceSize": {
                    "w": 65,
                    "h": 53
                }
            },
            {
                "filename": "Snow (13).png",
                "frame": {
                    "x": 379,
                    "y": 722,
                    "w": 76,
                    "h": 68
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 76,
                    "h": 68
                },
                "sourceSize": {
                    "w": 76,
                    "h": 68
                }
            },
            {
                "filename": "Snow (14).png",
                "frame": {
                    "x": 65,
                    "y": 722,
                    "w": 58,
                    "h": 58
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 58,
                    "h": 58
                },
                "sourceSize": {
                    "w": 58,
                    "h": 58
                }
            },
            {
                "filename": "Snow (15).png",
                "frame": {
                    "x": 455,
                    "y": 722,
                    "w": 43,
                    "h": 88
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 43,
                    "h": 88
                },
                "sourceSize": {
                    "w": 43,
                    "h": 88
                }
            },
            {
                "filename": "Snow (16).png",
                "frame": {
                    "x": 0,
                    "y": 378,
                    "w": 80,
                    "h": 35
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 80,
                    "h": 35
                },
                "sourceSize": {
                    "w": 80,
                    "h": 35
                }
            },
            {
                "filename": "Snow (17).png",
                "frame": {
                    "x": 498,
                    "y": 722,
                    "w": 62,
                    "h": 71
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 62,
                    "h": 71
                },
                "sourceSize": {
                    "w": 62,
                    "h": 71
                }
            },
            {
                "filename": "Snow (18).png",
                "frame": {
                    "x": 560,
                    "y": 722,
                    "w": 63,
                    "h": 71
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 63,
                    "h": 71
                },
                "sourceSize": {
                    "w": 63,
                    "h": 71
                }
            },
            {
                "filename": "Snow (19).png",
                "frame": {
                    "x": 2340,
                    "y": 1587,
                    "w": 168,
                    "h": 90
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 168,
                    "h": 90
                },
                "sourceSize": {
                    "w": 168,
                    "h": 90
                }
            },
            {
                "filename": "Snow (1).png",
                "frame": {
                    "x": 2340,
                    "y": 1677,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Snow (20).png",
                "frame": {
                    "x": 623,
                    "y": 722,
                    "w": 42,
                    "h": 70
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 42,
                    "h": 70
                },
                "sourceSize": {
                    "w": 42,
                    "h": 70
                }
            },
            {
                "filename": "Snow (21).png",
                "frame": {
                    "x": 1153,
                    "y": 722,
                    "w": 54,
                    "h": 96
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 54,
                    "h": 96
                },
                "sourceSize": {
                    "w": 54,
                    "h": 96
                }
            },
            {
                "filename": "Snow (22).png",
                "frame": {
                    "x": 0,
                    "y": 1818,
                    "w": 205,
                    "h": 153
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 205,
                    "h": 153
                },
                "sourceSize": {
                    "w": 205,
                    "h": 153
                }
            },
            {
                "filename": "Snow (23).png",
                "frame": {
                    "x": 205,
                    "y": 1818,
                    "w": 236,
                    "h": 411
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 236,
                    "h": 411
                },
                "sourceSize": {
                    "w": 236,
                    "h": 411
                }
            },
            {
                "filename": "Snow (24).png",
                "frame": {
                    "x": 441,
                    "y": 1818,
                    "w": 226,
                    "h": 463
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 226,
                    "h": 463
                },
                "sourceSize": {
                    "w": 226,
                    "h": 463
                }
            },
            {
                "filename": "Snow (25).png",
                "frame": {
                    "x": 667,
                    "y": 1818,
                    "w": 270,
                    "h": 243
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 270,
                    "h": 243
                },
                "sourceSize": {
                    "w": 270,
                    "h": 243
                }
            },
            {
                "filename": "Snow (26).png",
                "frame": {
                    "x": 937,
                    "y": 1818,
                    "w": 270,
                    "h": 242
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 270,
                    "h": 242
                },
                "sourceSize": {
                    "w": 270,
                    "h": 242
                }
            },
            {
                "filename": "Snow (27).png",
                "frame": {
                    "x": 1207,
                    "y": 1818,
                    "w": 270,
                    "h": 202
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 270,
                    "h": 202
                },
                "sourceSize": {
                    "w": 270,
                    "h": 202
                }
            },
            {
                "filename": "Snow (28).png",
                "frame": {
                    "x": 1477,
                    "y": 1818,
                    "w": 270,
                    "h": 201
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 270,
                    "h": 201
                },
                "sourceSize": {
                    "w": 270,
                    "h": 201
                }
            },
            {
                "filename": "Snow (29).png",
                "frame": {
                    "x": 1747,
                    "y": 1818,
                    "w": 270,
                    "h": 202
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 270,
                    "h": 202
                },
                "sourceSize": {
                    "w": 270,
                    "h": 202
                }
            },
            {
                "filename": "Snow (2).png",
                "frame": {
                    "x": 1404,
                    "y": 722,
                    "w": 87,
                    "h": 161
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 87,
                    "h": 161
                },
                "sourceSize": {
                    "w": 87,
                    "h": 161
                }
            },
            {
                "filename": "Snow (30).png",
                "frame": {
                    "x": 2017,
                    "y": 1818,
                    "w": 270,
                    "h": 243
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 270,
                    "h": 243
                },
                "sourceSize": {
                    "w": 270,
                    "h": 243
                }
            },
            {
                "filename": "Snow (31).png",
                "frame": {
                    "x": 2287,
                    "y": 1818,
                    "w": 270,
                    "h": 242
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 270,
                    "h": 242
                },
                "sourceSize": {
                    "w": 270,
                    "h": 242
                }
            },
            {
                "filename": "Snow (32).png",
                "frame": {
                    "x": 937,
                    "y": 2060,
                    "w": 270,
                    "h": 202
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 270,
                    "h": 202
                },
                "sourceSize": {
                    "w": 270,
                    "h": 202
                }
            },
            {
                "filename": "Snow (33).png",
                "frame": {
                    "x": 1207,
                    "y": 2060,
                    "w": 270,
                    "h": 201
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 270,
                    "h": 201
                },
                "sourceSize": {
                    "w": 270,
                    "h": 201
                }
            },
            {
                "filename": "Snow (34).png",
                "frame": {
                    "x": 1477,
                    "y": 2060,
                    "w": 270,
                    "h": 203
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 270,
                    "h": 203
                },
                "sourceSize": {
                    "w": 270,
                    "h": 203
                }
            },
            {
                "filename": "Snow (35).png",
                "frame": {
                    "x": 1747,
                    "y": 2060,
                    "w": 270,
                    "h": 202
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 270,
                    "h": 202
                },
                "sourceSize": {
                    "w": 270,
                    "h": 202
                }
            },
            {
                "filename": "Snow (36).png",
                "frame": {
                    "x": 2287,
                    "y": 2060,
                    "w": 269,
                    "h": 202
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 269,
                    "h": 202
                },
                "sourceSize": {
                    "w": 269,
                    "h": 202
                }
            },
            {
                "filename": "Snow (37).png",
                "frame": {
                    "x": 0,
                    "y": 2262,
                    "w": 258,
                    "h": 164
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 258,
                    "h": 164
                },
                "sourceSize": {
                    "w": 258,
                    "h": 164
                }
            },
            {
                "filename": "Snow (38).png",
                "frame": {
                    "x": 667,
                    "y": 2262,
                    "w": 325,
                    "h": 315
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 325,
                    "h": 315
                },
                "sourceSize": {
                    "w": 325,
                    "h": 315
                }
            },
            {
                "filename": "Snow (39).png",
                "frame": {
                    "x": 0,
                    "y": 2060,
                    "w": 188,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 141
                },
                "sourceSize": {
                    "w": 188,
                    "h": 141
                }
            },
            {
                "filename": "Snow (3).png",
                "frame": {
                    "x": 2204,
                    "y": 722,
                    "w": 87,
                    "h": 161
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 87,
                    "h": 161
                },
                "sourceSize": {
                    "w": 87,
                    "h": 161
                }
            },
            {
                "filename": "Snow (40).png",
                "frame": {
                    "x": 992,
                    "y": 2262,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Snow (41).png",
                "frame": {
                    "x": 1181,
                    "y": 2262,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Snow (42).png",
                "frame": {
                    "x": 1747,
                    "y": 2262,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Snow (43).png",
                "frame": {
                    "x": 1936,
                    "y": 2262,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Snow (44).png",
                "frame": {
                    "x": 2125,
                    "y": 2262,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Snow (45).png",
                "frame": {
                    "x": 2314,
                    "y": 2262,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Snow (46).png",
                "frame": {
                    "x": 258,
                    "y": 2403,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Snow (47).png",
                "frame": {
                    "x": 447,
                    "y": 2403,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Snow (48).png",
                "frame": {
                    "x": 992,
                    "y": 2403,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Snow (49).png",
                "frame": {
                    "x": 1181,
                    "y": 2403,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Snow (4).png",
                "frame": {
                    "x": 2291,
                    "y": 722,
                    "w": 87,
                    "h": 161
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 87,
                    "h": 161
                },
                "sourceSize": {
                    "w": 87,
                    "h": 161
                }
            },
            {
                "filename": "Snow (50).png",
                "frame": {
                    "x": 1370,
                    "y": 2403,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Snow (51).png",
                "frame": {
                    "x": 1559,
                    "y": 2403,
                    "w": 188,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 141
                },
                "sourceSize": {
                    "w": 188,
                    "h": 141
                }
            },
            {
                "filename": "Snow (52).png",
                "frame": {
                    "x": 1747,
                    "y": 2403,
                    "w": 188,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 188,
                    "h": 141
                },
                "sourceSize": {
                    "w": 188,
                    "h": 141
                }
            },
            {
                "filename": "Snow (53).png",
                "frame": {
                    "x": 1935,
                    "y": 2403,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Snow (54).png",
                "frame": {
                    "x": 2124,
                    "y": 2403,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Snow (55).png",
                "frame": {
                    "x": 2313,
                    "y": 2403,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Snow (56).png",
                "frame": {
                    "x": 0,
                    "y": 2544,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Snow (57).png",
                "frame": {
                    "x": 189,
                    "y": 2544,
                    "w": 189,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 189,
                    "h": 141
                },
                "sourceSize": {
                    "w": 189,
                    "h": 141
                }
            },
            {
                "filename": "Snow (5).png",
                "frame": {
                    "x": 2586,
                    "y": 722,
                    "w": 87,
                    "h": 162
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 87,
                    "h": 162
                },
                "sourceSize": {
                    "w": 87,
                    "h": 162
                }
            },
            {
                "filename": "Snow (6).png",
                "frame": {
                    "x": 258,
                    "y": 2262,
                    "w": 173,
                    "h": 138
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 173,
                    "h": 138
                },
                "sourceSize": {
                    "w": 173,
                    "h": 138
                }
            },
            {
                "filename": "Snow (7).png",
                "frame": {
                    "x": 2529,
                    "y": 1587,
                    "w": 125,
                    "h": 176
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 125,
                    "h": 176
                },
                "sourceSize": {
                    "w": 125,
                    "h": 176
                }
            },
            {
                "filename": "Snow (8).png",
                "frame": {
                    "x": 663,
                    "y": 884,
                    "w": 45,
                    "h": 158
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 45,
                    "h": 158
                },
                "sourceSize": {
                    "w": 45,
                    "h": 158
                }
            },
            {
                "filename": "Snow (9).png",
                "frame": {
                    "x": 2503,
                    "y": 2262,
                    "w": 161,
                    "h": 157
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 161,
                    "h": 157
                },
                "sourceSize": {
                    "w": 161,
                    "h": 157
                }
            },
            {
                "filename": "User01.png",
                "frame": {
                    "x": 2502,
                    "y": 2419,
                    "w": 158,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 158,
                    "h": 141
                },
                "sourceSize": {
                    "w": 158,
                    "h": 141
                }
            },
            {
                "filename": "User02.png",
                "frame": {
                    "x": 2651,
                    "y": 0,
                    "w": 157,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 157,
                    "h": 141
                },
                "sourceSize": {
                    "w": 157,
                    "h": 141
                }
            },
            {
                "filename": "User03.png",
                "frame": {
                    "x": 2680,
                    "y": 141,
                    "w": 157,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 157,
                    "h": 141
                },
                "sourceSize": {
                    "w": 157,
                    "h": 141
                }
            },
            {
                "filename": "User04.png",
                "frame": {
                    "x": 2680,
                    "y": 282,
                    "w": 158,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 158,
                    "h": 141
                },
                "sourceSize": {
                    "w": 158,
                    "h": 141
                }
            },
            {
                "filename": "User05.png",
                "frame": {
                    "x": 2675,
                    "y": 423,
                    "w": 157,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 157,
                    "h": 141
                },
                "sourceSize": {
                    "w": 157,
                    "h": 141
                }
            },
            {
                "filename": "User06.png",
                "frame": {
                    "x": 2685,
                    "y": 564,
                    "w": 157,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 157,
                    "h": 141
                },
                "sourceSize": {
                    "w": 157,
                    "h": 141
                }
            },
            {
                "filename": "User07.png",
                "frame": {
                    "x": 2685,
                    "y": 705,
                    "w": 158,
                    "h": 141
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 158,
                    "h": 141
                },
                "sourceSize": {
                    "w": 158,
                    "h": 141
                }
            },
            {
                "filename": "Winter (10).png",
                "frame": {
                    "x": 1722,
                    "y": 282,
                    "w": 55,
                    "h": 96
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 55,
                    "h": 96
                },
                "sourceSize": {
                    "w": 55,
                    "h": 96
                }
            },
            {
                "filename": "Winter (11).png",
                "frame": {
                    "x": 1777,
                    "y": 282,
                    "w": 152,
                    "h": 80
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 152,
                    "h": 80
                },
                "sourceSize": {
                    "w": 152,
                    "h": 80
                }
            },
            {
                "filename": "Winter (12).png",
                "frame": {
                    "x": 2686,
                    "y": 846,
                    "w": 87,
                    "h": 408
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 87,
                    "h": 408
                },
                "sourceSize": {
                    "w": 87,
                    "h": 408
                }
            },
            {
                "filename": "Winter (13).png",
                "frame": {
                    "x": 2676,
                    "y": 1254,
                    "w": 161,
                    "h": 158
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 161,
                    "h": 158
                },
                "sourceSize": {
                    "w": 161,
                    "h": 158
                }
            },
            {
                "filename": "Winter (14).png",
                "frame": {
                    "x": 2643,
                    "y": 1412,
                    "w": 139,
                    "h": 116
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 139,
                    "h": 116
                },
                "sourceSize": {
                    "w": 139,
                    "h": 116
                }
            },
            {
                "filename": "Winter (15).png",
                "frame": {
                    "x": 1929,
                    "y": 282,
                    "w": 77,
                    "h": 68
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 77,
                    "h": 68
                },
                "sourceSize": {
                    "w": 77,
                    "h": 68
                }
            },
            {
                "filename": "Winter (16).png",
                "frame": {
                    "x": 2654,
                    "y": 1528,
                    "w": 142,
                    "h": 171
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 142,
                    "h": 171
                },
                "sourceSize": {
                    "w": 142,
                    "h": 171
                }
            },
            {
                "filename": "Winter (17).png",
                "frame": {
                    "x": 2557,
                    "y": 1763,
                    "w": 195,
                    "h": 328
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 195,
                    "h": 328
                },
                "sourceSize": {
                    "w": 195,
                    "h": 328
                }
            },
            {
                "filename": "Winter (18).png",
                "frame": {
                    "x": 667,
                    "y": 2091,
                    "w": 234,
                    "h": 146
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 234,
                    "h": 146
                },
                "sourceSize": {
                    "w": 234,
                    "h": 146
                }
            },
            {
                "filename": "Winter (19).png",
                "frame": {
                    "x": 992,
                    "y": 2560,
                    "w": 362,
                    "h": 149
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 362,
                    "h": 149
                },
                "sourceSize": {
                    "w": 362,
                    "h": 149
                }
            },
            {
                "filename": "Winter (1).png",
                "frame": {
                    "x": 2017,
                    "y": 2091,
                    "w": 110,
                    "h": 127
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 110,
                    "h": 127
                },
                "sourceSize": {
                    "w": 110,
                    "h": 127
                }
            },
            {
                "filename": "Winter (20).png",
                "frame": {
                    "x": 2838,
                    "y": 0,
                    "w": 479,
                    "h": 377
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 479,
                    "h": 377
                },
                "sourceSize": {
                    "w": 479,
                    "h": 377
                }
            },
            {
                "filename": "Winter (21).png",
                "frame": {
                    "x": 2842,
                    "y": 377,
                    "w": 240,
                    "h": 190
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 240,
                    "h": 190
                },
                "sourceSize": {
                    "w": 240,
                    "h": 190
                }
            },
            {
                "filename": "Winter (22).png",
                "frame": {
                    "x": 3082,
                    "y": 377,
                    "w": 240,
                    "h": 193
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 240,
                    "h": 193
                },
                "sourceSize": {
                    "w": 240,
                    "h": 193
                }
            },
            {
                "filename": "Winter (23).png",
                "frame": {
                    "x": 2843,
                    "y": 570,
                    "w": 312,
                    "h": 247
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 312,
                    "h": 247
                },
                "sourceSize": {
                    "w": 312,
                    "h": 247
                }
            },
            {
                "filename": "Winter (24).png",
                "frame": {
                    "x": 2843,
                    "y": 817,
                    "w": 312,
                    "h": 251
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 312,
                    "h": 251
                },
                "sourceSize": {
                    "w": 312,
                    "h": 251
                }
            },
            {
                "filename": "Winter (25).png",
                "frame": {
                    "x": 2837,
                    "y": 1068,
                    "w": 313,
                    "h": 255
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 313,
                    "h": 255
                },
                "sourceSize": {
                    "w": 313,
                    "h": 255
                }
            },
            {
                "filename": "Winter (26).png",
                "frame": {
                    "x": 2837,
                    "y": 1323,
                    "w": 312,
                    "h": 247
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 312,
                    "h": 247
                },
                "sourceSize": {
                    "w": 312,
                    "h": 247
                }
            },
            {
                "filename": "Winter (27).png",
                "frame": {
                    "x": 2796,
                    "y": 1570,
                    "w": 312,
                    "h": 303
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 312,
                    "h": 303
                },
                "sourceSize": {
                    "w": 312,
                    "h": 303
                }
            },
            {
                "filename": "Winter (28).png",
                "frame": {
                    "x": 2752,
                    "y": 1873,
                    "w": 233,
                    "h": 169
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 233,
                    "h": 169
                },
                "sourceSize": {
                    "w": 233,
                    "h": 169
                }
            },
            {
                "filename": "Winter (29).png",
                "frame": {
                    "x": 2985,
                    "y": 1873,
                    "w": 233,
                    "h": 170
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 233,
                    "h": 170
                },
                "sourceSize": {
                    "w": 233,
                    "h": 170
                }
            },
            {
                "filename": "Winter (2).png",
                "frame": {
                    "x": 3155,
                    "y": 570,
                    "w": 104,
                    "h": 129
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 104,
                    "h": 129
                },
                "sourceSize": {
                    "w": 104,
                    "h": 129
                }
            },
            {
                "filename": "Winter (30).png",
                "frame": {
                    "x": 2752,
                    "y": 2043,
                    "w": 233,
                    "h": 170
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 233,
                    "h": 170
                },
                "sourceSize": {
                    "w": 233,
                    "h": 170
                }
            },
            {
                "filename": "Winter (31).png",
                "frame": {
                    "x": 2985,
                    "y": 2043,
                    "w": 233,
                    "h": 170
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 233,
                    "h": 170
                },
                "sourceSize": {
                    "w": 233,
                    "h": 170
                }
            },
            {
                "filename": "Winter (32).png",
                "frame": {
                    "x": 2664,
                    "y": 2213,
                    "w": 233,
                    "h": 169
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 233,
                    "h": 169
                },
                "sourceSize": {
                    "w": 233,
                    "h": 169
                }
            },
            {
                "filename": "Winter (33).png",
                "frame": {
                    "x": 2897,
                    "y": 2213,
                    "w": 233,
                    "h": 170
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 233,
                    "h": 170
                },
                "sourceSize": {
                    "w": 233,
                    "h": 170
                }
            },
            {
                "filename": "Winter (34).png",
                "frame": {
                    "x": 2664,
                    "y": 2383,
                    "w": 233,
                    "h": 169
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 233,
                    "h": 169
                },
                "sourceSize": {
                    "w": 233,
                    "h": 169
                }
            },
            {
                "filename": "Winter (35).png",
                "frame": {
                    "x": 2897,
                    "y": 2383,
                    "w": 233,
                    "h": 169
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 233,
                    "h": 169
                },
                "sourceSize": {
                    "w": 233,
                    "h": 169
                }
            },
            {
                "filename": "Winter (36).png",
                "frame": {
                    "x": 378,
                    "y": 2552,
                    "w": 233,
                    "h": 171
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 233,
                    "h": 171
                },
                "sourceSize": {
                    "w": 233,
                    "h": 171
                }
            },
            {
                "filename": "Winter (37).png",
                "frame": {
                    "x": 1354,
                    "y": 2552,
                    "w": 233,
                    "h": 154
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 233,
                    "h": 154
                },
                "sourceSize": {
                    "w": 233,
                    "h": 154
                }
            },
            {
                "filename": "Winter (38).png",
                "frame": {
                    "x": 1587,
                    "y": 2552,
                    "w": 233,
                    "h": 169
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 233,
                    "h": 169
                },
                "sourceSize": {
                    "w": 233,
                    "h": 169
                }
            },
            {
                "filename": "Winter (39).png",
                "frame": {
                    "x": 1820,
                    "y": 2552,
                    "w": 233,
                    "h": 170
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 233,
                    "h": 170
                },
                "sourceSize": {
                    "w": 233,
                    "h": 170
                }
            },
            {
                "filename": "Winter (3).png",
                "frame": {
                    "x": 3155,
                    "y": 699,
                    "w": 110,
                    "h": 127
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 110,
                    "h": 127
                },
                "sourceSize": {
                    "w": 110,
                    "h": 127
                }
            },
            {
                "filename": "Winter (40).png",
                "frame": {
                    "x": 2053,
                    "y": 2552,
                    "w": 233,
                    "h": 170
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 233,
                    "h": 170
                },
                "sourceSize": {
                    "w": 233,
                    "h": 170
                }
            },
            {
                "filename": "Winter (41).png",
                "frame": {
                    "x": 2660,
                    "y": 2552,
                    "w": 233,
                    "h": 170
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 233,
                    "h": 170
                },
                "sourceSize": {
                    "w": 233,
                    "h": 170
                }
            },
            {
                "filename": "Winter (42).png",
                "frame": {
                    "x": 2893,
                    "y": 2552,
                    "w": 233,
                    "h": 169
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 233,
                    "h": 169
                },
                "sourceSize": {
                    "w": 233,
                    "h": 169
                }
            },
            {
                "filename": "Winter (43).png",
                "frame": {
                    "x": 0,
                    "y": 2721,
                    "w": 233,
                    "h": 170
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 233,
                    "h": 170
                },
                "sourceSize": {
                    "w": 233,
                    "h": 170
                }
            },
            {
                "filename": "Winter (44).png",
                "frame": {
                    "x": 611,
                    "y": 2721,
                    "w": 233,
                    "h": 169
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 233,
                    "h": 169
                },
                "sourceSize": {
                    "w": 233,
                    "h": 169
                }
            },
            {
                "filename": "Winter (45).png",
                "frame": {
                    "x": 844,
                    "y": 2721,
                    "w": 233,
                    "h": 169
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 233,
                    "h": 169
                },
                "sourceSize": {
                    "w": 233,
                    "h": 169
                }
            },
            {
                "filename": "Winter (46).png",
                "frame": {
                    "x": 1077,
                    "y": 2721,
                    "w": 233,
                    "h": 171
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 233,
                    "h": 171
                },
                "sourceSize": {
                    "w": 233,
                    "h": 171
                }
            },
            {
                "filename": "Winter (47).png",
                "frame": {
                    "x": 1310,
                    "y": 2721,
                    "w": 233,
                    "h": 154
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 233,
                    "h": 154
                },
                "sourceSize": {
                    "w": 233,
                    "h": 154
                }
            },
            {
                "filename": "Winter (4).png",
                "frame": {
                    "x": 3155,
                    "y": 826,
                    "w": 104,
                    "h": 130
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 104,
                    "h": 130
                },
                "sourceSize": {
                    "w": 104,
                    "h": 130
                }
            },
            {
                "filename": "Winter (5).png",
                "frame": {
                    "x": 3155,
                    "y": 956,
                    "w": 160,
                    "h": 129
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 160,
                    "h": 129
                },
                "sourceSize": {
                    "w": 160,
                    "h": 129
                }
            },
            {
                "filename": "Winter (6).png",
                "frame": {
                    "x": 3150,
                    "y": 1085,
                    "w": 114,
                    "h": 166
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 114,
                    "h": 166
                },
                "sourceSize": {
                    "w": 114,
                    "h": 166
                }
            },
            {
                "filename": "Winter (7).png",
                "frame": {
                    "x": 136,
                    "y": 570,
                    "w": 38,
                    "h": 65
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 38,
                    "h": 65
                },
                "sourceSize": {
                    "w": 38,
                    "h": 65
                }
            },
            {
                "filename": "Winter (8).png",
                "frame": {
                    "x": 1653,
                    "y": 570,
                    "w": 32,
                    "h": 52
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 32,
                    "h": 52
                },
                "sourceSize": {
                    "w": 32,
                    "h": 52
                }
            },
            {
                "filename": "Winter (9).png",
                "frame": {
                    "x": 1685,
                    "y": 570,
                    "w": 42,
                    "h": 69
                },
                "rotated": false,
                "trimmed": false,
                "spriteSourceSize": {
                    "x": 0,
                    "y": 0,
                    "w": 42,
                    "h": 69
                },
                "sourceSize": {
                    "w": 42,
                    "h": 69
                }
            }
        ],
        "meta": {
            "app": "http://www.codeandweb.com/texturepacker",
            "version": "1.0",
            "image": "spritesheet.png",
            "format": "RGBA8888",
            "size": {
                "w": 3322,
                "h": 2892
            },
            "scale": "1"
        }
    }
    """u8;
#endregion

#region Setup
const string REFERENCE_SPRITE_NAME = "Snow (29).png";
const float TILE_DISTANCE_X = 70f;
const float TILE_DISTANCE_Y = 10f;
const float INITIAL_SCALE = 0.5f;
const int SIDE_LENGTH = 10;
const int NUMBER_OF_PLAYER_TILES = 7;
const float MOUSE_WHEEL_SENSITIVITY = 2000f;

// Load spritesheet from resources
var sprites = SpritesheetLoader.Load(framesJson, "Images/Spritesheet.png");

// Get a reference frame. This reference tile image is used to determine the
// size of a single tile. All other tile images will be resized to the size
// of the reference tile.
var referenceFrame = sprites.Frames[REFERENCE_SPRITE_NAME];
var tileSize = referenceFrame.SourceSize + new SKSize(TILE_DISTANCE_X, TILE_DISTANCE_Y);

float scale = INITIAL_SCALE; // Current scale factor (can be changed with mouse wheel)
SKPoint translation = new SKPoint(); // Current position of the viewport (can be changed with mouse drag)
SKPoint? mouseDownPosition = null; // Position of the mouse when the mouse button is pressed, for dragging the viewport

// Create the player sprite. We have multiple sprites (see NUMBER_OF_PLAYER_TILES).
// The player color changes whenever it picks up a bonus chip.
var playerTileId = 0;
var player = new Player(sprites, $"User0{playerTileId + 1}.png", -tileSize.Height / 2.5f);

// Create the game board
var level = new Tile[SIDE_LENGTH][];
#endregion

#region Level building
/*
// Simple level
for (int y = 0; y < SIDE_LENGTH; y++)
{
    level[y] = new Tile[SIDE_LENGTH];
    for (int x = 0; x < SIDE_LENGTH; x++)
    {
        level[y][x] = TileTypes.Arctic[1] with { };
    }
}
*/
// Current theme
var theme = TileTypes.Arctic;

// Generate 10_000 tile rows. This is for demo purposes only!
// Obviously this would not make sense in a real-world application. We generate
// many random rows to be able to demonstrate different variants of list patterns.
// You will see that a few lines below where we build the level.
var randomRows = Enumerable.Range(0, 10_000)
    .Select(_ => Enumerable.Range(0, SIDE_LENGTH).Select(_ => TileTypes.GetRandom(theme)).ToArray())
    .ToArray();

// Generic parse method. It works with any parsable type, including Tile.
// It splits a string by blanks and parses each resulting part.
// This approach is for demo purposes only. It is used to demonstrate the
// new generic parsing feature of .NET.
static T[] Parse<T>(string rowString) where T : IParsable<T>
    => rowString.Trim().Split(' ').Select(item => T.Parse(item, CultureInfo.InvariantCulture)).ToArray();

var row = 0;
// Demonstrate generic parse method to add a hand-crafted row to our level
// See also https://slides.com/rainerstropek/csharp-11/fullscreen#/11
level[row++] = Parse<Tile>("A2 A2 A3 A4 A4 A7 A14 A14 A15 A2");

#region List patterns
// Select random rows using list patterns.
// See also https://slides.com/rainerstropek/csharp-11/fullscreen#/4
level[row++] = randomRows.First(r => r is [{ IsEmpty: false, Decoration: null }, ..] && !r.Any(r => r.IsBridge)).CloneTiles();
level[row++] = randomRows.First(r => r is [{ IsBridge: false }, { Type: "" }, .., { IsBridge: false }]).CloneTiles();
level[row++] = randomRows.First(r => r is [{ IsBridge: false }, { IsBridge: true }, { IsBridge: false }, .., { IsBridge: false }]).CloneTiles();
level[row++] = randomRows.First(r => r is [{ IsBridge: false }, { Type: "" }, .., { IsBridge: false }]).CloneTiles();
level[row++] = randomRows.First(r => !r.Any(r => r.IsBridge)).CloneTiles();
#endregion

// Fill up the remaining rows with random tiles
for (; row < SIDE_LENGTH; row++) { level[row] = randomRows[Random.Shared.Next(0, randomRows.Length)].CloneTiles(); }
#endregion

#region Handler methods
void Draw(SKCanvas canvas, SKImageInfo imageInfo)
{
    // White background
    canvas.Clear(SKColors.White);

    canvas.Save();

    // Initialize viewport if it has not been initialized yet
    if (translation == SKPoint.Empty)
    {
        // Center the game board horizontally, and move it down (from very top) a bit
        translation = new(imageInfo.Width / 2, tileSize.Height * scale);
    }

    // Apply viewport transformation
    canvas.Translate(translation);
    canvas.Scale(scale);

    // Draw the game board
    for (SKPointI pos = new SKPointI(); pos.Y < SIDE_LENGTH; pos.Offset(-pos.X, 1))
    {
        for (; pos.X < SIDE_LENGTH; pos.Offset(1, 0))
        {
            canvas.Save();

            // Move to the tile position
            canvas.Translate(((pos.X - pos.Y) * tileSize.Width) / 2, ((pos.X + pos.Y) * tileSize.Height) / 2);

            // Draw the tile
            var tile = level[pos.Y][pos.X];
            tile.Draw(sprites, canvas, referenceFrame.SourceSize);

            #region Player drawing
            // Draw the player if it is on the current position
            if (pos == player.PlayerPosition)
            {
                // Check if player picked up a bonus chip
                if (tile.Decoration?.Pickable ?? false)
                {
                    // Remove bonus chip
                    tile.Decoration = null;

                    // Change player color
                    playerTileId = (playerTileId + 1) % NUMBER_OF_PLAYER_TILES;
                    player.SpriteName = $"User0{playerTileId + 1}.png";
                }

                // Draw the player
                player.Draw(canvas);
            }
            #endregion

            canvas.Restore();
        }
    }

    canvas.Restore();
}

#region Keyboard handling
void KeyDown(KeyEventArgs key)
{
    // We only care about the arrow keys
    if (key.Key is not Key.Up and not Key.Down and not Key.Left and not Key.Right) { return; }

    // Calculate next position
    var nextPosition = player.PlayerPosition;
    var offset = key.Key switch
    {
        Key.Up => new SKPointI(0, -1),
        Key.Down => new SKPointI(0, 1),
        Key.Left => new SKPointI(-1, 0),
        Key.Right => new SKPointI(1, 0),
        _ => SKPointI.Empty
    };
    nextPosition.Offset(offset);

    if (nextPosition.X >= 0 && nextPosition.Y >= 0 && nextPosition.X < SIDE_LENGTH
        && nextPosition.Y < SIDE_LENGTH
        && level[nextPosition.Y][nextPosition.X].CanGoTo)
    { 
        player.PlayerPosition = nextPosition;
    }
}
#endregion

#region Mouse handling
void MouseWheel(float delta) => scale += delta / MOUSE_WHEEL_SENSITIVITY;
void MouseDown(SKPoint position) => mouseDownPosition = position;
void MouseUp(SKPoint position) => mouseDownPosition = null;
bool MouseMove(SKPoint position)
{
    if (mouseDownPosition.HasValue)
    {
        translation += position - mouseDownPosition.Value;
        mouseDownPosition = position;
        return true;
    }

    return false;
}
#endregion

GameApplication.Run(new(
    Draw: Draw,
    KeyDown: KeyDown,
    MouseDown: MouseDown,
    MouseUp: MouseUp,
    MouseMove: MouseMove,
    MouseWheel: MouseWheel
));
#endregion
