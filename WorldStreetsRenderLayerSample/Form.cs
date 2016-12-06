using System;
using System.Configuration;
using System.Windows.Forms;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.WinForms;

namespace WorldStreetsRenderLayerSample
{
    public partial class Form : System.Windows.Forms.Form
    {
        private string sourceDataDirectory = ConfigurationManager.AppSettings["SourceDataDirectory"];

        public Form()
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
