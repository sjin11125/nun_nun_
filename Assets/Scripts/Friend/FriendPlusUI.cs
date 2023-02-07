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


            PlusBtn.OnClickAsObservable().Subscribe(_ =>
            {
                FirebaseScript.Instance.PlusFriend(gameObject.name).ContinueWith((task) =>
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
        //FriendImage.sprite=GameManager.Instance.ima            //�̹��� �ֱ�
        FriendMessage.text = friendInfo.FriendMessage;

        Addressables.LoadAssetAsync<Sprite>(friendInfo.FriendImage).Completed += (image) =>
        {            //��巹����� �̹��� �ҷ��� �ֱ�
            FriendImage.sprite = image.Result;

        };
    }
}
