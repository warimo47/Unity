using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouletteController : MonoBehaviour
{
    public float inputRotSpeed = 10.0f;

    private float rotSpeed = 0.0f;

    

    // Start is called before the first frame update
    void Start()
    {
        inputRotSpeed = Random.Range(100.0f, 200.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            inputRotSpeed = Random.Range(100.0f, 200.0f);
            this.rotSpeed = inputRotSpeed;
        }

        transform.Rotate(0.0f, 0.0f, this.rotSpeed);

        this.rotSpeed *= 0.96f;
    }
}
