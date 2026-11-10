using System.Net.Sockets;

namespace TCPChatServer;

public class ConnectedClient : IDisposable
{
    // TCP 클라이언트
    private readonly TcpClient _client;
    // 네트워크 스트림(Network Stream)
    private readonly NetworkStream _stream;  // byte[]
    // 스트림 - 읽기전용 (수신)
    private readonly StreamReader _reader;   // string  \n
    // 스트림 - 쓰기전용 (송신)
    private readonly StreamWriter _writer;
    
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
        
        // 스트림 초기화 (UTF-8 인코딩)
        _reader = new StreamReader(_stream, System.Text.Encoding.UTF8);
        _writer = new StreamWriter(_stream, System.Text.Encoding.UTF8) { AutoFlush = true };
        
        _clientId = _client.Client.RemoteEndPoint?.ToString() ?? Guid.NewGuid().ToString();
        _isDisposed = false;
        
        Console.WriteLine($"[연결] 클라이언트가 연결되었습니다.: {_clientId}");
    }
    
    // 비동기 메시지 수신 처리
    public async Task ReceiveMessageAsync()
    {
        try
        {
            while (IsConnected)
            {
                // 한줄씩 읽기
                string? message = await _reader.ReadLineAsync();
                
                // 연결이 끊어지면 null 반환
                if (message == null)
                {
                    Console.WriteLine($"[연결 종료] {_clientId}");
                    break;
                }
                
                Console.WriteLine($"[수신] {_clientId} : {message}");
            }
        }
        catch (Exception e)
        {
            if (!_isDisposed)
            {
                Console.WriteLine(e);
            }
        }
        finally
        {
            Dispose();
        }
    }
    
    
    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;
        
        _stream.Dispose();
        _client.Dispose();
    }
}
