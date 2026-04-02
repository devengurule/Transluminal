using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;

public class ScannerSelectionController : MonoBehaviour
{
    [SerializeField] GameObject currentSelection;
    private EventManager eventManager;
    private ScannerSelectorData scannerSelectorData;
    private Vector2 inputVector;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        currentSelection.transform.Find("Selected").gameObject.SetActive(true);
        
        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.Move, ChangeSelection);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.Move, ChangeSelection);
        }
    }

    private void ChangeSelection(object target)
    {
        if(target is Vector2 inputVector && gameObject.activeSelf && GetComponent<ScannerController>().CanChangeSelection())
        {
            this.inputVector = inputVector;

            UpdateSelection();
        }
    }

    private void UpdateSelection()
    {
        scannerSelectorData = currentSelection.GetComponent<ScannerSelectorData>();

        if(inputVector == Vector2.right)
        {
            if(scannerSelectorData.RightSelect != null)
            {
                currentSelection.transform.Find("Selected").gameObject.SetActive(false);
                currentSelection = scannerSelectorData.RightSelect.gameObject;
                currentSelection.transform.Find("Selected").gameObject.SetActive(true);
            }
        }
        else if(inputVector == Vector2.up)
        {
            if (scannerSelectorData.UpSelect != null)
            {
                currentSelection.transform.Find("Selected").gameObject.SetActive(false);
                currentSelection = scannerSelectorData.UpSelect.gameObject;
                currentSelection.transform.Find("Selected").gameObject.SetActive(true);
            }
        }
        else if (inputVector == Vector2.left)
        {
            if (scannerSelectorData.LeftSelect != null)
            {
                currentSelection.transform.Find("Selected").gameObject.SetActive(false);
                currentSelection = scannerSelectorData.LeftSelect.gameObject;
                currentSelection.transform.Find("Selected").gameObject.SetActive(true);
            }
        }
        else if (inputVector == Vector2.down)
        {
            if (scannerSelectorData.DownSelect != null)
            {
                currentSelection.transform.Find("Selected").gameObject.SetActive(false);
                currentSelection = scannerSelectorData.DownSelect.gameObject;
                currentSelection.transform.Find("Selected").gameObject.SetActive(true);
            }
        }
    }

    public GameObject GetCurrentSelection()
    {
        return currentSelection;
    }
}
