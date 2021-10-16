namespace BackupsServer
{
    class Program
    {
        public static void Main()
        {
            var server = new Server("127.0.0.1", 8888, @"d:\ServerStuff");
            while (true)
            {
                server.Work();
            }
        }
    }
}