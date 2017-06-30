using UnityEngine;

// https://stackoverflow.com/documentation/unity3d/2137/singletons-in-unity/9564/implementation-using-runtimeinitializeonloadmethodattribute#t=201701211257540950256
sealed class GameManager : MonoBehaviour {

    [System.NonSerialized]
    public Builder Builder;

    // Because of using RuntimeInitializeOnLoadMethod attribute to find/create and
    // initialize the instance, this property is accessible and
    // usable even in Awake() methods.
    public static GameManager Instance {
        get; private set;
    }

    // Thanks to the attribute, this method is executed before any other MonoBehaviour
    // logic in the game.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnRuntimeMethodLoad() {
        var instance = FindObjectOfType<GameManager>();

        if (instance == null)
            instance = new GameObject("Game Director").AddComponent<GameManager>();

        DontDestroyOnLoad(instance);

        Instance = instance;
    }

    // This Awake() will be called immediately after AddComponent() execution
    // in the OnRuntimeMethodLoad(). In other words, before any other MonoBehaviour's
    // in the scene will begin to initialize.
    private void Awake() {
		Builder = GetComponent<Builder>();
    }
}