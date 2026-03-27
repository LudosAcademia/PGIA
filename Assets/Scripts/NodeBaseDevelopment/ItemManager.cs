using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private PrefabDatabase prefabDatabase;
    [SerializeField] private TMP_Dropdown prefabDropdown;
    [SerializeField] private TMP_InputField itemName;
    [SerializeField] private TMP_Dropdown itemType;

    private void Awake()
    {
        //AssignPrefabs();
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

}

