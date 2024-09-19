using Godot;

namespace Tetraizor.Autoloads;

public abstract partial class AutoloadBase<T> : Node where T : AutoloadBase<T>
{
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                if (typeof(T) == typeof(NodeManager))
                {
                    return null;
                }

                var instance = NodeManager.FindNodeOfType<T>();

                if (instance != null)
                {
                    _instance = instance;
                }
                else
                {
                    GD.PrintErr($"{typeof(T).Name}: Instance is null.");
                    throw new System.NullReferenceException();
                }
            }

            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    private static T _instance;

    public override void _Ready()
    {
        if (_instance != null)
        {
            GD.PrintErr($"{GetType().Name}: Instance already exists under {_instance.GetParent().Name} as {_instance.Name}.");
            GD.PrintErr($"Current one exists under {GetParent().Name} as {Name}.");
            QueueFree();
            return;
        }
        GD.Print($"{GetType().Name}: Instance is ready.");

        _instance = (T)this;
    }

    public override void _ExitTree()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}