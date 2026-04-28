using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class GameController : MonoBehaviour
{
    #region Variables
    public static GameController instance;
    public EventManager eventManager { get; private set; }
    public Camera cam { get; private set; }
    public GameObject player { get; private set; }

    [SerializeField] private List<string> PlayerInputMapScenes = new();
    [SerializeField] private List<string> ShipInputMapScenes = new();
    [SerializeField] private Vector2 playerHidingPos;
    [SerializeField] private GameObject healthObject;
    [SerializeField] private GameObject shipHUDObject;
    [SerializeField] private float alphaFadeSpeed;

    private Dictionary<string, SceneData> shipScenesVisited = new Dictionary<string, SceneData>();
    private NavigationController navController;
    private PlayerInput playerInput;
    private GameObject parent;
    private string transportLayer = "TransportCollider";
    private string closetLayer = "ClosetCollider";
    private bool interactWithTransport = false;
    private bool interactWithCloset = false;
    private bool disableClosets = false;
    private object closetObject;
    private bool isHiding;
    private Vector2 playerLastPos;
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
            eventManager.Subscribe(EventType.PlayerHiding, PlayerOnHiding);
            eventManager.Subscribe(EventType.SpawnCreature, DisableCloset);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            // Unsubscribe Events
            eventManager.Unsubscribe(EventType.Interact, OnInteractPressed);
            eventManager.Unsubscribe(EventType.Restart, OnRestartGame);
            eventManager.Unsubscribe(EventType.PlayerCollidingEnter, OnPlayerEnterCollide);
            eventManager.Unsubscribe(EventType.PlayerCollidingExit, OnPlayerExitCollide);
            eventManager.Unsubscribe(EventType.DestroyScrap, OnDestroyScrap);
            eventManager.Unsubscribe(EventType.DestroySalvage, OnDestroySalvage);
            eventManager.Unsubscribe(EventType.ArrivedAtHomeNode, OnArrivedHome);
            eventManager.Unsubscribe(EventType.PlayerHiding, PlayerOnHiding);
            eventManager.Unsubscribe(EventType.SpawnCreature, DisableCloset);
        }
    }
    #endregion

    #region Event Methods

    // Change Input Map when changing scenes
    private void SceneChange(Scene current, Scene next)
    {
        eventManager.Publish(EventType.TransitionOff);

        // Get player in scene
        player = GameObject.Find("Player");

        // Get scene camera
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        // Setting input map based on current scene

        if (PlayerInputMapScenes.Contains(SceneController.GetCurrentSceneName()))
        {
            // We are in Player Input Map Scenes

            healthObject.SetActive(true);
            shipHUDObject.SetActive(false);

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

            healthObject.SetActive(false);
            shipHUDObject.SetActive(true);

            // Setting Position and Rotation Based on Saved Data

            if (!shipScenesVisited.ContainsKey(SceneController.GetCurrentSceneName()))
            {
                // never visited this scene before

                SetShipPos();

                // Reset spawning amount
                GetComponent<SpawnController>().ResetScrapLeftToSpawn();
                GetComponent<SpawnController>().ResetSalvageLeftToSpawn();

                ValueTier tier = GetComponent<NavigationController>().GetNodeTier();

                // spawn scrap
                GetComponent<SpawnController>().SpawnScrap(tier);

                // spawn salvage
                
                float chanceForAlien = GetComponent<NavigationController>().GetChanceForAlien();

                GetComponent<SpawnController>().SpawnSalvage(tier, chanceForAlien);

                // add new scene to dictionary
                SceneData data = GetSceneData();

                shipScenesVisited.Add(SceneController.GetCurrentSceneName(), data);
            }
            else
            {
                // visiting a ship scene previously visited

                if(SceneController.GetCurrentSceneName() != shipSaveData.sceneName)
                {
                    // Visited a new node
                    SetShipPos();
                }
                else
                {
                    SetShipPos(shipSaveData);
                }


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
            shipSaveData.sceneName = SceneController.GetCurrentSceneName();

            SceneController.GoToScene("Floor1Scene");
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
                else
                {
                    eventManager.Publish(EventType.NoHelmAccess);
                }
            }
            else if (interactWithCloset && !GameController.instance.GetComponent<UIController>().CanInteractWithUI())
            {
                isHiding = true;
                eventManager.Publish(EventType.OnEnterCloset, closetObject);
            }
            else if(isHiding)
            {
                isHiding = false;
                eventManager.Publish(EventType.OnExitCloset, closetObject);
                closetObject = null;
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
        int transportLayerID = LayerMask.NameToLayer(transportLayer);
        int closetLayerID = LayerMask.NameToLayer(closetLayer);

        if (gameObject.layer == transportLayerID)
        {
            interactWithTransport = true;
            TurnOnHighLight(gameObject);
        }
        else if (gameObject.layer == closetLayerID && !disableClosets)
        {
            interactWithCloset = true;
            closetObject = target;
            TurnOnHighLight(gameObject);
        }
    }

    private void OnPlayerExitCollide(object target)
    {
        GameObject gameObject = target as GameObject;
        int transportLayerID = LayerMask.NameToLayer(transportLayer);
        int closetLayerID = LayerMask.NameToLayer(closetLayer);

        if (gameObject.layer == transportLayerID)
        {
            interactWithTransport = false;
            TurnOffHighLight(gameObject);
        }
        else if(gameObject.layer == closetLayerID)
        {
            interactWithCloset = false;
            TurnOffHighLight(gameObject);
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
    }

    private void PlayerOnHiding(object target)
    {
        if(target is Vector2 lastPlayerPos)
        {
            playerLastPos = lastPlayerPos;
        }
    }

    private void DisableCloset(object target)
    {
        disableClosets = true;
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
            data.scale = obj.transform.localScale.x;

            data.value = obj.GetComponent<ScrapScript>().value;
            data.sprite = obj.GetComponent<SpriteRenderer>().sprite;
            

            scrapDataList.Add(data);
        }

        foreach (GameObject obj in salvageObjects)
        {
            SalvageSaveData data;

            data.position = obj.transform.position;
            data.eulerRotation = obj.transform.eulerAngles;
            data.scale = obj.transform.localScale.x;

            data.value = obj.GetComponent<SalvageScript>().value;
            data.alienData = obj.GetComponent<SalvageScript>().GetAlienData();


            data.salvageData = obj.GetComponent<SalvageScript>().GetSalvageData();


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

    public int GetScrapValue()
    {
        return GetComponent<CollectableManager>().GetCollectedScrapValue();
    }

    public int GetSalvageValue()
    {
        return GetComponent<CollectableManager>().GetCollectedSalvageValue();
    }

    private void SetShipPos()
    {
        GameObject.Find("PlayerShip").transform.position = GameObject.Find("ShipSpawn").transform.position;
        GameObject.Find("PlayerShip").transform.eulerAngles = Vector3.zero;
        GameObject.Find("Main Camera").transform.position = GameObject.Find("PlayerShip").transform.position;
        GameObject.Find("Main Camera").transform.eulerAngles = Vector3.zero;
    }

    private void SetShipPos(ShipSaveData data)
    {
        GameObject.Find("PlayerShip").transform.position = shipSaveData.position;
        GameObject.Find("PlayerShip").transform.eulerAngles = shipSaveData.eulerRotation;
        GameObject.Find("Main Camera").transform.position = shipSaveData.position;
        GameObject.Find("Main Camera").transform.eulerAngles = shipSaveData.eulerRotation;
    }

    public Vector2 PlayerHidingPos()
    {
        return playerHidingPos;
    }

    public bool IsHiding()
    {
        return isHiding;
    }

    public Vector2 GetPlayerPos()
    {
        if (isHiding)
        {
            return new Vector2(playerLastPos.x, playerLastPos.y - 1.7f);
        }

        return new Vector2(player.transform.position.x, player.transform.position.y - 1.7f);
    }

    public void TurnOnHighLight(GameObject gameObject)
    {
        StartCoroutine(FadeInAlpha(gameObject));
    }
    public void TurnOffHighLight(GameObject gameObject)
    {
        StartCoroutine(FadeOutAlpha(gameObject));
    }

    #endregion

    #region IEnumerator Methods
    IEnumerator FadeInAlpha(GameObject gameObject)
    {
        GameObject childObject = null;

        if(gameObject != null) childObject = gameObject.transform.Find("Highlight").gameObject;
        if (childObject != null)
        {
            SpriteRenderer childSpriteRenderer = childObject.GetComponent<SpriteRenderer>();

            float alpha = childSpriteRenderer.color.a;

            while (!Mathf.Approximately(alpha, 1))
            {
                alpha = Mathf.MoveTowards(alpha, 1, alphaFadeSpeed);
                if(childSpriteRenderer != null) childSpriteRenderer.color = new Vector4(1, 1, 1, alpha);

                yield return null;
            }

            if (childSpriteRenderer != null) childSpriteRenderer.color = new Vector4(1, 1, 1, 1);
        }
    }

    IEnumerator FadeOutAlpha(GameObject gameObject)
    {
        GameObject childObject = null;

        if (gameObject != null) childObject = gameObject.transform.Find("Highlight").gameObject;
        if (childObject != null)
        {
            SpriteRenderer childSpriteRenderer = childObject.GetComponent<SpriteRenderer>();

            float alpha = childSpriteRenderer.color.a;

            while (!Mathf.Approximately(alpha, 0))
            {
                alpha = Mathf.MoveTowards(alpha, 0, alphaFadeSpeed);
                if(childSpriteRenderer != null) childSpriteRenderer.color = new Vector4(1, 1, 1, alpha);

                yield return null;
            }

            if (childSpriteRenderer != null) childSpriteRenderer.color = new Vector4(1, 1, 1, 0);
        }
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(playerHidingPos, 0.5f);
    }
    #endregion
}