using System;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

#region [Packet define]
[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct TestPacket
{
    public byte header;
    public fixed char stringParam[30];
    public double doubleParam;
    public int intParam;
};
#endregion

public class TCPClient : MonoBehaviour
{
    // ========== Network ==========
    private TcpClient client;
    private NetworkStream ns;
    private StreamWriter writer;
    private TestPacket testPacket;

    // ========== UI ==========
    private InputField serverIPText;
    private InputField portText;
    private Text txt_Connect;

    private InputField ipf_string;
    private InputField ipf_double;
    private InputField ipf_int;

    private Button btn_SendPacket;
    
    // ========== State ==========
    private bool isConnect = false;

    // Start is called before the first frame update
    private void Start()
    {
        Initialize();
    }

    private unsafe void Initialize()
    {
        // ===== UI Binding =====
        serverIPText = GameObject.Find("IPF_ServerIP").GetComponent<InputField>();
        portText = GameObject.Find("IPF_Port").GetComponent<InputField>();
        txt_Connect = GameObject.Find("TXT_Connect").GetComponent<Text>();

        ipf_string = GameObject.Find("IPF_string").GetComponent<InputField>();
        ipf_double = GameObject.Find("IPF_double").GetComponent<InputField>();
        ipf_int = GameObject.Find("IPF_int").GetComponent<InputField>();

        btn_SendPacket = GameObject.Find("BTN_SendPacket").GetComponent<Button>();
    }

    public void Connect()
    {
        if (isConnect == false)
        {
            try
            {
                client = new TcpClient(serverIPText.text, Int32.Parse(portText.text)); // (ip주소 , 포트 번호)
            }
            catch (Exception ex)
            {
                Debug.Log("new TcpClient() " + ex.ToString());
            }

            ns = client.GetStream();

            try
            {
                writer = new StreamWriter(ns);
            }
            catch (Exception ex)
            {
                Debug.Log("new StreamWriter() " + ex.ToString());
            }

            // 패킷 보내는 버튼 활성화
            btn_SendPacket.enabled = true;

            // 서버 연결 버튼 글자 변경
            txt_Connect.text = "Disconnect";

            Debug.Log("Server connet");

            // 연결된 상태
            isConnect = true;
        }
        else
        {
            // Close StreamWriter
            writer.Close();

            // Close TcpClient
            client.Close();

            // 패킷 보내는 버튼 비활성화
            btn_SendPacket.enabled = false;

            // 서버 연결 버튼 글자 변경
            txt_Connect.text = "Connect";

            Debug.Log("Server disconnet");

            // 연결 안된 상태
            isConnect = false;
        }
    }

    public unsafe void SendTestPacket()
    {
        // ===== IF-SC100 value copy =====
        testPacket.header = 1;
        for (int i = 0; i < ipf_string.text.Length; ++i)
        {
            testPacket.stringParam[i] = ipf_string.text[i];
        }
        testPacket.doubleParam = double.Parse(ipf_double.text);
        testPacket.intParam = int.Parse(ipf_int.text);

        if (ns.CanWrite == true)
        {
            ns.Write(StructureToByteArray(testPacket), 0, sizeof(TestPacket));
            Debug.Log("Send IF_SC100");
        }
        else
        {
            Debug.Log("Can not write");
        }
    }

    public unsafe void SendTestPacket(string localString, double localDouble, int localInt)
    {
        // ===== IF-SC100 value copy =====
        testPacket.header = 1;
        for (int i = 0; i < localString.Length; ++i)
        {
            testPacket.stringParam[i] = localString[i];
        }
        testPacket.doubleParam = localDouble;
        testPacket.intParam = localInt;

        if (ns.CanWrite == true)
        {
            ns.Write(StructureToByteArray(testPacket), 0, sizeof(TestPacket));
            Debug.Log("Send IF_SC100");
        }
        else
        {
            Debug.Log("Can not write");
        }
    }

    private static byte[] StructureToByteArray(TestPacket localTestPacket)
    {
        byte[] bb = new byte[Marshal.SizeOf(localTestPacket)];
        GCHandle gch = GCHandle.Alloc(bb, GCHandleType.Pinned);
        Marshal.StructureToPtr(localTestPacket, gch.AddrOfPinnedObject(), false);
        gch.Free();
        return bb;
    }
}
