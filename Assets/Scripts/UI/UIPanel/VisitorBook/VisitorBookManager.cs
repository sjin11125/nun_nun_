using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;


public class VisitorBookManager : MonoBehaviour
{
    public GameObject VBWindow;
    public GameObject Content;
    //FriendInfo[] ;
    //public VisitorBook VB;

    public GameObject VBPrefab;             //���� ��� ������

    public InputField VBInput;
    public GameObject LoadingNuni;      //�ε� ���� ������
    public Button CloseBtn;

    public void Start()
    {


    }
 
   

    void Response(string json)                          
    {
       /*
        if (string.IsNullOrEmpty(json))
        {
         
            return;
        }
        if (json.Equals("null"))                          //���Ͽ� �ƹ��͵� ����
        {
            Destroy(LoadingNuni);
            return;
        }

        Newtonsoft.Json.Linq.JArray j = Newtonsoft.Json.Linq.JArray.Parse(json);
        Debug.Log("j����: "+j.Count);
      
        for (int i = 0; i < j.Count; i++)
        {
 
          //  VisitorBook friendBuildings;
        //    friendBuildings = JsonUtility.FromJson<VisitorBook>(j[i].ToString());

            GameObject VB = Instantiate(VBPrefab, Content.transform)as GameObject;

            Text[] VBtext = VB.GetComponentsInChildren<Text>();

            VBtext[0].text =friendBuildings.FriendName;
            VBtext[1].text = friendBuildings.FriendMessage;
            VBtext[2].text = friendBuildings.FriendTime;

            Image[] Images= VB.GetComponentsInChildren<Image>();
            Debug.Log("friendBuildings.f_image: "+ friendBuildings.FriendImage);
            for (int k = 0; k < GameManager.AllNuniArray.Length; k++)
            {
                if (GameManager.AllNuniArray[k].cardImage != friendBuildings.FriendImage)
                    continue;
                else
                {
                    Images[1].sprite = GameManager.AllNuniArray[k].Image;
                    
                }
               
            }
        }
        Debug.Log("The End");
        Destroy(LoadingNuni);*/
    }
}
