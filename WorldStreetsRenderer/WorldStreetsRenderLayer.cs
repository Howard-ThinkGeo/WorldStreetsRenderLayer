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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace ThinkGeo.MapSuite.Layers
{
    public class WorldStreetsRenderLayer : Layer
    {
        private static SplineType splineType = SplineType.StandardSplining;

        public delegate void RenderingCustomLayerDelegate(FeatureLayer layer);
        public RenderingCustomLayerDelegate RenderingCustomLayer;

        private string dataFolder;
        private bool showPOI;
        private Dictionary<string, string> globalFiles;
        private Collection<WorldStreetsShapeFileFeatureLayer> staticLayers;
        private Collection<WorldStreetsShapeFileFeatureLayer> dynamicLayers;
        private RtreeSpatialIndex rtreeSpatialIndex;

        public WorldStreetsRenderLayer(string dataFolder)
        {
            this.dataFolder = dataFolder;
            staticLayers = new Collection<WorldStreetsShapeFileFeatureLayer>();
            dynamicLayers = new Collection<WorldStreetsShapeFileFeatureLayer>();

            globalFiles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            globalFiles.Add("cover", "                  " + @"/World/physical/cover.shp");
            globalFiles.Add("ocean", "                  " + @"/World/physical/ocean.shp");
            globalFiles.Add("evergreen", "              " + @"/World/physical/evergreen.shp");
            globalFiles.Add("sand", "                   " + @"/World/physical/sand.shp");
            globalFiles.Add("innerwater", "             " + @"/World/physical/innerwater.shp");
            globalFiles.Add("innerwaterMinUSA", "       " + @"/World/physical/innerwaterMinUSA.shp");
            globalFiles.Add("lawrenceriver", "          " + @"/World/Physical/lawrenceriver.shp");
            globalFiles.Add("permanentriver", "         " + @"/World/Physical/permanentriver.shp");
            globalFiles.Add("permanentriverMinusUSA", " " + @"/World/Physical/permanentriverMinusUSA.shp");
            globalFiles.Add("oceantext", "              " + @"/World/physical/oceantext.shp");
            globalFiles.Add("lake", "                   " + @"/World/physical/lake.shp");
            globalFiles.Add("depthline", "              " + @"/World/physical/depthline.shp");
            globalFiles.Add("cntry02", "                " + @"/World/human/cntry02.shp");
            globalFiles.Add("urbanareaMinUSA", "        " + @"/World/human/urbanareaMinUSA.shp");
            globalFiles.Add("primaryborder", "          " + @"/World/human/primaryborder.shp");
            globalFiles.Add("internationalborder", "    " + @"/World/human/internationalborder.shp");
            globalFiles.Add("internationalborder_dtl", "" + @"/World/human/internationalborder_dtl.shp");
            globalFiles.Add("contestedborder", "        " + @"/World/human/contestedborder.shp");
            globalFiles.Add("railMinusUSA", "           " + @"/World/human/railMinusUSA.shp");
            globalFiles.Add("road", "                   " + @"/World/human/road.shp");
            globalFiles.Add("roadMinUSACan", "          " + @"/World/human/roadMinUSACan.shp");
            globalFiles.Add("airport", "                " + @"/World/human/airport.shp");
            globalFiles.Add("capital", "                " + @"/World/human/capital.shp");
            globalFiles.Add("noncapital", "             " + @"/World/human/noncapital.shp");
            globalFiles.Add("settlement", "             " + @"/World/human/settlement.shp");

            globalFiles.Add("urban_dtl", "              " + @"/USA/urban_dtl.shp");
            globalFiles.Add("fedlandnps", "             " + @"/USA/fedlandnps.shp");
            globalFiles.Add("fedlandnpsclip", "         " + @"/USA/fedlandnpsclip.shp");
            globalFiles.Add("fedlandfs", "              " + @"/USA/fedlandfs.shp");
            globalFiles.Add("fedlandfws", "             " + @"/USA/fedlandfws.shp");
            globalFiles.Add("fedlanddod", "             " + @"/USA/fedlanddod.shp");
            globalFiles.Add("fedlandbia", "             " + @"/USA/fedlandbia.shp");
            globalFiles.Add("stateborders", "           " + @"/USA/stateborders.shp");
            globalFiles.Add("rail100k", "               " + @"/USA/rail100k.shp");
            globalFiles.Add("ushigh1", "                " + @"/USA/ushigh1.shp");
            globalFiles.Add("highwaynetwork1_50m", "    " + @"/USA/highwaynetwork1_50m.shp");
            globalFiles.Add("HighwayNetwork2_1b", "     " + @"/USA/HighwayNetwork2_1b.shp");
            globalFiles.Add("HighwayNetwork2_2", "      " + @"/USA/HighwayNetwork2_2.shp");
            globalFiles.Add("highwaynetwork1_10m", "    " + @"/USA/highwaynetwork1_10m.shp");
            globalFiles.Add("HighwayNetwork1", "        " + @"/USA/HighwayNetwork1.shp");
            globalFiles.Add("cities_c2", "              " + @"/USA/cities_c2.shp");
            globalFiles.Add("cities_e", "               " + @"/USA/cities_e.shp");
            globalFiles.Add("statetext", "              " + @"/USA/statetext.shp");
            globalFiles.Add("gchurch", "                " + @"/USA/POI/gchurch.shp");
            globalFiles.Add("ggolf", "                  " + @"/USA/POI/ggolf.shp");
            globalFiles.Add("ghospitl", "               " + @"/USA/POI/ghospitl.shp");

            globalFiles.Add("provinceborder", "         " + @"/Canada/provinceborder.shp");
            globalFiles.Add("CanadaStreets", "          " + @"/Canada/CanadaStreets.shp");
            globalFiles.Add("provincetext", "           " + @"/Canada/provincetext.shp");
            globalFiles.Add("canhwy", "                 " + @"/Canada/canhwy.shp");
            globalFiles.Add("highway2", "               " + @"/Canada/highway2.shp");
            globalFiles.Add("highway1", "               " + @"/Canada/highway1.shp");

            LoadLayers();

            if (File.Exists(Path.Combine(dataFolder, "LayersBoundingBoxes.idx")) && rtreeSpatialIndex == null)
            {
                rtreeSpatialIndex = new RtreeSpatialIndex(Path.Combine(dataFolder, "LayersBoundingBoxes.idx"), GeoFileReadWriteMode.Read);
            }
        }

        public Collection<WorldStreetsShapeFileFeatureLayer> StaticLayers
        {
            get
            {
                return staticLayers;
            }
        }

        public Collection<WorldStreetsShapeFileFeatureLayer> DynamicLayers
        {
            get
            {
                return dynamicLayers;
            }
        }

        public static RectangleShape DefaultExtent
        {
            get
            {
                return new RectangleShape(-125, 50, -66, 23);
            }
        }

        public bool ShowPOI
        {
            get
            {
                return showPOI;
            }
            set
            {
                showPOI = true;
            }
        }

        public void LoadLayers()
        {
            staticLayers.Clear();
            dynamicLayers.Clear();

            RenderSymbolLayer("cover");
            RenderSymbolLayer("ocean");
            RenderSymbolLayer("cntry02");
            LoadStateSymbolLayers(@"USA/ctycu");
            RenderSymbolLayer("urbanareaMinUSA");
            RenderSymbolLayer("urban_dtl");
            RenderSymbolLayer("evergreen");
            RenderSymbolLayer("sand");
            RenderSymbolLayer("fedlandnps");
            RenderSymbolLayer("fedlandnpsclip");
            RenderSymbolLayer("fedlandfs");
            RenderSymbolLayer("fedlandfws");
            RenderSymbolLayer("fedlanddod");
            RenderSymbolLayer("fedlandbia");
            RenderSymbolLayer("innerwater");
            RenderSymbolLayer("innerwaterMinUSA");
            RenderSymbolLayer("lawrenceriver");
            RenderSymbolLayer("permanentriver");
            RenderSymbolLayer("permanentriverMinusUSA");
            RenderSymbolLayer("lake");
            LoadStateSymbolLayers(@"USA/wat");
            RenderSymbolLayer("stateborders");
            RenderSymbolLayer("provinceborder");
            RenderSymbolLayer("primaryborder");
            RenderSymbolLayer("internationalborder");
            RenderSymbolLayer("internationalborder_dtl");
            RenderSymbolLayer("contestedborder");
            RenderSymbolLayer("railMinusUSA");
            RenderSymbolLayer("rail100k");
            LoadLowerRoadSymbolLayers();
            RenderSymbolLayer("CanadaStreets");
            LoadHighRoadSymbolLayers();
            RenderSymbolLayer("road");
            RenderSymbolLayer("roadMinUSACan");
            RenderSymbolLayer("ushigh1");
            RenderSymbolLayer("canhwy");
            RenderSymbolLayer("highwaynetwork1_50m");
            RenderSymbolLayer("HighwayNetwork2_1b");
            RenderSymbolLayer("HighwayNetwork2_2");
            RenderSymbolLayer("highwaynetwork1_10m");
            RenderSymbolLayer("HighwayNetwork1");
            RenderSymbolLayer("highway2");
            RenderSymbolLayer("highway1");
            RenderSymbolLayer("airport");
            RenderSymbolLayer("cities_c2");
            RenderSymbolLayer("capital");
            RenderSymbolLayer("noncapital");
            RenderSymbolLayer("depthline");
            if (showPOI)
            {
                LoadPoiLptSymbolLayers();
                RenderSymbolLayer("gchurch");
                RenderSymbolLayer("ggolf");
                RenderSymbolLayer("ghospitl");
            }

            RenderLabelLayer("cntry02");
            RenderLabelLayer("capital");
            RenderLabelLayer("noncapital");
            RenderLabelLayer("airport");
            RenderLabelLayer("cities_c2");
            RenderLabelLayer("urbanareaMinUSA");
            RenderLabelLayer("urban_dtl");
            RenderLabelLayer("cities_e");
            RenderLabelLayer("settlement");
            LoadHighRoadLabelLayers();
            LoadLowerRoadLabelLayers();
            RenderLabelLayer("CanadaStreets");
            RenderLabelLayer("ushigh1");
            RenderLabelLayer("highwaynetwork1_50m");
            RenderLabelLayer("highwaynetwork1_10m");
            RenderLabelLayer("highway1");
            RenderLabelLayer("highway2");
            RenderLabelLayer("HighwayNetwork2_1b");
            RenderLabelLayer("HighwayNetwork2_2");
            RenderLabelLayer("HighwayNetwork1");
            RenderLabelLayer("fedlandbia");
            RenderLabelLayer("fedlandfs");
            RenderLabelLayer("fedlandfws");
            RenderLabelLayer("fedlandnpsclip");
            RenderLabelLayer("fedlanddod");
            RenderLabelLayer("innerwater");
            RenderLabelLayer("innerwaterMinUSA");
            LoadStateLabelLayers(@"USA/wat");
            RenderLabelLayer("statetext");
            RenderLabelLayer("provincetext");
            RenderLabelLayer("oceantext");
            RenderLabelLayer("depthline");
            if (showPOI)
            {
                LoadPoiLptLabelLayers();
                RenderLabelLayer("gchurch");
                RenderLabelLayer("ggolf");
                RenderLabelLayer("ghospitl");
            }
        }

        private void RenderSymbolLayer(string key)
        {
            string shapefileName = dataFolder + globalFiles[key].Trim();
            if (!File.Exists(shapefileName)) { return; }

            WorldStreetsShapeFileFeatureLayer currentLayer = new WorldStreetsShapeFileFeatureLayer(shapefileName, GeoFileReadWriteMode.Read);
            string LayerName = Path.GetFileNameWithoutExtension(shapefileName);


            switch (LayerName.ToUpperInvariant())
            {
                case "COVER":
                    RenderSymbolCover(ref currentLayer);
                    break;
                case "CNTRY02":
                    RenderSymbolCntry02(ref currentLayer);
                    break;
                case "LAKE":
                    RenderSymbolLake(ref currentLayer);
                    break;
                case "STATEBORDERS":
                    RenderSymbolStateBorders(ref currentLayer);
                    break;
                case "PROVINCEBORDER":
                    RenderSymbolProvinceBorders(ref currentLayer);
                    break;
                case "PRIMARYBORDER":
                    RenderSymbolPrimaryBorder(ref currentLayer);
                    break;
                case "FEDLANDNPS":
                    RenderSymbolFedlandNPS(ref currentLayer);
                    break;
                case "CAPITAL":
                    RenderSymbolCaptial(ref currentLayer);
                    break;
                case "NONCAPITAL":
                    RenderSymbolNonCapital(ref currentLayer);
                    break;
                case "DEPTHLINE":
                    RenderSymbolDepthline(ref currentLayer);
                    break;
                case "USHIGH1":
                    RenderSymbolUSHigh1(ref currentLayer);
                    break;
                case "CANHWY":
                    RenderSymbolCanhwy(ref currentLayer);
                    break;
                case "FEDLANDFS":
                    RenderSymbolFedlandFS(ref currentLayer);
                    break;
                case "FEDLANDNPSCLIP":
                    RenderSymbolFedlandNPSclip(ref currentLayer);
                    break;
                case "SAND":
                    RenderSymbolSand(ref currentLayer);
                    break;
                case "EVERGREEN":
                    RenderSymbolEvergreen(ref currentLayer);
                    break;
                case "HIGHWAYNETWORK1_10M":
                    RenderSymbolHighWayNetwork1_10m(ref currentLayer);
                    break;
                case "HIGHWAY1":
                    RenderSymbolHighway1(ref currentLayer);
                    break;
                case "HIGHWAY2":
                    RenderSymbolHighway2(ref currentLayer);
                    break;
                case "HIGHWAYNETWORK1_50M":
                    RenderSymbolHighWayNetwork1_50m(ref currentLayer);
                    break;
                case "CITIES_C2":
                    RenderSymbolCities_c2(ref currentLayer);
                    break;
                case "AIRPORT":
                    RenderSymbolAirport(ref currentLayer);
                    break;
                case "PERMANENTRIVER":
                    RenderSymbolPermanentriver(ref currentLayer);
                    break;
                case "PERMANENTRIVERMINUSUSA":
                    RenderSymbolPermanentriverMinusUSA(ref currentLayer);
                    break;
                case "FEDLANDBIA":
                    RenderSymbolFedlandBIA(ref currentLayer);
                    break;
                case "FEDLANDDOD":
                    RenderSymbolFedlandDOD(ref currentLayer);
                    break;
                case "HIGHWAYNETWORK2_1B":
                    RenderSymbolHighWayNetwork2_1b(ref currentLayer);
                    break;
                case "HIGHWAYNETWORK2_2":
                    RenderSymbolHighWayNetwork2_2(ref currentLayer);
                    break;
                case "OCEAN":
                    RenderSymbolOcean(ref currentLayer);
                    break;
                case "FEDLANDFWS":
                    RenderSymbolFedlandFWS(ref currentLayer);
                    break;
                case "INNERWATER":
                    RenderSymbolInnerWater(ref currentLayer);
                    break;
                case "INNERWATERMINUSA":
                    RenderSymbolInnerWaterMinusUSA(ref currentLayer);
                    break;
                case "LAWRENCERIVER":
                    RenderSymbolLawrenceRiver(ref currentLayer);
                    break;
                case "INTERNATIONALBORDER":
                    RenderSymbolInternationalborder(ref currentLayer);
                    break;
                case "INTERNATIONALBORDER_DTL":
                    RenderSymbolInternationalborder_dtl(ref currentLayer);
                    break;
                case "CONTESTEDBORDER":
                    RenderSymbolContestedborder(ref currentLayer);
                    break;
                case "ROADMINUSACAN":
                    RenderSymbolRoadMinusUSACan(ref currentLayer);
                    break;
                case "ROAD":
                    RenderSymbolRoad(ref currentLayer);
                    break;
                case "URBANAREAMINUSA":
                    RenderSymbolUrbanareaMinusUSA(ref currentLayer);
                    break;
                case "URBAN_DTL":
                    RenderSymbolUrban_dtl(ref currentLayer);
                    break;
                case "HIGHWAYNETWORK1":
                    RenderSymbolHighWayNetwork1(ref currentLayer);
                    break;
                case "RAILMINUSUSA":
                    RenderSymbolRailMinusUSA(ref currentLayer);
                    break;
                case "CANADASTREETS":
                    LoadSymbolCanadaStreetLayers(ref currentLayer);
                    break;
                case "RAIL100K":
                    RenderSymbolRailUSA(ref currentLayer);
                    break;
                case "GCHURCH":
                    RenderSymbolChurch(ref currentLayer);
                    break;
                case "GGOLF":
                    RenderSymbolGolf(ref currentLayer);
                    break;
                case "GHOSPITL":
                    RenderSymbolHospital(ref currentLayer);
                    break;
                default:
                    if (RenderingCustomLayer != null)
                    {
                        RenderingCustomLayer(currentLayer);
                    }
                    else
                    {
                        throw new ArgumentException("No Rendering Logic", "currentLayer");
                    }
                    break;
            }
        }

        private void RenderLabelLayer(string key)
        {
            string shapefileName = dataFolder + globalFiles[key].Trim();
            if (!File.Exists(shapefileName)) { return; }

            WorldStreetsShapeFileFeatureLayer currentLayer = new WorldStreetsShapeFileFeatureLayer(shapefileName, GeoFileReadWriteMode.Read);
            string LayerName = Path.GetFileNameWithoutExtension(shapefileName);

            switch (LayerName.ToUpperInvariant())
            {
                case "OCEANTEXT":
                    RenderLabelOceanText(ref currentLayer);
                    break;
                case "CNTRY02":
                    RenderLabelCntry02(ref currentLayer);
                    break;
                case "CAPITAL":
                    RenderLabelCaptial(ref currentLayer);
                    break;
                case "NONCAPITAL":
                    RenderLabelNonCapital(ref currentLayer);
                    break;
                case "USHIGH1":
                    RenderLabelUSHigh1(ref currentLayer);
                    break;
                case "HIGHWAYNETWORK1_10M":
                    RenderLabelHighWayNetwork1_10m(ref currentLayer);
                    break;
                case "HIGHWAYNETWORK1_50M":
                    RenderLabelHighWayNetwork1_50m(ref currentLayer);
                    break;
                case "CITIES_C2":
                    RenderLabelCities_c2(ref currentLayer);
                    break;
                case "DEPTHLINE":
                    RenderLabelDepthline(ref currentLayer);
                    break;
                case "FEDLANDBIA":
                    RenderLabelFedlandBIA(ref currentLayer);
                    break;
                case "FEDLANDFS":
                    RenderLabelFedlandFS(ref currentLayer);
                    break;
                case "FEDLANDNPSCLIP":
                    RenderLabelFedlandNPSclip(ref currentLayer);
                    break;
                case "FEDLANDFWS":
                    RenderLabelFedlandFWS(ref currentLayer);
                    break;
                case "FEDLANDDOD":
                    RenderLabelFedlandDOD(ref currentLayer);
                    break;
                case "STATETEXT":
                    RenderLabelStateText(ref currentLayer);
                    break;
                case "PROVINCETEXT":
                    RenderLabelProvinceText(ref currentLayer);
                    break;
                case "SETTLEMENT":
                    RenderLabelSettlement(ref currentLayer);
                    break;
                case "URBANAREAMINUSA":
                    RenderLabelUrbanareaMinusUSA(ref currentLayer);
                    break;
                case "URBAN_DTL":
                    RenderLabelUrbanarea_dtl(ref currentLayer);
                    break;
                case "HIGHWAY1":
                    RenderLabelHighway1(ref currentLayer);
                    break;
                case "HIGHWAY2":
                    RenderLabelHighway2(ref currentLayer);
                    break;
                case "HIGHWAYNETWORK2_1B":
                    RenderLabelHighWayNetwork2_1b(ref currentLayer);
                    break;
                case "HIGHWAYNETWORK2_2":
                    RenderLabelHighWayNetwork2_2(ref currentLayer);
                    break;
                case "INNERWATER":
                    RenderLabelInnerWater(ref currentLayer);
                    break;
                case "INNERWATERMINUSA":
                    RenderLabelInnerWaterMinusUSA(ref currentLayer);
                    break;
                case "CITIES_E":
                    RenderLabelCities_e(ref currentLayer);
                    break;
                case "HIGHWAYNETWORK1":
                    RenderLabelHighWayNetwork1(ref currentLayer);
                    break;
                case "AIRPORT":
                    RenderLabelAirport(ref currentLayer);
                    break;
                case "CANADASTREETS":
                    LoadLabelCanadaStreetLayers(ref currentLayer);
                    break;
                case "GCHURCH":
                    RenderLabelChurch(ref currentLayer);
                    break;
                case "GGOLF":
                    RenderLabelGolf(ref currentLayer);
                    break;
                case "GHOSPITL":
                    RenderLabelHospital(ref currentLayer);
                    break;
                default:
                    if (RenderingCustomLayer != null)
                    {
                        RenderingCustomLayer(currentLayer);
                    }
                    else
                    {
                        throw new ArgumentException("No Rendering Logic", "currentLayer");
                    }
                    break;
            }
        }

        private void RenderSymbolCntry02(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Country";

            layer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 242, 239, 233));

            layer.ZoomLevelSet.ZoomLevel02.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 242, 239, 233), new GeoColor(250, 156, 155, 154));
            layer.ZoomLevelSet.ZoomLevel02.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level05;

            layer.ZoomLevelSet.ZoomLevel06.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 242, 239, 233), new GeoColor(250, 156, 155, 154));
            layer.ZoomLevelSet.ZoomLevel06.DefaultAreaStyle.OutlinePen.Width = 1.5f;

            layer.ZoomLevelSet.ZoomLevel07.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 242, 239, 233));
            layer.ZoomLevelSet.ZoomLevel07.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level08;

            staticLayers.Add(layer);
        }

        private void RenderSymbolCover(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "BackGround";

            layer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 153, 179, 204));
            layer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level08;

            layer.ZoomLevelSet.ZoomLevel09.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 242, 239, 233));
            layer.ZoomLevelSet.ZoomLevel09.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(layer);
        }

        private void RenderSymbolLake(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Lake";

            layer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.FromArgb(255, 153, 179, 204);
            layer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level06;

            staticLayers.Add(layer);
        }

        private void RenderSymbolDepthline(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Depth Line";

            layer.ZoomLevelSet.ZoomLevel08.DefaultLineStyle.OuterPen.Color = GeoColor.FromArgb(50, GeoColor.StandardColors.Navy);
            layer.ZoomLevelSet.ZoomLevel08.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(layer);
        }

        private void RenderLabelDepthline(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Depth Line Label";

            layer.ZoomLevelSet.ZoomLevel09.DefaultTextStyle = new TextStyle("Crv", new GeoFont("Arial", 6, DrawingFontStyles.Italic), new GeoSolidBrush(GeoColor.StandardColors.Navy));
            layer.ZoomLevelSet.ZoomLevel09.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            layer.ZoomLevelSet.ZoomLevel09.DefaultTextStyle.SuppressPartialLabels = true;
            layer.ZoomLevelSet.ZoomLevel09.DefaultTextStyle.GridSize = 40;
            layer.ZoomLevelSet.ZoomLevel09.DefaultTextStyle.XOffsetInPixel = 0;
            layer.ZoomLevelSet.ZoomLevel09.DefaultTextStyle.YOffsetInPixel = 4;
            layer.ZoomLevelSet.ZoomLevel09.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            layer.ZoomLevelSet.ZoomLevel09.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(layer);
        }

        private void RenderLabelOceanText(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "OceanText Label";

            ValueStyle valueStyle1 = new ValueStyle();
            valueStyle1.ColumnName = "size";
            TextStyle textStyle1 = new TextStyle("Name", new GeoFont("Arial", 6, DrawingFontStyles.Italic), new GeoSolidBrush(GeoColor.StandardColors.Navy));
            textStyle1.TextLineSegmentRatio = 10;
            valueStyle1.ValueItems.Add(new ValueItem("1", textStyle1));

            ValueStyle valueStyle2 = new ValueStyle();
            valueStyle2.ColumnName = "size";
            TextStyle textStyle2 = new TextStyle("Name", new GeoFont("Arial", 8, DrawingFontStyles.Italic), new GeoSolidBrush(GeoColor.StandardColors.Navy));
            textStyle2.TextLineSegmentRatio = 10;
            valueStyle2.ValueItems.Add(new ValueItem("1", textStyle2));

            ValueStyle valueStyle3 = new ValueStyle();
            valueStyle3.ColumnName = "size";
            TextStyle textStyle3 = new TextStyle("Name", new GeoFont("Arial", 8, DrawingFontStyles.Italic), new GeoSolidBrush(GeoColor.StandardColors.Navy));
            textStyle3.TextLineSegmentRatio = 10;
            TextStyle textStyle4 = new TextStyle("Name", new GeoFont("Arial", 6, DrawingFontStyles.Italic), new GeoSolidBrush(GeoColor.StandardColors.Navy));
            textStyle4.TextLineSegmentRatio = 1.2;
            valueStyle3.ValueItems.Add(new ValueItem("1", textStyle3));
            valueStyle3.ValueItems.Add(new ValueItem("0", textStyle4));

            ValueStyle valueStyle4 = new ValueStyle();
            valueStyle4.ColumnName = "size";
            TextStyle textStyle5 = new TextStyle("Name", new GeoFont("Arial", 10, DrawingFontStyles.Italic), new GeoSolidBrush(GeoColor.StandardColors.Navy));
            textStyle5.TextLineSegmentRatio = 10;
            TextStyle textStyle6 = new TextStyle("Name", new GeoFont("Arial", 8, DrawingFontStyles.Italic), new GeoSolidBrush(GeoColor.StandardColors.Navy));
            textStyle6.TextLineSegmentRatio = 1.2;
            valueStyle4.ValueItems.Add(new ValueItem("1", textStyle5));
            valueStyle4.ValueItems.Add(new ValueItem("0", textStyle6));

            layer.ZoomLevelSet.ZoomLevel02.CustomStyles.Add(valueStyle1);

            layer.ZoomLevelSet.ZoomLevel03.CustomStyles.Add(valueStyle2);

            layer.ZoomLevelSet.ZoomLevel04.CustomStyles.Add(valueStyle3);
            layer.ZoomLevelSet.ZoomLevel04.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level06;

            layer.ZoomLevelSet.ZoomLevel07.CustomStyles.Add(valueStyle4);
            layer.ZoomLevelSet.ZoomLevel07.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            layer.DrawingMarginInPixel = 250;
            dynamicLayers.Add(layer);
        }

        private void RenderLabelCntry02(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "Country Label";

            ClassBreakStyle classBreakStyle1 = new ClassBreakStyle("SQKM");
            TextStyle textStyle1 = TextStyles.CreateSimpleTextStyle("CNTRY_NAME", "Arail", 9, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2, 0, 0);
            textStyle1.FittingPolygon = true;
            textStyle1.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            textStyle1.GridSize = 40;

            TextStyle textStyle2 = TextStyles.CreateSimpleTextStyle("CNTRY_NAME", "Arail", 12, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 3, 0, 0);
            textStyle2.FittingPolygon = true;
            textStyle2.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            textStyle2.GridSize = 40;

            classBreakStyle1.ClassBreaks.Add(new ClassBreak(243300, textStyle1));
            classBreakStyle1.ClassBreaks.Add(new ClassBreak(3000000, textStyle2));

            ClassBreakStyle classBreakStyle2 = new ClassBreakStyle("SQKM");
            TextStyle textStyle3 = TextStyles.CreateSimpleTextStyle("CNTRY_NAME", "Arail", 11, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2, 0, 0);
            textStyle3.BestPlacement = true;
            textStyle3.SuppressPartialLabels = true;
            textStyle3.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            textStyle3.GridSize = 40;

            TextStyle textStyle4 = TextStyles.CreateSimpleTextStyle("CNTRY_NAME", "Arail", 14, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 3, 0, 0);
            textStyle4.BestPlacement = true;
            textStyle4.SuppressPartialLabels = true;
            textStyle4.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            textStyle4.GridSize = 40;

            classBreakStyle2.ClassBreaks.Add(new ClassBreak(0, textStyle3));
            classBreakStyle2.ClassBreaks.Add(new ClassBreak(3000000, textStyle4));

            ClassBreakStyle classBreakStyle3 = new ClassBreakStyle("SQKM");
            TextStyle textStyle5 = TextStyles.CreateSimpleTextStyle("CNTRY_NAME", "Arail", 12, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2, 0, 0);
            textStyle5.BestPlacement = true;
            textStyle5.SuppressPartialLabels = true;
            textStyle5.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            textStyle5.GridSize = 40;

            TextStyle textStyle6 = TextStyles.CreateSimpleTextStyle("CNTRY_NAME", "Arail", 15, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 3, 0, 0);
            textStyle6.BestPlacement = true;
            textStyle6.SuppressPartialLabels = true;
            textStyle6.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            textStyle6.GridSize = 40;

            classBreakStyle3.ClassBreaks.Add(new ClassBreak(0, textStyle5));
            classBreakStyle3.ClassBreaks.Add(new ClassBreak(3000000, textStyle6));

            ClassBreakStyle classBreakStyle4 = new ClassBreakStyle("SQKM");
            TextStyle textStyle7 = TextStyles.CreateSimpleTextStyle("CNTRY_NAME", "Arail", 13, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2, 0, 0);
            textStyle7.BestPlacement = true;
            textStyle7.SuppressPartialLabels = true;
            textStyle7.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            textStyle7.GridSize = 40;

            TextStyle textStyle8 = TextStyles.CreateSimpleTextStyle("CNTRY_NAME", "Arail", 16, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 3, 0, 0);
            textStyle8.BestPlacement = true;
            textStyle8.SuppressPartialLabels = true;
            textStyle8.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            textStyle8.GridSize = 40;

            classBreakStyle4.ClassBreaks.Add(new ClassBreak(0, textStyle7));
            classBreakStyle4.ClassBreaks.Add(new ClassBreak(3000000, textStyle8));

            ClassBreakStyle classBreakStyle5 = new ClassBreakStyle("SQKM");
            TextStyle textStyle09 = TextStyles.CreateSimpleTextStyle("CNTRY_NAME", "Arail", 14, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2, 0, 0);
            textStyle09.BestPlacement = true;
            textStyle09.SuppressPartialLabels = true;
            textStyle09.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            textStyle09.GridSize = 40;

            TextStyle textStyle10 = TextStyles.CreateSimpleTextStyle("CNTRY_NAME", "Arail", 18, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 3, 0, 0);
            textStyle10.BestPlacement = true;
            textStyle10.SuppressPartialLabels = true;
            textStyle10.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            textStyle10.GridSize = 40;

            classBreakStyle5.ClassBreaks.Add(new ClassBreak(0, textStyle09));
            classBreakStyle5.ClassBreaks.Add(new ClassBreak(3000000, textStyle10));

            labelLayer.ZoomLevelSet.ZoomLevel03.CustomStyles.Add(classBreakStyle1);
            labelLayer.ZoomLevelSet.ZoomLevel03.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level04;

            labelLayer.ZoomLevelSet.ZoomLevel05.CustomStyles.Add(classBreakStyle2);

            labelLayer.ZoomLevelSet.ZoomLevel06.CustomStyles.Add(classBreakStyle3);

            labelLayer.ZoomLevelSet.ZoomLevel07.CustomStyles.Add(classBreakStyle4);

            labelLayer.ZoomLevelSet.ZoomLevel08.CustomStyles.Add(classBreakStyle5);
            labelLayer.ZoomLevelSet.ZoomLevel08.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level10;

            labelLayer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(labelLayer);
        }

        private void RenderSymbolFedlandNPS(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "National Park Service ";

            Layer.DrawingQuality = DrawingQuality.HighSpeed;

            Layer.ZoomLevelSet.ZoomLevel04.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 167, 204, 149));
            Layer.ZoomLevelSet.ZoomLevel04.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level09;

            staticLayers.Add(Layer);
        }

        private void RenderSymbolCaptial(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Capitals (Population)";

            ValueStyle valueStyle1 = new ValueStyle();
            valueStyle1.ColumnName = "pop_rank";
            ValueStyle valueStyle2 = new ValueStyle();
            valueStyle2.ColumnName = "pop_rank";

            PointStyle styles1 = PointStyles.CreateSimpleCircleStyle(GeoColor.FromArgb(255, 216, 95, 95), 7, GeoColor.StandardColors.Black, 1);
            PointStyle styles2 = PointStyles.CreateSimpleCircleStyle(GeoColor.FromArgb(255, 216, 95, 95), 5, GeoColor.StandardColors.Black, 1);
            PointStyle styles3 = PointStyles.CreateSimpleCircleStyle(GeoColor.FromArgb(255, 216, 95, 95), 5, GeoColor.StandardColors.Black, 1);

            ValueItem valueItem1 = new ValueItem("1", styles3);
            ValueItem valueItem2 = new ValueItem("2", styles3);
            ValueItem valueItem3 = new ValueItem("3", styles3);

            valueStyle1.ValueItems.Add(valueItem1);
            valueStyle1.ValueItems.Add(valueItem2);

            valueStyle2.ValueItems.Add(valueItem1);
            valueStyle2.ValueItems.Add(valueItem2);
            valueStyle2.ValueItems.Add(valueItem3);

            layer.ZoomLevelSet.ZoomLevel05.CustomStyles.Add(valueStyle1);

            layer.ZoomLevelSet.ZoomLevel06.CustomStyles.Add(valueStyle2);
            layer.ZoomLevelSet.ZoomLevel06.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level11;

            layer.DrawingMarginInPixel = 250;
            staticLayers.Add(layer);
        }

        private void RenderLabelCaptial(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Capitals (Population) Label";

            ValueStyle valueStyle1 = new ValueStyle();
            valueStyle1.ColumnName = "pop_rank";

            ValueStyle valueStyle2 = new ValueStyle();
            valueStyle2.ColumnName = "pop_rank";

            TextStyle textStyle1 = TextStyles.CreateSimpleTextStyle("City_Name", "Arial", 10, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2f, 0, 0);
            TextStyle textStyle2 = TextStyles.CreateSimpleTextStyle("City_Name", "Arial", 11, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2f, 0, 0);
            TextStyle textStyle3 = TextStyles.CreateSimpleTextStyle("City_Name", "Arial", 12, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2f, 0, 0);
            TextStyle textStyle4 = TextStyles.CreateSimpleTextStyle("City_Name", "Arial", 13, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2f, 0, 0);
            TextStyle textStyle5 = TextStyles.CreateSimpleTextStyle("City_Name", "Arial", 14, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2f, 0, 0);
            textStyle1.BestPlacement = true;
            textStyle2.BestPlacement = true;
            textStyle3.BestPlacement = true;
            textStyle4.BestPlacement = true;
            textStyle5.BestPlacement = true;

            textStyle1.SuppressPartialLabels = true;
            textStyle1.GridSize = 40;
            textStyle1.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;

            textStyle2.SuppressPartialLabels = true;
            textStyle2.GridSize = 40;
            textStyle2.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;

            textStyle3.SuppressPartialLabels = true;
            textStyle3.GridSize = 40;
            textStyle3.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;

            textStyle4.SuppressPartialLabels = true;
            textStyle4.GridSize = 40;
            textStyle4.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;

            textStyle5.SuppressPartialLabels = true;
            textStyle5.GridSize = 40;
            textStyle5.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;

            ValueItem valueItem1 = new ValueItem("1", textStyle1);
            ValueItem valueItem2 = new ValueItem("2", textStyle1);

            valueStyle1.ValueItems.Add(valueItem1);
            valueStyle1.ValueItems.Add(valueItem2);
            layer.ZoomLevelSet.ZoomLevel05.CustomStyles.Add(valueStyle1);

            layer.ZoomLevelSet.ZoomLevel06.DefaultTextStyle = textStyle1;

            layer.ZoomLevelSet.ZoomLevel07.DefaultTextStyle = textStyle2;

            layer.ZoomLevelSet.ZoomLevel08.DefaultTextStyle = textStyle3;

            layer.ZoomLevelSet.ZoomLevel09.DefaultTextStyle = textStyle4;
            layer.ZoomLevelSet.ZoomLevel09.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level11;

            layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle = textStyle5;
            layer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(layer);
        }

        private void RenderSymbolNonCapital(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Other Cities (Population)";

            PointStyle styles1 = PointStyles.CreateSimpleCircleStyle(GeoColor.FromArgb(255, 216, 95, 95), 7, GeoColor.StandardColors.Black, 1);
            PointStyle styles2 = PointStyles.CreateSimpleCircleStyle(GeoColor.FromArgb(255, 216, 95, 95), 5, GeoColor.StandardColors.Black, 1);
            PointStyle styles3 = PointStyles.CreateSimpleCircleStyle(GeoColor.FromArgb(255, 216, 95, 95), 5, GeoColor.StandardColors.Black, 1);
            PointStyle styles4 = PointStyles.CreateSimpleCircleStyle(GeoColor.FromArgb(255, 216, 95, 95), 4, GeoColor.StandardColors.Black, 1);
            PointStyle styles5 = PointStyles.CreateSimpleCircleStyle(GeoColor.FromArgb(255, 216, 95, 95), 3, GeoColor.StandardColors.Black, 1);
            PointStyle styles6 = PointStyles.CreateSimpleCircleStyle(GeoColor.FromArgb(255, 216, 95, 95), 2, GeoColor.StandardColors.Black, 1);
            PointStyle styles7 = PointStyles.CreateSimpleCircleStyle(GeoColor.FromArgb(255, 216, 95, 95), 2, GeoColor.StandardColors.Black, 1);

            ValueStyle valueStyle1 = new ValueStyle();
            valueStyle1.ColumnName = "pop_rank";
            valueStyle1.ValueItems.Add(new ValueItem("1", styles3));
            valueStyle1.ValueItems.Add(new ValueItem("2", styles3));
            valueStyle1.ValueItems.Add(new ValueItem("3", styles3));

            ValueStyle valueStyle2 = new ValueStyle();
            valueStyle2.ColumnName = "pop_rank";
            valueStyle2.ValueItems.Add(new ValueItem("1", styles3));
            valueStyle2.ValueItems.Add(new ValueItem("2", styles3));
            valueStyle2.ValueItems.Add(new ValueItem("3", styles3));
            valueStyle2.ValueItems.Add(new ValueItem("4", styles3));

            layer.ZoomLevelSet.ZoomLevel05.CustomStyles.Add(valueStyle1);
            layer.ZoomLevelSet.ZoomLevel06.CustomStyles.Add(valueStyle2);

            ValueStyle valueStyle3 = new ValueStyle();
            valueStyle3.ColumnName = "pop_rank";
            valueStyle3.ValueItems.Add(new ValueItem("1", styles4));
            valueStyle3.ValueItems.Add(new ValueItem("2", styles4));
            valueStyle3.ValueItems.Add(new ValueItem("3", styles4));
            valueStyle3.ValueItems.Add(new ValueItem("4", styles4));
            valueStyle3.ValueItems.Add(new ValueItem("5", styles4));
            valueStyle3.ValueItems.Add(new ValueItem("6", styles4));
            valueStyle3.ValueItems.Add(new ValueItem("7", styles4));
            layer.ZoomLevelSet.ZoomLevel07.CustomStyles.Add(valueStyle3);
            layer.ZoomLevelSet.ZoomLevel07.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level11;

            layer.DrawingMarginInPixel = 250;
            staticLayers.Add(layer);
        }

        private void RenderLabelNonCapital(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Other Cities (Population) Label";

            TextStyle textStyle1 = TextStyles.CreateSimpleTextStyle("City_Name", "Arial", 10, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2f, 0, 0);
            TextStyle textStyle2 = TextStyles.CreateSimpleTextStyle("City_Name", "Arial", 11, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2f, 0, 0);
            TextStyle textStyle3 = TextStyles.CreateSimpleTextStyle("City_Name", "Arial", 12, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2f, 0, 0);
            TextStyle textStyle4 = TextStyles.CreateSimpleTextStyle("City_Name", "Arial", 13, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2f, 0, 0);
            TextStyle textStyle5 = TextStyles.CreateSimpleTextStyle("City_Name", "Arial", 14, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2f, 0, 0);
            textStyle1.BestPlacement = true;
            textStyle2.BestPlacement = true;
            textStyle3.BestPlacement = true;
            textStyle4.BestPlacement = true;
            textStyle5.BestPlacement = true;

            textStyle1.SuppressPartialLabels = true;
            textStyle1.GridSize = 40;
            textStyle1.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;

            textStyle2.SuppressPartialLabels = true;
            textStyle2.GridSize = 40;
            textStyle2.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;

            textStyle3.SuppressPartialLabels = true;
            textStyle3.GridSize = 40;
            textStyle3.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;

            textStyle4.SuppressPartialLabels = true;
            textStyle4.GridSize = 40;
            textStyle4.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;

            textStyle5.SuppressPartialLabels = true;
            textStyle5.GridSize = 40;
            textStyle5.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;


            ValueStyle valueStyle2 = new ValueStyle();
            valueStyle2.ColumnName = "pop_rank";
            valueStyle2.ValueItems.Add(new ValueItem("1", textStyle1));
            valueStyle2.ValueItems.Add(new ValueItem("2", textStyle1));
            valueStyle2.ValueItems.Add(new ValueItem("3", textStyle1));
            layer.ZoomLevelSet.ZoomLevel05.CustomStyles.Add(valueStyle2);

            ValueStyle valueStyle3 = new ValueStyle();
            valueStyle3.ColumnName = "pop_rank";
            valueStyle3.ValueItems.Add(new ValueItem("1", textStyle1));
            valueStyle3.ValueItems.Add(new ValueItem("2", textStyle1));
            valueStyle3.ValueItems.Add(new ValueItem("3", textStyle1));
            valueStyle3.ValueItems.Add(new ValueItem("4", textStyle1));
            layer.ZoomLevelSet.ZoomLevel06.CustomStyles.Add(valueStyle3);

            layer.ZoomLevelSet.ZoomLevel07.DefaultTextStyle = textStyle2;
            layer.ZoomLevelSet.ZoomLevel08.DefaultTextStyle = textStyle3;
            layer.ZoomLevelSet.ZoomLevel09.DefaultTextStyle = textStyle4;
            layer.ZoomLevelSet.ZoomLevel09.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level11;

            layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle = textStyle5;
            layer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            layer.DrawingMarginInPixel = 250;
            dynamicLayers.Add(layer);
        }

        private void RenderSymbolUSHigh1(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "US Highway";

            Layer.DrawingQuality = DrawingQuality.CanvasSettings;

            LineStyle lineStyle1 = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 216, 171, 33), 1.75f, GeoColor.StandardColors.DarkGray, 1.75f, true);
            Layer.ZoomLevelSet.ZoomLevel06.DefaultLineStyle = lineStyle1;

            LineStyle lineStyle2 = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 216, 171, 33), 2.25f, GeoColor.StandardColors.DarkGray, 2.25f, true);
            Layer.ZoomLevelSet.ZoomLevel07.DefaultLineStyle = lineStyle2;

            staticLayers.Add(Layer);
        }

        private void RenderSymbolCanhwy(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Canada Highway";

            Layer.DrawingQuality = DrawingQuality.CanvasSettings;

            LineStyle lineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 216, 171, 33), 1.75f, GeoColor.StandardColors.DarkGray, 1.75f, true);
            Layer.ZoomLevelSet.ZoomLevel06.DefaultLineStyle = lineStyle;
            Layer.ZoomLevelSet.ZoomLevel06.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level07;

            Layer.ZoomLevelSet.ZoomLevel08.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 216, 171, 33), 2f, GeoColor.StandardColors.DarkGray, 4f, true);

            staticLayers.Add(Layer);
        }

        private void RenderSymbolFedlandFS(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Forest Service";
            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel07.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(150, 204, 213, 170));
            Layer.ZoomLevelSet.ZoomLevel07.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level11;

            Layer.ZoomLevelSet.ZoomLevel12.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(150, 204, 213, 170));
            Layer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelFedlandFS(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "Forest Service Label";

            labelLayer.DrawingQuality = DrawingQuality.CanvasSettings;

            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Name1", "Arial", 7, DrawingFontStyles.Regular, GeoColor.FromArgb(255, 109, 122, 61));
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.TextLineSegmentRatio = 2;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.GridSize = 40;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.FittingPolygon = true;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.FittingPolygonFactor = 1.5;
            labelLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            labelLayer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(labelLayer);
        }

        private void RenderLabelUSHigh1(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.DrawingQuality = DrawingQuality.HighSpeed;
            labelLayer.Name = "US Highway Label";
            labelLayer.ZoomLevelSet.ZoomLevel07.CustomStyles.Add(GetRoadIconStyleForUSHigh1());
            labelLayer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(labelLayer);
        }

        private IconValueStyle GetRoadIconStyleForUSHigh1()
        {
            IconValueStyle iconValueStyle = new IconValueStyle("Admn_class");

            IconValueItem item1 = new IconValueItem("Interstate", dataFolder + @"/Images/Small_Interstate_ThinOutline_1.gif", new TextStyle("Name", new GeoFont("Arial", 7, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColor.StandardColors.White)));
            item1.TextValueLengthMin = 1;
            item1.TextValueLengthMax = 2;

            IconValueItem item2 = new IconValueItem("Interstate", dataFolder + @"/Images/Small_Interstate_ThinOutline_2.gif", new TextStyle("Name", new GeoFont("Arial", 7, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColor.StandardColors.White)));
            item2.TextValueLengthMin = 3;
            item2.TextValueLengthMax = 4;

            iconValueStyle.IconValueItems.Add(item1);
            iconValueStyle.IconValueItems.Add(item2);

            iconValueStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            iconValueStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            iconValueStyle.GridSize = 100;
            iconValueStyle.SuppressPartialLabels = true;
            return iconValueStyle;
        }

        private void RenderSymbolSand(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Sand";

            layer.ZoomLevelSet.ZoomLevel07.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.FromArgb(255, 255, 238, 208);
            layer.ZoomLevelSet.ZoomLevel07.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(layer);
        }


        private void RenderSymbolEvergreen(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Evergreen";

            layer.ZoomLevelSet.ZoomLevel07.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.FromArgb(50, 167, 204, 149);
            layer.ZoomLevelSet.ZoomLevel07.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(layer);
        }

        private void RenderSymbolHighWayNetwork1_10m(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Interstates";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel09.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 216, 171, 33), 2.5f, GeoColor.StandardColors.DarkGray, 4.5f, true);

            Layer.ZoomLevelSet.ZoomLevel10.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 216, 171, 33), 3.5f, GeoColor.StandardColors.DarkGray, 5.5f, true);
            Layer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level10;

            staticLayers.Add(Layer);
        }

        private void RenderSymbolHighway1(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Canada Highway1";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel09.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 216, 171, 33), 2.5f, GeoColor.StandardColors.DarkGray, 4.5f, true);

            Layer.ZoomLevelSet.ZoomLevel10.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 216, 171, 33), 3.5f, GeoColor.StandardColors.DarkGray, 5.5f, true);
            Layer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelHighway1(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "Canada Highway1 Label";

            labelLayer.DrawingQuality = DrawingQuality.CanvasSettings;

            IconStyle iconStyle = new IconStyle(dataFolder + @"/Images/canada_hwy.gif", "Rtnumber1", new GeoFont("Arial", 8, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColor.StandardColors.Black));

            iconStyle.TextLineSegmentRatio = 2;
            iconStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            iconStyle.SuppressPartialLabels = true;
            iconStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;

            labelLayer.ZoomLevelSet.ZoomLevel09.CustomStyles.Add(iconStyle);
            labelLayer.ZoomLevelSet.ZoomLevel09.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            labelLayer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(labelLayer);
        }

        private void RenderLabelHighway2(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "Canada Highway2 Label";

            labelLayer.DrawingQuality = DrawingQuality.CanvasSettings;

            IconStyle iconStyle = new IconStyle(dataFolder + @"/Images/canada_road.gif", "Rtnumber1", new GeoFont("Arial", 8, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColor.StandardColors.Black));

            iconStyle.TextLineSegmentRatio = 2;
            iconStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            iconStyle.SuppressPartialLabels = true;
            iconStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;

            labelLayer.ZoomLevelSet.ZoomLevel10.CustomStyles.Add(iconStyle);
            labelLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            labelLayer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(labelLayer);
        }

        private void RenderSymbolHighway2(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Canada Highway2";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel09.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 239, 232, 119), 2, GeoColor.StandardColors.DarkGray, 4, true);
            Layer.ZoomLevelSet.ZoomLevel09.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level12;

            Layer.ZoomLevelSet.ZoomLevel13.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 239, 232, 119), 3, GeoColor.StandardColors.DarkGray, 6, true);
            Layer.ZoomLevelSet.ZoomLevel14.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 239, 232, 119), 5, GeoColor.StandardColors.DarkGray, 8, true);
            Layer.ZoomLevelSet.ZoomLevel15.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 239, 232, 119), 8, GeoColor.StandardColors.DarkGray, 11, true);
            Layer.ZoomLevelSet.ZoomLevel15.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }


        private void RenderLabelHighWayNetwork1_10m(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "Interstates Label";

            labelLayer.DrawingQuality = DrawingQuality.CanvasSettings;

            labelLayer.ZoomLevelSet.ZoomLevel09.CustomStyles.Add(GetRoadIconStyle(GeoColor.StandardColors.White));
            labelLayer.ZoomLevelSet.ZoomLevel09.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level10;
            labelLayer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(labelLayer);
        }

        private void RenderSymbolHighWayNetwork1_50m(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Interstates";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel08.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 216, 171, 33), 2f, GeoColor.StandardColors.DarkGray, 4f, true);

            staticLayers.Add(Layer);
        }

        private void RenderLabelHighWayNetwork1_50m(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "Interstates Label";

            labelLayer.DrawingQuality = DrawingQuality.CanvasSettings;

            labelLayer.ZoomLevelSet.ZoomLevel08.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.AllowOverlapping;
            labelLayer.ZoomLevelSet.ZoomLevel08.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel08.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel08.DefaultTextStyle.GridSize = 40;
            labelLayer.ZoomLevelSet.ZoomLevel08.CustomStyles.Add(GetRoadIconStyle(GeoColor.StandardColors.White));

            labelLayer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(labelLayer);
        }

        private IconValueStyle GetRoadIconStyle(GeoColor geoColor)
        {
            IconValueStyle iconValueStyle = new IconValueStyle("Signt1");

            IconValueItem item1 = new IconValueItem("I", dataFolder + @"/Images/Interstate_ThinOutline_1.gif", new TextStyle("Signn1", new GeoFont("Arial", 8, DrawingFontStyles.Bold), new GeoSolidBrush(geoColor)));
            item1.TextValueLengthMin = 1;
            item1.TextValueLengthMax = 2;

            IconValueItem item2 = new IconValueItem("I", dataFolder + @"/Images/Interstate_ThinOutline_2.gif", new TextStyle("Signn1", new GeoFont("Arial", 8, DrawingFontStyles.Bold), new GeoSolidBrush(geoColor)));
            item2.TextValueLengthMin = 3;
            item2.TextValueLengthMax = 4;

            IconValueItem item3 = new IconValueItem("S", dataFolder + @"/Images/state-oval.gif", new TextStyle("Signn1", new GeoFont("Arial", 8, DrawingFontStyles.Bold), new GeoSolidBrush(geoColor)));
            item3.TextValueLengthMin = 1;
            item3.TextValueLengthMax = 2;

            IconValueItem item4 = new IconValueItem("S", dataFolder + @"/Images/state-oval-2.gif", new TextStyle("Signn1", new GeoFont("Arial", 8, DrawingFontStyles.Bold), new GeoSolidBrush(geoColor)));
            item4.TextValueLengthMin = 3;
            item4.TextValueLengthMax = 5;

            IconValueItem item5 = new IconValueItem("U", dataFolder + @"/Images/ushwy-1.gif", new TextStyle("Signn1", new GeoFont("Arial", 8, DrawingFontStyles.Bold), new GeoSolidBrush(geoColor)));
            item5.TextValueLengthMin = 1;
            item5.TextValueLengthMax = 2;

            IconValueItem item6 = new IconValueItem("U", dataFolder + @"/Images/ushwy-2.gif", new TextStyle("Signn1", new GeoFont("Arial", 8, DrawingFontStyles.Bold), new GeoSolidBrush(geoColor)));
            item6.TextValueLengthMin = 3;
            item6.TextValueLengthMax = 3;

            IconValueItem item7 = new IconValueItem("U", dataFolder + @"/Images/ushwy-3.gif", new TextStyle("Signn1", new GeoFont("Arial", 8, DrawingFontStyles.Bold), new GeoSolidBrush(geoColor)));
            item7.TextValueLengthMin = 4;
            item7.TextValueLengthMax = 5;

            iconValueStyle.IconValueItems.Add(item1);
            iconValueStyle.IconValueItems.Add(item2);
            iconValueStyle.IconValueItems.Add(item3);
            iconValueStyle.IconValueItems.Add(item4);
            iconValueStyle.IconValueItems.Add(item5);
            iconValueStyle.IconValueItems.Add(item6);
            iconValueStyle.IconValueItems.Add(item7);

            iconValueStyle.GridSize = 100;
            iconValueStyle.TextLineSegmentRatio = 2;
            iconValueStyle.DuplicateRule = LabelDuplicateRule.NoDuplicateLabels;
            iconValueStyle.SuppressPartialLabels = true;
            iconValueStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;

            return iconValueStyle;
        }

        private void RenderSymbolCities_c2(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Small Cities";

            Layer.ZoomLevelSet.ZoomLevel08.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.StandardColors.White, 4, GeoColor.StandardColors.Black); ;
            Layer.ZoomLevelSet.ZoomLevel08.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level09;

            staticLayers.Add(Layer);
        }

        private void RenderLabelCities_c2(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "Small Cities Label";

            labelLayer.DrawingQuality = DrawingQuality.CanvasSettings;

            labelLayer.ZoomLevelSet.ZoomLevel08.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("areaname", "Arial", 10, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2f, 0, 0);
            labelLayer.ZoomLevelSet.ZoomLevel08.DefaultTextStyle.BestPlacement = true;
            labelLayer.ZoomLevelSet.ZoomLevel08.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel08.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel08.DefaultTextStyle.GridSize = 40;
            labelLayer.ZoomLevelSet.ZoomLevel08.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;

            labelLayer.ZoomLevelSet.ZoomLevel09.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("areaname", "Arial", 11, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2f, 0, 0);
            labelLayer.ZoomLevelSet.ZoomLevel09.DefaultTextStyle.BestPlacement = true;
            labelLayer.ZoomLevelSet.ZoomLevel09.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel09.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel09.DefaultTextStyle.GridSize = 40;
            labelLayer.ZoomLevelSet.ZoomLevel09.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            labelLayer.ZoomLevelSet.ZoomLevel09.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level11;

            labelLayer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("areaname", "Arial", 12, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2f, 0, 0);
            labelLayer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.BestPlacement = true;
            labelLayer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.GridSize = 40;
            labelLayer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            labelLayer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            labelLayer.DrawingMarginInPixel = 250;
            dynamicLayers.Add(labelLayer);
        }

        private void RenderSymbolAirport(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Airport";

            layer.DrawingQuality = DrawingQuality.CanvasSettings;

            // Only show major airpoint.
            ValueStyle valueStyle1 = new ValueStyle();
            valueStyle1.ColumnName = "USE";
            valueStyle1.ValueItems.Add(new ValueItem("8", new PointStyle(new GeoImage(dataFolder + @"/Images/airport_small_size2.png"))));
            valueStyle1.ValueItems.Add(new ValueItem("22", new PointStyle(new GeoImage(dataFolder + @"/Images/airport_small_size2.png"))));
            valueStyle1.ValueItems.Add(new ValueItem("49", new PointStyle(new GeoImage(dataFolder + @"/Images/airport_small_size2.png"))));

            ValueStyle valueStyle2 = new ValueStyle();
            valueStyle2.ColumnName = "USE";
            valueStyle2.ValueItems.Add(new ValueItem("0", new PointStyle(new GeoImage(dataFolder + @"/Images/airport_small_size3.png"))));
            valueStyle2.ValueItems.Add(new ValueItem("8", new PointStyle(new GeoImage(dataFolder + @"/Images/airport_small_size4.png"))));
            valueStyle2.ValueItems.Add(new ValueItem("22", new PointStyle(new GeoImage(dataFolder + @"/Images/airport_small_size4.png"))));
            valueStyle2.ValueItems.Add(new ValueItem("49", new PointStyle(new GeoImage(dataFolder + @"/Images/airport_small_size4.png"))));
            valueStyle2.ValueItems.Add(new ValueItem("999", new PointStyle(new GeoImage(dataFolder + @"/Images/airport_small_size3.png"))));

            layer.ZoomLevelSet.ZoomLevel10.CustomStyles.Add(valueStyle1);
            layer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level12;

            layer.ZoomLevelSet.ZoomLevel13.CustomStyles.Add(valueStyle2);
            layer.ZoomLevelSet.ZoomLevel13.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(layer);
        }

        private void RenderLabelAirport(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Airport Label";

            TextStyle textStyle = TextStyles.CreateSimpleTextStyle("Nam", "Arial", 7, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 12, 0);

            layer.ZoomLevelSet.ZoomLevel13.DefaultTextStyle = textStyle;
            layer.ZoomLevelSet.ZoomLevel13.DefaultTextStyle.SuppressPartialLabels = true;
            layer.ZoomLevelSet.ZoomLevel13.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            layer.ZoomLevelSet.ZoomLevel13.DefaultTextStyle.GridSize = 40;
            layer.ZoomLevelSet.ZoomLevel13.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            layer.DrawingQuality = DrawingQuality.CanvasSettings;
            layer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(layer);
        }

        private void RenderSymbolPermanentriver(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Permanentriver";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel08.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(150, 153, 179, 204), 1F, false);
            Layer.ZoomLevelSet.ZoomLevel08.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level11;

            staticLayers.Add(Layer);
        }

        private void RenderSymbolPermanentriverMinusUSA(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Permanentriver MinusUSA";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel12.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(150, 153, 179, 204), 1F, false); 
            Layer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderSymbolFedlandBIA(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Bureau Of Indian Affairs";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel07.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(150, 197, 167, 148));
            Layer.ZoomLevelSet.ZoomLevel07.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelFedlandBIA(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "Bureau Of Indian Affairs Label";

            labelLayer.DrawingQuality = DrawingQuality.CanvasSettings;

            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Name1", "Arial", 7, DrawingFontStyles.Regular, GeoColor.StandardColors.Brown);
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.GridSize = 40;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.FittingPolygon = true;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.FittingPolygonFactor = 1.5;
            labelLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            labelLayer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(labelLayer);
        }

        private void RenderSymbolFedlandFWS(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Fish and Wild Life Service";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel07.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(150, 170, 213, 191));
            Layer.ZoomLevelSet.ZoomLevel07.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level11;

            Layer.ZoomLevelSet.ZoomLevel12.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(150, 170, 213, 191));
            Layer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelFedlandFWS(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "Fish and Wild Life Service Label";

            labelLayer.DrawingQuality = DrawingQuality.CanvasSettings;

            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Name1", "Arial", 7, DrawingFontStyles.Regular, GeoColor.FromArgb(255, 68, 138, 103));
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.GridSize = 40;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.FittingPolygon = true;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.FittingPolygonFactor = 1.5;
            labelLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            labelLayer.DrawingMarginInPixel = 250;
            dynamicLayers.Add(labelLayer);
        }

        private void RenderSymbolFedlandDOD(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Department Of Defense";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel07.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(200, GeoColor.StandardColors.DarkGray));
            Layer.ZoomLevelSet.ZoomLevel07.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level11;

            Layer.ZoomLevelSet.ZoomLevel12.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(200, GeoColor.StandardColors.DarkGray));
            Layer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderSymbolHighWayNetwork2_1b(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Primary Roads";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel09.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 239, 232, 119), 2, GeoColor.StandardColors.DarkGray, 4, true);
            Layer.ZoomLevelSet.ZoomLevel09.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level12;

            Layer.ZoomLevelSet.ZoomLevel13.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 239, 232, 119), 3, GeoColor.StandardColors.DarkGray, 6, true);
            Layer.ZoomLevelSet.ZoomLevel14.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 239, 232, 119), 5, GeoColor.StandardColors.DarkGray, 8, true);
            Layer.ZoomLevelSet.ZoomLevel15.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 239, 232, 119), 8, GeoColor.StandardColors.DarkGray, 11, true);

            staticLayers.Add(Layer);
        }

        private void RenderSymbolHighWayNetwork2_2(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Primary Roads";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            ValueStyle valueStyleFor09ZoomLevel = new ValueStyle();
            valueStyleFor09ZoomLevel.ColumnName = "Status";
            valueStyleFor09ZoomLevel.ValueItems.Add(new ValueItem("0", LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 239, 232, 119), 2, GeoColor.StandardColors.DarkGray, 4, true)));
            valueStyleFor09ZoomLevel.ValueItems.Add(new ValueItem("1", LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 239, 232, 119), 2, GeoColor.StandardColors.DarkGray, 4, true)));
            Layer.ZoomLevelSet.ZoomLevel09.CustomStyles.Add(valueStyleFor09ZoomLevel);
            Layer.ZoomLevelSet.ZoomLevel09.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level12;

            // Draw features based on values
            ValueStyle valueStyleFor13ZoomLevel = new ValueStyle();
            valueStyleFor13ZoomLevel.ColumnName = "Status";
            valueStyleFor13ZoomLevel.ValueItems.Add(new ValueItem("0", LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 239, 232, 119), 3, GeoColor.StandardColors.DarkGray, 6, true)));
            valueStyleFor13ZoomLevel.ValueItems.Add(new ValueItem("1", LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 239, 232, 119), 3, GeoColor.StandardColors.DarkGray, 6, true)));
            valueStyleFor13ZoomLevel.ValueItems.Add(new ValueItem("2", CreateSimpleLineStyle(GeoColor.FromArgb(255, 141, 164, 204), 1.75F, LineDashStyle.Dash, GeoColor.StandardColors.Transparent, 1, LineDashStyle.Solid, GeoColor.StandardColors.Transparent, 1, LineDashStyle.Solid, false)));
            Layer.ZoomLevelSet.ZoomLevel13.CustomStyles.Add(valueStyleFor13ZoomLevel);

            ValueStyle valueStyleFor14ZoomLevel = new ValueStyle();
            valueStyleFor14ZoomLevel.ColumnName = "Status";
            valueStyleFor14ZoomLevel.ValueItems.Add(new ValueItem("0", LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 239, 232, 119), 5, GeoColor.StandardColors.DarkGray, 8, true)));
            valueStyleFor14ZoomLevel.ValueItems.Add(new ValueItem("1", LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 239, 232, 119), 5, GeoColor.StandardColors.DarkGray, 8, true)));
            valueStyleFor14ZoomLevel.ValueItems.Add(new ValueItem("2", CreateSimpleLineStyle(GeoColor.FromArgb(255, 141, 164, 204), 1.75F, LineDashStyle.Dash, GeoColor.StandardColors.Transparent, 1, LineDashStyle.Solid, GeoColor.StandardColors.Transparent, 1, LineDashStyle.Solid, false)));
            Layer.ZoomLevelSet.ZoomLevel14.CustomStyles.Add(valueStyleFor14ZoomLevel);

            ValueStyle valueStyleFor15ZoomLevel = new ValueStyle();
            valueStyleFor15ZoomLevel.ColumnName = "Status";
            valueStyleFor15ZoomLevel.ValueItems.Add(new ValueItem("0", LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 239, 232, 119), 8, GeoColor.StandardColors.DarkGray, 11, true)));
            valueStyleFor15ZoomLevel.ValueItems.Add(new ValueItem("1", LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 239, 232, 119), 8, GeoColor.StandardColors.DarkGray, 11, true)));
            valueStyleFor15ZoomLevel.ValueItems.Add(new ValueItem("2", CreateSimpleLineStyle(GeoColor.FromArgb(255, 141, 164, 204), 1.75F, LineDashStyle.Dash, GeoColor.StandardColors.Transparent, 1, LineDashStyle.Solid, GeoColor.StandardColors.Transparent, 1, LineDashStyle.Solid, false)));
            Layer.ZoomLevelSet.ZoomLevel15.CustomStyles.Add(valueStyleFor15ZoomLevel);

            staticLayers.Add(Layer);
        }

        public LineStyle CreateSimpleLineStyle(GeoColor centerlineColor, float centerlineWidth, LineDashStyle centerlineDashStyle, GeoColor innerLineColor, float innerLineWidth, LineDashStyle innerLineDashStyle, GeoColor outerLineColor, float outerLineWidth, LineDashStyle outerLineDashStyle, bool roundCap)
        {
            GeoPen centerPen = new GeoPen(centerlineColor, centerlineWidth);
            centerPen.DashStyle = centerlineDashStyle;
            GeoPen innerPen = new GeoPen(innerLineColor, innerLineWidth);
            innerPen.DashStyle = innerLineDashStyle;
            GeoPen outerPen = new GeoPen(outerLineColor, outerLineWidth);
            outerPen.DashStyle = outerLineDashStyle;

            if (roundCap)
            {
                centerPen.StartCap = DrawingLineCap.Round;
                centerPen.EndCap = DrawingLineCap.Round;
                innerPen.StartCap = DrawingLineCap.Round;
                innerPen.EndCap = DrawingLineCap.Round;
                outerPen.StartCap = DrawingLineCap.Round;
                outerPen.EndCap = DrawingLineCap.Round;
            }

            return new LineStyle(outerPen, innerPen, centerPen);
        }

        private void RenderSymbolOcean(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Ocean";
            layer.DrawingQuality = DrawingQuality.HighSpeed;
            layer.ZoomLevelSet.ZoomLevel09.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.FromArgb(255, 153, 179, 204);
            layer.ZoomLevelSet.ZoomLevel09.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(layer);
        }


        private void RenderLabelFedlandDOD(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "Department Of Defense Label";

            labelLayer.DrawingQuality = DrawingQuality.CanvasSettings;

            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Name1", "Arial", 7, DrawingFontStyles.Regular, GeoColor.StandardColors.Black);
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.GridSize = 40;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.FittingPolygon = true;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.FittingPolygonFactor = 1.5;
            labelLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            labelLayer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(labelLayer);
        }


        private void RenderSymbolPrimaryBorder(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Primary Border";

            Layer.ZoomLevelSet.ZoomLevel04.DefaultLineStyle.OuterPen.DashStyle = LineDashStyle.Dash;
            Layer.ZoomLevelSet.ZoomLevel04.DefaultLineStyle.OuterPen.Width = 1;
            Layer.ZoomLevelSet.ZoomLevel04.DefaultLineStyle.OuterPen.Color = GeoColor.FromArgb(100, 156, 155, 154);
            Layer.ZoomLevelSet.ZoomLevel04.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level06;

            Layer.ZoomLevelSet.ZoomLevel07.DefaultLineStyle.OuterPen.DashStyle = LineDashStyle.Dash;
            Layer.ZoomLevelSet.ZoomLevel07.DefaultLineStyle.OuterPen.Width = 1.5f;
            Layer.ZoomLevelSet.ZoomLevel07.DefaultLineStyle.OuterPen.Color = GeoColor.FromArgb(150, 156, 155, 154);

            Layer.ZoomLevelSet.ZoomLevel08.DefaultLineStyle.OuterPen.DashStyle = LineDashStyle.Dash;
            Layer.ZoomLevelSet.ZoomLevel08.DefaultLineStyle.OuterPen.Width = 2;
            Layer.ZoomLevelSet.ZoomLevel08.DefaultLineStyle.OuterPen.Color = GeoColor.FromArgb(150, 156, 155, 154);

            Layer.ZoomLevelSet.ZoomLevel09.DefaultLineStyle.OuterPen.DashStyle = LineDashStyle.Dash;
            Layer.ZoomLevelSet.ZoomLevel09.DefaultLineStyle.OuterPen.Width = 2;
            Layer.ZoomLevelSet.ZoomLevel09.DefaultLineStyle.OuterPen.Color = GeoColor.FromArgb(255, 156, 155, 154);
            Layer.ZoomLevelSet.ZoomLevel09.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelStateText(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "StateText Label";

            labelLayer.DrawingQuality = DrawingQuality.CanvasSettings;

            labelLayer.ZoomLevelSet.ZoomLevel04.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Abrv", "Arial", 7, DrawingFontStyles.Bold, GeoColor.FromArgb(255, 91, 91, 91));
            labelLayer.ZoomLevelSet.ZoomLevel04.DefaultTextStyle.PointPlacement = PointPlacement.Center;
            labelLayer.ZoomLevelSet.ZoomLevel04.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            labelLayer.ZoomLevelSet.ZoomLevel04.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel04.DefaultTextStyle.GridSize = 40;
            labelLayer.ZoomLevelSet.ZoomLevel04.DefaultTextStyle.SuppressPartialLabels = true;

            labelLayer.ZoomLevelSet.ZoomLevel05.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Name", "Arial", 8, DrawingFontStyles.Bold, GeoColor.FromArgb(255, 91, 91, 91));
            labelLayer.ZoomLevelSet.ZoomLevel05.DefaultTextStyle.PointPlacement = PointPlacement.Center;
            labelLayer.ZoomLevelSet.ZoomLevel05.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            labelLayer.ZoomLevelSet.ZoomLevel05.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel05.DefaultTextStyle.GridSize = 40;
            labelLayer.ZoomLevelSet.ZoomLevel05.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel05.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level06;

            labelLayer.ZoomLevelSet.ZoomLevel07.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Name", "Arial", 10, DrawingFontStyles.Bold, GeoColor.FromArgb(255, 91, 91, 91));
            labelLayer.ZoomLevelSet.ZoomLevel07.DefaultTextStyle.PointPlacement = PointPlacement.Center;
            labelLayer.ZoomLevelSet.ZoomLevel07.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            labelLayer.ZoomLevelSet.ZoomLevel07.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel07.DefaultTextStyle.GridSize = 40;
            labelLayer.ZoomLevelSet.ZoomLevel07.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel07.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level12;
            labelLayer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(labelLayer);
        }

        private void RenderLabelProvinceText(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "Canada Province Label";

            labelLayer.DrawingQuality = DrawingQuality.CanvasSettings;

            labelLayer.ZoomLevelSet.ZoomLevel04.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("ABBR", "Arial", 7, DrawingFontStyles.Bold, GeoColor.FromArgb(255, 91, 91, 91));
            labelLayer.ZoomLevelSet.ZoomLevel04.DefaultTextStyle.PointPlacement = PointPlacement.Center;
            labelLayer.ZoomLevelSet.ZoomLevel04.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            labelLayer.ZoomLevelSet.ZoomLevel04.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel04.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel04.DefaultTextStyle.GridSize = 40;

            labelLayer.ZoomLevelSet.ZoomLevel05.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Name", "Arial", 8, DrawingFontStyles.Bold, GeoColor.FromArgb(255, 91, 91, 91));
            labelLayer.ZoomLevelSet.ZoomLevel05.DefaultTextStyle.PointPlacement = PointPlacement.Center;
            labelLayer.ZoomLevelSet.ZoomLevel05.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            labelLayer.ZoomLevelSet.ZoomLevel05.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel05.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel05.DefaultTextStyle.GridSize = 40;
            labelLayer.ZoomLevelSet.ZoomLevel05.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level12;
            labelLayer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(labelLayer);
        }

        private void RenderSymbolInnerWater(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Inner Water";

            Layer.DrawingQuality = DrawingQuality.HighSpeed;

            Layer.ZoomLevelSet.ZoomLevel07.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.FromArgb(255, 153, 179, 204);
            Layer.ZoomLevelSet.ZoomLevel07.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level11;

            staticLayers.Add(Layer);
        }

        private void RenderLabelInnerWater(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Inner Water Label";

            layer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle = new TextStyle("nam", new GeoFont("Arial", 6, DrawingFontStyles.Italic), new GeoSolidBrush(GeoColor.StandardColors.Navy));
            layer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            layer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            layer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.GridSize = 40;
            layer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.SuppressPartialLabels = true;
            layer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level11;
            layer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(layer);
        }

        private void RenderSymbolInnerWaterMinusUSA(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Inner Water Minus USA";

            Layer.DrawingQuality = DrawingQuality.HighSpeed;

            Layer.ZoomLevelSet.ZoomLevel12.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.FromArgb(255, 153, 179, 204);
            Layer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelInnerWaterMinusUSA(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Inner Water Minus USA Label";

            layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle = new TextStyle("nam", new GeoFont("Arial", 6, DrawingFontStyles.Italic), new GeoSolidBrush(GeoColor.StandardColors.Navy));
            layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.GridSize = 40;
            layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.SuppressPartialLabels = true;
            layer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(layer);
        }

        private void RenderSymbolInternationalborder(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "International Border";

            layer.ZoomLevelSet.ZoomLevel07.DefaultLineStyle.OuterPen.Color = new GeoColor(250, 156, 155, 154);
            layer.ZoomLevelSet.ZoomLevel07.DefaultLineStyle.OuterPen.Width = 2;
            layer.ZoomLevelSet.ZoomLevel07.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level11;

            staticLayers.Add(layer);
        }

        private void RenderSymbolInternationalborder_dtl(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "International Border";

            layer.ZoomLevelSet.ZoomLevel12.DefaultLineStyle.OuterPen.Color = new GeoColor(250, 156, 155, 154);
            layer.ZoomLevelSet.ZoomLevel12.DefaultLineStyle.OuterPen.Width = 2;
            layer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(layer);
        }

        private void RenderSymbolContestedborder(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Contested Border";

            layer.ZoomLevelSet.ZoomLevel07.DefaultLineStyle.OuterPen.Color = new GeoColor(250, 156, 155, 154);
            layer.ZoomLevelSet.ZoomLevel07.DefaultLineStyle.OuterPen.Width = 2;
            layer.ZoomLevelSet.ZoomLevel07.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(layer);
        }

        private void RenderSymbolRoad(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Road";

            layer.ZoomLevelSet.ZoomLevel08.DefaultLineStyle.OuterPen = new GeoPen(GeoColor.FromArgb(150, 228, 193, 73), 1.25f);

            staticLayers.Add(layer);
        }

        private void RenderSymbolRoadMinusUSACan(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Road Minus USA Can";

            layer.ZoomLevelSet.ZoomLevel09.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 239, 232, 119), 2f, GeoColor.StandardColors.LightGray, 3f, true);

            layer.ZoomLevelSet.ZoomLevel10.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 239, 232, 119), 2, GeoColor.StandardColors.DarkGray, 4, true);

            layer.ZoomLevelSet.ZoomLevel11.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 239, 232, 119), 3, GeoColor.StandardColors.DarkGray, 5, true);
            layer.ZoomLevelSet.ZoomLevel11.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level12;

            layer.ZoomLevelSet.ZoomLevel13.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 239, 232, 119), 4, GeoColor.StandardColors.DarkGray, 6, true);
            layer.ZoomLevelSet.ZoomLevel13.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(layer);
        }

        private void RenderLabelSettlement(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Settlement Label";

            TextStyle textStyle1 = TextStyles.CreateSimpleTextStyle("TXT", "Arial", 8, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2f, 0, 0);
            TextStyle textStyle2 = TextStyles.CreateSimpleTextStyle("TXT", "Arial", 9, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2f, 0, 0);

            layer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle = textStyle1;
            layer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            layer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.GridSize = 40;
            layer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.SuppressPartialLabels = true;
            layer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level11;

            layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle = textStyle2;
            layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.GridSize = 40;
            layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.SuppressPartialLabels = true;
            layer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layer.DrawingMarginInPixel = 250;
            dynamicLayers.Add(layer);
        }

        private void RenderSymbolUrbanareaMinusUSA(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Urban Area Minus USA";

            ValueStyle valueStyle = new ValueStyle();
            valueStyle.ColumnName = "Pop";

            layer.ZoomLevelSet.ZoomLevel09.CustomStyles.Add(new AreaStyle(new GeoSolidBrush(GeoColor.FromArgb(150, GeoColor.StandardColors.LightGray))));
            layer.ZoomLevelSet.ZoomLevel09.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(layer);
        }

        private void RenderLabelUrbanareaMinusUSA(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Urban Area Minus USA Label";

            TextStyle textStyle1 = TextStyles.CreateSimpleTextStyle("Nam", "Arial", 8, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2f, 0, 0); ;
            TextStyle textStyle2 = TextStyles.CreateSimpleTextStyle("Nam", "Arial", 9, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2f, 0, 0); ;

            textStyle1.BestPlacement = true;
            layer.ZoomLevelSet.ZoomLevel09.DefaultTextStyle = textStyle1;
            layer.ZoomLevelSet.ZoomLevel09.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            layer.ZoomLevelSet.ZoomLevel09.DefaultTextStyle.GridSize = 40;
            layer.ZoomLevelSet.ZoomLevel09.DefaultTextStyle.SuppressPartialLabels = true;
            layer.ZoomLevelSet.ZoomLevel09.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level11;

            layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle = textStyle2;
            layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.GridSize = 40;
            layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.SuppressPartialLabels = true;
            layer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layer.DrawingMarginInPixel = 250;
            dynamicLayers.Add(layer);
        }

        private void RenderLabelUrbanarea_dtl(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Urban Area USA Label";

            TextStyle textStyle1 = TextStyles.CreateSimpleTextStyle("Name", "Arial", 8, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2f, 0, 0);
            TextStyle textStyle2 = TextStyles.CreateSimpleTextStyle("Name", "Arial", 9, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2f, 0, 0);

            textStyle1.BestPlacement = true;
            layer.ZoomLevelSet.ZoomLevel09.DefaultTextStyle = textStyle1;
            layer.ZoomLevelSet.ZoomLevel09.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            layer.ZoomLevelSet.ZoomLevel09.DefaultTextStyle.GridSize = 40;
            layer.ZoomLevelSet.ZoomLevel09.DefaultTextStyle.SuppressPartialLabels = true;
            layer.ZoomLevelSet.ZoomLevel09.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level11;

            layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle = textStyle2;
            layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.GridSize = 40;
            layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.SuppressPartialLabels = true;
            layer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(layer);
        }

        private void RenderSymbolStateBorders(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "State Borders";

            Layer.ZoomLevelSet.ZoomLevel03.DefaultLineStyle.OuterPen.DashStyle = LineDashStyle.Dash;
            Layer.ZoomLevelSet.ZoomLevel03.DefaultLineStyle.OuterPen.Width = 1;
            Layer.ZoomLevelSet.ZoomLevel03.DefaultLineStyle.OuterPen.Color = GeoColor.FromArgb(100, 156, 155, 154);
            Layer.ZoomLevelSet.ZoomLevel03.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level06;

            Layer.ZoomLevelSet.ZoomLevel07.DefaultLineStyle.OuterPen.DashStyle = LineDashStyle.Dash;
            Layer.ZoomLevelSet.ZoomLevel07.DefaultLineStyle.OuterPen.Width = 1.5f;
            Layer.ZoomLevelSet.ZoomLevel07.DefaultLineStyle.OuterPen.Color = GeoColor.FromArgb(150, 156, 155, 154);

            Layer.ZoomLevelSet.ZoomLevel08.DefaultLineStyle.OuterPen.DashStyle = LineDashStyle.Dash;
            Layer.ZoomLevelSet.ZoomLevel08.DefaultLineStyle.OuterPen.Width = 2;
            Layer.ZoomLevelSet.ZoomLevel08.DefaultLineStyle.OuterPen.Color = GeoColor.FromArgb(150, 156, 155, 154);

            Layer.ZoomLevelSet.ZoomLevel09.DefaultLineStyle.OuterPen.DashStyle = LineDashStyle.Dash;
            Layer.ZoomLevelSet.ZoomLevel09.DefaultLineStyle.OuterPen.Width = 2;
            Layer.ZoomLevelSet.ZoomLevel09.DefaultLineStyle.OuterPen.Color = GeoColor.FromArgb(255, 156, 155, 154);
            Layer.ZoomLevelSet.ZoomLevel09.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level11;

            staticLayers.Add(Layer);
        }

        private void RenderSymbolProvinceBorders(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Province Borders";

            Layer.ZoomLevelSet.ZoomLevel03.DefaultLineStyle.OuterPen.DashStyle = LineDashStyle.Dash;
            Layer.ZoomLevelSet.ZoomLevel03.DefaultLineStyle.OuterPen.Width = 1;
            Layer.ZoomLevelSet.ZoomLevel03.DefaultLineStyle.OuterPen.Color = GeoColor.FromArgb(100, 156, 155, 154);
            Layer.ZoomLevelSet.ZoomLevel03.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level06;

            Layer.ZoomLevelSet.ZoomLevel07.DefaultLineStyle.OuterPen.DashStyle = LineDashStyle.Dash;
            Layer.ZoomLevelSet.ZoomLevel07.DefaultLineStyle.OuterPen.Width = 1.5f;
            Layer.ZoomLevelSet.ZoomLevel07.DefaultLineStyle.OuterPen.Color = GeoColor.FromArgb(150, 156, 155, 154);

            Layer.ZoomLevelSet.ZoomLevel08.DefaultLineStyle.OuterPen.DashStyle = LineDashStyle.Dash;
            Layer.ZoomLevelSet.ZoomLevel08.DefaultLineStyle.OuterPen.Width = 2;
            Layer.ZoomLevelSet.ZoomLevel08.DefaultLineStyle.OuterPen.Color = GeoColor.FromArgb(150, 156, 155, 154);

            Layer.ZoomLevelSet.ZoomLevel09.DefaultLineStyle.OuterPen.DashStyle = LineDashStyle.Dash;
            Layer.ZoomLevelSet.ZoomLevel09.DefaultLineStyle.OuterPen.Width = 2;
            Layer.ZoomLevelSet.ZoomLevel09.DefaultLineStyle.OuterPen.Color = GeoColor.FromArgb(255, 156, 155, 154);
            Layer.ZoomLevelSet.ZoomLevel09.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelHighWayNetwork2_1b(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "Primary Roads Label";

            labelLayer.DrawingQuality = DrawingQuality.CanvasSettings;
            IconValueStyle iconValueStyle = GetRoadIconStyle(GeoColor.StandardColors.Black);

            labelLayer.ZoomLevelSet.ZoomLevel10.CustomStyles.Add(iconValueStyle);
            labelLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level13;
            labelLayer.DrawingMarginInPixel = 250;


            TextStyle textStyle1 = TextStyles.CreateSimpleTextStyle("LNAME", "Arial", 6, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 0, -1);
            textStyle1.GridSize = 40;
            textStyle1.SuppressPartialLabels = true;
            textStyle1.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;

            labelLayer.ZoomLevelSet.ZoomLevel14.CustomStyles.Add(textStyle1);
            labelLayer.ZoomLevelSet.ZoomLevel14.CustomStyles.Add(iconValueStyle);
            labelLayer.ZoomLevelSet.ZoomLevel14.DefaultTextStyle.TextLineSegmentRatio = 10;
            labelLayer.ZoomLevelSet.ZoomLevel14.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;

            TextStyle textStyle2 = TextStyles.CreateSimpleTextStyle("LNAME", "Arial", 7, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 0, -1);
            textStyle2.GridSize = 40;
            textStyle2.SuppressPartialLabels = true;
            textStyle2.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;

            labelLayer.ZoomLevelSet.ZoomLevel15.CustomStyles.Add(textStyle2);
            labelLayer.ZoomLevelSet.ZoomLevel15.CustomStyles.Add(iconValueStyle);
            labelLayer.ZoomLevelSet.ZoomLevel15.DefaultTextStyle.TextLineSegmentRatio = 10;
            labelLayer.ZoomLevelSet.ZoomLevel15.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            labelLayer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(labelLayer);
        }

        private void RenderSymbolFedlandNPSclip(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "National Park Service";

            Layer.DrawingQuality = DrawingQuality.HighSpeed;

            Layer.ZoomLevelSet.ZoomLevel10.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 167, 204, 149));
            Layer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level11;

            Layer.ZoomLevelSet.ZoomLevel12.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 167, 204, 149));
            Layer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelFedlandNPSclip(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "National Park Service Label";

            labelLayer.DrawingQuality = DrawingQuality.CanvasSettings;

            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Name1", "Arial", 7, DrawingFontStyles.Regular, GeoColor.StandardColors.DarkGreen);
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.GridSize = 40;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.FittingPolygon = true;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.FittingPolygonFactor = 1.5;
            labelLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            labelLayer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(labelLayer);
        }

        private void RenderLabelHighWayNetwork2_2(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "Primary Roads Label";

            labelLayer.DrawingQuality = DrawingQuality.CanvasSettings;

            IconValueStyle iconValueStyle = GetRoadIconStyle(GeoColor.StandardColors.Black);

            labelLayer.ZoomLevelSet.ZoomLevel10.CustomStyles.Add(iconValueStyle);
            labelLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level13;

            labelLayer.ZoomLevelSet.ZoomLevel14.CustomStyles.Add(TextStyles.CreateSimpleTextStyle("LNAME", "Arial", 6, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 0, -1));
            labelLayer.ZoomLevelSet.ZoomLevel14.CustomStyles.Add(iconValueStyle);
            labelLayer.ZoomLevelSet.ZoomLevel14.DefaultTextStyle.TextLineSegmentRatio = 2;
            labelLayer.ZoomLevelSet.ZoomLevel14.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            labelLayer.ZoomLevelSet.ZoomLevel14.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel14.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel14.DefaultTextStyle.GridSize = 40;

            labelLayer.ZoomLevelSet.ZoomLevel15.CustomStyles.Add(TextStyles.CreateSimpleTextStyle("LNAME", "Arial", 7, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 0, -1));
            labelLayer.ZoomLevelSet.ZoomLevel15.CustomStyles.Add(iconValueStyle);
            labelLayer.ZoomLevelSet.ZoomLevel15.DefaultTextStyle.TextLineSegmentRatio = 2;
            labelLayer.ZoomLevelSet.ZoomLevel15.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel15.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel15.DefaultTextStyle.GridSize = 40;
            labelLayer.ZoomLevelSet.ZoomLevel15.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            labelLayer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(labelLayer);
        }

        private void RenderLabelCities_e(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "Small Towns Label";

            labelLayer.DrawingQuality = DrawingQuality.CanvasSettings;

            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("name", "Arial", 8, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2f, 0, 0);
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.PointPlacement = PointPlacement.Center;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.TextLineSegmentRatio = 2;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel10.DefaultTextStyle.GridSize = 40;

            labelLayer.ZoomLevelSet.ZoomLevel11.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("name", "Arial", 9, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2f, 0, 0);
            labelLayer.ZoomLevelSet.ZoomLevel11.DefaultTextStyle.PointPlacement = PointPlacement.Center;
            labelLayer.ZoomLevelSet.ZoomLevel11.DefaultTextStyle.TextLineSegmentRatio = 2;
            labelLayer.ZoomLevelSet.ZoomLevel11.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            labelLayer.ZoomLevelSet.ZoomLevel11.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel11.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel11.DefaultTextStyle.GridSize = 40;

            labelLayer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("name", "Arial", 10, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.StandardColors.White, 2f, 0, 0);
            labelLayer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.PointPlacement = PointPlacement.Center;
            labelLayer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            labelLayer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.TextLineSegmentRatio = 2;
            labelLayer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.GridSize = 40;
            labelLayer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level15;
            labelLayer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(labelLayer);
        }

        private void RenderSymbolUrban_dtl(ref WorldStreetsShapeFileFeatureLayer Layer)
        {

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel09.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(150, GeoColor.StandardColors.LightGray));
            Layer.ZoomLevelSet.ZoomLevel09.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderSymbolHighWayNetwork1(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Interstates";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel11.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 216, 171, 33), 4, GeoColor.StandardColors.DarkGray, 7, true);
            Layer.ZoomLevelSet.ZoomLevel11.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level12;

            Layer.ZoomLevelSet.ZoomLevel13.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 216, 171, 33), 6, GeoColor.StandardColors.DarkGray, 9, true);
            Layer.ZoomLevelSet.ZoomLevel13.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level14;
            Layer.ZoomLevelSet.ZoomLevel15.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 216, 171, 33), 8, GeoColor.StandardColors.DarkGray, 11, true);

            staticLayers.Add(Layer);
        }

        private void RenderLabelHighWayNetwork1(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "Interstates Label";

            labelLayer.DrawingQuality = DrawingQuality.CanvasSettings;

            labelLayer.ZoomLevelSet.ZoomLevel11.CustomStyles.Add(GetRoadIconStyle(GeoColor.StandardColors.White));
            labelLayer.ZoomLevelSet.ZoomLevel11.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level15;
            labelLayer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(labelLayer);
        }

        private void LoadStateSymbolLayers(string StateLayer)
        {
            if (StateLayer.Contains("ctycu"))
            {
                string[] folders = Directory.GetDirectories(dataFolder + "//" + StateLayer);
                foreach (string folder in folders)
                {
                    string[] files = Directory.GetFiles(folder, "*.shp");
                    foreach (string file in files)
                    {
                        WorldStreetsShapeFileFeatureLayer layer = new WorldStreetsShapeFileFeatureLayer(file);
                        layer.Name = "County";
                        layer.DrawingQuality = DrawingQuality.HighQuality;
                        layer.ZoomLevelSet.ZoomLevel12.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 242, 239, 233));
                        layer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                        staticLayers.Add(layer);
                    }
                }
            }
            else
            {
                string[] folders = Directory.GetDirectories(dataFolder + "//" + StateLayer);
                foreach (string folder in folders)
                {
                    string[] files = Directory.GetFiles(folder, "*.shp");
                    foreach (string file in files)
                    {
                        WorldStreetsShapeFileFeatureLayer layer = new WorldStreetsShapeFileFeatureLayer(file);
                        layer.Name = "Lakes/Rivers";
                        layer.DrawingQuality = DrawingQuality.HighSpeed;
                        layer.ZoomLevelSet.ZoomLevel12.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 153, 179, 204));
                        layer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                        staticLayers.Add(layer);
                    }
                }
            }
        }

        private void LoadStateLabelLayers(string StateLayer)
        {
            if (StateLayer.Contains("wat"))
            {
                string[] folders = Directory.GetDirectories(dataFolder + "//" + StateLayer);
                foreach (string folder in folders)
                {
                    string[] files = Directory.GetFiles(folder, "*.shp");
                    foreach (string file in files)
                    {
                        WorldStreetsShapeFileFeatureLayer layer = new WorldStreetsShapeFileFeatureLayer(file);
                        layer.Name = "Lakes/Rivers Label";
                        layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("LandName", "Arial", 7, DrawingFontStyles.Italic, GeoColor.StandardColors.Navy);
                        layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
                        layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.FittingPolygon = true;
                        layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.SuppressPartialLabels = true;
                        layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
                        layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.GridSize = 40;
                        layer.ZoomLevelSet.ZoomLevel12.DefaultTextStyle.TextLineSegmentRatio = 2;
                        layer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                        dynamicLayers.Add(layer);
                    }
                }
            }
        }

        private void RenderSymbolRailUSA(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Rail USA";

            layer.DrawingQuality = DrawingQuality.HighQuality;

            Collection<float> dashPattern1 = new Collection<float>();
            dashPattern1.Add(.25F);
            dashPattern1.Add(1f);
            LineStyle lineStyle1 = GetRailWayStyle(GeoColor.StandardColors.White, 1.5F, GeoColor.StandardColors.DarkGray, 2.8F, GeoColor.StandardColors.DarkGray, 4.5F, dashPattern1);
            Collection<float> dashPattern2 = new Collection<float>();
            dashPattern2.Add(0.25F);
            dashPattern2.Add(1);

            LineStyle lineStyle2 = GetRailWayStyle(GeoColor.StandardColors.White, 2, GeoColor.StandardColors.DarkGray, 4, GeoColor.StandardColors.DarkGray, 6, dashPattern2);
            layer.ZoomLevelSet.ZoomLevel13.DefaultLineStyle = lineStyle1;
            layer.ZoomLevelSet.ZoomLevel13.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level15;
            layer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = lineStyle2;
            layer.ZoomLevelSet.ZoomLevel16.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(layer);
        }

        private void RenderSymbolRailMinusUSA(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Rail Minus USA";

            layer.DrawingQuality = DrawingQuality.HighQuality;

            Collection<float> dashPattern1 = new Collection<float>();
            dashPattern1.Add(.25F);
            dashPattern1.Add(1f);
            LineStyle lineStyle1 = GetRailWayStyle(GeoColor.StandardColors.White, 1.5F, GeoColor.StandardColors.DarkGray, 2.8F, GeoColor.StandardColors.DarkGray, 4.5F, dashPattern1);
            Collection<float> dashPattern2 = new Collection<float>();
            dashPattern2.Add(0.25F);
            dashPattern2.Add(1);

            LineStyle lineStyle2 = GetRailWayStyle(GeoColor.StandardColors.White, 2, GeoColor.StandardColors.DarkGray, 4, GeoColor.StandardColors.DarkGray, 6, dashPattern2);
            layer.ZoomLevelSet.ZoomLevel13.DefaultLineStyle = lineStyle1;
            layer.ZoomLevelSet.ZoomLevel13.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level15;
            layer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = lineStyle2;
            layer.ZoomLevelSet.ZoomLevel16.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(layer);
        }

        private static LineStyle GetRailWayStyle(GeoColor innerPenColor, float innerPenWidth, GeoColor outerPenColor, float outerPenWidth, GeoColor centerPenColor, float centerPenWidth, Collection<float> dashPattern)
        {
            LineStyle lineStyle = new LineStyle();
            lineStyle.InnerPen = new GeoPen(innerPenColor, innerPenWidth);
            lineStyle.OuterPen = new GeoPen(outerPenColor, outerPenWidth);

            GeoPen centerPen = new GeoPen(centerPenColor, centerPenWidth);
            centerPen.DashPattern.Add(dashPattern[0]);
            centerPen.DashPattern.Add(dashPattern[1]);
            lineStyle.CenterPen = centerPen;

            return lineStyle;
        }

        private void LoadLowerRoadSymbolLayers()
        {
            string[] folders = Directory.GetDirectories(dataFolder + "//USA//tlka//");
            Collection<string> files70 = new Collection<string>();
            Collection<string> files60 = new Collection<string>();
            Collection<string> files50 = new Collection<string>();
            Collection<string> files40 = new Collection<string>();
            Collection<string> files30 = new Collection<string>();

            foreach (string folder in folders)
            {
                string[] files = Directory.GetFiles(folder, "*.shp");
                foreach (string file in files)
                {
                    switch (file.Substring(file.Length - 6, 2))
                    {
                        case "70": files70.Add(file); break;
                        case "60": files60.Add(file); break;
                        case "50": files50.Add(file); break;
                        case "40": files40.Add(file); break;
                        case "30": files30.Add(file); break;
                        default: break;
                    }
                }
            }

            foreach (string file in files70)
            {
                WorldStreetsShapeFileFeatureLayer layer = new WorldStreetsShapeFileFeatureLayer(file);
                layer.ShareCanvasWithPreviousLayer = true;
                RenderSymbolLkaA70(ref layer);
            }

            foreach (string file in files60)
            {
                WorldStreetsShapeFileFeatureLayer layer = new WorldStreetsShapeFileFeatureLayer(file);
                layer.ShareCanvasWithPreviousLayer = true;
                RenderSymbolLkaA60(ref layer);
            }

            foreach (string file in files50)
            {
                WorldStreetsShapeFileFeatureLayer layer = new WorldStreetsShapeFileFeatureLayer(file);
                layer.ShareCanvasWithPreviousLayer = true;
                RenderSymbolLkaA50(ref layer);
            }

            foreach (string file in files40)
            {
                WorldStreetsShapeFileFeatureLayer layer = new WorldStreetsShapeFileFeatureLayer(file);
                layer.ShareCanvasWithPreviousLayer = true;
                RenderSymbolLkaA40(ref layer);
            }

            foreach (string file in files30)
            {
                WorldStreetsShapeFileFeatureLayer layer = new WorldStreetsShapeFileFeatureLayer(file);
                layer.ShareCanvasWithPreviousLayer = true;
                RenderSymbolLkaA30(ref layer);
            }
        }

        private void LoadSymbolCanadaStreetLayers(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Canada Loacal Roads";

            layer.DrawingQuality = DrawingQuality.HighQuality;

            layer.DrawingQuality = DrawingQuality.HighQuality;

            layer.ZoomLevelSet.ZoomLevel14.DefaultLineStyle = LineStyles.LocalRoad4;
            layer.ZoomLevelSet.ZoomLevel15.DefaultLineStyle = LineStyles.LocalRoad3;
            layer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = LineStyles.LocalRoad2;

            layer.ZoomLevelSet.ZoomLevel17.DefaultLineStyle = LineStyles.LocalRoad1;
            layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(layer);
        }

        private void LoadPoiLptSymbolLayers()
        {
            string[] folders = Directory.GetDirectories(dataFolder + "//USA//LPT//");
            foreach (string folder in folders)
            {
                string[] files = Directory.GetFiles(folder, "*.shp");
                foreach (string file in files)
                {
                    WorldStreetsShapeFileFeatureLayer layer = new WorldStreetsShapeFileFeatureLayer(file);
                    switch (file.Substring(file.Length - 6, 2))
                    {
                        case "10": RenderSymbolLptD10(ref layer); break;
                        case "20": RenderSymbolLptD20(ref layer); break;
                        case "30": RenderSymbolLptD30(ref layer); break;
                        case "40": RenderSymbolLptD40(ref layer); break;
                        case "50": RenderSymbolLptD50(ref layer); break;
                        case "60": RenderSymbolLptD60(ref layer); break;
                        case "70": RenderSymbolLptD70(ref layer); break;
                        case "80": RenderSymbolLptD80(ref layer); break;
                        case "90": RenderSymbolLptD90(ref layer); break;
                        default:
                            break;
                    }
                }
            }
        }

        private void LoadPoiLptLabelLayers()
        {
            string[] folders = Directory.GetDirectories(dataFolder + "//USA//LPT//");
            foreach (string folder in folders)
            {
                string[] files = Directory.GetFiles(folder, "*.shp");
                foreach (string file in files)
                {
                    WorldStreetsShapeFileFeatureLayer layer = new WorldStreetsShapeFileFeatureLayer(file);
                    switch (file.Substring(file.Length - 6, 2))
                    {
                        case "10": RenderLabelLptD10(ref layer); break;
                        case "20": RenderLabelLptD20(ref layer); break;
                        case "30": RenderLabelLptD30(ref layer); break;
                        case "40": RenderLabelLptD40(ref layer); break;
                        case "50": RenderLabelLptD50(ref layer); break;
                        case "60": RenderLabelLptD60(ref layer); break;
                        case "70": RenderLabelLptD70(ref layer); break;
                        case "80": RenderLabelLptD80(ref layer); break;
                        case "90": RenderLabelLptD90(ref layer); break;

                        default:
                            break;
                    }
                }
            }
        }

        private void LoadHighRoadSymbolLayers()
        {
            string[] folders = Directory.GetDirectories(dataFolder + "//USA//tlka//");
            Collection<string> files10 = new Collection<string>();
            Collection<string> files20 = new Collection<string>();

            foreach (string folder in folders)
            {
                string[] files = Directory.GetFiles(folder, "*.shp");
                foreach (string file in files)
                {
                    switch (file.Substring(file.Length - 6, 2))
                    {
                        case "10": files10.Add(file); break;
                        case "20": files20.Add(file); break;
                        default: break;
                    }
                }
            }

            foreach (string file in files20)
            {
                WorldStreetsShapeFileFeatureLayer layer = new WorldStreetsShapeFileFeatureLayer(file);
                layer.ShareCanvasWithPreviousLayer = true;
                RenderSymbolLkaA20(ref layer);
            }

            foreach (string file in files10)
            {
                WorldStreetsShapeFileFeatureLayer layer = new WorldStreetsShapeFileFeatureLayer(file);
                layer.ShareCanvasWithPreviousLayer = true;
                RenderSymbolLkaA10(ref layer);
            }
        }

        private void LoadHighRoadLabelLayers()
        {
            string[] folders = Directory.GetDirectories(dataFolder + "//USA//tlka//");
            Collection<string> files10 = new Collection<string>();
            Collection<string> files20 = new Collection<string>();

            foreach (string folder in folders)
            {
                string[] files = Directory.GetFiles(folder, "*.shp");
                foreach (string file in files)
                {
                    switch (file.Substring(file.Length - 6, 2))
                    {
                        case "10": files10.Add(file); break;
                        case "20": files20.Add(file); break;
                        default: break;
                    }
                }
            }

            foreach (string file in files20)
            {
                WorldStreetsShapeFileFeatureLayer layer = new WorldStreetsShapeFileFeatureLayer(file);
                RenderLabelLkaA20(ref layer);
            }

            foreach (string file in files10)
            {
                WorldStreetsShapeFileFeatureLayer layer = new WorldStreetsShapeFileFeatureLayer(file);
                RenderLabelLkaA10(ref layer);
            }
        }

        private void LoadLabelCanadaStreetLayers(ref WorldStreetsShapeFileFeatureLayer layer)
        {
            layer.Name = "Canada Local Roads Label";
            layer.DrawingQuality = DrawingQuality.HighQuality;

            layer.ZoomLevelSet.ZoomLevel15.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("[NAME] [TYPE]", "Arial", 7.5f, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 0, -1);
            layer.ZoomLevelSet.ZoomLevel15.DefaultTextStyle.TextLineSegmentRatio = 2;
            layer.ZoomLevelSet.ZoomLevel15.DefaultTextStyle.GridSize = 40;
            layer.ZoomLevelSet.ZoomLevel15.DefaultTextStyle.SuppressPartialLabels = true;
            layer.ZoomLevelSet.ZoomLevel15.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            layer.ZoomLevelSet.ZoomLevel15.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level16;

            layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("[NAME] [TYPE]", "Arial", 8, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 0, -1);
            layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.TextLineSegmentRatio = 2;
            layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.GridSize = 40;
            layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.SuppressPartialLabels = true;
            layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(layer);
        }

        private void LoadLowerRoadLabelLayers()
        {
            string[] folders = Directory.GetDirectories(dataFolder + "//USA//tlka//");
            Collection<string> files70 = new Collection<string>();
            Collection<string> files60 = new Collection<string>();
            Collection<string> files50 = new Collection<string>();
            Collection<string> files40 = new Collection<string>();
            Collection<string> files30 = new Collection<string>();

            foreach (string folder in folders)
            {
                string[] files = Directory.GetFiles(folder, "*.shp");
                foreach (string file in files)
                {
                    switch (file.Substring(file.Length - 6, 2))
                    {
                        case "70": files70.Add(file); break;
                        case "60": files60.Add(file); break;
                        case "50": files50.Add(file); break;
                        case "40": files40.Add(file); break;
                        case "30": files30.Add(file); break;
                        default: break;
                    }
                }
            }

            foreach (string file in files70)
            {
                WorldStreetsShapeFileFeatureLayer layer = new WorldStreetsShapeFileFeatureLayer(file);
                RenderLabelLkaA70(ref layer);
            }

            foreach (string file in files60)
            {
                WorldStreetsShapeFileFeatureLayer layer = new WorldStreetsShapeFileFeatureLayer(file);
                RenderLabelLkaA60(ref layer);
            }

            foreach (string file in files50)
            {
                WorldStreetsShapeFileFeatureLayer layer = new WorldStreetsShapeFileFeatureLayer(file);
                RenderLabelLkaA50(ref layer);
            }

            foreach (string file in files40)
            {
                WorldStreetsShapeFileFeatureLayer layer = new WorldStreetsShapeFileFeatureLayer(file);
                RenderLabelLkaA40(ref layer);
            }

            foreach (string file in files30)
            {
                WorldStreetsShapeFileFeatureLayer layer = new WorldStreetsShapeFileFeatureLayer(file);
                RenderLabelLkaA30(ref layer);
            }
        }

        private void RenderSymbolLkaA10(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Highways (10)";

            Layer.DrawingQuality = DrawingQuality.HighQuality;
            Layer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 216, 171, 33), 10, GeoColor.StandardColors.DarkGray, 13, true);
            Layer.ZoomLevelSet.ZoomLevel16.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelLkaA10(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "Highways (10) Label";
            labelLayer.DrawingMarginInPixel = 200; // 80 DrawingMarginPercentage.
            labelLayer.DrawingQuality = DrawingQuality.CanvasSettings;

            labelLayer.ZoomLevelSet.ZoomLevel16.CustomStyles.Add(GetHighwayIconValueStyle());
            labelLayer.ZoomLevelSet.ZoomLevel16.CustomStyles.Add(GetHighwayValueStyle());
            labelLayer.ZoomLevelSet.ZoomLevel16.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            dynamicLayers.Add(labelLayer);
        }

        private void RenderSymbolLkaA20(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Major Roads (20)";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 255, 255, 128), 9, GeoColor.StandardColors.LightGray, 12, true);
            Layer.ZoomLevelSet.ZoomLevel17.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 255, 255, 128), 12, GeoColor.StandardColors.LightGray, 15, true);
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelLkaA20(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "Major Roads (20) Label";
            labelLayer.DrawingMarginInPixel = 200; // 80 DrawingMarginPercentage.
            labelLayer.DrawingQuality = DrawingQuality.CanvasSettings;

            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("[fedirp] [fename] [fetype] [fedirs]", "Arial", 9, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 0, -1);
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.TextLineSegmentRatio = 2;
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.GridSize = 40;
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SplineType = splineType;
            labelLayer.ZoomLevelSet.ZoomLevel16.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            dynamicLayers.Add(labelLayer);
        }

        private void RenderSymbolLkaA30(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Secondary Roads (30)";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 255, 255, 185), 8, GeoColor.StandardColors.LightGray, 11, true);
            Layer.ZoomLevelSet.ZoomLevel17.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 255, 255, 185), 11, GeoColor.StandardColors.LightGray, 14, true);
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelLkaA30(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "Secondary Roads (30) Label";
            labelLayer.DrawingMarginInPixel = 200; // 80 DrawingMarginPercentage.
            labelLayer.DrawingQuality = DrawingQuality.CanvasSettings;

            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("[fedirp] [fename] [fetype] [fedirs]", "Arial", 9, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 0, -1);
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.GridSize = 40;
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.TextLineSegmentRatio = 2;
            labelLayer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            labelLayer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            dynamicLayers.Add(labelLayer);
        }

        private void RenderSymbolLkaA40(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Local Roads (40)";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel14.DefaultLineStyle = LineStyles.LocalRoad4;
            Layer.ZoomLevelSet.ZoomLevel15.DefaultLineStyle = LineStyles.LocalRoad3;
            Layer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = LineStyles.LocalRoad2;

            Layer.ZoomLevelSet.ZoomLevel17.DefaultLineStyle = LineStyles.LocalRoad1;
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelLkaA40(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "Local Roads (40) Label";
            labelLayer.DrawingMarginInPixel = 200; // 80 DrawingMarginPercentage.
            labelLayer.ZoomLevelSet.ZoomLevel15.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("[fedirp] [fename] [fetype] [fedirs]", "Arial", 7.5f, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 0, -1);
            labelLayer.ZoomLevelSet.ZoomLevel15.DefaultTextStyle.TextLineSegmentRatio = 2;
            labelLayer.ZoomLevelSet.ZoomLevel15.DefaultTextStyle.GridSize = 40;
            labelLayer.ZoomLevelSet.ZoomLevel15.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel15.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel15.DefaultTextStyle.SplineType = splineType;
            labelLayer.ZoomLevelSet.ZoomLevel15.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            dynamicLayers.Add(labelLayer);
        }

        private void RenderSymbolLkaA50(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Vehicular Trail (50)";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel15.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.White, 2, GeoColor.StandardColors.DarkGray, 4, true);
            Layer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.White, 8, GeoColor.StandardColors.DarkGray, 10, true);
            Layer.ZoomLevelSet.ZoomLevel17.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.White, 9, GeoColor.StandardColors.DarkGray, 12, true);
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelLkaA50(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "Vehicular Trail (50) Label";
            labelLayer.DrawingMarginInPixel = 200; // 80 DrawingMarginPercentage.
            labelLayer.DrawingQuality = DrawingQuality.CanvasSettings;

            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("[fedirp] [fename] [fetype] [fedirs]", "Arial", 7.5f, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 0, -1);
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.TextLineSegmentRatio = 2;
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.GridSize = 40;
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SplineType = splineType;
            labelLayer.ZoomLevelSet.ZoomLevel16.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            dynamicLayers.Add(labelLayer);
        }

        private void RenderSymbolLkaA60(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Special Roads (60)";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.White, 8, GeoColor.StandardColors.DarkGray, 10, true);
            Layer.ZoomLevelSet.ZoomLevel17.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.White, 9, GeoColor.StandardColors.DarkGray, 12, true);
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelLkaA60(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "Special Roads (60) Label";
            labelLayer.DrawingMarginInPixel = 200; // 80 DrawingMarginPercentage.
            labelLayer.DrawingQuality = DrawingQuality.CanvasSettings;

            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("[fedirp] [fename] [fetype] [fedirs]", "Arial", 7.5f, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 0, -1);
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.TextLineSegmentRatio = 2;
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.GridSize = 40;
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SplineType = splineType;

            labelLayer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("[fedirp] [fename] [fetype] [fedirs]", "Arial", 8, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 0, -1);
            labelLayer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.TextLineSegmentRatio = 2;
            labelLayer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.GridSize = 40;
            labelLayer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.SplineType = splineType;
            labelLayer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            dynamicLayers.Add(labelLayer);
        }

        private void RenderSymbolLkaA70(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Other Roads (70)";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel16.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.White, 8, GeoColor.StandardColors.DarkGray, 10, true);
            Layer.ZoomLevelSet.ZoomLevel17.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.White, 9, GeoColor.StandardColors.DarkGray, 12, true);
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelLkaA70(ref WorldStreetsShapeFileFeatureLayer labelLayer)
        {
            labelLayer.Name = "Other Roads (70) Label";
            labelLayer.DrawingMarginInPixel = 200; // 80 DrawingMarginPercentage.
            labelLayer.DrawingQuality = DrawingQuality.CanvasSettings;

            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("[fedirp] [fename] [fetype] [fedirs]", "Arial", 7.5f, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 0, -1);
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.TextLineSegmentRatio = 2;
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.GridSize = 40;
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel16.DefaultTextStyle.SplineType = splineType;

            labelLayer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("[fedirp] [fename] [fetype] [fedirs]", "Arial", 8, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 0, -1);
            labelLayer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.TextLineSegmentRatio = 2;
            labelLayer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.GridSize = 40;
            labelLayer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.SuppressPartialLabels = true;
            labelLayer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            labelLayer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.SplineType = splineType;
            labelLayer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            dynamicLayers.Add(labelLayer);
        }

        #region Render Points Of Interest

        private void RenderSymbolLptD10(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Military installation or reservation";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel17.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.StandardColors.DarkOliveGreen, 9, GeoColor.StandardColors.DarkGray, 2);
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelLptD10(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Military installation or reservation Label";

            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Name", "Arial", 7, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 6, 6);
            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.BestPlacement = true;
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            dynamicLayers.Add(Layer);
        }

        private void RenderSymbolLptD20(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Multi-household or Transient Quarters";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel17.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.StandardColors.Aquamarine, 9, GeoColor.StandardColors.DarkGray, 2);
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelLptD20(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Multi-household or Transient Quarters Label";

            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Name", "Arial", 7, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 6, 6);
            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.BestPlacement = true;
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            dynamicLayers.Add(Layer);
        }

        private void RenderSymbolLptD30(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Custodial Facility";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel17.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.StandardColors.Beige, 9, GeoColor.StandardColors.DarkGray, 2);
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelLptD30(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Custodial Facility Label";

            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Name", "Arial", 7, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 6, 6);
            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.BestPlacement = true;
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            dynamicLayers.Add(Layer);
        }

        private void RenderSymbolLptD40(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Educational or Religious Institution";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel17.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.StandardColors.Red, 9, GeoColor.StandardColors.DarkGray, 2);
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelLptD40(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Educational or Religious Institution Label";

            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Name", "Arial", 7, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 6, 6);
            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.BestPlacement = true;
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            dynamicLayers.Add(Layer);
        }

        private void RenderSymbolLptD50(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Transportation Terminal";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel17.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.StandardColors.DarkOrange, 9, GeoColor.StandardColors.DarkGray, 2);
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelLptD50(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Transportation Terminal Label";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Name", "Arial", 7, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 6, 6);
            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.BestPlacement = true;
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            dynamicLayers.Add(Layer);
        }

        private void RenderSymbolLptD60(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Employment Center";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel17.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.StandardColors.DarkSalmon, 9, GeoColor.StandardColors.DarkGray, 2);
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelLptD60(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Employment Center Label";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Name", "Arial", 7, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 6, 6);
            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.BestPlacement = true;
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            dynamicLayers.Add(Layer);
        }

        private void RenderSymbolLptD70(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Tower";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel17.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.StandardColors.DarkTurquoise, 9, GeoColor.StandardColors.DarkGray, 2);
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelLptD70(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Tower Label";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Name", "Arial", 7, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 6, 6);
            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.BestPlacement = true;
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            dynamicLayers.Add(Layer);
        }

        private void RenderSymbolLptD80(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Open Space";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel17.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.StandardColors.LawnGreen, 9, GeoColor.StandardColors.DarkGray, 2);
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelLptD80(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Open Space Label";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Name", "Arial", 7, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 6, 6);
            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.BestPlacement = true;
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            dynamicLayers.Add(Layer);
        }

        private void RenderSymbolLptD90(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Special Purpose Landmark";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel17.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.StandardColors.Khaki, 9, GeoColor.StandardColors.DarkGray, 2);
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelLptD90(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Special Purpose Landmark Label";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Name", "Arial", 7, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 6, 6);
            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.BestPlacement = true;
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            dynamicLayers.Add(Layer);
        }

        private void RenderSymbolChurch(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Church";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel17.DefaultPointStyle = new PointStyle(new GeoImage(dataFolder + @"/Images/church.png")); 
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelChurch(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Church Label";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Name", "Arial", 8, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 5, 5);
            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            Layer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(Layer);
        }

        private void RenderSymbolGolf(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Golf";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel17.DefaultPointStyle = new PointStyle(new GeoImage(dataFolder + @"/Images/golf.png")); 
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelGolf(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Golf Label";

            Layer.DrawingQuality = DrawingQuality.HighQuality;

            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Name", "Arial", 8, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 5, 5);
            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            Layer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(Layer);
        }

        private void RenderSymbolHospital(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Hospital";

            Layer.DrawingQuality = DrawingQuality.HighQuality;
            Layer.ZoomLevelSet.ZoomLevel17.DefaultPointStyle = new PointStyle(new GeoImage(dataFolder + @"/Images/hospital_detailed.png")); 
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        private void RenderLabelHospital(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Hospital Label";

            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Name", "Arial", 8, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, 10, 10);
            Layer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            Layer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            Layer.DrawingMarginInPixel = 250;

            dynamicLayers.Add(Layer);
        }


        #endregion
        private IconValueStyle GetHighwayIconValueStyle()
        {
            IconValueStyle iconValueStyle = new IconValueStyle("SymbolType");

            IconValueItem item1 = new IconValueItem("R", dataFolder + @"/Images/ushwy-1.gif", new TextStyle("fename", new GeoFont("Arial", 8, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColor.StandardColors.Black)));
            item1.TextValueLengthMin = 1;
            item1.TextValueLengthMax = 5;

            IconValueItem item2 = new IconValueItem("I", dataFolder + @"/Images/Interstate_ThinOutline_Wide.gif", new TextStyle("fename", new GeoFont("Arial", 8, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColor.StandardColors.White)));
            item2.TextValueLengthMin = 1;
            item2.TextValueLengthMax = 5;

            iconValueStyle.IconValueItems.Add(item1);
            iconValueStyle.IconValueItems.Add(item2);

            iconValueStyle.GridSize = 200;
            iconValueStyle.TextLineSegmentRatio = 2;
            iconValueStyle.DuplicateRule = LabelDuplicateRule.NoDuplicateLabels;
            iconValueStyle.SuppressPartialLabels = true;
            iconValueStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;

            return iconValueStyle;
        }

        private static ValueStyle GetHighwayValueStyle()
        {
            ValueStyle valueStyle = new ValueStyle();
            valueStyle.ColumnName = "SymbolType";

            ValueItem item1 = new ValueItem("T", new TextStyle("fename", new GeoFont("Arial", 8, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColor.StandardColors.Black)));

            valueStyle.ValueItems.Add(item1);

            return valueStyle;
        }

        private void RenderSymbolLawrenceRiver(ref WorldStreetsShapeFileFeatureLayer Layer)
        {
            Layer.Name = "Lawrence River";

            Layer.DrawingQuality = DrawingQuality.HighSpeed;

            Layer.ZoomLevelSet.ZoomLevel12.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.FromArgb(255, 153, 179, 204);
            Layer.ZoomLevelSet.ZoomLevel12.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            staticLayers.Add(Layer);
        }

        protected override void DrawCore(GeoCanvas canvas, Collection<SimpleCandidate> labelsInAllLayers)
        {
            RectangleShape extent = canvas.CurrentWorldExtent;
            object image = canvas.NativeImage;
            GeographyUnit unit = canvas.MapUnit;
            bool isInDrawing = canvas.IsDrawing;
            if (isInDrawing)
            {
                canvas.EndDrawing();
            }

            Collection<string> candidateLayerNames = new Collection<string>();
            Dictionary<string, Collection<WorldStreetsShapeFileFeatureLayer>> groupStaticLayers = new Dictionary<string, Collection<WorldStreetsShapeFileFeatureLayer>>();

            if (rtreeSpatialIndex != null)
            {
                candidateLayerNames = rtreeSpatialIndex.GetFeatureIdsIntersectingBoundingBox(extent);
            }

            foreach (WorldStreetsShapeFileFeatureLayer layer in staticLayers)
            {
                if (candidateLayerNames.Count == 0 || candidateLayerNames.Contains(Path.GetFileName(layer.ShapePathFilename)))
                {
                    if (!layer.ShareCanvasWithPreviousLayer)
                    {
                        if (canvas.IsDrawing)
                        {
                            canvas.EndDrawing();
                        }
                        canvas.BeginDrawing(image, extent, unit);
                        layer.Open();
                        layer.Draw(canvas, labelsInAllLayers);
                        canvas.EndDrawing();
                    }
                    else
                    {
                        if (!canvas.IsDrawing)
                        {
                            canvas.BeginDrawing(image, extent, unit);
                        }
                        layer.Open();
                        layer.Draw(canvas, labelsInAllLayers);
                    }
                }
            }

            foreach (WorldStreetsShapeFileFeatureLayer layer in dynamicLayers)
            {
                if (candidateLayerNames.Count == 0 || candidateLayerNames.Contains(Path.GetFileName(layer.ShapePathFilename)))
                {
                    if (!layer.ShareCanvasWithPreviousLayer)
                    {
                        if (canvas.IsDrawing)
                        {
                            canvas.EndDrawing();
                        }
                        canvas.BeginDrawing(image, extent, unit);
                        layer.Open();
                        layer.Draw(canvas, labelsInAllLayers);
                        canvas.EndDrawing();
                    }
                    else
                    {
                        if (!canvas.IsDrawing)
                        {
                            canvas.BeginDrawing(image, extent, unit);
                        }
                        layer.Open();
                        layer.Draw(canvas, labelsInAllLayers);
                    }
                }
            }
            canvas.EndDrawing();

            if (isInDrawing)
            {
                canvas.BeginDrawing(image, extent, unit);
            }
        }

        protected override void OpenCore()
        {
            if (rtreeSpatialIndex != null)
            {
                rtreeSpatialIndex.Open();
            }
        }

        protected override void CloseCore()
        {
            if (rtreeSpatialIndex != null)
            {
                rtreeSpatialIndex.Close();
            }
        }
    }
}
