using UnityEngine;

public class MobileControlManager : MonoBehaviour
{
    public GameObject mobileControlsCanvas;
    
    void Start()
    {
        // Always show controls for testing
        if (mobileControlsCanvas != null)
        {
            mobileControlsCanvas.SetActive(true);
            Debug.Log("Mobile controls activated");
        }
        else
        {
            Debug.LogError("MobileControlsCanvas not assigned!");
        }
    }
}