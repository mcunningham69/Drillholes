using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using Microsoft.VisualBasic;


namespace Drillholes.CreateDrillholes
{
    public class DesurveyDrillhole
    {
        DrillholeDesurveyValues _desurveyHoles;

        public DesurveyDrillhole(DrillholeDesurveyEnum value)
        {
            SetDrillholeType(value);
        }
        public void SetDrillholeType(DrillholeDesurveyEnum value)
        {
             _desurveyHoles = DrillholeDesurveyValues.createDesurvey(value);
        }

        public async Task<CollarDesurveyDto> VerticalTrace(ImportTableFields tableFields, List<XElement> drillholeValues)
        {
            return await _desurveyHoles.VerticalTrace(tableFields, drillholeValues);
        }
        public async Task<AssayDesurveyDto> AssayVerticalTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues)
        {
            return await _desurveyHoles.AssayVerticalTrace(collarTableFields, assayTableFields, drillholeValues);
        }

        public async Task<CollarDesurveyDto> CollarSurveyTrace(ImportTableFields tableFields, List<XElement> drillholeValues)
        {
            return await _desurveyHoles.CollarSurveyTrace(tableFields, drillholeValues);
        }

        public abstract class DrillholeDesurveyValues
        {
            public static DrillholeDesurveyValues createDesurvey(DrillholeDesurveyEnum value)
            {
                switch (value)
                {
                    //case DrillholeDesurveyEnum.AverageAngle:
                    //    {
                    //        return new AverageAngle();

                    //    }
                    //case DrillholeDesurveyEnum.BalancedTangential:
                    //    {
                    //        return new BalancedTangential();
                    //    }
                    //case DrillholeDesurveyEnum.MinimumCurvature:
                    //    {
                    //        return new BalancedTangential();
                    //    }
                    //case DrillholeDesurveyEnum.RadiusCurvature:
                    //    {
                    //        return new RadiusCurvature();
                    //    }
                    case DrillholeDesurveyEnum.Tangential:
                        {
                            return new Tangential();
                        }
                    default:
                        throw new Exception("Generic error with desurvey method");
                }


            }

            public abstract Task<CollarDesurveyDto> VerticalTrace(ImportTableFields tableFields, List<XElement> drillholeValues);
            public abstract Task<CollarDesurveyDto> CollarSurveyTrace(ImportTableFields tableFields, List<XElement> drillholeValues);

            public abstract Task<AssayDesurveyDto> AssayVerticalTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues);


        }

        public class AverageAngle//: DrillholeDesurveyValues
        {

        }

        public class BalancedTangential //: DrillholeDesurveyValues
        {

        }

        public class MinimumCurvature //: DrillholeDesurveyValues
        {

        }

        public class RadiusCurvature //: DrillholeDesurveyValues
        {

        }

        public class Tangential : DrillholeDesurveyValues
        {
            public override async Task<AssayDesurveyDto> AssayVerticalTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues)
            {
                AssayDesurveyDto assayDesurveyDto = new AssayDesurveyDto()
                {
                    id = new List<int>(),
                    bhid = new List<string>(),
                    mfrom = new List<double>(),
                    mto = new List<double>(),
                    length = new List<double>(),
                    isAssay = new List<bool>(),
                    desurveyType = DrillholeDesurveyEnum.Tangential,
                    surveyType = DrillholeSurveyType.vertical

                };

                //Need holeID, x, y, z, length => reference name in xml
                var collarHoleID = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var xField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.xName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var yField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.yName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var zField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.zName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var tdField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();

                var assayHoleID = assayTableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var mFromField = assayTableFields.Where(f => f.columnImportName == DrillholeConstants.distFromName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var mToField = assayTableFields.Where(f => f.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();

                XElement elements = drillholeValues[0];
                var collarElements = elements.Elements();

                XElement assElements = drillholeValues[2];
                var assayElements = assElements.Elements();

                var showElements = collarElements.Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE");

                assayDesurveyDto.Count = 0;

                foreach (XElement element in showElements)
                {
                    string hole = element.Element(assayHoleID).Value;
                    string mFrom = element.Element(mFromField).Value;
                    string mTo = element.Element(mToField).Value;
                    string zCoord = element.Element(zField).Value;
                    string totalDepth = element.Element(tdField).Value;
                    string holeAttr = element.Attribute("ID").Value;

                    bool bCheck = true;
                    if (hole != "")
                    {
                       // bCheck = await ValidateValues(xCoord, yCoord, zCoord, totalDepth, holeAttr, null, null);
                    }

                    if (bCheck)
                    {
                        //store collar
                        //collarDesurveyDto.id.Add(Convert.ToInt16(holeAttr));
                        //collarDesurveyDto.bhid.Add(hole);
                        //collarDesurveyDto.x.Add(Convert.ToDouble(xCoord));
                        //collarDesurveyDto.y.Add(Convert.ToDouble(yCoord));
                        //collarDesurveyDto.z.Add(Convert.ToDouble(zCoord));
                        //collarDesurveyDto.length.Add(Convert.ToDouble(totalDepth));
                        //collarDesurveyDto.isCollar.Add(true);

                        ////store toe
                        //double depth = Convert.ToDouble(totalDepth);
                        //double zcoord = Convert.ToDouble(zCoord);

                        //double newZ = zcoord - depth;

                        //collarDesurveyDto.id.Add(Convert.ToInt16(holeAttr));
                        //collarDesurveyDto.bhid.Add(hole);
                        //collarDesurveyDto.x.Add(Convert.ToDouble(xCoord));
                        //collarDesurveyDto.y.Add(Convert.ToDouble(yCoord));
                        //collarDesurveyDto.z.Add(Convert.ToDouble(newZ));

                        //collarDesurveyDto.length.Add(Convert.ToDouble(depth));
                        //collarDesurveyDto.isCollar.Add(false);

                        //collarDesurveyDto.Count++;
                    }

                }

                return assayDesurveyDto;
            }

            public override async Task<CollarDesurveyDto> CollarSurveyTrace(ImportTableFields tableFields, List<XElement> drillholeValues)
            {
                CollarDesurveyDto collarDesurveyDto = new CollarDesurveyDto()
                {
                    id = new List<int>(),
                    bhid = new List<string>(),
                    x = new List<double>(),
                    y = new List<double>(),
                    z = new List<double>(),
                    length = new List<double>(),
                    azimuth = new List<double>(),
                    dip = new List<double>(),
                    isCollar = new List<bool>()
                };

                //Need holeID, x, y, z, length => reference name in xml
                var holeId = tableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var xField = tableFields.Where(f => f.columnImportName == DrillholeConstants.xName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var yField = tableFields.Where(f => f.columnImportName == DrillholeConstants.yName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var zField = tableFields.Where(f => f.columnImportName == DrillholeConstants.zName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var tdField = tableFields.Where(f => f.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var dipField = tableFields.Where(f => f.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var azimuthField = tableFields.Where(f => f.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();

                XElement elements = drillholeValues[0];
                var collarElements = elements.Elements();

                var showElements = collarElements.Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE");

                foreach (XElement element in showElements)
                {
                    string colID = element.Attribute("ID").Value;
                    string hole = element.Element(holeId).Value;
                    string xCoord = element.Element(xField).Value;
                    string yCoord = element.Element(yField).Value;
                    string zCoord = element.Element(zField).Value;
                    string totalDepth = element.Element(tdField).Value;
                    string dip = element.Attribute(dipField).Value;
                    string azimuth = element.Attribute(azimuthField).Value;

                    if (hole != "")
                    {
                        bool bCheck = await ValidateValues(xCoord, yCoord, zCoord, totalDepth, colID, dip, azimuth);

                        if (!bCheck)
                            break;
                    }

                    double dblX = Convert.ToDouble(xCoord);
                    double dblY = Convert.ToDouble(yCoord);
                    double dblZ = Convert.ToDouble(zCoord);
                    double dblDistance = Convert.ToDouble(tdField);
                    double dblDip = Convert.ToDouble(dip);
                    double dblAzimuth = Convert.ToDouble(azimuth);


                    //store collar
                    collarDesurveyDto.id.Add(Convert.ToInt16(colID));
                    collarDesurveyDto.bhid.Add(hole);
                    collarDesurveyDto.x.Add(dblX);
                    collarDesurveyDto.y.Add(dblY);
                    collarDesurveyDto.z.Add(dblZ);
                    collarDesurveyDto.length.Add(dblDistance);
                    collarDesurveyDto.dip.Add(dblDip);
                    collarDesurveyDto.azimuth.Add(dblAzimuth);
                    collarDesurveyDto.isCollar.Add(false);

                    //store toe
                    Coordinate3D coordinate = await CalculateSurveys.ReturnCoordinateTangential(dblX, dblY, dblZ, dblDistance, dblAzimuth, dblDip);

                    //check not null
                    if (coordinate == null)
                        return null;

                    collarDesurveyDto.id.Add(Convert.ToInt16(colID));
                    collarDesurveyDto.bhid.Add(hole);
                    collarDesurveyDto.x.Add(coordinate.x);
                    collarDesurveyDto.y.Add(coordinate.y);
                    collarDesurveyDto.z.Add(coordinate.z);
                    collarDesurveyDto.length.Add(dblDistance);
                    collarDesurveyDto.dip.Add(dblDip);
                    collarDesurveyDto.azimuth.Add(dblAzimuth);
                    collarDesurveyDto.isCollar.Add(false);

                }

                return collarDesurveyDto;
            }

            public override async Task<CollarDesurveyDto> VerticalTrace(ImportTableFields tableFields, List<XElement> drillholeValues)
            {
                CollarDesurveyDto collarDesurveyDto = new CollarDesurveyDto()
                {
                    id = new List<int>(),
                    bhid = new List<string>(),
                    x = new List<double>(),
                    y = new List<double>(),
                    z = new List<double>(),
                    length = new List<double>(),
                    isCollar = new List<bool>(),
                    desurveyType = DrillholeDesurveyEnum.Tangential,
                    surveyType = DrillholeSurveyType.vertical

            };

                //Need holeID, x, y, z, length => reference name in xml
                var holeId = tableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var xField = tableFields.Where(f => f.columnImportName == DrillholeConstants.xName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var yField = tableFields.Where(f => f.columnImportName == DrillholeConstants.yName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var zField = tableFields.Where(f => f.columnImportName == DrillholeConstants.zName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var tdField = tableFields.Where(f => f.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();

                XElement elements = drillholeValues[0];
                var collarElements = elements.Elements();

                var showElements = collarElements.Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE");

                collarDesurveyDto.Count = 0;

                foreach (XElement element in showElements)
                {
                    string hole = element.Element(holeId).Value;
                    string xCoord = element.Element(xField).Value;
                    string yCoord = element.Element(yField).Value;
                    string zCoord = element.Element(zField).Value;
                    string totalDepth = element.Element(tdField).Value;
                    string holeAttr = element.Attribute("ID").Value;

                    bool bCheck = true;
                    if (hole != "")
                    {
                        bCheck = await ValidateValues(xCoord, yCoord, zCoord, totalDepth, holeAttr, null, null);
                    }

                    if (bCheck)
                    {
                        //store collar
                        collarDesurveyDto.id.Add(Convert.ToInt16(holeAttr));
                        collarDesurveyDto.bhid.Add(hole);
                        collarDesurveyDto.x.Add(Convert.ToDouble(xCoord));
                        collarDesurveyDto.y.Add(Convert.ToDouble(yCoord));
                        collarDesurveyDto.z.Add(Convert.ToDouble(zCoord));
                        collarDesurveyDto.length.Add(Convert.ToDouble(totalDepth));
                        collarDesurveyDto.isCollar.Add(true);

                        //store toe
                        double depth = Convert.ToDouble(totalDepth);
                        double zcoord = Convert.ToDouble(zCoord);

                        double newZ = zcoord - depth;

                        collarDesurveyDto.id.Add(Convert.ToInt16(holeAttr));
                        collarDesurveyDto.bhid.Add(hole);
                        collarDesurveyDto.x.Add(Convert.ToDouble(xCoord));
                        collarDesurveyDto.y.Add(Convert.ToDouble(yCoord));
                        collarDesurveyDto.z.Add(Convert.ToDouble(newZ));

                        collarDesurveyDto.length.Add(Convert.ToDouble(depth));
                        collarDesurveyDto.isCollar.Add(false);

                        collarDesurveyDto.Count++;
                    }

                }

                return collarDesurveyDto;
            }

            private async Task<bool> ValidateValues(string x, string y, string z, string td, string holeID, string dip, string azi)
            {
                if (!Information.IsNumeric(x))
                    return false;

                if (!Information.IsNumeric(y))
                    return false;

                if (!Information.IsNumeric(z))
                    return false;
                if (!Information.IsNumeric(td))
                    return false;
                if (!Information.IsNumeric(holeID))
                    return false;

                if (dip != null)
                {
                    if (!Information.IsNumeric(dip))
                        return false;

                    if (!Information.IsNumeric(azi))
                        return false;
                }

                return true;
            }
        }
    }
}
