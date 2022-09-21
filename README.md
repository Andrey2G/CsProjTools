# Tools for managing *.csproj files

In case when you have a solution with tens+ projects  it's very hard to manage them.

Commands:

- sort - sorting Packages

``` cmd
Usage:
  CsProjTools -sort [options]

Options:
  --file <file>            process specified .csproj file.
  --directory <directory>  process .csproj files from specified directory
  -?, -h, --help           Show help and usage information
  
```

## Warning

Please, make backup of *csproj file(s) before executing this tool to avoid any unexpected corruptions

## Notice

- Only for *.csproj files with .Net 5+
- If the group of packages have a condition(s) it will be ignored at the moment

## What's Next?

- Updating product information?
- Updating product/assembly versions?
