# WatchFaceCLI

A command-line tool for generating .bin files for watchfaces, using the same logic as the main WatchFace WPF application.

## Prerequise
- .NET 6.0 SDK or later (with Windows 7.0 support)
- The main WatchFace solution and all dependencies must be built

## Usage

### Build the CLI
```
dotnet build WatchFaceCLI/WatchFaceCLI.csproj
```

### Generate a bin file
Run the following command, replacing the input and output folders as needed:
```
dotnet run --project WatchFaceCLI/WatchFaceCLI.csproj -- "<inputFolder>" "<outputFolder>"
```
- `<inputFolder>`: Path to the watchface folder (e.g., `JL_文件/方形_240x280_普通_202#商务_李剑_009_00`)
- `<outputFolder>`: Path to the output directory where the bin file will be generated

#### Example
```
dotnet run --project WatchFaceCLI/WatchFaceCLI.csproj -- "JL_文件/方形_240x280_普通_202#商务_李剑_009_00" "output_folder"
dotnet run --project WatchFaceCLI/WatchFaceCLI.csproj -- "JL_文件/方形_240x280_普通_206#指针_李剑_001_00" "output_folder"
```

## Output
- The generated `.bin` file will be placed in a timestamped subfolder inside your specified output folder.
- The CLI applies the same LZO compression and logic as the UI, so the output is compatible with the watch.

## Troubleshooting
- Ensure all dependencies are built and up to date.
- If you see errors about `CommonDefintion.Setting`, make sure the CLI is initializing settings (this is handled in `Program.cs`).
- If the output file size is much larger than expected, verify that LZO compression is being applied (already fixed in this implementation).

## License
This CLI is part of the WatchFace solution and follows the same license as the main project.
