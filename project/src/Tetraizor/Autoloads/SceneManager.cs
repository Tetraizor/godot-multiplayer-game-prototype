namespace Tetraizor.Autoloads;

using Godot;
using Godot.Collections;

public partial class SceneManager : AutoloadBase<SceneManager>
{
    [Export] private Node _rootNode;
    [Export] private int _initialSceneId = -1;

    [Export] private Dictionary<int, PackedScene> _sceneList;
    private Dictionary<int, PackedScene> SceneList => _sceneList;
    private Dictionary<int, Node> _loadedScenes = new Dictionary<int, Node>();

    public delegate void SceneLoadedEventHandler(int sceneId, Node sceneRoot);
    public SceneLoadedEventHandler SceneLoaded;

    public delegate void SceneUnloadedEventHandler(int sceneId, Node sceneRoot);
    public SceneUnloadedEventHandler SceneUnloaded;

    public override void _Ready()
    {
        base._Ready();

        if (_rootNode == null)
        {
            _rootNode = this;
        }

        if (_initialSceneId != -1)
        {
            LoadScene(_initialSceneId);
        }
    }

    public static void LoadScene(int sceneId)
    {
        if (!Instance._sceneList.ContainsKey(sceneId))
        {
            GD.PrintErr("Scene ID not found.");
            return;
        }

        var scene = Instance._sceneList[sceneId].Instantiate();
        Instance._rootNode.AddChild(scene);

        Instance.SceneLoaded?.Invoke(sceneId, scene);
        Instance._loadedScenes.Add(sceneId, scene);
    }

    public static void UnloadScene(int sceneId)
    {
        if (!Instance._sceneList.ContainsKey(sceneId))
        {
            GD.PrintErr($"Scene with ID {sceneId} not found.");
            return;
        }

        if (!Instance._loadedScenes.ContainsKey(sceneId))
        {
            GD.PrintErr($"Scene with ID {sceneId} not loaded.");
            return;
        }

        var scene = Instance._loadedScenes[sceneId];

        Instance.SceneUnloaded?.Invoke(sceneId, scene);

        scene.QueueFree();
        Instance._loadedScenes.Remove(sceneId);
    }
}