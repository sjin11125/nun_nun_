using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class BuildingSave : MonoBehaviour
{               //건물들 저장하는 스크립트
                //저장하면 구글 스프레드 시트로 전송

    // string URL = GameManager.URL;
    public static BuildingSave _Instance;
    public static BuildingSave Instance
    {
        get {
            if (_Instance == null) 
            return null;
            return _Instance;
        }
    }
  

    public bool isMe;       //내 자신의 건물을 불러오는가?
    // Start is called before the first frame update


    
  
   
}

