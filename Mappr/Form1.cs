using Mappr.Controls;
using Mappr.Entities;
using Mappr.Games.Tarkov;
using System.Buffers;
using System.Diagnostics;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mappr
{
    public partial class Form1 : Form
    {
        MapView mapView = new MapView();
        TarkovManager tarkovManager;

        public Form1()
        {
            InitializeComponent();

            this.Controls.Add(this.mapView);
            mapView.Dock = DockStyle.Fill;
            mapView.BorderStyle = BorderStyle.FixedSingle;

            tarkovManager = new TarkovManager(mapView);

            tarkovManager.LoadMap("maps/tarkov/woods");
        }

        
    }



    public class CalibrationPoint : MapEntity
    {
        private bool hover = false;
        public Vector2 CalculatedMapPosition { get; set; }
        public Vector2 ClickedWorldPosition { get; set; }
        public Vector2 ClickedMapPosition { get; set; }


        public override void Draw(Graphics g, CoordinateScaler2D scaler, Vector2 screenSize)
        {
            var screenPos = scaler.ApplyTransformation(CalculatedMapPosition);
            bool isObjectOnScreen = screenPos.X >= 0 && screenPos.Y >= 0 && screenPos.X < screenSize.X && screenPos.Y < screenSize.Y;
            if (isObjectOnScreen)
            {
                var clickedScreenPos = scaler.ApplyTransformation(ClickedMapPosition);

                if (hover)
                {
                    DrawCross(g, Pens.Green, screenPos);
                    DrawCross(g, Pens.Green, clickedScreenPos);
                }
                else
                {
                    DrawCross(g, Pens.Red, screenPos);
                    DrawCross(g, Pens.Blue, clickedScreenPos);
                }
            }
        }

        public override void HandleMouseMove(object sender, MapMouseEventArgs e)
        {
            var screenPos = e.MapToScreenScaler.ApplyTransformation(CalculatedMapPosition);
            float distance = Vector2.Distance(e.MouseScreenPosition, screenPos);
            hover = distance < 10f;
            e.RequestRedraw = true;
            e.IsActive = hover;
        }


        void DrawCross(Graphics g, Pen pen, Vector2 screenPos, int crossSize = 10)
        {
            // Calculate the starting and ending points for the cross lines
            Point startPointHorizontal = new Point((int)screenPos.X - crossSize, (int)screenPos.Y);
            Point endPointHorizontal = new Point((int)screenPos.X + crossSize, (int)screenPos.Y);
            Point startPointVertical = new Point((int)screenPos.X, (int)screenPos.Y - crossSize);
            Point endPointVertical = new Point((int)screenPos.X, (int)screenPos.Y + crossSize);

            // Draw the horizontal and vertical lines to create the cross
            g.DrawLine(pen, startPointHorizontal, endPointHorizontal);
            g.DrawLine(pen, startPointVertical, endPointVertical);
        }
    }
    public class Calibrator
    {
        List<CalibrationPoint> calibrationPoints = new List<CalibrationPoint>();
        private readonly MapView mapView;
        public CoordinateScaler2D? WorldToMapScaler { get; private set; }

        public Calibrator(MapView mapView)
        {
            this.mapView = mapView;

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            };

            options.Converters.Add(new Vector2JsonConverter());
            calibrationPoints = JsonSerializer.Deserialize<List<CalibrationPoint>>(File.ReadAllText("woods.json"), options);
            mapView.Entities?.AddRange(calibrationPoints);
            if (calibrationPoints.Count >= 2)
            {
                var worldPositions = calibrationPoints.Select(c => c.ClickedWorldPosition).ToArray();
                var mapPositions = calibrationPoints.Select(c => c.ClickedMapPosition).ToArray();
                WorldToMapScaler = CoordinateRegression.Fit(worldPositions, mapPositions);
                foreach (var c in calibrationPoints)
                    c.CalculatedMapPosition = WorldToMapScaler.ApplyTransformation(c.ClickedWorldPosition);
            }
        }

        public void AddPoint(Vector2 worldPos, Vector2 mapPos)
        {
            CalibrationPoint calibrationPoint = new CalibrationPoint()
            {
                ClickedMapPosition = mapPos,
                ClickedWorldPosition = worldPos,
                CalculatedMapPosition = mapPos
            };
            calibrationPoints.Add(calibrationPoint);
            mapView.Entities?.Add(calibrationPoint);

            Debug.WriteLine($" world = {worldPos}   map = {mapPos}");

            if (calibrationPoints.Count >= 2)
            {
                var worldPositions = calibrationPoints.Select(c => c.ClickedWorldPosition).ToArray();
                var mapPositions = calibrationPoints.Select(c => c.ClickedMapPosition).ToArray();
                WorldToMapScaler = CoordinateRegression.Fit(worldPositions, mapPositions);
                foreach (var c in calibrationPoints)
                    c.CalculatedMapPosition = WorldToMapScaler.ApplyTransformation(c.ClickedWorldPosition);
            }

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                WriteIndented = true,
            };

            options.Converters.Add(new Vector2JsonConverter());

            File.WriteAllText("woods.json", JsonSerializer.Serialize(calibrationPoints, options));

        }
    }
    public class Vector2JsonConverter : JsonConverter<Vector2>
    {
        public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.Read();
            reader.Read();
            var x = reader.GetSingle();
            reader.Read();
            reader.Read();
            var y = reader.GetSingle();
            reader.Read();

            return new Vector2(x, y);
        }

        public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("X", value.X);
            writer.WriteNumber("Y", value.Y);
            writer.WriteEndObject();
        }
    }

}


