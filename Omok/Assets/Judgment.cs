using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judgment : MonoBehaviour
{
    private int[] ground;
    public int turnCount = 1;

    public SceneManager sceneManager;

    // Start is called before the first frame update
    void Start()
    {
        // Initalize ground
        ground = new int[19 * 19];

        for (int y = 0; y < 19; ++y)
        {
            for (int x = 0; x < 19; ++x)
            {
                ground[y * 19 + x] = 2;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TryPutRock(int _y, int _x)
    {
        if (ground[_y * 19 + _x] != 2) return;
        
        sceneManager.PutRock(_y, _x, turnCount % 2);
        ground[_y * 19 + _x] = turnCount % 2;
        turnCount++;

        checkVictory(turnCount % 2);
    }

    void checkVictory(int _isBlack)
    {

    }
}
