using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

static class Define
{
    public const int recvBufferSize = 1024;
    public const int portNum = 4211;
}

public class TCPServer : MonoBehaviour
{
    // ========== UI ==========
    private Text logText = null;
    private ScrollRect scrollRect = null;

    // ========== Atomic queue  ==========
    public ConcurrentQueue<string> atomicQueue = null;

    // ========== Network ==========
    private Socket mainSock = null;
    private List<MyClient> myClientsList = null;

    // Start is called before the first frame update
    void Start()
    {
        // UI 래퍼런스 가져오기
        logText = GameObject.Find("Log").GetComponent<Text>();
        scrollRect = GameObject.Find("ScrollView").GetComponent<ScrollRect>();

        // 멀티스레드 안전 큐 초기화
        atomicQueue = new ConcurrentQueue<string>();

        // 소켓 생성
        mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

        // 클라이언트 리스트 초기화
        myClientsList = new List<MyClient>();

        // 소켓 오픈
        IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, Define.portNum);

        // 소켓 바인드
        mainSock.Bind(serverEP);

        // 소켓 리슨
        mainSock.Listen(10); // Why 10 ???

        try
        {
            // Accept 시작
            mainSock.BeginAccept(ClinetAccept, null);
        }
        catch (Exception ex)
        {
            Debug.Log("FirstBeginAccept() " + ex.ToString());
        }

        // 서버 시작 UI 표시
        atomicQueue.Enqueue("Server Start\n");
    }

    private void Update()
    {
        string logStr = "";

        // 큐에 로그 쌓을 것이 있는 지 확인
        while (atomicQueue.IsEmpty == false)
        {
            // 로그 쌓은 것이 있으면
            if (atomicQueue.TryDequeue(out logStr))
            {
                // Log UI에 추가
                if (logText.text.Length > 10000) logText.text = "";
                logText.text += logStr;
            }
        }

        // UI 스크롤을 맨 마지막으로 내려줌
        scrollRect.verticalNormalizedPosition = 0.0f;
    }

    private void ClinetAccept(IAsyncResult ar)
    {
        Socket acceptedSocket = null;
        MyClient newClient = null;

        try
        {
            // 클라이언트의 연결 요청 수락
            acceptedSocket = mainSock.EndAccept(ar);

            // 연결된 클라이언트 IP:Port UI 표시
            atomicQueue.Enqueue("ClinetAccept() " + acceptedSocket.RemoteEndPoint.ToString() + "\n");
        }
        catch (Exception ex)
        {
            Debug.Log("EndAccept() " + ex.ToString());
        }

        try
        {
            // 또 다른 클라이언트의 연결 대기
            mainSock.BeginAccept(ClinetAccept, null);
        }
        catch (Exception ex)
        {
            Debug.Log("BeginAccept() " + ex.ToString());
        }

        try
        {
            // 새로운 클라이언트 객체 생성
            newClient = new MyClient(this, acceptedSocket, logText, scrollRect);

            // 새로운 클라이언트 객체 배열에 추가
            myClientsList.Add(newClient);
        }
        catch (Exception ex)
        {
            Debug.Log("new MyClient() " + ex.ToString());
        }

        if (newClient != null)
        {
            try
            {
                // 비동기적 Recv 시작
                acceptedSocket.BeginReceive(newClient.recvBuffer, 0, Define.recvBufferSize, 0, newClient.PacketReceived, newClient);
            }
            catch (Exception ex)
            {
                Debug.Log("BeginReceive() " + ex.ToString());
            }
        }
    }

    public void DisconnectClient(MyClient localMyClient)
    {
        // 클라이언트 리스트에서 연결이 끊어진 클라이언트 제거
        myClientsList.Remove(localMyClient);
    }
}
