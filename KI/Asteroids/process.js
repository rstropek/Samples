let input = '';

process.stdin.on('data', function(chunk) {
    input += chunk;
});

process.stdin.on('end', function() {
    processInput(input);
});

function processInput(input) {
    // Split input by double newlines to get individual JSON documents
    const docs = input.split('\n\n');

    for (let doc of docs) {
        doc = doc.trim();
        if (!doc) continue;

        // Remove 'data: ' prefix if present
        if (doc.startsWith('data: ')) {
            doc = doc.substring('data: '.length);
        }

        try {
            const json = JSON.parse(doc);

            if (json.choices && Array.isArray(json.choices)) {
                for (let choice of json.choices) {
                    if (choice.delta && choice.delta.content !== undefined) {
                        process.stdout.write(choice.delta.content);
                    }
                }
            }
        } catch (e) {
            // Ignore parsing errors and continue
        }
    }
}
