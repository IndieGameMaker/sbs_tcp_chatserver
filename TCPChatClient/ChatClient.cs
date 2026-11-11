using System.Net.Sockets;

namespace TCPChatClient;

public class ChatClient
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
}
