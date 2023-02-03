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
       
        //이미지 설정

        Addressables.LoadAssetAsync<Sprite>(image).Completed += (image) =>
        {            //어드레서블로 이미지 불러서 넣기
            Image.sprite = image.Result;
            Name.text = name;
            Message.text = message;
            Time.text = time;

        };
    }

}
