using TMPro;
using UnityEngine;

public class GUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI userDetailText;

    private void Start()
    {
        UpdateDetailText();
    }

    private void UpdateDetailText()
    {
        userDetailText.text = " Name: " + GameManager.Instance.GetGameData().currentUser.username
    + "\n Position: " + GameManager.Instance.GetGameData().currentUser.type;

    }

}
