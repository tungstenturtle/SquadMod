using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Media.Media3D;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SquadMod
{
    public struct RadioData
    {
        public RadioData(int tagID, Vector3D position)
        {
            this.TagID = tagID;
            this.Position = position;
        }

        public int TagID { get; set; }
        public Vector3D Position { get; set; }
    }

    public class UDPListener
    {
        private static UDPListener instance;

        private static readonly int listenPort = 51515;
        private UdpClient listener = OpenConnection();
        private Queue<RadioData> dataBuffer = new Queue<RadioData>(10);
        private bool done;      //flag used to stop the listen() method potentially because it runs seperately from the rest of the program
        private Vector3D defaultVector = new Vector3D(0, 0, 0);

        private UDPListener() { }

        public static UDPListener Instance
        {
            get
            {
                if (instance == null)
                    instance = new UDPListener();
                
                return instance;
            }
        }

        private static UdpClient OpenConnection()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), listenPort);

            UdpClient client = new UdpClient();
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.Client.Bind(endPoint);
            
            client.Connect(endPoint);

            return client;
        }

        public void Listen()
        {
            done = false;
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), listenPort);
            Task.Run(async () =>
            {
                while (!done)
                {
                    //IPEndPoint object will allow us to read datagrams sent from any source.
                    var receivedBytes = await listener.ReceiveAsync();
                    byte[] data = receivedBytes.Buffer;

                    int tagID;
                    double x, y, z;

                    tagID = BitConverter.ToInt32(data, 0); //returns a 32-bit int from 4 bytes of byte[]
                    x = BitConverter.ToDouble(data, 4); //starting 4 bytes in byte[], reads 8 bytes and converts to a double   
                    y = BitConverter.ToDouble(data, 12);
                    z = BitConverter.ToDouble(data, 20);
                    
                    if (dataBuffer.Count == 10)
                        dataBuffer.Dequeue();
                    dataBuffer.Enqueue(new RadioData(tagID, new Vector3D(x, y, z)));
                }
            });
        }

        public Vector3D NextPoint()
        {
            try
            {
                return dataBuffer.Dequeue().Position;
            }
            catch
            {
                return defaultVector;
            }
        }

        public void Stop() //kills the listen method by setting the flag to true
        {
            done = true;
        }

        public void Close()
        {
            listener.Close();
        }
    }
}