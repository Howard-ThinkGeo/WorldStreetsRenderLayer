// <%STEMCELL:PerDirectoryNoticeText%>
// ALL RIGHTS RESERVED:
// ====================
// The contents of this file, and associated files in this directory, are
// Copyright (C) ThinkGeo,LLC , all rights reserved, 2003-2013.
//
// All software Source Code, Images, Database-Design and code, Graphics Design
// and source files, and related content (collectively referred to as SOURCE)
// are Copyright (c) ThinkGeo,LLC, 2003-2013, All Rights Reserved.
// ThinkGeo,LLC is a USA corporation at 2801 Network Blvd. Suite 215, Frisco,TX 75034
// http://ThinkGeo.com
//
// MapSuite is a Registered Trademark of ThinkGeo,LLC.
//
//</%STEMCELL:PerDirectoryNoticeText%>

namespace ThinkGeo.MapSuite.Layers
{
    public class WorldStreetsShapeFileFeatureLayer : ShapeFileFeatureLayer
    {
        public WorldStreetsShapeFileFeatureLayer()
        {
            ShareCanvasWithPreviousLayer = false;
        }

        public WorldStreetsShapeFileFeatureLayer(string shapePathFilename)
        {
            FeatureSource = new WorldStreetsShapeFileFeatureSource(shapePathFilename);
            ShareCanvasWithPreviousLayer = false;
        }

        public WorldStreetsShapeFileFeatureLayer(string shapePathFilename, GeoFileReadWriteMode readWriteMode)
        {
            FeatureSource = new WorldStreetsShapeFileFeatureSource(shapePathFilename, readWriteMode);
            ShareCanvasWithPreviousLayer = false;
        }

        public bool ShareCanvasWithPreviousLayer
        {
            get;
            set;
        }
    }
}
