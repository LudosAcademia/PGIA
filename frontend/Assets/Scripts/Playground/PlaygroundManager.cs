using System;
using System.Collections.Generic;
using UnityEngine;

public class PlaygroundManager : MonoBehaviour
{
    public static event Action OnStartPlaygroundCreate;
    public static event Action OnCancelPlaygroundCreate;
    public static event Action OnEndPlaygroundCreate;
    public static event Action<bool> OnCheckPlaygrounds;

    private void Start()
    {

    }

    private void CheckForPlaygrounds()
    {
        List<PlaygroundData> playgrounds = GameManager.Instance.GetGameData().currentUser.playgrounds;
        if (playgrounds.Count != 0)
        {
            OnCheckPlaygrounds?.Invoke(true);
        }
        else
        {
            OnCheckPlaygrounds?.Invoke(false);
        }
    }

    //Starts the creation process for a new playground
    public void StartCreatePlayground()
    {
        OnStartPlaygroundCreate?.Invoke();
    }

    //Cancels the creation process for a new playground
    public void CancelCreatePlayground()
    {
        OnCancelPlaygroundCreate?.Invoke();
    }

    public void EndCreatePlayground()
    {
        OnEndPlaygroundCreate?.Invoke();
    }

}
