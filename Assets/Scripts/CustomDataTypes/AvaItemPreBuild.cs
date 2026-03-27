using System;
using UnityEngine;

[Serializable]
public class AvaItemPreBuild : MonoBehaviour
{
    public string layer;
    public int index;
    public string itemName;
    public int containId;
    public GameEnums.InteractType type;
    public Guid itemId;
    public bool avalible = true;
}
