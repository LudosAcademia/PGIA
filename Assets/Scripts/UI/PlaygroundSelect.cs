using UnityEngine;

public class PlaygroundSelect : MonoBehaviour
{
    public void SelectThisPlayground()
    {
        int index = int.Parse(transform.GetChild(0).name);
        transform.root.gameObject.GetComponent<PlaygroundUI>().SelectedPlaygroundIndex = index;
        transform.root.gameObject.GetComponent<PlaygroundUI>().SelectPlayground();
        GameManager.Instance.GameData.currentUser.curr_ply_index = index;
    }

}
