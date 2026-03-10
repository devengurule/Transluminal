using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public EventManager eventManager { get; private set; }

    private PlayerInput playerInput;


    private void Awake()
    {
        if(eventManager == null)
        {
            eventManager = GetComponent<EventManager>();
        }
        if(instance == null)
        {
            instance = this;
        }

        playerInput = GetComponent<PlayerInput>();

        if (isActiveScene("ISDevRoom"))
        {
            ChangeInputMap("Player");
        }
        else if (isActiveScene("OSDevRoom"))
        {
            ChangeInputMap("Ship");
        }
    }


    #region Methods
    private void ChangeInputMap(string mapName)
    {
        playerInput.SwitchCurrentActionMap(mapName);
    }

    private bool isActiveScene(string sceneName)
    {
        return SceneManager.GetActiveScene().name == sceneName;
    }
    #endregion
}
