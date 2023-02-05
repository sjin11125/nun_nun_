using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

struct Node : IComparable<Node>
{
    public int F;               //최종 점수 (G-H)
    public int G;               //시작점에서 목적지까지 이동하는데 드는 비용(거리)
    public int H;               //목적지로부터 얼마나 가까운 곳인지에 대한 점수

    public int Y;               //
    public int X;
    public int CompareTo(Node other)                //IComparable 인터페이스를 상속받았으니까 무조건 구현
    {
        if (F == other.F)
            return 0;

        return F < other.F ? 1 : -1;
           
    }
}
public class WonderAI : MonoBehaviour

{

    //콜라이더 충돌하면 방향바꾸기

    SpriteRenderer[] rend;
    SpriteRenderer[] rend2;

    //public Animator animator;
    //추가
    public GameObject ai;
    //public Animator ani;

    public float speed;
    private float waitTime;

    public float startWaitTime;

    public Transform moveSpot;


    float minX;
    float maxX;
    float minY;
    float maxY;

    public BoundsInt DestPos;
    public BoundsInt StartPos;

    // Start is called before the first frame update
    void Start()
    {
        //ani = gameObject.GetComponent<Animator>();

        //추가
        /*rend = GetComponentsInChildren<SpriteRenderer>();
        waitTime = Random.Range(3, 7); // 3~7초동안 기다림 

        //추가 이거를 바꿔서 AI들이 움직이는 범위 지정하면 될거 같음 Good 
        // 이거의 좌표를 지금 맵 전체로 잡아야 할 것 같음. 그리고 벽을 칠해서 거기 닿이면 방향반대로 하면 될 것 같은데 
        //벽만들고 벽/건물충돌시 방향반대, 누니는 무시하기
        minX = Random.Range(-10, 0);
        maxX = Random.Range(0, 10);
        minY = Random.Range(-11, 0);
        maxY = Random.Range(0, 7);*/
        //Vector3 moveSpot = ai.GameObject.transform.position;


        waitTime = startWaitTime;
        //moveSpot.position = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        moveSpot.position = new Vector2(ai.transform.position.x, ai.transform.position.y); //ai의 position에 spot위치 // move 위치찍기

    }



    // Update is called once per frame
    void Update()
    {
        //animator.SetBool("Walk", true);
        transform.position = Vector2.MoveTowards(transform.position, moveSpot.position, speed * Time.deltaTime);
        //충돌하면 X방향 바꾸기

       /* if (Vector2.Distance(transform.position, moveSpot.position) < 0.2f) // 거리가 0.2f가안되면 새로운 스팟 찾기
        {
            if (waitTime <= 0)
            {
                moveSpot.position = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));

                if (moveSpot.position.x > ai.transform.position.x)
                {

                    for (int i = 0; i < rend.Length; i++) // 뒤집히기
                    {
                        rend[i].flipX = true;
                    }
                }
                else
                {
                    for (int i = 0; i < rend.Length; i++)
                    {
                        rend[i].flipX = false;
                    }
                }

                waitTime = startWaitTime;




                // wait-deltaTime=0이 되면 다음포지션을 정한다. 
            }
            else
            {
                waitTime -= Time.deltaTime; //wait - deltaTIme=0이 될 때 까지 움직여라


                //얘가 움직이는 함수

            }



        }*/

    }
    public void Astar()
    {
        int[] MoveY = new int[8] { 1, -1, 0, 0, 1, 1, -1, -1 };//상,하,좌,우,(좌상,우상,좌하,우하)만큼 이동하려면 더해주기 ex) 상으로 가려면 y를 1해줘야함
        int[] MoveX = new int[8] { 1, -1, 0, 0, 1, 1, -1, -1 };

        int[] Cost = new int[8] { 10, 10, 10, 10, 14, 14, 14, 14 };

       List<Node> Closed = new List<Node>();           //이미 방문한 타일 모음
       List<Node> Opened = new List<Node>();           //방문할 타일들 모음
       List<Node> Parent = new List<Node>();           //부모 타일(이전 타일)


        Node StartNode = new Node();
        StartNode.G = 0;
        StartNode.H= StartNode.F = Math.Abs( DestPos.position.x - StartPos.x) + Math.Abs(DestPos.position.y - StartPos.y);

        Parent.Add(StartNode);                  //제일 처음 타일 부모 타일 리스트에 넣기




    }
    public void OnCollisionStay2D(Collision2D other)
    {
        /*if (other.gameObject.tag == "Building")
        {
            //반대쪽 X좌표를 찍게 해야 할 것 같음 
            //if ( this.transform.position.x < moveSpot.position.x)
            moveSpot.position = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));

            if (moveSpot.position.x > ai.transform.position.x)
            {

                for (int i = 0; i < rend.Length; i++) // 뒤집히기
                {
                    rend[i].flipX = true;
                }
            }
            else
            {
                for (int i = 0; i < rend.Length; i++)
                {
                    rend[i].flipX = false;
                }
            }

            waitTime = startWaitTime;




            // wait-deltaTime=0이 되면 다음포지션을 정한다. 
        }*/

    }
}

