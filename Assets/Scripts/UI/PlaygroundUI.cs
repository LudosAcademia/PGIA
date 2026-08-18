using GameEnums;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlaygroundUI : MonoBehaviour
{
    [SerializeField] private GameObject createPlaygroundPanel;
    [SerializeField] private GameObject viewPlaygroundPanel;
    [SerializeField] private Transform viewPlaygroundContent;
    [SerializeField] private GameObject playModePanel;

    [SerializeField] private GameObject buttonPrefabPlaygroundView;


    [SerializeField] private GameObject emptyPlaygroundsText;
    [SerializeField] private GameObject selectedPlaygroundsbuttons;


    [SerializeField] private TextMeshProUGUI titleHeader;
    [SerializeField] private TextMeshProUGUI subjectHeader;
    [SerializeField] private GameObject noSelectedPlaygroundsText;
    [SerializeField] private GameObject creationPanel;
    [SerializeField] private GameObject editPanel;
    [SerializeField] private GameObject mainEditPanel;

    [SerializeField] private GameObject buildingPanel;
    [SerializeField] private GameObject toolsPanel;


    [SerializeField] private GameObject settingsPanel;


    [Header("Selection Panel: ")]
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private GameObject selectedTilePanel;
    [SerializeField] private GameObject editItemButton;
    [SerializeField] private GameObject editInfoPanel;
    [SerializeField] private GameObject editSelectedButtonsPanel;
    [Space(5)]

    [Header("Tile Item Edit: ")]
    [SerializeField] private GameObject tileItemEditPanel;

    [Space(10)]

    [Header("Input Fields: ")]
    [SerializeField] private TMP_InputField playgroundName;
    [SerializeField] private TMP_InputField playgroundDesc;
    [Space(5)]


    [Header("Block Panels: ")]
    [SerializeField] private GameObject serverBlockPanel;
    [SerializeField] private GameObject moveObjectBlockPanel;
    [SerializeField] private GameObject serverMessageGameObject;
    [SerializeField] private TextMeshProUGUI serverMessageText;
    [Space(5)]

    [Header("Dropdowns: ")]
    [SerializeField] private TMP_Dropdown itemDropdownMenu;
    [SerializeField] private TMP_Dropdown toolDropdownMenu;
    [SerializeField] private TMP_Dropdown layerDropdownMenu;
    [SerializeField] private TMP_Dropdown playgroundSize;
    [Space(5)]

    [Header("Default Header Name: ")]
    [SerializeField] private string gameTitle = "G.I.A.";
    private string subjectHeaderText;
    [Space(10)]

    [Header("Confirmation Panel: ")]
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private TextMeshProUGUI confirmationText;
    [Space(10)]


    [Header("Logic Manipulation Panel: ")]
    [SerializeField] private GameObject itemManipulationPanel;
    [SerializeField] private GameObject eventManipulationPanel;
    [SerializeField] private GameObject logicManipulatePanel;
    [SerializeField] private GameObject worldItemsPanel;
    [SerializeField] private GameObject worldItemUIPrefab;
    [SerializeField] private RectTransform worldItemDragParent;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private GameObject UIEvents;
    [SerializeField] private GameObject worldEventPrefab;
    [SerializeField] private GameObject logicEditConfirmPanel;
    [SerializeField] private TextMeshProUGUI logicEditConfirmEventName;
    [Space(10)]




    //[SerializeField] private PlaygroundManager playgroundManager;

    private string username;
    private ReturnState returnButtonState = ReturnState.CreationState;
    private List<GameObject> playgroundViewButtons = new();
    List<PlaygroundData> playgroundsData = new();

    //for creating playgrounds:
    public static Action<string, string, int> OnPlaygroundInputValueChange;
    public static Action<int> OnItemChange;
    public static Action<int> OnToolChange;
    public static Action<int> OnLayerChange;


    private void OnEnable()
    {
        SubscribeListeners();
    }

    private void SubscribeListeners()
    {
        PlaygroundManager.OnStartPlaygroundCreate += OpenCreatePlaygroundPanel;
        PlaygroundManager.OnStartPlaygroundCreate += CloseViewPlaygroundPanel;
        PlaygroundManager.OnCancelPlaygroundCreate += CloseCreatePlaygroundPanel;
        PlaygroundManager.OnCancelPlaygroundCreate += OpenViewPlaygroundPanel;
        PlaygroundManager.OnEndPlaygroundCreate += CloseCreatePlaygroundPanel;
        PlaygroundManager.OnEndPlaygroundCreate += OpenViewPlaygroundPanel;
        PlaygroundManager.OnCloseBlockPanel += CloseServerMessage;
        PlaygroundManager.OnStartPlaygroundEdit += EditPlayground;
        PlaygroundManager.OnGeneralReturn += GeneralReturnButton;
        PlaygroundManager.OnStartPlaygroundDelete += OpenConfirmationDelete;
        PlaygroundManager.OnCancelPlaygroundDelete += CloseConfirmationDelete;
        PlaygroundManager.OnEndPlaygroundDelete += CloseConfirmationDelete;
        PlaygroundManager.OnItemCreateStart += OpenItemManipulationPanel;
        PlaygroundManager.OnEventCreateStart += OpenEventManipulationPanel;
        PlaygroundManager.OnEventCreateWorldPanel += SetWorldItemsPanel;

        PlaygroundManager.OnItemCreateEnd += CloseItemManipulationPanel;
        PlaygroundManager.OnEventCreateEnd += CloseEventManipulationPanel;
        PlaygroundManager.OnStartEventItemPanel += OpenLogicManipulatePanel;
        PlaygroundManager.OnEndEventItemPanel += CloseLogicManipulatePanel;

        PlaygroundManager.OnEditTileItem += OpenTileItemEditPanel;
        PlaygroundManager.OnCancelLogicManagement += CloseLogicEditConfirmPanel;

        PlaygroundManager.OnStartPlayMode += OpenPlayModePanel;
        PlaygroundManager.OnEndPlayMode += ClosePlayModePanel;


        ServerClient.OnServerWait += OpenServerBlockPanel;
        ServerClient.PlaygroundSaved += AddPlaygroundToView;
        ServerClient.PlaygroundSaved += OpenServerMessageOnCreation;
        ServerClient.PlaygroundUpdated += OpenServerMessageOnUpdate;
        ServerClient.PlaygroundDeleted += OpenServerMessageOnDelete;
        ServerClient.PlaygroundDeleted += UpdatePlaygroundView;

        GridManager.OnToolChange += SetToolsText;
        GridManager.OnLayerChange += SetLayersInDropdown;
        GridManager.OnItemsChange += SetItemsInDropdown;

        GridManager.OnItemSelected += SetSelectedTile;
        GridManager.OnItemSelected += ToggleSelectedButtons;

        GridManager.OnMoveObjectStart += SetInfoPanel;
        GridManager.OnMoveObjectStart += ToggleMoveObjectBlockPanel;
        GridManager.OnBuildingSection += OpenBuildingPanel;
        GridManager.OnSelectionSection += OpenSelectionPanel;
        GridManager.OnGridConstFinished += TriggerAllTools;

        SettingsManager.OnSettingsToggle += ToggleSettingsPanel;

        EventManager.OnEventCreated += SetEventsInScrollView;

        ItemManager.OnItemChangeEnd += CloseTileItemEditPanel;

        WorldEventRef.OnStartLogicEditConfirm += OpenLogicEditConfirmPanel;
        WorldEventRef.OnEndLogicEditConfirm += CloseLogicEditConfirmPanel;
    }

    public void UnsubscribeListeners()
    {
        PlaygroundManager.OnStartPlaygroundCreate -= OpenCreatePlaygroundPanel;
        PlaygroundManager.OnStartPlaygroundCreate -= CloseViewPlaygroundPanel;
        PlaygroundManager.OnCancelPlaygroundCreate -= CloseCreatePlaygroundPanel;
        PlaygroundManager.OnCancelPlaygroundCreate -= OpenViewPlaygroundPanel;
        PlaygroundManager.OnEndPlaygroundCreate -= CloseCreatePlaygroundPanel;
        PlaygroundManager.OnEndPlaygroundCreate -= OpenViewPlaygroundPanel;
        PlaygroundManager.OnCloseBlockPanel -= CloseServerMessage;
        PlaygroundManager.OnStartPlaygroundEdit -= EditPlayground;
        PlaygroundManager.OnGeneralReturn -= GeneralReturnButton;
        PlaygroundManager.OnStartPlaygroundDelete -= OpenConfirmationDelete;
        PlaygroundManager.OnCancelPlaygroundDelete -= CloseConfirmationDelete;
        PlaygroundManager.OnEndPlaygroundDelete -= CloseConfirmationDelete;
        PlaygroundManager.OnItemCreateStart -= OpenItemManipulationPanel;
        PlaygroundManager.OnEventCreateStart -= OpenEventManipulationPanel;
        PlaygroundManager.OnItemCreateEnd -= CloseItemManipulationPanel;
        PlaygroundManager.OnEventCreateEnd -= CloseEventManipulationPanel;
        PlaygroundManager.OnStartEventItemPanel -= OpenLogicManipulatePanel;
        PlaygroundManager.OnEndEventItemPanel -= CloseLogicManipulatePanel;
        PlaygroundManager.OnEventCreateWorldPanel -= SetWorldItemsPanel;

        PlaygroundManager.OnEditTileItem -= OpenTileItemEditPanel;
        PlaygroundManager.OnCancelLogicManagement -= CloseLogicEditConfirmPanel;

        PlaygroundManager.OnStartPlayMode -= OpenPlayModePanel;
        PlaygroundManager.OnEndPlayMode -= ClosePlayModePanel;


        ServerClient.OnServerWait -= OpenServerBlockPanel;
        ServerClient.PlaygroundSaved -= AddPlaygroundToView;
        ServerClient.PlaygroundSaved -= OpenServerMessageOnCreation;
        ServerClient.PlaygroundUpdated -= OpenServerMessageOnUpdate;
        ServerClient.PlaygroundDeleted -= OpenServerMessageOnDelete;
        ServerClient.PlaygroundDeleted -= UpdatePlaygroundView;

        GridManager.OnToolChange -= SetToolsText;
        GridManager.OnLayerChange -= SetLayersInDropdown;
        GridManager.OnItemsChange -= SetItemsInDropdown;
        GridManager.OnItemSelected -= SetSelectedTile;
        GridManager.OnItemSelected -= ToggleSelectedButtons;
        GridManager.OnMoveObjectStart -= SetInfoPanel;
        GridManager.OnMoveObjectStart -= ToggleMoveObjectBlockPanel;
        GridManager.OnBuildingSection -= OpenBuildingPanel;
        GridManager.OnSelectionSection -= OpenSelectionPanel;
        GridManager.OnGridConstFinished -= TriggerAllTools;

        SettingsManager.OnSettingsToggle -= ToggleSettingsPanel;

        EventManager.OnEventCreated -= SetEventsInScrollView;
        ItemManager.OnItemChangeEnd -= CloseTileItemEditPanel;

        WorldEventRef.OnStartLogicEditConfirm -= OpenLogicEditConfirmPanel;
        WorldEventRef.OnEndLogicEditConfirm -= CloseLogicEditConfirmPanel;
    }


    private void OpenPlayModePanel()
    {
        playModePanel.SetActive(true);
        CloseCreationPanel();
    }

    private void ClosePlayModePanel()
    {
        playModePanel.SetActive(false);
        OpenCreationPanel();
    }

    private void OpenLogicEditConfirmPanel(string text)
    {
        logicEditConfirmPanel.SetActive(true);
        logicEditConfirmEventName.text = text;
        //logicEditConfirmPanel.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    private void CloseLogicEditConfirmPanel()
    {
        logicEditConfirmPanel.SetActive(false);
    }


    private void ResetLogicManipulationPanel()
    {
        CloseEventManipulationPanel();
        CloseItemManipulationPanel();
    }

    private void OpenTileItemEditPanel()
    {
        tileItemEditPanel.SetActive(true);
    }

    private void CloseTileItemEditPanel(bool set)
    {
        tileItemEditPanel.SetActive(false);
    }

    private void OpenLogicManipulatePanel()
    {
        logicManipulatePanel.SetActive(true);
    }

    private void CloseLogicManipulatePanel()
    {
        logicManipulatePanel.SetActive(false);
        ResetLogicManipulationPanel();
    }

    private void OpenItemManipulationPanel()
    {
        itemManipulationPanel.SetActive(true);
    }

    private void CloseItemManipulationPanel()
    {
        itemManipulationPanel.SetActive(false);
    }

    private void OpenEventManipulationPanel()
    {
        eventManipulationPanel.SetActive(true);
    }
    private void CloseEventManipulationPanel()
    {
        eventManipulationPanel.SetActive(false);
    }

    private void OpenConfirmationDelete()
    {
        ToggleConfirmPanel(true, "Are you sure you want to delete the selected playground?");
    }

    private void CloseConfirmationDelete()
    {
        ToggleConfirmPanel(false, "");
    }

    public void ToggleConfirmPanel(bool set, string msg)
    {
        confirmationPanel.SetActive(set);
        confirmationText.text = msg;
    }

    public void OnItemValueChange()
    {
        OnItemChange?.Invoke(itemDropdownMenu.value);
    }

    public void OnToolValueChange()
    {
        OnToolChange?.Invoke(toolDropdownMenu.value);
    }

    public void OnLayerValueChange()
    {
        OnLayerChange?.Invoke(layerDropdownMenu.value);
    }

    public void TriggerAllTools()
    {
        OnItemValueChange();
        OnToolValueChange();
        OnLayerValueChange();
    }

    public void OnPlaygroundValueChange()
    {
        string name = playgroundName.text;
        string desc = playgroundDesc.text;
        int sizeIndex = playgroundSize.value;

        OnPlaygroundInputValueChange?.Invoke(name, desc, sizeIndex);
    }

    public void PlaygroundUIStart()
    {
        username = GameManager.Instance.GameData.currentUser.username;
        SetHeader(gameTitle, username);
        playgroundsData = GameManager.Instance.GetGameData().currentUser.playgrounds;
        AddAllPlaygrounds();
    }

    private void ToggleSettingsPanel(bool set)
    {
        settingsPanel.SetActive(set);
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
        noSelectedPlaygroundsText.SetActive(true);

    }

    private void CloseViewPlaygroundPanel()
    {
        viewPlaygroundPanel.SetActive(false);
        noSelectedPlaygroundsText.SetActive(false);
    }

    private void ToggleMoveObjectBlockPanel(bool set, string none)
    {
        moveObjectBlockPanel.SetActive(set);
    }

    private void OpenServerBlockPanel()
    {
        serverBlockPanel.SetActive(true);
    }

    private void CloseServerBlockPanel()
    {
        serverBlockPanel.SetActive(false);
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

    public void OpenMainEditPanel()
    {
        mainEditPanel.SetActive(true);
    }

    private void OpenBuildingPanel()
    {
        returnButtonState = ReturnState.BuildState;
        buildingPanel.SetActive(true);
        toolsPanel.SetActive(true);
    }

    private void CloseBuildingPanel()
    {
        buildingPanel.SetActive(false);
        toolsPanel.SetActive(false);
    }

    public void OpenSelectionPanel()
    {
        returnButtonState = ReturnState.BuildState;
        selectionPanel.SetActive(true);
    }

    public void CloseSelectionPanel()
    {
        selectionPanel.SetActive(false);
    }

    private void ReturnCreationState()
    {
        returnButtonState = ReturnState.CreationState;
        SetHeader(gameTitle, username);
        CloseEditPanel();
        OpenCreationPanel();
    }

    private void ReturnEditState()
    {
        returnButtonState = ReturnState.EditState;
        //SetHeader(username, );
        CloseBuildingPanel();
        CloseSelectionPanel();
        //OpenSelectionPanel();
        OpenEditPanel();
    }

    private void SetHeader(string title, string subject)
    {
        titleHeader.text = title;
        subjectHeader.text = subject;
    }

    private void GeneralReturnButton()
    {
        //Debug.Log("Test Return Button");
        switch (returnButtonState)
        {
            case ReturnState.CreationState:
                break;
            case ReturnState.EditState:
                ReturnCreationState();
                break;
            case ReturnState.BuildState:
                ReturnEditState();
                break;
        }
    }

    private void OpenServerMessageOnCreation(bool result)
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

    private void OpenServerMessageOnUpdate(bool result)
    {
        serverMessageGameObject.SetActive(true);
        if (result)
        {
            serverMessageText.text = "Playground saved successfully!";
        }
        else
        {
            serverMessageText.text = "Playground save failed due to a Server Error";
        }
    }

    private void OpenServerMessageOnDelete(bool result)
    {
        serverMessageGameObject.SetActive(true);
        if (result)
        {
            serverMessageText.text = "Playground deleted successfully!";
        }
        else
        {
            serverMessageText.text = "Playground delete failed due to a Server Error";
        }
    }

    public void CloseServerMessage()
    {
        CloseServerBlockPanel();
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

    public void EditPlayground(int none, int none2, bool playMode)
    {
        if (playMode)
        {
            int index = GameManager.Instance.GameData.currentUser.curr_ply_index;
            if (index != -1)
            {
                SetHeader(username, playgroundsData[index].plygrd_name);
                //OpenEditPanel();
                CloseCreationPanel();
                //Debug.Log("The Selected Playground Id: " + playgroundsData[index].id);
            }
        }
        else
        {
            int index = GameManager.Instance.GameData.currentUser.curr_ply_index;
            if (index != -1)
            {
                SetHeader(username, playgroundsData[index].plygrd_name);
                OpenEditPanel();
                CloseCreationPanel();
                //Debug.Log("The Selected Playground Id: " + playgroundsData[index].id);
            }
            returnButtonState = ReturnState.EditState;
        }

     
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
            emptyPlaygroundsText.SetActive(false);
            int lastIndex = GameManager.Instance.GameData.currentUser.playgrounds.Count - 1;
            PlaygroundData plygrd = GameManager.Instance.GetGameData().currentUser.playgrounds[lastIndex];
            //Debug.Log("Add to view: " + plygrd.plygrd_name);
            GameObject newPlygrd = Instantiate(buttonPrefabPlaygroundView);
            newPlygrd.transform.SetParent(viewPlaygroundContent, false);
            newPlygrd.transform.GetChild(0).gameObject.name = lastIndex.ToString();
            newPlygrd.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = plygrd.plygrd_name;
            playgroundViewButtons.Add(newPlygrd);
        }
    }

    private void UpdatePlaygroundView(bool result)
    {
        if (result)
        {
            RemoveAllPlaygrounds();
            AddAllPlaygrounds();
        }
    }

    private void AddAllPlaygrounds()
    {
        RemoveAllPlaygrounds();
        if (playgroundsData.Count != 0)
        {
            playgroundViewButtons = new();
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
            //Debug.Log("There are no playgroundsData");
            emptyPlaygroundsText.SetActive(true);
        }
    }

    private void SetToolsText(string[] tools)
    {
        subjectHeaderText = "Layer: " + tools[0] + "\n"
            + "ObjecId: " + tools[1] + "\n"
            + "Tool: " + tools[2] + "\n";

        subjectHeader.text = subjectHeaderText;
    }

    private void SetLayersInDropdown(List<string> layers)
    {
        layerDropdownMenu.ClearOptions();
        layerDropdownMenu.AddOptions(layers);
        //Transform parent = layerDropdownMenu.transform.GetChild(1).GetChild(0).GetChild(0).transform;
        //GameObject button = Instantiate(buttonPrefabAddLayer);
        // button.transform.SetParent(parent, false);
    }

    private void SetItemsInDropdown(ObjectsDatabase items)
    {
        itemDropdownMenu.ClearOptions();
        List<TMP_Dropdown.OptionData> optionsData = new();

        foreach (var item in items.objectData)
        {
            TMP_Dropdown.OptionData option = new();
            option.text = item.Name;
            optionsData.Add(option);
        }
        itemDropdownMenu.AddOptions(optionsData);
    }

    private void SetWorldItemsPanel(PlaygroundGrid playgroundGrid)
    {
        Dictionary<Guid, AvaItemPreBuild> worldItems = playgroundGrid.logicReadyItems;
        //List <AvaItemPreBuild> 

        if (worldItemsPanel.transform.childCount != 0)
        {
            for (int i = 0; i < worldItemsPanel.transform.childCount; i++)
            {
                Destroy(worldItemsPanel.transform.GetChild(i).gameObject);
            }
        }

        if (worldItems.Count == 0)
        {
            return;
        }

        int j = 0;
        foreach (var item in worldItems)
        {
            if (item.Value.avalible)
            {
                GameObject newItem = Instantiate(worldItemUIPrefab);
                newItem.transform.SetParent(worldItemsPanel.transform, false);
                newItem.GetComponentInChildren<TextMeshProUGUI>().text = item.Value.itemName + " " + j;
                newItem.GetComponent<WorldItemRef>().localParent = worldItemsPanel.transform;
                newItem.GetComponent<WorldItemRef>().worldParent = worldItemDragParent;
                newItem.GetComponent<WorldItemRef>().baseParent = worldItemsPanel.transform;
                newItem.GetComponent<WorldItemRef>().itemRef = item.Value;
                newItem.GetComponent<WorldItemRef>().parentRectTransform = worldItemDragParent;
                newItem.GetComponent<WorldItemRef>().inputManager = inputManager;
            }
            j++;
        }
    }

    private void SetEventsInScrollView()
    {

        if (UIEvents.transform.childCount != 0)
        {
            for (int i = 0; i < UIEvents.transform.childCount; i++)
            {
                Destroy(UIEvents.transform.GetChild(i).gameObject);
            }
        }

        int playgroundIndex = GameManager.Instance.GameData.currentUser.curr_ply_index;
        List<EventData> events = GameManager.Instance.GameData.currentUser.playgrounds[playgroundIndex].event_data;
        //worldEventPrefab

        for (int i = 0; i < events.Count; i++)
        {
            GameObject newEventRef = Instantiate(worldEventPrefab);
            newEventRef.GetComponent<WorldEventRef>().inputManager = inputManager;
            newEventRef.GetComponentInChildren<TextMeshProUGUI>().text = events[i].name;
            newEventRef.GetComponent<WorldEventRef>().thisEvent = events[i];
            newEventRef.GetComponent<WorldEventRef>().eventIndex = i;
            newEventRef.transform.SetParent(UIEvents.transform, false);
        }
        CloseLogicManipulatePanel();
    }

    private void SetSelectedTile(bool set, string info, bool interactive)
    {
        selectedTilePanel.SetActive(set);
        editItemButton.SetActive(interactive);
        selectedTilePanel.GetComponentInChildren<TextMeshProUGUI>().text = info;
    }

    private void SetInfoPanel(bool set, string info)
    {
        editInfoPanel.SetActive(set);
        editInfoPanel.GetComponentInChildren<TextMeshProUGUI>().text = info;
    }

    private void ToggleSelectedButtons(bool set, string nul, bool interaction)
    {
        editSelectedButtonsPanel.SetActive(set);
        if (!set)
        {
            SetInfoPanel(set, "Please select an object");
            //selectionPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Please select an object";
        }
        else
        {
            SetInfoPanel(set, "");
            //selectionPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        }
    }

}

/*
 editInfoPanel;
    [SerializeField] private GameObject editSelectedButtonsPanel;


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

 */