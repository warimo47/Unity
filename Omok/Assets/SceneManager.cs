using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public GameObject[] buttonArray;
    public Sprite[] blackWhiteClearRock;
    public Judgment judgment;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < buttonArray.Length; ++i)
        {
            buttonArray[i].GetComponent<Image>().sprite = blackWhiteClearRock[2];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PutRock(int _y, int _x, int _isBlackTurn)
    {
        buttonArray[_y * 19 + _x].GetComponent<Image>().sprite = blackWhiteClearRock[_isBlackTurn];
    }

    public void MousePointerEnter(GameObject _targetButton)
    {
        Color alpha = Color.white;
        alpha.a = 0.7f;

        _targetButton.GetComponent<Image>().sprite = blackWhiteClearRock[judgment.turnCount % 2];
        _targetButton.GetComponent<Image>().color = alpha;
    }

    public void MousePointerExit(GameObject _targetButton)
    {
        _targetButton.GetComponent<Image>().sprite = blackWhiteClearRock[2];
    }
}
