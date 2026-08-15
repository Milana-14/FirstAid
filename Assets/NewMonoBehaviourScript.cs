using UnityEngine;

public class FPSCheck : MonoBehaviour
{
    private float deltaTime;

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;
        
        Debug.Log("VSync: " + QualitySettings.vSyncCount);
        Debug.Log("Target FPS: " + Application.targetFrameRate);
        Debug.Log("Refresh rate: " + Screen.currentResolution.refreshRateRatio);
    }

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    private void OnGUI()
    {
        float fps = 1.0f / deltaTime;
        GUI.Label(
            new Rect(20, 20, 300, 40),
            $"FPS: {fps:0.0}");
    }
}
