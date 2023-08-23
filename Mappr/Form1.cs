using Mappr.Controls;

namespace Mappr
{
    public partial class Form1 : Form
    {
        MapView mapView = new MapView();
        public Form1()
        {
            InitializeComponent();
            this.Controls.Add(this.mapView);
            mapView.Location = new System.Drawing.Point(50, 50);
            mapView.Size = new System.Drawing.Size(512, 512);
            mapView.BorderStyle = BorderStyle.FixedSingle;
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