using UnityEngine;

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
        if(isCurrentNode)
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "NavSelector")
        {
            if (!isSelected && !isCurrentNode)
            {
                GameObject hoverObject = transform.Find("Hovered").gameObject;
                hoverObject.SetActive(true);
            }
            isHovered = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "NavSelector")
        {
            if (!isCurrentNode)
            {
                GameObject hoverObject = transform.Find("Hovered").gameObject;
                hoverObject.SetActive(false);
                isHovered = false;

            }
        }
    }
    public void SetIsCurrentNode(bool input)
    {
        isCurrentNode = input;
    }
}
