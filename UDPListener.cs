using System;
using System.Net;
using System.Net.Sockets;
using System.BitConverter;
using System.Windows.Media.Media3D.Vector3D;

struct RadioData
{
    int tagID;
    Vector3D position;
}

public class UDPListener
{
    int listenport;
    byte[] receivedBytes;
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
    private static void Listen()
    {
        Task.Run(async () =>
        {
            using (listener)
            {
                while (!done)
                {
                    //IPEndPoint object will allow us to read datagrams sent from any source.
                    receivedBytes = await listener.ReceiveAsync();
                    radioData[bufferPtr].tagID = BitConverter.ToInt32(receivedBytes, 0); //returns a 32-bit int from 4 bytes of byte[]
                    radioData[bufferPtr].position.x = BitConverter.ToDouble(receivedBytes, 4);//starting 4 bytes in byte[], reads 8 bytes and converts to a double   
                    radioData[bufferPtr].position.y = BitConverter.ToDouble(receivedBytes, 12);
                    radioData[bufferPtr].position.z = BitConverter.ToDouble(receivedBytes, 20);
                    if (bufferPtr == 9)
                        bufferPtr = 0;
                    else
                        bufferPtr = bufferPtr = bufferPtr + 1;
                }
            }
        });
    }

    public Vector3D NextPoint()
    {
        Vector3D data = radioData[currentData].position; //!Concern: What if no data has been stored yet?
        if (currentData == bufferPtr) //to prevent the reading pointer from passing the writing pointer and to keep from reading old/null data
        {
            /* logic that tells application that no new data is available*/
        }
        else
        {
            if (currentData == 9) //if at end of buffer
                currentData = 0;
            else
                currentData = currentData + 1;
        }
        return data;
    }
        
    public void Stop() //kills the listen method by setting the flag to true
    {
        done = true;
    }
   

}