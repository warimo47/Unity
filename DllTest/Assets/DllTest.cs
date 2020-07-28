using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DllTest : MonoBehaviour
{
    [DllImport("MySum.dll")]
    extern public static int mySum(int a, int b);

    [DllImport("TCPDll.dll")]
    extern public static int getMyID();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("시작");
        Debug.Log("mySum() " + mySum(-1, 3));
        Debug.Log("getMyID() " + getMyID());
        Debug.Log("종료");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
