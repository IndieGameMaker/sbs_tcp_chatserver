namespace TCPChatClient;

class Program
{
    static async Task Main(string[] args)
    {
        const string SERVER_IP = "127.0.0.1";
        const int SERVER_PORT = 7700;
        
        Console.WriteLine("======= TCP 채팅 클라이언트 =======");
        
        using var client = new ChatClient(SERVER_IP, SERVER_PORT);

        try
        {
            await client.ConnectAsync();
            // 메시지 수신 처리
            _ = Task.Run(() => client.ReceiveMessageAsync());
            
            // 사용자 입력 루프
            Console.WriteLine("메시지를 입력하세요. (종료:exit)");
            Console.WriteLine("==========================");

            while (client.IsConnected)
            {
                // 사용자 입력 받기
                string? input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) continue;

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("채팅을 종료합니다. ...");
                    break;
                }
                
                // 서버에 메시지 전송
                //await client.
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"[오류] {e.Message}");
        }
    }
}