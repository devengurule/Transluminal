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
    public Camera cam { get; private set; }

    [SerializeField] private List<string> PlayerInputMapScenes = new();
    [SerializeField] private List<string> ShipInputMapScenes = new();
    [SerializeField] private bool devMode;

    private Dictionary<string, SceneData> shipScenesVisited = new Dictionary<string, SceneData>();
    private NavigationController navController;
    private PlayerInput playerInput;
    private GameObject parent;
    private string transportLayer = "TransportCollider";
    private bool interactWithTransport = false;
    
    private ShipSaveData shipSaveData;
    private PlayerSaveData playerSaveData;

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
        if (cam == null)
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
        if (instance == null)
        {
            instance = this;
        }

        playerInput = GetComponent<PlayerInput>();

        playerSaveData.position = GameObject.Find("Player").transform.position;
        playerSaveData.eulerRotation = GameObject.Find("Player").transform.eulerAngles;

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
            eventManager.Subscribe(EventType.DestroySalvage, OnDestroySalvage);
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
            eventManager.Subscribe(EventType.DestroySalvage, OnDestroySalvage);
            eventManager.Unsubscribe(EventType.ArrivedAtHomeNode, OnArrivedHome);
        }
    }
    #endregion

    #region Event Methods

    // Change Input Map when changing scenes
    private void SceneChange(Scene current, Scene next)
    {
        // Get scene camera
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        // Setting input map based on current scene

        if (PlayerInputMapScenes.Contains(SceneController.GetCurrentSceneName()))
        {
            // We are in Player Input Map Scenes

            GameObject.Find("Player").transform.position = playerSaveData.position;
            GameObject.Find("Player").transform.eulerAngles = playerSaveData.eulerRotation;

            // Disable old map
            playerInput.actions.FindActionMap("Ship").Disable();

            // Enable new map
            ChangeInputMap("Player");

        }
        else if (ShipInputMapScenes.Contains(SceneController.GetCurrentSceneName()))
        {
            // We are in Ship Input Map Scenes

            GameObject.Find("PlayerShip").transform.position = shipSaveData.position;
            GameObject.Find("PlayerShip").transform.eulerAngles = shipSaveData.eulerRotation;

            if (!shipScenesVisited.ContainsKey(SceneController.GetCurrentSceneName()))
            {
                // never visited this scene before

                // Reset spawning amount
                GetComponent<SpawnController>().ResetScrapLeftToSpawn();
                GetComponent<SpawnController>().ResetSalvageLeftToSpawn();

                // spawn scrap
                GetComponent<SpawnController>().SpawnScrap(GetComponent<NavigationController>().GetNodeTier());

                // spawn salvage
                ValueTier tier = GetComponent<NavigationController>().GetNodeTier();

                if (GetComponent<SpawnController>() == null) print(1);

                GetComponent<SpawnController>().SpawnSalvage(tier);

                // add new scene to dictionary
                SceneData data = GetSceneData();

                shipScenesVisited.Add(SceneController.GetCurrentSceneName(), data);
            }
            else
            {
                // visiting a ship scene previously visited

                // spawn scrap
                GetComponent<SpawnController>().SpawnExistingScrap(shipScenesVisited[SceneController.GetCurrentSceneName()].scrapSaveDataList);

                // spawn salvage
                GetComponent<SpawnController>().SpawnExistingSalvage(shipScenesVisited[SceneController.GetCurrentSceneName()].salvageSaveDataList);

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
            shipSaveData.position = GameObject.Find("PlayerShip").transform.position;
            shipSaveData.eulerRotation = GameObject.Find("PlayerShip").transform.eulerAngles;

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

            playerSaveData.position = GameObject.Find("Player").transform.position;
            playerSaveData.eulerRotation = GameObject.Find("Player").transform.eulerAngles;

            if (interactWithTransport)
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
        
        shipScenesVisited[SceneController.GetCurrentSceneName()].scrapSaveDataList.Remove(FindObjWithSameData(destroyedObject, shipScenesVisited[SceneController.GetCurrentSceneName()].scrapSaveDataList));

        Destroy(destroyedObject);
    }

    private void OnDestroySalvage(object target)
    {
        GameObject destroyedObject = target as GameObject;

        shipScenesVisited[SceneController.GetCurrentSceneName()].salvageSaveDataList.Remove(FindObjWithSameData(destroyedObject, shipScenesVisited[SceneController.GetCurrentSceneName()].salvageSaveDataList));

        Destroy(destroyedObject);
    }

    private void OnArrivedHome(object target)
    {
        // Clears out dictionary when coming home
        shipScenesVisited.Clear();

        // Add all scrap collected value to money counter
        int totalScrapValue = GetComponent<CollectableManager>().GetCollectedScrapValue();

        GetComponent<MoneyManager>().AddMoney(totalScrapValue);

        GetComponent<CollectableManager>().ResetScrapTotal();
    }

    #endregion

    #region Methods
    private void ChangeInputMap(string mapName)
    {
        playerInput.SwitchCurrentActionMap(mapName);
        playerInput.actions.FindActionMap(mapName).Enable();
    }

    private SceneData GetSceneData()
    {
        GameObject[] scrapObjects = GameObject.FindGameObjectsWithTag("Scrap");
        GameObject[] salvageObjects = GameObject.FindGameObjectsWithTag("Salvage");

        List<ScrapSaveData> scrapDataList = new List<ScrapSaveData>();
        List<SalvageSaveData> salvageDataList = new List<SalvageSaveData>();

        foreach (GameObject obj in scrapObjects)
        {
            ScrapSaveData data;
            data.position = obj.transform.position;
            data.eulerRotation = obj.transform.eulerAngles;
            data.scrapData = obj.GetComponent<ScrapScript>().GetScrapData();
            data.value = obj.GetComponent<ScrapScript>().value;

            scrapDataList.Add(data);
        }

        foreach (GameObject obj in salvageObjects)
        {
            SalvageSaveData data;
            data.position = obj.transform.position;
            data.eulerRotation = obj.transform.eulerAngles;
            data.salvageData = obj.GetComponent<SalvageScript>().GetSalvageData();
            data.value = obj.GetComponent<SalvageScript>().value;
            data.type = obj.GetComponent<SalvageScript>().GetSalvageType();

            salvageDataList.Add(data);
        }

        SceneData sceneData = new();
        sceneData.scrapSaveDataList = scrapDataList;
        sceneData.salvageSaveDataList = salvageDataList;

        return sceneData;
    }

    private ScrapSaveData FindObjWithSameData(GameObject obj, List<ScrapSaveData> data)
    {
        foreach(ScrapSaveData scrapData in data)
        {
            if ((Vector2)obj.transform.position == scrapData.position && obj.transform.eulerAngles == scrapData.eulerRotation)
            {
                return scrapData;
            }
        }
        // Never hit this
        return data[0];
    }

    private SalvageSaveData FindObjWithSameData(GameObject obj, List<SalvageSaveData> data)
    {
        foreach (SalvageSaveData salvageData in data)
        {
            if ((Vector2)obj.transform.position == salvageData.position && obj.transform.eulerAngles == salvageData.eulerRotation)
            {
                return salvageData;
            }
        }
        // Never hit this
        return data[0];
    }

    public bool CanPurchase(int price)
    {
        if (GetComponent<MoneyManager>().GetCurrentMoney() >= price) return true;
        else return false;
    }

    public void MakePurchase(int price)
    {
        GetComponent<MoneyManager>().SubtractMoney(price);
        GetComponent<MoneyManager>().UpdateMoneyCounter();
    }

    #endregion
}