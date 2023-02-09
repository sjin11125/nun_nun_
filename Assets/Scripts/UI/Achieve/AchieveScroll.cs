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
        
        if (GameManager.Instance.MyAchieveInfos.ContainsKey(Info.Id))           //내 업적 정보 딕셔너리에서 해당 업적 정보가 있을때
        {
            index = GameManager.Instance.MyAchieveInfos[Info.Id].Index;         //해당 업적의 몇번째 인덱스인지
            count = GameManager.Instance.MyAchieveInfos[Info.Id].Count;         //카운트 세팅
            if (GameManager.Instance.MyAchieveInfos[Info.Id].isReward[index]=="true")          //보상을 받을 수 있으면
            {
                isRewardImage.SetActive(true);                                  //보상 받을 수 있다는 알림 표시
            }
        }
        else                                        //내 업적 정보 딕셔너리에서 해당 업적 정보가 없을 때
        {
            index = 0;
            count = 0;
        }

        AchieveName.text = Info.AchieveName;        //업적 제목
        //AchieveContext.text = Info.Context;
       
        AchieveCount.text = count+ "/"+ Info.Count[index].ToString();          //내 업적 카운트/총 카운트

        CountSlider.maxValue = Info.Count[index];//총 카운트
        //CountSlider.minValue= 0;
        CountSlider.value = count;//내 업적 카운트
        RewardText.text ="+"+ GameManager.Instance.AchieveInfos[Info.Id].Reward[index];

        switch (Info.RewardType[index])         //보상 이미지 세팅
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

        RewardBtn.OnClickAsObservable().Subscribe(_ => {                //보상 받기 버튼 구독
            Debug.Log("Before Money: "+ GameManager.Instance.Money.Value);

            if (GameManager.Instance.MyAchieveInfos[Info.Id].isReward[index] == "false") 
                return;
            switch (GameManager.Instance.AchieveInfos[Info.Id].RewardType[index])
            {           //해당 업적의 보상 리워드 타입에 따라 
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
            //해당 업적의 인덱스 보상받기 여부를 false로 설정

            GameManager.Instance.MyAchieveInfos[Info.Id].Index += 1;
            index = GameManager.Instance.MyAchieveInfos[Info.Id].Index;
            //해당 업적의 인덱스 추가

            isRewardImage.SetActive(false);
            //보상 받을 수 있다는 알림 표시 삭제

            AchieveCount.text = count + "/" + Info.Count[index].ToString();//내 업적 카운트/총 카운트

            CountSlider.maxValue = Info.Count[index];//총 카운트
            FirebaseScript.Instance.SetMyAchieveInfo(); //서버로 결과 전송

        }).AddTo(this);



    }

}
