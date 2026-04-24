using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private PrefabDatabase prefabDatabase;
    [SerializeField] private TMP_Dropdown prefabDropdown;
    [SerializeField] private TMP_InputField itemName;
    [SerializeField] private TMP_Dropdown itemType;

    [SerializeField] private TMP_InputField newItemNameDesktop;
    [SerializeField] private TMP_InputField newItemNameMobile;
    private AvaItemPreBuild currentItem;
    public static event Action<bool> OnItemChangeEnd;

    private void OnEnable()
    {
        SelectState.OnTileItemSelect += SetCurrentItem;
    }

    private void OnDisable()
    {
        SelectState.OnTileItemSelect -= SetCurrentItem;
    }

    private void SetCurrentItem(AvaItemPreBuild itemRef)
    {
        currentItem = itemRef;
        Debug.Log("Item ref has been assigned");
    }

    public void CreateNewItem()
    {
        ItemData itemData = new ItemData();



        int playgroundIndex = GameManager.Instance.GameData.currentUser.curr_ply_index;
        GameManager.Instance.GameData.currentUser.playgrounds[playgroundIndex].item_data.Add(itemData);
    }

    private void AssignPrefabs()
    {
        prefabDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> optionsData = new();

        foreach (var data in prefabDatabase.prefabData)
        {
            TMP_Dropdown.OptionData option = new();
            option.text = data.prefab.name;
            option.image = data.symbol;
            optionsData.Add(option);
        }
        prefabDropdown.AddOptions(optionsData);
    }

    private void AssignImages()
    {

    }

    public void ApplyItemChangeDesktop()
    {
        if (string.IsNullOrEmpty(newItemNameDesktop.text))
        {
            Debug.LogError("Name cannot be empty");
            return;
        }

        currentItem.itemName = newItemNameDesktop.text;
        OnItemChangeEnd?.Invoke(true);
    }

    public void ApplyItemChangeMobile()
    {
        if (string.IsNullOrEmpty(newItemNameMobile.text))
        {
            Debug.LogError("Name cannot be empty");
            return;
        }

        currentItem.itemName = newItemNameMobile.text;
        OnItemChangeEnd?.Invoke(true);
    }


    public void CancelItemChange()
    {
        OnItemChangeEnd?.Invoke(false);
    }
}

