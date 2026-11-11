using System.Collections.Concurrent;
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

    // Thread-Safe 한 컬랙션을 사용
    private readonly ConcurrentDictionary<string, ConnectedClient> _clients;
    
    // 생성자
    public ChatServer(int port)
    {
        _port = port;
        _isRunning = false;
        _clients = new ConcurrentDictionary<string, ConnectedClient>();
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
                
                // 접속한 클라이언트 저장
                var connectedClinet = new ConnectedClient(client);
                // 클라이언트 아이디 생성
                var clientId = connectedClinet.ClientId;
                // 클라이언트 목록 추가
                // _clients.TryAdd(clientId, connectedClinet);
                // 인덱서 방식
                _clients[clientId] = connectedClinet;
                
                // 이벤트 구독
                connectedClinet.OnMessageReceived += onMessageReceived;
                
                // 연결된 클라이언트 정보 출력 (EndPoint = IPAddress + Port)
                // var endPoint = client.Client.RemoteEndPoint;
                Console.WriteLine($"[정보] 현재 연결된 클라이언트 수: {_clients.Count}");
                
                // 클라이언트로 부터 메시지 수신 시작 (비동기)
                _ = Task.Run(() => connectedClinet.ReceiveMessageAsync());
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
    
    private void onMessageReceived(ConnectedClient sender, string message)
    {
        // 메시지 포맷 : [발신자ID]: 메시지
        string msg = $"[{sender.ClientId}] {message}";
        Console.WriteLine($"[브로드캐스팅] {msg}");
        
        _ = Task.Run(() => BroadcastMessageAsync(message));
    }
    
    // 같은 채팅방에 접속한 모든 클라이언트에게 메시지를 전달(Broadcasting)
    private async Task BroadcastMessageAsync(string message)
    {
        foreach (var client in _clients)
        {
            if (client.Value.IsConnected)
            {
                await client.Value.SendMessageAsync(message);
            }
        }
    }
}

/* BOM (Byte Order Mark)
 *
 * 
 */
