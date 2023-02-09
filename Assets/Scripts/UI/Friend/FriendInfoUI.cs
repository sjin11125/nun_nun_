using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.AddressableAssets;

public class FriendInfoUI : MonoBehaviour
{
    // Start is called before the first frame update
   // FriendInfo FriendInfo;
    public Button GoBtn;          //ģ�� ���� ���� ��ư
    public Button RemoveBtn;          //ģ�� ���� ��ư
    

    public Text FriendName;     //ģ�� �г�
    public Text FriendMessage;     //ģ�� �޼���
    public Image FriendImage;       //ģ�� ����

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
    public void Start()
    {

        if (GoBtn!=null)
        {
            GoBtn.OnClickAsObservable().Subscribe(_ => {             //ģ������ �����
                //GameManager.Instance.FriendUid = FriendName.text;
                LoadingSceneController.Instance.LoadScene(SceneName.FriendMain, FriendName.text);
            }).AddTo(this);
        }

        if (RemoveBtn!=null)
        {
            RemoveBtn.OnClickAsObservable().Subscribe(_=> {
            
            }).AddTo(this);
        }
    }
    
}
