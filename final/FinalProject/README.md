# Virtual Organizer Move Challenge

Virtual Organizer Move Challenge is a move-limited console game where you draft a custom rule pack to sort a virtual inbox into folders. Each file is routed by your rules in priority order, and every action costs moves, so accuracy and efficiency both matter.

## How It Works

1. Enter a seed and choose a difficulty. This creates a deterministic scenario with a fixed set of files and dates.
2. Build your starting rules. You can add, remove, and reorder rules to set priority. Exactly one fallback rule is required.
3. Start the move challenge. On each move you can sort, undo, review the inbox/rules, add a new rule with priority, or change rule priority mid-run.
4. Sorting applies the first matching rule. Correct sorts score +10, incorrect sorts score -5 and cost an extra move. Items are always moved.
5. Win by clearing the inbox before the move limit. You can undo only the most recent move (it costs 1 move).

## Seed System

The scenario is generated deterministically from an integer seed. Using the same seed and difficulty will always produce the same items, dates, and correct folders.

## Grading Verification Guide

This section is written for instructor grading. It defines exact commands, expected results, and pass/fail criteria.

### 1) Environment Prerequisites

1. Open a terminal in `final/FinalProject`.
2. Verify .NET SDK is available: `dotnet --version`
3. Build the project: `dotnet build`
4. Expected result: `Build succeeded.` with `0 Error(s)`.

### 2) Automated Verification (Required)

Run the full self-check suite:

`dotnet run -- --selftest --selftest-nodelay`

Expected evidence:
1. The run prints each test with `[RUN ]` and then `[PASS]` or `[FAIL]`.
2. The summary line ends with `Summary: 14 passed, 0 failed`.
3. Final line is exactly `PASS`.
4. Process exit code is `0`.

PowerShell exit code check:

`$LASTEXITCODE`

Expected: `0`

What this suite validates:
1. Deterministic scenario generation
2. Difficulty ranges and move-limit formula
3. Seed determinism matrix across difficulties
4. Rule priority first-match behavior
5. ExtensionRule case-insensitive matching
6. NamePatternRule Contains/StartsWith/EndsWith
7. DateBucketRule Year/YearMonth behavior
8. Correct sort scoring/move bookkeeping
9. Incorrect sort penalty bookkeeping
10. Undo after correct sort
11. Undo after incorrect sort
12. Undo on empty stack move cost
13. Perfect rule pack clears inbox within move limit
14. Undo restores one item after full clear

### 3) Manual Acceptance Tests (Required)

Run interactive mode:

`dotnet run`


#### A. Rule-pack validation before gameplay

1. At rule workshop, press `4` (Done) with no rules.
2. Expected: program refuses and prints `You must have exactly 1 fallback rule.`
3. Add one `FallbackRule`.
4. Try to add a second `FallbackRule`.
5. Expected: `A fallback rule already exists.`

#### B. Priority assignment when creating new rules

1. Add an `ExtensionRule` after fallback exists.
2. When prompted for priority, choose `1`.
3. Open rule list.
4. Expected: new rule appears above fallback (higher priority).

#### C. Gameplay add-rule and priority actions

1. Start gameplay.
2. Select action `5`.
3. Expected: add-rule flow opens, including priority prompt in the same action.
4. Complete the add flow and choose priority.
5. Expected: total move usage increases by exactly `1` for the full add+priority action.
6. Select action `6`.
7. Expected: reorder flow opens (priority change only).
8. If fewer than 2 rules exist, expected message: `Need at least 2 rules to reorder.`

#### D. Move economy and undo behavior

1. Sort one inbox item that a rule does not cover.
2. Expected: sort is incorrect and move usage increases by `2`.
3. Select `Undo`.
4. Expected: move usage increases by `1`, and one item returns to Inbox.

## Organization Skills

The game teaches organization by turning file sorting into a rules-and-priority exercise. You practice defining clear categories, placing broad rules last (fallback) and specific rules earlier, and adjusting priorities when mistakes happen. The move limit and scoring reinforce accuracy, consistency, and efficient workflows, which are the same habits that keep real folders and naming schemes tidy.

## OOP Principles Demonstrated

1. Abstraction: `RuleBase` defines the common rule contract (`IsMatch`, `DestinationName`, `Describe`) so the engine works with rule behavior instead of rule internals. `VirtualFileSystem` exposes high-level operations (`AddToFolder`, `MoveItem`, `GetFolderItems`) and hides storage details.
2. Encapsulation: `GameState` keeps scoring, move accounting, undo bookkeeping, and correctness checks in one class with controlled updates (`ApplySort`, `ApplyUndo`, `SpendMove`). `RulePack` protects rule ordering and selection logic behind methods (`AddRule`, `RemoveAt`, `Move`, `Pick`) rather than exposing mutable internals.
3. Inheritance: `ExtensionRule`, `NamePatternRule`, `DateBucketRule`, and `FallbackRule` inherit from `RuleBase`.
4. Polymorphism: `RulePack.Pick` iterates over `RuleBase` references and calls overridden methods at runtime, so each rule type applies its own matching logic without conditionals in the engine.
