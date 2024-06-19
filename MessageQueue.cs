using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace DRGSoundPad
{

    public class DRGMessage
    {
        public string player;
        public string msg;
    }


    public class MessageQueue
    {
        private const string pipeName = "drg_named_pipe";
        private NamedPipeServerStream pipeServer = null;
        private Action<DRGMessage> callback;

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


        public void SetCallBack(Action<DRGMessage> callback)
        {
            this.callback = callback;
        }

        public void MainLoop()
        {

            while (true)
            {
                try
                {
                    pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.None);
                    Console.WriteLine("Server created.");
                    Console.WriteLine("Waiting for client connection...");
                    pipeServer.WaitForConnection();
                    Console.WriteLine("Client connected.");

                    string? data = Read();
                    if (data?.Length > 0)
                    {
                        DRGMessage? msg = JsonConvert.DeserializeObject<DRGMessage>(data);
                        if (msg != null)
                            this.callback(msg);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: {0}", e.ToString());
                }

                Thread.Sleep(100);

            }

        }
    }
}
