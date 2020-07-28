using System.Collections;
using System.Collections.Generic;
using UMP;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CCTV_PTZ : MonoBehaviour
{
    // ========== Common value ==========
    private InputField ipAddress; // ex) 192.168.0.80

    // ========== CCTV Streaming ==========
    private bool isStreaming = false;
    private UniversalMediaPlayer ump;
    private RawImage cctvView;
    private Text txtStreaming;

    // ========== Control PTZ ==========
    private UnityWebRequest www;
    private Button btnLeft;
    private Button btnRight;
    private Button btnUp;
    private Button btnDown;
    private Button btnZoomIn;
    private Button btnZoomOut;

    // Start is called before the first frame update
    void Start()
    {
        // ========== CCTV Streaming ==========
        ump = GameObject.Find("UMP").GetComponent<UniversalMediaPlayer>();
        cctvView = GameObject.Find("CCTV_View").GetComponent<RawImage>();
        txtStreaming = GameObject.Find("TXT_Streaming").GetComponent<Text>();

        // ========== Control PTZ ==========
        ipAddress = GameObject.Find("IFD_IP").GetComponent<InputField>();
        btnLeft = GameObject.Find("BTN_Left").GetComponent<Button>();
        btnRight = GameObject.Find("BTN_Right").GetComponent<Button>();
        btnUp = GameObject.Find("BTN_Up").GetComponent<Button>();
        btnDown = GameObject.Find("BTN_Down").GetComponent<Button>();
        btnZoomIn = GameObject.Find("BTN_ZoomIn").GetComponent<Button>();
        btnZoomOut = GameObject.Find("BTN_ZoomOut").GetComponent<Button>();
    }

    public void StreamingToggle()
    {
        if (isStreaming == false)
        {
            ump.Path = "rtsp://" + ipAddress.text + ":8554";
            ump.Play();
            isStreaming = true;
            txtStreaming.text = "스트리밍 멈춤";

            btnLeft.interactable = true;
            btnRight.interactable = true;
            btnUp.interactable = true;
            btnDown.interactable = true;
            btnZoomIn.interactable = true;
            btnZoomOut.interactable = true;
        }
        else
        {
            ump.Stop();
            cctvView.texture = null;
            isStreaming = false;
            txtStreaming.text = "스트리밍 시작";

            btnLeft.interactable = false;
            btnRight.interactable = false;
            btnUp.interactable = false;
            btnDown.interactable = false;
            btnZoomIn.interactable = false;
            btnZoomOut.interactable = false;
        }
    }

    //ㅡㅡㅡㅡㅡ 카메라 움직이는 부분
    public void Move_End()
    {
        StartCoroutine(Move_CCTV(0));
    }

    public void Move_Left()
    {
        StartCoroutine(Move_CCTV(2));
    }

    public void Move_Right()
    {
        StartCoroutine(Move_CCTV(3));
    }

    public void Move_Up()
    {
        StartCoroutine(Move_CCTV(4));
    }

    public void Move_Down()
    {
        StartCoroutine(Move_CCTV(5));
    }

    public void Move_LeftUp()
    {
        StartCoroutine(Move_CCTV(6));
    }

    public void Move_LeftDown()
    {
        StartCoroutine(Move_CCTV(7));
    }

    public void Move_RightUp()
    {
        StartCoroutine(Move_CCTV(8));
    }

    public void Move_RightDown()
    {
        StartCoroutine(Move_CCTV(9));
    }
    
    public void Move_ZoomIn()
    {
        StartCoroutine(Move_CCTV(13));
    }

    public void Move_ZoomOut()
    {
        StartCoroutine(Move_CCTV(14));
    }

    IEnumerator Move_CCTV(int action)
    {
        string url = "";

        if (action == 0)
        {
            url = ipAddress.text + "/cgi-bin/control.cgi?action=update&group=PTZCTRL&channel=0&PTZCTRL.action=" + action.ToString();
        }
        else if (action == 13 || action == 14 || action == 15 || action == 16) // Zoom in, Zoom out, Focus in, Focus out
        {
            url = ipAddress.text + "/cgi-bin/control.cgi?action=update&group=PTZCTRL&channel=0&PTZCTRL.action=" + action.ToString() + "&PTZCTRL.speed=5";
        }
        else
        {
            url = ipAddress.text + "/cgi-bin/control.cgi?action=update&group=PTZCTRL&channel=0&PTZCTRL.action=" + action.ToString() + "&PTZCTRL.speed=30";
        }

        using (www = UnityWebRequest.Get(url))
        {
            www.SetRequestHeader("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("admin:admin")));
            yield return www.SendWebRequest();
        }
    }
}
