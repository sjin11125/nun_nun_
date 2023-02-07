using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class LoginScene : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public List<LoginMenu> LoginMenus;
    void Start()
    {
        foreach (var item in LoginMenus)
        {
            item.LoginBtn.OnClickAsObservable().Subscribe(_=> {

                switch (item.Type)
                {
                    case LoginType.Google:
                        FirebaseScript.Instance.SignInWithGoogle();
                        break;

                    case LoginType.Anonymously:
                        FirebaseScript.Instance.SignInWithAnonyMously();
                        break;

                    default:
                        break;
                }

            });
        }
    }


}
