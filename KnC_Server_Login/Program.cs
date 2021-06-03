using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace KnC_Server
{
    class Program
    {
        private static bool isRunning = false;

        static void Main(string[] args)
        {
            Console.Title = "KnC Login Server";
            Server.Start(10, 23500);
            Constants.SERVER_STATUS = Constants.SERVER_STATUS_MANTEINANCE;
            isRunning = true;

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();
        }

        private static void MainThread()
        {
            Console.WriteLine($"MainThread iniciado para el servidor a {Constants.TICKS_PER_SECOND} TICKS por segundo.");
            DateTime _nextLoop = DateTime.Now;

            while(isRunning)
            {
                while(_nextLoop < DateTime.Now)
                {
                    GameLogic.Update();
                    _nextLoop = _nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

                    if(_nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(_nextLoop - DateTime.Now);
                    }
                }
            }
        }
    }
}
