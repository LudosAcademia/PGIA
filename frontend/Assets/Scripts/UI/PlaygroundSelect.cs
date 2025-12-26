using UnityEngine;

public class PlaygroundSelect : MonoBehaviour
{
    public void SelectThisPlayground()
    {
        int index = int.Parse(transform.GetChild(0).name);
        transform.root.gameObject.GetComponent<GUI>().PlaygroundIndex = index;
        GameManager.Instance.GetGameData().currentUser.curr_ply_index = index;
        transform.root.gameObject.GetComponent<GUI>().UpdatePlaygroundSelection();
    }

}
