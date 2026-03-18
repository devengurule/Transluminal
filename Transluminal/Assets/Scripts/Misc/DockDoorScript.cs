using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class DockDoorScript : MonoBehaviour
{
    [SerializeField] private Sprite doorSprite;
    [SerializeField] private Sprite shopSprite;

    private void Start()
    {
        if (GameController.instance != null)
        {
            // Do inital check for if we are at home node
            if (GameController.instance.GetComponent<NavigationController>().IsAtHomeNode())
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = shopSprite;
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = doorSprite;
            }
        }
    }
}
