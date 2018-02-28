# Math library for decimal processing

```csharp
decimal dd = 10M;//sample parameter
Console.WriteLine("Math            = " + Math.Exp((double)dd));
Console.WriteLine("DecimalMath     = " + DecimalMath.Exp(dd));
Console.WriteLine("----------------------------------------");
Console.WriteLine("Math            = " + Math.Sin((double)dd));
Console.WriteLine("DecimalMath     = " + DecimalMath.Sin(dd));
dd = 0.1024M;//decreasing dd for asin
Console.WriteLine("----------------------------------------");
Console.WriteLine("Math            = " + Math.Asin((double)dd));
Console.WriteLine("DecimalMath     = " + DecimalMath.Asin(dd));
```
Above code gives the result :
```csharp
Math            = 22026.4657948067
DecimalMath     = 22026.465794806716516957900647
----------------------------------------
Math            = -0.54402111088937
DecimalMath     = -0.5440211108893698134047476618
----------------------------------------
Math            = 0.00976578022709082
DecimalMath     = 0.0097657802270908188917562020
```
Contains :<br/>
```csharp
PI
PIx2
PIdiv2
PIdiv4
E
Einv
LOG2
Zero
One
Exp
Power
PowerN
Log
Sin
Cos
Tan
Sqrt
Sinh
Cosh
Sign
Tanh
Asin
Acos
Atan
Atan2
```
