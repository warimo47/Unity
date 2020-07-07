using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private float speed = 0.0f;
    private Vector2 startPos;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            this.startPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector2 endPos = Input.mousePosition;
            float swipeLength = endPos.x - this.startPos.x;

            this.speed = swipeLength / 1000.0f;

            GetComponent<AudioSource>().Play();
        }

        transform.Translate(this.speed, 0.0f, 0.0f);
        this.speed *= 0.98f;
    }
}
