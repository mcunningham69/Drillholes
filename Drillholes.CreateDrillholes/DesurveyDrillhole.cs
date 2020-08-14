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
        public async Task<AssayDesurveyDto> AssayVerticalTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe)
        {
            return await _desurveyHoles.AssayVerticalTrace(collarTableFields, assayTableFields, drillholeValues, bToe);
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

            public abstract Task<AssayDesurveyDto> AssayVerticalTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe);


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
            public override async Task<AssayDesurveyDto> AssayVerticalTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe)
            {
                //initialise object and set up lists
                AssayDesurveyDto assayDesurveyDto = new AssayDesurveyDto()
                {
                    //id = new List<int>(),
                    //bhid = new List<string>(),
                    //distFrom = new List<double>(),
                    //distTo = new List<double>(),
                    //x = new List<double>(),
                    //y = new List<double>(),
                    //z = new List<double>(),
                    //length = new List<double>(),
                    //isAssay = new List<bool>(),
                    desurveyType = DrillholeDesurveyEnum.Tangential,
                    surveyType = DrillholeSurveyType.vertical

                };

                //Need holeID, x, y, z, length => reference name in xml
                var collarHoleID = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var xField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.xName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var yField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.yName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var zField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.zName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var tdField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();

                //TODO - add assay fields 
                var assayHoleID = assayTableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var mFromField = assayTableFields.Where(f => f.columnImportName == DrillholeConstants.distFromName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var mToField = assayTableFields.Where(f => f.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();

                XElement elements = drillholeValues[0];
                var collarElements = elements.Elements();

                XElement assElements = drillholeValues[2];
                var assayElements = assElements.Elements();

                //return collar coordiantes and length for all holes which are not flagged to be ignored
                var showElements = collarElements.Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE");

                foreach (XElement element in showElements)
                {
                    //get the values from XML
                    string holeAttr = element.Attribute("ID").Value;
                    string hole = element.Element(collarHoleID).Value;
                    string xCoord = element.Element(xField).Value;
                    string yCoord = element.Element(yField).Value;
                    string zCoord = element.Element(zField).Value;
                    string totalDepth = element.Element(tdField).Value; //FOR NOW< IGNORE

                    //check values are number otherwise go to next hole
                    bool bCheck = await ValidateValues(xCoord, yCoord, zCoord, totalDepth, holeAttr, null, null, null, null);

                    if (bCheck)
                    {
                        double dblX = 0.0, dblY = 0.0, dblZ = 0.0, dblLength = 0.0, dblFrom = 0.0, dblTo = 0.0, dblDistance = 0.0, dblNewZ; //Vertical so only z will change

                        var assayHole = assayElements.Where(h => h.Element(assayHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").ToList(); //get all samples for hole and ignroe those flagged

                        int counter = 0;
                        foreach (var assayValue in assayHole) //loop through each sample interval
                        {
                            string assayHoleAttr = assayValue.Attribute("ID").Value;
                            string mFrom = assayValue.Element(mFromField).Value;
                            string mTo = assayValue.Element(mToField).Value;

                            bool bCheckDistance = true;

                            if (counter == 0) //always set as condition above shows collar coordiantes are valid
                            {
                                dblX = Convert.ToDouble(xCoord);
                                dblY = Convert.ToDouble(yCoord);
                                dblZ = Convert.ToDouble(zCoord);
                                dblLength = Convert.ToDouble(totalDepth);

                                //create collar => future will have a choice to create or not
                                assayDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
                                assayDesurveyDto.assayId.Add(Convert.ToInt16(assayHoleAttr));
                                assayDesurveyDto.bhid.Add(hole);
                                assayDesurveyDto.x.Add(Convert.ToDouble(xCoord));
                                assayDesurveyDto.y.Add(Convert.ToDouble(yCoord));
                                assayDesurveyDto.z.Add(Convert.ToDouble(dblZ));
                                assayDesurveyDto.distFrom.Add(0);
                                assayDesurveyDto.distTo.Add(0);
                                assayDesurveyDto.dip.Add(-90.0);
                                assayDesurveyDto.azimuth.Add(0.0);
                                assayDesurveyDto.length.Add(Convert.ToDouble(dblLength)); //use total hole length for collar
                                assayDesurveyDto.isAssay.Add(false);

                                bCheckDistance = await ValidateValues(null, null, null, null, assayHoleAttr, null, null, mFrom, mTo);

                                if (bCheckDistance)
                                {
                                    //create first sample record
                                    dblFrom = Convert.ToDouble(mFrom);
                                    dblTo = Convert.ToDouble(mTo);

                                    dblDistance = dblTo - dblFrom;

                                    dblNewZ = dblZ - dblDistance;

                                    assayDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
                                    assayDesurveyDto.assayId.Add(Convert.ToInt16(assayHoleAttr));
                                    assayDesurveyDto.bhid.Add(hole);
                                    assayDesurveyDto.x.Add(Convert.ToDouble(xCoord));
                                    assayDesurveyDto.y.Add(Convert.ToDouble(yCoord));
                                    assayDesurveyDto.z.Add(dblNewZ);
                                    assayDesurveyDto.distFrom.Add(dblFrom);
                                    assayDesurveyDto.distTo.Add(dblTo);
                                    assayDesurveyDto.dip.Add(-90.0);
                                    assayDesurveyDto.azimuth.Add(0.0);
                                    assayDesurveyDto.length.Add(dblDistance); //use total hole length for collar
                                    assayDesurveyDto.isAssay.Add(true);
                                }
                            }
                            else
                            {
                                bCheckDistance = await ValidateValues(null, null, null, null, assayHoleAttr, null, null, mFrom, mTo);

                                if (bCheckDistance)
                                {
                                    dblFrom = Convert.ToDouble(mFrom);
                                    dblTo = Convert.ToDouble(mTo);

                                    dblDistance = dblTo - dblFrom;

                                    dblNewZ = dblZ - dblTo; //take from surface in case a record has been ignored

                                    assayDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
                                    assayDesurveyDto.assayId.Add(Convert.ToInt16(assayHoleAttr));
                                    assayDesurveyDto.bhid.Add(hole);
                                    assayDesurveyDto.x.Add(Convert.ToDouble(xCoord));
                                    assayDesurveyDto.y.Add(Convert.ToDouble(yCoord));
                                    assayDesurveyDto.z.Add(Convert.ToDouble(dblNewZ));
                                    assayDesurveyDto.distFrom.Add(dblFrom);
                                    assayDesurveyDto.distTo.Add(dblTo);
                                    assayDesurveyDto.dip.Add(-90.0);
                                    assayDesurveyDto.azimuth.Add(0.0);
                                    assayDesurveyDto.length.Add(Convert.ToDouble(dblDistance));
                                    assayDesurveyDto.Count++;
                                    assayDesurveyDto.isAssay.Add(true);
                                }

                            }

                            counter++;
                        }

                        //check to add TD
                        if (bToe)
                        {
                            if (dblLength > dblTo)
                            {
                                dblDistance = dblLength - dblTo;

                                dblNewZ = dblZ - dblLength;

                                int value = assayDesurveyDto.assayId.Last();
                                assayDesurveyDto.assayId.Add(value);
                                assayDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
                                assayDesurveyDto.bhid.Add(hole);
                                assayDesurveyDto.x.Add(Convert.ToDouble(xCoord));
                                assayDesurveyDto.y.Add(Convert.ToDouble(yCoord));
                                assayDesurveyDto.z.Add(Convert.ToDouble(dblZ));
                                assayDesurveyDto.length.Add(Convert.ToDouble(dblDistance));
                                assayDesurveyDto.Count++;
                                assayDesurveyDto.isAssay.Add(false);
                            }
                        }
                    }

                }

                return assayDesurveyDto;
            }

            public override async Task<CollarDesurveyDto> CollarSurveyTrace(ImportTableFields tableFields, List<XElement> drillholeValues)
            {
                CollarDesurveyDto collarDesurveyDto = new CollarDesurveyDto()
                {
                    //colId = new List<int>(),
                    //bhid = new List<string>(),
                    //x = new List<double>(),
                    //y = new List<double>(),
                    //z = new List<double>(),
                    //length = new List<double>(),
                    //azimuth = new List<double>(),
                    //dip = new List<double>(),
                    //isCollar = new List<bool>()
                    desurveyType = DrillholeDesurveyEnum.Tangential,
                    surveyType = DrillholeSurveyType.collarsurvey
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
                        bool bCheck = await ValidateValues(xCoord, yCoord, zCoord, totalDepth, colID, dip, azimuth, null,null);

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
                    collarDesurveyDto.colId.Add(Convert.ToInt16(colID));
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

                    collarDesurveyDto.colId.Add(Convert.ToInt16(colID));
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
                    //id = new List<int>(),
                    //bhid = new List<string>(),
                    //x = new List<double>(),
                    //y = new List<double>(),
                    //z = new List<double>(),
                    //length = new List<double>(),
                    //isCollar = new List<bool>(),
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
                        bCheck = await ValidateValues(xCoord, yCoord, zCoord, totalDepth, holeAttr, null, null, null, null);
                    }

                    if (bCheck)
                    {
                        //store collar
                        collarDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
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

                        collarDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
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

            private async Task<bool> ValidateValues(string x, string y, string z, string td, string holeID, string dip, string azi, string distance, string distanceTo)
            {
                if (x != null)
                {
                    if (!Information.IsNumeric(x))
                        return false;

                    if (!Information.IsNumeric(y))
                        return false;

                    if (!Information.IsNumeric(z))
                        return false;
                    if (!Information.IsNumeric(td))
                        return false;
                }

                if (!Information.IsNumeric(holeID))
                    return false;


                if (dip != null)
                {
                    if (!Information.IsNumeric(dip))
                        return false;

                    if (!Information.IsNumeric(azi))
                        return false;
                }

                if (distance != null)
                {
                    if (!Information.IsNumeric(distance))
                        return false;

                    if (distanceTo != null)
                    {
                        if (!Information.IsNumeric(distanceTo))
                            return false;
                    }
                }

                return true;
            }
        }
    }
}
