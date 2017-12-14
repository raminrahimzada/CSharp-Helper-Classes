using System;
using System.Collections;
using System.Management;
using System.Collections.Generic;
using System.Printing;
using System.Reflection;

public enum PrinterStatus
{
    Other = 1,
    Unknown,
    Idle,
    Printing,
    Warmup,
    Stopped,
    Offline
}

public class Win32_Printer
{
    public string DriverName;
    public string Location;
    public string Name;
    public bool Network;
    public string PortName;
    public string ServerName;
    public bool Shared;
    public PrinterStatus Status;
    public bool WorkOffline;

    public static List<Win32_Printer> GetList()
    {
        var query = "Select * From Win32_Printer";

        var searcher = new ManagementObjectSearcher(query);

        var results = searcher.Get();

        var list = new List<Win32_Printer>(results.Count);

        foreach (ManagementObject obj in results)
        {
            var entry = new Win32_Printer();

            foreach (var field in typeof(Win32_Printer).GetFields())
                field.SetValue(entry, ConvertValue(obj[field.Name], field.FieldType));

            list.Add(entry);
        }
        return list;
    }

    private static object ConvertValue(object value, Type type)
    {
        if (value != null)
        {
            if (type == typeof(DateTime))
            {
                var time = value.ToString();
                time = time.Substring(0, time.IndexOf("."));
                return DateTime.ParseExact(time, "yyyyMMddHHmmss", null);
            }
            else if (type == typeof(long))
                return Convert.ToInt64(value);
            else if (type == typeof(int))
                return Convert.ToInt32(value);
            else if (type == typeof(short))
                return Convert.ToInt16(value);
            else if (type == typeof(string))
                return value.ToString();
            else if (type == typeof(PrinterStatus))
                return (PrinterStatus)Enum.Parse(typeof(PrinterStatus), value.ToString());
        }
        return null;
    }

    // ---------------------- GetPrintTicketFromPrinter ----------------------- 
    /// <summary> 
    ///   Returns a PrintTicket based on the current default printer.</summary> 
    /// <returns> 
    ///   A PrintTicket for the current local default printer.</returns> 
    public static   PrintTicket GetPrintTicketFromPrinter()
    {
        PrintQueue printQueue = null;

        var localPrintServer = new LocalPrintServer();

        // Retrieving collection of local printer on user machine
        var localPrinterCollection =
            localPrintServer.GetPrintQueues();

        IEnumerator localPrinterEnumerator =
            localPrinterCollection.GetEnumerator();

        if (localPrinterEnumerator.MoveNext())
        {
            // Get PrintQueue from first available printer
            printQueue = (PrintQueue)localPrinterEnumerator.Current;
        }
        else
        {
            // No printer exist, return null PrintTicket 
            return null;
        }

        // Get default PrintTicket from printer
        var printTicket = printQueue.DefaultPrintTicket;

        var printCapabilites = printQueue.GetPrintCapabilities();

        // Modify PrintTicket 
        if (printCapabilites.CollationCapability.Contains(Collation.Collated))
        {
            printTicket.Collation = Collation.Collated;
        }

        if (printCapabilites.DuplexingCapability.Contains(
                Duplexing.TwoSidedLongEdge))
        {
            printTicket.Duplexing = Duplexing.TwoSidedLongEdge;
        }

        if (printCapabilites.StaplingCapability.Contains(Stapling.StapleDualLeft))
        {
            printTicket.Stapling = Stapling.StapleDualLeft;
        }

        return printTicket;
    }// 

}
