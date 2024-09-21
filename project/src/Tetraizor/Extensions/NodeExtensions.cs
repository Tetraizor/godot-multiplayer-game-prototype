namespace Tetraizor.Extensions;

using Godot;

public static class NodeExtensions
{
    public static bool IsParent(this Node node, Node target)
    {
        if (node == null || target == null) return false;

        Node nodeToSearch = node.GetParent();
        while (nodeToSearch != null)
        {
            if (nodeToSearch == target) return true;

            nodeToSearch = nodeToSearch.GetParent();
        }

        return false;
    }
}