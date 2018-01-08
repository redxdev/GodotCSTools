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
        public static T GetNode<T>(this Node self, string path)
            where T : Node
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
        public static T GetNode<T>(this Node self, NodePath path)
            where T : Node
        {
            return self.GetNode(path) as T;
        }

        /// <summary>
        /// Finds a descendant of this node. See <see cref="Godot.Node.FindNode(string, bool, bool)"/>.
        /// </summary>
        /// <remarks>
        /// This does not search by the type - it only helps with casting the result of <see cref="Godot.Node.FindNode(string, bool, bool)"/>.
        /// </remarks>
        /// <typeparam name="T">The type of the node to find.</typeparam>
        /// <param name="self"></param>
        /// <param name="mask"></param>
        /// <param name="recursive"></param>
        /// <param name="owned"></param>
        /// <returns>The requested node on success, null on failure or if the requested node is of the wrong type.</returns>
        public static T FindNode<T>(this Node self, string mask, bool recursive = true, bool owned = true)
            where T : Node
        {
            return self.FindNode(mask, recursive, owned) as T;
        }

        /// <summary>
        /// Get a child node by its index, casted to the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the child node.</typeparam>
        /// <param name="self"></param>
        /// <param name="idx"></param>
        /// <returns>The requested node on success, null on failure or if the requested node is of the wrong type.</returns>
        public static T GetChild<T>(this Node self, int idx)
            where T : Node
        {
            return self.GetChild(idx) as T;
        }
    }
}
