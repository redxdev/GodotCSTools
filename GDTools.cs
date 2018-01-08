using System;
using System.Reflection;
using Godot;

namespace GodotCSTools
{
    public static class GDTools
    {
        /// <summary>
        /// Run tools on the given node. This calls <see cref="SetupObjectTools(Godot.Object)"/>.
        /// </summary>
        /// <remarks>
        /// This will fill in fields and register signals as per attributes such as <see cref="NodePathAttribute"/> and <see cref="SignalAttribute"/>.
        /// </remarks>
        /// <param name="node">The node.</param>
        public static void SetupNodeTools(this Node node)
        {
            SetupObjectTools(node);

            var type = node.GetType();
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
            {
                foreach (var attr in field.GetCustomAttributes())
                {
                    switch (attr)
                    {
                        case ResolveNodeAttribute resolveAttr:
                            ResolveNodeFromPathField(node, field, resolveAttr);
                            break;

                        case NodePathAttribute pathAttr:
                            ResolveNodeFromPath(node, field, pathAttr);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Run tools on the given object.
        /// </summary>
        /// <remarks>
        /// This will register signals as per <see cref="SignalAttribute"/>.
        /// </remarks>
        /// <param name="obj">The object.</param>
        public static void SetupObjectTools(this Godot.Object obj)
        {
            var type = obj.GetType();
            foreach (var nested in type.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic))
            {
                foreach (var attr in nested.GetCustomAttributes())
                {
                    switch (attr)
                    {
                        case SignalAttribute signalAttr:
                            DefineSignalFromType(obj, nested, signalAttr);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Emit a signal based on a <see cref="SignalAttribute"/> delegate.
        /// </summary>
        /// <remarks>
        /// This is recommended over <see cref="Godot.Object.EmitSignal(string, object[])"/> when using <see cref="SignalAttribute"/> (though they can be mixed) as it retrieves the signal
        /// name from the delegate's attribute. If you change the name of the signal in the attribute, you will not have to change anything when calling this function.
        /// </remarks>
        /// <typeparam name="Signal">The delegate type.</typeparam>
        /// <param name="obj">The object to emit from.</param>
        /// <param name="args">Arguments passed to the signal.</param>
        public static void EmitSignal<Signal>(this Godot.Object obj, params object[] args)
        {
            if (!typeof(Signal).IsSubclassOf(typeof(Delegate)))
            {
                throw new GodotCSToolException($"Invalid EmitSignal<T>() call, T was {typeof(Signal).FullName}");
            }

            var attr = typeof(Signal).GetCustomAttribute<SignalAttribute>();
            if (attr == null)
            {
                throw new GodotCSToolException($"Delegate {typeof(Signal).FullName} does not have a SignalAttribute");
            }

            var name = string.IsNullOrEmpty(attr.SignalName) ? typeof(Signal).Name : attr.SignalName;
            obj.EmitSignal(name, args);
        }

        /// <summary>
        /// Connect a signal based on a <see cref="SignalAttribute"/> delegate.
        /// </summary>
        /// <remarks>
        /// This is recommended over <see cref="Godot.Object.Connect(string, Godot.Object, string, object[], int)"/> when using <see cref="SignalAttribute"/> (though they can be mixed)
        /// as it retrieves the signal name from the delegate's attribute. If you change the name of the signal in the attribute, you will not have to change anything when calling this
        /// function.
        /// </remarks>
        /// <typeparam name="Signal">The delegate type.</typeparam>
        /// <param name="obj">The object emitting the signal.</param>
        /// <param name="target">The target object to receive the signal.</param>
        /// <param name="method">The name of the target method.</param>
        /// <param name="binds"></param>
        /// <param name="flags"></param>
        public static void Connect<Signal>(this Godot.Object obj, Godot.Object target, string method, object[] binds = null, int flags = 0)
        {
            if (!typeof(Signal).IsSubclassOf(typeof(Delegate)))
            {
                throw new GodotCSToolException($"Invalid EmitSignal<T>() call, T was {typeof(Signal).FullName}");
            }

            var attr = typeof(Signal).GetCustomAttribute<SignalAttribute>();
            if (attr == null)
            {
                throw new GodotCSToolException($"Delegate {typeof(Signal).FullName} does not have a SignalAttribute");
            }

            var name = string.IsNullOrEmpty(attr.SignalName) ? typeof(Signal).Name : attr.SignalName;
            obj.Connect(name, target, method, binds, flags);
        }

        private static void ResolveNodeFromPathField(Node node, FieldInfo field, ResolveNodeAttribute attr)
        {
            var type = node.GetType();
            var targetField = type.GetField(attr.TargetFieldName);
            if (targetField == null)
                throw new GodotCSToolException($"ResolveNodeAttribute on {type.FullName}.{field.Name} targets nonexistant field {attr.TargetFieldName}");

            if (!typeof(NodePath).IsAssignableFrom(targetField.FieldType))
                throw new GodotCSToolException($"ResolveNodeAttribute on {type.FullName}.{field.Name} targets field {attr.TargetFieldName} which is not a NodePath");

            var path = (NodePath)targetField.GetValue(node);
            AssignPathToField(node, field, path, "ResolveNodeAttribute");
        }

        private static void ResolveNodeFromPath(Node node, FieldInfo field, NodePathAttribute attr)
        {
            AssignPathToField(node, field, attr.NodePath, "NodePathAttribute");
        }

        private static void AssignPathToField(Node node, FieldInfo field, string path, string source)
        {
            var value = node.GetNode(path);
            try
            {
                field.SetValue(node, value);
            }
            catch (ArgumentException e)
            {
                throw new GodotCSToolException($"{source} on {node.GetType().FullName}.{field.Name} - cannot set value of type {value?.GetType().Name} on field type {field.FieldType.Name}", e);
            }
        }

        private static void DefineSignalFromType(Godot.Object obj, Type type, SignalAttribute attr)
        {
            var name = string.IsNullOrEmpty(attr.SignalName) ? type.Name : attr.SignalName;
            obj.AddUserSignal(name, attr.Arguments);
        }
    }
}
