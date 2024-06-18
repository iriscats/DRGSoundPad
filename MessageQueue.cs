using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace DRGSoundPad
{

    public class MessageQueue
    {
        private const string pipeName = "drg_named_pipe";
        private NamedPipeServerStream pipeServer = null;
        private Action<string> callback;

        public MessageQueue()
        {

        }


        private void Write(String txt)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(pipeServer))
                {
                    sw.WriteLine(txt);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
            }
        }

        private string? Read()
        {
            string? temp = null;
            try
            {
                using (StreamReader sr = new StreamReader(pipeServer))
                {
                    temp = sr.ReadLine();
                    Console.WriteLine("Received from client: {0}", temp);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
            }
            return temp;
        }


        public void SetCallBack(Action<string> callback)
        {
            this.callback = callback;
        }

        public void MainLoop()
        {

            while (true)
            {
                pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.None);
                Console.WriteLine("Server created.");
                Console.WriteLine("Waiting for client connection...");
                pipeServer.WaitForConnection();
                Console.WriteLine("Client connected.");

                string? msg = Read();
                if (msg?.Length > 0)
                {
                    this.callback(msg);
                }
                Thread.Sleep(100);

            }

        }
    }
}
