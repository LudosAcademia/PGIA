using System;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static event Action<bool> OnSettingsToggle;
    private bool toggleSettings = true;

    public void ToggleSettings()
    {
        if (toggleSettings)
        {
            OnSettingsToggle?.Invoke(true);
            toggleSettings = false;
        }
        else
        {
            OnSettingsToggle?.Invoke(false);
            toggleSettings = true;
        }
    }

}
