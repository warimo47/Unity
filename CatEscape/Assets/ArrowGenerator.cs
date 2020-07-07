using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowGenerator : MonoBehaviour
{
    public GameObject arrowPrefab;
    float span = 1.0f;
    float delta = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.delta += Time.deltaTime;
        if (this.delta > this.span)
        {
            this.delta = 0.0f;
            GameObject go = Instantiate(arrowPrefab) as GameObject;
            int px = Random.Range(-6, 7);
            go.transform.position = new Vector3(px, 7.0f, 0.0f);
        }
    }
}
