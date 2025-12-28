using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using GameEnums;
public class PlaygroundUI : MonoBehaviour
{
    [SerializeField] private GameObject createPlaygroundPanel;
    [SerializeField] private GameObject viewPlaygroundPanel;
    [SerializeField] private Transform viewPlaygroundContent;
    [SerializeField] private GameObject buttonPrefabPlaygroundView;
    [SerializeField] private GameObject emptyPlaygroundsText;
    [SerializeField] private GameObject selectedPlaygroundsbuttons;
    [SerializeField] private GameObject serverMessageGameObject;
    [SerializeField] private TextMeshProUGUI serverMessageText;
    [SerializeField] private GameObject blockPanel;
    [SerializeField] private TextMeshProUGUI titleHeader;
    [SerializeField] private TextMeshProUGUI subjectHeader;
    [SerializeField] private GameObject noSelectedPlaygroundsText;
    [SerializeField] private GameObject creationPanel;
    [SerializeField] private GameObject editPanel;
    [SerializeField] private string gameTitle = "G.I.A.";
    private string username;
    private ReturnState returnButtonState = ReturnState.SelectionState;
    private int selectedPlaygroundIndex = -1;
    private List<GameObject> playgroundViewButtons = new();
    List<PlaygroundData> playgroundsData = new();

    public int SelectedPlaygroundIndex { get => selectedPlaygroundIndex; set => selectedPlaygroundIndex = value; }

    private void Start()
    {
        if (GameManager.Instance.GameData.currentUser.name == null)
        {
            CreateDummyData();
        }
        username = GameManager.Instance.GameData.currentUser.name;
        SetHeader(gameTitle, username);
        playgroundsData = GameManager.Instance.GetGameData().currentUser.playgrounds;
        AddAllPlaygrounds();
    }


    private void OnEnable()
    {
        PlaygroundManager.OnStartPlaygroundCreate += OpenCreatePlaygroundPanel;
        PlaygroundManager.OnStartPlaygroundCreate += CloseViewPlaygroundPanel;
        PlaygroundManager.OnCancelPlaygroundCreate += CloseCreatePlaygroundPanel;
        PlaygroundManager.OnCancelPlaygroundCreate += OpenViewPlaygroundPanel;
        PlaygroundManager.OnEndPlaygroundCreate += CloseCreatePlaygroundPanel;
        PlaygroundManager.OnEndPlaygroundCreate += OpenViewPlaygroundPanel;
        CreatePlayground.OnPlaygroundCreated += OpenBlockPanel;
        ServerClient.PlaygroundSaved += OpenServerMessage;
        ServerClient.PlaygroundSaved += AddPlaygroundToView;


    }

    private void OnDisable()
    {
        PlaygroundManager.OnStartPlaygroundCreate -= OpenCreatePlaygroundPanel;
        PlaygroundManager.OnStartPlaygroundCreate -= CloseViewPlaygroundPanel;
        PlaygroundManager.OnCancelPlaygroundCreate -= CloseCreatePlaygroundPanel;
        PlaygroundManager.OnCancelPlaygroundCreate -= OpenViewPlaygroundPanel;
        PlaygroundManager.OnEndPlaygroundCreate -= CloseCreatePlaygroundPanel;
        PlaygroundManager.OnEndPlaygroundCreate -= OpenViewPlaygroundPanel;
        ServerClient.PlaygroundSaved += OpenServerMessage;
        ServerClient.PlaygroundSaved -= AddPlaygroundToView;
    }

    private void OpenCreatePlaygroundPanel()
    {
        createPlaygroundPanel.SetActive(true);
        selectedPlaygroundsbuttons.SetActive(false);
    }

    private void CloseCreatePlaygroundPanel()
    {
        createPlaygroundPanel.SetActive(false);
        selectedPlaygroundsbuttons.SetActive(true);
    }

    private void OpenViewPlaygroundPanel()
    {
        viewPlaygroundPanel.SetActive(true);
    }

    private void CloseViewPlaygroundPanel()
    {
        viewPlaygroundPanel.SetActive(false);
    }

    private void OpenBlockPanel(PlaygroundData none)
    {
        blockPanel.SetActive(true);
    }

    private void CloseBlockPanel()
    {
        blockPanel.SetActive(false);
    }

    private void OpenEditPanel()
    {
        editPanel.SetActive(true);
    }
    private void CloseEditPanel()
    {
        editPanel.SetActive(false);
    }

    private void OpenCreationPanel()
    {
        creationPanel.SetActive(true);
    }
    private void CloseCreationPanel()
    {
        creationPanel.SetActive(false);
    }

    private void OpenBuildMenu()
    {

    }

    private void CloseBuildMenu()
    {

    }

    private void ReturnSelectionState()
    {
        returnButtonState = ReturnState.SelectionState;
        SetHeader(gameTitle, username);
        CloseEditPanel();
        OpenCreationPanel();
    }

    private void ReturnEditState()
    {

    }

    private void SetHeader(string title, string subject)
    {
        titleHeader.text = title;
        subjectHeader.text = subject;
    }


    public void GeneralReturnButton()
    {
        Debug.Log("Test Return Button");
        switch (returnButtonState)
        {
            case ReturnState.SelectionState:
                break;
            case ReturnState.EditState:
                ReturnSelectionState();
                break;
            case ReturnState.BuildState:
                ReturnEditState();
                break;
        }
    }


    private void OpenServerMessage(bool result)
    {
        serverMessageGameObject.SetActive(true);
        if (result)
        {
            serverMessageText.text = "Playground created successfully!";
        }
        else
        {
            serverMessageText.text = "Playground creation failed due to a Server Error";
        }
    }


    public void CloseServerMessage()
    {
        CloseBlockPanel();
        serverMessageGameObject.SetActive(false);
        CloseCreatePlaygroundPanel();
        OpenViewPlaygroundPanel();
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

    public void SelectPlayground()
    {
        selectedPlaygroundsbuttons.SetActive(true);
        noSelectedPlaygroundsText.SetActive(false);
    }

    public void EditPlayground()
    {
        int index = selectedPlaygroundIndex;
        if (index != -1)
        {
            SetHeader(username, playgroundsData[index].plygrd_name);
            OpenEditPanel();
            CloseCreationPanel();
            Debug.Log("The Selected Playground Id: " + playgroundsData[index].id);
        }
        returnButtonState = ReturnState.EditState;
    }

    public void PlayPlayground(int index)
    {
        if (index != -1)
        {
            //Play the playground #TO BE ADDED
        }
    }


    private void AddPlaygroundToView(bool result)
    {
        if (result)
        {
            int lastIndex = GameManager.Instance.GameData.currentUser.playgrounds.Count - 1;
            PlaygroundData plygrd = GameManager.Instance.GetGameData().currentUser.playgrounds[lastIndex];
            Debug.Log("Add to view: " + plygrd.plygrd_name);
            GameObject newPlygrd = Instantiate(buttonPrefabPlaygroundView);
            newPlygrd.transform.SetParent(viewPlaygroundContent, false);
            newPlygrd.transform.GetChild(0).gameObject.name = lastIndex.ToString();
            newPlygrd.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = plygrd.plygrd_name;
            playgroundViewButtons.Add(newPlygrd);
        }
    }


    private void AddAllPlaygrounds()
    {
        RemoveAllPlaygrounds();
        if (playgroundsData.Count != 0)
        {
            //Debug.Log(playgrounds.Count + " " + playgrounds[0].plygrd_name);
            emptyPlaygroundsText.SetActive(false);
            for (int i = 0; i < playgroundsData.Count; i++)
            {
                GameObject newPlygrd = Instantiate(buttonPrefabPlaygroundView);
                newPlygrd.transform.SetParent(viewPlaygroundContent, false);
                newPlygrd.transform.GetChild(0).gameObject.name = i.ToString();
                newPlygrd.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = playgroundsData[i].plygrd_name;
                playgroundViewButtons.Add(newPlygrd);
            }
        }
        else
        {
            Debug.Log("There are no playgroundsData");
            emptyPlaygroundsText.SetActive(true);
        }
    }

    private void SetAllChildren(Transform parent, bool set)
    {
        if (parent.childCount != 0)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                parent.GetChild(i).gameObject.SetActive(set);
            }
        }
    }

    private void CreateDummyData()
    {
        GameData gameData = new GameData();
        gameData.currentUser = new();
        gameData.currentUser.name = "Test User";
        gameData.currentUser.curr_ply_index = 0;
        gameData.currentUser.playgrounds = new();
        PlaygroundData testPlayground = new();
        testPlayground.plygrd_name = "TestPlayground";
        testPlayground.plygrd_desc = "This playground is for testing";
        testPlayground.tiles = new();
        testPlayground.plygrd_size = 25;

        for (int i = 0; i < testPlayground.plygrd_size; i++)
        {
            TileData newTile = new(); 
            newTile.tile_index = i;
            newTile.tile_contain = "empty";
            testPlayground.tiles.Add(newTile);  
        }
        GameManager.Instance.GameData = gameData;
    }



}
