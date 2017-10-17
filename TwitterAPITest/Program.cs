using System;
using System.Threading.Tasks;

namespace TwitterAPITest
{
    class Program
    {
        static void Main(string[] args)
        {
            Tweet t = new Tweet();


            t.GetTweetsAsync("datascience");
            Console.ReadKey();
            
        }
    }
}