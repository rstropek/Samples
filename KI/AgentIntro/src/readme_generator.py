# Standard library imports
from typing import Any
import os

# Third-party imports
from agents import Agent, RunContextWrapper, function_tool

@function_tool(name_override="list_files")  
def list_files(ctx: RunContextWrapper[Any]) -> list[str]:
    """
    Recursively lists all files (not folders) in the current directory and its subdirectories.
    Returns:
        list[str]: A list of file paths relative to the current directory.
    """
    # List to store the relative file paths
    file_list: list[str] = []

    # Walk through the directory tree
    for root, _, files in os.walk("."):
        for file_name in files:
            # Construct the relative file path
            rel_dir: str = os.path.relpath(root, ".")
            rel_file: str = os.path.join(rel_dir, file_name) if rel_dir != "." else file_name
            file_list.append(rel_file)

    return file_list

@function_tool(name_override="read_file")
def read_file(ctx: RunContextWrapper[Any], path: str) -> str:
    """
    Reads the contents of a file.

    Args:
        path: The path to the file to read.

    Returns:
        str: The contents of the file.
    """
    try:
        # Open the file in read mode with UTF-8 encoding
        with open(path, "r", encoding="utf-8") as file:
            content: str = file.read()
        return content
    except FileNotFoundError:
        # Return a helpful error message if the file does not exist
        return f"Error: File '{path}' not found."
    except Exception as e:
        # Return a generic error message for any other exceptions
        return f"Error reading file '{path}': {e}"

readme_generator = Agent(
    model="gpt-4.1",
    name="Readme Generator",
    handoff_description="Specialist agent for generating readme files",
    tools=[list_files, read_file],
    instructions="""
      You are a coding agent specialized in generating high-quality README 
      files for code repositories. Your primary goal is to create clear, 
      comprehensive, and well-structured README files that help users and 
      developers understand the purpose, structure, and usage of the project.

      To accomplish this, you must use the provided function tools to:
      - List the files and directories in the project to understand its 
        structure and main components.
      - Read the contents of relevant files (such as source code, configuration files, 
        and documentation) to extract key information about the project's 
        functionality, dependencies, setup instructions, and usage examples.
      - NEVER READ `.env` FILES as they might contain sensitive information.

      Your README should typically include:
      - Project title and a concise description
      - Features and main functionality
      - Explanation of the project structure (with a file/folder overview)

      Always ensure that the information you provide is accurate and based on the 
      actual content of the files you have read. Do not make assumptions about 
      the project; instead, rely on the data you gather using the tools. If you 
      need more information, continue to explore the codebase using the available tools.

      Always start your response with: "Readme Generator here!\n\n"

      Be clear, concise, and helpful in your explanations. Format the README 
      using appropriate Markdown syntax for readability.
      """,
)
