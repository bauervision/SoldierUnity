using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttachmentManager : MonoBehaviour
{
    public static AttachmentManager instance { get; private set; }
    public UnityEvent clearPacks = new();
    public UnityEvent clearHelmets = new();
    public UnityEvent clearWeapons = new();

    List<HighlightableObject> highlightableObjects = new();

    private void Awake() { instance = this; }

    public void AddAttachment(HighlightableObject highlightableObject)
    {
        // check to make sure we only attach one of these if there can be only one
        HandleSingleAttachments(highlightableObject.transform.GetComponent<LoadItem>().weapon.itemType);

        highlightableObject.AttachModel();
        highlightableObjects.Add(highlightableObject);
    }

    public void RemoveAttachment(int index)
    {
        highlightableObjects[index].DetachModel();
        highlightableObjects.Remove(highlightableObjects[index]);
    }

    /// <summary>
    /// There can only 1 Pack attached to the soldier, this method ensures this
    /// </summary>
    public void HandleSingleAttachments(ItemType itemType)
    {
        // check to see if we already have a a single item attached which can only have 1
        foreach (HighlightableObject item in highlightableObjects)
        {
            // we found a pack already attached
            if ((item.GetComponent<LoadItem>().weapon.itemType == ItemType.PACK) && (itemType == ItemType.PACK))
                clearPacks.Invoke();

            // we found a pack already attached
            if ((item.GetComponent<LoadItem>().weapon.itemType == ItemType.WEAPON) && (itemType == ItemType.WEAPON))
                clearWeapons.Invoke();

            // we found a pack already attached
            if ((item.GetComponent<LoadItem>().weapon.itemType == ItemType.HELMET) && (itemType == ItemType.HELMET))
                clearHelmets.Invoke();


        }

    }
}
