using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
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

public class TCPClient : MonoBehaviour
{
    TcpClient client;
    NetworkStream ns;
    StreamWriter writer;

    IF_SC100 if_sc100;
    IF_SC200 if_sc200;

    // Start is called before the first frame update
    unsafe void Start()
    {
        Screen.SetResolution(640, 480, true);

        if_sc100.header = 1;

        string myID = "Human";
        for (int i = 0; i < myID.Length; ++i)
        {
            if_sc100.ID[i] = myID[i];
        }

        string myip = "";
        foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                myip = ip.ToString();
                break;
            }
        }
        for(int i = 0; i < myip.Length; ++i)
        {
            if_sc100.IP[i] = myip[i];
            if_sc200.IP[i] = myip[i];
        }

        if_sc100.latitude = 123.345678f;
        if_sc100.longitude = -789.345678f;
        if_sc100.objType = 2;
        if_sc100.isApproved = 1;
        if_sc100.accidentRiskLevel = 3;

        if_sc200.header = 2;
        string mySectionName = "Zone1";
        for (int i = 0; i < myip.Length; ++i)
        {
            if_sc200.IP[i] = myip[i];
        }
        for (int i = 0; i < mySectionName.Length; ++i)
        {
            if_sc200.sectionName[i] = mySectionName[i];
        }
        if_sc200.accidentRiskType = 2;

        client = new TcpClient("127.0.0.1", Int32.Parse("4211")); // (ip주소 , 포트 번호)
        ns = client.GetStream();
        writer = new StreamWriter(ns);

        Debug.Log("Server connet");
    }

    // Update is called once per frame
    unsafe void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Debug.Log("Send IF_SC100");

            if (ns.CanWrite == true)
            {
                ns.Write(StructureToByteArray(if_sc100), 0, sizeof(IF_SC100));
            }
            else
            {
                Debug.Log("Can not write");
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            Debug.Log("Send IF_SC200");

            if (ns.CanWrite == true)
            {
                ns.Write(StructureToByteArray(if_sc200), 0, sizeof(IF_SC200));
            }
            else
            {
                Debug.Log("Can not write");
            }
        }
    }

    void Close()
    {
        writer.Close();    
        client.Close();
    }

    public static byte[] StructureToByteArray(IF_SC100 if_sc100)
    {
        byte[] bb = new byte[Marshal.SizeOf(if_sc100)];
        GCHandle gch = GCHandle.Alloc(bb, GCHandleType.Pinned);
        Marshal.StructureToPtr(if_sc100, gch.AddrOfPinnedObject(), false);
        gch.Free();
        return bb;
    }

    public static byte[] StructureToByteArray(IF_SC200 if_sc200)
    {
        byte[] bb = new byte[Marshal.SizeOf(if_sc200)];
        GCHandle gch = GCHandle.Alloc(bb, GCHandleType.Pinned);
        Marshal.StructureToPtr(if_sc200, gch.AddrOfPinnedObject(), false);
        gch.Free();
        return bb;
    }
}
