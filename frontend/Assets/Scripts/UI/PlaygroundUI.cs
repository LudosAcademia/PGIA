using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlaygroundUI : MonoBehaviour
{
    [SerializeField] private GameObject createPlaygroundPanel;
    [SerializeField] private GameObject viewPlaygroundPanel;
    [SerializeField] private Transform viewPlaygroundContent;
    [SerializeField] private GameObject buttonPrefabPlaygroundView;
    [SerializeField] private TextMeshProUGUI userNameHeader;
    private int selectedPlaygroundIndex = -1;
    private List<GameObject> playgroundViewButtons = new List<GameObject>();


    private void Start()
    {
        userNameHeader.text = GameManager.Instance.GetGameData().currentUser.name;
    }


    private void OnEnable()
    {
        PlaygroundManager.OnStartPlaygroundCreate += OpenCreatePlaygroundPanel;
        PlaygroundManager.OnStartPlaygroundCreate += CloseViewPlaygroundPanel;
        PlaygroundManager.OnCancelPlaygroundCreate += CloseCreatePlaygroundPanel;
        PlaygroundManager.OnCancelPlaygroundCreate += OpenViewPlaygroundPanel;
        PlaygroundManager.OnEndPlaygroundCreate += CloseCreatePlaygroundPanel;
        PlaygroundManager.OnEndPlaygroundCreate += OpenViewPlaygroundPanel;

    }

    private void OnDisable()
    {
        PlaygroundManager.OnStartPlaygroundCreate -= OpenCreatePlaygroundPanel;
        PlaygroundManager.OnStartPlaygroundCreate -= CloseViewPlaygroundPanel;
        PlaygroundManager.OnCancelPlaygroundCreate -= CloseCreatePlaygroundPanel;
        PlaygroundManager.OnCancelPlaygroundCreate -= OpenViewPlaygroundPanel;
        PlaygroundManager.OnEndPlaygroundCreate -= CloseCreatePlaygroundPanel;
        PlaygroundManager.OnEndPlaygroundCreate -= OpenViewPlaygroundPanel;
    }

    private void OpenCreatePlaygroundPanel()
    {
        createPlaygroundPanel.SetActive(true);
    }

    private void CloseCreatePlaygroundPanel()
    {
        createPlaygroundPanel.SetActive(false);
    }

    private void OpenViewPlaygroundPanel()
    {
        viewPlaygroundPanel.SetActive(false);
    }

    private void CloseViewPlaygroundPanel()
    {
        viewPlaygroundPanel.SetActive(false);
    }

    private void AddPlaygroundToView()
    {
        PlaygroundData plygrd = GameManager.Instance.GetGameData().currentUser.playgrounds.Last();
        GameObject newPlygrd = Instantiate(buttonPrefabPlaygroundView);
        newPlygrd.transform.SetParent(viewPlaygroundContent, false);
        playgroundViewButtons.Add(newPlygrd);
    }

    private void RemoveAllPlaygrounds()
    {
        if (viewPlaygroundContent.childCount != 0)
        {
            for (int i = 0; i < viewPlaygroundContent.childCount; i++)
            {
                Destroy(viewPlaygroundContent.GetChild(i).gameObject);
            }
        }

    }


    private void AddAllPlaygrounds()
    {
        RemoveAllPlaygrounds();
        List<PlaygroundData> playgrounds = GameManager.Instance.GetGameData().currentUser.playgrounds;
        if (playgroundViewButtons.Count != 0)
        {
            for (int i = 0; i < playgrounds.Count; i++)
            {
                GameObject newPlygrd = Instantiate(buttonPrefabPlaygroundView);
                newPlygrd.transform.SetParent(viewPlaygroundContent, false);
                newPlygrd.transform.GetChild(0).gameObject.name = i.ToString();
                newPlygrd.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = playgrounds[i].plygrd_name;
                playgroundViewButtons.Add(newPlygrd);
            }
        }
    }

    private void SetAllChildren(Transform parent,bool set)
    {
        if (parent.childCount != 0)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                parent.GetChild(i).gameObject.SetActive(set);
            }
        }
    }



}
