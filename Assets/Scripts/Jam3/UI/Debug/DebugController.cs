using UnityEngine;

public class DebugController : MonoBehaviour
{
    public GameObject DebugUI = null;

    public void ShowDebug()
    {
        if (DebugUI != null)
            DebugUI.SetActive(true);
    }

    public void HideDebug()
    {
        if (DebugUI != null)
            DebugUI.SetActive(false);
    }
}
