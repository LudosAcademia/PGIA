using System;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class PlaygroundManager : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameObject gridContainer;
    [SerializeField] private GameObject vCam;
    [SerializeField] private InputManager inputManager;

    public GridManager GridManager { get => gridManager; set => gridManager = value; }

    public static event Action<int, int> OnStartPlaygroundEdit;
    public static event Action OnCancelPlaygroundEdit;
    public static event Action OnEndPlaygroundEdit;

    public static event Action OnStartPlaygroundDelete;
    public static event Action OnCancelPlaygroundDelete;
    public static event Action OnEndPlaygroundDelete;

    public static event Action OnStartPlaygroundCreate;
    public static event Action OnCancelPlaygroundCreate;
    public static event Action OnEndPlaygroundCreate;

    public static event Action OnGeneralReturn;
    public static event Action OnCloseBlockPanel;

    public static event Action OnStartLogicManagement;
    public static event Action OnEndLogicManagement;

    public static event Action OnItemCreateStart;
    public static event Action OnItemCreateEnd;

    public static event Action OnEventCreateStart;
    public static event Action<PlaygroundGrid> OnEventCreateWorldPanel;
    public static event Action OnEventCreateEnd;

    public static event Action OnStartEventItemPanel;
    public static event Action OnEndEventItemPanel;


    public void OnOpenEventItemPanel()
    {
        OnStartEventItemPanel?.Invoke();
        vCam.SetActive(false);

    }

    public void OnCloseEventItemPanel()
    {
        OnEndEventItemPanel?.Invoke();
        vCam.SetActive(true);
    }

    public void OnItemCreateOpen()
    {
        OnItemCreateStart?.Invoke();
    }

    public void OnItemCreateClose()
    {
        OnItemCreateStart?.Invoke();
    }

    public void OnEventCreateOpen()
    {
        OnEventCreateStart?.Invoke();
        OnEventCreateWorldPanel?.Invoke(gridManager.PlaygroundGrid);
    }

    public void OnEventCreateClose()
    {
        OnEventCreateEnd?.Invoke();
    }

    public void OnLogicManagementOpen()
    {
        OnItemCreateEnd?.Invoke();
        vCam.SetActive(false);
    }

    public void OnLogicManagementClose()
    {
        OnEndLogicManagement?.Invoke();
        vCam.SetActive(true);
    }

    public void CloseBlockPanel()
    {
        OnCloseBlockPanel?.Invoke();
    }

    public void GeneralReturn()
    {
        OnGeneralReturn?.Invoke();
    }

    public void StartDeletePlayground()
    {
        OnStartPlaygroundDelete?.Invoke();
    }

    public void CancelDeletePlayground()
    {
        OnCancelPlaygroundDelete?.Invoke();
    }

    public void EndDeletePlayground()
    {
        int index = GameManager.Instance.GameData.currentUser.curr_ply_index;
        GameManager.Instance.GameData.lastSavedIndex = index;
        GameManager.Instance.GameData.currentUser.playgrounds[index].delete = true;
        OnEndPlaygroundDelete?.Invoke();
    }

    public void StartEditPlayground()
    {
        int index = GameManager.Instance.GameData.currentUser.curr_ply_index;
        int totalSize = GameManager.Instance.GameData.currentUser.playgrounds[index].plygrd_size;
        int size = (int)Math.Sqrt(totalSize);
        //Debug.Log("The index: " + index + "The Size: " + size);
        if (gridContainer.transform.childCount != 0)
        {
            for (int i = 0; i < gridContainer.transform.childCount; i++)
            {
                Destroy(gridContainer.transform.GetChild(i).gameObject);
            }
        }
        OnStartPlaygroundEdit?.Invoke(size, index);
        vCam.SetActive(true);
    }

    public void CancelEditPlayground()
    {
        vCam.SetActive(false);
        if (gridContainer.transform.childCount != 0)
        {
            for (int i = 0; i < gridContainer.transform.childCount; i++)
            {
                Destroy(gridContainer.transform.GetChild(i).gameObject);
            }
        }
        OnCancelPlaygroundEdit?.Invoke();
    }

    public void EndEditPlayground()
    {
        vCam.SetActive(false);
        int index = GameManager.Instance.GameData.currentUser.curr_ply_index;
        gridManager.SaveGrid(index);
        GameManager.Instance.GameData.currentUser.playgrounds[index].update = true;
        OnEndPlaygroundEdit?.Invoke();
    }


    //Starts the creation process for a new playground
    public void StartCreatePlayground()
    {
        OnStartPlaygroundCreate?.Invoke();
    }

    //Cancels the creation process for a new playground
    public void CancelCreatePlayground()
    {
        OnCancelPlaygroundCreate?.Invoke();
    }

    public void EndCreatePlayground()
    {
        OnEndPlaygroundCreate?.Invoke();
    }

}

/*
 
       private Vector3Int lastDetectedPosition = Vector3Int.zero;
    [SerializeField] private Grid grid;
    private PlaygroundGrid playgroundGrid;
    public void CreateGrid()
    {
        //Creates a grid that is 10x10
        playgroundGrid = new PlaygroundGrid(10);
        Debug.Log("Grid is created");
    }

    public void TestGetTile(int index)
    {
        for (int i = 0; i < index; i++)
        {
            Debug.Log($"The tile {i} contains " + playgroundGrid.GetTileContain(i));
        }
    }

    public void TestSetTile()
    {
        int size = playgroundGrid.GridSize * playgroundGrid.GridSize;
        for (int i = 0; i < size; i++)
        {
            playgroundGrid.PlaceTile(i, "chair");
        }
        Debug.Log("Grid created and chairs placed");
        for (int i = 0; i < size; i++)
        {
            if (playgroundGrid.GetTileContain(i) != "empty")
            {
                GameObject newChair = Instantiate(chairObject);
                newChair.transform.position = new Vector3(playgroundGrid.GetTileCord(i)[0], 0, playgroundGrid.GetTileCord(i)[1]);
            }
        }
    }

 
 */