using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DBConnector : MonoBehaviour
{
    private string dbUrl = "http://localhost/eduar/request.php?query=select * from teacher;";
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetInfo());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GetInfo()
    {
        UnityWebRequest info_get = UnityWebRequest.Get(dbUrl);
        yield return info_get.SendWebRequest();

        if(info_get.isNetworkError || info_get.isHttpError)
        {
            Debug.Log(info_get);
        } else
        {
            Debug.Log(info_get.downloadHandler.text);
        }
    }
}
