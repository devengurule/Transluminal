using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    #region Variables
    [SerializeField] private string pauseMenuTag;

    private EventManager eventManager;
    private bool interactWithUI = false;
    private string colliderLayerName = "UICollider";
    private static string availableUIInteractTag = "";
    private bool canInteract;

    public static bool isUIUP { get; private set; } = false;
    #endregion

    #region Unity Methods
    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.PlayerCollidingEnter, OnPlayerCollidingEnter);
            eventManager.Subscribe(EventType.PlayerCollidingExit, OnPlayerCollidingExit);
            eventManager.Subscribe(EventType.Interact, OnInteractPressed);
            eventManager.Subscribe(EventType.PauseOn, PauseGameOn);
            eventManager.Subscribe(EventType.PauseOff, PauseGameOff);
            eventManager.Subscribe(EventType.TransitionOffFinished, TransitionOnFinished);
            eventManager.Subscribe(EventType.TransitionOffFinished, TransitionOffFinished);
        }

        SceneManager.activeSceneChanged += SceneChange;
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.PlayerCollidingEnter, OnPlayerCollidingEnter);
            eventManager.Unsubscribe(EventType.PlayerCollidingExit, OnPlayerCollidingExit);
            eventManager.Unsubscribe(EventType.Interact, OnInteractPressed);
            eventManager.Unsubscribe(EventType.PauseOn, PauseGameOn);
            eventManager.Unsubscribe(EventType.PauseOff, PauseGameOff);
            eventManager.Unsubscribe(EventType.TransitionOffFinished, TransitionOnFinished);
            eventManager.Unsubscribe(EventType.TransitionOffFinished, TransitionOffFinished);
        }
    }
    #endregion

    #region Event Methods

    private void SceneChange(Scene current, Scene next)
    {
        canInteract = false;
    }

    private void OnPlayerCollidingEnter(object target)
    {
        GameObject gameObject = target as GameObject;
        int layerID = LayerMask.NameToLayer(colliderLayerName);

        if (gameObject.layer == layerID)
        {
            interactWithUI = true;
            availableUIInteractTag = gameObject.tag;
            gameObject.transform.Find("Highlight").GetComponent<HighlightScript>().TurnOnHighLight();
            //GameController.instance.GetComponent<GameController>().TurnOnHighLight(gameObject);
        }
    }

    private void OnPlayerCollidingExit(object target)
    {
        GameObject gameObject = target as GameObject;
        int layerID = LayerMask.NameToLayer(colliderLayerName);
          
        if (gameObject.layer == layerID)
        {
            interactWithUI = false;
            availableUIInteractTag = "";
            gameObject.transform.Find("Highlight").GetComponent<HighlightScript>().TurnOffHighLight();
            //GameController.instance.GetComponent<GameController>().TurnOffHighLight(gameObject);
        }
    }

    private void OnInteractPressed(object target)
    {
        if(availableUIInteractTag == "Shop" && !GameController.instance.GetComponent<NavigationController>().IsAtHomeNode() && canInteract)
        {
            eventManager.Publish(EventType.NoShopAccess);
            return;
        }
        else if(interactWithUI && !isUIUP && canInteract)
        {
            TurnOnMenu();

            if (!PauseController.isPaused)
            {
                PauseController.PauseGame();
            }
        }
    }

    private void PauseGameOn(object target)
    {
        // Open Pause Menu
        if (!isUIUP)
        {
            try
            {
                GameObject canvas = GameObject.Find("Canvas");
                GameObject pauseMenuUI = UIController.FindChildWithTag(canvas, pauseMenuTag);

                pauseMenuUI.SetActive(true);
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to Open Pause Menu: {e}");
            }
        }
    }

    private void PauseGameOff(object target)
    {
        // Close Pause Menu
        if (!isUIUP)
        {
            try
            {
                GameObject canvas = GameObject.Find("Canvas");
                GameObject pauseMenuUI = UIController.FindChildWithTag(canvas, pauseMenuTag);

                pauseMenuUI.SetActive(false);
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to Close Pause Menu: {e}");
            }
        }
    }

    private void TransitionOnFinished(object target)
    {
        //canInteract = false;
    }

    private void TransitionOffFinished(object target)
    {
        canInteract = true;
    }

    #endregion

    #region Methods
    // Turn on UI by tag
    public static void TurnOnMenu()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject UIObject = UIController.FindChildWithTag(canvas, UIController.availableUIInteractTag);

        if(UIObject != null)
        {
            UIObject.SetActive(true);
            isUIUP = true;
        }
    }

    // Turn off UI by tag
    public static void TurnOffMenu()
    {
        GameObject canvas = GameObject.Find("Canvas");
        GameObject UIObject = UIController.FindChildWithTag(canvas, UIController.availableUIInteractTag);

        if (UIObject != null)
        {
            UIObject.SetActive(false);
            isUIUP = false;
        }
    }

    // Find child game object of obj parent by tag
    private static GameObject FindChildWithTag(GameObject obj, string tag)
    {
        foreach (Transform child in obj.transform)
        {
            if(child.gameObject.tag == tag)
            {
                return child.gameObject;
            }
        }
        return null;
    }

    public bool CanInteractWithUI()
    {
        return interactWithUI;
    }
    #endregion
}
