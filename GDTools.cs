using System;
using System.Reflection;
using Godot;

namespace GodotCSTools
{
    /// <summary>
    /// Contains tools that need to be setup on nodes or objects before using.
    /// </summary>
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
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
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

            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                foreach (var attr in property.GetCustomAttributes())
                {
                    switch (attr)
                    {
                        case ResolveNodeAttribute resolveAttr:
                            ResolveNodeFromPathField(node, property, resolveAttr);
                            break;

                        case NodePathAttribute pathAttr:
                            ResolveNodeFromPath(node, property, pathAttr);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Run tools on the given object.
        /// </summary>
        /// <param name="obj">The object.</param>
        public static void SetupObjectTools(this Godot.Object obj)
        {
        }

        private static void ResolveNodeFromPathField(Node node, FieldInfo field, ResolveNodeAttribute attr)
        {
            var type = node.GetType();
            var targetField = type.GetField(attr.TargetFieldName);
            NodePath path;
            if (targetField == null)
            {
                var targetProperty = type.GetProperty(attr.TargetFieldName);
                if (targetProperty == null)
                {
                    throw new GodotCSToolException($"ResolveNodeAttribute on {type.FullName}.{field.Name} targets nonexistant field or property {attr.TargetFieldName}");
                }

                if (!typeof(NodePath).IsAssignableFrom(targetProperty.PropertyType))
                    throw new GodotCSToolException($"ResolveNodeAttribute on {type.FullName}.{field.Name} targets property {attr.TargetFieldName} which is not a NodePath");

                path = (NodePath)targetProperty.GetValue(node);
            }
            else
            {
                if (!typeof(NodePath).IsAssignableFrom(targetField.FieldType))
                    throw new GodotCSToolException($"ResolveNodeAttribute on {type.FullName}.{field.Name} targets field {attr.TargetFieldName} which is not a NodePath");

                path = (NodePath)targetField.GetValue(node);
            }
            
            AssignPathToField(node, field, path, "ResolveNodeAttribute");
        }

        private static void ResolveNodeFromPathField(Node node, PropertyInfo property, ResolveNodeAttribute attr)
        {
            var type = node.GetType();
            var targetField = type.GetField(attr.TargetFieldName);
            NodePath path;
            if (targetField == null)
            {
                var targetProperty = type.GetProperty(attr.TargetFieldName);
                if (targetProperty == null)
                {
                    throw new GodotCSToolException($"ResolveNodeAttribute on {type.FullName}.{property.Name} targets nonexistant field or property {attr.TargetFieldName}");
                }

                if (!typeof(NodePath).IsAssignableFrom(targetProperty.PropertyType))
                    throw new GodotCSToolException($"ResolveNodeAttribute on {type.FullName}.{property.Name} targets property {attr.TargetFieldName} which is not a NodePath");

                path = (NodePath)targetProperty.GetValue(node);
            }
            else
            {
                if (!typeof(NodePath).IsAssignableFrom(targetField.FieldType))
                    throw new GodotCSToolException($"ResolveNodeAttribute on {type.FullName}.{property.Name} targets field {attr.TargetFieldName} which is not a NodePath");

                path = (NodePath)targetField.GetValue(node);
            }

            AssignPathToProperty(node, property, path, "ResolveNodeAttribute");
        }

        private static void ResolveNodeFromPath(Node node, FieldInfo field, NodePathAttribute attr)
        {
            AssignPathToField(node, field, attr.NodePath, "NodePathAttribute");
        }

        private static void ResolveNodeFromPath(Node node, PropertyInfo property, NodePathAttribute attr)
        {
            AssignPathToProperty(node, property, attr.NodePath, "NodePathAttribute");
        }

        private static void AssignPathToField(Node node, FieldInfo field, string path, string source)
        {
            var value = node.GetNode(path);
            if (value == null)
            {
                GD.Print($"Warning: {source} on {node.GetType().FullName}.{field.Name} - node at \"{path}\" is null");
            }

            try
            {
                field.SetValue(node, value);
            }
            catch (ArgumentException e)
            {
                throw new GodotCSToolException($"{source} on {node.GetType().FullName}.{field.Name} - cannot set value of type {value?.GetType().Name} on field type {field.FieldType.Name}", e);
            }
        }

        private static void AssignPathToProperty(Node node, PropertyInfo property, string path, string source)
        {
            var value = node.GetNode(path);
            if (value == null)
            {
                GD.Print($"Warning: {source} on {node.GetType().FullName}.{property.Name} - node at \"{path}\" is null");
            }

            try
            {
                property.SetValue(node, value);
            }
            catch (ArgumentException e)
            {
                throw new GodotCSToolException($"{source} on {node.GetType().FullName}.{property.Name} - cannot set value of type {value?.GetType().Name} on property type {property.PropertyType.Name}", e);
            }
        }
    }
}
