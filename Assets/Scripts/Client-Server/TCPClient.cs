using System;
using System.Net.Sockets;
using UnityEngine;
using System.Text;

#if !UNITY_EDITOR
using System.Threading.Tasks;
#endif

public class TCPClient: MonoBehaviour
{
    public string host;
    public int port;

    public string message;
    public string msgType;
    public string receivedMessage;

#if UNITY_EDITOR
    TcpClient tcpClient;
    NetworkStream networkStream;
#else
    Windows.Networking.Sockets.StreamSocket socket;
    Task exchangeTask;
    Stream streamOut;
    Stream streamIn;
#endif
    public PCDWriter pcdWriter;

    byte[] msgLengthBuffer;
    byte[] messageBuffer;
    byte[] msgTypeBuffer;
    private void Start()
    {
        ConnectToServerAsync();
    }

#if UNITY_EDITOR
    public void ConnectToServerAsync()
#else
    private async void ConnectToServerAsync()
#endif
    {
#if UNITY_EDITOR
        try
        {
            msgLengthBuffer = new byte[1024];
            messageBuffer = new byte[1024];
            msgTypeBuffer = new byte[1024];

            tcpClient = new TcpClient(host, port);
            networkStream = tcpClient.GetStream();

            var helloServer = "Hello, Server!";
            var msgType = "HELLO";
            //echoing
            
            SendMessageToNet(helloServer, msgType);

        }
        catch (SocketException ex)
        {
            Console.WriteLine(ex);
        }
#else
        socket = new Windows.Networking.Sockets.StreamSocket();
        Windows.Networking.HostName serverHost = new Windows.Networking.HostName(host);
        await socket.ConnectAsync(serverHost, port.ToString());

        streamOut = socket.OutputStream.AsStreamForWrite();
        streamIn = socket.InputStream.AsStreamForRead();
#endif
    }

    public void Disconnect()
    {
#if UNITY_EDITOR
        SendMessageToNet("", "!DISCONNECT!");
        networkStream.Close();
        tcpClient.Close();
#else
        streamOut.Close();
        streamIn.Close();
        socket.Dispose();
#endif
    }

    public void SendMessageToNet(string message, string msgType)
    {        
        ExchangePackets(message, msgType);
    }

    public void ReceivePacket()
    {
        int bytes;
#if UNITY_EDITOR
        messageBuffer = new byte[tcpClient.ReceiveBufferSize];        
        bytes = networkStream.Read(messageBuffer, 0, messageBuffer.Length);
#else   
        bytes = streamIn.Read(messageBuffer, 0, messageBuffer.Length);
#endif
        receivedMessage = Encoding.UTF8.GetString(messageBuffer, 0, bytes);
        Debug.Log(receivedMessage);
    }

    public void SendPointCloud()
    {
        string pointcloud = pcdWriter.header;
        ExchangePackets(pointcloud, "PCD_FILE");
    }

    public void SendPacket(string message, string msgType)
    {
        messageBuffer = Encoding.UTF8.GetBytes(message);
        msgTypeBuffer = Encoding.UTF8.GetBytes(msgType);
        string msgLength;
        byte[] msgLengthBuffer;

        msgLength = msgTypeBuffer.Length.ToString();
        msgLengthBuffer = Encoding.UTF8.GetBytes(msgLength.PadRight(64, ' '));
#if UNITY_EDITOR
        networkStream.Write(msgLengthBuffer, 0,  msgLengthBuffer.Length);
        networkStream.Write(msgTypeBuffer,   0,    msgTypeBuffer.Length);
#else
        streamOut.Write(msgLengthBuffer, 0, msgLengthBuffer.Length);
        streamOut.Write(msgTypeBuffer,   0,    msgTypeBuffer.Length);
#endif
        msgLength = messageBuffer.Length.ToString();
        msgLengthBuffer = Encoding.UTF8.GetBytes(msgLength.PadRight(64, ' '));
#if UNITY_EDITOR
        networkStream.Write(msgLengthBuffer, 0, msgLengthBuffer.Length);
        networkStream.Write(messageBuffer,   0,   messageBuffer.Length);
#else
        streamOut.Write(msgLengthBuffer, 0, msgLengthBuffer.Length);
        streamOut.Write(messageBuffer,   0,   messageBuffer.Length);
#endif
    }



    public void ExchangePackets(string message, string msgType)
    {
        SendPacket(message, msgType);
        ReceivePacket();
    }
}