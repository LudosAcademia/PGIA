using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userDetailText;

    [SerializeField] private GameObject setupPanel, userInfoPanel, interactObjPanel, propObjPanel;

    [SerializeField] private TextMeshProUGUI playgroundMessage;
    [SerializeField] private GameObject createPlaygroundPanel;
    [SerializeField] private GameObject editPlaygroundPanel;
    [SerializeField] private Transform playgroundParent;
    [SerializeField] private GameObject playgroundView;
    [SerializeField] private TextMeshProUGUI playgroundInfo;
    [SerializeField] private GameObject playgroundViewPanel;


    private int playgroundIndex = -1;
    public int PlaygroundIndex { get => playgroundIndex; set => playgroundIndex = value; }

    private void Awake()
    {
        CheckForPlaygrounds();
    }

    private void Start()
    {
        UpdateDetailText();
    }


    private void CheckForPlaygrounds()
    {
        List<PlaygroundData> currentPlaygrounds = GameManager.Instance.GetGameData().currentUser.playgrounds;
        string msg = "";
        if (currentPlaygrounds.Count == 0)
        {
            msg = "No Playground found press \"Create New Playground\" to create new playground";
        }
        else
        {
            msg = $"Currently There are {currentPlaygrounds.Count} under this account, Select to view or edit them";
            UpdatePlaygroundView();

        }

        playgroundMessage.text = msg;
    }

    public void UpdatePlaygroundView()
    {
        if (playgroundParent.childCount != 0)
        {
            for (int i = 0; i < playgroundParent.childCount; i++)
            {
                Destroy(playgroundParent.GetChild(i).gameObject);
            }
        }

        List<PlaygroundData> currentPlaygrounds = GameManager.Instance.GetGameData().currentUser.playgrounds;
        if (currentPlaygrounds.Count != 0)
        {
            for (int i = 0; i < currentPlaygrounds.Count; i++)
            {
                GameObject newPlaygroundView = Instantiate(playgroundView);
                newPlaygroundView.transform.SetParent(playgroundParent);
                newPlaygroundView.name = i.ToString();
                string playgroundName = "Playground " + GameManager.Instance.GetGameData().currentUser.playgrounds[i].id.ToString();
                newPlaygroundView.GetComponentInChildren<TextMeshProUGUI>().text = playgroundName;
            }
        }

    }

    public void UpdatePlaygroundSelection()
    {
        string msg = string.Empty;

        if (playgroundIndex != -1)
        {
            msg = "Playground ID: " + GameManager.Instance.GetGameData().currentUser.playgrounds[playgroundIndex].id;
            OpenEditPlaygroundPanel();
        }

        playgroundInfo.text = msg;
    }


    public void OpenEditPlaygroundPanel()
    {
        createPlaygroundPanel.SetActive(false);
        editPlaygroundPanel.SetActive(true);
    }

    public void OpenCreatePlaygroundPanel()
    {
        createPlaygroundPanel.SetActive(true);
        editPlaygroundPanel.SetActive(false);
    }


    public void OpenInteractObjPanel()
    {
        interactObjPanel.SetActive(true);
        propObjPanel.SetActive(false);
    }

    public void OpenProbObjPanel()
    {
        interactObjPanel.SetActive(false);
        propObjPanel.SetActive(true);
    }

    public void OpenSetupPanel()
    {
        setupPanel.SetActive(true);
        userInfoPanel.SetActive(false);
        playgroundViewPanel.SetActive(false);
    }

    public void CloseSetupPanel()
    {
        setupPanel.SetActive(false);
        userInfoPanel.SetActive(true);
        playgroundViewPanel.SetActive(true);
    }

    public void Return()
    {
        CloseSetupPanel();
    }

    private void UpdateDetailText()
    {
        userDetailText.text = " Name: " + GameManager.Instance.GetGameData().currentUser.username
    + "\n Position: " + GameManager.Instance.GetGameData().currentUser.curr_ply_index;

    }

}
