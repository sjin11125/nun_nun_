using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Puzzle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int X;
    public int Y;
    public Image Image;
    public int ShapeNum;            //0~3
    public int Index = 0;
    public Button Btn;
    public PuzzleState State;            //�Ϲݺ�: NB ,��ġŲ ��: MB, ���� ����:LB, ĵ��: CB
    public bool isSelected = false;
    Vector3 FirstPosition;



    int[] moveDir = new int[2] { 1, -1 };
    public Puzzle(int x, int y, int shapeNum, int index, PuzzleState state)
    {
        X = x;
        Y = y;
        ShapeNum = shapeNum;
        State = state;
        Index = index;
    }

    public Puzzle()
    {
    }

    public Puzzle DeepCopy()
    {
        Puzzle tempShape = new Puzzle();

        tempShape.X = this.X;
        tempShape.Y = this.Y;
        tempShape.Index = this.Index;
        tempShape.ShapeNum = this.ShapeNum;
        tempShape.State = this.State;

        return tempShape;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("�ڴ���");

        FirstPosition = Input.mousePosition;
        isSelected = true;
        Board.Instance.SelectedShape = gameObject.GetComponent<Puzzle>();
        // Debug.Log("FirstPosition.x: " + FirstPosition.x);


    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isSelected = false;
        // Board.Instance.SelectedShape = null;
    }
    public void SetInfo(Puzzle shapeInfo)
    {
        X = shapeInfo.X;
        Y = shapeInfo.Y;
        ShapeNum = shapeInfo.ShapeNum;
        State = shapeInfo.State;
        Index = shapeInfo.Index;
        Image.sprite = Board.Instance.ShapeImages[ShapeNum];
    }
    private void Update()
    {
        if (isSelected)
        {
            if ((FirstPosition.x < Input.mousePosition.x) &&
          Mathf.Abs(FirstPosition.x) + 100 < Mathf.Abs(Input.mousePosition.x))//���������� ������
            {
                isSelected = false;
                Debug.Log("���������� ������");

                if (State != PuzzleState.MB)
                    StartCoroutine(Board.Instance.MatchCheck(Dir.Right));
                else
                    StartCoroutine(Board.Instance.MunchkinBlock(Dir.Right, X, Y, this.gameObject));


            }
            else if ((FirstPosition.x > Input.mousePosition.x) &&
               Mathf.Abs(FirstPosition.x) - 100 > Mathf.Abs(Input.mousePosition.x))                                  //�������� ������
            {
                isSelected = false;
                Debug.Log("�������� ������");

                if (State != PuzzleState.MB)
                    StartCoroutine(Board.Instance.MatchCheck(Dir.Left));
                else
                    StartCoroutine(Board.Instance.MunchkinBlock(Dir.Left, X, Y, this.gameObject));
            }
            else if ((FirstPosition.y < Input.mousePosition.y) &&
               Mathf.Abs(FirstPosition.y) + 100 < Mathf.Abs(Input.mousePosition.y))                                  //�������� ������
            {
                isSelected = false;
                Debug.Log("�������� ������");

                if (State != PuzzleState.MB)
                    StartCoroutine(Board.Instance.MatchCheck(Dir.Up));
                else
                    StartCoroutine(Board.Instance.MunchkinBlock(Dir.Up, X, Y, this.gameObject));
            }
            else if ((FirstPosition.y > Input.mousePosition.y) &&
               Mathf.Abs(FirstPosition.y) - 100 > Mathf.Abs(Input.mousePosition.y))                                  //�Ʒ������� ������
            {
                isSelected = false;
                Debug.Log("�Ʒ������� ������");

                if (State != PuzzleState.MB)
                    StartCoroutine(Board.Instance.MatchCheck(Dir.Down));
                else
                    StartCoroutine(Board.Instance.MunchkinBlock(Dir.Down, X, Y, this.gameObject));
            }

            //  StartCoroutine(Board.Instance.DownCreateMatchCheck());
        }
    }


    public IEnumerator MoveCoroutine(GameObject DesObj)
    {

        while (transform.position != DesObj.transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, DesObj.transform.position,  500*Time.deltaTime);
            
            yield return null;
        }
        yield return null;
    }
    public IEnumerator MBBlockMoveCoroutine(GameObject DesObj, Dir dir)
    {
        while (transform.position != DesObj.transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, DesObj.transform.position, 500 * Time.deltaTime);


            yield return null;
        }
        yield return null;
    }
    public IEnumerator DownPuzzle(int x, int y, int index)
    {
        //int index = DestroyShape.Index;
        //int y = DestroyShape.Y;
        int blank = 0;
        while (y >= 1)
        {
            if (Board.Instance.Boards[y - 1, x] == null)         //�������� ��ĭ�� ������� 
            {
                index -= 7;
                blank += 1;
                y -= 1;
                continue;
            }

            Board.Instance.Boards[y + blank, x] = Board.Instance.Boards[y - 1, x];            //������ �ٲٱ�
            Board.Instance.Boards[y - 1, x] = null;
            Board.Instance.Boards[y + blank, x].Index = index;
            Board.Instance.Boards[y + blank, x].Y = y;

            yield return StartCoroutine(Board.Instance.Boards[y + blank, x].MoveCoroutine(Board.Instance.BoardObj[index]));               //������Ʈ �̵�
            //null;
            index -= 7;
            y -= 1;

        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (State == PuzzleState.NB && Board.Instance.SelectedShape != this)
        {
            if (collision.CompareTag("MB"))         //��ġŲ ���̶� ������
            {
                Destroy(gameObject);
            }
        }
    }
    private void OnDisable()
    {
        if (State != PuzzleState.MB)
        {
            Board.Instance.Boards[Y, X] = null;
            
        }
        Board.Instance.Score.Value += 10;
    }
}

