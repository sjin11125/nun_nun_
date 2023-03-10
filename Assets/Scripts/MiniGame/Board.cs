using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UniRx;

public class Board : Singleton<Board>
{
    public Sprite[] ShapeImages;

    public int Height, Width = 7;
    public ReactiveProperty<int> Score=new ReactiveProperty<int> ();
    public ReactiveProperty<int> time = new ReactiveProperty<int>();

    public List<Puzzle> NewPuzzle = new List<Puzzle>();
    //ReactiveCollection<Puzzle> NewPuzzle = new ReactiveCollection<Puzzle>();
    public GameObject ShapeParent;
    public GameObject ShapePrefab;

    public Puzzle[,] Boards = new Puzzle[7, 7];
    public GameObject[] BoardObj;

    public Puzzle SelectedShape;


    public Text ScoreText;
    public Text TimerText;
    public GameObject GameOverPanel;
    public Text GameOverScoreText;
    public Text GetMoneyText;

    int[,] CheckDirections =
 {
    { -1, 1 },   // 우측상단
    { 1, 1 },   // 우측하단
    {-1, -1 },   // 좌측상단
    { 1, -1 },   // 좌측하단
    };

    List<GameObject> ObjPooling = new List<GameObject>();

    GameState gameState = GameState.Start;
    void Start()
    {
        BoardSetting();     //맨 처음 타일 세팅

        Score.AsObservable().Subscribe(score =>
        {
            ScoreText.text = score.ToString();


        });

        //타이머 세팅
        Observable.Timer(System.TimeSpan.FromSeconds(1)).Repeat().Subscribe(_ =>
        {
            if (time.Value != 0)
            {


                time.Value--;

                TimerText.text = time.Value.ToString();

                if (time.Value == 0)            //타이머 시간이 끝났을 때
                {
                    GameOverPanel.SetActive(true);
                    GetMoneyText.text = Score.Value.ToString();
                    GameOverScoreText.text = Score.Value.ToString();
                    GameManager.Instance.Money.Value += Score.Value;            //점수를 돈으로 추가
                }
            }
        });

        gameState = GameState.Playing;

      
    }
    public GameObject GetObject()           //오브젝트풀에서 비활성화된 오브젝트 불러오기
    {
        foreach (var item in ObjPooling)
        {
            if (!item.activeInHierarchy)       //활성화가 안되어있는 오브젝트가 있으면?
            {
                //내보내기
                return item;
            }
        }
        //활성화가 안되어있는 오브젝트가 없을 때 (그럴 일은 없겠지만)
        GameObject newObj = Instantiate(ShapePrefab,ShapeParent.transform);
        return newObj;
    }


    public void BoardSetting()
    {
        int startIndex = 0;

        time.Value = 60;            //타이머 시간 세팅
        Score.Value = 0;            //점수 세팅

        for (int i = 0; i < Height; i++)
        {
            for (int k = 0; k < Width; k++)
            {
                GameObject ShapeObj = Instantiate(ShapePrefab, ShapeParent.transform) as GameObject;
                ObjPooling.Add(ShapeObj);           //오브젝트 풀링 대입
                Puzzle newShape = ShapeObj.GetComponent<Puzzle>();
                do
                {
                    newShape.SetInfo(new Puzzle(k, i, Random.Range(0, 5), startIndex, PuzzleState.NB));            //노멀블럭으로 랜덤0~3
                    Boards[i, k] = newShape;
                    ShapeObj.transform.position = BoardObj[startIndex].transform.position;
                } while (Match2X2(newShape) || Match345(newShape.X, newShape.Y, newShape.Index));       //매칭 안될 때까지 루프
            
                
                startIndex++;
            }
        }
    }
   
    public void MatchCheckStart(Dir dir)
   {
        StartCoroutine(Board.Instance.MatchCheck(dir));
    }
    public void MunchkinBlockCheck(Dir dir,int X,int Y,GameObject gameObject)
    {

        StartCoroutine(MunchkinBlock(dir, X, Y, gameObject));
    }
    public IEnumerator MatchCheck(Dir dir)
    {
        switch (dir)
        {
            case Dir.Right:
                yield return StartCoroutine(BlockMatchCheck(1, 0, dir));
                break;
            case Dir.Left:
                yield return StartCoroutine(BlockMatchCheck(-1, 0, dir));
                break;
            case Dir.Up:
                yield return StartCoroutine(BlockMatchCheck(0, -1, dir));
                break;
            case Dir.Down:
                yield return StartCoroutine(BlockMatchCheck(0, 1, dir));
                break;
            default:
                break;
        }
        // yield return StartCoroutine(DownCreateMatchCheck());
    }
    public IEnumerator MunchkinBlock(Dir dir, int X, int Y, GameObject MBBlockObj)         //먼치킨 블럭 굴리기 코루틴
    {
        Puzzle shapeInfo = Boards[Y, X];
        //int index = shapeInfo.Index;
        GameObject obj;
        switch (dir)
        {
            case Dir.Right:
                obj = BoardObj[Boards[Y, X].Index + (Boards.GetLength(0) - X - 1)];
                yield return StartCoroutine(Boards[Y, X].MBBlockMoveCoroutine(obj, dir));


                //shapeInfo.GetComponent<Image>().enabled = false;
                //shapeInfo.GetComponent<BoxCollider2D>().enabled = false;//먼치킨 블럭 삭제 후 퍼즐 내려오기
                Boards[Y, X] = null;
                MBBlockObj.SetActive(false);
                yield return StartCoroutine(DownPuzzle());


                yield return new WaitForSeconds(1);
                
                yield return StartCoroutine(CreatePuzzle());


                /*shapeInfo.GetComponent<Image>().enabled = true;
                shapeInfo.GetComponent<BoxCollider2D>().enabled = true;
                shapeInfo.State = PuzzleState.NB;*/
                //Destroy(MBBlockObj);

                break;

            case Dir.Left:
                obj = BoardObj[(Boards.GetLength(0) * Y)];
                yield return StartCoroutine(Boards[Y, X].MBBlockMoveCoroutine(obj, dir));
                Boards[Y, X] = null;

                MBBlockObj.SetActive(false);
                yield return StartCoroutine(DownPuzzle());


                yield return new WaitForSeconds(1);
             
                yield return StartCoroutine(CreatePuzzle());



                break;
            case Dir.Up:
                obj = BoardObj[X];
                yield return StartCoroutine(Boards[Y, X].MBBlockMoveCoroutine(obj, dir));

                Boards[Y, X] = null;
                // shapeInfo.GetComponent<Image>().enabled = false;
                //shapeInfo.GetComponent<BoxCollider2D>().enabled = false;

                MBBlockObj.SetActive(false);
                yield return new WaitForSeconds(1);

                yield return StartCoroutine(CreatePuzzle());

  
                break;
            case Dir.Down:
                obj = BoardObj[shapeInfo.Index + (7 * (Boards.GetLength(0) - Y - 1))];
                yield return StartCoroutine(Boards[Y, X].MBBlockMoveCoroutine(obj, dir));
                Boards[Y, X] = null;
                // shapeInfo.GetComponent<Image>().enabled = false;
                //shapeInfo.GetComponent<BoxCollider2D>().enabled = false;
                MBBlockObj.SetActive(false);
                yield return StartCoroutine(DownPuzzle());


                yield return new WaitForSeconds(1);

                yield return StartCoroutine(CreatePuzzle());

                break;
            default:
                obj = null;
                break;
        }
    }
    public IEnumerator Swap(int XDir, int YDir, int tempDir)
    {
        StartCoroutine(SelectedShape.MoveCoroutine(BoardObj[SelectedShape.Index + tempDir]));       //타일 무브 코루틴
                                                                                                    //Puzzle tempShape = this;
        yield return StartCoroutine(Boards[SelectedShape.Y + YDir, SelectedShape.X + XDir].MoveCoroutine(BoardObj[SelectedShape.Index]));

        Puzzle temp = SelectedShape;
        Boards[SelectedShape.Y, SelectedShape.X] = Boards[SelectedShape.Y + YDir, SelectedShape.X + XDir];
        Boards[SelectedShape.Y + YDir, SelectedShape.X + XDir] = temp;
        //퍼즐 정보 스왑


        Boards[SelectedShape.Y, SelectedShape.X].X -= XDir;
        Boards[SelectedShape.Y, SelectedShape.X].Y -= YDir;
        Boards[SelectedShape.Y, SelectedShape.X].Index -= tempDir;

        SelectedShape.X += XDir;
        SelectedShape.Y += YDir;
        SelectedShape.Index += tempDir;

    }
    public IEnumerator BlockMatchCheck(int XDir, int YDir, Dir tempDir)           //움직인 쪽의 타일과 바꾸고 매칭 체크
    {
        yield return StartCoroutine(Swap(XDir, YDir, (int)tempDir));        //타일 스왑

        int x = 0, y = 0;
        bool isMove = false;
        do
        {
            if (Match2X2(Boards[y, x]))        //(2x2 매치 처리)
            {
                yield return StartCoroutine(DownPuzzle());          //위에 있던 퍼즐이 아래로 내려오는 코루틴
               yield return StartCoroutine(CreatePuzzle());          //새로운 퍼즐 만드는 코루틴

                x = 0;              //매치가 된다면 첨부터 다시 탐색
                y = 0;
                isMove=true;
                   yield return new WaitForSeconds(1f);
            }
            else if (Match345(Boards[y, x].X, Boards[y, x].Y, Boards[y, x].Index))       //(3 4 5 이상 매치 처리)
            {      //(3 매치 처리)움직인 타일 반대편도 처리해주기
                yield return StartCoroutine(DownPuzzle());          //위에 있던 퍼즐이 아래로 내려오는 코루틴
                yield return StartCoroutine(CreatePuzzle());          //새로운 퍼즐 만드는 코루틴

                x = 0;              //매치가 된다면 첨부터 다시 탐색
                y = 0;
                isMove=true;
                  yield return new WaitForSeconds(1f);
            }
            else            //하나도 맞는게 없다면
            {
                if (!isMove&&x == SelectedShape.X&&y== SelectedShape.Y)
                    yield return StartCoroutine(Swap(-XDir, -YDir, -(int)tempDir));     //스왑했던 퍼즐 다시 원위치로
            }
            x++;
            if (x>=7)
            {
                x = 0;
                y++;
            }
        } while (y!=7);                     //전체 퍼즐에서 매칭체크
    }

    public bool Match2X2(Puzzle ShapeData)
    {
        bool isMatch = false;
        for (int i = 0; i < 4; i++)
        {
            if (ShapeData.Y + CheckDirections[i, 0] <= 6 && ShapeData.Y + CheckDirections[i, 0] >= 0 &&             //검사하는 범위가 인덱스 범위를 넘어서지 않으면
               ShapeData.X + CheckDirections[i, 1] <= 6 && ShapeData.X + CheckDirections[i, 1] >= 0)
            {
                if (Boards[ShapeData.Y, ShapeData.X + CheckDirections[i, 1]] != null
                    && Boards[ShapeData.Y + CheckDirections[i, 0], ShapeData.X + CheckDirections[i, 1]] != null &&
                    Boards[ShapeData.Y + CheckDirections[i, 0], ShapeData.X] != null)
                {


                    if ((ShapeData.ShapeNum == Boards[ShapeData.Y, ShapeData.X + CheckDirections[i, 1]].ShapeNum) &&
                                   (ShapeData.ShapeNum == Boards[ShapeData.Y + CheckDirections[i, 0], ShapeData.X + CheckDirections[i, 1]].ShapeNum) &&
                                   (ShapeData.ShapeNum == Boards[ShapeData.Y + CheckDirections[i, 0], ShapeData.X].ShapeNum))//블럭 매칭 체크(2x2)
                    {
                        Debug.Log("2x2 블럭 매칭");
                        if (gameState != GameState.Start)
                        {


                            Debug.Log("매칭된 방향은 [" + CheckDirections[i, 0] + ". " + CheckDirections[i, 1] + "]");
                            Boards[ShapeData.Y, ShapeData.X].State = PuzzleState.MB;//먼치킨 블럭으로 바꿈s
                            Boards[ShapeData.Y, ShapeData.X].ShapeNum = 5;
                            Boards[ShapeData.Y, ShapeData.X].Image.sprite = ShapeImages[5];
                            Boards[ShapeData.Y, ShapeData.X].gameObject.tag = "MB";

                            Boards[ShapeData.Y, ShapeData.X + CheckDirections[i, 1]].gameObject.SetActive(false);
                            Boards[ShapeData.Y + CheckDirections[i, 0], ShapeData.X + CheckDirections[i, 1]].gameObject.SetActive(false);
                            Boards[ShapeData.Y + CheckDirections[i, 0], ShapeData.X].gameObject.SetActive(false);
                            //Destroy(Boards[ShapeData.Y, ShapeData.X + CheckDirections[i, 1]].gameObject);  //다른 블럭 삭제
                            // Destroy(Boards[ShapeData.Y + CheckDirections[i, 0], ShapeData.X + CheckDirections[i, 1]].gameObject);
                          //  Destroy(Boards[ShapeData.Y + CheckDirections[i, 0], ShapeData.X].gameObject);  //다른 블럭 삭제
                        }
                        isMatch = true;
                        return isMatch;     //바로 넘겨버려

                    }
                    else
                        isMatch = false;
                }
            }
            else
            {
                Debug.Log("범위를 넘은 방향은 [" + CheckDirections[i, 0] + ". " + CheckDirections[i, 1] + "]");

            }
        }
        return isMatch;
    }

    public bool Match345(int x, int y, int index)
    {
        Puzzle SelectedShape = Boards[y, x].GetComponent<Puzzle>();

        int RightCount = 0;
        int LeftCount = 0;
        int UpCount = 0;
        int DownCount = 0;

        //---------------------------수정중---------------------------------------
        for (int i = 1; i < 7; i++)                //상
        {
            if (y - i >= 0&& Boards[y - i, x]!=null)
            {

                if (SelectedShape.ShapeNum == Boards[y - i, x].ShapeNum)
                {
                    UpCount += 1;
                    // RemoveShape.Add(Boards[y - i, x]);
                }
                else
                    break;

            }
        }

        for (int i = 1; i < 7; i++)                //하
        {
            if (y + i < 7&& Boards[y + i, x]!=null)
            {
                if (SelectedShape.ShapeNum == Boards[y + i, x].ShapeNum)
                {
                    //RemoveShape.Add(Boards[y + i, x]);
                    DownCount += 1;
                }
                else
                {
                    /*if (DownCount < 2)//아래로 같은 색상의 타일의 갯수가 2미만이라면 
                    {
                        DownCount = 0;
                        RemoveShape.Clear();
                    }*/
                    break;
                }
            }
        }

        for (int i = 1; i < 7; i++)                //좌
        {
            if (x - i >= 0&& Boards[y, x - i]!=null)
            {
                if (SelectedShape.ShapeNum == Boards[y, x - i].ShapeNum)
                {
                    // RemoveShape.Add(Boards[y, x - i]);
                    LeftCount += 1;
                }
                else
                {
                    /* if (LeftCount < 2)      //왼쪽으로 같은 색상의 타일의 갯수가 2미만이라면 
                     {
                         LeftCount = 0;
                         RemoveShape.Clear();
                     }*/
                    break;
                }
            }
        }

        for (int i = 1; i < 7; i++)                //우
        {
            if (x + i < 7&& Boards[y, x + i]!=null)
            {
                if (SelectedShape.ShapeNum == Boards[y, x + i].ShapeNum)
                {
                    //RemoveShape.Add(Boards[y, x + i]);
                    RightCount += 1;
                }
                else
                {
                    /*if (RightCount < 2)             //오른쪽으로 같은 색상의 타일의 갯수가 2미만이라면 
                    {
                        RightCount = 0;
                        RemoveShape.Clear();
                    }*/
                    break;
                }
            }
        }

        //RemoveShape.Add(SelectedShape);         //선택한 타일을 제거할 타일 리스트에 넣어줌

        if (RightCount >= 1 && LeftCount >= 1)      //왼+오+선택한 타일이 3칸이상
        {
            if (gameState != GameState.Start)
            {

                for (int j = 0; j <= RightCount; j++)
                {

                    //RemoveShape.Add(Boards[y, x + i]);
                    if (Boards[y, x + j] != null)
                        Boards[y, x + j].gameObject.SetActive(false);

                }
                for (int j = 1; j <= LeftCount; j++)
                {
                    //RemoveShape.Add(Boards[y, x - i]);
                    if (Boards[y, x - j] != null)
                        Boards[y, x - j].gameObject.SetActive(false);
                }
            }
            return true;
        }
        else if (UpCount >= 1 && DownCount >= 1)     //위+아래+선택한 타일이 3칸이상
        {
            if (gameState != GameState.Start)
            {

                for (int i = 0; i <= UpCount; i++)
                {
                    // RemoveShape.Add(Boards[y-i, x]);
                    if (Boards[y - i, x] != null)
                        Boards[y - i, x].gameObject.SetActive(false);
                }
                for (int i = 0; i <= DownCount; i++)
                {
                    // RemoveShape.Add(Boards[y+i, x ]);
                    if (Boards[y + i, x] != null)
                        Boards[y + i, x].gameObject.SetActive(false);

                }
            }
            return true;

        }
        /* else if ((UpCount >= 2 && LeftCount >= 2)|| (UpCount >= 2 && RightCount >= 2)||
                  (DownCount >= 2 && LeftCount >= 2) || (DownCount >= 2 && RightCount >= 2))
         {
             DestroyShape(RightCount);
             DestroyShape(LeftCount);
             DestroyShape(UpCount);
             DestroyShape(DownCount);
         }*/
        else if ((UpCount >= 2 && LeftCount >= 2) || (UpCount >= 2 && RightCount >= 2) ||
                 (DownCount >= 2 && LeftCount >= 2) || (DownCount >= 2 && RightCount >= 2) ||
                UpCount >= 2 || RightCount >= 2 || LeftCount >= 2 || DownCount >= 2)     //같은 타일의 갯수가 상하좌우 각각 2개이상일때
        {
            if (gameState != GameState.Start)
            {

                for (int i = 0; i <= RightCount; i++)
                {
                    if (Boards[y, x + i] != null)
                        Boards[y, x + i].gameObject.SetActive(false);


                }
                for (int i = 0; i <= LeftCount; i++)
                {
                    if (Boards[y, x - i] != null)
                        Boards[y, x - i].gameObject.SetActive(false);

                }
                for (int i = 0; i <= UpCount; i++)
                {
                    if (Boards[y - i, x] != null)
                        Boards[y - i, x].gameObject.SetActive(false);
                }
                for (int i = 0; i <= DownCount; i++)
                {
                    if (Boards[y + i, x] != null)
                        Boards[y + i, x].gameObject.SetActive(false);

                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public IEnumerator DownPuzzle()          //밑에서 부터 탐색해 타일을 하나씩 밑으로 내리는 함수
    {
        int x = 0;
        while (x < 7)
        {
            int y = 6;
            int blank = 0;
            while (y >= 1)
            {
                if (Boards[y, x] == null)
                {
                    if (Boards[y - 1, x] == null)         //도착지점 윗칸이 비었으면 
                    {
                        blank += 1;
                        y -= 1;
                        continue;
                    }
                    Boards[y + blank, x] = Boards[y - 1, x];            //데이터 바꾸기
                    Boards[y - 1, x] = null;
                    Boards[y + blank, x].Y = y + blank;
                    Boards[y + blank, x].Index = x + (y + blank) * 7;

                    StartCoroutine(Boards[y + blank, x].MoveCoroutine(BoardObj[x + (y + blank) * 7]));               //오브젝트 이동
                    
                    yield return 0;
                    //null;
                    y -= 1;

                }
                else
                    y -= 1;
            }
            
            x++;
        }
    }

    public IEnumerator CreatePuzzle()           //새로운 퍼즐 생성하는 함수
    {
        int x = 0;
        while (x < 7)
        {
            int y = 6;
            while (y >= 0)
            {

                if (Boards[y, x] == null)
                {

                    GameObject ShapeObj = GetObject();  //오브젝트 풀에서 비활성화 된 오브젝트 불러옴
                    ShapeObj.SetActive(true);
                    ShapeObj.transform.position = BoardObj[x].transform.position;
                    ShapeObj.tag = "NB";

                    Puzzle newShape = ShapeObj.GetComponent<Puzzle>();
                    newShape.SetInfo(new Puzzle(x, 0, Random.Range(0, 3), x, PuzzleState.NB));
                    //새로운 퍼즐 정보 세팅

                    Boards[newShape.Y, newShape.X] = newShape;

                    yield return StartCoroutine(DownPuzzle());          //퍼즐 내리는 함수
                    
                    yield return 0;
                }
                else
                    y--;
            }
            x++;
        }

    }
  
}