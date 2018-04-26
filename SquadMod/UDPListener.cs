using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Media.Media3D;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SquadMod
{
    /// <summary>
    /// A struct to store data about the position of UWB radios
    /// </summary>
    public struct RadioData
    {
        /// <summary>
        /// Creates a new RadioData struct with the given parameters
        /// </summary>
        /// <param name="tagID">the ID of the tag radio the point is from</param>
        /// <param name="position">the position of the tag radio</param>
        public RadioData(int tagID, Vector3D position)
        {
            this.TagID = tagID;
            this.Position = position;
        }

        /// <summary>
        /// The tag's ID
        /// </summary>
        public int TagID { get; set; }

        /// <summary>
        /// The tag's position
        /// </summary>
        public Vector3D Position { get; set; }
    }

    /// <summary>
    /// Opens a UDP connection on port 51515 and stores
    /// </summary>
    public class UDPListener
    {
        private static UDPListener instance;

        private static readonly int listenPort = 51515;
        private UdpClient listener = OpenConnection();
        private Queue<RadioData> dataBuffer = new Queue<RadioData>(10);
        private bool done;
        private Vector3D lastPoint = new Vector3D(0, 0, 0);

        private UDPListener() { }

        /// <summary>
        /// Returns an instance of UDPListener
        /// </summary>
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

            return client;
        }

        /// <summary>
        /// Listens for UDP messages. Adds any data to a data buffer
        /// </summary>
        public void Listen()
        {
            done = false;
            Task.Run(async () =>
            {
                while (!done)
                {
                    var receivedBytes = await listener.ReceiveAsync();
                    byte[] data = receivedBytes.Buffer;

                    int tagID;
                    double x, y, z;

                    // Read the UDP datagram
                    tagID = BitConverter.ToInt32(data, 0); 
                    x = BitConverter.ToDouble(data, 4);
                    y = BitConverter.ToDouble(data, 12);
                    z = BitConverter.ToDouble(data, 20);
                    
                    if (dataBuffer.Count == 10)
                        dataBuffer.Dequeue();
                    dataBuffer.Enqueue(new RadioData(tagID, new Vector3D(x, y, z)));
                }
            });
        }

        /// <summary>
        /// Gets the next point from the data buffer. If it is empty, get the last point returned instead.
        /// </summary>
        /// <returns>the next point from the data buffer</returns>
        public Vector3D NextPoint()
        {
            try { return lastPoint = dataBuffer.Dequeue().Position; }
            catch { return lastPoint; }
        }
        
        /// <summary>
        /// Makes the UDPListener stop listening
        /// </summary>
        public void Stop()
        {
            done = true;
        }

        /// <summary>
        /// Closes the UDP connection
        /// </summary>
        public void Close()
        {
            listener.Close();
        }
    }
}