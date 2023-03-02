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
            case "Game":

                LoadingSceneController.Instance.LoadScene(SceneName.Game);
                break;
            case "MiniGame":

                LoadingSceneController.Instance.LoadScene(SceneName.MiniGame);
                break;
            default:
                break;
        }
        //builinSave.BuildingLoad();
    }
}