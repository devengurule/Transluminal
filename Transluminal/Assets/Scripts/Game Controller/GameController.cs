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

    private Dictionary<string, List<ScrapData>> shipScenesVisited = new Dictionary<string, List<ScrapData>>();
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
            eventManager.Subscribe(EventType.DestroyScrap, OnDestroyScrap);
            eventManager.Subscribe(EventType.ArrivedAtHomeNode, OnArrivedHome);
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
            eventManager.Subscribe(EventType.DestroyScrap, OnDestroyScrap);
            eventManager.Unsubscribe(EventType.ArrivedAtHomeNode, OnArrivedHome);
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

            if (!shipScenesVisited.ContainsKey(SceneController.GetCurrentSceneName()))
            {
                // never visited this scene before

                GetComponent<ScrapSpawnController>().ResetScrapLefToSpawn();

                // spawn scrap
                GetComponent<ScrapSpawnController>().SpawnScrap();

                // add new scene to dictionary
                shipScenesVisited.Add(SceneController.GetCurrentSceneName(), GetListOfScrapData());
            }
            else
            {
                // visiting a ship scene previously visited

                // spawn scrap
                GetComponent<ScrapSpawnController>().SpawnExistingScrap(shipScenesVisited[SceneController.GetCurrentSceneName()]);

            }

            // Disable old map
            playerInput.actions.FindActionMap("Player").Disable();

            // Setup new map
            ChangeInputMap("Ship");
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

    private void OnDestroyScrap(object target)
    {
        GameObject destroyedObject = target as GameObject;

        shipScenesVisited[SceneController.GetCurrentSceneName()].Remove(FindObjWithSameData(destroyedObject, shipScenesVisited[SceneController.GetCurrentSceneName()]));

        Destroy(destroyedObject);
    }

    private void OnArrivedHome(object target)
    {
        // Clears out dictionary when coming home
        shipScenesVisited.Clear();
    }

    #endregion

    #region Methods
    private void ChangeInputMap(string mapName)
    {
        playerInput.SwitchCurrentActionMap(mapName);
        playerInput.actions.FindActionMap(mapName).Enable();
    }

    private List<ScrapData> GetListOfScrapData()
    {
        GameObject[] scrapObjects = GameObject.FindGameObjectsWithTag("Scrap");
        
        List<ScrapData> scrapDataList = new List<ScrapData>();

        foreach (GameObject obj in scrapObjects)
        {
            ScrapData data;
            data.position = obj.transform.position;
            data.eulerRotation = obj.transform.eulerAngles;

            scrapDataList.Add(data);
        }

        return scrapDataList;
    }

    private ScrapData FindObjWithSameData(GameObject obj, List<ScrapData> data)
    {
        foreach(ScrapData scrapData in data)
        {
            if ((Vector2)obj.transform.position == scrapData.position && obj.transform.eulerAngles == scrapData.eulerRotation)
            {
                return scrapData;
            }
        }
        // Never hit this
        return data[0];
    }

    #endregion
}