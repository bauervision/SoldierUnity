using UnityEngine;

public class FindCenterObject : MonoBehaviour
{
    public Material centerMaterial, originalMaterial; // Assign this in the Inspector 
    public float threshold = 0.1f; // Adjust this value as needed 


    void Update()
    {
        if (IsInCenter()) { GetComponent<Renderer>().material = centerMaterial; }
        else
        {
            GetComponent<Renderer>().material = originalMaterial;
        }
    }
    bool IsInCenter()
    {
        Camera mainCamera = Camera.main; Vector3 screenPoint = mainCamera.WorldToScreenPoint(transform.position);
        return screenPoint.x > Screen.width * (0.5f - threshold) && screenPoint.x < Screen.width * (0.5f + threshold);
    }
}
