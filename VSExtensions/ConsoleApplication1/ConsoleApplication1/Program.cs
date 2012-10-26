using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {

        //static AutoResetEvent AEvent=new AutoResetEvent (true );
        //static AutoResetEvent BEvent = new AutoResetEvent(false);

        //static void PrintA()
        //{

        //    AEvent.WaitOne();
        //    Console.WriteLine("A");
        //    BEvent.Set();
        //}

        //static void PrintB()
        //{

        //    BEvent.WaitOne();
        //    Console.WriteLine("B");
        //    AEvent.Set();
        //}

        static object _Lock = new object();
        static void PrintA()
        {

            lock (_Lock)
            {
                Console.WriteLine("A");
            }
        }

        static void PrintB()
        {

            lock (_Lock)
            {
                Console.WriteLine("B");
            }
        }

        static void Main(string[] args)
        {

            for (var i = 0; i < 10; i++)
            {
                new Thread(() =>
                {
                    while (true)
                    {
                        PrintA();
                        PrintB();
                    }
                }).Start();
            }
            Console.Read();
        }
    }
}
