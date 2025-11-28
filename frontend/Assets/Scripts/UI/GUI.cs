using TMPro;
using UnityEngine;

public class GUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userDetailText;

    [SerializeField] private GameObject setupPanel, userInfoPanel, interactObjPanel, propObjPanel;

    private void Start()
    {
        UpdateDetailText();
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
    }

    public void CloseSetupPanel()
    {
        setupPanel.SetActive(false);
        userInfoPanel.SetActive(true);
    }

    public void Return()
    {
        CloseSetupPanel();
    }

    private void UpdateDetailText()
    {
        userDetailText.text = " Name: " + GameManager.Instance.GetGameData().currentUser.username
    + "\n Position: " + GameManager.Instance.GetGameData().currentUser.type;

    }

}
