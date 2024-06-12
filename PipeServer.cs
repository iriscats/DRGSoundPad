using System.IO.Pipes;


namespace DRGSoundPad
{

    class NamedPipeServer
    {
        public static void Main()
        {
            const string pipeName = "my_named_pipe";
            using (NamedPipeServerStream pipeServer =
                new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.None))
            {
                Console.WriteLine("NamedPipeServerStream object created.");

                // 等待客户端连接  
                Console.WriteLine("Waiting for client connection...");
                pipeServer.WaitForConnection();
                Console.WriteLine("Client connected.");

                try
                {
                    // 读取客户端发送的消息  
                    using (StreamReader sr = new StreamReader(pipeServer))
                    {
                        string temp = sr.ReadLine();
                        Console.WriteLine("Received from client: {0}", temp);
                    }

                    // 发送响应给客户端  
                    using (StreamWriter sw = new StreamWriter(pipeServer))
                    {
                        sw.WriteLine("Hello from server!");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: {0}", e.ToString());
                }
            }
        }
    }
}
