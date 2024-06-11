# Melting Snowperson

## Introduction

This program is a simple console game written in C# called *Melting Snowperson*. The game is about a snowperson who is melting and the player has to save the snowperson by guessing a word. The game works like this:

1. The game will pick a random word (or word combination) from a list of words.
2. The game will display the word with all the letters replaced by underscores.
3. The player will guess a letter.
4. If the letter is in the word, the game will reveal the letter in the word.
5. If the letter is not in the word, the snowperson will melt a little bit.
6. The player will continue to guess letters until they guess the word or the snowperson melts completely.

"Melting" means that for every wrong guess a line of a a given image of a snowperson (ASCII art) is removed (from bottom top top). Here is the snowperson ([source](https://ascii.co.uk/art/snowman)):

```txt
                  _.--"""-,
                .'         `\
               /             \
               |  /.-.--.-.--.)
               .\|(_._.__._.__)
              (   )   0 _ 0   \
     \  /_     `-|     (_)     |       _\  /
     \\/       /`|             |`\       \//
   '-.\\ \/   |  \   \     /   /  |   \/ //.-'
     __\\|    \   '.  '._.'  .'   /    |//__
        \\   .-'.   `'-----'`   .'-.   //
         \\.'    '-._        .-'\   './/
         /`          `'''''')    )    `\
        /                  (    (      ,\
       ;                O  /\    '-..-'/ ;
       |                  (  '.       /  |
       |                O  )   `;---'`   |
       ;                  /__.-'         ;_
   .-''-\               O `             /  '---'-.
         `.                           .'
           '-._                   _.-'
               `"  '  - - -  ' "``
```

## Words

* Snowflakes
* Frostbite
* Snowboarding
* Ice skating
* Thermometer
* Snowmobile
* Hibernation
* Blizzard
* Wintercoat
* Fireplace
* Snowstorm
* Ice fishing
* Scarves
* Frostwork
* Windchill
* Snowshoes
* Ice crystals
* Freezing rain
* Snowplough
* Antifreeze
