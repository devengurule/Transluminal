using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScannerController : MonoBehaviour
{
    [SerializeField] private GameObject alienDensityImage;
    [SerializeField] private GameObject densityImage;
    [SerializeField] private GameObject salvageObject;
    [SerializeField] private GameObject displayObject;

    private EventManager eventManager;
    private List<SalvageSaveData> salvageList;
    private SalvageSaveData currentSalvage;
    private bool canChangeSelection = true;
    private bool canScan;

    private void Start()
    {
        eventManager = GameController.instance.eventManager;

        densityImage.SetActive(false);
        alienDensityImage.SetActive(false);

        UpdateDisplay("Hello World");

        if (eventManager != null )
        {
            eventManager.Subscribe(EventType.Interact, SelectButton);
        }
    }

    private void OnDestroy()
    {
        if (eventManager != null)
        {
            eventManager.Unsubscribe(EventType.Interact, SelectButton);
        }
    }

    private void OnEnable()
    {
        salvageList = GameController.instance.GetComponent<CollectableManager>().GetSalvageList();

        if(salvageList.Count > 0 && salvageList != null)
        {
            currentSalvage = salvageList[0];
        }

        if (currentSalvage.salvageData != null)
        {
            salvageObject.GetComponent<Image>().enabled = true;
            salvageObject.GetComponent<Image>().sprite = currentSalvage.salvageData.sprite;
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

    private void SelectButton(object target)
    {
        if (gameObject.activeSelf && currentSalvage.salvageData != null && canScan)
        {
            GameObject selectedScan = GetComponent<ScannerSelectionController>().GetCurrentSelection();

            switch (selectedScan.tag)
            {
                case "FluidAmount":

                    if(currentSalvage.alienData != null)
                    {
                        UpdateDisplay((currentSalvage.salvageData.fluidAmount + currentSalvage.alienData.fluidSignature).ToString());
                    }
                    else
                    {
                        UpdateDisplay(currentSalvage.salvageData.fluidAmount.ToString());
                    }
                    
                    break;
                case "DensityCT":

                    ToggleDensityScan();

                    break;
                case "FluidType":

                    UpdateDisplay(currentSalvage.salvageData.fluidType.ToString());

                    break;
                case "SalvageIt":

                    KeepSalvage();

                    break;
                case "TrashIt":

                    TrashSalvage();

                    break;
            }
        }
    }

    private void KeepSalvage()
    {
        if(currentSalvage.alienData != null)
        {
            // Theres an alien on board

            switch(currentSalvage.alienData.alienType)
            {
                case AlienType.creature:



                    break;

                case AlienType.rat:



                    break;

                case AlienType.hunter:

                    eventManager.Publish(EventType.SpawnHunter, currentSalvage.alienData);

                    break;
            }
        }

        GameController.instance.GetComponent<CollectableManager>().AddCollectedSalvageValue(currentSalvage.value);
        GameController.instance.GetComponent<CollectableManager>().RemoveSalvage(0);

        ResetScanner();
    }
    private void TrashSalvage()
    {
        GameController.instance.GetComponent<CollectableManager>().RemoveSalvage(0);

        ResetScanner();
    }

    private void ToggleDensityScan()
    {
        if (currentSalvage.salvageData != null)
        {
            if (!densityImage.GetComponent<Image>().IsActive())
            {
                densityImage.GetComponent<Image>().sprite = currentSalvage.salvageData.densityImage;
                densityImage.SetActive(true);
                canChangeSelection = false;

                if (currentSalvage.alienData != null)
                {
                    alienDensityImage.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
                    alienDensityImage.GetComponent<RectTransform>().localScale = Vector3.one * Random.Range(0.5f, 1);
                    alienDensityImage.GetComponent<RectTransform>().position = densityImage.GetComponent<RectTransform>().position + new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), 0);
                    alienDensityImage.GetComponent<Image>().sprite = currentSalvage.alienData.densityImage;
                    alienDensityImage.SetActive(true);
                }
            }
            else
            {
                densityImage.SetActive(false);
                alienDensityImage.SetActive(false);
                canChangeSelection = true;
            }
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

    private void ResetScanner()
    {
        if (GameController.instance.GetComponent<CollectableManager>().GetSalvageList().Count > 0)
        {
            currentSalvage = GameController.instance.GetComponent<CollectableManager>().GetSalvageList()[0];

            salvageObject.GetComponent<Image>().enabled = true;
            salvageObject.GetComponent<Image>().sprite = currentSalvage.salvageData.sprite;
        }
        else
        {
            currentSalvage.salvageData = null;
            currentSalvage.alienData = null;
            alienDensityImage = null;
            densityImage = null;
            UpdateDisplay("Hello World");
            salvageObject.GetComponent<Image>().enabled = false;
        }
    }
}
