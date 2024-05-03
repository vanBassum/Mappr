namespace Mappr
{
    public class GameSettings
    {
        public string Name { get; set; } = "";
        public string Engine { get; set; } = "";
        public string Process { get; set; } = "";


        public override string ToString() => Name;
    }
}


