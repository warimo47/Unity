using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Judgment judgment;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LeftMouseClick(int _coordinate)
    {
        judgment.TryPutRock(_coordinate / 100, _coordinate % 100);
    }
}
