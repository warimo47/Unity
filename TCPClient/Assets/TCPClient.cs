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

public class TCPClient : MonoBehaviour
{
    TcpClient client;
    NetworkStream ns;
    StreamWriter writer;

    IF_SC100 if_sc100;

    // Start is called before the first frame update
    unsafe void Start()
    {
        if_sc100.header = 1;

        if_sc100.ID[0] = 'H';
        if_sc100.ID[1] = 'u';
        if_sc100.ID[2] = 'm';
        if_sc100.ID[3] = 'a';
        if_sc100.ID[4] = 'n';

        if_sc100.IP[0] = '1';
        if_sc100.IP[1] = '9';
        if_sc100.IP[2] = '2';

        if_sc100.latitude = 123.345678f;
        if_sc100.longitude = -789.345678f;
        if_sc100.objType = 2;
        if_sc100.isApproved = 1;
        if_sc100.accidentRiskLevel = 3;

        client = new TcpClient("127.0.0.1", Int32.Parse("4211")); // (ip주소 , 포트 번호)
        ns = client.GetStream();
        writer = new StreamWriter(ns);

        Debug.Log("Server connet");
    }

    // Update is called once per frame
    unsafe void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space");

            if (ns.CanWrite == true)
            {
                ns.Write(StructureToByteArray(if_sc100), 0, sizeof(IF_SC100));
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
}
