using System.Numerics;

namespace Mappr.Games.Tarkov
{
    public class TarkovSettings : GameSettings
    {
        public List<Map> Maps { get; set; } = new List<Map>();

        public class Map
        {
            public string Name { get; set; } = "";
            public string Path { get; set; } = "";
            public int MinZoom { get; set; }
            public int MaxZoom { get; set; }
            public List<CalibrationPointx> CalibrationPoints { get; set; } = new List<CalibrationPointx>();

            public override string ToString() => Name;
        }

        public class CalibrationPointx
        {
            public Vector2 World { get; set; }
            public Vector2 Local { get; set; }

            public override string ToString()
            {
                return $"{World.ToString()}    {Local.ToString()}";
            }
        }
    }
}


