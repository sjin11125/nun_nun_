using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class AchieveScroll : MonoBehaviour
{
    public Text AchieveName;
    //public Text AchieveContext;
    public Text AchieveCount;

    public List<Text> CountText;
    public Slider CountSlider;

    public Button RewardBtn;
    public Image RewardImage;

    public Sprite IStoneSprite,StoneSprite,ZemSprite;

    public GameObject isRewardImage;
    // Start is called before the first frame update
    private void Start()
    {
            
    }
    public void SetData(AchieveInfo Info)
    {
        int index,count;
        
        if (GameManager.Instance.MyAchieveInfos.ContainsKey(Info.Id))
        {
            index = GameManager.Instance.MyAchieveInfos[Info.Id].Index;
            count = GameManager.Instance.MyAchieveInfos[Info.Id].Count;
            if (GameManager.Instance.MyAchieveInfos[Info.Id].isReward[GameManager.Instance.MyAchieveInfos[Info.Id].Index]=="true")          //������ ���� �� ������
            {
                isRewardImage.SetActive(true);  
            }
        }
        else
        {
            index = 0;
            count = 0;
        }

        AchieveName.text = Info.AchieveName;
        //AchieveContext.text = Info.Context;
       
        AchieveCount.text = count+ "/"+ Info.Count[index].ToString();          //�� ���� ī��Ʈ/�� ī��Ʈ

        CountSlider.maxValue = Info.Count[index];//�� ī��Ʈ
        //CountSlider.minValue= 0;
        CountSlider.value = count;//�� ���� ī��Ʈ
        switch (Info.RewardType[index])
        {
            case "Money":
                RewardImage.sprite = StoneSprite;
                break;
            case "ShinMoney":
                RewardImage.sprite = IStoneSprite;
                break;
            case "Zem":
                RewardImage.sprite = ZemSprite;
                break;

            default:
                break;
        }
        RewardBtn.OnClickAsObservable().Subscribe(_ => {                //���� �ޱ�
            Debug.Log("Before Money: "+ GameManager.Instance.PlayerUserInfo.Money);

            if (GameManager.Instance.MyAchieveInfos[Info.Id].isReward[GameManager.Instance.MyAchieveInfos[Info.Id].Index] == "false")
                return;
            
            switch (GameManager.Instance.AchieveInfos[Info.Id].RewardType[GameManager.Instance.MyAchieveInfos[Info.Id].Index])
            {
                case "Money":
                    int money = int.Parse(GameManager.Instance.PlayerUserInfo.Money);
                    money+= int.Parse(GameManager.Instance.AchieveInfos[Info.Id].Reward[GameManager.Instance.MyAchieveInfos[Info.Id].Index]);
                    GameManager.Instance.PlayerUserInfo.Money = money.ToString();

                    Debug.Log("After Money: " + GameManager.Instance.PlayerUserInfo.Money);
                    break;

                case "ShinMoney":
                    int shinmoney = int.Parse(GameManager.Instance.PlayerUserInfo.ShinMoney);
                    shinmoney += int.Parse(GameManager.Instance.AchieveInfos[Info.Id].Reward[GameManager.Instance.MyAchieveInfos[Info.Id].Index]);
                    GameManager.Instance.PlayerUserInfo.ShinMoney = shinmoney.ToString();
                    break;

                case "Zem":
                    int zem = int.Parse(GameManager.Instance.PlayerUserInfo.Zem);
                    zem += int.Parse(GameManager.Instance.AchieveInfos[Info.Id].Reward[GameManager.Instance.MyAchieveInfos[Info.Id].Index]);
                    GameManager.Instance.PlayerUserInfo.Zem = zem.ToString();
                    break;

                default:
                    break;
            }
            

            GameManager.Instance.MyAchieveInfos[Info.Id].isReward[GameManager.Instance.MyAchieveInfos[Info.Id].Index] = "false";
            GameManager.Instance.MyAchieveInfos[Info.Id].Index += 1;
            isRewardImage.SetActive(false);

            AchieveCount.text = count + "/" + Info.Count[GameManager.Instance.MyAchieveInfos[Info.Id].Index].ToString();          //�� ���� ī��Ʈ/�� ī��Ʈ

            CountSlider.maxValue = Info.Count[GameManager.Instance.MyAchieveInfos[Info.Id].Index];//�� ī��Ʈ
            FirebaseLogin.Instance.SetMyAchieveInfo(); //������ ��� ����
        }).AddTo(this);



    }

}
