using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEditor;

public class GameController : MonoBehaviour
{
    #region Variables
    public static GameController instance;
    public EventManager eventManager { get; private set; }


    [SerializeField] private List<string> PlayerInputMapScenes = new();
    [SerializeField] private List<string> ShipInputMapScenes = new();
    [SerializeField] private bool devMode;


    private NavigationController navController;
    private PlayerInput playerInput;
    private GameObject parent;
    private string transportLayer = "TransportCollider";
    private bool interactWithTransport = false;
    


    #endregion

    #region Unity Methods
    private void Awake()
    {
        navController = GetComponent<NavigationController>();

        // Parent object
        parent = transform.parent != null ? transform.parent.gameObject : gameObject;

        // Destroy itself if singleton intance variable is not self
        if (instance != null && instance != this)
        {
            if(parent != null) Destroy(parent);
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
            eventManager.Subscribe(EventType.PlayerCollidingEnter, OnPlayerEnterCollide);
            eventManager.Subscribe(EventType.PlayerCollidingExit, OnPlayerExitCollide);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            // Unsubscribe Events
            eventManager.Unsubscribe(EventType.Interact, OnInteractPressed);
            eventManager.Unsubscribe(EventType.Restart, OnRestartGame);
            eventManager.Subscribe(EventType.PlayerCollidingEnter, OnPlayerEnterCollide);
            eventManager.Subscribe(EventType.PlayerCollidingExit, OnPlayerExitCollide);
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

            GetComponent<ScrapSpawnController>().SpawnScrap();
        }
    }

    private void OnInteractPressed(object target)
    {
        // Switch to floor1scene if inside a ship scene
        if (ShipInputMapScenes.Contains(SceneController.GetCurrentSceneName()))
        {
            // Inside a ship scene
            if (devMode)
            {
                SceneController.GoToScene("ISDevRoom");
            }
            else
            {
                SceneController.GoToScene("Floor1Scene");
            }
        }
        else
        {
            // Not inside a ship scene
            if(interactWithTransport)
            {
                if (!navController.IsAtHomeNode())
                {
                    SceneController.GoToScene(navController.GetCurrentShipScene());
                }
            }
        }
    }
    private void OnRestartGame(object target)
    {
        SceneController.GoToScene("Floor1Scene");
    }

    private void OnPlayerEnterCollide(object target)
    {
        GameObject gameObject = target as GameObject;
        int layerID = LayerMask.NameToLayer(transportLayer);

        if (gameObject.layer == layerID)
        {
            interactWithTransport = true;
        }
    }
    private void OnPlayerExitCollide(object target)
    {
        GameObject gameObject = target as GameObject;
        int layerID = LayerMask.NameToLayer(transportLayer);

        if (gameObject.layer == layerID)
        {
            interactWithTransport = false;
        }
    }

    #endregion

    #region Methods
    private void ChangeInputMap(string mapName)
    {
        playerInput.SwitchCurrentActionMap(mapName);
        playerInput.actions.FindActionMap(mapName).Enable();
    }
    #endregion
}