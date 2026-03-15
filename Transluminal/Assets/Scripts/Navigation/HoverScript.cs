using UnityEngine;

public class HoverScript : MonoBehaviour
{
    
    #region Variables

    private EventManager eventManager;
    private bool isHovered;
    private bool isSelected;

    #endregion

    #region Unity Methods
    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.Interact, OnGameInteract);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.Interact, OnGameInteract);
        }
    }

    #endregion

    private void OnGameInteract(object target)
    {
        if (isHovered)
        {
            if (!isSelected)
            {
                GameObject selectObject = transform.Find("Selected").gameObject;
                selectObject.SetActive(true);
                isSelected = true;
            }
            else if (isSelected)
            {
                GameObject selectObject = transform.Find("Selected").gameObject;
                selectObject.SetActive(false);
                isSelected = false;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "NavSelector")
        {
            if (!isSelected)
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
            GameObject hoverObject = transform.Find("Hovered").gameObject;
            hoverObject.SetActive(false);
            isHovered = false;
        }
    }
}
