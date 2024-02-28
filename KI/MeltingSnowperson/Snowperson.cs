namespace MeltingSnowperson;

public class Snowperson
{
    public readonly string OriginalImage = """"
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
    """";

    public string GetImage(int numberOfWrongGuesses)
    {
        // Return the original image minus numberOfWrongGuesses lines from the bottom
        return string.Join('\n', OriginalImage.Split('\n').Take(OriginalImage.Split('\n').Length - numberOfWrongGuesses));
    }
}