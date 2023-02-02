using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class ShopScene : MonoBehaviour
{
    // Start is called before the first frame update
    public Button HomeBtn;
    void Start()
    {
        HomeBtn.OnClickAsObservable().Subscribe(_=> {

            LoadingSceneController.Instance.LoadScene(SceneName.Main);

        });
    }

}
