using Mappr.Controls;
using Mappr.Entities;
using Mappr.MapInteractions;
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
        ContextMenuManager<MapMouseEventArgs> menuManager;
        public Form1()
        {
            InitializeComponent();

            this.Controls.Add(this.mapView);
            mapView.Dock = DockStyle.Fill;
            mapView.BorderStyle = BorderStyle.FixedSingle;

            menuManager = new ContextMenuManager<MapMouseEventArgs>(mapView);
            menuManager.AddMenuItem("Add point", e => {
                entitySource.Add(new MapEntity { MapPosition = e.MouseMapPosition });
                mapView.Redraw();   
            });

            FileTileSource fileSource = new FileTileSource("maps/gta5");
            ScalerTileSource scaler = new ScalerTileSource(fileSource);
            CachingTileSource cashing = new CachingTileSource(scaler, (1920*1080) * 5 / (128*128));
            mapView.TileSource = cashing;
            mapView.MapEntitySource = entitySource;
            mapView.ConfigInteractions(c=>c
                .AddInteraction(new EntityDragging(entitySource))
                .AddInteraction(new EntityHover(entitySource))
                .AddInteraction(new Panning())
                .AddInteraction(new Zooming(a=>a.WithMinZoom(1f).WithMaxZoom(128f).WithZoomFactor(2f)))
                .AddInteraction(new ShowContextMenu(menuManager))
                );

            entitySource.Add(new MapEntity { MapPosition = new Vector2(100, 100) });
            entitySource.Add(new MapEntity { MapPosition = new Vector2(50, 50) });
            entitySource.Add(playerEntity);
        }
    }


    

}
