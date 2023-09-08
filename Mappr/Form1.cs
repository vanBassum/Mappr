using Mappr.Controls;
using Mappr.Entities;
using Mappr.Tiles;
using System.Diagnostics;
using System.Net;
using System.Numerics;

namespace Mappr
{
    public partial class Form1 : Form
    {
        MapView mapView = new MapView();
        MapEntitySource entitySource = new MapEntitySource();
        PlayerEntity playerEntity = new PlayerEntity(new Vector2(75, 75));
        MapMouseHandler mapMouseHandler;
        public Form1()
        {
            InitializeComponent();

            this.Controls.Add(this.mapView);
            mapView.Dock = DockStyle.Fill;
            mapView.BorderStyle = BorderStyle.FixedSingle;

            FileTileSource fileSource = new FileTileSource("maps/gta5");
            ScalerTileSource scaler = new ScalerTileSource(fileSource);
            CachingTileSource cashing = new CachingTileSource(scaler, (1920*1080) * 5 / (128*128));
            mapView.TileSource = cashing;
            mapView.MapEntitySource = entitySource;
            mapMouseHandler = new MapMouseHandler(mapView, entitySource);

            entitySource.Add(new MapEntity { MapPosition = new Vector2(100, 100) });
            entitySource.Add(new MapEntity { MapPosition = new Vector2(50, 50) });
            entitySource.Add(playerEntity);
        }
    }


    

}
