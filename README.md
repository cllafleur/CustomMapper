# MapperDSL

[![NuGet](https://img.shields.io/nuget/v/cllafleur.MapperDslLib.svg)](https://nuget.org/packages/cllafleur.MapperDslLib)
[![Build](https://github.com/cllafleur/MapperDSL/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/cllafleur/MapperDSL/actions/workflows/dotnet.yml)

## Goals

MapperDSL is component to specify mapping between objects with a mapping definition discovered at runtime.

The mapping definition is provided as a plain text easy to change without implying to recompile your solution.

The component could be used as low-code solution to specify mapping by customer.


## Get started

### Basic usage

If you have 2 models to map as follow :

```csharp
class OriginObject
{
    public string Title { get; set; }               // value : "MyTitle"
    public string Description { get; set; }         // value : "MyDescription"

    public DateTime ModificationDate { get; set; }  // value : 01/01/2022
}
``` 

```csharp
class TargetObject
{
    public string Label { get; set; }
    public MoreProperties Properties { get; set; }
    public DateTime Date { get; set; }
}

class MoreProperties
{
    public string Tooltip { get; set; }
}
```

Instead of doing a full procedural implementation of this mapping you could define a specification as this :

```csharp
Title -> Label
Description -> Properties.Tooltip
ModificationDate -> Date
```

To evaluate the mapping definition, you need to write these lines :

```csharp
string mappingDefinition = @"
Title -> Label
Description -> Properties.Tooltip
ModificationDate -> Date
";

// load the mapping definition.
var mapper = new Mapper(new FunctionHandlerProvider(), new StringReader(mappingDefinition));
mapper.Load();

// Get a mapping handler for the specified object types
var handler = mapper.GetMapper<OriginObject, TargetObject>();

var origin = new OriginObject()
{
    Title = "MyTitle",
    Description = "MyDescription",
    ModificationDate = new DateTime(2022, 1, 1),
};
var target = new TargetObject() { Properties = new MoreProperties() };

// Invoke the mapping
handler.Map(origin, target);            
```

### Advance features

#### Functions

#### Tuples

#### Collections
