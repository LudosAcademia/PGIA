using TMPro;
using UnityEngine;

public class NodeUIManager : MonoBehaviour
{

    [SerializeField] private GameObject nodeUIContoller;
    [SerializeField] private GameObject nodeUIInspector;
    [SerializeField] private TextMeshProUGUI nodeInpectorPreview;
    [SerializeField] private GameObject selectedDeleteButton;


    private void OnEnable()
    {
        LogicManager.OnSelected += SetNodeInspectorPreview;
    }

    private void OnDisable()
    {
        LogicManager.OnSelected -= SetNodeInspectorPreview;
    }


    public void SetNodeInspectorPreview(string text)
    {
        nodeInpectorPreview.text = text;
    }




}
