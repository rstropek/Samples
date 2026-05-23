# Output Formatting

## Overview

This application generates a HTML fragment on the server-side. It is injected in [index.html](../src/templates/index.html) inside the `ai-output` div.

## Styles

Add necessary styles to [style.css](../src/static/style.css) for:

* Headings (h1 to h4)
* Paragraphs
* Lists (ordered and unordered)
* Tables
* Images (must not exceed the width of the container, maintain aspect ratio)
* Links
* Code blocks and inline code

Ensure the styles provide a clean and professional appearance, enhancing readability and user experience.

## Show Loading Indicator

Display a loading indicator **instead** of the output area in the frontend while waiting for the `/ask` response. Replace the loading indicator with the response output once the response is received. Use the following CSS for it:

```css
.loader {
  width: 8px;
  height: 40px;
  border-radius: 4px;
  display: inline-block;
  margin-left: 20px;
  margin-top: 10px;
  position: relative;
  background: currentColor;
  color: #FFF;
  box-sizing: border-box;
  animation: animloader 0.3s 0.3s linear infinite alternate;
}
.loader::after,
.loader::before {
  content: '';  
  box-sizing: border-box;
  width: 8px;
  height: 40px;
  border-radius: 4px;
  background: currentColor;
  position: absolute;
  bottom: 0;
  left: 20px;
  animation: animloader1 0.3s  0.45s  linear infinite alternate;
}
.loader::before {
  left: -20px;
  animation-delay: 0s;
}

@keyframes animloader {
  0% {
    height: 40px;
    transform: translateY(0);
  }
  100% {
    height: 10px;
    transform: translateY(30px);
  }
}

@keyframes animloader1 {
  0% {
    height: 48px;
  }
  100% {
    height: 4.8px;
  }
}
```

If necessary, adjust the `hx-indicator` attribute in the form accordingly.
