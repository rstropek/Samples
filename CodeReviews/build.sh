#!/bin/bash
docker run --rm -v $(pwd):/data -w /data  rstropek/pandoc-latex -f markdown \
    --template https://raw.githubusercontent.com/Wandmalfarbe/pandoc-latex-template/v1.4.0/eisvogel.tex \
    -t latex --lua-filter pagebreak.lua --filter pandoc-citeproc \
    -o dist/CodeReviews.pdf \
    --metadata-file=CodeReviews.yaml \
    CodeReviews.md
