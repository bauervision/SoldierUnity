using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VRSoldier;

public class AttachmentManager : MonoBehaviour
{
    public static AttachmentManager instance { get; private set; }
    public GameObject uiPrefab, packObjectUI, helmetObjectUI, weaponObjectUI; // Assign the UI prefab in the Inspector 
    HighlightableObject _packObject, _helmetObject, _weaponObject;
    public Transform contentParent; // Assign the Scroll View Content in the Inspector
    public TextMeshProUGUI totalText, totalAttachedText;
    public Slider slider;
    public UnityEvent clearPacks = new();
    public UnityEvent clearHelmets = new();
    public UnityEvent clearWeapons = new();

    int totalAttached = 0;
    List<HighlightableObject> highlightableObjects = new();

    private void Awake() { instance = this; }

    public void AddAttachment(HighlightableObject highlightableObject)
    {
        // check to make sure we only attach one of these if there can be only one
        HandleSingleAttachments(highlightableObject.transform.GetComponent<LoadItem>().weapon.itemType);

        highlightableObject.AttachModel();
        // now let's find out if we need to collect the attachments in a list
        ItemType itemType = highlightableObject.transform.GetComponent<LoadItem>().weapon.itemType;
        AttachTypes(itemType, highlightableObject);

    }

    /// <summary>
    /// There can only 1 Pack attached to the soldier, this method ensures this
    /// </summary>
    public void HandleSingleAttachments(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.WEAPON: clearWeapons.Invoke(); break;
            case ItemType.PACK: clearPacks.Invoke(); break;
            case ItemType.HELMET: clearHelmets.Invoke(); break;
        }


    }

    void AttachTypes(ItemType itemType, HighlightableObject highlightableObject)
    {
        switch (itemType)
        {
            case ItemType.WEAPON: _weaponObject = highlightableObject; break;
            case ItemType.PACK: _packObject = highlightableObject; break;
            case ItemType.HELMET: _helmetObject = highlightableObject; break;
            // case ItemType.MASK: _weaponObject = highlightableObject; break;
            // case ItemType.PISTOL: _weaponObject = highlightableObject; break;
            // case ItemType.VEST: _weaponObject = highlightableObject; break;
            default: highlightableObjects.Add(highlightableObject); break;
        }
    }

    public void RemoveAttachment(int index)
    {
        highlightableObjects[index].DetachModel();
        highlightableObjects.Remove(highlightableObjects[index]);
    }



    float RoundToTwoDecimalPlaces(float value) { return Mathf.Round(value * 100f) / 100f; }

    string HandleWeightTextUpdate(float weight)
    {
        slider.value = weight;
        return HighlightManager.instance.totalWeight + "lbs";
    }

    public void AddattachmentToUI()
    {
        // now we  need to handle the 4 types of attachments, first find out which type this is
        HandleTypeAttachmentUI();

        // handle attachment
        AddAttachment(HighlightManager.instance.currentLoadItem.transform.GetComponent<HighlightableObject>());


        // Play a random click sound using AudioManager 
        if (AudioManager.Instance != null) { AudioManager.Instance.PlaySound(HighlightManager.instance.currentLoadItem.weapon.itemType); }
    }


    float _lastWeaponWeight = 0, _lastPackWeight = 0, _lastHelmetWeight = 0;
    void HandleTypeAttachmentUI()
    {
        switch (HighlightManager.instance.currentLoadItem.weapon.itemType)
        {
            case ItemType.WEAPON:
                {
                    // if this item was not yet added
                    if (!weaponObjectUI.activeInHierarchy)
                    {
                        totalAttached++;
                        totalAttachedText.text = "Total Attached: " + totalAttached;
                        weaponObjectUI.SetActive(true);
                    }
                    // set the text
                    weaponObjectUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = HighlightManager.instance.currentLoadItem.weapon.itemName + " : " + HighlightManager.instance.currentLoadItem.weapon.itemWeight;
                    // handle the weight
                    float newWeight = HighlightManager.instance.currentLoadItem.weapon.itemWeight;
                    float oldWeight = _lastWeaponWeight;
                    UpdateTotalWeight(newWeight, oldWeight);
                    _lastWeaponWeight = newWeight;
                    break;
                }
            case ItemType.HELMET:
                {
                    if (!helmetObjectUI.activeInHierarchy)
                    {
                        totalAttached++;
                        totalAttachedText.text = "Total Attached: " + totalAttached;
                        helmetObjectUI.SetActive(true);
                    }
                    // set the text
                    helmetObjectUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = HighlightManager.instance.currentLoadItem.weapon.itemName + " : " + HighlightManager.instance.currentLoadItem.weapon.itemWeight;
                    // handle the weight
                    float newWeight = HighlightManager.instance.currentLoadItem.weapon.itemWeight;
                    float oldWeight = _lastHelmetWeight;
                    UpdateTotalWeight(newWeight, oldWeight);
                    _lastHelmetWeight = newWeight;
                    break;
                }
            case ItemType.PACK:
                {
                    if (!packObjectUI.activeInHierarchy)
                    {
                        totalAttached++;
                        totalAttachedText.text = "Total Attached: " + totalAttached;
                        packObjectUI.SetActive(true);
                    }
                    // set the text
                    packObjectUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = HighlightManager.instance.currentLoadItem.weapon.itemName + " : " + HighlightManager.instance.currentLoadItem.weapon.itemWeight;
                    // handle the weight
                    float newWeight = HighlightManager.instance.currentLoadItem.weapon.itemWeight;
                    float oldWeight = _lastPackWeight;
                    UpdateTotalWeight(newWeight, oldWeight);
                    _lastPackWeight = newWeight;
                    break;
                }
            default:// basic attachments
                {
                    totalAttached++;
                    totalAttachedText.text = "Total Attached: " + totalAttached;

                    GameObject instantiatedUI = Instantiate(uiPrefab, contentParent);
                    // set all the text values
                    instantiatedUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = HighlightManager.instance.currentLoadItem.weapon.itemName + " : " + HighlightManager.instance.currentLoadItem.weapon.itemWeight;
                    instantiatedUI.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = HighlightManager.instance.currentLoadItem.weapon.itemType.ToString();
                    // handle the button
                    Button removeButton = instantiatedUI.transform.GetChild(1).GetComponent<Button>();
                    float weight = HighlightManager.instance.currentLoadItem.weapon.itemWeight;
                    removeButton.onClick.AddListener(() => HandleRemoval(instantiatedUI, weight));

                    UpdateTotalWeight(weight, 0);
                    break;
                }

        }
    }

    void UpdateTotalWeight(float newWeight, float oldWeight)
    {
        float updatedWeight = HighlightManager.instance.totalWeight + newWeight - oldWeight;
        HighlightManager.instance.totalWeight = RoundToTwoDecimalPlaces(Mathf.Clamp(updatedWeight, 0f, 300));
        totalText.text = HandleWeightTextUpdate(HighlightManager.instance.totalWeight);

    }

    public void RemovePack()
    {
        totalAttached--;
        totalAttachedText.text = "Total Attached: " + totalAttached;
        float newweight = HighlightManager.instance.totalWeight - _lastPackWeight;
        HighlightManager.instance.totalWeight = RoundToTwoDecimalPlaces(Mathf.Clamp(newweight, 0f, 300f));
        // Check if totalWeight is very close to zero, and set it to zero if so 
        if (Mathf.Abs(HighlightManager.instance.totalWeight) < 0.001f) { HighlightManager.instance.totalWeight = 0f; }
        totalText.text = HandleWeightTextUpdate(HighlightManager.instance.totalWeight);

        _packObject.transform.GetComponent<HighlightableObject>().DetachModel();
        _packObject = null;
        packObjectUI.SetActive(false);
        if (AudioManager.Instance != null) { AudioManager.Instance.PlayRemove(); }

    }

    public void RemoveHelmet()
    {
        totalAttached--;
        totalAttachedText.text = "Total Attached: " + totalAttached;
        float newweight = HighlightManager.instance.totalWeight - _lastPackWeight;
        HighlightManager.instance.totalWeight = RoundToTwoDecimalPlaces(Mathf.Clamp(newweight, 0f, 300f));
        // Check if totalWeight is very close to zero, and set it to zero if so 
        if (Mathf.Abs(HighlightManager.instance.totalWeight) < 0.001f) { HighlightManager.instance.totalWeight = 0f; }
        totalText.text = HandleWeightTextUpdate(HighlightManager.instance.totalWeight);

        _helmetObject.transform.GetComponent<HighlightableObject>().DetachModel();
        _helmetObject = null;
        helmetObjectUI.SetActive(false);
        if (AudioManager.Instance != null) { AudioManager.Instance.PlayRemove(); }
    }

    public void RemoveWeapon()
    {
        totalAttached--;
        totalAttachedText.text = "Total Attached: " + totalAttached;
        float newweight = HighlightManager.instance.totalWeight - _lastPackWeight;
        HighlightManager.instance.totalWeight = RoundToTwoDecimalPlaces(Mathf.Clamp(newweight, 0f, 300f));
        // Check if totalWeight is very close to zero, and set it to zero if so 
        if (Mathf.Abs(HighlightManager.instance.totalWeight) < 0.001f) { HighlightManager.instance.totalWeight = 0f; }
        totalText.text = HandleWeightTextUpdate(HighlightManager.instance.totalWeight);

        _weaponObject.transform.GetComponent<HighlightableObject>().DetachModel();
        _weaponObject = null;
        weaponObjectUI.SetActive(false);
        if (AudioManager.Instance != null) { AudioManager.Instance.PlayRemove(); }
    }

    public void HandleRemoval(GameObject removeButton, float weight)
    {
        totalAttached--;
        totalAttachedText.text = "Total Attached: " + totalAttached;
        float newweight = HighlightManager.instance.totalWeight - weight;
        HighlightManager.instance.totalWeight = RoundToTwoDecimalPlaces(Mathf.Clamp(newweight, 0f, 300f));
        // Check if totalWeight is very close to zero, and set it to zero if so 
        if (Mathf.Abs(HighlightManager.instance.totalWeight) < 0.001f) { HighlightManager.instance.totalWeight = 0f; }
        totalText.text = HandleWeightTextUpdate(HighlightManager.instance.totalWeight);

        RemoveAttachment(removeButton.transform.GetComponentIndex());

        Destroy(removeButton);

        if (AudioManager.Instance != null) { AudioManager.Instance.PlayRemove(); }
    }

}
