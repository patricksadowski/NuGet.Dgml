# NuGet.Dgml

The aim of the repository is to provide tools for analyzing and visualizing
dependencies of NuGet packages leveraging directed graphs
([DGML](https://en.wikipedia.org/wiki/DGML)).

The library was indented to manage and maintain local repositories at home or
office.

# Usage

*Note:* Due some limitations of
[NuGet.Core](http://www.nuget.org/packages/NuGet.Core/) it's not recommended
to execute the functions against a large repository like [nuget.org]().
Many functions have to investigate a whole package repository!

You only have to import the namespace *NuGet.Dgml* and look for new extension
methods on the NuGet types.

Sample:
```c#
using NuGet;
using NuGet.Dgml;

var repository = PackageRepositoryFactory.Default.CreateRepository("N:\My Package Repository\");
var directedGraph = repository.VisualizeUpgradeableDependencies();
directedGraph.AsXDocument().Save("C:\My Package Repository.dgml");
```
# Functions

The functions are implemented as extension methods. These methods use public
types included in the library.

| Extension method for type: | IPackageRepository | IPackageMetadata |
|:--|:-:|:-:|
| VisualizeUpgradeableDependencies | Implemented  | *Planned* |
