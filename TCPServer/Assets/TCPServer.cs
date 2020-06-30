using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct IF_SC100
{
    public byte header;
    public fixed char ID[32];
    public fixed char IP[30];
    public double latitude;
    public double longitude;
    public byte objType;
    public byte isApproved;
    public byte accidentRiskLevel;
};

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct IF_SC200
{
    public byte header;
    public fixed char sectionName[32];
    public fixed char IP[30];
    public byte accidentRiskType;
};

public class MyClient
{
    public static int recvBufferSize = 200;
    public byte[] recvBuffer = new Byte[recvBufferSize];
    public TcpClient tcpClient;
    public NetworkStream networkStream;
    public StreamReader streamReader;

    public void AcceptClient(TcpClient tmpClient)
    {
        this.tcpClient = tmpClient;

        this.networkStream = this.tcpClient.GetStream();
        this.streamReader = new StreamReader(this.networkStream);

        Debug.Log("Accept " + this.tcpClient.Client.RemoteEndPoint.ToString());
    }

    public void Recv()
    {
        int recvSize = -1;

        while (true)
        {
            recvSize = networkStream.Read(recvBuffer, 0, recvBufferSize);
            if (recvSize == 0) break;

            // Debug.Log("recvSize = " + recvSize);

            ProccessRecv(recvBuffer);
        }
    }

    public unsafe void ProccessRecv(byte[] recvBuffer)
    {
        switch ((int)recvBuffer[0])
        {
            case 1:
                IF_SC100 if_sc100 = (IF_SC100)ByteArrayToStructure(recvBuffer, typeof(IF_SC100));
                string id = new string(if_sc100.ID);
                string ipstr = new string(if_sc100.IP);
                string debugLine = "[" + this.tcpClient.Client.RemoteEndPoint.ToString() + "] : "
                    + id + " " + ipstr + " " + if_sc100.latitude + " " + if_sc100.longitude + " " + if_sc100.objType + " " + if_sc100.isApproved + " " + if_sc100.accidentRiskLevel;
                Debug.Log(debugLine);
                break;
            case 2:
                IF_SC200 if_sc200 = (IF_SC200)ByteArrayToStructure(recvBuffer, typeof(IF_SC200));
                string sectionNameStr = new string(if_sc200.sectionName);
                string ipstr2 = new string(if_sc200.IP);
                string debugLine2 = "[" + this.tcpClient.Client.RemoteEndPoint.ToString() + "] : "
                    + sectionNameStr + " " + ipstr2 + " " + if_sc200.accidentRiskType;
                Debug.Log(debugLine2);
                break;
            default:
                Debug.Log("recv wrong data " + recvBuffer[0]);
                break;
        }
    }

    static public object ByteArrayToStructure(byte[] byteData, Type type)
    {
        GCHandle gch = GCHandle.Alloc(byteData, GCHandleType.Pinned);
        object result = Marshal.PtrToStructure(gch.AddrOfPinnedObject(), type);
        gch.Free();
        return result;
    }

    //public static object ByteArrayToStructure(byte[] data, Type type)
    //{
    //    IntPtr buff = Marshal.AllocHGlobal(data.Length); // 배열의 크기만큼 비관리 메모리 영역에 메모리를 할당한다.

    //    Marshal.Copy(data, 0, buff, data.Length); // 배열에 저장된 데이터를 위에서 할당한 메모리 영역에 복사한다.
    //    object obj = Marshal.PtrToStructure(buff, type); // 복사된 데이터를 구조체 객체로 변환한다.

    //    Marshal.FreeHGlobal(buff); // 비관리 메모리 영역에 할당했던 메모리를 해제함

    //    if (Marshal.SizeOf(obj) != data.Length) // (((PACKET_DATA)obj).TotalBytes != data.Length) // 구조체와 원래의 데이터의 크기 비교
    //    {
    //        return null; // 크기가 다르면 null 리턴
    //    }

    //    return obj; // 구조체 리턴
    //}
};

public class TCPServer : MonoBehaviour
{
    private Thread acceptThread;

    private MyClient[] myClients = new MyClient[4];
    private int clientCount = 0;

    private TcpListener tcp_Listener;

    // Start is called before the first frame update
    void Start()
    {
        myClients[0] = new MyClient();
        myClients[1] = new MyClient();
        myClients[2] = new MyClient();
        myClients[3] = new MyClient();

        this.tcp_Listener = new TcpListener(IPAddress.Any, 4211);
        this.tcp_Listener.Start();

        acceptThread = new Thread(new ThreadStart(this.Accept), 8192);
        acceptThread.Start();

        Debug.Log("Server Start");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        for(int i = 0; i < 4; ++i)
        {
            // streamReaders[i].Close();
            // clients[i].Close();
        }
    }

    public void Accept()
    {
        while (true)
        {
            TcpClient tempTCPClient = this.tcp_Listener.AcceptTcpClient(); // 클라이언트와 접속
            this.myClients[clientCount].AcceptClient(tempTCPClient);

            Thread recvThread = new Thread(new ThreadStart(this.myClients[clientCount].Recv), 8192);

            clientCount++;

            recvThread.Start();
        }
    }
}
