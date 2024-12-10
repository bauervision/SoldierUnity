using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using NeoFPS;
namespace VRSoldier
{
    public class HighlightManager : MonoBehaviour
    {
        public static HighlightManager instance;
        public float MAXWEIGHT = 300;
        public MouseAndGamepadAimController mouseAndGamepadAimController;
        public TextMeshProUGUI itemText, weightText, weaponText, maxWeightText;
        public Image highlightIcon;

        public Color highlightIconColor, originalIconColor;

        private HighlightableObject currentHighlightedObject;

        public LoadItem currentLoadItem;

        public float totalWeight = 0;

        bool showCursor = false;
        bool canAttach = true;



        HighlightableObject _tempObject;

        private void Awake() { instance = this; }

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
                        AttachmentManager.instance.AddattachmentToUI();
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

    }
}
