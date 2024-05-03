using Mappr.Controls;
using Mappr.Entities;
using Mappr.Games.Tarkov;
using System.Buffers;
using System.Diagnostics;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Text.Json.Serialization;
using YamlDotNet.Core.Events;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.Utilities;
using System.Globalization;
using Mappr.Kernel;

namespace Mappr
{
    public partial class Form1 : Form
    {
        MapView mapView = new MapView();
        List<GameSettings> Settings = new List<GameSettings>();
        IMapManager? manager;
        System.Windows.Forms.Timer gameManagerTimer;

        public Form1()
        {
            InitializeComponent();

            this.Controls.Add(this.mapView);
            mapView.Dock = DockStyle.Fill;
            mapView.BorderStyle = BorderStyle.FixedSingle;

            GameSettingsLoader loader = new GameSettingsLoader();
            Directory.CreateDirectory("config");
            foreach(var file in Directory.GetFiles("config"))
            {
                var settings = loader.LoadFile(file);
                if(settings != null)
                    Settings.Add(settings);
            }

            TryLoadGameManager();
            gameManagerTimer = new ();
            gameManagerTimer.Interval = 5000;
            gameManagerTimer.Tick += (sender, e) => TryLoadGameManager();
            gameManagerTimer.Start();
        }

        void TryLoadGameManager()
        {
            if(manager == null)
            {
                var settings = Settings.FirstOrDefault(s => MemoryManager.IsProcessRunning(s.Process));   
                if(settings != null)
                    manager = CreateManager(settings);
            }
            else
            {
                if(!manager.IsAttached())
                {
                    //Delete manager
                    manager.Dispose();
                    manager = null;
                }
            }
        }


        IMapManager? CreateManager(GameSettings settings)
        {
            switch (settings.Engine, settings)
            {
                case ("Tarkov", TarkovSettings tarkovSettings):
                    return new TarkovManager(tarkovSettings, mapView);
            }

            return null;
        }
    }



    public interface IMapManager : IDisposable
    {
        bool IsAttached();
    }


    public class GameSettingsLoader
    {
        private readonly IDeserializer deserializer;
        private readonly ISerializer serializer;

        public GameSettingsLoader()
        {
            deserializer = new DeserializerBuilder()
                .WithTypeConverter(new Vector2YamlConverter())
                .IgnoreUnmatchedProperties()
                .Build();

            serializer = new SerializerBuilder()
                .WithTypeConverter(new Vector2YamlConverter())
                .Build();
        }

        public GameSettings? LoadFile(string file)
        {
            GameSettings baseSettings = GetSettings<GameSettings>(file);
            switch (baseSettings.Engine)
            {
                case "Tarkov":
                    return GetSettings<TarkovSettings>(file);
                default:
                    Console.WriteLine("Unsupported engine type.");
                    break;
            }
            return null;
        }

        public void SaveToFile(GameSettings settings, string file)
        {
            using var writer = new StreamWriter(file);
            serializer.Serialize(writer, settings);
        }

        T GetSettings<T>(string file)
        {
            using var reader = new StreamReader(file);
            return deserializer.Deserialize<T>(reader);
        }
    }


    public class Vector2YamlConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type) => type == typeof(Vector2);

        public object? ReadYaml(IParser parser, Type type)
        {
            if (parser.TryConsume<SequenceStart>(out var start))
            {
                float x, y;
                if (!float.TryParse(parser.Consume<Scalar>().Value, CultureInfo.InvariantCulture, out x))
                    throw new InvalidOperationException("Expected a YAML float");

                if (!float.TryParse(parser.Consume<Scalar>().Value, CultureInfo.InvariantCulture, out y))
                    throw new InvalidOperationException("Expected a YAML float");
                parser.MoveNext();
                return new Vector2(x, y);
            }

            throw new InvalidOperationException("Expected a YAML object or array");
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type)
        {
            if (value is Vector2 vector)
            {
                emitter.Emit(new SequenceStart(null, null, false, SequenceStyle.Flow));
                emitter.Emit(new Scalar(vector.X.ToString(CultureInfo.InvariantCulture)));
                emitter.Emit(new Scalar(vector.Y.ToString(CultureInfo.InvariantCulture)));
                emitter.Emit(new SequenceEnd());
            }
            else
                throw new InvalidOperationException($"Expected a type of {typeof(Vector2).Name}");
        }
    }




}


