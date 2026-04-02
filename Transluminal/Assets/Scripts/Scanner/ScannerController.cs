using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScannerController : MonoBehaviour
{
    [SerializeField] private GameObject densityImage;
    [SerializeField] private GameObject salvageObject;
    [SerializeField] private GameObject displayObject;

    private GameObject alienDesnityImage;
    private EventManager eventManager;
    private List<SalvageSaveData> salvageList;
    private SalvageData currentSalvage;
    private bool canChangeSelection = true;
    private bool canScan;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        alienDesnityImage = densityImage.transform.Find("AleinDensityScan").gameObject;

        densityImage.SetActive(false);
        alienDesnityImage.SetActive(false);

        if (eventManager != null )
        {
            eventManager.Subscribe(EventType.Interact, SelectScan);
            eventManager.Subscribe(EventType.Confirm, ConfirmSalvage);
            eventManager.Subscribe(EventType.Quit, TrashSalvage);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.Interact, SelectScan);
            eventManager.Unsubscribe(EventType.Confirm, ConfirmSalvage);
            eventManager.Unsubscribe(EventType.Quit, TrashSalvage);
        }
    }

    private void OnEnable()
    {
        salvageList = GameController.instance.GetComponent<CollectableManager>().GetSalvageList();

        if(salvageList.Count > 0 && salvageList != null)
        {
            currentSalvage = salvageList[0].salvageData;
        }

        if (currentSalvage != null)
        {
            salvageObject.GetComponent<Image>().enabled = true;
            salvageObject.GetComponent<Image>().sprite = currentSalvage.sprite;
        }
        else
        {
            salvageObject.GetComponent<Image>().enabled = false;
        }

        StartCoroutine(Wait());
    }

    private void OnDisable()
    {
        canScan = false;
    }

    private IEnumerator Wait()
    {
        yield return null;

        canScan = true;
    }

    private void SelectScan(object target)
    {
        if (gameObject.activeSelf && currentSalvage != null && canScan)
        {
            GameObject selectedScan = GetComponent<ScannerSelectionController>().GetCurrentSelection();

            switch (selectedScan.tag)
            {
                case "FluidAmount":

                    UpdateDisplay(currentSalvage.fluidAmount.ToString());

                    break;
                case "DensityCT":

                    ToggleDensityScan();

                    break;
                case "FluidType":

                    UpdateDisplay(currentSalvage.fluidType.ToString());

                    break;
            }
        }
    }

    private void ConfirmSalvage(object target)
    {
        if (currentSalvage != null && gameObject.activeSelf)
        {
            int value = Random.Range(currentSalvage.tier.minValue, currentSalvage.tier.maxValue);
            GameController.instance.GetComponent<CollectableManager>().AddCollectedSalvageValue(value);
            GameController.instance.GetComponent<CollectableManager>().RemoveSalvage(0);
            salvageObject.GetComponent<Image>().enabled = false;

            currentSalvage = null;
        }
    }
    private void TrashSalvage(object target)
    {
        if (currentSalvage != null && gameObject.activeSelf)
        {
            GameController.instance.GetComponent<CollectableManager>().RemoveSalvage(0);
            salvageObject.GetComponent<Image>().enabled = false;
            
            currentSalvage = null;
        }
    }

    private void ToggleDensityScan()
    {
        if (!densityImage.GetComponent<Image>().IsActive())
        {
            densityImage.GetComponent<Image>().sprite = currentSalvage.densityImage;
            densityImage.SetActive(true);
            canChangeSelection = false;
        }
        else
        {
            densityImage.SetActive(false);
            canChangeSelection = true;
        }
    }

    private void UpdateDisplay(string input)
    {
        displayObject.GetComponent<TMP_Text>().text = input;
    }

    public bool CanChangeSelection()
    {
        return canChangeSelection;
    }
}
