# Journal Program (C# Console Application)

## Overview
This project is a C# console-based journal application that allows users to record daily events by responding to prompted questions. Each journal entry stores the date, the prompt, the user’s response, and an optional mood. Users can create, view, save, load, and delete journal entries through a menu-driven interface.

This program was developed to exceed the requirements of the prove Develop02 Journal assignment.

---

## Features

### Core Functional Requirements

- **Write a new entry**
  - Displays a random prompt from a predefined list
  - Saves the prompt, user response, and date as a journal entry

- **Display the journal**
  - Displays all saved journal entries to the console

- **Save the journal to a file**
  - Prompts the user for a filename
  - Saves all journal entries to the specified file

- **Load the journal from a file**
  - Prompts the user for a filename
  - Loads entries from the file, replacing any existing entries in memory

- **Menu-driven interface**
  - Allows the user to select actions until they choose to quit the program

- **Prompt list**
  - Contains multiple prompts, including prompts inspired by the assignment examples

- **Delete Entry** (Note, not a core requirement, but rather a feature that exceeds expectations)
  - Allows the user to delete a journal entry by selecting its number

---

## Design & Structure

### Object-Oriented Design

- `Program` – Handles the main menu and user interaction
- `Entry` – Represents a single journal entry
- `Journal` – Manages the list of entries and handles saving and loading

---

### Abstraction and Encapsulation
- Member variables are kept private inside their classes
- Public methods expose only the functionality needed by other parts of the program
- The `Program` class controls program flow without directly managing journal data