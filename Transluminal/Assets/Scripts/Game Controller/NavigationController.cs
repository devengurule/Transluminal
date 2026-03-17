using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationController : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject currentNode;
    [SerializeField] private GameObject lineObject;
    [SerializeField] private GameObject toolTipUIObject;

    private EventManager eventManager;
    private GameObject targetNode;
    private string currentShipScene;
    private bool atHomeNode;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        currentNode.GetComponent<HoverScript>().SetIsCurrentNode(true);

        // Update current ship scene
        if (currentNode.GetComponent<NodeScript>().IsHomeNode())
        {
            atHomeNode = true;
        }
        else
        {
            atHomeNode = false;
            currentShipScene = currentNode.GetComponent<NodeScript>().TargetShipScene().name;
        }
    }
    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        if(atHomeNode)
        {
            eventManager.Publish(EventType.ArrivedAtHomeNode);
        }

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.NodeSelected, OnNodeSelect);
            eventManager.Subscribe(EventType.NodeDeselected, OnNodeDeselect);
            eventManager.Subscribe(EventType.Confirm, OnConfirmTravel);
        }

    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.NodeSelected, OnNodeSelect);
            eventManager.Unsubscribe(EventType.NodeDeselected, OnNodeDeselect);
            eventManager.Unsubscribe(EventType.Confirm, OnConfirmTravel);
        }
    }
    private void Update()
    {
        //string targetName = targetNode != null ? targetNode.name : "Null";
        //string currentName = currentNode != null ? currentNode.name : "Null";
        //print($"{currentName} --> {targetName}");

        if (targetNode != null)
        {
            lineObject.GetComponent<LineScript>().SetDrawLine(true);
            lineObject.GetComponent<LineScript>().SetStart(currentNode.transform.position);
            lineObject.GetComponent<LineScript>().SetEnd(targetNode.transform.position);
        }
    }
    #endregion

    #region Event Methods

    private void OnNodeSelect(object target)
    {
        if(target is GameObject targetNode)
        {
            // Turn on nav controls UI
            toolTipUIObject.SetActive(true);

            // set targetNode to the currently selected node
            this.targetNode = targetNode;
        }
    }
    private void OnNodeDeselect(object target)
    {
        // reset target node to null
        targetNode = null;

        // turn off the nav controls UI
        toolTipUIObject.SetActive(false);

        // turn off the line
        lineObject.GetComponent<LineScript>().SetDrawLine(false);
    }

    private void OnConfirmTravel(object target)
    {
        print($"Traveled to {targetNode.name}");
        if(targetNode != null) ChangeCurrentNode(targetNode);
    }
    #endregion

    #region Methods

    private void ChangeCurrentNode(GameObject newNode)
    {
        // Set old current node to not be selected
        currentNode.GetComponent<HoverScript>().SetIsCurrentNode(false);
        currentNode.GetComponent<HoverScript>().Reset();

        // Change current node to new node
        currentNode = newNode;

        // Set new current node to be selected
        currentNode.GetComponent<HoverScript>().SetIsCurrentNode(true);

        // Update current ship scene
        if (currentNode.GetComponent<NodeScript>().IsHomeNode())
        {
            atHomeNode = true;
            eventManager.Publish(EventType.ArrivedAtHomeNode);
        }
        else
        {
            atHomeNode = false;
            currentShipScene = currentNode.GetComponent<NodeScript>().TargetShipScene().name;
        }
    }

    public string GetCurrentShipScene()
    {
        return currentShipScene;
    }
    public bool IsAtHomeNode()
    {
        return atHomeNode;
    }
    #endregion
}
