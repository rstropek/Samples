import sqlite from 'better-sqlite3';
import * as fs from 'fs';
export function migrate() {
    const db = sqlite('database.db');
    runMigrations(db);
    db.close();
}
function runMigrations(db) {
    db.exec('CREATE TABLE IF NOT EXISTS migrations (id INTEGER PRIMARY KEY, filename TEXT)');
    const migrations = getMigrations(db);
    const files = fs.readdirSync('migrations').sort();
    for (const file of files) {
        if (migrations.some(m => m.filename === file)) {
            continue;
        }
        const sql = fs.readFileSync(`migrations/${file}`, 'utf-8');
        db.exec(sql);
        const stmt = db.prepare('INSERT INTO migrations (filename) VALUES (?)');
        stmt.run(file);
    }
}
function getMigrations(db) {
    const stmt = db.prepare('SELECT filename FROM migrations');
    return stmt.all();
}
//# sourceMappingURL=database.js.map