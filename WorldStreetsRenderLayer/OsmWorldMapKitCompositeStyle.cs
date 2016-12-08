using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace ThinkGeo.MapSuite
{
    class OsmWorldMapKitCompositeStyle : CompositeStyle
    {
        protected override void DrawCore(IEnumerable<Feature> features, GeoCanvas canvas, Collection<SimpleCandidate> labelsInThisLayer, Collection<SimpleCandidate> labelsInAllLayers)
        {
            Collection<Feature> tempFeatures = new Collection<Feature>();

            foreach (var feature in features)
            {
                tempFeatures.Add(feature);
                base.DrawCore(tempFeatures, canvas, labelsInThisLayer, labelsInAllLayers);
                tempFeatures.Remove(feature);
            }
        }
    }
}
