using Godot;

namespace GodotCSTools
{
    /// <summary>
    /// Contains extensions to the <see cref="Godot.Node"/> class.
    /// </summary>
    public static class NodeExtensions
    {
        /// <summary>
        /// Fetches a node, casting it to the given type.
        /// </summary>
        /// <typeparam name="T">The type of the node.</typeparam>
        /// <param name="self"></param>
        /// <param name="path"></param>
        /// <returns>The requested node on success, null on failure or if the requested node is of the wrong type.</returns>
        public static T GetNode<T>(this Node self, string path) where T : Node
        {
            return self.GetNode(path) as T;
        }

        /// <summary>
        /// Fetches a node, casting it to the given type.
        /// </summary>
        /// <typeparam name="T">The type of the node.</typeparam>
        /// <param name="self"></param>
        /// <param name="path"></param>
        /// <returns>The requested node on success, null on failure or if the requested node is of the wrong type.</returns>
        public static T GetNode<T>(this Node self, NodePath path) where T : Node
        {
            return self.GetNode(path) as T;
        }
    }
}
