using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("Components")]
    public RectTransform background;
    public RectTransform handle;
    public JoystickVisualFeedback visualFeedback;

    [Header("Settings")]
    public float handleRange = 1f;
    public float deadZone = 0.25f;
    public bool showBackgroundInitially = true;

    private Vector2 inputVector = Vector2.zero;
    public Vector2 InputVector { get { return inputVector; } }

    private Canvas canvas;
    private Camera uiCamera;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            uiCamera = canvas.worldCamera;
        
        // Show background initially based on setting
        if (background != null)
        {
            background.gameObject.SetActive(showBackgroundInitially);
            Debug.Log($"Joystick background set to active: {showBackgroundInitially}");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Joystick OnPointerDown");
        
        // If background was hidden, show it and position it
        if (!background.gameObject.activeSelf)
        {
            background.gameObject.SetActive(true);
            background.position = eventData.position;
            handle.anchoredPosition = Vector2.zero;
        }
        
        if (visualFeedback != null)
            visualFeedback.SetPressed(true);
            
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Joystick OnDrag");
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, 
            eventData.position, 
            uiCamera, 
            out position
        );

        Vector2 sizeDelta = background.sizeDelta;
        position = new Vector2(
            (position.x / sizeDelta.x) * 2 - 1,
            (position.y / sizeDelta.y) * 2 - 1
        );

        inputVector = position.magnitude > 1 ? position.normalized : position;

        // Apply dead zone
        if (inputVector.magnitude < deadZone)
            inputVector = Vector2.zero;

        handle.anchoredPosition = inputVector * handleRange * (background.sizeDelta / 2);
        
        Debug.Log($"Joystick input vector: {inputVector}");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Joystick OnPointerUp");
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
        
        // Hide background if it was hidden initially
        if (!showBackgroundInitially)
            background.gameObject.SetActive(false);
        
        if (visualFeedback != null)
            visualFeedback.SetPressed(false);
    }
}