namespace TCPChatServer;

class Program
{
    static void Main(string[] args)
    {
        // Windows 콘솔에서 UTF-8 인코딩 설정
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;
        
        // 기본 포트 번호
        const int port = 7000;
        
        // 채팅서버 인스턴스를 생성 & 시작
        var server = new ChatServer(port);
        // 서버 시작
        server.Start();
        
        Console.WriteLine("서버를 중지하려면 아무 키나 누르세요...");
        Console.ReadKey(true);
        
        // TODO: 서버 중지 로직 추가
    }
}