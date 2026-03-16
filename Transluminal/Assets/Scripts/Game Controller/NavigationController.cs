using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationController : MonoBehaviour
{

    [SerializeField] private GameObject currentNode;
    [SerializeField] private GameObject lineObject;
    [SerializeField] private GameObject toolTipUIObject;

    private EventManager eventManager;
    private GameObject targetNode;
    private string currentShipScene;
    private bool atHomeNode;

    private void Awake()
    {
        // Subscribe to active scene change event
        SceneManager.activeSceneChanged += SceneChange;
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

        if (eventManager != null)
        {
            eventManager.Subscribe(EventType.NodeSelected, OnNodeSelect);
            eventManager.Subscribe(EventType.NodeDeselected, OnNodeDeselect);
        }

    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.NodeSelected, OnNodeSelect);
            eventManager.Unsubscribe(EventType.NodeDeselected, OnNodeDeselect);
        }
    }
    private void Update()
    {
        // string targetName = targetNode != null ? targetNode.name : "Null";
        // print($"{currentNode.name} --> {targetName}");

        if(targetNode != null)
        {
            lineObject.GetComponent<LineScript>().SetDrawLine(true);
            lineObject.GetComponent<LineScript>().SetStart(currentNode.transform.position);
            lineObject.GetComponent<LineScript>().SetEnd(targetNode.transform.position);
        }
    }

    private void OnNodeSelect(object target)
    {
        if(target is GameObject targetNode)
        {
            toolTipUIObject.SetActive(true);
            this.targetNode = targetNode;
        }
    }
    private void OnNodeDeselect(object target)
    {
        targetNode = null;
        toolTipUIObject.SetActive(false);
        lineObject.GetComponent<LineScript>().SetDrawLine(false);
    }

    private void SceneChange(Scene current, Scene next)
    {
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

    public string GetCurrentShipScene()
    {
        return currentShipScene;
    }
    public bool IsAtHomeNode()
    {
        return atHomeNode;
    }
}
