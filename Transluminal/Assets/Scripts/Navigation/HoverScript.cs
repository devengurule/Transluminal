using UnityEngine;
using UnityEngine.SceneManagement;

public class HoverScript : MonoBehaviour
{
    
    #region Variables

    private EventManager eventManager;
    private bool isHovered;
    private bool isSelected;
    private bool isCurrentNode;

    #endregion

    #region Unity Methods
    private void Start()
    {
        // Subscribe to active scene change event
        SceneManager.activeSceneChanged += SceneChange;

        if (isCurrentNode)
        {
            GameObject selectObject = transform.Find("Selected").gameObject;
            selectObject.SetActive(true);
            isSelected = true;
        }

        eventManager = GameController.instance.eventManager;

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.Interact, OnGameInteract);
            eventManager.Subscribe(EventType.NodeSelected, OnNodeSelected);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.Interact, OnGameInteract);
            eventManager.Unsubscribe(EventType.NodeSelected, OnNodeSelected);
        }
    }

    #endregion

    private void SceneChange(Scene current, Scene next)
    {
        if (isCurrentNode)
        {
            GameObject selectObject = transform.Find("Selected").gameObject;
            selectObject.SetActive(true);
            isSelected = true;
        }
    }


    private void OnGameInteract(object target)
    {
        if (isHovered && !isCurrentNode)
        {
            if (!isSelected)
            {
                GameObject selectObject = transform.Find("Selected").gameObject;
                selectObject.SetActive(true);
                isSelected = true;

                eventManager.Publish(EventType.NodeSelected, gameObject);
            }
            else if (isSelected)
            {
                GameObject selectObject = transform.Find("Selected").gameObject;
                selectObject.SetActive(false);
                isSelected = false;

                eventManager.Publish(EventType.NodeDeselected, gameObject);
            }
        }
    }

    private void OnNodeSelected(object target)
    {
        // Deselect Node if another node is selected and this node is not the current node
        if((GameObject)target != gameObject && !isCurrentNode)
        {
            GameObject selectObject = transform.Find("Selected").gameObject;
            selectObject.SetActive(false);
            isSelected = false;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "NavSelector")
        {
            if (!isCurrentNode)
            {
                if (!isSelected)
                {
                    GameObject hoverObject = transform.Find("Hovered").gameObject;
                    hoverObject.SetActive(true);

                    isHovered = true;

                    collision.gameObject.GetComponent<SelectorMovement>().SetMove(true);
                }
                else
                {
                    collision.gameObject.GetComponent<SelectorMovement>().SetMove(false);
                    collision.gameObject.transform.position = gameObject.transform.position;
                }
            }
            else
            {
                collision.gameObject.GetComponent<SelectorMovement>().SetMove(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "NavSelector")
        {
            if (!isCurrentNode)
            {
                GameObject hoverObject = transform.Find("Hovered").gameObject;
                hoverObject.SetActive(false);
                isHovered = false;

                if (isSelected)
                {
                    GameObject selectObject = transform.Find("Selected").gameObject;
                    selectObject.SetActive(false);
                    isSelected = false;

                    eventManager.Publish(EventType.NodeDeselected, gameObject);
                }
            }
        }
    }
    public void SetIsCurrentNode(bool input)
    {
        isCurrentNode = input;
    }

    public void Reset()
    {
        // Reset Hover Sprite
        GameObject hoverObject = transform.Find("Hovered").gameObject;
        hoverObject.SetActive(false);
        isHovered = false;

        // Reset Selected Sprite
        GameObject selectObject = transform.Find("Selected").gameObject;
        selectObject.SetActive(false);
        isSelected = false;
    }
}
