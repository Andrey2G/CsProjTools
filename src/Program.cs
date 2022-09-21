using System.CommandLine;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

var fileOption = new Option<FileInfo?>(name: "--file",description: "process specified .csproj file.");
var directoryOption = new Option<DirectoryInfo?>(name: "--directory", description: "process .csproj files from specified directory");

var rootCommand = new RootCommand("Tools for *.csproj files");
var readCommand = new Command("-sort", "Sorting Package References in csproj file(s)")
            {
                fileOption,
                directoryOption,
            };
rootCommand.AddCommand(readCommand);

readCommand.SetHandler((file, directory) =>
{
    if (file != null) ProcessFile(file);
    if (directory != null) ProcessDirectory(directory);
}, fileOption, directoryOption);

await rootCommand.InvokeAsync(args);

Console.WriteLine("completed");



void ProcessFile(FileInfo file)
{
    Console.WriteLine($"process file {file.FullName}");

    //load csproj as XML
    var doc = XDocument.Load(file.FullName);
    //get a list of packages from the csproj file
    var packages = doc.XPathSelectElements("//PackageReference");

    //add packages to the dictionary
    Dictionary<string, XElement> items = new();
    foreach (var item in packages)
    {
        var attribute = item.Attribute("Include");
        if (attribute!=null)
            items[attribute.Value] = item;
    }

    if (items.Count==0)
    {
        Console.WriteLine($"no packages in {file.FullName}. Skipping.");
        return;
    }
    //now we need to remove a section with all packages
    var itemGroups = doc.XPathSelectElements("//ItemGroup[PackageReference|Reference]");
    itemGroups.Remove();

    //create a new node ItemGroup
    var itemGroup = new XElement("ItemGroup");
    //and add all packages in correct order
    foreach(var package in items.Keys.OrderBy(p=>p))
        itemGroup.Add(items[package]);

    //add new ItemGroup to the csproj file
    doc.Root?.Add(itemGroup);

    //now we are updating changes
    XmlWriterSettings settings = new XmlWriterSettings();
    settings.OmitXmlDeclaration = true; //remove XML header
    settings.Indent = true; //keep formatting
    using (XmlWriter xw = XmlWriter.Create(file.FullName, settings))
    {
        doc.Save(xw);
    }
}

void ProcessDirectory(DirectoryInfo directory)
{
    Console.WriteLine($"process directory {directory.FullName}");
    var files = directory.GetFiles("*.csproj",SearchOption.AllDirectories);
    foreach (var file in files)
        ProcessFile(file);
}

