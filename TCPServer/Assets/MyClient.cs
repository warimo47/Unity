using System;
using System.IO;
using System.Net.Sockets;
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

public class MyClient
{
    // ========== TCP Module ==========
    private TCPServer myTCPServer = null;

    // ========== Network ==========
    public byte[] recvBuffer = new byte[Define.recvBufferSize];
    public Socket mySocket;

    // ========== UI ==========
    private Text logText;
    private ScrollRect scrollRect;

    public MyClient(TCPServer localTCPServer,  Socket localMySocket, Text localLogText, ScrollRect localScrollRect)
    {
        myTCPServer = localTCPServer;
        mySocket = localMySocket;

        logText = localLogText;
        scrollRect = localScrollRect;
    }

    ~MyClient()
    {
        // Debug.Log("Delete() " + mySocket.RemoteEndPoint.ToString());
    }

    // 아무 패킷이나 받으면 호출 되는 콜벡 함수
    public void PacketReceived(IAsyncResult ar)
    {
        try
        {
            // 패킷 수신 함수 호출
            int received = mySocket.EndReceive(ar);

            // Debug.Log("EndReceive() called");

            // 연결 끊김
            if (received == 0)
            {
                mySocket.Close();

                myTCPServer.DisconnectClient(this);

                return;
            }

            // 패킷 처리 함수 호출
            ProccessRecv();
        }
        catch (Exception ex)
        {
            Debug.Log("EndReceive() " + ex.ToString());
        }

        try
        {
            // 다시 비동기 수신 대기
            mySocket.BeginReceive(recvBuffer, 0, Define.recvBufferSize, 0, PacketReceived, this);
        }
        catch (Exception ex)
        {
            Debug.Log("BeginReceive() " + ex.ToString());
        }
    }
    
    // 패킷 처리 함수
    public unsafe void ProccessRecv()
    {
        string logString = "[" + mySocket.RemoteEndPoint.ToString() + "] ";

        // 패킷의 종류에 따라 분기
        switch ((int)recvBuffer[0])
        {
            case 1:
                TestPacket testPacket = (TestPacket)ByteArrayToStructure(recvBuffer, typeof(TestPacket));
                string ipStr = new string(testPacket.stringParam);
                logString += ipStr + " " + testPacket.doubleParam + " " + testPacket.intParam + "\n";
                break;
            default:
                Debug.Log("recv wrong data " + recvBuffer[0]);
                break;
        }

        // 로그 출력 할 것이 있으면 큐에 넣음
        myTCPServer.atomicQueue.Enqueue(logString);
    }

    // 버퍼를 구조체로 형변환해주는 함수
    static public object ByteArrayToStructure(byte[] byteData, Type type)
    {
        GCHandle gch = GCHandle.Alloc(byteData, GCHandleType.Pinned);
        object result = Marshal.PtrToStructure(gch.AddrOfPinnedObject(), type);
        gch.Free();
        return result;
    }
}
