namespace MinesraftRenderer;

class Program
{
    static void Main(string[] args)
    {
        using Game game = new(500, 500);
        game.Run();
    }
}
