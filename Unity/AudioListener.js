#pragma strict
 
import System.IO; // for FileStream
import System; // for BitConverter and Byte Type
 
 
 
private var bufferSize : int;
private var numBuffers : int;
private var outputRate : int = 44100;
private var fileName : String = "recTest.wav";
private var headerSize : int = 44; //default for uncompressed wav
 
private var recOutput : boolean;
 
private var fileStream : FileStream;
 
function Awake()
{
    AudioSettings.outputSampleRate = outputRate;
}
 
function Start()
{
    AudioSettings.GetDSPBufferSize(bufferSize,numBuffers);
}
 
function Update()
{
    if(Input.GetKeyDown("r"))
    {
    print("rec");
        if(recOutput == false)
        {
            StartWriting(fileName);
            recOutput = true;
        }
        else
        {
            recOutput = false;
            WriteHeader();     
            print("rec stop");
        }
    }  
}
 
function StartWriting(name : String)
{
    fileStream = new FileStream(name, FileMode.Create);
    var emptyByte : byte = new byte();
   
    for(var i : int = 0; i<headerSize; i++) //preparing the header
    {
        fileStream.WriteByte(emptyByte);
    }
}
 
function OnAudioFilterRead(data : float[], channels : int)
{
    if(recOutput)
    {
        ConvertAndWrite(data); //audio data is interlaced
    }
}
 
function ConvertAndWrite(dataSource : float[])
{
   
    var intData : Int16[] = new Int16[dataSource.length];
//converting in 2 steps : float[] to Int16[], //then Int16[] to Byte[]
   
    var bytesData : Byte[] = new Byte[dataSource.length*2];
//bytesData array is twice the size of
//dataSource array because a float converted in Int16 is 2 bytes.
   
    var rescaleFactor : int = 32767; //to convert float to Int16
   
    for (var i : int = 0; i<dataSource.length;i++)
    {
        intData[i] = dataSource[i]*rescaleFactor;
        var byteArr : Byte[] = new Byte[2];
        byteArr = BitConverter.GetBytes(intData[i]);
        byteArr.CopyTo(bytesData,i*2);
    }
   
    fileStream.Write(bytesData,0,bytesData.length);
}
 
function WriteHeader()
{
   
    fileStream.Seek(0,SeekOrigin.Begin);
   
    var riff : Byte[] = System.Text.Encoding.UTF8.GetBytes("RIFF");
    fileStream.Write(riff,0,4);
   
    var chunkSize : Byte[] = BitConverter.GetBytes(fileStream.Length-8);
    fileStream.Write(chunkSize,0,4);
   
    var wave : Byte[] = System.Text.Encoding.UTF8.GetBytes("WAVE");
    fileStream.Write(wave,0,4);
   
    var fmt : Byte[] = System.Text.Encoding.UTF8.GetBytes("fmt ");
    fileStream.Write(fmt,0,4);
   
    var subChunk1 : Byte[] = BitConverter.GetBytes(16);
    fileStream.Write(subChunk1,0,4);
   
    var two : UInt16 = 2;
    var one : UInt16 = 1;
   
    var audioFormat : Byte[] = BitConverter.GetBytes(one);
    fileStream.Write(audioFormat,0,2);
   
    var numChannels : Byte[] = BitConverter.GetBytes(two);
    fileStream.Write(numChannels,0,2);
   
    var sampleRate : Byte[] = BitConverter.GetBytes(outputRate);
    fileStream.Write(sampleRate,0,4);
   
    var byteRate : Byte[] = BitConverter.GetBytes(outputRate*4);
 // sampleRate * bytesPerSample*number of channels, here 44100*2*2
 
    fileStream.Write(byteRate,0,4);
   
    var four : UInt16 = 4;
    var blockAlign : Byte[] = BitConverter.GetBytes(four);
    fileStream.Write(blockAlign,0,2);
   
    var sixteen : UInt16 = 16;
    var bitsPerSample : Byte[] = BitConverter.GetBytes(sixteen);
    fileStream.Write(bitsPerSample,0,2);
   
    var dataString : Byte[] = System.Text.Encoding.UTF8.GetBytes("data");
    fileStream.Write(dataString,0,4);
   
    var subChunk2 : Byte[] = BitConverter.GetBytes(fileStream.Length-headerSize);
    fileStream.Write(subChunk2,0,4);
   
    fileStream.Close();
}
