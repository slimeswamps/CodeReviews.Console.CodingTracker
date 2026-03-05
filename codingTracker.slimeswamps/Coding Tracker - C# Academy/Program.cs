namespace CodingTracker;

class Program
{
    public static void Main(string[] args)
    {
        UserInterface ui = new UserInterface();
        DatabaseConnector db = new DatabaseConnector();

        db.CreateTable();
        ui.MainMenu();
    }
}