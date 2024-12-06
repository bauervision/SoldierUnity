using UnityEngine;
using UnityEngine.Events;

public class HighlightableObject : MonoBehaviour
{
    private Material[] originalMaterials;  // Array to store the original materials
    public Material[] highlightMaterials;  // Array of highlight materials

    public UnityEvent onAttach = new UnityEvent();
    public UnityEvent onDetach = new UnityEvent();

    private Renderer[] childRenderers;

    LoadItem _loadItem;

    void Start()
    {
        // Get all child renderers
        childRenderers = GetComponentsInChildren<Renderer>();
        _loadItem = GetComponent<LoadItem>();

        if (childRenderers.Length > 0)
        {
            // Assuming all children use the same set of materials
            originalMaterials = childRenderers[0].materials;
        }
    }

    public void AttachModel() { onAttach.Invoke(); }
    public void DetachModel() { onDetach.Invoke(); }

    public void Highlight()
    {

        foreach (Renderer renderer in childRenderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                if (i < highlightMaterials.Length)
                {
                    materials[i] = highlightMaterials[i];  // Replace with the corresponding highlight material
                }
            }
            renderer.materials = materials;
        }
    }

    public void ResetHighlight()
    {

        foreach (Renderer renderer in childRenderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                if (i < originalMaterials.Length)
                {
                    materials[i] = originalMaterials[i];  // Replace with the original material
                }
            }
            renderer.materials = materials;
        }
    }
}
