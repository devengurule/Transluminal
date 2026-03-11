using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public EventManager eventManager { get; private set; }
    private PlayerInput playerInput;
    private GameObject parent;

    private void Awake()
    {
        parent = transform.parent.gameObject;

        if (instance != null && instance != this)
        {
            Destroy(parent);
            return;
        }

        instance = this;

        DontDestroyOnLoad(parent);

        // Subscribe to changing scene event
        SceneManager.sceneLoaded += OnSceneChange;

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
        if (isActiveScene("ISDevRoom"))
        {
            ChangeInputMap("Player");
        }
        else if (isActiveScene("OSDevRoom"))
        {
            ChangeInputMap("Ship");
        }
    }

    private void OnSceneChange(Scene scene, LoadSceneMode mode)
    {
        //if(GameObject.FindGameObjectsWithTag(parent.tag).Length > 1)
        //{
        //    Destroy(parent);
        //    return;
        //}
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