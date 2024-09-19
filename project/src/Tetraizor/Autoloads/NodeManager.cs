using System.Collections.Generic;
using Godot;

namespace Tetraizor.Autoloads;

public partial class NodeManager : AutoloadBase<NodeManager>
{
    public static T FindNodeOfType<T>(Node nodeToSearch = null) where T : Node
    {
        if (nodeToSearch == null)
        {
            if (Instance != null)
            {
                nodeToSearch = Instance.GetViewport();
            }
            else
            {
                GD.PrintErr("NodeManager: Instance is null.");
                return null;
            }
        }

        if (nodeToSearch is T)
        {
            return nodeToSearch as T;
        }
        else
        {
            foreach (Node childNode in nodeToSearch.GetChildren())
            {
                if (childNode is T)
                {
                    return childNode as T;
                }
                else
                {
                    Node foundNode = FindNodeOfType<T>(childNode);

                    if (foundNode != null)
                    {
                        return foundNode as T;
                    }
                }
            }
        }

        return null;
    }

    public static T[] FindAllNodesOfType<T>(Node nodeToSearch = null) where T : Node
    {
        var foundNodes = new List<T>();

        if (nodeToSearch == null)
        {
            if (Instance != null)
            {
                nodeToSearch = Instance.GetViewport();
            }
            else
            {
                GD.PrintErr("NodeManager: Instance is null.");
                return null;
            }
        }

        if (nodeToSearch is T)
        {
            foundNodes.Add(nodeToSearch as T);
        }
        else
        {
            foreach (Node childNode in nodeToSearch.GetChildren())
            {
                if (childNode is T)
                {
                    foundNodes.Add(childNode as T);
                }
                else
                {
                    T[] foundChildNodes = FindAllNodesOfType<T>(childNode);

                    if (foundChildNodes != null)
                    {
                        foundNodes.AddRange(foundChildNodes);
                    }
                }
            }
        }

        return foundNodes.ToArray();
    }
}