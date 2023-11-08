

namespace Pong
{
    class Program
    {
     
    
        static void Main()
        {
            using (Pong game = new Pong(800, 600, "LearnOpenTK"))
            {
                game.Run();
            }

        }
    }
}