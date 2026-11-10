using System.Net;
using System.Net.Sockets;

namespace TCPChatServer;

public class ChatServer
{
    // TCP 연결을 기다리는 리스너(Listener)
    private TcpListener? _listener;
    
    // 서버가 실행중인지 여부
    private bool _isRunning;
    
    // 서버 포트
    private readonly int _port;
    
    // 생성자
    public ChatServer(int port)
    {
        _port = port;
        _isRunning = false;
    }

    // 서버 시작 메서드
    public void Start()
    {
        if (_isRunning)
        {
            Console.WriteLine("서버가 이미 실행 중입니다.");
            return;
        }
        
        // TCP 리스너 초기화 및 생성
        _listener = new TcpListener(IPAddress.Any, _port);
        _listener.Start();
        _isRunning = true;
        
        Console.WriteLine($"서버가 포트 {_port}에서 시작되었습니다.");
        
        // 백그라운드에서 클라이언트 연결 수락 구동(비동기)
        _ = Task.Run(AcceptClientAsync);
    }
    
    // 클라이언트 연결 (비동기)
    private async Task AcceptClientAsync()
    {
        Console.WriteLine("클라이언트 연결을 기다리는 중 ...");
        while (_isRunning)
        {
            try
            {
                // 클라이언트 연결 수락
                var client = await _listener!.AcceptTcpClientAsync();
                
                // 연결된 클라이언트 정보 출력 (EndPoint = IPAddress + Port)
                var endPoint = client.Client.RemoteEndPoint;
                Console.WriteLine($"[연결] 클라이언트가 연결되었습니다.: {endPoint}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public void Stop()
    {
        if (!_isRunning)
        {
            Console.WriteLine("서버가 이미 종료되었습니다.");
            return;
        }

        _isRunning = false;
        _listener?.Stop();
        Console.WriteLine("서버가 중지되었습니다.");
    }
}
