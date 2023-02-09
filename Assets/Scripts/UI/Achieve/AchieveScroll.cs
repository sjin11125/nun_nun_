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
    public Text RewardText;

    public Slider CountSlider;

    public Button RewardBtn;
    public Image RewardImage;

    public Sprite IStoneSprite,StoneSprite,ZemSprite;

    public GameObject isRewardImage;

    // Start is called before the first frame update

    public void SetData(AchieveInfo Info)
    {
        int index,count;
        
        if (GameManager.Instance.MyAchieveInfos.ContainsKey(Info.Id))           //�� ���� ���� ��ųʸ����� �ش� ���� ������ ������
        {
            index = GameManager.Instance.MyAchieveInfos[Info.Id].Index;         //�ش� ������ ���° �ε�������
            count = GameManager.Instance.MyAchieveInfos[Info.Id].Count;         //ī��Ʈ ����
            if (GameManager.Instance.MyAchieveInfos[Info.Id].isReward[index]=="true")          //������ ���� �� ������
            {
                isRewardImage.SetActive(true);                                  //���� ���� �� �ִٴ� �˸� ǥ��
            }
        }
        else                                        //�� ���� ���� ��ųʸ����� �ش� ���� ������ ���� ��
        {
            index = 0;
            count = 0;
        }

        AchieveName.text = Info.AchieveName;        //���� ����
        //AchieveContext.text = Info.Context;
       
        AchieveCount.text = count+ "/"+ Info.Count[index].ToString();          //�� ���� ī��Ʈ/�� ī��Ʈ

        CountSlider.maxValue = Info.Count[index];//�� ī��Ʈ
        //CountSlider.minValue= 0;
        CountSlider.value = count;//�� ���� ī��Ʈ
        RewardText.text ="+"+ GameManager.Instance.AchieveInfos[Info.Id].Reward[index];

        switch (Info.RewardType[index])         //���� �̹��� ����
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

        RewardBtn.OnClickAsObservable().Subscribe(_ => {                //���� �ޱ� ��ư ����
            Debug.Log("Before Money: "+ GameManager.Instance.Money.Value);

            if (GameManager.Instance.MyAchieveInfos[Info.Id].isReward[index] == "false") 
                return;
            switch (GameManager.Instance.AchieveInfos[Info.Id].RewardType[index])
            {           //�ش� ������ ���� ������ Ÿ�Կ� ���� 
                case "Money":
      
                    GameManager.Instance.Money.Value += int.Parse(GameManager.Instance.AchieveInfos[Info.Id].Reward[index]);
                  
                    break;

                case "ShinMoney":
                    GameManager.Instance.ShinMoney.Value += int.Parse(GameManager.Instance.AchieveInfos[Info.Id].Reward[index]);
                  
                    break;

                case "Zem":
                    int zem = int.Parse(GameManager.Instance.PlayerUserInfo.Zem);
                    zem += int.Parse(GameManager.Instance.AchieveInfos[Info.Id].Reward[index]);
                    GameManager.Instance.PlayerUserInfo.Zem = zem.ToString();
                    break;

                default:
                    break;
            }
            GameManager.Instance.MyAchieveInfos[Info.Id].isReward[index] = "false";
            //�ش� ������ �ε��� ����ޱ� ���θ� false�� ����

            GameManager.Instance.MyAchieveInfos[Info.Id].Index += 1;
            index = GameManager.Instance.MyAchieveInfos[Info.Id].Index;
            //�ش� ������ �ε��� �߰�

            isRewardImage.SetActive(false);
            //���� ���� �� �ִٴ� �˸� ǥ�� ����

            AchieveCount.text = count + "/" + Info.Count[index].ToString();//�� ���� ī��Ʈ/�� ī��Ʈ

            CountSlider.maxValue = Info.Count[index];//�� ī��Ʈ
            FirebaseScript.Instance.SetMyAchieveInfo(); //������ ��� ����

        }).AddTo(this);



    }

}
