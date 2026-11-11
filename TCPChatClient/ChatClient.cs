#pragma warning disable CS0649, CS0169, CS8618
using System.Net.Sockets;

namespace TCPChatClient;

public class ChatClient : IDisposable
{
    // TCP 클라이언트
    private TcpClient? _tcpClient;
    
    private NetworkStream? _stream;
    private StreamWriter? _writer;
    private StreamReader? _reader;
    
    // 서버와 연결 여부
    private bool _isConnected;
    // 리소스 해제 여부
    private bool _isDisposed;
    
    // 서버 IP 주소 및 포트 번호
    private readonly string _serverIp;
    private readonly int _serverPort;
    
    public void Dispose()
    {
        _reader?.Dispose();
        _writer?.Dispose();
        _stream?.Dispose();
        _tcpClient?.Dispose();
    }
}
