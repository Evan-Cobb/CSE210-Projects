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
Demo seed: `133742`

## Self-check

Run: `dotnet run -- --selftest`
Fast mode (no demo delay): `dotnet run -- --selftest --selftest-nodelay`

The self-check runs a small named test suite using the demo seed on Easy difficulty. It prints each test as `RUN`, then `PASS` or `FAIL`, including timing and a short result line so it is clear what was verified.

Current checks:
1. Deterministic scenario generation
2. Difficulty ranges and turn-limit formula
3. Seed determinism matrix across difficulties
4. Rule priority uses first-match behavior
5. ExtensionRule case-insensitive matching
6. NamePatternRule mode behavior
7. DateBucketRule Year/YearMonth behavior
8. Correct sort bookkeeping
9. Incorrect sort penalty bookkeeping
10. Undo after correct sort
11. Undo after incorrect sort
12. Undo on empty stack turn cost
13. Perfect rule pack clears inbox within turn limit
14. Undo restores one item after full clear

Final output is `PASS` or `FAIL` with exit code `0` or `1`.

## Organization Skills

The game teaches organization by turning file sorting into a rules-and-priority exercise. You practice defining clear categories, placing broad rules last (fallback) and specific rules earlier, and adjusting priorities when mistakes happen. The turn limit and scoring reinforce accuracy, consistency, and efficient workflows, which are the same habits that keep real folders and naming schemes tidy.
