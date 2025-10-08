import readline from 'readline';

/**
 * Prompts the user for input with the given prompt text and returns the entered value.
 * 
 * @param {string} prompt - The text to display to the user as a prompt
 * @returns {Promise<string>} A promise that resolves with the user's input
 * @example
 * const name = await readLine('Enter your name: ');
 */
export function readLine(prompt: string): Promise<string> {
  return new Promise((resolve) => {
    const rl = readline.createInterface({
      input: process.stdin,
      output: process.stdout,
    });

    rl.question(prompt, (answer) => {
      rl.close();
      resolve(answer);
    });
  });
}

/**
 * Waits for a single keypress from the user without requiring Enter to be pressed.
 * 
 * @returns {Promise<void>} A promise that resolves when a key is pressed
 * @example
 * console.log('Press any key to continue...');
 * await readKey();
 */
export function readKey(): Promise<void> {
  return new Promise((resolve) => {
    process.stdin.setRawMode(true);
    process.stdin.setEncoding('utf-8');

    const onKeyPress = (key: any) => {
      process.stdin.removeListener('data', onKeyPress);
      process.stdin.setRawMode(false);
      resolve(key);
    };

    process.stdin.on('data', onKeyPress);
  });
}