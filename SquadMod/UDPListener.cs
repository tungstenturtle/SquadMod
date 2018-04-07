using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Media.Media3D;
using System.Threading;
using System.Threading.Tasks;

struct RadioData
{

    public int tagID;
    public Vector3D position;
}

public class UDPListener
{
    int listenport;
    UdpClient listener;
    RadioData[] radioData;
    int bufferPtr;   //used to point to the next location to store data in circular buffer
    int currentData; //used as an index to get the next unread point
    bool done;      //flag used to stop the listen() method potentially because it runs seperately from the rest of the program
    
    public UDPListener(int port_num)
    {
        listenport = port_num;
        listener = new UdpClient(listenport);
        radioData = new RadioData[10];
        bufferPtr = 0;
        currentData = 0;
        done = false;
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
    private void Listen()
    {
        Task.Run(async () =>
        {
            using (listener)
            {
                while (!done)
                {
                    //IPEndPoint object will allow us to read datagrams sent from any source.
                    var receivedBytes = await listener.ReceiveAsync();
                    byte[] data = receivedBytes.Buffer;
                    radioData[bufferPtr].tagID = BitConverter.ToInt32(data, 0); //returns a 32-bit int from 4 bytes of byte[]
                    radioData[bufferPtr].position.X = BitConverter.ToDouble(data, 4);//starting 4 bytes in byte[], reads 8 bytes and converts to a double   
                    radioData[bufferPtr].position.Y = BitConverter.ToDouble(data, 12);
                    radioData[bufferPtr].position.Z = BitConverter.ToDouble(data, 20);
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
            data = radioData[currentData].position; //!Concern: What if no data has been stored yet?
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
   

}