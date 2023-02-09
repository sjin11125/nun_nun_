using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Priority_Queue;
using System.Linq;

public class Node 
{
    public int F;               //최종 점수 (G-H)
    public int G;               //시작점에서 목적지까지 이동하는데 드는 비용(거리)
    public int H;               //목적지로부터 얼마나 가까운 곳인지에 대한 점수

    public int Y;               //
    public int X;
    public Node Parent;
  
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


    public void SetPos(BoundsInt Start, BoundsInt Dest)
    {
        StartPos = Start;
        DestPos = Dest;
    }

   
    public List<Node> Astar()
    {
        int[] MoveY = new int[4] { 1, -1, 0, 0 };//상,하,좌,우 만큼 이동하려면 더해주기 ex) 상으로 가려면 y를 1해줘야함
        int[] MoveX = new int[4] { 0, 0, -1, 1};

        int[] Cost = new int[4] { 1,1,1,1 };            //비용(거리)

       List<Node> Closed = new List<Node>();           //이미 방문한 타일 모음
       List<Node> Parents = new List<Node>();           //방문할 타일들 모음
       Node EndParent = new Node();           //마지막 타일

        SimplePriorityQueue<Node> priorityQueue = new SimplePriorityQueue<Node>();      //우선순위큐

        Node StartNode = new Node();
        StartNode.G = 0;
        StartNode.H= StartNode.F = Math.Abs( DestPos.position.x - StartPos.x) + Math.Abs(DestPos.position.y - StartPos.y);
        StartNode.X = StartPos.x;
        StartNode.Y = StartPos.y;
        StartNode.Parent = StartNode;

        priorityQueue.Enqueue(StartNode, StartNode.F);

        while (priorityQueue.Count>0)       //큐가 안남을때까지 루프
        {
            Node NewNode= priorityQueue.Dequeue();//F값이 제일 적은 노드 빼오기

            if (Closed.Any(node => node.X == NewNode.X && node.Y == NewNode.Y))     //이미 방문한 타일이라면 패스
                continue;

            Closed.Add(NewNode);            //방문 안했다면 방문한 타일 리스트에 추가

            if (NewNode.X == DestPos.x && NewNode.Y == DestPos.y)       //목적지라면 탈출
            {
                EndParent = NewNode;
                break;
            }

            for (int i = 0; i < MoveY.Length; i++)      //상하좌우
            {
                
                int nextY = NewNode.Y + MoveY[i];
                int nextX = NewNode.X + MoveX[i];

                Node BNode = new Node() { Y = nextY, X = nextX };

                BoundsInt position = new BoundsInt();
                position.x = nextX;
                position.y = nextY;
                position.size = new Vector3Int(1,1,1);

                if (!GridBuildingSystem.current.CanTakeArea(position))         //타일맵 밖이나 건물이 있다면 패스
                    continue;

                int G = NewNode.G + Cost[i];            //G계산(처음 시작지점에서 현재 위치까지 거리)
                int H= Math.Abs(DestPos.position.x - nextX) + Math.Abs(DestPos.position.y - nextY);//현재 위치에서 목적지까지 최단거리

                if (NewNode.G + NewNode.H < G + H)               //현재위치에서 상하좌우 중 제일 거리(F)가 짧은 타일로 감
                   continue;

                if (priorityQueue.Any(node => node.X == BNode.X && node.Y == BNode.Y))          //오픈리스트에 있을 때
                {
                    if (NewNode.G<G)
                    {
                        //오픈리스트에 넣기
                        priorityQueue.Enqueue(new Node() { F = G + H, G = G, H = H, Y = nextY, X = nextX, Parent = NewNode }, G + H);

                    }
                }
                else                    //오픈리스트에 없을때
                {
                                    //오픈리스트에 넣기
                    priorityQueue.Enqueue(new Node() { F = G + H, G = G, H = H, Y = nextY, X = nextX, Parent = NewNode }, G + H);
                }
            }
        }
        Debug.Log("EndParent: "+EndParent.X+", "+ EndParent.Y);
        Debug.Log("StartPos: " + StartPos.x+", "+ StartPos.y);
        while (EndParent.Parent.X!= StartPos.x||
            EndParent.Parent.Y != StartPos.y)
        {
            Parents.Add(EndParent);
                  EndParent = EndParent.Parent;
            if (EndParent.Parent != null)
                continue;
            else
                break;
        }
        Parents.Add(new Node() {  Y = StartPos.y, X = StartPos.x });
        Parents.Reverse();

        return Parents;
    }
}

