import * as readline from 'readline';
function buildMenuText(menuItems) {
    return menuItems.map((item, index) => `${index + 1}. ${item.label}`).join('\n');
}
export async function getMenuChoice(menuName, menuItems) {
    console.log(`\n${menuName}\n${'='.repeat(menuName.length)}\n`);
    console.log(buildMenuText(menuItems));
    const choice = await getUserChoice(1, menuItems.length);
    console.log("\n");
    return menuItems[choice - 1];
}
async function getUserChoice(min, max) {
    const rl = readline.createInterface({
        input: process.stdin,
        output: process.stdout
    });
    function askQuestion(query) {
        return new Promise(resolve => rl.question(query, resolve));
    }
    let choice;
    while (true) {
        const answer = await askQuestion(`Please enter a number between ${min} and ${max}: `);
        choice = parseInt(answer, 10);
        if (!isNaN(choice) && choice >= min && choice <= max) {
            break;
        }
        else {
            console.log(`Invalid input. Please enter a number between ${min} and ${max}.`);
        }
    }
    rl.close();
    return choice;
}
//# sourceMappingURL=menu.js.map