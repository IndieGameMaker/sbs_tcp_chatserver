#pragma warning disable CS0649, CS0169, CS8618
using System.Net.Sockets;
using System.Text;

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
    
    public bool IsConnected => _isConnected && !_isDisposed;
    
    // 생성자
    public ChatClient(string serverIp, int serverPort)
    {
        _serverIp = serverIp;
        _serverPort = serverPort;
        _isDisposed = false;
        _isConnected = false;
    }

    #region 서버연결

    public async Task ConnectAsync()
    {
        if (_isConnected)
        {
            Console.WriteLine("이미 서버에 연결되어 있습니다.");
            return;
        }

        try
        {
            Console.WriteLine($"{_serverIp}:{_serverPort} 서버 연결 중 ...");
            
            // TcpClient 생성 및 서버 연결
            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(_serverIp, _serverPort);
            
            // 스트림 생성
            _stream = _tcpClient.GetStream();
            _reader = new StreamReader(_stream, Encoding.UTF8);
            _writer = new StreamWriter(_stream, new UTF8Encoding(false)) { AutoFlush = true };
            
            _isConnected = true;
            Console.WriteLine("서버에 연결되었습니다.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"서버 연결 실패 {e.Message}");
            throw;
        }
    }

    #endregion

    #region 메시지 송수신
    // 메시지 비동기 수신
    public async Task ReceiveMessageAsync()
    {
        if (!IsConnected || _reader == null)
        {
            Console.WriteLine("서버에 연결되지 않았습니다.");
            return;
        }

        try
        {
            while (IsConnected)
            {
                // 한 라인씩 읽기 (\n)
                string? message = await _reader.ReadLineAsync();

                if (message == null)
                {
                    if (IsConnected)
                    {
                        Console.WriteLine("서버와 연결이 종료되었습니다.");
                    }

                    break;
                }
                Console.WriteLine($"{message}");
            }
        }
        catch (Exception e)
        {
            if (IsConnected)
            {
                Console.WriteLine($"메시지 수신 오류:{e.Message}");
            }
        }
        finally
        {
           Dispose(); 
        }
    }
    
    // 메시지 전송 (비동기)
    public async Task SendMessageAsync(string message)
    {
        if (!IsConnected || _writer == null)
        {
            Console.WriteLine("서버에 연결되지 않았습니다.");
            return;
        }

        try
        {
            await _writer.WriteLineAsync(message);
        }
        catch (Exception e)
        {
            Console.WriteLine($"메시지 전송 오류:{e.Message}");
        }
    }
    
    #endregion
    
    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;
        _isConnected = false;
        Console.WriteLine("연결 종료 중 ...");
        
        _reader?.Dispose();
        _writer?.Dispose();
        _stream?.Dispose();
        _tcpClient?.Dispose();
        Console.WriteLine("연결 종료되었습니다.");
        
    }
}
