namespace MinesraftRenderer;

class Program
{
    static void Main(string[] args)
    {
        using Game game = new(1280, 720);
        game.Run();
    }
}
