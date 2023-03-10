using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.AddressableAssets;

public class UIInventoryPanel : UIBase
{    // Start is called before the first frame update
    // Start is called before the first frame update
    public GameObject inventory_prefab;     //인벤토리 활성화된 버튼 프리팹
    public GameObject inventory_nuni_prefab;     //인벤토리 버튼 프리팹
    public Transform Content;
    public GameObject NuniParent;                //누니 오브젝트 부모

    public Button InvenBuildingBtn;
    public Button InvenStrBtn;
    public Button InvenNuniBtn;
    public Button InvenCloseBtn;

    GameObject ActiveBuildingPrefab=null;
    InventoryButton ActiveButton=null;
    public UIInventoryPanel(GameObject UIPrefab)
    {
        UIInventoryPanel r = UIPrefab.GetComponent<UIInventoryPanel>();
        r.Awake();
        r.UIPrefab = UIPrefab;

        r.InstantiatePrefab();
    }

    // Start is called before the first frame update
   public override void Start()
    {
        base.Start();
        if (LoadManager.Instance == null)
            return;
        NuniParent = GameObject.Find("nunis");


        InvenBuildingBtn.OnClickAsObservable().Subscribe(_ =>
        {
            Inventory_Building_Open(false);
        }).AddTo(this);

        InvenStrBtn.OnClickAsObservable().Subscribe(_ =>
        {
            Inventory_Building_Open(true);
        }).AddTo(this);


        InvenNuniBtn.OnClickAsObservable().Subscribe(_ =>
        {
            Inventory_Nuni_Open();
        }).AddTo(this);

        InvenCloseBtn.OnClickAsObservable().Subscribe(_ =>
        {
            // Inventory_Nuni_Open();
            if (ActiveButton != null)           //이전에 인벤토리 클릭한 적이 있나?
            {
                if (LoadManager.Instance.MyBuildings[ActiveButton.temp_building.Id].isLock == "F")       //이전에 배치안한 건물이 있었다면
                {
                    Destroy(ActiveBuildingPrefab);                  //해당 건물의 오브젝트를 삭제

                    //ActiveButton.temp_building.Type = BuildType.Load;

                    ActiveButton.SetNoImage(false);                  //X표시 생기게

                    GridBuildingSystem.current.ClearTempArea(LoadManager.Instance.MyBuildings[ActiveButton.temp_building.Id].area);   //건설모드 ON (타일 초기화)

                }
            }
            Inventory_Exit();

            if (GameManager.Instance.isTuto)
            {
                TutorialsManager.IsGoNext = true;
            }

            this.gameObject.transform.parent.gameObject.SetActive(false);
            Destroy(this.gameObject);
        }).AddTo(this);

    }
    public void Inventory_Building_Open(bool isStr)            //건물 인벤 버튼 눌렀을 때
    {
        Inventory_Exit();           //원래 있던 목록 다 지우기
        foreach (var item in LoadManager.Instance.MyBuildings)
        {
            if (isStr)          //설치물 인벤인가
            {
                if (!item.Value.isStr)          //해당 건물이 설치물이 아니라면 그냥 넘기기
                {
                    continue;
                }
            }
            else                        //건물 인벤인가
            {
                if (item.Value.isStr)          //해당 건물이 설치물이 아니라면 그냥 넘기기
                {
                    continue;
                }
            }

            if (item.Value.Id != "ii1y1")         //분수가 아니고 설치물이 아니라면
            {

                Debug.Log(item.Value.Id);
                GameObject inven = Instantiate(inventory_prefab, Content) as GameObject;         //인벤 버튼 프리팹 생성


                InventoryButton inventoryBtn = inven.GetComponent<InventoryButton>();

                Addressables.LoadAssetAsync<Sprite>(item.Value.Building_Image).Completed += (image) =>
                {            //어드레서블로 이미지 불러서 넣기
                    //ButtonImage.sprite = image.Result;
                    inventoryBtn.SetButtonImage(image.Result);   //버튼 이미지 설정
                };
               
                if (LoadManager.Instance.MyBuildings[item.Value.Id].isLock == "T")
                {

                    inventoryBtn.SetNoImage(true);                  //X표시 안생기게
                }
                else
                {
                    inventoryBtn.SetNoImage(false);                  //X표시 생기게
                }

                

                Building building = item.Value;
                inventoryBtn.temp_building=building;                           //해당 건물 정보 등록
                //인벤토리 버튼에 건물 정보 넣어주기(ex. 건물 사진 등);

                inventoryBtn.temp_building =LoadManager.Instance.MyBuildings[item.Value.Id];

      

                inventoryBtn.button.OnClickAsObservable().Subscribe(_ =>                     //인벤토리 건물 클릭 구독
                {
                    inventoryBtn.temp_building=LoadManager.Instance.MyBuildings[inventoryBtn.temp_building.Id];
                    if (ActiveButton != null)           //이전에 인벤토리 클릭한 적이 있나?
                    {
                        if (LoadManager.Instance.MyBuildings[ActiveButton.temp_building.Id].isLock == "F")       //이전에 배치안한 건물이 있었다면
                        {
                            Destroy(ActiveBuildingPrefab);                  //해당 건물의 오브젝트를 삭제

                            //ActiveButton.temp_building.Type = BuildType.Load;

                            ActiveButton.SetNoImage(false);                  //X표시 생기게

                            GridBuildingSystem.current.ClearTempArea(LoadManager.Instance.MyBuildings[ActiveButton.temp_building.Id].area);   //건설모드 ON (타일 초기화)

                        }
                    }
                    if (inventoryBtn.temp_building.isLock == "T")         //해당 건물이 설치되었으면
                    {
                        inventoryBtn.temp_building.Type = BuildType.Load;       //BuildType을 Load로 
                        GridBuildingSystem.OnEditMode.OnNext(inventoryBtn.temp_building);  //건설모드 ON (타일 초기화)

                        Destroy(GameManager.Instance.BuildingPrefabData[inventoryBtn.temp_building.Id]); //해당 건물 오브젝트 삭제

                        inventoryBtn.temp_building.isLock = "F";                            //배치안된 상태로 바꾸기

                        inventoryBtn.SetNoImage(false);                             //버튼에 X 이미지 넣기

                        inventoryBtn.temp_building.BuildingPosition.x = 0;                            //위치 초기화
                        inventoryBtn.temp_building.BuildingPosition.y = 0;
                        inventoryBtn.temp_building.Placed = false;

                        FirebaseScript.Instance.AddBuilding(inventoryBtn.temp_building.BuildingToJson());            //정보를 서버로 전송
                    }
                    else                               //해당 건물이 설치안되어있으면
                    {
                        inventoryBtn.temp_building.Type= BuildType.Move;

                       

                        LoadManager.Instance.InstantiateBuilding(inventoryBtn.temp_building,ref ActiveBuildingPrefab, () =>      //건물 프리팹 Instantiate하고 콜백 실행
                        {
                            LoadManager.Instance.MyBuildings[inventoryBtn.temp_building.Id]= ActiveBuildingPrefab.GetComponent<Building>();
                            //MyBuildings딕셔너리의 해당 건물 데이터는 Instantiate한 오브젝트의 Buiilding 컴포넌트를 참조

                            GridBuildingSystem.OnEditMode.OnNext(LoadManager.Instance.MyBuildings[inventoryBtn.temp_building.Id]);  //건설모드 ON

                                inventoryBtn.SetNoImage(true);          //버튼 X표시 사라지게

                                ActiveButton = inventoryBtn;
                        });
                    }
                }).AddTo(this);


            }

        }
    }
    public void Inventory_Exit()
    {
        Transform[] Content_Child = Content.GetComponentsInChildren<Transform>();
        for (int i = 1; i < Content_Child.Length; i++)
        {
            Destroy(Content_Child[i].gameObject);
        }
    }
    public void Inventory_Nuni_Open()            //누니 인벤 버튼 눌렀을 때
    {
        Inventory_Exit();

        foreach (var item in GameManager.Instance.CharacterList)
        {
            GameObject inven = Instantiate(inventory_nuni_prefab, Content) as GameObject;         //인벤 버튼 프리팹 생성

            //inven.name = GameManager.Instance.CharacterList[i].cardImage;
            inven.tag = "Inven_Nuni";            //인벤 버튼 태그 설정
            inven.name = item.Value.Id;
            Image ButtonImage = inven.GetComponent<Image>();


            Addressables.LoadAssetAsync<Sprite>(item.Value.cardImage).Completed += (image) =>
            {            //어드레서블로 이미지 불러서 넣기
                ButtonImage.sprite = image.Result;

            };

            //ButtonImage.sprite = GameManager.GetCharacterImage(item.Value.cardImage);

            InventoryButton inventoryBtn = inven.GetComponent<InventoryButton>();
            inventoryBtn.this_nuni = item.Value;

            Button Button = inven.GetComponent<Button>();
            Button.OnClickAsObservable().Subscribe(_ => {
                if (inventoryBtn.this_nuni.isLock == "T")         //해당 누니가 있으면
                {
                    LoadManager.Instance.RemoveNuni(inventoryBtn.this_nuni.Id);//해당 누니 오브젝트 없앰

                    inventoryBtn.this_nuni.isLock = "F";
                    GameManager.Instance.CharacterList[inventoryBtn.this_nuni.Id].isLock = "F";

                    Cardsave nuni = new Cardsave(GameManager.Instance.PlayerUserInfo.Uid, inventoryBtn.this_nuni.cardImage, inventoryBtn.this_nuni.isLock, inventoryBtn.this_nuni.Id);


                    FirebaseScript.Instance.SetNuni(nuni);//서버로 전송
                    inventoryBtn.SetNoImage(false); //버튼에 x표시 함
                }
                else                                            //해당 누니가 없으면
                {
                    GameObject nunObject = Instantiate(GameManager.CharacterPrefab[inventoryBtn.this_nuni.cardImage], NuniParent.transform);//해당 누니 오브젝트 생성
                    LoadManager.Instance.MyNuniPrefab.Add(inventoryBtn.this_nuni.Id, nunObject);
                    inventoryBtn.this_nuni.isLock = "T";
                    GameManager.Instance.CharacterList[inventoryBtn.this_nuni.Id].isLock = "T";

                    Cardsave nuni = new Cardsave(GameManager.Instance.PlayerUserInfo.Uid, inventoryBtn.this_nuni.cardImage, inventoryBtn.this_nuni.isLock, inventoryBtn.this_nuni.Id);
                    FirebaseScript.Instance.SetNuni(nuni);//서버로 전송
                    inventoryBtn.SetNoImage(true); //버튼에 x표시 없앰
                }
            }).AddTo(this);
        }
    }
}
