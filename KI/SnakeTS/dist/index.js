// Simulating an async operation
const delay = (ms) => new Promise(resolve => setTimeout(resolve, ms));
console.log('Starting the application...');
// Using top-level await
await delay(1000);
console.log('After 1 second delay');
const result = await Promise.all([
    delay(500).then(() => 'First operation'),
    delay(800).then(() => 'Second operation')
]);
console.log('Results:', result);
console.log('Application finished!');
export {};
