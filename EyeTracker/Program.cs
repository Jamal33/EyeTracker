using System;
using System.Threading;
using Tobii.Gaze.Core;

namespace EyeTrackerInfo
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            Thread eventLoopThread = null;
            EyeTracker tracker = null;

            try
            {
                // Get the connected eye tracker.
                tracker = GetConnectedEyeTracker();

                // Create and run the eye tracker event loop.
                eventLoopThread = CreateAndRunEventLoopThread(tracker);

                // Connect to the eye tracker.
                tracker.Connect();

                // Get tracker information.
                var info = tracker.GetDeviceInfo();

                // Print information to the console.
                Console.WriteLine("Eye tracker model: {0}", info.Model);
                Console.WriteLine("Eye tracker serial number: {0}", info.SerialNumber);
                Console.WriteLine("Firmware version: {0}", info.FirmwareVersion);
            }
            finally
            {
                if (eventLoopThread != null)
                {
                    tracker.BreakEventLoop();
                    eventLoopThread.Join();
                }

                if (tracker != null)
                {
                    tracker.Dispose();
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press ANY key to quit");
            Console.ReadKey(true);
        }

        private static EyeTracker GetConnectedEyeTracker()
        {
            // Get the URL to the eye tracker.
            var url = new EyeTrackerCoreLibrary().GetConnectedEyeTracker();

            // Create a new eye tracker using the URL.
            return new EyeTracker(url);
        }

        private static Thread CreateAndRunEventLoopThread(IEyeTracker tracker)
        {
            var thread = new Thread(() =>
            {
                try
                {
                    tracker.RunEventLoop();
                }
                catch (EyeTrackerException ex)
                {
                    Console.WriteLine("An error occurred in the eye tracker event loop: " + ex.Message);
                }
            });
            thread.Start();
            return thread;
        }
    }
}