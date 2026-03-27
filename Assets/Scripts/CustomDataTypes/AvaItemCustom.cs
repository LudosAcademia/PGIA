using System;
using UnityEngine;

[Serializable]
public class AvaItemCustom : MonoBehaviour
{
    public string layer;
    public int index;
    public Guid containId;
    public GameEnums.ItemType type;
    public bool avalible = true;
}
