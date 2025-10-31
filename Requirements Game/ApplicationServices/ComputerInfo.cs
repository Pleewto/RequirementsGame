using System.Management;
using System;

/// <summary>
/// Provides basic computer system memory information
/// </summary>
class ComputerInfo {

    /// <summary>
    /// Returns the total amount of physical memory (RAM) in megabytes
    /// </summary>
    public static double GetTotalMemory() {

        // Query WMI for the total visible memory size

        var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem");

        // Convert and return the first result in MB

        foreach (var obj in searcher.Get()) {

            return Convert.ToDouble(obj["TotalVisibleMemorySize"]) / 1024;

        }

        // Return 0 if query fails or returns no result

        return 0;

    }

    /// <summary>
    /// Returns the currently available physical memory (free RAM) in megabytes
    /// </summary>
    public static double GetAvailableMemory() {

        // Query WMI for the amount of free physical memory

        var searcher = new ManagementObjectSearcher("SELECT FreePhysicalMemory FROM Win32_OperatingSystem");

        // Convert and return the first result in MB

        foreach (var obj in searcher.Get()) {

            return Convert.ToDouble(obj["FreePhysicalMemory"]) / 1024;

        }

        // Return 0 if query fails or returns no result

        return 0;

    }

}

