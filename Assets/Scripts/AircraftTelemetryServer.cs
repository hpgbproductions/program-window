/*
 * Reference:
 * 
 * https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-use-named-pipes-for-network-interprocess-communication
 * http://v01ver-howto.blogspot.com/2010/04/howto-use-named-pipes-to-communicate.html
 */

/*

using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;

public class AircraftTelemetryServer : MonoBehaviour
{
    private static int numThreads = 8;

    private static bool quit = false;

    private Thread[] servers;

    private void Start()
    {
        servers = new Thread[numThreads];
        for (int i = 0; i < numThreads; i++)
        {
            servers[i] = new Thread(TelemetryThread);
            servers[i].Start();
            Debug.Log($"Thread {servers[i].ManagedThreadId}: AircraftTelemetryServer thread created");
        }
    }

    private void OnApplicationQuit()
    {
        for (int i = 0; i < numThreads; i++)
        {
            quit = true;
            servers[i].Abort();
        }
    }

    public static void TelemetryThread()
    {
        int threadId = Thread.CurrentThread.ManagedThreadId;

        NamedPipeServerStream stream = new NamedPipeServerStream("SPTelemetryServer", PipeDirection.InOut, numThreads);

        while (!quit)
        {
            // Wait for connection
            while (!stream.IsConnected)
            {
                if (quit)
                {
                    return;
                }
                Thread.Sleep(500);
            }

            StreamReader sr = new StreamReader(stream, Encoding.UTF8);
            StreamWriter sw = new StreamWriter(stream, Encoding.UTF8);

            try
            {
                sw.Write($"Connected to AircraftTelemetryServer thread {threadId}");
                stream.Flush();

                while (sr.Read() == -1)
                {
                    Thread.Sleep(15);
                }

                sw.Write("Hello, world!\n");
                stream.Flush();
            }
            catch (IOException)
            {
                // On client disconnect
                stream.Close();
                stream = new NamedPipeServerStream("SPTelemetryServer", PipeDirection.InOut, numThreads);
            }
        }
        
        stream.Close();
    }
}
*/