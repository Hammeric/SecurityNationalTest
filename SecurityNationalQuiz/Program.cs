using System;
using System.IO;
using System.Collections.Generic;

namespace SecurityNationalQuiz
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Project.");

            var project = new SalaryProject();
            project.Run();

            Console.WriteLine("Finished.");
            Console.ReadLine();
        }
    }
}
