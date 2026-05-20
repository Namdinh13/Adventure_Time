using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private void Start()
    {
        HideCursor();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowCursor();
        }

        if (Input.GetMouseButtonDown(0))
        {
            HideCursor();
        }
    }

    private void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
