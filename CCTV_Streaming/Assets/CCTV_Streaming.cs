using System.Collections;
using System.Collections.Generic;
using UMP;
using UnityEngine;
using UnityEngine.UI;

public class CCTV_Streaming : MonoBehaviour
{
    // ========== CCTV Streaming ==========
    private bool isStreaming = false;
    public UniversalMediaPlayer ump;
    public InputField rtspURL; // ex) rtsp://192.168.0.80:8554
    public RawImage cctvView;
    public Text txtStreaming;

    // Start is called before the first frame update
    void Start()
    {
        rtspURL = GameObject.Find("IFD_RTSP").GetComponent<InputField>();
        cctvView = GameObject.Find("CCTV_View").GetComponent<RawImage>();
        txtStreaming = GameObject.Find("TXT_Streaming").GetComponent<Text>();
    }

    public void StreamingToggle()
    {
        if (isStreaming == false)
        {
            ump.Path = rtspURL.text;
            ump.Play();
            isStreaming = true;
            txtStreaming.text = "스트리밍 멈춤";
        }
        else
        {
            ump.Stop();
            cctvView.texture = null;
            isStreaming = false;
            txtStreaming.text = "스트리밍 시작";
        }
    }
}
