# ExeTypeToolset

A toolset for identifying .exe files within multiple releases (archived as .zip/.7z/.rar or inside cd images e.g. .iso/.cue+.bin)

After analysis, ExeTypeToolset creates a tabulator separated .csv table with the analysis result (directory, file, number of DOS/NONDOS/ERROR executables).

You can also let ExeTypeToolset move the original releases to different locations based on the analysis outcome.

## Dependencies

-  **7zip** and **Iso7z** (included in the binary releases as 7z subdirectory) - which allow extraction of archives and cd images in order to find .exe files to analyze.
- **dosfinder** (included in the binary releases as dosfinder subdirectory) - this tool by **anormal** utilizes itself multiple detection engines to identify executables
- **dosidcli** (still included, but deprecated by **dosfinder**) (https://github.com/theMK2k/DOSIDCLI) - a small command line tool which identifies an executable being DOS or SOMETHINGELSE

## Command Line Options

ExeTypeToolset.exe provides the following command line options:

````
-i={input path}      - location of (sub)directories containing to-be-analyzed releases (.zip/.7z/.cue+bin/.iso)
-o={output file}     - (optional) location of output .csv file (default: 'results.csv')
-t={temp path}       - (optional) location for temporary, extracted, files (default: system-defined temp)
-7z={7z location}    - (optional) location of your 7z installation including iso7z (default: '.\7z\7z.exe')
-7zextensions={list} - (optional) list of extensions you want to handle via 7z (default: '.zip .rar .7z .cue .iso')
-movedos={path}      - move DOS releases to this path (optional)
-movenondos={path}   - move non-DOS releases to this path (optional)
-moveerror={path}    - move releases containing errors to this path (optional)
-movedirectory       - instead of single files, move the whole directory (use this if a release is always contained in its own directory)
-movedryrun          - don't actually move anything just mention it in the console
-h                   - show this help
````

## Examples
The simplest call would be just to provide an input path for analysis:
````
ExeTypeToolset.exe -i="g:\My ISO Collection\Redump"
````

A more complex example:
````
ExeTypeToolset.exe -i="g:\My ISO Collection\Redump" -o="redump.csv" -7z="d:\Apps\7Zip\7z.exe" -t="d:\Temp" -7zextensions=".zip .rar .7z .cue .bin" -movedos="g:\My ISO Collection\Redump\_DOS_" -movenondos="g:\My ISO Collection\Redump\_NON_DOS_" -moveerror="g:\My ISO Collection\Redump\_ERROR_"
````