using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rextester {
 public class Program {
  public static void Main(string[] args) {
   double realCoord, imagCoord;
   double realTemp, imagTemp, realTemp2, arg;
   int iterations;
   for (imagCoord = 1.2; imagCoord >= -1.2; imagCoord -= 0.05) {
    for (realCoord = -0.6; realCoord <= 1.77; realCoord += 0.03) {
     iterations = 0;
     realTemp = realCoord;
     imagTemp = imagCoord;
     arg = (realCoord * realCoord) + (imagCoord * imagCoord);
     while ((arg < 4) && (iterations < 40)) {
      realTemp2 = (realTemp * realTemp) - (imagTemp * imagTemp) - realCoord;
      imagTemp = (2 * realTemp * imagTemp) - imagCoord;
      realTemp = realTemp2;
      arg = (realTemp * realTemp) + (imagTemp * imagTemp);
      iterations += 1;
     }
     switch (iterations % 4) {
      case 0:
       Console.Write(".");
       break;
      case 1:
       Console.Write("o");
       break;
      case 2:
       Console.Write("O");
       break;
      case 3:
       Console.Write("@");
       break;
     }
    }
    Console.Write("\n");
   }
   Console.ReadKey();
  }
 }
}
