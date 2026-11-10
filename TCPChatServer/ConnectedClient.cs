using System.Net.Sockets;

namespace TCPChatServer;

public class ConnectedClient : IDisposable
{
    // TCP 클라이언트
    private readonly TcpClient _client;
    // 네트워크 스트림(Network Stream)
    private readonly NetworkStream _stream;
    // 클라이언트 ID 
    private readonly string _clientId;
    // 메모리 해제 여부
    private bool _isDisposed;
    
    // 클라이언트 ID 프로퍼티
    public string ClientId => _clientId;
    // 클라이언트 연결 여부 프로퍼티
    public bool IsConnected => !_isDisposed && _client.Connected;
    
    // 생성자
    public ConnectedClient(TcpClient client)
    {
        _client = client;
        _stream = client.GetStream();
        
        _clientId = _client.Client.RemoteEndPoint?.ToString() ?? Guid.NewGuid().ToString();
        _isDisposed = false;
        
        Console.WriteLine($"[연결] 클라이언트가 연결되었습니다.: {_clientId}");
    }
    
    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;
        
        _stream.Dispose();
        _client.Dispose();
    }
}
