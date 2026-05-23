#!/bin/bash

# Remove temp.md if it exists
rm -f temp.md

echo -e "\\pagebreak\n\n" >> temp.md

# Process all other .md files, removing h1 headers
for file in [0-9][0-9]-*.md; do
    echo "Processing $file..."
    sed 's/🔴/O/g' "$file" | sed 's/⚪/o/g' >> temp.md
    echo -e "\n\n\\pagebreak\n\n" >> temp.md
done

echo "Created temp.md"

docker run --rm \
    -v $(pwd):/data \
    pandoc/extra temp.md --metadata-file=metadata.yaml --template eisvogel -o output.pdf

# Remove temp.md if it exists
rm -f temp.md
