const { exec } = require('child_process');

exec('az account get-access-token --resource "https://vault.azure.net"', (err, out) => {
    if (!err) {
        const result = JSON.parse(out);
        console.log(result.accessToken);
    }
});