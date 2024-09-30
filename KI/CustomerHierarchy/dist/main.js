import { migrate } from './database.js';
import { importData } from './import.js';
import { getMenuChoice } from './menu.js';
import { printLinkedPersons } from './printLinked.js';
const mainMenu = [
    { id: '1', label: 'Update database' },
    { id: '2', label: 'Import data' },
    { id: '3', label: 'Print linked persons' },
    { id: '4', label: 'Exit' },
];
while (true) {
    const choice = await getMenuChoice('Main Menu', mainMenu);
    switch (choice.id) {
        case '1':
            console.log('Updating database...');
            migrate();
            console.log('Database updated.');
            break;
        case '2':
            console.log('Importing data...');
            importData();
            console.log('Data imported.');
            break;
        case '3':
            console.log('Print linked persons');
            printLinkedPersons("ACC123");
            console.log('Linked persons printed.');
            break;
        default:
            console.log('Goodbye!');
            break;
    }
    if (choice.id === '3') {
        break;
    }
}
//# sourceMappingURL=main.js.map