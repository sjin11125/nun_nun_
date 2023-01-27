using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GridScript : MonoBehaviour
{
    public ShapeStorage shapeStorage;
    int columns = 5;
    int rows = 6;
    float squaresGap = 0f;
    public GameObject gridSquare;
    Vector2 startPosition = new Vector2(-375f, 275f);
    float squareScale = 1.5f;
    float everySquareOffset = 12f;

    private Vector2 _offset = new Vector2(0.0f, 0.0f);
    private System.Collections.Generic.List<GameObject> _gridSquares = new System.Collections.Generic.List<GameObject>();
    private LineIndicator _lineIndicator;

    public GameObject gameOver;
    int keepSquareIndex;
    int trashCanIndex;

    public GameObject effectShape;
    private GameObject settigPanel;

    static public int EraserItemTurn = 10;
    static public int ReloadItemTurn = 15;
    static public int NextExchangeItemTurn = 15;
    static public int KeepItemTurn = 30;
    static public int TrashItemTurn = 20;
    static public int RainbowItemTurn = 40;
    static public int ChangeShapeItem = 40;
    static public int ThreeVerticalItem = 30;
    static public int ThreeHorizontalItem = 30;

    public GameObject QuestControll;

    int completeShin = 0;

    public GameObject ComboImg;
    int Combo = 0;
    List<GameObject> comboObject = new List<GameObject>();
    GameObject shapestorageObj;

    private void OnEnable()
    {
        GameEvents.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
    }

    private void OnDisable()
    {
        GameEvents.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
    }

    void Start()
    {
        _lineIndicator = GetComponent<LineIndicator>();
        CreateGrid();
        //SettingKeep();
        settigPanel = GameObject.FindGameObjectWithTag("SettingPanel");
    }

    void Update()
    {
        GetInformation();
    }

    private void CreateGrid()
    {
        SpawnGridSquares();
        SetGridSquaresPositions();
    }

    private void SpawnGridSquares()
    {
        //0, 1, 2, 3, 4,
        //5, 6, 7, 8, 9

        int square_index = 0;

        for (var row = 0; row < rows; ++row)
        {
            for (var column = 0; column < columns; ++column)
            {
                _gridSquares.Add(Instantiate(gridSquare) as GameObject);

                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SquareIndex = square_index;
                _gridSquares[_gridSquares.Count - 1].transform.SetParent(this.transform);
                _gridSquares[_gridSquares.Count - 1].transform.localScale = new Vector3(squareScale, squareScale, squareScale);
                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SetImage(_lineIndicator.GetGridSquareIndex(square_index) % 2 == 0);
                square_index++;
            }
        }
    }

    private void SetGridSquaresPositions()
    {
        int column_number = 0;
        int row_number = 0;
        Vector2 square_gap_number = new Vector2(0.0f, 0.0f);
        bool row_moved = false;

        var square_rect = _gridSquares[0].GetComponent<RectTransform>();

        _offset.x = square_rect.rect.width * square_rect.transform.localScale.x + everySquareOffset;
        _offset.y = square_rect.rect.height * square_rect.transform.localScale.y + everySquareOffset;

        foreach (GameObject square in _gridSquares)
        {
            if (column_number + 1 > columns)
            {
                square_gap_number.x = 0;
                //go to next col
                column_number = 0;
                row_number++;
                row_moved = true;
            }

            var pos_x_offset = _offset.x * column_number + (square_gap_number.x * squaresGap);
            var pos_y_offset = _offset.y * row_number + (square_gap_number.y * squaresGap);

            if (column_number > 0 && column_number % 3 == 0)
            {
                square_gap_number.x++;
                pos_x_offset += squaresGap;
            }

            if (row_number > 0 && row_number % 3 == 0 && row_moved == false)
            {
                row_moved = true;
                square_gap_number.y++;
                pos_y_offset += squaresGap;
            }

            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPosition.x + pos_x_offset,
                startPosition.y - pos_y_offset);
            square.GetComponent<RectTransform>().localPosition = new Vector3(startPosition.x + pos_x_offset,
                startPosition.y - pos_y_offset, 0.0f);

            column_number++;
        }
    }

    public void CheckIfShapeCanBePlaced()
    {
        var squareIndexes = new List<int>();

        foreach (var square in _gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();//�� ������ ������ ������ ���������� ����

            if (gridSquare.Selected && !gridSquare.SquareOccupied)//����� ����ִ� ����
            {
                squareIndexes.Add(gridSquare.SquareIndex);//squareIndexes�� ���� ������ ��ġ �ε��� ����_0-24
                gridSquare.Selected = false;//������ ���� ���ø��ϰ�
            }
        }

        var currentSelectedShape = shapeStorage.GetCurrentSelectedShape();//�������� ������ ���޹���
        if (currentSelectedShape == null) return; //there is no selected shape;

        if (currentSelectedShape.TotalSquareNumber == squareIndexes.Count)//shape��ũ��Ʈ TotalSquareNumber�� squareIndexes�� ����ִ� �� ���� ��
        {
            //Debug.Log(squareIndexes.Count);//�׻� 1�� ����
            foreach (var squareIndex in squareIndexes)//squareIndexes�迭���ִ� �ε�������
            {
                _gridSquares[squareIndex].GetComponent<GridSquare>().PlaceShapeOnBoard();//�� �ε����� ������� �Լ��ѱ�[0-24]_��Ƽ��Ʈ��� ����� ǥ��
            }

            var shapeLeft = 0;

            foreach (var shape in shapeStorage.shapeList)//Shape�� �ϳ��� �˻�
            {
                if (shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())//������ ������϶�_������� �����̰������� ���ϴ°��� �׸��� ����� ������ ����
                {
                    shapeLeft++;
                }
            }

            if (shapeLeft == 0)//������ΰ� ����==�ùٸ� ������� ����
            {
                GameEvents.RequestNewShapes();//���ο� ������ ���� shapeStorage�� ����
            }

            else//������ΰ� �ִ�
            {
                GameEvents.SetShapeInactive();//�����̰�
            }

            //�������� ������ ����� ������
            CheckIfAnyLineIsCompleted();
        }
        else
        {
            GameEvents.MoveShapeToStartPosition();//ó����ġ��
        }
    }

    private void CheckIfLine()
    {
        List<int[]> lines = new List<int[]>();

        //columns
        foreach (var column in _lineIndicator.columnIndexes)//0-5
        {
            lines.Add(_lineIndicator.GetVerticalLine(column));//column�� 0-4_5��
        }

        //rows
        for (var row = 0; row < 5; row++)
        {
            List<int> data = new List<int>(5);
            for (var index = 0; index < 5; index++)
            {
                data.Add(_lineIndicator.line_data[row, index]); //5���� data�� ����
            }
            lines.Add(data.ToArray());//lines�� ����
        }

        var completedLines = CheckIfSquaresAreCompleted(lines);//��(0-5)��(0-5) �������� �� ������ ��ȯ int�� ����
        var totalScores = 0;

        if (completedLines == 0)
        {
            Combo = 0;
            settigPanel.GetComponent<AudioController>().Sound[3].pitch = 1;
            if (GameObject.FindGameObjectsWithTag("Combo") != null)
            {
                comboObject.AddRange(GameObject.FindGameObjectsWithTag("Combo"));
                foreach (var item in comboObject)
                {
                    Destroy(item);
                }
                comboObject.Clear();
            }
            
            if (GameOver())
            {
                gameOver.gameObject.SetActive(true);
                FirebaseLogin.Instance.SetMyAchieveInfo();//���� ��� FireStore�� ����
            }
        }
        else
        {
            Combo += completedLines;
            settigPanel.GetComponent<AudioController>().Sound[3].Play();
            if (Combo > 1)
            {
                settigPanel.GetComponent<AudioController>().Sound[3].pitch += (Combo - 1) * 0.2f;
                for (int i = 0; i < completedLines; i++)
                {
                    Instantiate(ComboImg, this.transform.parent.GetChild(0));
                    totalScores = 10 * Combo;
                }
                //totalScores = 10 * Combo;
            }
            else
            {
                totalScores = 10 * completedLines;
            }
        }
        
        GameEvents.AddScores(totalScores, completeShin);
    }

    public void CheckIfKeepLineIsCompleted()//ŵ�� ���Ͷ� ������ ���⶧���� ���ο� �Լ� ������
    {
        CheckIfLine();
        KeepItemTurn++;
        EraserItemTurn++;
        ReloadItemTurn++;
        NextExchangeItemTurn++;
        TrashItemTurn++;
        RainbowItemTurn++;
        ChangeShapeItem++;
        ThreeVerticalItem++;
        ThreeHorizontalItem++;
        /*
        if (GameOver() && Combo==0)
        {
            gameOver.gameObject.SetActive(true);
        }
        */
    }

    public void CheckIfAnyLineIsCompleted()//�ϳ� ���������� �ѹ�����
    {
        CheckIfLine();
        /*
        if (GameOver() && Combo == 0)
        {
            gameOver.gameObject.SetActive(true);
        }
        */
    }

    int[] sameColorColumLine = new int[5];
    int[] sameColorRowLine = new int[5];
    int[] sameColorZeroLine = new int[5];
    int[] sameColorOneLine = new int[5];
    [HideInInspector]
    public int[] completeIndexArray = new int[5];
    private int CheckIfSquaresAreCompleted(List<int[]> data)//�����������_data.Count==10;
    {
        KeepItemTurn--;
        EraserItemTurn--;
        ReloadItemTurn--;
        NextExchangeItemTurn--;
        TrashItemTurn--;
        RainbowItemTurn--;
        ChangeShapeItem--;
        ThreeVerticalItem--;
        ThreeHorizontalItem--;
        List<int[]> completedLines = new List<int[]>();
        var linesCompleted = 0;

        if (CheckColumColor())      //�� üũ
        {
            completedLines.Add(sameColorColumLine);
        }
        if (CheckRowColor())         //�� üũ
        {
            completedLines.Add(sameColorRowLine);
        } 
        if (CheckDiaZeroColor())            //�밢 üũ
        {
            completedLines.Add(sameColorZeroLine);           
        }
        if (CheckDiaOneColor())
        {
            completedLines.Add(sameColorOneLine);
        }       
        foreach (var line in completedLines)//����� ��� ���� ����
        {
            var i = 0;

            foreach (var squareIndex in line)
            {
                completeIndexArray[i] = squareIndex;
                i++;
            }          
            if (SameColorLines()) // QuestController.girdCompLine.Add(squareIndex);//����Ʈ�� ���� ��������
            {
                linesCompleted++;
                QuestControll.GetComponent<QuestController>().QuestIndex();//����Ʈ �Լ� ����
            }
        }
        if (trashCanIndex != 30)
        {
            UseTrashCan();
        }
        if (keepSquareIndex != 30)
        {
            UseKeep();
        }
        return linesCompleted;
    }

    public bool SameColorLines()
    {
        var sameColorLine = false;
        
        for (int i = 0; i < completeIndexArray.Length; i++)
        {
            var com = _gridSquares[completeIndexArray[i]].GetComponent<GridSquare>();
            if (com.shinActive)
            {
                completeShin++;
            }
            com.Deactivate();
            com.ClearOccupied();
           
            GameObject effect = Instantiate(effectShape, new Vector3(_gridSquares[completeIndexArray[i]].transform.localPosition.x,
                _gridSquares[completeIndexArray[i]].transform.localPosition.y, 0), Quaternion.identity) as GameObject;
            effect.transform.SetParent(GameObject.FindGameObjectWithTag("Grid").transform, false);
            //square�� ������� �� ��ġ ���� �޾Ƽ� Instance ����
            //setting_panel ����� ��Ʈ�ѷ� 3�� �÷���
        }
        sameColorLine = true;

        return sameColorLine;
    }

    public string[] colors = new string[30];
    public string[] shapes = new string[30];
    public string[,] achieveId = new string[2,30];
    public void GetInformation()
    {
        for (int i = 0; i < 30; i++)
        {
            colors[i] = _gridSquares[i].GetComponent<GridSquare>().currentColor;
            shapes[i] = _gridSquares[i].GetComponent<GridSquare>().currentShape;
            achieveId[0,i] = _gridSquares[i].GetComponent<GridSquare>().currentAchieveId[0];
            achieveId[1,i] = _gridSquares[i].GetComponent<GridSquare>().currentAchieveId[1];
        }
    }
    public bool CheckDiaZeroColor()
    {
        var sameTrueDiaz = false;
        var sameColorTrueDiaz = false;
        var sameShapeTrueDiaz = false;

        if (colors[0] != null && colors[6] != null && colors[12] != null && colors[18] != null && colors[24] != null)
        {
            if (colors[0] == colors[6] && colors[0] == colors[12] && colors[0] == colors[18] && colors[0] == colors[24])
            {
                sameColorTrueDiaz =  true;
                int j = 0;
                for (int i = 0; i < 25; i += 6)
                {
                    sameColorZeroLine[j] = i;
                    j++;
                }
                GameManager.Instance.UpdateMyAchieveInfo(achieveId[0,0], 5);
            }
            if (shapes[0] == shapes[6] && shapes[0] == shapes[12] && shapes[0] == shapes[18] && shapes[0] == shapes[24])
            {
                sameShapeTrueDiaz = true;
                int j = 0;
                for (int i = 0; i < 25; i += 6)
                {
                    sameColorZeroLine[j] = i;
                    j++;
                }
                GameManager.Instance.UpdateMyAchieveInfo(achieveId[1,0], 5);
            }
        }

        if(sameColorTrueDiaz || sameShapeTrueDiaz)
        {
            sameTrueDiaz = true;
        }
        else
        {
            sameTrueDiaz = false;
        }
        return sameTrueDiaz;
    }

    public bool CheckDiaOneColor()
    {
        var sameTrueDia = false;
        var sameColorTrueDia = false;
        var sameShapeTrueDia = false;

        if (colors[4] != null && colors[8] != null && colors[12] != null && colors[16] != null && colors[20] != null)
        {
            if (colors[4] == colors[8] && colors[4] == colors[12] && colors[4] == colors[16] && colors[4] == colors[20])
            {
                sameColorTrueDia = true;
                int j = 0;
                for (int i = 4; i < 21; i += 4)
                {
                    sameColorOneLine[j] = i;
                    j++;
                }
            }

            if (shapes[4] == shapes[8] && shapes[4] == shapes[12] && shapes[4] == shapes[16] && shapes[4] == shapes[20])
            {
                sameShapeTrueDia = true;
                int j = 0;
                for (int i = 4; i < 21; i += 4)
                {
                    sameColorOneLine[j] = i;
                    j++;
                }
            }
        }

        if (sameColorTrueDia || sameShapeTrueDia)
        {
            sameTrueDia = true;
        }
        else
        {
            sameTrueDia = false;
        }
        return sameTrueDia;
    }

    public bool CheckColumColor()
    {
        var sameCompCol = 0;
        var sameTrueCol = false;
        var sameColorTrueCol = false;
        var sameShapeTrueCol = false;

        for (int i = 0; i < 21; i += 5)//0 5 10 15 20
        {
            if (colors[i] != null && colors[i + 1] != null && colors[i + 2] != null && colors[i + 3] != null && colors[i + 4] != null)
            {
                if (colors[i] == colors[i + 1] && colors[i] == colors[i + 2] && colors[i] == colors[i + 3] && colors[i] == colors[i + 4])       //�÷��� ����
                {
                    sameCompCol = i;
                    sameColorTrueCol = true;
                    GameManager.Instance.UpdateMyAchieveInfo(achieveId[0,i],5);

                }
                if (shapes[i] == shapes[i + 1] && shapes[i] == shapes[i + 2] && shapes[i] == shapes[i + 3] && shapes[i] == shapes[i + 4])       //����� ����
                {
                    sameCompCol = i;
                    sameShapeTrueCol = true;
                    GameManager.Instance.UpdateMyAchieveInfo(achieveId[1,i], 5);
                }
            }
        }

        if(sameColorTrueCol|| sameShapeTrueCol)
        {
            sameTrueCol = true;
            for (int i = 0; i < 5; i++)
            {
                sameColorColumLine[i] = sameCompCol + i;
            }
        }
        else
        {
            sameTrueCol = false;
        }
        return sameTrueCol;
    }

    public bool CheckRowColor() 
    {
        var sameCompRow = 0;
        var sameTrueRow = false;
        var sameColorTrueRow = false;
        var sameShapeTrueRow = false;

        for (int i = 0; i < 5; i++)
        {
            if (colors[i] != null && colors[i + 5] != null && colors[i + 10] != null && colors[i + 15] != null && colors[i + 20] != null)
            {
                if (colors[i] == colors[i + 5] && colors[i] == colors[i + 10] && colors[i] == colors[i + 15] && colors[i] == colors[i + 20])
                {
                    sameCompRow = i;
                    sameColorTrueRow = true;
                    GameManager.Instance.UpdateMyAchieveInfo(achieveId[0,i], 5);
                }
                if (shapes[i] == shapes[i + 5] && shapes[i] == shapes[i + 10] && shapes[i] == shapes[i + 15] && shapes[i] == shapes[i + 20])
                {
                    sameCompRow = i;
                    sameShapeTrueRow = true;
                    GameManager.Instance.UpdateMyAchieveInfo(achieveId[1,i], 5);
                }
            }
        }

        if (sameColorTrueRow || sameShapeTrueRow)
        {
            sameTrueRow = true;
            int j = 0;
            for (int i = 0; i < 21; i += 5)
            {
                sameColorRowLine[j] = sameCompRow + i;
                j++;
            }
        }
        else
        {
            sameTrueRow = false;
        }
        return sameTrueRow;
    }

    bool GameOver()
    {
        bool isGameover = false;
        int fullNum = 0;
        int fullSNum = 0;

        for (int i = 0; i < 25; i++)
        {         
            if (colors[i] != null)
            {
                fullNum++;
            }
        }
        for (int i = 0; i < 25; i++)
        {
            if (shapes[i] != null)
            {
                fullSNum++;
            }
        }
        if(fullNum > 24 || fullSNum>24)
        {
            isGameover = true;
        }

        return isGameover;
    }

    void UseKeep()//29�� ������ Ŭ������ shape�� ������ ��������
    {
        if (keepSquareIndex != 30)
        {           
            var comp = _gridSquares[keepSquareIndex].GetComponent<GridSquare>();//index�� ģ��

            if (KeepItemTurn < 1)
            {
                comp.Deactivate();
                comp.ClearOccupied();
                _gridSquares[keepSquareIndex].transform.GetChild(0).gameObject.transform
               .GetChild(0).gameObject.GetComponent<Text>().text = " ";
            }
            else//KeepItemTurn�� 1,2,3...�϶�
            {
                comp.ActivateSquare();
                _gridSquares[keepSquareIndex].SetActive(true);
                _gridSquares[keepSquareIndex].transform.GetChild(0).gameObject.transform
                    .GetChild(0).gameObject.GetComponent<Text>().text = KeepItemTurn.ToString();
            }                     
        }
    }

    void UseTrashCan()
    {
        if (trashCanIndex != 30)
        {
            var comp = _gridSquares[trashCanIndex].GetComponent<GridSquare>();
            if(TrashItemTurn < 1)
            {
                comp.Deactivate();
                comp.ClearOccupied();
                _gridSquares[trashCanIndex].transform.GetChild(0).gameObject.transform
               .GetChild(0).gameObject.GetComponent<Text>().text = " ";
            }
            else
            {
                comp.ActivateSquare();
                _gridSquares[trashCanIndex].transform.GetChild(0).gameObject.transform
                .GetChild(0).gameObject.GetComponent<Text>().text = TrashItemTurn.ToString();
            }
        }
    }

    public void SettingKeep()//LineIndicator�� ���� �ϳ� �� ������µ� �츰 keep�ڸ��� can�ڸ��� �ʿ��ϴ� �װ� �ƴ϶�� ����
    {
        GameObject ItemControllerObj = GameObject.FindGameObjectWithTag("ItemController");
        if (ItemControllerObj != null)
        {
            trashCanIndex = ItemControllerObj.GetComponent<ItemController>().trashCanItemIndex;
            keepSquareIndex = ItemControllerObj.GetComponent<ItemController>().keepItemIndex;
        }

        for (int i = 25; i < 30; i++)//ItemController���� ���ù������� �ֵ��� ����
        {
            if (trashCanIndex != i && keepSquareIndex != i)
            {
                var comp = _gridSquares[i].GetComponent<GridSquare>();
                comp.NonKeep();//GridSquare�� �ڱ��ڽ��� ���� �Լ� ȣ��
            }
            else
            {
                var comp = _gridSquares[i].GetComponent<GridSquare>();
                if(trashCanIndex == i)
                {
                    comp.IMtrash = true;
                    _gridSquares[i].SetActive(true);
                    UseTrashCan();
                }
                else if(keepSquareIndex == i)
                {
                    comp.IMkeep = true;
                    _gridSquares[i].SetActive(true);
                    UseKeep();
                }
            }
        }
    }

    public void GameRestart()
    {
        
        if (ItemController.reStart)
        {
            for (int i = 0; i < _gridSquares.Count; i++)
            {
                var comp = _gridSquares[i].GetComponent<GridSquare>();
                comp.Deactivate();
                comp.ClearOccupied();
            }
           
            shapestorageObj = GameObject.FindGameObjectWithTag("ShapeStorage");
            shapestorageObj.GetComponent<ShapeStorage>().ReloadItem();
            shapestorageObj.GetComponent<ShapeStorage>().ReloadItem();
        }
        SettingKeep();
    }
}
