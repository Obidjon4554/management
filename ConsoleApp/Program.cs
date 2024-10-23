using ConsoleApp.Handlers;
using System;

namespace ConsoleApp
{
    public static class Program
    {
        static void Main(string[] args)
        {
            string input = Console.ReadLine();

            int num = int.Parse(input);

            IHandler oneHun = new OneHundredHandler();
            IHandler fifty = new FiftyHandler();
            IHandler twnety = new TwentyHandler();
            IHandler ten = new TenHandler();
            IHandler five = new FiveHandler();
            IHandler one = new OneHandler();

            oneHun.MoveNext(fifty);
            fifty.MoveNext(twnety);
            twnety.MoveNext(ten);
            ten.MoveNext(five);
            five.MoveNext(one);

            string res = oneHun.Handle(num);

            Console.WriteLine(res);
        }
    }

}
