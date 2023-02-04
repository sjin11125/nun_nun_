using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour
{
    public Text cardName; // 카드이름도 텍스트로
    public static List<string> AchieveNuniName = new List<string>();

    // 카드의 정보를 초기화
    public Card CardUISet(Card card)  //변경
    {

        card =new Card( GameManager.Instance.NuniInfo[card.cardImage]);
        cardName.text = card.cardName;

        AchieveNuniName.Add(card.cardName);
        for (int j = 0; j < AchieveNuniName.Count - 1; j++)
        {
            if (AchieveNuniName[j] != AchieveNuniName[AchieveNuniName.Count - 1])
            {
                int goalCount = CanvasManger.achieveContNuniIndex[15];
                switch (goalCount)
                {
                    case 0:
                        if (AchieveNuniName.Count >= 3)
                        {
                            CanvasManger.currentAchieveSuccess[15] = true;
                        }
                        break;
                    case 1:
                        if (AchieveNuniName.Count >= 6)
                        {
                            CanvasManger.currentAchieveSuccess[15] = true;
                        }
                        break;
                    case 2:
                        if (AchieveNuniName.Count >= 10)
                        {
                            CanvasManger.currentAchieveSuccess[15] = true;
                        }
                        break;
                    case 3:
                        if (AchieveNuniName.Count >= 16)
                        {
                            CanvasManger.currentAchieveSuccess[15] = true;
                        }
                        break;
                    case 4:
                        if (AchieveNuniName.Count >= 24)
                        {
                            CanvasManger.currentAchieveSuccess[15] = true;
                        }
                        break;
                    default:
                        CanvasManger.currentAchieveSuccess[15] = false;
                        break;
                }
            }
            else
            {
                AchieveNuniName.RemoveAt(AchieveNuniName.Count - 1);
            }
        }


        Card result = new Card(card);
        return result;
    }
}
