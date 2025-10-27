using System;
using System.Reflection;
using TMPro;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dataText;

    private void Awake()
    {
        GameManager.Instance.GetGameData();
    }

    public void PrintData()
    {
        GameData data = GameManager.Instance.GetGameData();
        Type type = data.GetType();
        string text = "";
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        Debug.Log("PrintData Pressed, Field Length: " + fields.Length);

        for (int i = 0; i < fields.Length; i++)
        {
            object value = fields[i].GetValue(data);
            Debug.Log("" + fields[i].Name + ": " + value);
            text += "" + fields[i].Name + ": " + value + "\n";
        }
        dataText.text = text;
    }


}
