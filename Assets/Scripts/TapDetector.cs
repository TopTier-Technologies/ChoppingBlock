using UnityEngine;
using UnityEngine.InputSystem;

public class TapDetector : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        
        // If Camera.main doesn't work, find it manually
        if (mainCamera == null)
        {
            mainCamera = FindFirstObjectByType<Camera>();
        }
        
        if (mainCamera == null)
        {
            Debug.LogError("No camera found! Make sure your camera has the 'MainCamera' tag.");
        }
    }

    void Update()
    {
        if (mainCamera == null) return; // Safety check
        
        DetectTap();
    }

    void DetectTap()
    {
        // Check for touch input
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
                HandleTap(touchPosition);
            }
        }

        // Mouse input for testing in Unity Editor
        #if UNITY_EDITOR
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            HandleTap(mousePosition);
        }
        #endif
    }

    void HandleTap(Vector2 screenPosition)
    {
        if (mainCamera == null)
        {
            Debug.LogError("Camera is null in HandleTap!");
            return;
        }
        
        // Convert screen position to world position
        Vector2 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        
        Debug.Log("Tapped at screen position: " + screenPosition + " | World position: " + worldPosition);

        // Perform a raycast to detect what was tapped
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider != null)
        {
            Debug.Log("Hit object: " + hit.collider.gameObject.name);
            
            // Check if the tapped object is a food item
            FoodItem foodItem = hit.collider.GetComponent<FoodItem>();
            
            if (foodItem != null)
            {
                Debug.Log("Food item found! Calling OnTapped()");
                foodItem.OnTapped();
            }
            else
            {
                Debug.Log("Object has no FoodItem component");
            }
        }
        else
        {
            Debug.Log("No object hit by raycast");
        }
    }
}