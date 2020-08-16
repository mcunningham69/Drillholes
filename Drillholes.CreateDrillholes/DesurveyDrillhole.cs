using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using Drillholes.Domain.Exceptions;
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
        public async Task<CollarDesurveyDto> CollarSurveyTrace(ImportTableFields tableFields, List<XElement> drillholeValues)
        {
            return await _desurveyHoles.CollarSurveyTrace(tableFields, drillholeValues);
        }

        public async Task<SurveyDesurveyDto> SurveyDownholeTrace(ImportTableFields collarTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues)
        {
            return await _desurveyHoles.SurveyDownhole(collarTableFields, surveyTableFields, drillholeValues);
        }

        public async Task<AssayDesurveyDto> AssayVerticalTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            return await _desurveyHoles.AssayVerticalTrace(collarTableFields, assayTableFields, drillholeValues, bToe, bCollar);
        }
        public async Task<AssayDesurveyDto> AssayCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            return await _desurveyHoles.AssayCollarSurveyTrace(collarTableFields, assayTableFields, drillholeValues, bToe, bCollar);
        }
        public async Task<AssayDesurveyDto> AssayDownholeTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            return await _desurveyHoles.AssayDownholeTrace(collarTableFields, assayTableFields, surveyTableFields, drillholeValues, bToe, bCollar);
        }

        public async Task<IntervalDesurveyDto> IntervalVerticalTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            return await _desurveyHoles.IntervalVerticalTrace(collarTableFields, intervalTableFields, drillholeValues, bToe, bCollar);
        }
        public async Task<IntervalDesurveyDto> IntervalCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            return await _desurveyHoles.IntervalCollarSurveyTrace(collarTableFields, intervalTableFields, drillholeValues, bToe, bCollar);
        }
        public async Task<IntervalDesurveyDto> IntervalDownholeTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            return await _desurveyHoles.IntervalDownholeTrace(collarTableFields, intervalTableFields,surveyTableFields, drillholeValues, bToe, bCollar);
        }

        public async Task<ContinuousDesurveyDto> ContinuousVerticalTrace(ImportTableFields collarTableFields, ImportTableFields contTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            return await _desurveyHoles.ContinuousVerticalTrace(collarTableFields, contTableFields, drillholeValues, bToe, bCollar);
        }
        public async Task<ContinuousDesurveyDto> ContinuousCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields contTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            return await _desurveyHoles.ContinuousCollarSurveyTrace(collarTableFields, contTableFields, drillholeValues, bToe, bCollar);
        }
        public async Task<ContinuousDesurveyDto> ContinuousDownholeTrace(ImportTableFields collarTableFields, ImportTableFields contTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            return await _desurveyHoles.ContinuousDownholeTrace(collarTableFields, contTableFields, surveyTableFields, drillholeValues, bToe, bCollar);
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

            public abstract Task<SurveyDesurveyDto> SurveyDownhole(ImportTableFields collarTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues);

            public abstract Task<AssayDesurveyDto> AssayVerticalTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar);
            public abstract Task<AssayDesurveyDto> AssayCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar);
            public abstract Task<AssayDesurveyDto> AssayDownholeTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar);

            public abstract Task<IntervalDesurveyDto> IntervalVerticalTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar);
            public abstract Task<IntervalDesurveyDto> IntervalCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar);
            public abstract Task<IntervalDesurveyDto> IntervalDownholeTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar);


            public abstract Task<ContinuousDesurveyDto> ContinuousVerticalTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar);
            public abstract Task<ContinuousDesurveyDto> ContinuousCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar);
            public abstract Task<ContinuousDesurveyDto> ContinuousDownholeTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar);



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
            public override Task<AssayDesurveyDto> AssayDownholeTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
            {
                throw new NotImplementedException();
            }

            public override async Task<AssayDesurveyDto> AssayCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
            {
                AssayDesurveyDto assayDesurveyDto = new AssayDesurveyDto()
                {
                    desurveyType = DrillholeDesurveyEnum.Tangential,
                    surveyType = DrillholeSurveyType.collarsurvey
                };

                #region fieldnames
                //Need holeID, x, y, z, length => reference name in xml
                var collarHoleID = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var xField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.xName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var yField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.yName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var zField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.zName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var tdField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var dipField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var azimuthField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();

                if (dipField == null || azimuthField == null)
                {
                    throw new CollarException("Check Dip and Azimuth fields");
                }

                //TODO - add assay fields 
                var assayHoleID = assayTableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var mFromField = assayTableFields.Where(f => f.columnImportName == DrillholeConstants.distFromName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var mToField = assayTableFields.Where(f => f.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                #endregion

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
                    string dip = element.Element(dipField).Value;
                    string azimuth = element.Element(azimuthField).Value;

                    int assayCount = assayElements.Where(h => h.Element(assayHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Count(); //check collar exists in assay table
                    if (assayCount > 0)
                    {
                        //check values are number otherwise go to next hole
                        bool bCheck = await ValidateValues(xCoord, yCoord, zCoord, totalDepth, holeAttr, dip, azimuth, null, null);

                        if (bCheck)
                        {
                            double dblX = 0.0, dblY = 0.0, dblZ = 0.0, dblLength = 0.0, dblFrom = 0.0, dblTo = 0.0,
                                dblDip = 0.0, dblAzimuth = 0.0;

                            dblX = Convert.ToDouble(xCoord);
                            dblY = Convert.ToDouble(yCoord);
                            dblZ = Convert.ToDouble(zCoord);
                            dblLength = Convert.ToDouble(totalDepth);
                            dblAzimuth = Convert.ToDouble(azimuth);
                            dblDip = Convert.ToDouble(dip);

                            var assayHole = assayElements.Where(h => h.Element(assayHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").ToList(); //get all samples for hole and ignroe those flagged

                            string assayHoleAttr = "";
                            int counter = 0;
                            foreach (var assayValue in assayHole) //loop through each sample interval for hole
                            {
                                assayHoleAttr = assayValue.Attribute("ID").Value;
                                string mFrom = assayValue.Element(mFromField).Value;
                                string mTo = assayValue.Element(mToField).Value;

                                bool bCheckDistance = true;

                                Coordinate3D coordinate = new Coordinate3D();

                                if (counter == 0) //always set as condition above shows collar coordiantes are valid
                                {
                                    if (bCollar)
                                    {
                                        await PopulateDesurveyObject(dblX, dblY, dblZ, 0, 0, dblAzimuth, dblDip, holeAttr, assayHoleAttr, hole, assayDesurveyDto,
                                            DrillholeTableType.assay, false, true);
                                    }
                                    //TODO separate dip and azimuth
                                    bCheckDistance = await ValidateValues(null, null, null, null, assayHoleAttr, null, null, mFrom, mTo);

                                    if (bCheckDistance)
                                    {
                                        dblFrom = Convert.ToDouble(mFrom);
                                        dblTo = Convert.ToDouble(mTo);

                                        coordinate = await PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, dblAzimuth, dblDip, holeAttr, assayHoleAttr,
                                            hole, assayDesurveyDto, DrillholeTableType.assay, true, false);
                                    }
                                }
                                else
                                {
                                    bCheckDistance = await ValidateValues(null, null, null, null, assayHoleAttr, dip, azimuth, mFrom, mTo);

                                    if (bCheckDistance)
                                    {
                                        dblFrom = Convert.ToDouble(mFrom);
                                        dblTo = Convert.ToDouble(mTo);

                                        coordinate = await PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, dblAzimuth, dblDip, holeAttr, assayHoleAttr, hole, 
                                            assayDesurveyDto, DrillholeTableType.assay, true, false);
                                    }
                                }

                                dblX = coordinate.x;
                                dblY = coordinate.y;
                                dblZ = coordinate.z;

                                counter++;
                            }

                            //check to add TD
                            if (bToe)
                            {
                                if (dblLength > dblTo)
                                {
                                    await PopulateDesurveyObject(dblX, dblY, dblZ, dblTo, dblLength, dblAzimuth, dblDip, holeAttr, assayHoleAttr, hole, assayDesurveyDto,
                                        DrillholeTableType.assay, false, false);
                                }
                            }
                        }
                    }
                }

                return assayDesurveyDto;
            }

            private async Task<Coordinate3D> PopulateDesurveyObject(double dblX, double dblY, double dblZ, double dblFrom, double dblTo, double dblAzimuth, double dblDip,
                string holeAttr, string sampleAttr, string hole, object desurveyDto, DrillholeTableType tableType, bool isSample, bool bCollar)
            {
                Coordinate3D coordinate = null;

                if (tableType == DrillholeTableType.assay)
                {
                    AssayDesurveyDto assayDesurveyDto = desurveyDto as AssayDesurveyDto;

                    assayDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
                    assayDesurveyDto.assayId.Add(Convert.ToInt16(sampleAttr));
                    assayDesurveyDto.bhid.Add(hole);
                    assayDesurveyDto.distFrom.Add(dblFrom);
                    assayDesurveyDto.distTo.Add(dblTo);
                    assayDesurveyDto.dip.Add(dblDip);
                    assayDesurveyDto.azimuth.Add(dblAzimuth);
                    assayDesurveyDto.length.Add(Convert.ToDouble(dblTo - dblFrom));
                    assayDesurveyDto.Count++;
                    assayDesurveyDto.isAssay.Add(isSample);

                    if (!bCollar)
                    {
                        coordinate = await CalculateSurveys.ReturnCoordinateTangential(dblX, dblY, dblZ, dblTo - dblFrom, dblAzimuth, dblDip);

                        assayDesurveyDto.x.Add(Convert.ToDouble(coordinate.x));
                        assayDesurveyDto.y.Add(Convert.ToDouble(coordinate.y));
                        assayDesurveyDto.z.Add(Convert.ToDouble(coordinate.z));

                    }
                    else
                    {
                        assayDesurveyDto.x.Add(dblX);
                        assayDesurveyDto.y.Add(dblY);
                        assayDesurveyDto.z.Add(dblZ);
                    }

                }
                else if (tableType == DrillholeTableType.interval)
                {
                    IntervalDesurveyDto intervalDesurveyDto = desurveyDto as IntervalDesurveyDto;

                    intervalDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
                    intervalDesurveyDto.intId.Add(Convert.ToInt16(sampleAttr));
                    intervalDesurveyDto.bhid.Add(hole);
                    intervalDesurveyDto.distFrom.Add(dblFrom);
                    intervalDesurveyDto.distTo.Add(dblTo);
                    intervalDesurveyDto.dip.Add(dblDip);
                    intervalDesurveyDto.azimuth.Add(dblAzimuth);
                    intervalDesurveyDto.length.Add(Convert.ToDouble(dblTo - dblFrom));
                    intervalDesurveyDto.Count++;
                    intervalDesurveyDto.isInterval.Add(isSample);

                    if (!bCollar)
                    {
                        coordinate = await CalculateSurveys.ReturnCoordinateTangential(dblX, dblY, dblZ, dblTo - dblFrom, dblAzimuth, dblDip);

                        intervalDesurveyDto.x.Add(Convert.ToDouble(coordinate.x));
                        intervalDesurveyDto.y.Add(Convert.ToDouble(coordinate.y));
                        intervalDesurveyDto.z.Add(Convert.ToDouble(coordinate.z));

                    }
                    else
                    {
                        intervalDesurveyDto.x.Add(dblX);
                        intervalDesurveyDto.y.Add(dblY);
                        intervalDesurveyDto.z.Add(dblZ);
                    }

                }
                else if (tableType == DrillholeTableType.continuous)
                {
                    ContinuousDesurveyDto continuousDesurveyDto = desurveyDto as ContinuousDesurveyDto;

                    continuousDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
                    continuousDesurveyDto.contId.Add(Convert.ToInt16(sampleAttr));
                    continuousDesurveyDto.bhid.Add(hole);
                    continuousDesurveyDto.distFrom.Add(dblFrom);
                    continuousDesurveyDto.dip.Add(dblDip);
                    continuousDesurveyDto.azimuth.Add(dblAzimuth);
                    continuousDesurveyDto.Count++;
                    continuousDesurveyDto.isContinuous.Add(isSample);

                    if (!bCollar)
                    {
                        coordinate = await CalculateSurveys.ReturnCoordinateTangential(dblX, dblY, dblZ, dblTo - dblFrom, dblAzimuth, dblDip);

                        continuousDesurveyDto.x.Add(Convert.ToDouble(coordinate.x));
                        continuousDesurveyDto.y.Add(Convert.ToDouble(coordinate.y));
                        continuousDesurveyDto.z.Add(Convert.ToDouble(coordinate.z));

                    }
                    else
                    {
                        continuousDesurveyDto.x.Add(dblX);
                        continuousDesurveyDto.y.Add(dblY);
                        continuousDesurveyDto.z.Add(dblZ);
                    }
                }
                else if (tableType == DrillholeTableType.survey)
                {
                    SurveyDesurveyDto surveyDesurveyDto = desurveyDto as SurveyDesurveyDto;

                    surveyDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
                    surveyDesurveyDto.survId.Add(Convert.ToInt16(sampleAttr));
                    surveyDesurveyDto.bhid.Add(hole);
                    surveyDesurveyDto.distFrom.Add(dblFrom);
                    surveyDesurveyDto.dip.Add(dblDip);
                    surveyDesurveyDto.azimuth.Add(dblAzimuth);
                    surveyDesurveyDto.Count++;
                    surveyDesurveyDto.isSurvey.Add(isSample);

                    if (!bCollar)
                    {
                        coordinate = await CalculateSurveys.ReturnCoordinateTangential(dblX, dblY, dblZ, dblTo - dblFrom, dblAzimuth, dblDip);

                        surveyDesurveyDto.x.Add(Convert.ToDouble(coordinate.x));
                        surveyDesurveyDto.y.Add(Convert.ToDouble(coordinate.y));
                        surveyDesurveyDto.z.Add(Convert.ToDouble(coordinate.z));

                    }
                    else
                    {
                        surveyDesurveyDto.x.Add(dblX);
                        surveyDesurveyDto.y.Add(dblY);
                        surveyDesurveyDto.z.Add(dblZ);
                    }
                }

                return coordinate;
            }

            public override async Task<AssayDesurveyDto> AssayVerticalTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
            {
                //initialise object and set up lists
                AssayDesurveyDto assayDesurveyDto = new AssayDesurveyDto()
                {
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

                                if (bCollar)
                                {
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
                                    assayDesurveyDto.Count++;
                                    assayDesurveyDto.length.Add(Convert.ToDouble(dblLength)); //use total hole length for collar
                                    assayDesurveyDto.isAssay.Add(false);
                                }

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
                                    assayDesurveyDto.Count++;
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
                                assayDesurveyDto.z.Add(Convert.ToDouble(dblNewZ));
                                assayDesurveyDto.length.Add(Convert.ToDouble(dblDistance));
                                assayDesurveyDto.Count++;
                                assayDesurveyDto.isAssay.Add(false);
                                assayDesurveyDto.distFrom.Add(0);
                                assayDesurveyDto.distTo.Add(0);
                                assayDesurveyDto.dip.Add(-90.0);
                                assayDesurveyDto.azimuth.Add(0.0);
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
                    desurveyType = DrillholeDesurveyEnum.Tangential,
                    surveyType = DrillholeSurveyType.collarsurvey
                };

                #region Field names
                //Need holeID, x, y, z, length => reference name in xml
                var holeId = tableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var xField = tableFields.Where(f => f.columnImportName == DrillholeConstants.xName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var yField = tableFields.Where(f => f.columnImportName == DrillholeConstants.yName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var zField = tableFields.Where(f => f.columnImportName == DrillholeConstants.zName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var tdField = tableFields.Where(f => f.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var dipField = tableFields.Where(f => f.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var azimuthField = tableFields.Where(f => f.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();

                if (dipField == null || azimuthField == null)
                {
                    throw new CollarException("Check Dip and Azimuth fields");
                }
                #endregion

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
                    string dip = element.Element(dipField).Value;
                    string azimuth = element.Element(azimuthField).Value;

                    bool bCheck = false;
                    if (hole != "")
                    {
                        //TODO if dip and azimuth don't make sense then change to vertical
                        bCheck = await ValidateValues(xCoord, yCoord, zCoord, totalDepth, colID, dip, azimuth, null, null);
                    }

                    if (bCheck)
                    {
                        double dblX = Convert.ToDouble(xCoord);
                        double dblY = Convert.ToDouble(yCoord);
                        double dblZ = Convert.ToDouble(zCoord);
                        double dblDistance = Convert.ToDouble(totalDepth);
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
                        collarDesurveyDto.isCollar.Add(true);

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

                        collarDesurveyDto.Count++;

                    }
                }

                return collarDesurveyDto;
            }

            public override Task<ContinuousDesurveyDto> ContinuousCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
            {
                throw new NotImplementedException();
            }

            public override Task<ContinuousDesurveyDto> ContinuousDownholeTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
            {
                throw new NotImplementedException();
            }

            public override async Task<ContinuousDesurveyDto> ContinuousVerticalTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
            {
                ContinuousDesurveyDto continuousDesurveyDto = new ContinuousDesurveyDto()
                {
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
                var contHoleID = continuousTableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var distanceField = continuousTableFields.Where(f => f.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();

                XElement elements = drillholeValues[0];
                var collarElements = elements.Elements();

                XElement contElements = drillholeValues[2];
                var continuousElements = contElements.Elements();

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
                        double dblX = 0.0, dblY = 0.0, dblZ = 0.0, dblLength = 0.0, dblDistance = 0.0, dblNewZ; //Vertical so only z will change

                        var continuousHole = continuousElements.Where(h => h.Element(contHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").ToList(); //get all samples for hole and ignroe those flagged

                        int counter = 0;
                        foreach (var contValue in continuousHole) //loop through each sample interval
                        {
                            string contHoleAttr = contValue.Attribute("ID").Value;
                            string distance = contValue.Element(distanceField).Value;

                            bool bCheckDistance = true;

                            if (counter == 0) //always set as condition above shows collar coordiantes are valid
                            {
                                dblX = Convert.ToDouble(xCoord);
                                dblY = Convert.ToDouble(yCoord);
                                dblZ = Convert.ToDouble(zCoord);
                                dblLength = Convert.ToDouble(totalDepth);

                                if (bCollar)
                                {
                                    //create collar => future will have a choice to create or not
                                    continuousDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
                                    continuousDesurveyDto.contId.Add(Convert.ToInt16(contHoleAttr));
                                    continuousDesurveyDto.bhid.Add(hole);
                                    continuousDesurveyDto.x.Add(Convert.ToDouble(xCoord));
                                    continuousDesurveyDto.y.Add(Convert.ToDouble(yCoord));
                                    continuousDesurveyDto.z.Add(Convert.ToDouble(dblZ));
                                    continuousDesurveyDto.distFrom.Add(0);
                                    continuousDesurveyDto.dip.Add(-90.0);
                                    continuousDesurveyDto.azimuth.Add(0.0);
                                    continuousDesurveyDto.Count++;
                                    continuousDesurveyDto.length.Add(Convert.ToDouble(dblLength)); //use total hole length for collar
                                    continuousDesurveyDto.isContinuous.Add(false);
                                }

                                bCheckDistance = await ValidateValues(null, null, null, null, contHoleAttr, null, null, distance, null);

                                if (bCheckDistance)
                                {
                                    //create first sample record
                                    dblDistance = Convert.ToDouble(distance);

                                    dblNewZ = dblZ - dblDistance;

                                    continuousDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
                                    continuousDesurveyDto.contId.Add(Convert.ToInt16(contHoleAttr));
                                    continuousDesurveyDto.bhid.Add(hole);
                                    continuousDesurveyDto.x.Add(Convert.ToDouble(xCoord));
                                    continuousDesurveyDto.y.Add(Convert.ToDouble(yCoord));
                                    continuousDesurveyDto.z.Add(dblNewZ);
                                    continuousDesurveyDto.distFrom.Add(dblDistance);
                                    continuousDesurveyDto.dip.Add(-90.0);
                                    continuousDesurveyDto.azimuth.Add(0.0);
                                    continuousDesurveyDto.Count++;
                                    //   continuousDesurveyDto.length.Add(dblLength); //use total hole length for collar
                                    continuousDesurveyDto.isContinuous.Add(true);
                                }
                            }
                            else
                            {
                                bCheckDistance = await ValidateValues(null, null, null, null, contHoleAttr, null, null, distance, null);

                                if (bCheckDistance)
                                {
                                    dblDistance = Convert.ToDouble(distance);

                                    dblNewZ = dblZ - dblDistance; //take from surface in case a record has been ignored

                                    continuousDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
                                    continuousDesurveyDto.contId.Add(Convert.ToInt16(contHoleAttr));
                                    continuousDesurveyDto.bhid.Add(hole);
                                    continuousDesurveyDto.x.Add(Convert.ToDouble(xCoord));
                                    continuousDesurveyDto.y.Add(Convert.ToDouble(yCoord));
                                    continuousDesurveyDto.z.Add(Convert.ToDouble(dblNewZ));
                                    continuousDesurveyDto.distFrom.Add(dblDistance);
                                    continuousDesurveyDto.dip.Add(-90.0);
                                    continuousDesurveyDto.azimuth.Add(0.0);
                                    // intervalDesurveyDto.length.Add(Convert.ToDouble(dblDistance));
                                    continuousDesurveyDto.Count++;
                                    continuousDesurveyDto.isContinuous.Add(true);
                                }

                            }

                            counter++;
                        }

                        //check to add TD
                        if (bToe)
                        {
                            if (dblLength > dblDistance)
                            {
                                //  dblDistance = dblLength - dblTo;

                                dblNewZ = dblZ - dblLength;

                                int value = continuousDesurveyDto.contId.Last();
                                continuousDesurveyDto.contId.Add(value);
                                continuousDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
                                continuousDesurveyDto.bhid.Add(hole);
                                continuousDesurveyDto.x.Add(Convert.ToDouble(xCoord));
                                continuousDesurveyDto.y.Add(Convert.ToDouble(yCoord));
                                continuousDesurveyDto.z.Add(Convert.ToDouble(dblNewZ));
                                continuousDesurveyDto.distFrom.Add(Convert.ToDouble(dblDistance));
                                continuousDesurveyDto.Count++;
                                continuousDesurveyDto.isContinuous.Add(false);
                                continuousDesurveyDto.azimuth.Add(0.0);
                                continuousDesurveyDto.dip.Add(-90.0);
                                continuousDesurveyDto.isContinuous.Add(false);
                                continuousDesurveyDto.distFrom.Add(0.0);
                            }
                        }
                    }

                }

                return continuousDesurveyDto;
            }

            public override Task<IntervalDesurveyDto> IntervalCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
            {
                throw new NotImplementedException();
            }

            public override Task<IntervalDesurveyDto> IntervalDownholeTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
            {
                throw new NotImplementedException();
            }

            public override async Task<IntervalDesurveyDto> IntervalVerticalTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
            {
                IntervalDesurveyDto intervalDesurveyDto = new IntervalDesurveyDto()
                {
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
                var intervalHoleID = intervalTableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var mFromField = intervalTableFields.Where(f => f.columnImportName == DrillholeConstants.distFromName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var mToField = intervalTableFields.Where(f => f.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();

                XElement elements = drillholeValues[0];
                var collarElements = elements.Elements();

                XElement intElements = drillholeValues[2];
                var intervalElements = intElements.Elements();

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

                        var intervalHole = intervalElements.Where(h => h.Element(intervalHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").ToList(); //get all samples for hole and ignroe those flagged

                        int counter = 0;
                        foreach (var intervalValue in intervalHole) //loop through each sample interval
                        {
                            string intervalHoleAttr = intervalValue.Attribute("ID").Value;
                            string mFrom = intervalValue.Element(mFromField).Value;
                            string mTo = intervalValue.Element(mToField).Value;

                            bool bCheckDistance = true;

                            if (counter == 0) //always set as condition above shows collar coordiantes are valid
                            {
                                dblX = Convert.ToDouble(xCoord);
                                dblY = Convert.ToDouble(yCoord);
                                dblZ = Convert.ToDouble(zCoord);
                                dblLength = Convert.ToDouble(totalDepth);

                                if (bCollar)
                                {
                                    //create collar => future will have a choice to create or not
                                    intervalDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
                                    intervalDesurveyDto.intId.Add(Convert.ToInt16(intervalHoleAttr));
                                    intervalDesurveyDto.bhid.Add(hole);
                                    intervalDesurveyDto.x.Add(Convert.ToDouble(xCoord));
                                    intervalDesurveyDto.y.Add(Convert.ToDouble(yCoord));
                                    intervalDesurveyDto.z.Add(Convert.ToDouble(dblZ));
                                    intervalDesurveyDto.distFrom.Add(0);
                                    intervalDesurveyDto.distTo.Add(0);
                                    intervalDesurveyDto.dip.Add(-90.0);
                                    intervalDesurveyDto.azimuth.Add(0.0);
                                    intervalDesurveyDto.Count++;
                                    intervalDesurveyDto.length.Add(Convert.ToDouble(dblLength)); //use total hole length for collar
                                    intervalDesurveyDto.isInterval.Add(false);
                                }

                                bCheckDistance = await ValidateValues(null, null, null, null, intervalHoleAttr, null, null, mFrom, mTo);

                                if (bCheckDistance)
                                {
                                    //create first sample record
                                    dblFrom = Convert.ToDouble(mFrom);
                                    dblTo = Convert.ToDouble(mTo);

                                    dblDistance = dblTo - dblFrom;

                                    dblNewZ = dblZ - dblDistance;

                                    intervalDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
                                    intervalDesurveyDto.intId.Add(Convert.ToInt16(intervalHoleAttr));
                                    intervalDesurveyDto.bhid.Add(hole);
                                    intervalDesurveyDto.x.Add(Convert.ToDouble(xCoord));
                                    intervalDesurveyDto.y.Add(Convert.ToDouble(yCoord));
                                    intervalDesurveyDto.z.Add(dblNewZ);
                                    intervalDesurveyDto.distFrom.Add(dblFrom);
                                    intervalDesurveyDto.distTo.Add(dblTo);
                                    intervalDesurveyDto.dip.Add(-90.0);
                                    intervalDesurveyDto.azimuth.Add(0.0);
                                    intervalDesurveyDto.Count++;
                                    intervalDesurveyDto.length.Add(dblDistance); //use total hole length for collar
                                    intervalDesurveyDto.isInterval.Add(true);
                                }
                            }
                            else
                            {
                                bCheckDistance = await ValidateValues(null, null, null, null, intervalHoleAttr, null, null, mFrom, mTo);

                                if (bCheckDistance)
                                {
                                    dblFrom = Convert.ToDouble(mFrom);
                                    dblTo = Convert.ToDouble(mTo);

                                    dblDistance = dblTo - dblFrom;

                                    dblNewZ = dblZ - dblTo; //take from surface in case a record has been ignored

                                    intervalDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
                                    intervalDesurveyDto.intId.Add(Convert.ToInt16(intervalHoleAttr));
                                    intervalDesurveyDto.bhid.Add(hole);
                                    intervalDesurveyDto.x.Add(Convert.ToDouble(xCoord));
                                    intervalDesurveyDto.y.Add(Convert.ToDouble(yCoord));
                                    intervalDesurveyDto.z.Add(Convert.ToDouble(dblNewZ));
                                    intervalDesurveyDto.distFrom.Add(dblFrom);
                                    intervalDesurveyDto.distTo.Add(dblTo);
                                    intervalDesurveyDto.dip.Add(-90.0);
                                    intervalDesurveyDto.azimuth.Add(0.0);
                                    intervalDesurveyDto.length.Add(Convert.ToDouble(dblDistance));
                                    intervalDesurveyDto.Count++;
                                    intervalDesurveyDto.isInterval.Add(true);
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

                                int value = intervalDesurveyDto.intId.Last();
                                intervalDesurveyDto.intId.Add(value);
                                intervalDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
                                intervalDesurveyDto.bhid.Add(hole);
                                intervalDesurveyDto.x.Add(Convert.ToDouble(xCoord));
                                intervalDesurveyDto.y.Add(Convert.ToDouble(yCoord));
                                intervalDesurveyDto.z.Add(Convert.ToDouble(dblNewZ));
                                intervalDesurveyDto.length.Add(Convert.ToDouble(dblDistance));
                                intervalDesurveyDto.Count++;
                                intervalDesurveyDto.azimuth.Add(0.0);
                                intervalDesurveyDto.dip.Add(-90.0);
                                intervalDesurveyDto.isInterval.Add(false);
                                intervalDesurveyDto.distFrom.Add(0.0);
                                intervalDesurveyDto.distTo.Add(0.0);
                            }
                        }
                    }

                }

                return intervalDesurveyDto;
            }

            public override Task<SurveyDesurveyDto> SurveyDownhole(ImportTableFields collarTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues)
            {
                throw new NotImplementedException();
            }

            public override async Task<CollarDesurveyDto> VerticalTrace(ImportTableFields tableFields, List<XElement> drillholeValues)
            {
                CollarDesurveyDto collarDesurveyDto = new CollarDesurveyDto()
                {
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
