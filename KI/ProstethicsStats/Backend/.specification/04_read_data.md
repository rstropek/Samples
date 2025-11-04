# Read Data

## Overview

This application already executes prompts against an AI model. The AI model generates Python code, which will be executed. The Python code generates HTML and optional plots. The HTML fragment is sent to the frontend using HTMX.

In this iteration, we want to integrate the reading of a dataset into the application as the users' questions will revolve around the data in the dataset.

## Read Data Frame

* The application must read the dataset in [prosthetics_data.csv](../data/prosthetics_data.csv) at startup (note: it is large; nearly 100MB) into memory. You can assume that the data fits into memory easily.
* The dataset must be stored in a Pandas DataFrame for further processing.
* The dataset must be made accessible to the generated Python code (`exec()` environment) under the variable name `df`. Provide a copy of the DataFrame to avoid accidental modifications.
* The system prompt in [system_prompt.md](../src/config/system_prompt.md) must be adjusted accordingly.
  * It must inform the AI model that the dataset is available in a Pandas DataFrame named `df`.
  * It must inform the AI model about the structure of the DataFrame (columns, data types, first few rows).
  * It must instruct the AI model to use the DataFrame `df` to answer user.
  * It must instruct the AI model to not make any changes to the DataFrame `df`.

## Add Sample Requests

[index.html](../src/templates/index.html) already contains placeholders for sample requests. Replace them with the following sample requests:

* Show me the distribution of amputation levels
* What is the average satisfactory rating per foot type, knee type, and hip type?
* Show me a scatter plot of days since last fitting versus outcome score
