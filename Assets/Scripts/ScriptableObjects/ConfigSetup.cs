using UnityEngine;

[CreateAssetMenu(fileName = "ConfigSetup", menuName = "Scriptable Objects/ConfigSetup")]
public class ConfigSetup : ScriptableObject
{
    public bool production = false;
    public string productionFilePath;
    public string testFilePath;

}
