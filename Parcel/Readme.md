<strong>C# Alternative of  Android Parcel classes  </strong><br/>
You can write write your own objects to Parcel and read from Parcel<br/>
```csharp
Parcel works with stream 
var stream = File.Open("data.dat", FileMode.Create); 
var parcel = new Parcel(stream); 
//writing object to parcel 
parcel.Write(m); 
//flushing changes 
parcel.Flush(); 
//Reset Seek Position To Zero To Read All Data in Parcel 
parcel.ResetToOrigin();           
//reading full object from Parcel 
var mm = parcel.Read<TestClassTwo>();
```
For more see test classes<br/>
