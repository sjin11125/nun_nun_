using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using Random = System.Random;

[Serializable]
public class QuestInfo   //���� ��ũ��Ʈ�� ����� ����Ʈ ����
{
    public string quest,count,title;           //����Ʈ�ڵ�, Ƚ��

}
public class QuestManager : MonoBehaviour
{


    TextAsset csvData;             //����Ʈ ����Ʈ(�ϴ� 3���� �ϰ� ���߿� �߰���)
    List<QuestInfo> Questlist;
    public Text[] QuestText;         //����Ʈ �ؽ�Ʈ��
    QuestInfo[] Quest;

    QuestInfo[] QuestArray=new QuestInfo[3];          //����Ʈ �����Ȳ �迭
    List<QuestInfo> GetQuestList = new List<QuestInfo>();
    bool isStart = false;

    string isReset;           //���� �ʱ�ȭ ����
    // Start is called before the first frame update


    public void QuestSave(string quest)
    {
        WWWForm form1 = new WWWForm();
        form1.AddField("order", "questSave");
        form1.AddField("player_nickname", GameManager.NickName);
        form1.AddField("quest", quest);
        form1.AddField("isReset", isReset);
        form1.AddField("time", DateTime.Now.ToString("yyyy.MM.dd"));

      

    }
    public void QuestClick()            //����Ʈ ��ư Ŭ������ �� ����Ʈ ���� ��Ȳ �ҷ���
    {
        for (int i = 0; i < 3; i++)
        {
            WWWForm form1 = new WWWForm();                                      //�����Ȳ �ҷ���
            form1.AddField("order", "questGet");
            form1.AddField("player_nickname", GameManager.NickName);

        }
       
    }

   
    void Response_Time(string json)                          //����Ʈ �ʱ�ȭ Ȯ��
    {
        Debug.Log(json);
       

        if (string.IsNullOrEmpty(json))
        {
            Debug.Log(json);
            return;
        }

        if (json == DateTime.Now.ToString("yyyy.MM.dd"))                         //�ҷ��� ��¥�� ���� ��¥�� �ʱ�ȭ ���� true
        {
            //GameManager.isReset = true;
            isReset = "true";
            QuestClick();    //�ʱ�ȭ ������ ���� ��ũ��Ʈ���� �����Ȳ�� �ҷ����� ���� ��ũ��Ʈ�� ���ó�¥ �־�

            return;
        }
        else                                                                    //�ҷ��� ��¥�� ������ �ƴϰ�(�ʱ�ȭ ���ߴٸ�) ���� �����ߴٸ� false
        {
            isReset = "false";
            // GameManager.isReset = false;
        }
        isStart = true;


    }
        void Response(string json)                          //����Ʈ �����Ȳ �ҷ�����
    {
        //List<QuestInfo> Questlist = new List<QuestInfo>();
        Debug.Log("Quest: "+json);
        if (json=="null")
        {
            return;
        }
        if (string.IsNullOrEmpty(json))
        {
            Debug.Log(json);
            return;
        }
        
        QuestInfo questInfo = JsonUtility.FromJson<QuestInfo>(json);
        
        GetQuestList.Add(questInfo);
        Debug.Log(GetQuestList.Count);
    }

    public void QuestExit()
    {
        GetQuestList.Clear();               //����Ʈ �����Ȳ ����Ʈ �ʱ�ȭ
    }
}
