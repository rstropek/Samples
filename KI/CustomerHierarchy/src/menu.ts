import { stringify } from 'querystring';
import * as readline from 'readline';

export type MenuItem = {
    id: string;
    label: string;
};

function buildMenuText(menuItems: MenuItem[]): string {
    return menuItems.map((item, index) => `${index + 1}. ${item.label}`).join('\n');
}

export async function getMenuChoice(menuName: string, menuItems: MenuItem[]): Promise<MenuItem> {

    console.log(`\n${menuName}\n${'='.repeat(menuName.length)}\n`);
    console.log(buildMenuText(menuItems));
    
    const choice = await getUserChoice(1, menuItems.length);
    console.log("\n");

    return menuItems[choice - 1];
}

async function getUserChoice(min: number, max: number): Promise<number> {
    const rl = readline.createInterface({
        input: process.stdin,
        output: process.stdout
    });

    function askQuestion(query: string): Promise<string> {
        return new Promise(resolve => rl.question(query, resolve));
    }

    let choice: number;
    while (true) {
        const answer = await askQuestion(`Please enter a number between ${min} and ${max}: `);
        choice = parseInt(answer, 10);

        if (!isNaN(choice) && choice >= min && choice <= max) {
            break;
        } else {
            console.log(`Invalid input. Please enter a number between ${min} and ${max}.`);
        }
    }

    rl.close();
    return choice;
}