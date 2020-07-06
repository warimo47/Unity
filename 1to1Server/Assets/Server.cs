using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
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

public class Server : MonoBehaviour
{
    public static int recvBufferSize = 200;

    private TcpListener tcpListener;
    private Thread acceptThread;
    private TcpClient tcpClient;
    private NetworkStream networkStream;
    private int recvSize = -1;
    private byte[] recvBuffer = new Byte[recvBufferSize];

    private Text logText = null;
    private ScrollRect scrollRect = null;

    // Start is called before the first frame update
    void Start()
    {
        logText = GameObject.Find("Log").GetComponent<Text>();
        scrollRect = GameObject.Find("ScrollView").GetComponent<ScrollRect>();

        tcpListener = new TcpListener(IPAddress.Any, 4211);
        tcpListener.Start();

        acceptThread = new Thread(new ThreadStart(this.Accept), 8192);
        acceptThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        Recv();
    }

    public void Accept()
    {
        tcpClient = tcpListener.AcceptTcpClient(); // 클라이언트와 접속
        networkStream = tcpClient.GetStream();
    }

    public void Recv()
    {
        if (networkStream != null)
        {
            if (networkStream.CanRead == true)
            {
                recvSize = networkStream.Read(recvBuffer, 0, recvBufferSize);

                // Debug.Log("recvSize = " + recvSize);

                if (recvSize == 0)
                {
                    Debug.Log("recvSize = 0");
                    Quit();
                }

                ProccessRecv(recvBuffer);
            }
        }
    }

    public unsafe void ProccessRecv(byte[] recvBuffer)
    {
        string debugLine = "[" + this.tcpClient.Client.RemoteEndPoint.ToString() + "] : ";

        switch ((int)recvBuffer[0])
        {
            case 1:
                IF_SC100 if_sc100 = (IF_SC100)ByteArrayToStructure(recvBuffer, typeof(IF_SC100));
                string ipStr = new string(if_sc100.IP);
                string timeStr = new string(if_sc100.time);
                debugLine += ipStr + " " + if_sc100.latitude + " " + if_sc100.longitude + " " + if_sc100.objType + " "
                    + timeStr + " " + if_sc100.objX1 + " " + if_sc100.objY1 + " " + if_sc100.objX2 + " " + if_sc100.objY2 + "\n";
                break;
            case 2:
                IF_SC200 if_sc200 = (IF_SC200)ByteArrayToStructure(recvBuffer, typeof(IF_SC200));
                debugLine += "\n";
                break;
            case 3:
                IF_SC300 if_sc300 = (IF_SC300)ByteArrayToStructure(recvBuffer, typeof(IF_SC300));
                debugLine += "\n";
                break;
            default:
                Debug.Log("recv wrong data " + recvBuffer[0]);
                break;
        }

        logText.text += debugLine;
        scrollRect.verticalNormalizedPosition = 0.0f;

        Debug.Log(debugLine);
    }

    static public object ByteArrayToStructure(byte[] byteData, Type type)
    {
        GCHandle gch = GCHandle.Alloc(byteData, GCHandleType.Pinned);
        object result = Marshal.PtrToStructure(gch.AddrOfPinnedObject(), type);
        gch.Free();
        return result;
    }

    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
