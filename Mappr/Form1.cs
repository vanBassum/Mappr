using Mappr.Controls;

namespace Mappr
{
    public partial class Form1 : Form
    {
        MapView mapView = new MapView();
        FileTileSource mapTileSource;
        public Form1()
        {
            InitializeComponent();
            this.Controls.Add(this.mapView);
            mapView.Location = new System.Drawing.Point(20, 20);
            mapView.Size = new System.Drawing.Size(1024, 1024);
            mapView.BorderStyle = BorderStyle.FixedSingle;
            mapTileSource = new FileTileSource("maps");
            mapView.TileSource = mapTileSource;
        }

        private void mapView1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mapView.Redraw();
        }
    }
}