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
    { -1, 1 },   // �������
    { 1, 1 },   // �����ϴ�
    {-1, -1 },   // �������
    { 1, -1 },   // �����ϴ�
    };

    List<GameObject> ObjPooling = new List<GameObject>();

    GameState gameState = GameState.Start;
    void Start()
    {
        BoardSetting();     //�� ó�� Ÿ�� ����

        Score.AsObservable().Subscribe(score =>
        {
            ScoreText.text = score.ToString();


        });



        Observable.Timer(System.TimeSpan.FromSeconds(1)).Repeat().Subscribe(_ =>
        {
            if (time.Value != 0)
            {


                time.Value--;

                TimerText.text = time.Value.ToString();

                if (time.Value == 0)            //Ÿ�̸� �ð��� ������ ��
                {
                    GameOverPanel.SetActive(true);
                    GetMoneyText.text= Score.Value.ToString();
                    GameOverScoreText.text = Score.Value.ToString();
                    GameManager.Instance.Money.Value += Score.Value;            //������ ������ �߰�
                }
            }
        });
        // CountText.text = ClearCount.ToString();

        OverlapCheck();         //�ߺ� üũ

        gameState = GameState.Playing;

    }
    public GameObject GetObject()           //������ƮǮ���� ��Ȱ��ȭ�� ������Ʈ �ҷ�����
    {
        foreach (var item in ObjPooling)
        {
            if (!item.activeInHierarchy)       //Ȱ��ȭ�� �ȵǾ��ִ� ������Ʈ�� ������?
            {
                //��������

                return item;
            }
        }
        //Ȱ��ȭ�� �ȵǾ��ִ� ������Ʈ�� ���� �� (�׷� ���� ��������)
        GameObject newObj = Instantiate(ShapePrefab,ShapeParent.transform);
        return newObj;
    }


    public void BoardSetting()
    {
        int startIndex = 0;

        time.Value = 60;            //Ÿ�̸� �ð� ����
        Score.Value = 0;            //���� ����

        for (int i = 0; i < Height; i++)
        {
            for (int k = 0; k < Width; k++)
            {
                /*if (!BoardObj[startIndex].activeSelf)
                {
                    Boards[i, k] = null;
                    startIndex++;
                    continue;
                }*/


                GameObject ShapeObj = Instantiate(ShapePrefab, ShapeParent.transform) as GameObject;

                ObjPooling.Add(ShapeObj);           //������Ʈ Ǯ�� ����

                Puzzle newShape = ShapeObj.GetComponent<Puzzle>();

                newShape.SetInfo(new Puzzle(k, i, Random.Range(0, 3), startIndex, PuzzleState.NB));            //��ֺ����� ����0~3
                Boards[i, k] = newShape;

                /*  Vector3 childWorldPos = transform.TransformPoint(new Vector3(
                           (startIndex / 7) * (cellSize.y + spacing.y) + cellSize.y / 2,
                           (startIndex % 7) * (cellSize.x + spacing.x) + cellSize.x / 2,
                          0f
                          ));*/

                ShapeObj.transform.position = BoardObj[startIndex].transform.position;
                startIndex++;
            }
        }
    }
    public void OverlapCheck()
    {

        for (int i = 0; i < Height; i++)
        {
            for (int k = 0; k < Width; k++)
            {
                int rand = 0;

                while (Match2X2(Boards[i, k]) || Match345(Boards[i, k].X, Boards[i, k].Y, Boards[i, k].Index))           //ó���� ��ġ �Ǵ� �� ����
                {
                    do
                    {
                        rand = Random.Range(0, 3);
                    } while (Boards[i, k].ShapeNum == rand);
                    Boards[i, k].ShapeNum = rand;
                    Boards[i, k].Image.sprite = ShapeImages[rand];
                }
            }
        }
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
    public IEnumerator MunchkinBlock(Dir dir, int X, int Y, GameObject MBBlockObj)         //��ġŲ �� ������ �ڷ�ƾ
    {
        Puzzle shapeInfo = Boards[Y, X];
        //int index = shapeInfo.Index;
        GameObject obj;
        switch (dir)
        {
            case Dir.Right:
                obj = BoardObj[Boards[Y, X].Index + (Boards.GetLength(0) - X - 1)];
                yield return StartCoroutine(Boards[Y, X].MBBlockMoveCoroutine(obj, dir));


                Boards[Y, X].GetComponent<Image>().enabled = false;
                Boards[Y, X].GetComponent<BoxCollider2D>().enabled = false;//��ġŲ �� ���� �� ���� ��������
                Boards[Y, X] = null;
                yield return StartCoroutine(DownPuzzle());


                yield return new WaitForSeconds(1);
                MBBlockObj.SetActive(false);
                yield return StartCoroutine(CreatePuzzle());



                //Destroy(MBBlockObj);
                
                break;

            case Dir.Left:
                obj = BoardObj[(Boards.GetLength(0) * Y)];
                yield return StartCoroutine(Boards[Y, X].MBBlockMoveCoroutine(obj, dir));
                Boards[Y, X] = null;
                shapeInfo.GetComponent<Image>().enabled = false;
                shapeInfo.GetComponent<BoxCollider2D>().enabled = false;

                yield return StartCoroutine(DownPuzzle());


                yield return new WaitForSeconds(1);
                MBBlockObj.SetActive(false);
                yield return StartCoroutine(CreatePuzzle());


                //Destroy(MBBlockObj);
                
                break;
            case Dir.Up:
                obj = BoardObj[X];
                yield return StartCoroutine(Boards[Y, X].MBBlockMoveCoroutine(obj, dir));

                Boards[Y, X] = null;
                shapeInfo.GetComponent<Image>().enabled = false;
                shapeInfo.GetComponent<BoxCollider2D>().enabled = false;


                yield return new WaitForSeconds(1);

                MBBlockObj.SetActive(false);
                yield return StartCoroutine(CreatePuzzle());

                //Destroy(MBBlockObj);
                break;
            case Dir.Down:
                obj = BoardObj[shapeInfo.Index + (7 * (Boards.GetLength(0) - Y - 1))];
                yield return StartCoroutine(Boards[Y, X].MBBlockMoveCoroutine(obj, dir));
                Boards[Y, X] = null;
                shapeInfo.GetComponent<Image>().enabled = false;
                shapeInfo.GetComponent<BoxCollider2D>().enabled = false;

                yield return StartCoroutine(DownPuzzle());


                yield return new WaitForSeconds(1);

                MBBlockObj.SetActive(false);
                yield return StartCoroutine(CreatePuzzle());

                //Destroy(MBBlockObj);
                break;
            default:
                obj = null;
                break;
        }
    }
    public IEnumerator Swap(int XDir, int YDir, int tempDir)
    {
        StartCoroutine(SelectedShape.MoveCoroutine(BoardObj[SelectedShape.Index + tempDir]));       //Ÿ�� ���� �ִϸ��̼�
                                                                                                    //Puzzle tempShape = this;
        yield return StartCoroutine(Boards[SelectedShape.Y + YDir, SelectedShape.X + XDir].MoveCoroutine(BoardObj[SelectedShape.Index]));

        Puzzle temp = SelectedShape;
        Boards[SelectedShape.Y, SelectedShape.X] = Boards[SelectedShape.Y + YDir, SelectedShape.X + XDir];
        Boards[SelectedShape.Y + YDir, SelectedShape.X + XDir] = temp;



        Boards[SelectedShape.Y, SelectedShape.X].X -= XDir;
        Boards[SelectedShape.Y, SelectedShape.X].Y -= YDir;
        Boards[SelectedShape.Y, SelectedShape.X].Index -= tempDir;
        SelectedShape.X += XDir;
        SelectedShape.Y += YDir;
        SelectedShape.Index += tempDir;

    }
    public IEnumerator BlockMatchCheck(int XDir, int YDir, Dir tempDir)           //������ ���� Ÿ�ϰ� �ٲ۴�
    {


        yield return StartCoroutine(Swap(XDir, YDir, (int)tempDir));
        ////////------------------------������---------------------------------------
        ///2x2 ���� üũ �� �� y-1�� 0 �̸� OR x+1�� GetLength �̻��̸� 2x2 üũ ���
        ///2x2 �Ʒ��� üũ �� �� y+1�� GetLength �̻� OR x+1�� 0 �̸��̸� 2x2 üũ ���
        // if (SelectedShape.X + 1 >= 0 && SelectedShape.Y + 1 < Boards.GetLength(0))
        // {
        if (Match2X2(SelectedShape) || Match2X2(Boards[SelectedShape.Y - YDir, SelectedShape.X - XDir]))        //(2x2 ��ġ ó��)������ Ÿ�� �ݴ��� ó�����ֱ�
        {
            yield return StartCoroutine(DownPuzzle());
            yield return StartCoroutine(CreatePuzzle());
            // yield return StartCoroutine(DownCreateMatchCheck());
            // yield return StartCoroutine(CreatePuzzle());
        }
        else if (Match345(SelectedShape.X, SelectedShape.Y, SelectedShape.Index) || Match345(SelectedShape.X - XDir, SelectedShape.Y - YDir, SelectedShape.Index - (int)tempDir))
        {      //(3 ��ġ ó��)������ Ÿ�� �ݴ��� ó�����ֱ�
            yield return StartCoroutine(DownPuzzle());
            yield return StartCoroutine(CreatePuzzle());
            //  yield return StartCoroutine(DownCreateMatchCheck());
            //yield return StartCoroutine(CreatePuzzle());
        }
        else            //�ϳ��� �´°� ���ٸ�
        {

            yield return StartCoroutine(Swap(-XDir, -YDir, -(int)tempDir));
        }


    }
    public IEnumerator DownCreateMatchCheck()
    {

        //������ Ÿ�ϵ��� ��ġ�� �Ǵ��� Ȯ��
        for (int i = 0; i < Height; i++)
        {
            for (int k = 0; k < Width; k++)
            {

                while (Match2X2(Boards[i, k]) || Match345(Boards[i, k].X, Boards[i, k].Y, Boards[i, k].Index))
                {
                    yield return StartCoroutine(DownPuzzle());
                    yield return StartCoroutine(CreatePuzzle());
                }
            }
        }
    }
    public bool Match2X2(Puzzle ShapeData)
    {
        bool isMatch = false;
        for (int i = 0; i < 4; i++)
        {
            if (ShapeData.Y + CheckDirections[i, 0] <= 6 && ShapeData.Y + CheckDirections[i, 0] >= 0 &&             //�˻��ϴ� ������ �ε��� ������ �Ѿ�� ������
               ShapeData.X + CheckDirections[i, 1] <= 6 && ShapeData.X + CheckDirections[i, 1] >= 0)
            {
                if (Boards[ShapeData.Y, ShapeData.X + CheckDirections[i, 1]] != null
                    && Boards[ShapeData.Y + CheckDirections[i, 0], ShapeData.X + CheckDirections[i, 1]] != null &&
                    Boards[ShapeData.Y + CheckDirections[i, 0], ShapeData.X] != null)
                {


                    if ((ShapeData.ShapeNum == Boards[ShapeData.Y, ShapeData.X + CheckDirections[i, 1]].ShapeNum) &&
                                   (ShapeData.ShapeNum == Boards[ShapeData.Y + CheckDirections[i, 0], ShapeData.X + CheckDirections[i, 1]].ShapeNum) &&
                                   (ShapeData.ShapeNum == Boards[ShapeData.Y + CheckDirections[i, 0], ShapeData.X].ShapeNum))//�� ��Ī üũ(2x2)
                    {
                        Debug.Log("2x2 �� ��Ī");
                        if (gameState != GameState.Start)
                        {


                            Debug.Log("��Ī�� ������ [" + CheckDirections[i, 0] + ". " + CheckDirections[i, 1] + "]");
                            Boards[ShapeData.Y, ShapeData.X].State = PuzzleState.MB;//��ġŲ ������ �ٲ�s
                            Boards[ShapeData.Y, ShapeData.X].ShapeNum = 4;
                            Boards[ShapeData.Y, ShapeData.X].Image.sprite = ShapeImages[4];
                            Boards[ShapeData.Y, ShapeData.X].gameObject.tag = "MB";

                            Boards[ShapeData.Y, ShapeData.X + CheckDirections[i, 1]].gameObject.SetActive(false);
                            Boards[ShapeData.Y + CheckDirections[i, 0], ShapeData.X + CheckDirections[i, 1]].gameObject.SetActive(false);
                            Boards[ShapeData.Y + CheckDirections[i, 0], ShapeData.X].gameObject.SetActive(false);
                            //Destroy(Boards[ShapeData.Y, ShapeData.X + CheckDirections[i, 1]].gameObject);  //�ٸ� �� ����
                            // Destroy(Boards[ShapeData.Y + CheckDirections[i, 0], ShapeData.X + CheckDirections[i, 1]].gameObject);
                          //  Destroy(Boards[ShapeData.Y + CheckDirections[i, 0], ShapeData.X].gameObject);  //�ٸ� �� ����
                        }
                        isMatch = true;
                        return isMatch;     //�ٷ� �Ѱܹ���

                    }
                    else
                        isMatch = false;
                }
            }
            else
            {
                Debug.Log("������ ���� ������ [" + CheckDirections[i, 0] + ". " + CheckDirections[i, 1] + "]");

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

        //---------------------------������---------------------------------------
        for (int i = 1; i < 7; i++)                //��
        {
            if (y - i >= 0)
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

        for (int i = 1; i < 7; i++)                //��
        {
            if (y + i < 7)
            {
                if (SelectedShape.ShapeNum == Boards[y + i, x].ShapeNum)
                {
                    //RemoveShape.Add(Boards[y + i, x]);
                    DownCount += 1;
                }
                else
                {
                    /*if (DownCount < 2)//�Ʒ��� ���� ������ Ÿ���� ������ 2�̸��̶�� 
                    {
                        DownCount = 0;
                        RemoveShape.Clear();
                    }*/
                    break;
                }
            }
        }

        for (int i = 1; i < 7; i++)                //��
        {
            if (x - i >= 0)
            {
                if (SelectedShape.ShapeNum == Boards[y, x - i].ShapeNum)
                {
                    // RemoveShape.Add(Boards[y, x - i]);
                    LeftCount += 1;
                }
                else
                {
                    /* if (LeftCount < 2)      //�������� ���� ������ Ÿ���� ������ 2�̸��̶�� 
                     {
                         LeftCount = 0;
                         RemoveShape.Clear();
                     }*/
                    break;
                }
            }
        }

        for (int i = 1; i < 7; i++)                //��
        {
            if (x + i < 7)
            {
                if (SelectedShape.ShapeNum == Boards[y, x + i].ShapeNum)
                {
                    //RemoveShape.Add(Boards[y, x + i]);
                    RightCount += 1;
                }
                else
                {
                    /*if (RightCount < 2)             //���������� ���� ������ Ÿ���� ������ 2�̸��̶�� 
                    {
                        RightCount = 0;
                        RemoveShape.Clear();
                    }*/
                    break;
                }
            }
        }

        //RemoveShape.Add(SelectedShape);         //������ Ÿ���� ������ Ÿ�� ����Ʈ�� �־���

        if (RightCount >= 1 && LeftCount >= 1)      //��+��+������ Ÿ���� 3ĭ�̻�
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
        else if (UpCount >= 1 && DownCount >= 1)     //��+�Ʒ�+������ Ÿ���� 3ĭ�̻�
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
                UpCount >= 2 || RightCount >= 2 || LeftCount >= 2 || DownCount >= 2)     //���� Ÿ���� ������ �����¿� ���� 2���̻��϶�
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
    /*  public void DestroyShape(int x, int y,int count)
      {
          for (int i = 0; i <= count; i++)
          {
              if (Boards[y + i, x] != null)
                  Boards[y + i, x].gameObject.SetActive(false);

          }
      }*/
    public IEnumerator DownPuzzle()          //�ؿ��� ���� Ž���� Ÿ���� �ϳ��� ������ ������ �Լ�
    {
        //int index = DestroyShape.Index;
        //int y = DestroyShape.Y;

        int x = 0;

        while (x < 7)
        {
            int y = 6;
            int blank = 0;
            while (y >= 1)
            {
                if (Boards[y, x] == null)
                {
                    if (Boards[y - 1, x] == null)         //�������� ��ĭ�� ������� 
                    {
                        blank += 1;
                        y -= 1;
                        continue;
                    }

                    Boards[y + blank, x] = Boards[y - 1, x];            //������ �ٲٱ�
                    Boards[y - 1, x] = null;
                    Boards[y + blank, x].Y = y + blank;
                    Boards[y + blank, x].Index = x + (y + blank) * 7;

                    StartCoroutine(Boards[y + blank, x].MoveCoroutine(BoardObj[x + (y + blank) * 7]));               //������Ʈ �̵�
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
    public IEnumerator CreatePuzzle()
    {

        int x = 0;

        while (x < 7)
        {
            int y = 6;
            while (y >= 0)
            {

                if (Boards[y, x] == null)
                {

                    GameObject ShapeObj = GetObject();
                    ShapeObj.SetActive(true);
                    ShapeObj.transform.position = BoardObj[x].transform.position;
                    Puzzle newShape = ShapeObj.GetComponent<Puzzle>();
                    newShape.SetInfo(new Puzzle(x, 0, Random.Range(0, 3), x, PuzzleState.NB));

                    Boards[newShape.Y, newShape.X] = newShape;
                    yield return StartCoroutine(DownPuzzle());
                    // break;
                }
                else
                    y--;

            }
            x++;
        }

    }
  
}