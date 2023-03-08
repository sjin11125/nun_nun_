using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Google;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Functions;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;


public class FirebaseScript : MonoBehaviour
{
    public Text infoText;
    public string webClientId = "494831558708-2dq0fqt5ut11d37l24139nad54it8h04.apps.googleusercontent.com";

    private FirebaseAuth auth;
    private GoogleSignInConfiguration configuration;

    FirebaseFirestore db;
    FirebaseFunctions functions;

    public GameObject NickNamePanelPrefab;

    public GameObject LoginPanel;

    public static FirebaseScript _Instance;
    public static FirebaseScript Instance
    {
        get
        {
            if (_Instance == null)
            {
                return null;
            }
            return _Instance;
        }
    }

    private void Awake()
    {

        //PlayerPrefs.DeleteAll();
        if (_Instance == null)
        {
            _Instance = this;
        }
        else if (_Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);  // 아래의 함수를 사용하여 씬이 전환되더라도 선언되었던 인스턴스가 파괴되지 않는다.

        configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };

        CheckFirebaseDependencies();


    }

    public Task<string> GetBuilding(string uid)
    {
        functions = FirebaseFunctions.GetInstance(FirebaseApp.DefaultInstance);
        // Create the arguments to the callable function.
        //Buildingsave test = new Buildingsave("7.349999", "6.875","T", "bunsu_level(Clone)0", "bunsu_level(Clone)","1","F");
        // var data = JsonUtility.ToJson(test);
        SendMessage IdToken = new SendMessage("Send IdToken", uid);

        var function = functions.GetHttpsCallable("getBuilding");
        return function.CallAsync(JsonUtility.ToJson(IdToken)).ContinueWithOnMainThread((task) => {
            return (string)task.Result.Data;
        });
    }
    public Task<string> AddBuilding(Buildingsave building)
    {
        functions = FirebaseFunctions.GetInstance(FirebaseApp.DefaultInstance);
        // Create the arguments to the callable function.
        // Buildingsave test = new Buildingsave("7.349999", "6.875","T", "bunsu_level(Clone)0", "bunsu_level(Clone)","1","F","sd25hr");
        building.Uid = GameManager.Instance.PlayerUserInfo.Uid;
        var data = JsonUtility.ToJson(building);

        var function = functions.GetHttpsCallable("addBuilding");

        return function.CallAsync(data).ContinueWithOnMainThread((task) => {
            Debug.Log("task.Result: " + task.Result);
            return (string)task.Result.Data;
        });
    }
    public Task<string> SetAllMyBuilding()
    {
        functions = FirebaseFunctions.GetInstance(FirebaseApp.DefaultInstance);
        // Create the arguments to the callable function.
        // Buildingsave test = new Buildingsave("7.349999", "6.875","T", "bunsu_level(Clone)0", "bunsu_level(Clone)","1","F","sd25hr");
        if (LoadManager.Instance.MyBuildings.Count == 0)
            return null;


        //building.Uid = GameManager.Instance.PlayerUserInfo.Uid;
        List<Buildingsave> BuildingSaveList = new List<Buildingsave>();
        //LoadManager.Instance.MyBuildings.ToList();
        foreach (var item in LoadManager.Instance.MyBuildings)
        {
            BuildingSaveList.Add(item.Value.BuildingToJson());
        }
        // var data = JsonUtility.ToJson(BuildingSaveList);

        var function = functions.GetHttpsCallable("setAllBuilding");

        return function.CallAsync(JsonHelper.ToJson<Buildingsave>(BuildingSaveList.ToArray())).ContinueWithOnMainThread((task) =>
        {
            Debug.Log("task.Result: " + task.Result);
            return (string)task.Result.Data;
        });

    }
    public Task<string> RemoveBuilding(Buildingsave building)
    {
        functions = FirebaseFunctions.GetInstance(FirebaseApp.DefaultInstance);
        // Create the arguments to the callable function.
        // Buildingsave test = new Buildingsave("7.349999", "6.875","T", "bunsu_level(Clone)0", "bunsu_level(Clone)","1","F","sd25hr");
        building.Uid = GameManager.Instance.PlayerUserInfo.Uid;
        var data = JsonUtility.ToJson(building);

        var function = functions.GetHttpsCallable("deleteBuilding");

        return function.CallAsync(data).ContinueWithOnMainThread((task) => {
            Debug.Log("task.Result: " + task.Result);
            return (string)task.Result.Data;
        });
    }
    private void CheckFirebaseDependencies()
    {
        Debug.Log("CheckFirebaseDependencies Start ");
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result == DependencyStatus.Available)
                    auth = FirebaseAuth.DefaultInstance;
                //else
                // AddToInformation("Could not resolve all Firebase dependencies: " + task.Result.ToString());
            }
            else
            {
                //AddToInformation("Dependency check was not completed. Error : " + task.Exception.Message);
            }
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/UI/UIPanel/LoadingPanel.prefab").Completed += (gameobject) =>
                {

                    UIYesNoPanel LoadingPanel = new UIYesNoPanel(gameobject.Result);




                    GetGameData().ContinueWithOnMainThread((task) =>
                    {
                        Debug.Log("res: " + task.Result);
                        Newtonsoft.Json.Linq.JArray Result = Newtonsoft.Json.Linq.JArray.Parse(task.Result.ToString());


                        Newtonsoft.Json.Linq.JArray AchieveData = Newtonsoft.Json.Linq.JArray.Parse(Result[0].ToString());

                        foreach (var achieve in AchieveData)//업적
                        {
                            AchieveInfo achieveInfo = JsonUtility.FromJson<AchieveInfo>(achieve.ToString());
                            GameManager.Instance.AchieveInfos.Add(achieveInfo.Id, achieveInfo);

                            Debug.Log("id: " + GameManager.Instance.AchieveInfos[achieveInfo.Id].Id);
                        }


                        Newtonsoft.Json.Linq.JArray BuildingData = Newtonsoft.Json.Linq.JArray.Parse(Result[1].ToString());  //빌딩 정보 넣기
                        foreach (var achieve in BuildingData)//건물
                        {
                            BuildingParse buildingInfo = JsonUtility.FromJson<BuildingParse>(achieve.ToString());
                            GameManager.Instance.BuildingInfo.Add(buildingInfo.Building_Image, buildingInfo);

                            Debug.Log("Building_Image: " + GameManager.Instance.BuildingInfo[buildingInfo.Building_Image].Building_Image);
                        }

                        Newtonsoft.Json.Linq.JArray StrData = Newtonsoft.Json.Linq.JArray.Parse(Result[2].ToString());  //설치물 정보 넣기
                        foreach (var achieve in StrData)//설치물
                        {
                            BuildingParse strInfo = JsonUtility.FromJson<BuildingParse>(achieve.ToString());
                            GameManager.Instance.StrInfo.Add(strInfo.Building_Image, strInfo);

                            Debug.Log("StrData: " + GameManager.Instance.StrInfo[strInfo.Building_Image].Building_Image);
                        }

                        Newtonsoft.Json.Linq.JArray NuniData = Newtonsoft.Json.Linq.JArray.Parse(Result[3].ToString());  //누니 정보 넣기
                        foreach (var achieve in NuniData)//누니
                        {
                            CardInfo nuniInfo = JsonUtility.FromJson<CardInfo>(achieve.ToString());
                            GameManager.Instance.NuniInfo.Add(nuniInfo.cardImage, nuniInfo);

                            Debug.Log("nuniData: " + GameManager.Instance.NuniInfo[nuniInfo.cardImage].cardImage);
                        }

                        Destroy(LoadingPanel.UIPrefab);
                        if (PlayerPrefs.HasKey("Uid"))      //로그인한 기록이 있으면
                        {

                            switch (PlayerPrefs.GetString("SignMethod"))
                            {
                                case "Anonymous":
                                    SignInAnon();
                                    break;

                                case "Google":
                                    OnSignIn();
                                    break;

                                default:
                                    break;
                            }
                        }
                        //자동로그인

                        //.ClosePanel();
                        //  Debug.Log(item.Value<string>("Id") + "    "+item.Children.);

                        //GameManager.Instance.GameDataInfos.Add("AchieveData", Newtonsoft.Json.Linq.JArray.Parse(Result[0].ToString()).ToString());

                        //Debug.Log(GameManager.Instance.GameDataInfos["AchieveData"]);

                        //정보 다 넣은 다음에 씬 로드


                    });



                };
            });
            Debug.Log("Done " + task.Result.ToString());
        });

    }

    public void SignInWithGoogle() { OnSignIn(); }

    public void SignInWithAnonyMously()
    {
        SignInAnon();
    }
    public void WirteButton()
    {
        Write();
    }
    public Task<string> GetGameData()
    {
        functions = FirebaseFunctions.GetInstance(FirebaseApp.DefaultInstance);
        var function = functions.GetHttpsCallable("getGameData");

        return function.CallAsync(null).ContinueWithOnMainThread((task) => {

            return (string)task.Result.Data;



        });
    }
    public void GetUserInfo(string idToken)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            LoginPanel.SetActive(false);                            //로그인 패널 지우기
        });

        functions = FirebaseFunctions.GetInstance(FirebaseApp.DefaultInstance);
        var function = functions.GetHttpsCallable("findUser");                  //유저 정보 불러오기

        SendMessage IdToken = new SendMessage("Send IdToken", idToken);

        function.CallAsync(JsonUtility.ToJson(IdToken)).ContinueWithOnMainThread((task) => {
            Debug.Log("res: " + task.Result.Data);

            if (!task.IsFaulted)
            {
                try
                {
                    GameManager.Instance.PlayerUserInfo = JsonUtility.FromJson<UserInfo>((string)task.Result.Data);     //유저 정보 세팅

                    GameManager.Instance.PlayerUserInfo.Uid = idToken;

                    //재화 설정
                    GameManager.Instance.Money.Value = long.Parse(GameManager.Instance.PlayerUserInfo.Money);
                    GameManager.Instance.ShinMoney.Value = long.Parse(GameManager.Instance.PlayerUserInfo.ShinMoney);
                    GameManager.Instance.Zem.Value = long.Parse(GameManager.Instance.PlayerUserInfo.Zem);

                    Addressables.InitializeAsync().Completed += (result) =>
                    {
                        if (result.IsDone)
                            Addressables.LoadAssetAsync<Sprite>(GameManager.Instance.PlayerUserInfo.Image).Completed += (image) =>
                            {            //어드레서블로 이미지 불러서 넣기
                                GameManager.Instance.ProfileImage.Value = image.Result;
                            };

                    };

                    //내 업적 넣기
                    GetMyAchieveInfo(GameManager.Instance.PlayerUserInfo.Uid).ContinueWithOnMainThread((task) => {
                        if (task.IsCompleted)
                        {
                            Newtonsoft.Json.Linq.JArray Result = Newtonsoft.Json.Linq.JArray.Parse(task.Result.ToString());

                            foreach (var achieve in Result)//업적
                            {
                                MyAchieveInfo achieveInfo = JsonUtility.FromJson<MyAchieveInfo>(achieve.ToString());
                                achieveInfo.Uid = GameManager.Instance.PlayerUserInfo.Uid;
                                //achieveInfo.CountRP.Value =int.Parse( achieveInfo.Count);
                                GameManager.Instance.MyAchieveInfos.Add(achieveInfo.Id, achieveInfo);



                                Debug.Log("My id: " + GameManager.Instance.MyAchieveInfos[achieveInfo.Id].Id);
                            }
                            UnityMainThreadDispatcher.Instance().Enqueue(() => {

                                if (GameManager.Instance.PlayerUserInfo.NickName == "")       //닉네임이 설정안되어있다면
                                {
                                    // Debug.Log("");
                                    UINicknamePanel NicknamePanel = new UINicknamePanel(NickNamePanelPrefab);

                                    //SetUserInfo(GameManager.Instance.PlayerUserInfo);

                                    //  NicknamePanel.Callback = () => LoadingSceneController.Instance.LoadScene(SceneName.Main);
                                }
                                else
                                {
                                    LoadingSceneController.Instance.LoadScene(SceneName.Main);
                                }

                            });
                        }
                    });
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    throw;
                }
            }
            else
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => {

                    LoginPanel.SetActive(true);
                    Debug.LogError(task.Result);
                });

            }
        });
    }
    public Task<string> NickNameCheck(string nickname)
    {
        functions = FirebaseFunctions.GetInstance(FirebaseApp.DefaultInstance);
        var function = functions.GetHttpsCallable("nickNameCheck");

        SendMessage IdToken = new SendMessage("Send IdToken", nickname);

        return function.CallAsync(JsonUtility.ToJson(IdToken)).ContinueWithOnMainThread((task) => {

            return (string)task.Result.Data;

        });

    }
    public Task<string> GetMyAchieveInfo(string uid)
    {
        functions = FirebaseFunctions.GetInstance(FirebaseApp.DefaultInstance);
        var function = functions.GetHttpsCallable("getMyAchieveInfo");

        SendMessage IdToken = new SendMessage("Send IdToken", uid);

        return function.CallAsync(JsonUtility.ToJson(IdToken)).ContinueWithOnMainThread((task) => {

            return (string)task.Result.Data;

        });

    }
    public void SetMyAchieveInfo()
    {
        functions = FirebaseFunctions.GetInstance(FirebaseApp.DefaultInstance);
        var function = functions.GetHttpsCallable("setMyAchieveInfo");

        List<MyAchieveInfo> AchieveInfoArray = new List<MyAchieveInfo>();
        foreach (var item in GameManager.Instance.MyAchieveInfos)
        {
            AchieveInfoArray.Add(item.Value);
        }
        Debug.Log("MyAchieve: " + JsonHelper.ToJson<MyAchieveInfo>(AchieveInfoArray.ToArray(), true));
        function.CallAsync(JsonHelper.ToJson<MyAchieveInfo>(AchieveInfoArray.ToArray())).ContinueWithOnMainThread((task) => {
            //JsonUtility.ToJson(AchieveInfoArray.ToArray())
            Debug.Log(task.Result.Data);
        });
    }
    public Task<string> GetVisitorBook(string uid)
    {
        functions = FirebaseFunctions.GetInstance(FirebaseApp.DefaultInstance);
        var function = functions.GetHttpsCallable("getVisitorBook");

        SendMessage IdToken = new SendMessage("Send IdToken", uid);

        return function.CallAsync(JsonUtility.ToJson(IdToken)).ContinueWithOnMainThread((task) => {
            Debug.Log("res: " + task.Result.Data);

            return (string)task.Result.Data;



        });
    }
    public void SetVisitorBook(VisitorBookInfo message)
    {
        functions = FirebaseFunctions.GetInstance(FirebaseApp.DefaultInstance);
        var function = functions.GetHttpsCallable("setVisitorBook");

        function.CallAsync(JsonUtility.ToJson(message)).ContinueWithOnMainThread((task) => {
            Debug.Log("task: " + task.Result.Data);
        });
    }
    public void SetUserInfo(UserInfo userInfo)
    {
        functions = FirebaseFunctions.GetInstance(FirebaseApp.DefaultInstance);
        var function = functions.GetHttpsCallable("setUser");

        function.CallAsync(JsonUtility.ToJson(userInfo)).ContinueWithOnMainThread((task) => {
            Debug.Log("res: " + task.Result.Data);

        });
    }
    public void SetNuni(Cardsave nuni)
    {
        functions = FirebaseFunctions.GetInstance(FirebaseApp.DefaultInstance);
        var function = functions.GetHttpsCallable("setNuni");

        function.CallAsync(JsonUtility.ToJson(nuni)).ContinueWithOnMainThread((task) => {
            Debug.Log("res: " + task.Result.Data);

        });
    }

    public Task<string> GetNuni(string uid)
    {
        functions = FirebaseFunctions.GetInstance(FirebaseApp.DefaultInstance);
        // Create the arguments to the callable function.
        // Cardsave test = new Buildingsave("7.349999", "6.875","T", "bunsu_level(Clone)0", "bunsu_level(Clone)","1","F");
        // var data = JsonUtility.ToJson(test);
        SendMessage IdToken = new SendMessage("Send IdToken", uid);

        var function = functions.GetHttpsCallable("getNuni");
        return function.CallAsync(JsonUtility.ToJson(IdToken)).ContinueWithOnMainThread((task) => {
            return (string)task.Result.Data;
        });
    }
    public Task<string> GetFriend(string uid)
    {
        functions = FirebaseFunctions.GetInstance(FirebaseApp.DefaultInstance);
        // Create the arguments to the callable function.
        // Cardsave test = new Buildingsave("7.349999", "6.875","T", "bunsu_level(Clone)0", "bunsu_level(Clone)","1","F");
        // var data = JsonUtility.ToJson(test);
        SendMessage IdToken = new SendMessage("Send IdToken", uid);

        var function = functions.GetHttpsCallable("getFriend");
        return function.CallAsync(JsonUtility.ToJson(IdToken)).ContinueWithOnMainThread((task) => {
            return (string)task.Result.Data;
        });
    }
    public Task<string> GetRequestFriend(string uid)
    {
        functions = FirebaseFunctions.GetInstance(FirebaseApp.DefaultInstance);
        // Create the arguments to the callable function.
        // Cardsave test = new Buildingsave("7.349999", "6.875","T", "bunsu_level(Clone)0", "bunsu_level(Clone)","1","F");
        // var data = JsonUtility.ToJson(test);
        SendMessage IdToken = new SendMessage("Send IdToken", uid);

        var function = functions.GetHttpsCallable("getRequestFriend");
        return function.CallAsync(JsonUtility.ToJson(IdToken)).ContinueWithOnMainThread((task) => {
            return (string)task.Result.Data;
        });
    }
    public Task<string> PlusFriend(string uid)
    {
        functions = FirebaseFunctions.GetInstance(FirebaseApp.DefaultInstance);

        // SendMessage IdToken = new SendMessage("Send IdToken", uid);
        FriendAddInfo AddInfo = new FriendAddInfo(GameManager.Instance.PlayerUserInfo.Uid, uid);

        var function = functions.GetHttpsCallable("plusFriend");
        return function.CallAsync(JsonUtility.ToJson(AddInfo)).ContinueWithOnMainThread((task) => {
            return (string)task.Result.Data;
        });
    }
    public Task<string> AddFriend(string uid)
    {
        functions = FirebaseFunctions.GetInstance(FirebaseApp.DefaultInstance);

        // SendMessage IdToken = new SendMessage("Send IdToken", uid);
        FriendAddInfo AddInfo = new FriendAddInfo(GameManager.Instance.PlayerUserInfo.Uid, uid);

        var function = functions.GetHttpsCallable("addFriend");
        return function.CallAsync(JsonUtility.ToJson(AddInfo)).ContinueWithOnMainThread((task) => {
            return (string)task.Result.Data;
        });
    }
    public Task<string> GetSearchFriend(string friendName)
    {
        functions = FirebaseFunctions.GetInstance(FirebaseApp.DefaultInstance);
        // Create the arguments to the callable function.
        // Cardsave test = new Buildingsave("7.349999", "6.875","T", "bunsu_level(Clone)0", "bunsu_level(Clone)","1","F");
        // var data = JsonUtility.ToJson(test);
        SendMessage IdToken = new SendMessage("Send IdToken", friendName);

        var function = functions.GetHttpsCallable("searchFriend");
        return function.CallAsync(JsonUtility.ToJson(IdToken)).ContinueWithOnMainThread((task) => {
            return (string)task.Result.Data;
        });
    }
    public Task<string> Write()
    {
        functions = FirebaseFunctions.GetInstance(FirebaseApp.DefaultInstance);
        var function = functions.GetHttpsCallable("addBuilding");
        return function.CallAsync().ContinueWith((task) => {
            return (string)task.Result.Data;
        });
    }
    //-------------------------로그인-----------------------------------
    private void OnSignIn()
    {
        Debug.Log("OnSignIn Start ");
        LoginPanel.SetActive(false);
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        //("Calling SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);

        Debug.Log("OnSignIn End ");
    }
    void SignInAnon()
    {
        auth.SignInAnonymouslyAsync().ContinueWith(task => {

            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);


            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {

                if (!PlayerPrefs.HasKey("Uid"))                                 //로그인한 기록 없으면 PlayerPref에 저장
                {
                    PlayerPrefs.SetString("Uid", newUser.UserId);
                    PlayerPrefs.SetString("SignMethod", "Anonymous");
                }
            });
            GetUserInfo(task.Result.UserId);            //유저 정보 불러오기
        });
    }

    public void OnDisconnect()
    {
        //AddToInformation("Calling Disconnect");
        GoogleSignIn.DefaultInstance.Disconnect();
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    //AddToInformation("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    //AddToInformation("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            //AddToInformation("Canceled");
        }
        else            //로그인 성공
        {
          
            SignInWithGoogle(task.Result.IdToken);

        }
    }
    void SetUserInfo(Task<string> task)
    {
        Debug.Log((string)task.Result);
        try
        {
            Debug.Log("(string)task.Result: " + (string)task.Result);
            GameManager.Instance.PlayerUserInfo = JsonUtility.FromJson<UserInfo>((string)task.Result);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw;
        }
    }
    private void SignInWithGoogle(string idToken)
    {

        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            AggregateException ex = task.Exception;
            if (ex != null)
            {
                if (ex.InnerExceptions[0] is FirebaseException inner && (inner.ErrorCode != 0))
                    Debug.Log("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
            }
            else
            {
                Debug.Log("IDToken: " + task.Result.UserId);
                Debug.Log("Sign In Successful.");

                if (!PlayerPrefs.HasKey("Uid"))                         //로그인한 기록이 있으면 자동 로그인
                {
                    PlayerPrefs.SetString("Uid", task.Result.UserId);
                    PlayerPrefs.SetString("SignMethod", "Google");
                }

                PlayerPrefs.SetString("SignMethod", "Google");
                GetUserInfo(task.Result.UserId);                    //받은 id 토큰으로 유저정보 불러오기
            }
        });

    }

    public void OnSignInSilently()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        Debug.Log("Calling SignIn Silently");

        GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnAuthenticationFinished);
    }

    public void OnGamesSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = true;
        GoogleSignIn.Configuration.RequestIdToken = false;

        Debug.Log("Calling Games SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

}
