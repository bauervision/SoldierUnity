using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using NeoFPS;
namespace VRSoldier
{
    public class HighlightManager : MonoBehaviour
    {
        public float MAXWEIGHT = 300;
        public MouseAndGamepadAimController mouseAndGamepadAimController;
        public TextMeshProUGUI itemText, weightText, weaponText, totalText, totalAttachedText, maxWeightText;
        public Image highlightIcon;
        public Slider slider;
        public Color highlightIconColor, originalIconColor;
        public GameObject uiPrefab; // Assign the UI prefab in the Inspector 
        public Transform contentParent; // Assign the Scroll View Content in the Inspector
        private HighlightableObject currentHighlightedObject;

        LoadItem currentLoadItem;

        float totalWeight = 0;
        int totalAttached = 0;
        bool showCursor = false;
        bool canAttach = true;



        HighlightableObject _tempObject;

        private void Start()
        {
            originalIconColor = highlightIcon.color;
            weightText.text = "";
            weaponText.text = "";
        }


        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                showCursor = !showCursor;
                ToggleCursorMode(showCursor);
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (currentLoadItem)
                {
                    if (totalWeight <= MAXWEIGHT)
                    {
                        canAttach = true;
                        AddattachmentToUI();
                    }
                    else
                    {
                        canAttach = false;
                        print("cant attach anything else");
                    }
                }

            }



            if (canAttach)
            {
                Camera mainCamera = Camera.main;
                Ray ray = new(mainCamera.transform.position, mainCamera.transform.forward);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    HighlightableObject highlightableObject = hit.collider.GetComponentInParent<HighlightableObject>();
                    currentLoadItem = hit.collider.GetComponentInParent<LoadItem>();

                    if (highlightableObject != null)
                    {
                        if (currentHighlightedObject != highlightableObject)
                        {
                            if (currentHighlightedObject != null)
                            {
                                currentHighlightedObject.ResetHighlight();
                            }
                            // handle highlighting
                            highlightableObject.Highlight();
                            currentHighlightedObject = highlightableObject;
                            highlightIcon.color = highlightIconColor;
                            weightText.text = "Weight: " + currentLoadItem.weapon.itemWeight + "lbs";
                            weaponText.text = currentLoadItem.weapon.itemName;

                        }
                    }
                    else if (currentHighlightedObject != null)
                    {
                        currentHighlightedObject.ResetHighlight();
                        currentHighlightedObject = null;

                        highlightIcon.color = originalIconColor;
                        weightText.text = "";
                        weaponText.text = "";
                    }
                }
                else if (currentHighlightedObject != null)
                {
                    currentHighlightedObject.ResetHighlight();
                    currentHighlightedObject = null;

                    highlightIcon.color = originalIconColor;
                    weightText.text = "";
                    weaponText.text = "";
                }
            }

        }

        public void ChangeMissionType(int value)
        {
            if (value > 0)
            {
                switch (value)
                {
                    case 1: MAXWEIGHT = 300; maxWeightText.text = "300 lb MAX"; break;
                    case 2: MAXWEIGHT = 250; maxWeightText.text = "250 lb MAX"; break;
                    case 3: MAXWEIGHT = 125; maxWeightText.text = "125 lb MAX"; break;
                }
            }

        }
        void ToggleCursorMode(bool state)
        {
            Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = state;
            mouseAndGamepadAimController.enabled = !state;
        }

        void AddattachmentToUI()
        {
            totalAttached++;
            totalAttachedText.text = "Total Attached: " + totalAttached;
            // Instantiate the UI prefab and set its parent to the Scroll View Content 
            if (uiPrefab != null && contentParent != null)
            {

                float newweight = totalWeight + currentLoadItem.weapon.itemWeight;
                totalWeight = RoundToTwoDecimalPlaces(Mathf.Clamp(newweight, 0f, 300));
                totalText.text = HandleWeightTextUpdate(totalWeight);

                GameObject instantiatedUI = Instantiate(uiPrefab, contentParent);
                // set all the text values
                instantiatedUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentLoadItem.weapon.itemName + " : " + currentLoadItem.weapon.itemWeight;
                instantiatedUI.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentLoadItem.weapon.itemType.ToString();
                // handle the button
                Button removeButton = instantiatedUI.transform.GetChild(1).GetComponent<Button>();
                float weight = currentLoadItem.weapon.itemWeight;
                removeButton.onClick.AddListener(() => HandleRemoval(instantiatedUI, weight));
            }

            // handle attachment
            AttachmentManager.instance.AddAttachment(currentLoadItem.transform.GetComponent<HighlightableObject>());


            // Play a random click sound using AudioManager 
            if (AudioManager.Instance != null) { AudioManager.Instance.PlaySound(currentLoadItem.weapon.itemType); }
        }

        void HandleRemoval(GameObject removeButton, float weight)
        {
            totalAttached--;
            totalAttachedText.text = "Total Attached: " + totalAttached;
            float newweight = totalWeight - weight;
            totalWeight = RoundToTwoDecimalPlaces(Mathf.Clamp(newweight, 0f, 300f));
            // Check if totalWeight is very close to zero, and set it to zero if so 
            if (Mathf.Abs(totalWeight) < 0.001f) { totalWeight = 0f; }
            totalText.text = HandleWeightTextUpdate(totalWeight);

            AttachmentManager.instance.RemoveAttachment(removeButton.transform.GetComponentIndex());

            Destroy(removeButton);

            if (AudioManager.Instance != null) { AudioManager.Instance.PlayRemove(); }
        }

        string HandleWeightTextUpdate(float weight)
        {
            slider.value = weight;
            return totalWeight + "lbs";
        }

        float RoundToTwoDecimalPlaces(float value) { return Mathf.Round(value * 100f) / 100f; }
    }
}
