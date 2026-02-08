# Smart File Organizer (CLI)

## Usage

```
dotnet run -- --preset downloads --mode dryrun
```

```
dotnet run -- --root "C:\\Path\\To\\Folder" --recursive --mode apply --conflict rename
```

```
dotnet run -- --preset documents --undo
```

Notes:
- Default rule pack location is `<root>/rulepack.json`. Use `--rules <path>` to override.
- `NamePatternRule` matches against the file name without extension.

## Publish (self-contained)

```
dotnet publish -c Release -r win-x64 --self-contained true
```

```
dotnet publish -c Release -r osx-x64 --self-contained true
```

```
dotnet publish -c Release -r linux-x64 --self-contained true
```
