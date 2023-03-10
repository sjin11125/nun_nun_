using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.AddressableAssets;

public class FriendPlusUI : MonoBehaviour
{
    // Start is called before the first frame update
    public Button PlusBtn;
    public Button AddBtn;

    public Text FriendName;
    public Text FriendMessage;
    public Image FriendImage;

    public Text PlusTxt;
    public Text AddTxt;
    public Text AddedTxt;


    void Start()
    {
        if (PlusBtn != null)
        {


            PlusBtn.OnClickAsObservable().Subscribe(_ =>            //친구 요청 버튼을 누르면 구독
            {
                FirebaseScript.Instance.PlusFriend(gameObject.name).ContinueWith((task) =>      //요청된 친구 정보 전송
                {
                    Debug.Log("task: " + task.Result);
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {


                        if (task.Result == "fail")
                        {
                            PlusTxt.gameObject.SetActive(false);
                            AddedTxt.gameObject.SetActive(true);
                        }
                        else
                        {
                            PlusTxt.gameObject.SetActive(false);
                            AddTxt.gameObject.SetActive(true);
                        }
                    });
                });
            }).AddTo(this);
        }
        if (AddBtn!=null)
        {
            AddBtn.OnClickAsObservable().Subscribe(_ => {
                FirebaseScript.Instance.AddFriend(gameObject.name).ContinueWith((task) =>
                {
                    Debug.Log("task: " + task.Result);
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {


                        if (task.Result == "fail")
                        {
                            PlusTxt.gameObject.SetActive(false);
                            AddedTxt.gameObject.SetActive(true);
                        }
                        else
                        {
                            PlusTxt.gameObject.SetActive(false);
                            AddTxt.gameObject.SetActive(true);
                        }
                    });
                });
            }).AddTo(this);
        }
    }

    public void SetFriendInfo(FriendInfo friendInfo)
    {
        FriendName.text = friendInfo.FriendName;
        //FriendImage.sprite=GameManager.Instance.ima            //이미지 넣기
        FriendMessage.text = friendInfo.FriendMessage;

        Addressables.LoadAssetAsync<Sprite>(friendInfo.FriendImage).Completed += (image) =>
        {            //어드레서블로 이미지 불러서 넣기
            FriendImage.sprite = image.Result;

        };
    }
}
