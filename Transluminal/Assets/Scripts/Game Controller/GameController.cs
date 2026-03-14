using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameController : MonoBehaviour
{
    #region Variables
    [SerializeField] private List<string> PlayerInputMapScenes = new();
    [SerializeField] private List<string> ShipInputMapScenes = new();

    public static GameController instance;
    public EventManager eventManager { get; private set; }
    private PlayerInput playerInput;
    private GameObject parent;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        // Parent object
        parent = transform.parent != null ? transform.parent.gameObject : gameObject;

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
        if (instance == null)
        {
            instance = this;
        }

        playerInput = GetComponent<PlayerInput>();

        // Subscribe to active scene change event
        SceneManager.activeSceneChanged += SceneChange;
    }

    private void Start()
    {
        if (eventManager != null)
        {
            // Subscribe Events
            eventManager.Subscribe(EventType.Interact, OnInteractPressed);
            eventManager.Subscribe(EventType.Restart, OnRestartGame);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            // Unsubscribe Events
            eventManager.Unsubscribe(EventType.Interact, OnInteractPressed);
            eventManager.Unsubscribe(EventType.Restart, OnRestartGame);
        }
    }
    #endregion

    #region Event Methods

    // Change Input Map when changing scenes
    private void SceneChange(Scene current, Scene next)
    {
        // Setting input map based on current scene

        if (PlayerInputMapScenes.Contains(SceneController.GetCurrentSceneName()))
        {
            // We are in Player Input Map Scenes

            // Disable old map
            playerInput.actions.FindActionMap("Ship").Disable();

            // Enable new map
            ChangeInputMap("Player");

        }
        else if (ShipInputMapScenes.Contains(SceneController.GetCurrentSceneName()))
        {
            // We are in Ship Input Map Scenes

            // Disable old map
            playerInput.actions.FindActionMap("Player").Disable();

            // Setup new map
            ChangeInputMap("Ship");
        }
    }

    private void OnInteractPressed(object target)
    {
        if (ShipInputMapScenes.Contains(SceneController.GetCurrentSceneName()))
        {
            // Inside a ship scene
            SceneController.GoToScene("Floor1Scene");
        }
        else
        {
            print(2);
        }
    }
    private void OnRestartGame(object target)
    {
        SceneController.GoToScene("Floor1Scene");
    }

    #endregion

    #region Methods
    private void ChangeInputMap(string mapName)
    {
        playerInput.SwitchCurrentActionMap(mapName);
    }
    #endregion
}