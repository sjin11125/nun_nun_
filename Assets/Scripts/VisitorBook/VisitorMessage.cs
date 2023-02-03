using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class VisitorMessage : MonoBehaviour
{
    public Text Name,Message,Time;

    public Image Image;
 
    public void SetMessage(string name, string message,string time,string image)
    {
       
        //�̹��� ����

        Addressables.LoadAssetAsync<Sprite>(image).Completed += (image) =>
        {            //��巹����� �̹��� �ҷ��� �ֱ�
            Image.sprite = image.Result;
            Name.text = name;
            Message.text = message;
            Time.text = time;

        };
    }

}
