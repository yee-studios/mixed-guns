using UnityEngine;

public class SettingsApplier : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
}
