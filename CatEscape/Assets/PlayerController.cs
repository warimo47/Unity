using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.Translate(-3.0f, 0.0f, 0.0f);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.Translate(3.0f, 0.0f, 0.0f);
        }
    }

    public void LButtonDown()
    {
        transform.Translate(-3.0f, 0.0f, 0.0f);
    }

    public void RButtonDown()
    {
        transform.Translate(3.0f, 0.0f, 0.0f);
    }
}
