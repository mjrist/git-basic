using System;
using System.Diagnostics;
using System.Management;

namespace GitBasic
{
    public static class ProcessExtensions
    {
        /// <summary>
        /// Immediately stops the associated process and all of its descendants.       
        /// </summary>
        /// <param name="process">The root process.</param>
        public static void KillProcessTree(this Process process)
        {
            KillProcessTree(process.Id);
        }

        private static void KillProcessTree(int pid)
        {
            // pid 0 is "system idle process" which cannot be killed.

            if (pid == 0)
            {
                return;
            }

            string query = $"Select * From Win32_Process Where ParentProcessID={pid}";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection managementObjectCollection = searcher.Get();
            foreach (ManagementObject managementObject in managementObjectCollection)
            {
                KillProcessTree(Convert.ToInt32(managementObject["ProcessID"]));
            }

            try
            {
                Process process = Process.GetProcessById(pid);
                process.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
        }
    }
}
