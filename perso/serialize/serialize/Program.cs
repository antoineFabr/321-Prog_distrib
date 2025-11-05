using System.Text.Json;


Character gérard = new Character() { FirstName = "Gérard", LastName = "Depardieux", Description = "Un très gros monsieur"};

var dataj = JsonSerializer.Serialize(gérard);

System.IO.File.WriteAllText("gérard.json", dataj);

public class Character
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Description { get; set; }
    public Actor PlayedBy { get; set; }
}
public class Actor
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public string Country { get; set; }
    public bool IsAlive { get; set; }
}