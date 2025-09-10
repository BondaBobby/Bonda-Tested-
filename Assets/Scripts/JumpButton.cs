using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JumpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Visual Settings")]
    public Color normalColor = new Color(1, 1, 1, 0.7f);
    public Color pressedColor = new Color(1, 0.2f, 0.2f, 0.9f);
    
    [Header("Components")]
    public Image buttonImage;
    public Text buttonText;

    private bool isPressed = false;
    public bool IsPressed { get { return isPressed; } }

    void Start()
    {
        // Set initial visual state
        if (buttonImage != null)
            buttonImage.color = normalColor;
        
        Debug.Log("JumpButton Start completed");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("JumpButton OnPointerDown - Setting isPressed to true");
        isPressed = true;
        UpdateVisualState(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("JumpButton OnPointerUp - Setting isPressed to false");
        isPressed = false;
        UpdateVisualState(false);
    }

    private void UpdateVisualState(bool pressed)
    {
        Debug.Log($"JumpButton UpdateVisualState: {pressed}");
        if (buttonImage != null)
        {
            buttonImage.color = pressed ? pressedColor : normalColor;
        }
        
        // Add a slight scale effect when pressed
        if (transform != null)
        {
            transform.localScale = pressed ? new Vector3(0.95f, 0.95f, 1f) : Vector3.one;
        }
    }
}