using System.Windows;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Wpf;
using System.Configuration;

namespace WorldStreetsRenderLayerSample
{
    public partial class MainWindow : Window
    {
        private string sourceDataDirectory = ConfigurationManager.AppSettings["SoureDataDirectory"];

        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            map.MapUnit = GeographyUnit.Meter;
            map.CurrentExtent = new RectangleShape(-13086298.60, 7339062.72, -8111177.75, 2853137.62);

            LayerOverlay layerOverlay = new LayerOverlay();
            WorldStreetsRenderLayer worldStreetsRenderLayer = new WorldStreetsRenderLayer(sourceDataDirectory);
            layerOverlay.Layers.Add(worldStreetsRenderLayer);
            map.Overlays.Add(layerOverlay);
        }
    }
}
