using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class TransferMap : MonoBehaviour
{
    public string transferMapName;
    public void OnClick()
    {
        //SceneManager.LoadSceneAsync(transferMapName);
        switch (transferMapName)
        {
            case "Main":

                LoadingSceneController.Instance.LoadScene(SceneName.Main);
                break;
            default:
                break;
        }
        //builinSave.BuildingLoad();
    }
}