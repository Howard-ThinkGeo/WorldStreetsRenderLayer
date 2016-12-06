using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Shapes;
using System.Configuration;
using ThinkGeo.MapSuite.WinForms;
using ThinkGeo.MapSuite.Layers;

namespace WorldStreetsRenderLayerSample
{
    public partial class Form1 : Form
    {
        private string sourceDataDirectory = ConfigurationManager.AppSettings["SourceDataDirectory"];

        public Form1()
        {
            InitializeComponent();
        }

        private void FormLoad(object sender, EventArgs e)
        {
            winformsMap.MapUnit = GeographyUnit.Meter;
            winformsMap.CurrentExtent = new RectangleShape(-13086298.60, 7339062.72, -8111177.75, 2853137.62);

            LayerOverlay layerOverlay = new LayerOverlay();
            WorldStreetsRenderLayer worldStreetsRenderLayer = new WorldStreetsRenderLayer(sourceDataDirectory);
            layerOverlay.Layers.Add(worldStreetsRenderLayer);
            winformsMap.Overlays.Add(layerOverlay);
            winformsMap.Refresh();
        }
    }
}
