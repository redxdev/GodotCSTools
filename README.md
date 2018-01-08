# Godot C# Tools

This is a small library of utility functions that make working in C# when using [Godot](https://godotengine.org/) much easier.

## Installing

This library is [available on nuget](https://www.nuget.org/packages/GodotCSTools/)!

To install via NuGet, create a `packages.config` file in the same folder as your Godot project's `.csproj`.

Inside, add the following:

```xml
<package id="GodotCSTools" version="1.0.0-beta1" targetFramework="net45" />
```

Then, edit the `.csproj` file for your Godot project. Find the lines referencing `GodotSharp.dll` and `GodotSharpEditor.dll`
and add the following lines below the `</Reference>` tag:

```xml
<Reference Include="GodotCSTools">
    <HintPath>packages\GodotCSTools.1.0.0-beta1\lib\net45\GodotCSTools.dll</HintPath>
</Reference>
```

The result should look something like this:

```xml
<ItemGroup>
    <Reference Include="GodotSharp">
      <HintPath>$(ProjectDir)\.mono\assemblies\GodotSharp.dll</HintPath>
      <Private>False</Private>
    </Reference>
    <Reference Include="GodotSharpEditor" Condition=" '$(Configuration)' == 'Tools' ">
      <HintPath>$(ProjectDir)\.mono\assemblies\GodotSharpEditor.dll</HintPath>
      <Private>False</Private>
    </Reference>
    <Reference Include="GodotCSTools">
        <HintPath>packages\GodotCSTools.1.0.0-beta1\lib\net45\GodotCSTools.dll</HintPath>
    </Reference>
    <Reference Include="System" />
  </ItemGroup>
```

Finally, make sure you have `nuget` installed (see `https://www.nuget.org/downloads`) and run the following in the same directory
as your `packages.config` file:

    nuget restore

If you are not using the nuget command line, refer to your tool's documentation on restoring packages.

## Building Yourself

Copy a `.mono` folder into this project's folder from an existing Godot (with mono) project. The `GodotCSTools.csproj` looks
for a couple of assemblies from Godot in that folder.

## Examples

### NodePathAttribute

`NodePathAttribute` gets a node at the given path.

```csharp
using System;
using Godot;
using GodotCSTools;

public class MyNode : Node
{
    [NodePath("Sprite")] // gets the node named "Sprite"
    private AnimatedSprite _sprite = null;

    [NodePath("Sprite/OtherNode")] // relative paths work too!
    private Node2D _otherNode = null;

    public override void _Ready()
    {
        this.SetupNodeTools(); // required to apply the effects of attributes

        _sprite.Play("SomeAnimation"); // _sprite and _otherNode should now contain nodes!
    }
}
```

#### Gotchas

You will receive a warning, `CS0649: field is never assigned to`, if you declare a field as so:

```csharp
[NodePath("spritePath")]
private AnimatedSprite _sprite;
```

This is because the compiler doesn't know that GodotCSTools will be setting the value later. To hide the warning, give
the field a default value of null:

```csharp
[NodePath("spritePath")]
private AnimatedSprite _sprite = null;
```

### ResolveNodeAttribute

`ResolveNodeAttribute` gets a node based on a separate `NodePath` field, which will generally be an export.

```csharp
using System;
using Godot;
using GodotCSTools;

public class MyNode : Node
{
    [Export]
    public NodePath spritePath; // this is set in the editor

    [ResolveNode("spritePath")] // gets the node from the field named "spritePath"
    private AnimatedSprite _sprite = null;

    public override void _Ready()
    {
        this.SetupNodeTools(); // required to apply the effects of attributes

        _sprite.Play("SomeAnimation"); // _sprite should now contain a node!
    }
}
```


#### Gotchas

You will receive a warning, `CS0649: field is never assigned to`, if you declare a field as so:

```csharp
[ResolveNode("spritePath")]
private AnimatedSprite _sprite;
```

This is because the compiler doesn't know that GodotCSTools will be setting the value later. To hide the warning, give
the field a default value of null:

```csharp
[ResolveNode("spritePath")]
private AnimatedSprite _sprite = null;
```

### SignalAttribute

`SignalAttribute` registers a signal on a node or object.

__Warning:__ Arguments are not currently checked on `SignalAttribute` delegates. Right now they exist purely for documentation's sake.


```csharp
using System;
using Godot;
using GodotCSTools;

public class MyNode : Node
{
    [Signal] // registers the signal as "MySignal"
    public delegate void MySignal();

    [Signal("my_other_signal")] // registers the signal as "my_other_signal"
    public delegate void MyOtherSignal();

    public override void _Ready()
    {
        this.SetupNodeTools(); // required to apply the effects of attributes

        // if this were extending Godot.Object instead of a Node type, you might want to use
        // this.SetupObjectTools();

        // You can use the normal Connect() method or the following:
        this.Connect<MySignal>(targetObj, "TargetMethod");
    }

    public void EmitMySignals()
    {
        // You can use the normal EmitSignal() or the following:
        this.EmitSignal<MySignal>();
    }
}
```

## Contributing

Contributions are welcome! Post an issue or make a pull request [here](https://github.com/redxdev/GodotCSTools/issues)!