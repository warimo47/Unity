using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DllTest : MonoBehaviour
{
    [DllImport("MySum.dll")]
    extern public static int mySum(int a, int b);

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("시작");
        Debug.Log(mySum(2, 5));
        Debug.Log("종료");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
