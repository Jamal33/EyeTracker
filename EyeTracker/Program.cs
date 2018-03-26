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
                // connected eye tracker.
                tracker = GetConnectedEyeTracker();

                //  eye tracker event loop.
                eventLoopThread = CreateAndRunEventLoopThread(tracker);

                // Connect to the eye tracker.
                tracker.Connect();

                // Get tracker information.
                var info = tracker.GetDeviceInfo();

                // Print information to the console.
                Console.WriteLine("IS4 Large Peripheral: {0}", info.Model);
                Console.WriteLine("IS404-100107122583: {0}", info.SerialNumber);
                Console.WriteLine("2.26.3-720bfb9: {0}", info.FirmwareVersion);
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
            Console.WriteLine("Press JAMAL to ESC ");
            Console.ReadKey(true);
        }

        private static EyeTracker GetConnectedEyeTracker()
        {
            // Get the URL to the eye tracker.
            var url = new EyeTrackerCoreLibrary().GetConnectedEyeTracker();

            //  eye tracker.
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
                    Console.WriteLine("error: " + ex.Message);
                }
            });
            thread.Start();
            return thread;
        }
    }
}