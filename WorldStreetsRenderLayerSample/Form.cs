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
            winformsMap.CurrentExtent = new RectangleShape(-10801705.3653354, 3895575.29512613, -10750034.0634737, 3838706.28834338);

            LayerOverlay layerOverlay = new LayerOverlay();
            WorldStreetsRenderLayer worldStreetsRenderLayer = new WorldStreetsRenderLayer(sourceDataDirectory);
            layerOverlay.Layers.Add(worldStreetsRenderLayer);
            winformsMap.Overlays.Add(layerOverlay);
            winformsMap.Refresh();
        }
    }
}
