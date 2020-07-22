using System;
using System.IO;
using System.Net.Sockets;
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
    public byte objType; // 1:Person 2:Movable Equipment 3:Unidentified object
    public fixed char time[20];
    public int objX1;
    public int objY1;
    public int objX2;
    public int objY2;
    public int percent; // 0 ~ 100
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

public class MyClient
{
    // ========== ID ==========
    public int clientID = -1;

    // ========== Network ==========
    public byte[] recvBuffer = new byte[Define.recvBufferSize];
    public Socket mySocket;
    // public NetworkStream networkStream;
    // public StreamReader streamReader;

    // ========== UI ==========
    private Text logText = null;
    private ScrollRect scrollRect = null;

    public MyClient(int clientID, Socket mySocket)
    {
        this.clientID = clientID;
        this.mySocket = mySocket;
    }

    public void Start()
    {
        logText = GameObject.Find("Log").GetComponent<Text>();
        scrollRect = GameObject.Find("ScrollView").GetComponent<ScrollRect>();
    }

    /*
    public void Recv()
    {
        int recvSize = -1;

        while (true)
        {
            recvSize = networkStream.Read(recvBuffer, 0, Define.recvBufferSize);
            if (recvSize == 0) break;

            // Debug.Log("recvSize = " + recvSize);

            // ProccessRecv(recvBuffer);
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
                    + timeStr + " " + if_sc100.objX1 + " " + if_sc100.objY1 + " " + if_sc100.objX2 + " " + if_sc100.objY2 + " " + if_sc100.percent + "\n";
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
    */

    static public object ByteArrayToStructure(byte[] byteData, Type type)
    {
        GCHandle gch = GCHandle.Alloc(byteData, GCHandleType.Pinned);
        object result = Marshal.PtrToStructure(gch.AddrOfPinnedObject(), type);
        gch.Free();
        return result;
    }
}
