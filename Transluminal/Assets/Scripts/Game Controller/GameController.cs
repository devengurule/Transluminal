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
        if (isActiveScene("ISDevRoom"))
        {
            ChangeInputMap("Player");
        }
        else if (isActiveScene("OSDevRoom"))
        {
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