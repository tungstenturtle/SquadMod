using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Media.Media3D;
using System.Threading.Tasks;

namespace SquadMod
{
    struct RadioData
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

        private int listenPort;
        private UdpClient listener = new UdpClient(45454);
        private RadioData[] radioData = new RadioData[10];
        private int bufferPtr;   //used to point to the next location to store data in circular buffer
        private int currentData; //used as an index to get the next unread point
        private bool done;      //flag used to stop the listen() method potentially because it runs seperately from the rest of the program

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

        /*
            private void Listen() //Listen for a datagram !issue is that ReceiveAsync only gets one
            {
                received_byte_array = listener.ReceiveAsync();
                //process byte array, assumes in the form of int, double, double, double
                radioData[bufferPtr].tagID = BitConverter.ToInt32(receivedBytes, 0); //returns a 32-bit int from 4 bytes of byte[]
                radioData[bufferPtr].position.x = BitConverter.ToDouble(receivedBytes, 4);//starting 4 bytes in byte[], reads 8 bytes and converts to a double   
                radioData[bufferPtr].position.y = BitConverter.ToDouble(receivedBytes, 12);
                radioData[bufferPtr].position.z = BitConverter.ToDouble(receivedBytes, 20);
            }
        */

        public void Listen()
        {
            done = false;

            Task.Run(async () =>
            {
                using (listener)
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

                        radioData[bufferPtr] = new RadioData(tagID, new Vector3D(x, y, z));

                        bufferPtr = (bufferPtr + 1) % 10;
                    }
                }
            });
        }

        public Vector3D NextPoint()
        {
            Vector3D data;
            try
            {
                data = radioData[currentData].Position; //!Concern: What if no data has been stored yet?
            }
            catch
            {
                data = new Vector3D(0, 0, 0);
            }
            if (currentData == bufferPtr) //to prevent the reading pointer from passing the writing pointer and to keep from reading old/null data
            {
                /* logic that tells application that no new data is available*/
            }
            else
            {
                currentData = (currentData + 1) % 10;
            }
            return data;
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