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
            #region Assay
            /// <summary>
            /// Desurvey assay table using downhole surveys from survey table
            /// </summary>
            /// <param name="collarTableFields"></param>
            /// <param name="assayTableFields"></param>
            /// <param name="surveyTableFields"></param>
            /// <param name="drillholeValues"></param>
            /// <param name="bToe"></param>
            /// <param name="bCollar"></param>
            /// <returns></returns>
            public override async Task<AssayDesurveyDto> AssayDownholeTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
            {
                AssayDesurveyDto assayDesurveyDto = new AssayDesurveyDto()
                {
                    desurveyType = DrillholeDesurveyEnum.Tangential,
                    surveyType = DrillholeSurveyType.downholesurvey
                };

                #region fieldnames
                //Need holeID, x, y, z, length => reference name in xml
                var collarHoleID = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var xField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.xName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var yField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.yName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var zField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.zName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var tdField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();

                var surveyHoleID = surveyTableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var distanceField = surveyTableFields.Where(f => f.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var dipField = surveyTableFields.Where(f => f.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var azimuthField = surveyTableFields.Where(f => f.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();

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

                XElement surElements = drillholeValues[1];
                var surveyElements = surElements.Elements();

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

                    //string distance = surveyElements.Ele
                    //string dip = element.Element(dipField).Value;
                    //string azimuth = element.Element(azimuthField).Value;

                    double dblDip = 0.0, dblAzimuth = 0.0, dblDistance = 0.0;

                    int surveyCount = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Count(); //check collar exists in assay table
                    if (surveyCount == 0)
                    {
                        dblAzimuth = 0.0;
                        dblDip = -90;
                    }

                    int assayCount = assayElements.Where(h => h.Element(assayHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Count(); //check collar exists in assay table

                    if (assayCount > 0)
                    {
                        //check values are number otherwise go to next hole
                        bool bCheck = await DesurveyMethods.ValidateValues(xCoord, yCoord, zCoord, totalDepth, holeAttr, null, null, null, null);

                        var distances = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Element(distanceField).Value).ToList();
                        var dips = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Element(dipField).Value).ToList();
                        var azimuths = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Element(azimuthField).Value).ToList();

                        if (bCheck)
                        {
                            double dblX = 0.0, dblY = 0.0, dblZ = 0.0, dblLength = 0.0, dblFrom = 0.0, dblTo = 0.0;

                            dblX = Convert.ToDouble(xCoord);
                            dblY = Convert.ToDouble(yCoord);
                            dblZ = Convert.ToDouble(zCoord);
                            dblLength = Convert.ToDouble(totalDepth);
                            //dblAzimuth = Convert.ToDouble(azimuth);
                            //dblDip = Convert.ToDouble(dip);

                            var assayHole = assayElements.Where(h => h.Element(assayHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").ToList(); //get all samples for hole and ignroe those flagged

                            string assayHoleAttr = "";
                            int counter = 0;
                            foreach (var assayValue in assayHole) //loop through each sample interval for hole
                            {
                                assayHoleAttr = assayValue.Attribute("ID").Value;
                                string mFrom = assayValue.Element(mFromField).Value;
                                string mTo = assayValue.Element(mToField).Value;

                                DownholeSurveys surveys = await DesurveyMethods.ReturnSurveysPerInterval(azimuths, dips, distances, mFrom, mTo, totalDepth);

                                for (int s = 0; s < surveys.distance.Count; s++)
                                {
                                    dblAzimuth = surveys.azimuth[s];
                                    dblDip = surveys.dip[s];

                                    bool bCheckDistance = true;

                                    Coordinate3D coordinate = new Coordinate3D();

                                    if (counter == 0) //always set as condition above shows collar coordiantes are valid
                                    {
                                        if (bCollar)
                                        {
                                            await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, 0, 0, dblAzimuth, dblDip, holeAttr, assayHoleAttr, hole, assayDesurveyDto,
                                                DrillholeTableType.assay, false, true);
                                        }
                                        //TODO separate dip and azimuth
                                        bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, assayHoleAttr, null, null, mFrom, mTo);

                                        if (bCheckDistance)
                                        {
                                            dblFrom = Convert.ToDouble(mFrom);
                                            dblTo = Convert.ToDouble(mTo);

                                            coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, dblAzimuth, dblDip, holeAttr, assayHoleAttr,
                                                hole, assayDesurveyDto, DrillholeTableType.assay, true, false);
                                        }
                                    }
                                    else
                                    {
                                        //  bCheckDistance = await ValidateValues(null, null, null, null, assayHoleAttr, dip, azimuth, mFrom, mTo);

                                        if (bCheckDistance)
                                        {
                                            dblFrom = Convert.ToDouble(mFrom);
                                            dblTo = Convert.ToDouble(mTo);

                                            coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, dblAzimuth, dblDip, holeAttr, assayHoleAttr, hole,
                                                assayDesurveyDto, DrillholeTableType.assay, true, false);
                                        }
                                    }

                                    dblX = coordinate.x;
                                    dblY = coordinate.y;
                                    dblZ = coordinate.z;

                                    counter++;
                                }
                            }

                            //check to add TD
                            if (bToe)
                            {
                                if (dblLength > dblTo)
                                {
                                    await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblTo, dblLength, dblAzimuth, dblDip, holeAttr, assayHoleAttr, hole, assayDesurveyDto,
                                        DrillholeTableType.assay, false, false);
                                }
                            }
                        }
                    }
                }

                return assayDesurveyDto;
            }

            /// <summary>
            /// Desurvey assay table using dip and azimuth from collar table
            /// </summary>
            /// <param name="collarTableFields"></param>
            /// <param name="assayTableFields"></param>
            /// <param name="drillholeValues"></param>
            /// <param name="bToe"></param>
            /// <param name="bCollar"></param>
            /// <returns></returns>
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
                        bool bCheck = await DesurveyMethods.ValidateValues(xCoord, yCoord, zCoord, totalDepth, holeAttr, dip, azimuth, null, null);

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
                                        await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, 0, 0, dblAzimuth, dblDip, holeAttr, assayHoleAttr, hole, assayDesurveyDto,
                                            DrillholeTableType.assay, false, true);
                                    }
                                    //TODO separate dip and azimuth
                                    bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, assayHoleAttr, null, null, mFrom, mTo);

                                    if (bCheckDistance)
                                    {
                                        dblFrom = Convert.ToDouble(mFrom);
                                        dblTo = Convert.ToDouble(mTo);

                                        coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, dblAzimuth, dblDip, holeAttr, assayHoleAttr,
                                            hole, assayDesurveyDto, DrillholeTableType.assay, true, false);
                                    }
                                }
                                else
                                {
                                    bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, assayHoleAttr, dip, azimuth, mFrom, mTo);

                                    if (bCheckDistance)
                                    {
                                        dblFrom = Convert.ToDouble(mFrom);
                                        dblTo = Convert.ToDouble(mTo);

                                        coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, dblAzimuth, dblDip, holeAttr, assayHoleAttr, hole, 
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
                                    await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblTo, dblLength, dblAzimuth, dblDip, holeAttr, assayHoleAttr, hole, assayDesurveyDto,
                                        DrillholeTableType.assay, false, false);
                                }
                            }
                        }
                    }
                }

                return assayDesurveyDto;
            }

            /// <summary>
            /// Creates vertical intervals for each assay record
            /// </summary>
            /// <param name="collarTableFields"></param>
            /// <param name="assayTableFields"></param>
            /// <param name="drillholeValues"></param>
            /// <param name="bToe"></param>
            /// <param name="bCollar"></param>
            /// <returns></returns>
            public override async Task<AssayDesurveyDto> AssayVerticalTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
            {
                //initialise object and set up lists
                AssayDesurveyDto assayDesurveyDto = new AssayDesurveyDto()
                {
                    desurveyType = DrillholeDesurveyEnum.Tangential,
                    surveyType = DrillholeSurveyType.vertical
                };

                #region fieldnames
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

                    int assayCount = assayElements.Where(h => h.Element(assayHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Count(); //check collar exists in assay table
                    if (assayCount > 0)
                    {
                        //check values are number otherwise go to next hole
                        bool bCheck = await DesurveyMethods.ValidateValues(xCoord, yCoord, zCoord, totalDepth, holeAttr, null, null, null, null);

                        Coordinate3D coordinate = new Coordinate3D();

                        string assayHoleAttr = "";
                        if (bCheck)
                        {
                            double dblX = 0.0, dblY = 0.0, dblZ = 0.0, dblLength = 0.0, dblFrom = 0.0, dblTo = 0.0; //Vertical so only z will change

                            var assayHole = assayElements.Where(h => h.Element(assayHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").ToList(); //get all samples for hole and ignroe those flagged

                            int counter = 0;
                            foreach (var assayValue in assayHole) //loop through each sample interval
                            {
                                assayHoleAttr = assayValue.Attribute("ID").Value;
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
                                        await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, 0, 0, 0.0, -90, holeAttr, assayHoleAttr, hole, assayDesurveyDto,
                                               DrillholeTableType.assay, false, true);

                                    }

                                    bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, assayHoleAttr, null, null, mFrom, mTo);

                                    if (bCheckDistance)
                                    {
                                        //create first sample record
                                        dblFrom = Convert.ToDouble(mFrom);
                                        dblTo = Convert.ToDouble(mTo);

                                        coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, 0.0, -90, holeAttr, assayHoleAttr,
                                            hole, assayDesurveyDto, DrillholeTableType.assay, true, false);

                                    }
                                }
                                else
                                {
                                    bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, assayHoleAttr, null, null, mFrom, mTo);

                                    if (bCheckDistance)
                                    {
                                        dblFrom = Convert.ToDouble(mFrom);
                                        dblTo = Convert.ToDouble(mTo);

                                        coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, 0.0, -90, holeAttr, assayHoleAttr, hole, assayDesurveyDto,
                                              DrillholeTableType.assay, true, false);
                                    }

                                }

                                dblZ = coordinate.z;

                                counter++;
                            }

                            //check to add TD
                            if (bToe)
                            {

                                if (dblLength > dblFrom)
                                {
                                    await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblLength, 0.0, -90.0, holeAttr, assayHoleAttr, hole, assayDesurveyDto,
                                        DrillholeTableType.assay, false, false);
                                }


                            }
                        }
                    }

                }

                return assayDesurveyDto;
            }
            #endregion


            #region Collar
            /// <summary>
            /// Create two points per collar
            /// </summary>
            /// <param name="tableFields"></param>
            /// <param name="drillholeValues"></param>
            /// <returns></returns>
            public override async Task<CollarDesurveyDto> VerticalTrace(ImportTableFields tableFields, List<XElement> drillholeValues)
            {
                CollarDesurveyDto collarDesurveyDto = new CollarDesurveyDto()
                {
                    desurveyType = DrillholeDesurveyEnum.Tangential,
                    surveyType = DrillholeSurveyType.vertical

                };

                #region fields
                //Need holeID, x, y, z, length => reference name in xml
                var holeId = tableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var xField = tableFields.Where(f => f.columnImportName == DrillholeConstants.xName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var yField = tableFields.Where(f => f.columnImportName == DrillholeConstants.yName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var zField = tableFields.Where(f => f.columnImportName == DrillholeConstants.zName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var tdField = tableFields.Where(f => f.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                #endregion

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
                        bCheck = await DesurveyMethods.ValidateValues(xCoord, yCoord, zCoord, totalDepth, holeAttr, null, null, null, null);
                    }

                    double dblX = 0.0, dblY = 0.0, dblZ = 0.0, dblLength = 0.0; //Vertical so only z will change

                    if (bCheck)
                    {
                        dblX = Convert.ToDouble(xCoord);
                        dblY = Convert.ToDouble(yCoord);
                        dblZ = Convert.ToDouble(zCoord);
                        dblLength = Convert.ToDouble(totalDepth);

                        await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, 0, 0, 0.0, -90.0, holeAttr, "", hole, collarDesurveyDto,
                                            DrillholeTableType.collar, true, true);

                        //store toe
                        await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, 0.0, dblLength, 0.0, -90.0, holeAttr, "", hole, collarDesurveyDto,
                                            DrillholeTableType.collar, false, false);
                    }
                }

                return collarDesurveyDto;
            }

            /// <summary>
            /// Creates collar and toe based on azimuth and dip
            /// </summary>
            /// <param name="tableFields"></param>
            /// <param name="drillholeValues"></param>
            /// <returns></returns>
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
                        bCheck = await DesurveyMethods.ValidateValues(xCoord, yCoord, zCoord, totalDepth, colID, dip, azimuth, null, null);
                    }

                    Coordinate3D coordinate = new Coordinate3D();
                    double dblX = 0.0, dblY = 0.0, dblZ = 0.0, dblDistance = 0.0, dblDip = 0.0, dblAzimuth = 0.0 ; 

                    if (bCheck)
                    {
                        dblX = Convert.ToDouble(xCoord);
                        dblY = Convert.ToDouble(yCoord);
                        dblZ = Convert.ToDouble(zCoord);
                        dblDistance = Convert.ToDouble(totalDepth);
                        dblDip = Convert.ToDouble(dip);
                        dblAzimuth = Convert.ToDouble(azimuth);

                        await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, 0, 0, dblAzimuth, dblDip, colID, "", hole, collarDesurveyDto,
                                          DrillholeTableType.collar, true, true);

                        //store toe
                        await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, 0.0, dblDistance, dblAzimuth, dblDip, colID, "", hole, collarDesurveyDto,
                                            DrillholeTableType.collar, false, false);

                        collarDesurveyDto.Count++;

                    }
                }

                return collarDesurveyDto;
            }
            #endregion

            #region Continuous

            /// <summary>
            /// Create a desruveyed continous table (i.e. distance to field) using dip and azi from collar table
            /// </summary>
            /// <param name="collarTableFields"></param>
            /// <param name="continuousTableFields"></param>
            /// <param name="drillholeValues"></param>
            /// <param name="bToe"></param>
            /// <param name="bCollar"></param>
            /// <returns></returns>
            public override async Task<ContinuousDesurveyDto> ContinuousCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
            {
                ContinuousDesurveyDto continuousDesurveyDto = new ContinuousDesurveyDto()
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
                var continuousHoleID = continuousTableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var distField = continuousTableFields.Where(f => f.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                #endregion

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
                    string dip = element.Element(dipField).Value;
                    string azimuth = element.Element(azimuthField).Value;

                    int contCount = continuousElements.Where(h => h.Element(continuousHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Count(); //check collar exists in assay table
                    if (contCount > 0)
                    {
                        //check values are number otherwise go to next hole
                        bool bCheck = await DesurveyMethods.ValidateValues(xCoord, yCoord, zCoord, totalDepth, holeAttr, dip, azimuth, null, null);

                        if (bCheck)
                        {
                            double dblX = 0.0, dblY = 0.0, dblZ = 0.0, dblLength = 0.0, dblDistanceTo = 0.0,
                                dblDip = 0.0, dblAzimuth = 0.0, dblNewDistTo = 0.0, dblPreviousDistance = 0.0;

                            dblX = Convert.ToDouble(xCoord);
                            dblY = Convert.ToDouble(yCoord);
                            dblZ = Convert.ToDouble(zCoord);
                            dblLength = Convert.ToDouble(totalDepth);
                            dblAzimuth = Convert.ToDouble(azimuth);
                            dblDip = Convert.ToDouble(dip);

                            var continuousHole = continuousElements.Where(h => h.Element(continuousHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").ToList(); //get all samples for hole and ignroe those flagged

                            string continuousHoleAttr = "";
                            int counter = 0;
                            foreach (var continuousValue in continuousHole) //loop through each sample interval for hole
                            {
                                continuousHoleAttr = continuousValue.Attribute("ID").Value;
                                string mDist = continuousValue.Element(distField).Value;

                                bool bCheckDistance = true;

                                Coordinate3D coordinate = new Coordinate3D();

                                if (counter == 0) //always set as condition above shows collar coordiantes are valid
                                {
                                    if (bCollar)
                                    {
                                        await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, 0, 0, dblAzimuth, dblDip, holeAttr, continuousHoleAttr, hole, continuousDesurveyDto,
                                            DrillholeTableType.continuous, false, true);
                                    }
                                    //TODO separate dip and azimuth
                                    bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, continuousHoleAttr, null, null, mDist, null);

                                    if (bCheckDistance)
                                    {
                                        dblDistanceTo = Convert.ToDouble(mDist);

                                        coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, 0, dblDistanceTo, dblAzimuth, dblDip, holeAttr, continuousHoleAttr,
                                            hole, continuousDesurveyDto, DrillholeTableType.continuous, true, false);
                                    }
                                }
                                else
                                {
                                    bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, continuousHoleAttr, dip, azimuth, mDist, null);

                                    if (bCheckDistance)
                                    {
                                        dblPreviousDistance = dblDistanceTo;

                                        dblDistanceTo = Convert.ToDouble(mDist);

                                        dblNewDistTo = dblDistanceTo - dblPreviousDistance;

                                        coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, 0, dblNewDistTo, dblAzimuth, dblDip, holeAttr, continuousHoleAttr, hole,
                                            continuousDesurveyDto, DrillholeTableType.continuous, true, false);
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
                                if (dblLength > dblDistanceTo)
                                {
                                    await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblDistanceTo, dblLength, dblAzimuth, dblDip, holeAttr, continuousHoleAttr, hole, continuousDesurveyDto,
                                        DrillholeTableType.continuous, false, false);
                                }
                            }
                        }
                    }
                }

                return continuousDesurveyDto;
            }

            /// <summary>
            /// Desurvey continuous table using downhole surveys from survey table
            /// </summary>
            /// <param name="collarTableFields"></param>
            /// <param name="continuousTableFields"></param>
            /// <param name="surveyTableFields"></param>
            /// <param name="drillholeValues"></param>
            /// <param name="bToe"></param>
            /// <param name="bCollar"></param>
            /// <returns></returns>
            public override Task<ContinuousDesurveyDto> ContinuousDownholeTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Create a vertical desurvey for continuous table
            /// </summary>
            /// <param name="collarTableFields"></param>
            /// <param name="continuousTableFields"></param>
            /// <param name="drillholeValues"></param>
            /// <param name="bToe"></param>
            /// <param name="bCollar"></param>
            /// <returns></returns>
            public override async Task<ContinuousDesurveyDto> ContinuousVerticalTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
            {
                ContinuousDesurveyDto continuousDesurveyDto = new ContinuousDesurveyDto()
                {
                    desurveyType = DrillholeDesurveyEnum.Tangential,
                    surveyType = DrillholeSurveyType.vertical
                };

                #region Fields
                //Need holeID, x, y, z, length => reference name in xml
                var collarHoleID = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var xField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.xName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var yField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.yName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var zField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.zName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var tdField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();

                //TODO - add assay fields 
                var contHoleID = continuousTableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var distanceField = continuousTableFields.Where(f => f.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                #endregion

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


                    int contCount = continuousElements.Where(h => h.Element(contHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Count(); //check collar exists in assay table
                    if (contCount > 0)
                    {
                        //check values are number otherwise go to next hole
                        bool bCheck = await DesurveyMethods.ValidateValues(xCoord, yCoord, zCoord, totalDepth, holeAttr, null, null, null, null);


                        if (bCheck)
                        {
                            double dblX = 0.0, dblY = 0.0, dblZ = 0.0, dblDistTo = 0.0, dblLength = 0.0,
                                dblDip = -90.0, dblAzimuth = 0.0, dblNewDistTo = 0.0, dblPreviousDistance = 0.0; //Vertical so only z will change

                            var continuousHole = continuousElements.Where(h => h.Element(contHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").ToList(); //get all samples for hole and ignroe those flagged
                            string contHoleAttr = "";
                            Coordinate3D coordinate = new Coordinate3D();

                            dblX = Convert.ToDouble(xCoord);
                            dblY = Convert.ToDouble(yCoord);
                            dblZ = Convert.ToDouble(zCoord);
                            dblLength = Convert.ToDouble(totalDepth);

                            int counter = 0;
                            foreach (var contValue in continuousHole) //loop through each sample interval
                            {
                                contHoleAttr = contValue.Attribute("ID").Value;
                                string distance = contValue.Element(distanceField).Value;

                                bool bCheckDistance = true;


                                if (counter == 0) //always set as condition above shows collar coordiantes are valid
                                {

                                    if (bCollar)
                                    {
                                        await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, 0, 0, dblAzimuth, dblDip, holeAttr, contHoleAttr, hole, continuousDesurveyDto,
                                              DrillholeTableType.continuous, false, true);
                                    }

                                    bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, contHoleAttr, null, null, distance, null);

                                    if (bCheckDistance)
                                    {
                                        dblDistTo = Convert.ToDouble(distance);

                                        //dblFrom is actually Distance to so it is in the dblTo position
                                        coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, 0, dblDistTo, dblAzimuth, dblDip, holeAttr, contHoleAttr, hole, continuousDesurveyDto,
                                              DrillholeTableType.continuous, true, false);
                                    }
                                }
                                else
                                {
                                    bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, contHoleAttr, null, null, distance, null);

                                    if (bCheckDistance)
                                    {
                                        dblPreviousDistance = dblDistTo;

                                        dblDistTo = Convert.ToDouble(distance);
                                        dblNewDistTo = dblDistTo - dblPreviousDistance;

                                        coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, 0, dblNewDistTo, 0.0, -90, holeAttr, contHoleAttr, hole, continuousDesurveyDto,
                                              DrillholeTableType.continuous, true, false);
                                    }

                                }

                                dblZ = coordinate.z;

                                counter++;
                            }

                            if (bToe)
                            {
                                if (dblLength > dblDistTo)
                                {
                                    await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblDistTo, dblLength, dblAzimuth, dblDip, holeAttr, contHoleAttr, hole,
                                        continuousDesurveyDto, DrillholeTableType.continuous, false, false);
                                }

                            }
                        }

                    }
                }

                return continuousDesurveyDto;
            }
            #endregion

            #region Interval

            /// <summary>
            /// Desurvey interval table using azimuth and dip from collar table
            /// </summary>
            /// <param name="collarTableFields"></param>
            /// <param name="intervalTableFields"></param>
            /// <param name="drillholeValues"></param>
            /// <param name="bToe"></param>
            /// <param name="bCollar"></param>
            /// <returns></returns>
            public override async Task<IntervalDesurveyDto> IntervalCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
            {
                IntervalDesurveyDto intervalDesurveyDto = new IntervalDesurveyDto()
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
                var intervalHoleID = intervalTableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var mFromField = intervalTableFields.Where(f => f.columnImportName == DrillholeConstants.distFromName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var mToField = intervalTableFields.Where(f => f.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                #endregion

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
                    string dip = element.Element(dipField).Value;
                    string azimuth = element.Element(azimuthField).Value;

                    int intervalCount = intervalElements.Where(h => h.Element(intervalHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Count(); //check collar exists in assay table
                    if (intervalCount > 0)
                    {
                        //check values are number otherwise go to next hole
                        bool bCheck = await DesurveyMethods.ValidateValues(xCoord, yCoord, zCoord, totalDepth, holeAttr, dip, azimuth, null, null);

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

                            var intervalHole = intervalElements.Where(h => h.Element(intervalHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").ToList(); //get all samples for hole and ignroe those flagged

                            string intervalHoleAttr = "";
                            int counter = 0;
                            foreach (var intervalValue in intervalHole) //loop through each sample interval for hole
                            {
                                intervalHoleAttr = intervalValue.Attribute("ID").Value;
                                string mFrom = intervalValue.Element(mFromField).Value;
                                string mTo = intervalValue.Element(mToField).Value;

                                bool bCheckDistance = true;

                                Coordinate3D coordinate = new Coordinate3D();

                                if (counter == 0) //always set as condition above shows collar coordiantes are valid
                                {
                                    if (bCollar)
                                    {
                                        await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, 0, 0, dblAzimuth, dblDip, holeAttr, intervalHoleAttr, hole, intervalDesurveyDto,
                                            DrillholeTableType.interval, false, true);
                                    }
                                    //TODO separate dip and azimuth
                                    bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, intervalHoleAttr, null, null, mFrom, mTo);

                                    if (bCheckDistance)
                                    {
                                        dblFrom = Convert.ToDouble(mFrom);
                                        dblTo = Convert.ToDouble(mTo);

                                        coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, dblAzimuth, dblDip, holeAttr, intervalHoleAttr,
                                            hole, intervalDesurveyDto, DrillholeTableType.interval, true, false);
                                    }
                                }
                                else
                                {
                                    bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, intervalHoleAttr, dip, azimuth, mFrom, mTo);

                                    if (bCheckDistance)
                                    {
                                        dblFrom = Convert.ToDouble(mFrom);
                                        dblTo = Convert.ToDouble(mTo);

                                        coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, dblAzimuth, dblDip, holeAttr, intervalHoleAttr, hole,
                                            intervalDesurveyDto, DrillholeTableType.interval, true, false);
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
                                    await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblTo, dblLength, dblAzimuth, dblDip, holeAttr, intervalHoleAttr, hole, intervalDesurveyDto,
                                        DrillholeTableType.interval, false, false);
                                }
                            }
                        }
                    }
                }

                return intervalDesurveyDto;
            }

            /// <summary>
            /// Desurvey interval table using dowhole surveys from survey table
            /// </summary>
            /// <param name="collarTableFields"></param>
            /// <param name="intervalTableFields"></param>
            /// <param name="surveyTableFields"></param>
            /// <param name="drillholeValues"></param>
            /// <param name="bToe"></param>
            /// <param name="bCollar"></param>
            /// <returns></returns>
            public override Task<IntervalDesurveyDto> IntervalDownholeTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Create a vertical trace for each interval
            /// </summary>
            /// <param name="collarTableFields"></param>
            /// <param name="intervalTableFields"></param>
            /// <param name="drillholeValues"></param>
            /// <param name="bToe"></param>
            /// <param name="bCollar"></param>
            /// <returns></returns>
            public override async Task<IntervalDesurveyDto> IntervalVerticalTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
            {
                IntervalDesurveyDto intervalDesurveyDto = new IntervalDesurveyDto()
                {
                    desurveyType = DrillholeDesurveyEnum.Tangential,
                    surveyType = DrillholeSurveyType.vertical
                };

                #region fieldnames
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
                #endregion

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

                    int intervalCount = intervalElements.Where(h => h.Element(intervalHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Count(); //check collar exists in assay table
                    if (intervalCount > 0)
                    {
                        //check values are number otherwise go to next hole
                        bool bCheck = await DesurveyMethods.ValidateValues(xCoord, yCoord, zCoord, totalDepth, holeAttr, null, null, null, null);

                        if (bCheck)
                        {
                            double dblX = 0.0, dblY = 0.0, dblZ = 0.0, dblLength = 0.0, dblFrom = 0.0, dblTo = 0.0; //Vertical so only z will change

                            var intervalHole = intervalElements.Where(h => h.Element(intervalHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").ToList(); //get all samples for hole and ignroe those flagged

                            string intervalHoleAttr = "";

                            int counter = 0;
                            foreach (var intervalValue in intervalHole) //loop through each sample interval
                            {
                                intervalHoleAttr = intervalValue.Attribute("ID").Value;
                                string mFrom = intervalValue.Element(mFromField).Value;
                                string mTo = intervalValue.Element(mToField).Value;

                                bool bCheckDistance = true;
                                Coordinate3D coordinate = new Coordinate3D();

                                if (counter == 0) //always set as condition above shows collar coordiantes are valid
                                {
                                    dblX = Convert.ToDouble(xCoord);
                                    dblY = Convert.ToDouble(yCoord);
                                    dblZ = Convert.ToDouble(zCoord);
                                    dblLength = Convert.ToDouble(totalDepth);

                                    if (bCollar)
                                    {
                                        await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, 0, dblLength, 0.0, -90, holeAttr, intervalHoleAttr, hole, intervalDesurveyDto,
                                              DrillholeTableType.interval, false, true);
                                    }

                                    bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, intervalHoleAttr, null, null, mFrom, mTo);

                                    if (bCheckDistance)
                                    {
                                        //create first sample record
                                        dblFrom = Convert.ToDouble(mFrom);
                                        dblTo = Convert.ToDouble(mTo);

                                        coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, 0.0, -90, holeAttr, intervalHoleAttr, hole, intervalDesurveyDto,
                                              DrillholeTableType.interval, true, false);
                                    }
                                }
                                else
                                {
                                    bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, intervalHoleAttr, null, null, mFrom, mTo);

                                    if (bCheckDistance)
                                    {
                                        dblFrom = Convert.ToDouble(mFrom);
                                        dblTo = Convert.ToDouble(mTo);

                                        coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, 0.0, -90, holeAttr, intervalHoleAttr, hole, intervalDesurveyDto,
                                             DrillholeTableType.interval, true, false);
                                    }

                                }

                                dblZ = coordinate.z;
                                counter++;
                            }

                            //check to add TD
                            if (bToe)
                            {
                                if (dblLength > dblFrom)
                                {
                                    await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblLength, 0.0, -90.0, holeAttr, intervalHoleAttr, hole,
                                        intervalDesurveyDto, DrillholeTableType.interval, false, false);
                                }
                            }
                        }
                    }

                }

                return intervalDesurveyDto;
            }
            #endregion

         
            public override async Task<SurveyDesurveyDto> SurveyDownhole(ImportTableFields collarTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues)
            {
                SurveyDesurveyDto surveyDesurveyDto = new SurveyDesurveyDto()
                {
                    desurveyType = DrillholeDesurveyEnum.Tangential,
                    surveyType = DrillholeSurveyType.downholesurvey
                };

                #region fieldnames
                //Need holeID, x, y, z, length => reference name in xml
                var collarHoleID = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var xField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.xName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var yField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.yName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var zField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.zName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var tdField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();

                //TODO - add assay fields 
                var surveyHoleID = surveyTableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var distField = surveyTableFields.Where(f => f.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var dipField = surveyTableFields.Where(f => f.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var azimuthField = surveyTableFields.Where(f => f.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();

                #endregion

                XElement elements = drillholeValues[0];
                var collarElements = elements.Elements();

                XElement survElements = drillholeValues[1];
                var surveyElements = survElements.Elements();

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

                    int survCount = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Count(); //check collar exists in assay table
                    
                    if (survCount > 0)
                    {
                        //check values are number otherwise go to next hole
                        bool bCheck = await DesurveyMethods.ValidateValues(xCoord, yCoord, zCoord, totalDepth, holeAttr, null, null, null, null);

                        if (bCheck)
                        {
                            double dblX = 0.0, dblY = 0.0, dblZ = 0.0, dblLength = 0.0, dblNewDistanceTo = 0.0, dblDistanceTo = 0.0,
                                dblDip = 0.0, dblAzimuth = 0.0;

                            dblX = Convert.ToDouble(xCoord);
                            dblY = Convert.ToDouble(yCoord);
                            dblZ = Convert.ToDouble(zCoord);
                            dblLength = Convert.ToDouble(totalDepth);

                            var surveyHole = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").ToList(); //get all samples for hole and ignroe those flagged

                            string surveyHoleAttr = "";
                            for (int i = 0; i < surveyHole.Count; i++)
                            {
                                surveyHoleAttr = surveyHole[i].Attribute("ID").Value;
                                string mDist = surveyHole[i].Element(distField).Value;
                                string dip = surveyHole[i].Element(dipField).Value;
                                string azimuth = surveyHole[i].Element(azimuthField).Value;

                                bool bCheckDistance = true;

                                Coordinate3D coordinate = new Coordinate3D();

                                if (i == 0)
                                {
                                    bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, surveyHoleAttr, dip, azimuth, mDist, null);

                                    if (bCheckDistance)
                                    {
                                        dblDistanceTo = Convert.ToDouble(mDist);
                                        dblAzimuth = Convert.ToDouble(azimuth);
                                        dblDip = Convert.ToDouble(dip);

                                        //get collar first
                                        await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, 0, 0, dblAzimuth, dblDip, holeAttr, surveyHoleAttr,
                                            hole, surveyDesurveyDto, DrillholeTableType.survey, false, true);

                                        if (surveyHole.Count > 1)
                                        {
                                            string newDist = surveyHole[i + 1].Element(distField).Value;
                                            bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, surveyHoleAttr, dip, azimuth, mDist, null);

                                            dblNewDistanceTo = Convert.ToDouble(newDist);

                                            coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblDistanceTo, dblNewDistanceTo, dblAzimuth, dblDip, holeAttr, surveyHoleAttr,
                                                hole, surveyDesurveyDto, DrillholeTableType.survey, true, false);
                                        }
                                    }
                                }
                                else if (i == surveyHole.Count - 1)
                                {
                                    if (dblLength > dblNewDistanceTo)
                                    {

                                        coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblNewDistanceTo, dblLength, dblAzimuth, dblDip, holeAttr, surveyHoleAttr, hole,
                                                surveyDesurveyDto, DrillholeTableType.survey, false, false);
                                    }
                                }

                                else
                                {
                                    if (surveyHole.Count > 1)
                                    {
                                        string newDist = surveyHole[i + 1].Element(distField).Value;

                                        bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, surveyHoleAttr, dip, azimuth, newDist, null); //TODO add dblDist and new dist

                                        dblAzimuth = Convert.ToDouble(azimuth);
                                        dblDip = Convert.ToDouble(dip);

                                        if (bCheckDistance)
                                        {
                                            dblDistanceTo = Convert.ToDouble(surveyHole[i].Element(distField).Value);

                                            dblNewDistanceTo = Convert.ToDouble(newDist);


                                            coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblDistanceTo, dblNewDistanceTo, dblAzimuth, dblDip, holeAttr, surveyHoleAttr, hole,
                                                surveyDesurveyDto, DrillholeTableType.survey, true, false);
                                        }
                                    }
                                }

                                dblX = coordinate.x;
                                dblY = coordinate.y;
                                dblZ = coordinate.z;

                            }
                        }
                    }
                }

                return surveyDesurveyDto;
            }
           
        }
    }

    public static class DesurveyMethods
    {

        public static async Task<DownholeSurveys>ReturnSurveysPerInterval(List<string> azimuths, List<string> dips, List<string> distances, string mFrom, string mTo, string tD)
        {
            double dblFrom = Convert.ToDouble(mFrom);
            double dblTo = Convert.ToDouble(mTo);
            double dbltD = Convert.ToDouble(tD);

            //TODO give user choice to restrict to hole length
            //if (dblTo > dbltD)
            //    dblTo = dbltD;

            double dblInterval = dblTo - dblFrom; //TODO, work out ratio of length for weighting
           

            List<double> dblAzimuths = new List<double>();
            List<double> dblDips = new List<double>();
            List<double> dblDistances = new List<double>();

            for (int i = 0; i < dips.Count; i++)
            {
                if (Information.IsNumeric(distances[i]))
                {
                    dblDistances.Add(Convert.ToDouble(distances[i]));
                }
                else
                {
                    dblDistances.Add(-99);

                }

                if (Information.IsNumeric(dips[i]))
                {
                    dblDips.Add(Convert.ToDouble(dips[i]));

                }
                else
                    dblDips.Add(-90);

                if (Information.IsNumeric(azimuths[i]))
                {
                    dblAzimuths.Add(Convert.ToDouble(azimuths[i]));
                }
                else
                    dblAzimuths.Add(0.0);

            }

            bool bRepeat = false;
            int counter = 0;

            DownholeSurveys downholeSurveys = new DownholeSurveys();

            //TODO - create seperate procedure
            for (int v = 0; v < dblDistances.Count; v++)
            {

                if (dblFrom == dblDistances[v])
                {
                    downholeSurveys.distFrom.Add(dblFrom);
                    downholeSurveys.distance.Add(dblDistances[v]);
                    downholeSurveys.dip.Add(dblDips[v]);
                    downholeSurveys.azimuth.Add(dblAzimuths[v]);

                    if (dblTo < dblDistances[v + 1])
                    {
                        downholeSurveys.distTo.Add(dblTo);
                    }
                    else
                    {
                        downholeSurveys.distTo.Add(dblDistances[v + 1]);

                        dblFrom = dblDistances[v + 1];

                        bRepeat = true;
                    }
                }

                else if (dblFrom > dblDistances[v])
                {
                    if (dblFrom < dblDistances[v + 1])
                    {
                        downholeSurveys.distFrom.Add(dblFrom);
                        downholeSurveys.distance.Add(dblDistances[v]);
                        downholeSurveys.dip.Add(dblDips[v]);
                        downholeSurveys.azimuth.Add(dblAzimuths[v]);

                        if (dblTo <= dblDistances[v + 1])
                        {
                            downholeSurveys.distTo.Add(dblTo);
                        }
                        else
                        {
                            downholeSurveys.distTo.Add(dblDistances[v + 1]);

                            dblFrom = dblDistances[v + 1];

                            bRepeat = true;
                        }

                    }
                    else if (dblFrom == dblDistances[v + 1])
                    {
                        downholeSurveys.distFrom.Add(dblFrom);
                        downholeSurveys.distance.Add(dblDistances[v + 1]);
                        downholeSurveys.dip.Add(dblDips[v + 1]);
                        downholeSurveys.azimuth.Add(dblAzimuths[v + 1]);
                    }
                    else if (dblFrom > dblDistances[v + 1])
                    {
                        //Loop
                    }


                }

                if (bRepeat)
                    break;

            }

            //for (int d = 0; d < dblDistances.Count; d++)
            //{
            //    //Survey 1
            //    dblDistanceCheck = dblDistances[d];

            //    if (!bLower)
            //    {
            //        if (counter < dblDistances.Count - 1 )
            //        {

            //            if (dblDistanceCheck >= dblFrom)
            //            {
            //                //Survey 1
            //                downholeSurveys.distFrom.Add(dblFrom);

            //                if (dblDistanceCheck == dblFrom)
            //                {
            //                    //Survey 1
            //                    downholeSurveys.distance.Add(dblDistanceCheck);
            //                }
            //                else
            //                    //Survey 1
            //                    downholeSurveys.distance.Add(dblDistances[d - 1]);

            //                bLower = true;
            //            }
            //        }

            //        else
            //        {
            //            if (dblDistanceCheck == dblFrom)
            //            {
            //                //Survey 1
            //                downholeSurveys.distance.Add(dblDistanceCheck);
            //            }
            //            else
            //                //Survey 1
            //                downholeSurveys.distance.Add(dblDistances[d - 1]);

            //            bLower = true;
            //        }

            //    }
                
            //    if (bLower)
            //    {
            //        if (counter < dblDistances.Count - 1)
            //        {
            //            if (dblFrom <= dblDistanceCheck || dblDistanceCheck >= dblTo)
            //            {
            //                //Survey 1
            //                downholeSurveys.distTo.Add(dblTo);
            //                downholeSurveys.azimuth.Add(dblAzimuths[d]);
            //                downholeSurveys.dip.Add(dblDips[d]);
            //                break;
            //            }
            //            else //it means greater than end of survey, so create survey to end of survey interval, and then start a new survey
            //            {
            //                //Survey 1
            //                downholeSurveys.distTo.Add(dblDistances[d]);
            //                downholeSurveys.azimuth.Add(dblAzimuths[d]);
            //                downholeSurveys.dip.Add(dblDips[d]);

            //                //Survey 2
            //                dblFrom = dblDistances[d];
            //                downholeSurveys.distFrom.Add(dblFrom);
            //                downholeSurveys.azimuth.Add(dblAzimuths[d + 1]);
            //                downholeSurveys.dip.Add(dblDips[d + 1]);

            //                bSurvey = await DistanceGreaterThanSurvey(dblDistances[d + 1], dblTo);

            //                if (bSurvey)
            //                {
            //                    //Survey 2
            //                    downholeSurveys.distTo.Add(dblTo);
            //                }
            //                else
            //                {
            //                    //Survey 2
            //                    downholeSurveys.distTo.Add(dblDistances[d + 1]);
            //                }

            //                break;
            //            }
       
            //        }
            //        else
            //        {
            //            downholeSurveys.distTo.Add(dblDistances[d]);
            //            downholeSurveys.azimuth.Add(dblAzimuths[d]);
            //            downholeSurveys.dip.Add(dblDips[d]);
            //        }
            //    }

            //    counter++;
            //}


            return downholeSurveys;
        }

        private static async Task<bool> ReturnDistanceTo( double dblDistance, double mTo)
        {
            if (dblDistance >= mTo)
                return true;
            

            return false;
        }

        private static async Task<bool> DistanceGreaterThanSurvey(double dblDistance, double mTo)
        {
            if (dblDistance >= mTo)
                return true;


            return false;
        }

        /// <summary>
        /// Checks for valid values
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="td"></param>
        /// <param name="holeID"></param>
        /// <param name="dip"></param>
        /// <param name="azi"></param>
        /// <param name="distance"></param>
        /// <param name="distanceTo"></param>
        /// <returns></returns>
        /// 
        public static async Task<bool> ValidateValues(string x, string y, string z, string td, string holeID, string dip, string azi, string distance, string distanceTo)
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

        /// <summary>
        /// Populates the desurvey class with values
        /// </summary>
        /// <param name="dblX"></param>
        /// <param name="dblY"></param>
        /// <param name="dblZ"></param>
        /// <param name="dblFrom"></param>
        /// <param name="dblTo"></param>
        /// <param name="dblAzimuth"></param>
        /// <param name="dblDip"></param>
        /// <param name="holeAttr"></param>
        /// <param name="sampleAttr"></param>
        /// <param name="hole"></param>
        /// <param name="desurveyDto"></param>
        /// <param name="tableType"></param>
        /// <param name="isSample"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public static async Task<Coordinate3D> PopulateDesurveyObject(double dblX, double dblY, double dblZ, double dblFrom, double dblTo, double dblAzimuth, double dblDip,
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
                assayDesurveyDto.dip.Add(dblDip);
                assayDesurveyDto.azimuth.Add(dblAzimuth);
                assayDesurveyDto.length.Add(Convert.ToDouble(dblTo - dblFrom));
                assayDesurveyDto.Count++;
                assayDesurveyDto.isAssay.Add(isSample);

                if (!bCollar)
                {
                    coordinate = await CalculateSurveys.ReturnCoordinateTangential(dblX, dblY, dblZ, dblTo - dblFrom, dblAzimuth, dblDip);

                    assayDesurveyDto.distTo.Add(dblTo);

                    assayDesurveyDto.x.Add(Convert.ToDouble(coordinate.x));
                    assayDesurveyDto.y.Add(Convert.ToDouble(coordinate.y));
                    assayDesurveyDto.z.Add(Convert.ToDouble(coordinate.z));

                }
                else
                {
                    assayDesurveyDto.distTo.Add(0.0);
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
                intervalDesurveyDto.dip.Add(dblDip);
                intervalDesurveyDto.azimuth.Add(dblAzimuth);
                intervalDesurveyDto.length.Add(Convert.ToDouble(dblTo - dblFrom));
                intervalDesurveyDto.Count++;
                intervalDesurveyDto.isInterval.Add(isSample);

                if (!bCollar)
                {
                    coordinate = await CalculateSurveys.ReturnCoordinateTangential(dblX, dblY, dblZ, dblTo - dblFrom, dblAzimuth, dblDip);

                    intervalDesurveyDto.distTo.Add(dblTo);
                    intervalDesurveyDto.x.Add(Convert.ToDouble(coordinate.x));
                    intervalDesurveyDto.y.Add(Convert.ToDouble(coordinate.y));
                    intervalDesurveyDto.z.Add(Convert.ToDouble(coordinate.z));

                }
                else
                {
                    intervalDesurveyDto.distTo.Add(0.0);

                    intervalDesurveyDto.x.Add(dblX);
                    intervalDesurveyDto.y.Add(dblY);
                    intervalDesurveyDto.z.Add(dblZ);
                }

            }
            else if (tableType == DrillholeTableType.continuous) //dblFrom is actually distance
            {
                ContinuousDesurveyDto continuousDesurveyDto = desurveyDto as ContinuousDesurveyDto;

                continuousDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
                continuousDesurveyDto.contId.Add(Convert.ToInt16(sampleAttr));
                continuousDesurveyDto.bhid.Add(hole);
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
                    continuousDesurveyDto.distFrom.Add(dblTo - dblFrom);

                }
                else
                {
                    continuousDesurveyDto.distFrom.Add(dblTo);
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
                // surveyDesurveyDto.distFrom.Add(dblTo);
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
                    surveyDesurveyDto.distFrom.Add(dblTo - dblFrom);

                }
                else
                {
                    surveyDesurveyDto.distFrom.Add(dblTo);
                    surveyDesurveyDto.x.Add(dblX);
                    surveyDesurveyDto.y.Add(dblY);
                    surveyDesurveyDto.z.Add(dblZ);
                }
            }

            else if (tableType == DrillholeTableType.collar)
            {
                CollarDesurveyDto collarDesurveyDto = desurveyDto as CollarDesurveyDto;

                collarDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
                collarDesurveyDto.bhid.Add(hole);
                collarDesurveyDto.dip.Add(dblDip);
                collarDesurveyDto.azimuth.Add(dblAzimuth);
                collarDesurveyDto.length.Add(Convert.ToDouble(dblTo - dblFrom));
                collarDesurveyDto.Count++;
                collarDesurveyDto.isCollar.Add(isSample);

                if (!bCollar)
                {
                    coordinate = await CalculateSurveys.ReturnCoordinateTangential(dblX, dblY, dblZ, dblTo - dblFrom, dblAzimuth, dblDip);
                    collarDesurveyDto.x.Add(Convert.ToDouble(coordinate.x));
                    collarDesurveyDto.y.Add(Convert.ToDouble(coordinate.y));
                    collarDesurveyDto.z.Add(Convert.ToDouble(coordinate.z));

                }
                else
                {
                    collarDesurveyDto.x.Add(dblX);
                    collarDesurveyDto.y.Add(dblY);
                    collarDesurveyDto.z.Add(dblZ);
                }

            }

            return coordinate;
        }
    }
}
