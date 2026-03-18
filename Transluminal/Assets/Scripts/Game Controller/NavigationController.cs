using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationController : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject currentNode;
    [SerializeField] private GameObject lineObject;
    [SerializeField] private GameObject nodeSelectionUI;
    [SerializeField] private int foodCostConstant;
    [SerializeField] private int fuelCostConstant;

    private EventManager eventManager;
    private GameObject targetNode;
    private string currentShipScene;
    private bool atHomeNode;
    private int foodCost;
    private int fuelCost;
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
            currentShipScene = currentNode.GetComponent<NodeScript>().TargetShipScene();
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
    #endregion

    #region Event Methods

    private void OnNodeSelect(object target)
    {
        if(target is GameObject targetNode)
        {
            
            // Turn on nav controls UI
            nodeSelectionUI.SetActive(true);

            // set targetNode to the currently selected node
            this.targetNode = targetNode;

            if (this.targetNode != null)
            {
                lineObject.GetComponent<LineScript>().SetStart(currentNode.transform.position);
                lineObject.GetComponent<LineScript>().SetEnd(targetNode.transform.position);
                UpdateSelectedNodeUI(lineObject.GetComponent<LineScript>().DrawLine());
            }
        }
    }
    private void OnNodeDeselect(object target)
    {
        // reset target node to null
        targetNode = null;

        // turn off the nav controls UI
        nodeSelectionUI.SetActive(false);

        // turn off the line
        lineObject.GetComponent<LineScript>().StopDrawingLine();
    }

    private void OnConfirmTravel(object target)
    {
        if (targetNode != null && currentNode != targetNode)
        {
            SubtractCosts(foodCost, fuelCost);

            // turn off the nav controls UI
            nodeSelectionUI.SetActive(false);

            // turn off the line
            lineObject.GetComponent<LineScript>().StopDrawingLine();

            //print($"Traveled to {targetNode.name}");
            ChangeCurrentNode(targetNode);
        }
    }
    #endregion


    #region Methods

    private void ChangeCurrentNode(GameObject newNode)
    {
        // Set old current node to not be selected
        currentNode.GetComponent<HoverScript>().SetIsCurrentNode(false);
        currentNode.GetComponent<HoverScript>().Reset();

        // Publish we are leaving home node if player was at one
        if (currentNode.GetComponent<NodeScript>().IsHomeNode()) eventManager.Publish(EventType.LeftHomeNode);

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
            currentShipScene = currentNode.GetComponent<NodeScript>().TargetShipScene();
        }
    }

    private void UpdateSelectedNodeUI(float distance)
    {
        TMP_Text foodCounter = GameObject.Find("FoodCounter").GetComponent<TMP_Text>();
        TMP_Text fuelCounter = GameObject.Find("FuelPercent").GetComponent<TMP_Text>();
        TMP_Text foodCostCounter = GameObject.Find("FoodCostCounter").GetComponent<TMP_Text>();
        TMP_Text fuelCostCounter = GameObject.Find("FuelCostPercent").GetComponent<TMP_Text>();

        foodCounter.text = GameController.instance.GetComponent<FoodManager>().GetCurrentFood().ToString();
        fuelCounter.text = GameController.instance.GetComponent<FuelManager>().GetCurrentFuel().ToString();

        UpdateValues(distance);


        foodCostCounter.text = foodCost.ToString();
        fuelCostCounter.text = fuelCost.ToString();
    }

    private void UpdateValues(float distance)
    {
        foodCost = (int)(distance / foodCostConstant);
        fuelCost = (int)(distance / fuelCostConstant);
    }

    private void SubtractCosts(int food, int fuel)
    {
        GameController.instance.GetComponent<FoodManager>().SubtractFood(food);
        GameController.instance.GetComponent<FuelManager>().SubtractFuel(fuel);
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
