using UnityEngine;
using UnityEngine.UI;

public class JoystickVisualFeedback : MonoBehaviour
{
    [Header("Visual Settings")]
    public Color normalColor = new Color(0.2f, 0.6f, 1f, 0.9f);
    public Color pressedColor = new Color(0.1f, 0.3f, 0.8f, 1f);
    
    private Image handleImage;
    private Vector3 originalScale;

    void Start()
    {
        handleImage = GetComponent<Image>();
        originalScale = transform.localScale;
        
        if (handleImage != null)
            handleImage.color = normalColor;
    }

    public void SetPressed(bool isPressed)
    {
        if (handleImage != null)
        {
            handleImage.color = isPressed ? pressedColor : normalColor;
        }
        
        // Add a slight scale effect when pressed
        if (transform != null)
        {
            transform.localScale = isPressed ? originalScale * 0.9f : originalScale;
        }
    }
}