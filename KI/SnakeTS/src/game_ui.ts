import * as readline from 'readline';

export function listenToKeyPress(callback: (key: string) => void) {
    readline.emitKeypressEvents(process.stdin);
    process.stdin.setRawMode(true);

    const handleKeyPress = (str: string, key: readline.Key) => {
        if (key.name === 'escape') {
            process.stdin.setRawMode(false);
            process.stdin.removeListener('keypress', handleKeyPress);
            process.exit();
        }
        callback(key.name ?? '');
    };

    process.stdin.on('keypress', handleKeyPress);
}