using System.Drawing;
using UnityEngine;

public class PlaygroundSelect : MonoBehaviour
{
    public void SelectThisPlayground()
    {
        int index = int.Parse(transform.GetChild(0).name);
        Debug.Log("The Selected index on PlaygroundSelect: " + index);
        GameManager.Instance.GameData.currentUser.curr_ply_index = index;
        transform.root.gameObject.GetComponent<PlaygroundUI>().SelectPlayground();
    }

}
