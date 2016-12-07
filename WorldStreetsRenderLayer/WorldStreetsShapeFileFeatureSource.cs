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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using ThinkGeo.MapSuite.Shapes;

namespace ThinkGeo.MapSuite.Layers
{
    public class WorldStreetsShapeFileFeatureSource : ShapeFileFeatureSource
    {
        private bool isAlreadyOpened;

        public WorldStreetsShapeFileFeatureSource()
        {
        }

        public WorldStreetsShapeFileFeatureSource(string shapePathFilename)
            : base(shapePathFilename)
        { }


        public WorldStreetsShapeFileFeatureSource(string shapePathFilename, GeoFileReadWriteMode readWriteMode)
            : base(shapePathFilename, readWriteMode)
        { }

        protected override void OpenCore()
        {
        }

        protected override void CloseCore()
        {
            if (isAlreadyOpened)
            {
                isAlreadyOpened = false;
                base.CloseCore();
            }
        }

        protected override Collection<Feature> GetFeaturesForDrawingCore(RectangleShape boundingBox, double screenWidth, double screenHeight, IEnumerable<string> returningColumnNames)
        {
            PrivateOpen();
            return base.GetFeaturesForDrawingCore(boundingBox, screenWidth, screenHeight, returningColumnNames);
        }

        protected override Collection<FeatureSourceColumn> GetColumnsCore()
        {
            PrivateOpen();
            return base.GetColumnsCore();
        }

        private void PrivateOpen()
        {
            if (!isAlreadyOpened)
            {
                isAlreadyOpened = true;
                base.OpenCore();
            }
        }
    }
}
