# Godot C# Tools

This is a small library of utility functions that make working in C# when using [Godot](https://godotengine.org/) much easier.

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