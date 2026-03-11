using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameController : MonoBehaviour
{
    [SerializeField] private List<string> PlayerInputMapScenes = new();
    [SerializeField] private List<string> ShipInputMapScenes = new();


    public static GameController instance;
    public EventManager eventManager { get; private set; }
    private PlayerInput playerInput;
    private GameObject parent;

    private void Awake()
    {
        // Parent object
        parent = transform.parent.gameObject;

        // Destroy itself if singleton intance variable is not self
        if (instance != null && instance != this)
        {
            Destroy(parent);
            return;
        }

        // Dont destroy parent object when loading into scene
        DontDestroyOnLoad(parent);

        // Set up instance so other objects can get the event manager instance
        if (eventManager == null)
        {
            eventManager = GetComponent<EventManager>();
        }
        if(instance == null)
        {
            instance = this;
        }

        playerInput = GetComponent<PlayerInput>();

        // Setting input map based on current scene

        if (PlayerInputMapScenes.Contains(SceneController.GetCurrentSceneName()))
        {
            // We are in Player Input Map Scenes
            ChangeInputMap("Player");
        }
        else if (ShipInputMapScenes.Contains(SceneController.GetCurrentSceneName()))
        {
            // We are in Ship Input Map Scenes
            ChangeInputMap("Ship");
        }
    }

    #region Event Methods

    #endregion

    #region Methods
    private void ChangeInputMap(string mapName)
    {
        playerInput.SwitchCurrentActionMap(mapName);
    }

    private bool isActiveScene(string sceneName)
    {
        return SceneController.GetCurrentSceneName() == sceneName;
    }
    #endregion
}