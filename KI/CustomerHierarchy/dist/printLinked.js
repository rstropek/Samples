import Database from 'better-sqlite3';
export function printLinkedPersons(accountNumber) {
    // Replace 'path_to_database.db' with the path to your SQLite database file
    const db = new Database('database.db');
    // Method to get all persons linked to an account (directly or indirectly)
    // Prepare SQL statements
    const getAccount = db.prepare('SELECT id FROM Account WHERE number = ?');
    const getDirectHolders = db.prepare(`
        SELECT Person.id, Person.name 
        FROM Holder 
        JOIN Person ON Holder.person_id = Person.id 
        WHERE Holder.account_id = ?
    `);
    const getConnections = db.prepare(`
        SELECT person2_id 
        FROM Connection 
        WHERE person1_id = ?
    `);
    // Find the account ID for the given account number
    const account = getAccount.get(accountNumber);
    if (!account) {
        console.log('Account not found.');
        return;
    }
    const accountId = account.id;
    // Set to store IDs of persons who have already been visited (to prevent infinite loops)
    const visitedPersons = new Set();
    // Array to collect the names of linked persons
    const linkedPersons = [];
    // Recursive function to find all connected persons
    function findConnectedPersons(personId) {
        if (visitedPersons.has(personId))
            return; // Prevent processing the same person again
        visitedPersons.add(personId);
        // Get the connections of the current person
        const connections = getConnections.all(personId);
        for (const connection of connections) {
            findConnectedPersons(connection.person2_id);
        }
        // Find the name of the current person
        const person = db.prepare('SELECT name FROM Person WHERE id = ?').get(personId);
        if (person) {
            linkedPersons.push(person.name);
        }
    }
    // Get all direct holders of the account
    const directHolders = getDirectHolders.all(accountId);
    for (const holder of directHolders) {
        findConnectedPersons(holder.id);
    }
    // Print all linked persons
    console.log(`Persons linked to account '${accountNumber}':`);
    linkedPersons.forEach((name) => console.log(name));
}
//# sourceMappingURL=printLinked.js.map