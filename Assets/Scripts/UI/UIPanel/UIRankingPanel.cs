using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StackExchange.Redis;

public class UIRankingPanel : UIBase
{

    IDatabase db;
    ConnectionMultiplexer redisConnection;
    ISubscriber sub;

    public UIRankingPanel(GameObject UIPrefab)
    {
        UIRankingPanel r = UIPrefab.GetComponent<UIRankingPanel>();
        r.Awake();
        r.UIPrefab = UIPrefab;

        r.InstantiatePrefab();
    }

    public override void Start()
    {
        base.Start();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
