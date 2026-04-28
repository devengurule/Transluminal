using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevatorButtonController : MonoBehaviour
{
    #region Variables
    [SerializeField] private List<GameObject> buttonList;

    private EventManager eventManager;
    private float input;
    private int currentIndex;
    private int selectedIndex;
    private bool canInteract = true;
    #endregion

    #region Unity Methods
    private void Start()
    {
        eventManager = GameController.instance.eventManager;
        currentIndex = 0;
        selectedIndex = currentIndex;

        UpdateButton("Hover");
        UpdateButton("Selected");

        // Subscribe to active scene change event
        SceneManager.activeSceneChanged += SceneChange;

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.ScrollVert, OnChangeButton);
            eventManager.Subscribe(EventType.Interact, SelectButton);
            eventManager.Subscribe(EventType.TransitionOnFinished, TransitionOnFinished);
            eventManager.Subscribe(EventType.TransitionOffFinished, TransitionOffFinished);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.ScrollVert, OnChangeButton);
            eventManager.Unsubscribe(EventType.Interact, SelectButton);
            eventManager.Unsubscribe(EventType.TransitionOnFinished, TransitionOnFinished);
            eventManager.Unsubscribe(EventType.TransitionOffFinished, TransitionOffFinished);
        }
    }

    private void OnEnable()
    {
        canInteract = false;
        StartCoroutine(Wait());

        currentIndex = selectedIndex;
        UpdateButton("Hover");
        UpdateButton("Selected");
    }
    private void OnDisable()
    {
        canInteract = false;
    }
    #endregion

    #region Event Methods
    private void SceneChange(Scene current, Scene next)
    {
        // When changing scenes, set the seleted button to the current floor the player is on
        switch (SceneController.GetCurrentSceneName())
        {
            case "Floor1Scene":
                // Set to index 0
                currentIndex = 0;

                break;
            case "Floor2Scene":
                // Set to index 1
                currentIndex = 1;

                break;
            case "Floor3Scene":
                // Set to index 2
                currentIndex = 2;

                break;
            default:
                // Default to index 0
                currentIndex = 0;

                break;
        }
    }

    private void OnChangeButton(object target)
    {
        if (gameObject.activeSelf && canInteract)
        {
            if (target is float input)
            {
                this.input = input;
            }
            if (this.input != 0)
            {
                ChangeButtonLogic();
            }
        }
    }

    private void SelectButton(object target)
    {
        if (gameObject.activeSelf && canInteract)
        {
            UpdateButton("Selected");

            selectedIndex = currentIndex;

            if (SceneController.GetCurrentSceneName() != $"Floor{currentIndex + 1}Scene")
            {
                canInteract = false;
                eventManager.Publish(EventType.TransitionOn);
            }
        }
    }

    private void TransitionOnFinished(object target)
    {
        switch (currentIndex)
        {
            case 0:
                SceneController.GoToScene("Floor1Scene");
                break;
            case 1:
                SceneController.GoToScene("Floor2Scene");
                break;
            case 2:
                SceneController.GoToScene("Floor3Scene");
                break;
        }
    }
    
    private void TransitionOffFinished(object target)
    {
        PauseController.UnPauseGame();
        canInteract = true;
    }
    #endregion

    IEnumerator Wait()
    {
        yield return null;

        canInteract = true;
    }

    #region Methods
    private void ChangeButtonLogic()
    {
        if(currentIndex == 0)
        {
            // At beginning of list

            // If moving up then send selection to the last button
            if (input > 0) currentIndex = buttonList.Count - 1;
            // Else send selection one down
            else currentIndex++;
        }
        else if(currentIndex == buttonList.Count - 1)
        {
            // At end of list

            // If moving down then send selection to the first button
            if (input < 0) currentIndex = 0;
            // Else send selection one up
            else currentIndex--;
        }
        else
        {
            // Somewhere in middle of list

            currentIndex -= (int)input;
        }

        UpdateButton("Hover");
    }

    private void UpdateButton(string objName)
    {
        foreach(GameObject obj in buttonList)
        {
            obj.transform.Find(objName).gameObject.SetActive(false);
        }
        GameObject button = buttonList[currentIndex];
        button.transform.Find(objName).gameObject.SetActive(true);
    }
    #endregion
}