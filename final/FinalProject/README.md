# Virtual Organizer Arena

Virtual Organizer Arena is a turn-based console game where you draft a custom rule pack to sort a virtual inbox into folders. Each file is routed by your rules in priority order, and every action costs turns, so accuracy and efficiency both matter.

## How It Works

1. Enter a seed and choose a difficulty. This creates a deterministic scenario with a fixed set of files and dates.
2. Draft your rules. You can add, remove, and reorder rules to set priority. Exactly one fallback rule is required.
3. Start combat. On each turn you can sort, undo, or review the inbox and rules.
4. Sorting applies the first matching rule. Correct sorts score +10, incorrect sorts score -5 and cost an extra turn. Items are always moved.
5. Win by clearing the inbox before the turn limit. You can undo only the most recent move (it costs 1 turn).

## Seed System

The scenario is generated deterministically from an integer seed. Using the same seed and difficulty will always produce the same items, dates, and correct folders.

## Organization Skills

The game teaches organization by turning file sorting into a rules-and-priority exercise. You practice defining clear categories, placing broad rules last (fallback) and specific rules earlier, and adjusting priorities when mistakes happen. The turn limit and scoring reinforce accuracy, consistency, and efficient workflows, which are the same habits that keep real folders and naming schemes tidy.
