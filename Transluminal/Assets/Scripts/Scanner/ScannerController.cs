using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class ScannerController : MonoBehaviour
{
    [SerializeField] private GameObject salvageObject;
    [SerializeField] private GameObject displayObject;
    private EventManager eventManager;
    private List<SalvageSaveData> salvageList;
    private SalvageData currentSalvage;
    private bool canScan;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        if(eventManager != null )
        {
            eventManager.Subscribe(EventType.Interact, SelectScan);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.Interact, SelectScan);
        }
    }

    private void OnEnable()
    {
        salvageList = GameController.instance.GetSalvageList();

        if(salvageList.Count > 0 && salvageList != null)
        {
            currentSalvage = salvageList[0].salvageData;
        }

        if (currentSalvage != null)
        {
            canScan = true;
            salvageObject.GetComponent<Image>().enabled = true;
            salvageObject.GetComponent<Image>().sprite = currentSalvage.sprite;
        }
        else
        {
            canScan = false;
            salvageObject.GetComponent<Image>().enabled = false;
        }
    }

    private void SelectScan(object target)
    {
        if (gameObject.activeSelf && currentSalvage != null)
        {
            GameObject selectedScan = GetComponent<ScannerSelectionController>().GetCurrentSelection();

            switch (selectedScan.tag)
            {
                case "FluidAmount":
                    canScan = false;

                    UpdateDisplay(currentSalvage.fluidAmount.ToString());

                    break;
                case "DensityCT":
                    canScan = false;

                    print("Scanning...");

                    break;
                case "FluidType":
                    canScan = false;

                    UpdateDisplay(currentSalvage.fluidType.ToString());

                    break;
                default:
                    canScan = true;
                    break;
            }
        }
    }


    private void UpdateDisplay(string input)
    {
        displayObject.GetComponent<TMP_Text>().text = input;
    }
}
