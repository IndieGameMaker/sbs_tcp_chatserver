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
