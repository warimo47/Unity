using System;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

#region [Packet define]
[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct IF_SC100
{
    public byte header;
    public fixed char IP[30];
    public double latitude;
    public double longitude;
    public byte objType;
    public fixed char time[20];
    public float objX1;
    public float objY1;
    public float objX2;
    public float objY2;
};

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct IF_SC200
{
    public byte header;
    public fixed char deviceID[8];
    public fixed char time[10];
    public fixed char fireStatusInformation[10];
    public fixed char deviceStatusInformation[10];
};

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct IF_SC300
{
    public byte header;
    public fixed char deviceID[8];
    public fixed char time[10];
    public fixed char eventStatusInformation[10];
    public fixed char deviceStatusInformation[10];
    public fixed char emergencySituationInformation[10];
};
#endregion

public class Client : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream ns;
    private StreamWriter writer;

    private Button btn_sendIFSC100;
    private Text txt_sendIFSC100;

    private bool isConnect = false;

    private InputField serverIPText;
    private InputField portText;

    private IF_SC100 if_sc100;
    private InputField ipf_IF_SC100_IP;
    private InputField ipf_IF_SC100_latitude;
    private InputField ipf_IF_SC100_longitude;
    private InputField ipf_IF_SC100_objType;
    private InputField ipf_IF_SC100_time;
    private InputField ipf_IF_SC100_objX1;
    private InputField ipf_IF_SC100_objY1;
    private InputField ipf_IF_SC100_objX2;
    private InputField ipf_IF_SC100_objY2;

    // Start is called before the first frame update
    private void Start()
    {
        Screen.SetResolution(640, 480, true);
        
        Initialize();
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private unsafe void Initialize()
    {
        // ===== UI Binding =====
        serverIPText = GameObject.Find("IPF_ServerIP").GetComponent<InputField>();
        portText = GameObject.Find("IPF_Port").GetComponent<InputField>();
        btn_sendIFSC100 = GameObject.Find("BTN_SendIF-SC100").GetComponent<Button>();
        txt_sendIFSC100 = GameObject.Find("TXT_Connect").GetComponent<Text>();

        ipf_IF_SC100_IP = GameObject.Find("IPF_IF-SC100_IP").GetComponent<InputField>();
        ipf_IF_SC100_latitude = GameObject.Find("IPF_IF-SC100_latitude").GetComponent<InputField>();
        ipf_IF_SC100_longitude = GameObject.Find("IPF_IF-SC100_longitude").GetComponent<InputField>();
        ipf_IF_SC100_objType = GameObject.Find("IPF_IF-SC100_objType").GetComponent<InputField>();
        ipf_IF_SC100_time = GameObject.Find("IPF_IF-SC100_time").GetComponent<InputField>();
        ipf_IF_SC100_objX1 = GameObject.Find("IPF_IF-SC100_objX1").GetComponent<InputField>();
        ipf_IF_SC100_objY1 = GameObject.Find("IPF_IF-SC100_objY1").GetComponent<InputField>();
        ipf_IF_SC100_objX2 = GameObject.Find("IPF_IF-SC100_objX2").GetComponent<InputField>();
        ipf_IF_SC100_objY2 = GameObject.Find("IPF_IF-SC100_objY2").GetComponent<InputField>();
    }

    public void Connect()
    {
        try
        {
            client = new TcpClient(serverIPText.text, Int32.Parse(portText.text)); // (ip주소 , 포트 번호)
            ns = client.GetStream();
            writer = new StreamWriter(ns);

            if (isConnect == false)
            {
                btn_sendIFSC100.enabled = true;
                txt_sendIFSC100.text = "Disconnect";
                Debug.Log("Server connet");
                isConnect = true;
            }
            else
            {
                btn_sendIFSC100.enabled = false;
                txt_sendIFSC100.text = "Connect";
                Debug.Log("Server disconnet");
                isConnect = false;
            }
        }
        catch
        {
            Debug.Log("Connect fail");
        }
    }

    public unsafe void SendIF_SC100()
    {
        // ===== IF-SC100 value copy =====
        if_sc100.header = 1;
        for (int i = 0; i < ipf_IF_SC100_IP.text.Length; ++i)
        {
            if_sc100.IP[i] = ipf_IF_SC100_IP.text[i];
        }
        if_sc100.latitude = double.Parse(ipf_IF_SC100_latitude.text);
        if_sc100.longitude = double.Parse(ipf_IF_SC100_longitude.text);
        if_sc100.objType = byte.Parse(ipf_IF_SC100_objType.text);
        for (int i = 0; i < ipf_IF_SC100_time.text.Length; ++i)
        {
            if_sc100.time[i] = ipf_IF_SC100_time.text[i];
        }
        if_sc100.objX1 = float.Parse(ipf_IF_SC100_objX1.text);
        if_sc100.objY1 = float.Parse(ipf_IF_SC100_objY1.text);
        if_sc100.objX2 = float.Parse(ipf_IF_SC100_objX2.text);
        if_sc100.objY2 = float.Parse(ipf_IF_SC100_objY2.text);

        if (ns.CanWrite == true)
        {
            ns.Write(StructureToByteArray(if_sc100), 0, sizeof(IF_SC100));
            Debug.Log("Send IF_SC100");
        }
        else
        {
            Debug.Log("Can not write");
        }
    }

    private void Close()
    {
        writer.Close();
        client.Close();
    }

    private static byte[] StructureToByteArray(IF_SC100 if_sc100)
    {
        byte[] bb = new byte[Marshal.SizeOf(if_sc100)];
        GCHandle gch = GCHandle.Alloc(bb, GCHandleType.Pinned);
        Marshal.StructureToPtr(if_sc100, gch.AddrOfPinnedObject(), false);
        gch.Free();
        return bb;
    }

    private static byte[] StructureToByteArray(IF_SC200 if_sc200)
    {
        byte[] bb = new byte[Marshal.SizeOf(if_sc200)];
        GCHandle gch = GCHandle.Alloc(bb, GCHandleType.Pinned);
        Marshal.StructureToPtr(if_sc200, gch.AddrOfPinnedObject(), false);
        gch.Free();
        return bb;
    }
}
