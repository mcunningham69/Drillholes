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
            return await _desurveyHoles.CollarVerticalTrace(tableFields, drillholeValues);
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

        public async Task<ContinuousDesurveyDto> ContinuousVerticalTrace(ImportTableFields collarTableFields, ImportTableFields contTableFields, List<XElement> drillholeValues, bool bToe, 
            bool bCollar)
        {
            return await _desurveyHoles.ContinuousVerticalTrace(collarTableFields, contTableFields, drillholeValues, bToe, bCollar);
        }
        public async Task<ContinuousDesurveyDto> ContinuousCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields contTableFields, List<XElement> drillholeValues, 
            bool bToe, bool bCollar, bool bBottom)
        {
            return await _desurveyHoles.ContinuousCollarSurveyTrace(collarTableFields, contTableFields, drillholeValues, bToe, bCollar, bBottom);
        }
        public async Task<ContinuousDesurveyDto> ContinuousDownholeTrace(ImportTableFields collarTableFields, ImportTableFields contTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, 
            bool bToe, bool bCollar, bool bBottom)
        {
            return await _desurveyHoles.ContinuousDownholeTrace(collarTableFields, contTableFields, surveyTableFields, drillholeValues, bToe, bCollar, bBottom);
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
                    //        return new MinimumCurvature();
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

            public abstract Task<CollarDesurveyDto> CollarVerticalTrace(ImportTableFields tableFields, List<XElement> drillholeValues);
            public abstract Task<CollarDesurveyDto> CollarSurveyTrace(ImportTableFields tableFields, List<XElement> drillholeValues);

            public abstract Task<SurveyDesurveyDto> SurveyDownhole(ImportTableFields collarTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues);

            public abstract Task<AssayDesurveyDto> AssayVerticalTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar);
            public abstract Task<AssayDesurveyDto> AssayCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar);
            public abstract Task<AssayDesurveyDto> AssayDownholeTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar);

            public abstract Task<IntervalDesurveyDto> IntervalVerticalTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar);
            public abstract Task<IntervalDesurveyDto> IntervalCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar);
            public abstract Task<IntervalDesurveyDto> IntervalDownholeTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar);


            public abstract Task<ContinuousDesurveyDto> ContinuousVerticalTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues, 
                bool bToe, bool bCollar);
            public abstract Task<ContinuousDesurveyDto> ContinuousCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues, 
                bool bToe, bool bCollar, bool bBottom);
            public abstract Task<ContinuousDesurveyDto> ContinuousDownholeTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, ImportTableFields surveyTableFields, 
                List<XElement> drillholeValues, bool bToe, bool bCollar, bool bBottom);



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

                if (dipField == null || azimuthField == null || distanceField == null)
                {
                    throw new SurveyException("Check Dip, Azimuth and Distance fields. Assays");
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
                surveyElements = surveyElements.OrderBy(o => Convert.ToDouble(o.Element(distanceField).Value));

                XElement assElements = drillholeValues[2];
                var assayElements = assElements.Elements();
                assayElements = assayElements.OrderBy(o => Convert.ToDouble(o.Element(mFromField).Value));


                //CHECK FOR DUPLICATE HOLES AND CHANGE FLAG TO IGNORE ONE OF THEM
                var dupHoles = collarElements.GroupBy(d => d.Element(collarHoleID).Value).Where(group => group.Count() > 1).Select(group => group.Key).ToList();
                for(int i = 0; i< dupHoles.Count; i++)
                {
                    var selectDup = collarElements.Where(h => h.Element(collarHoleID).Value == dupHoles[i]).ToList();

                    bool bFirst = true;
                    foreach(var dup in selectDup)
                    {
                        if (!bFirst)
                        {
                            dup.Attribute("Ignore").Value = "true"; 
                        }

                        bFirst = false;
                    }
                }

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

                    double dblDip = 0.0, dblAzimuth = 0.0;

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

                        //  var distances = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").OrderByDescending(d => Convert.ToDouble(d.Element(distanceField).Value)).Select(d => d.Element(distanceField).Value).ToList();
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

                            var assayHole = assayElements.Where(h => h.Element(assayHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").ToList(); //get all samples for hole and ignroe those flagged

                            List<string> depthsFrom = assayHole.Select(f => f.Element(mFromField).Value).ToList();
                            List<string> depthsTo = assayHole.Select(f => f.Element(mToField).Value).ToList();

                            string assayHoleAttr = "";
                            int counter = 0;
                            foreach (var assayValue in assayHole) //loop through each sample interval for hole
                            {
                                assayHoleAttr = assayValue.Attribute("ID").Value;
                                string mFrom = assayValue.Element(mFromField).Value;
                                string mTo = assayValue.Element(mToField).Value;

                                if (azimuths.Count == 0)
                                {
                                    azimuths.Add("0");

                                    if (dips.Count == 0)
                                        dips.Add("-90.0");

                                    if (distances.Count == 0)
                                        distances.Add(totalDepth);
                                }

                                DownholeSurveys surveys = null;

                                int surveyCounter = 0;

                                surveys = await DesurveyMethods.ReturnDownholeSurveys(azimuths, dips, distances, mFrom, mTo);

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
                                                DrillholeTableType.assay, false, true, DrillholeDesurveyEnum.Tangential);
                                        }
                                        //TODO separate dip and azimuth
                                        bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, assayHoleAttr, null, null, mFrom, mTo);

                                        if (bCheckDistance)
                                        {
                                            dblFrom = surveys.distFrom[s];
                                            dblTo = surveys.distTo[s];

                                            coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, dblAzimuth, dblDip, holeAttr, assayHoleAttr,
                                                hole, assayDesurveyDto, DrillholeTableType.assay, true, false, DrillholeDesurveyEnum.Tangential);
                                        }
                                    }
                                    else
                                    {
                                        if (bCheckDistance)
                                        {
                                            dblFrom = surveys.distFrom[s];
                                            dblTo = surveys.distTo[s];

                                            coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, dblAzimuth, dblDip, holeAttr, assayHoleAttr, hole,
                                                assayDesurveyDto, DrillholeTableType.assay, true, false, DrillholeDesurveyEnum.Tangential);
                                        }
                                    }

                                    dblX = coordinate.x;
                                    dblY = coordinate.y;
                                    dblZ = coordinate.z;

                                    surveyCounter++;
                                }

                                counter++;
                            }

                            //check to add TD
                            if (bToe)
                            {
                                if (dblLength > dblTo)
                                {
                                    await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblTo, dblLength, dblAzimuth, dblDip, holeAttr, assayHoleAttr, hole, assayDesurveyDto,
                                        DrillholeTableType.assay, false, false, DrillholeDesurveyEnum.Tangential);
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
                AssayDesurveyDto assayDesurveyDto = await DesurveyMethods.AssayCollarSurveyTrace(collarTableFields, assayTableFields, drillholeValues, bToe, bCollar, DrillholeDesurveyEnum.Tangential);
                /*
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
                assayElements = assayElements.OrderBy(o => Convert.ToDouble(o.Element(mFromField).Value));

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
                                            DrillholeTableType.assay, false, true, DrillholeDesurveyEnum.Tangential);
                                    }
                                    //TODO separate dip and azimuth
                                    bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, assayHoleAttr, null, null, mFrom, mTo);

                                    if (bCheckDistance)
                                    {
                                        dblFrom = Convert.ToDouble(mFrom);
                                        dblTo = Convert.ToDouble(mTo);

                                        coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, dblAzimuth, dblDip, holeAttr, assayHoleAttr,
                                            hole, assayDesurveyDto, DrillholeTableType.assay, true, false, DrillholeDesurveyEnum.Tangential);
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
                                            assayDesurveyDto, DrillholeTableType.assay, true, false, DrillholeDesurveyEnum.Tangential);
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
                                        DrillholeTableType.assay, false, false, DrillholeDesurveyEnum.Tangential);
                                }
                            }
                        }
                    }
                }
                */
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
                AssayDesurveyDto assayDesurveyDto = await DesurveyMethods.AssayVerticalTrace(collarTableFields, assayTableFields, drillholeValues, bToe, bCollar, DrillholeDesurveyEnum.Tangential);
                //initialise object and set up lists
               /*AssayDesurveyDto assayDesurveyDto = new AssayDesurveyDto()
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
                                               DrillholeTableType.assay, false, true, DrillholeDesurveyEnum.Tangential);

                                    }

                                    bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, assayHoleAttr, null, null, mFrom, mTo);

                                    if (bCheckDistance)
                                    {
                                        //create first sample record
                                        dblFrom = Convert.ToDouble(mFrom);
                                        dblTo = Convert.ToDouble(mTo);

                                        coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, 0.0, -90, holeAttr, assayHoleAttr,
                                            hole, assayDesurveyDto, DrillholeTableType.assay, true, false, DrillholeDesurveyEnum.Tangential);

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
                                              DrillholeTableType.assay, true, false, DrillholeDesurveyEnum.Tangential);
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
                                        DrillholeTableType.assay, false, false, DrillholeDesurveyEnum.Tangential);
                                }


                            }
                        }
                    }

                }
                */
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
            public override async Task<CollarDesurveyDto> CollarVerticalTrace(ImportTableFields tableFields, List<XElement> drillholeValues)
            {
                CollarDesurveyDto collarDesurveyDto = await DesurveyMethods.CollarVerticalTrace(tableFields, drillholeValues, DrillholeDesurveyEnum.Tangential);

              /*  CollarDesurveyDto collarDesurveyDto = new CollarDesurveyDto()
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
                                            DrillholeTableType.collar, true, true, DrillholeDesurveyEnum.Tangential);

                        //store toe
                        await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, 0.0, dblLength, 0.0, -90.0, holeAttr, "", hole, collarDesurveyDto,
                                            DrillholeTableType.collar, false, false, DrillholeDesurveyEnum.Tangential);
                    }
                }*/

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
                CollarDesurveyDto collarDesurveyDto = await DesurveyMethods.CollarSurveyTrace(tableFields, drillholeValues, DrillholeDesurveyEnum.Tangential);
              
                return collarDesurveyDto;
            }
            #endregion

            #region Continuous

            /// <summary>
            /// Create a desurveyed continuous table (i.e. distance to field) using dip and azi from collar table
            /// </summary>
            /// <param name="collarTableFields"></param>
            /// <param name="continuousTableFields"></param>
            /// <param name="drillholeValues"></param>
            /// <param name="bToe"></param>
            /// <param name="bCollar"></param>
            /// <returns></returns>
            public override async Task<ContinuousDesurveyDto> ContinuousCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues, 
                bool bToe, bool bCollar, bool bBottom)
            {
                ContinuousDesurveyDto continuousDesurveyDto = await DesurveyMethods.ContinuousCollarSurveyTrace(collarTableFields,continuousTableFields,  drillholeValues, bToe, 
                    bCollar, DrillholeDesurveyEnum.Tangential, bBottom);
               
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
            public override async Task<ContinuousDesurveyDto> ContinuousDownholeTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, 
                bool bToe, bool bCollar, bool bBottom)
            {
                ContinuousDesurveyDto continuousDesurveyDto = new ContinuousDesurveyDto()
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
                
                if (dipField == null || azimuthField == null || distanceField == null)
                {
                    throw new SurveyException("Check Dip, Azimuth and Distance fields. Continuous");
                }

                //TODO - add assay fields 
                var continuousHoleID = continuousTableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var distField = continuousTableFields.Where(f => f.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();

                //structural measurement fields
                string alphaField = "";
                string betaField = "";
                string gammaField = "";

                bool bStructures = false;
                bool bGamma = false;

                //check for structure fields
                bStructures = await DesurveyMethods.CheckForAlphaBeta(continuousTableFields);

                if (bStructures)
                {
                    alphaField = continuousTableFields.Where(f => f.columnImportName == "Alpha").Select(f => f.columnHeader).SingleOrDefault();
                    betaField = continuousTableFields.Where(f => f.columnImportName == "Beta").Select(f => f.columnHeader).SingleOrDefault();

                    bGamma = await DesurveyMethods.CheckForGamma(continuousTableFields);

                    if (bGamma)
                        gammaField = continuousTableFields.Where(f => f.columnImportName == "Gamma").Select(f => f.columnHeader).SingleOrDefault();
                }
                #endregion

                double alpha = -99.0, beta = -99.0, gamma = -99.0;

                XElement elements = drillholeValues[0];
                var collarElements = elements.Elements();

                XElement surElements = drillholeValues[1];
                var surveyElements = surElements.Elements();
                surveyElements = surveyElements.OrderBy(o => Convert.ToDouble(o.Element(distanceField).Value));

                XElement contElements = drillholeValues[2];
                var continuousElements = contElements.Elements();
                continuousElements = continuousElements.Where(v => v.Element(continuousHoleID).Value != "-").OrderBy(o => Convert.ToDouble(o.Element(distField).Value));

                //CHECK FOR DUPLICATE HOLES AND CHANGE FLAG TO IGNORE ONE OF THEM
                 var dupHoles = collarElements.GroupBy(d => d.Element(collarHoleID).Value).Where(group => group.Count() > 1).Select(group => group.Key).ToList();
                for (int i = 0; i < dupHoles.Count; i++)
                {
                    var selectDup = collarElements.Where(h => h.Element(collarHoleID).Value == dupHoles[i]).ToList();

                    bool bFirst = true;
                    foreach (var dup in selectDup)
                    {
                        if (!bFirst)
                        {
                            dup.Attribute("Ignore").Value = "true";
                        }

                        bFirst = false;
                    }
                }

                //return collar coordiantes and length for all holes which are not flagged to be ignored
                var showElements = collarElements.Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE");

                foreach (XElement element in showElements)
                {
                    string hole = element.Element(collarHoleID).Value;

                    //get the values from XML
                    string holeAttr = element.Attribute("ID").Value;
                    
                    string xCoord = element.Element(xField).Value;
                    string yCoord = element.Element(yField).Value;
                    string zCoord = element.Element(zField).Value;
                    string totalDepth = element.Element(tdField).Value; //FOR NOW< IGNORE


                    double dblDip = 0.0, dblAzimuth = 0.0;


                    int surveyCount = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Count(); //check collar exists in assay table
                    if (surveyCount == 0)
                    {
                        dblAzimuth = 0.0;
                        dblDip = -90;
                    }

                    int contCount = continuousElements.Where(h => h.Element(continuousHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Count(); //check collar exists in assay table

                    if (contCount > 0)
                    {
                        //check values are number otherwise go to next hole
                        bool bCheck = await DesurveyMethods.ValidateValues(xCoord, yCoord, zCoord, totalDepth, holeAttr, null, null, null, null);
                        var distances = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Element(distanceField).Value).ToList();
                        var dips = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Element(dipField).Value).ToList();
                        var azimuths = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Element(azimuthField).Value).ToList();

                        if (bCheck)
                        {
                            double dblX = 0.0, dblY = 0.0, dblZ = 0.0, dblLength = 0.0, dblDistanceTo = 0.0, dblNewDistTo = 0.0, dblPreviousDistance = 0.0;

                            dblX = Convert.ToDouble(xCoord);
                            dblY = Convert.ToDouble(yCoord);
                            dblZ = Convert.ToDouble(zCoord);
                            dblLength = Convert.ToDouble(totalDepth);

                            var continuousHole = continuousElements.Where(h => h.Element(continuousHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").ToList(); //get all samples for hole and ignroe those flagged

                            List<string> depthsTo = continuousHole.Select(f => f.Element(distField).Value).ToList();

                            string continuousHoleAttr = "";
                            int counter = 0;
                            foreach (var continuousValue in continuousHole) //loop through each sample interval for hole
                            {
                                continuousHoleAttr = continuousValue.Attribute("ID").Value;
                                string mDist = continuousValue.Element(distField).Value;

                                //get structural measurements from oriented core
                                if (alphaField != "")
                                {
                                    string sAlpha = continuousValue.Element(alphaField).Value;
                                    string sBeta = continuousValue.Element(betaField).Value;

                                    if (Information.IsNumeric(sAlpha))
                                    {
                                        alpha = Convert.ToDouble(sAlpha);
                                        if (Information.IsNumeric(sBeta))
                                        {
                                            beta = Convert.ToDouble(sBeta);

                                            if (gammaField != "")
                                            {
                                                string sGamma = continuousValue.Element(gammaField).Value; ;

                                                if (Information.IsNumeric(sGamma))
                                                    gamma = Convert.ToDouble(sGamma);
                                            }
                                        }
                                    }
                                }

                                DownholeSurveys surveys = null;

                                int surveyCounter = 0;

                                if (azimuths.Count == 0)
                                {
                                    azimuths.Add("0");

                                    if (dips.Count == 0)
                                        dips.Add("-90.0");

                                    if (distances.Count == 0)
                                        distances.Add(totalDepth);
                                }

                                surveys = await DesurveyMethods.ReturnDownholeSurveys(azimuths, dips, distances, mDist);

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
                                            await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, 0, 0, dblAzimuth, dblDip, holeAttr, continuousHoleAttr, hole, continuousDesurveyDto,
                                                DrillholeTableType.continuous, false, true, DrillholeDesurveyEnum.Tangential, bStructures, bGamma, -99.0, -99.0, -99.0, bBottom, 0);
                                        }
                                        //TODO separate dip and azimuth
                                        bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, continuousHoleAttr, null, null, mDist, null);

                                        if (bCheckDistance)
                                        {
                                            dblDistanceTo = Convert.ToDouble(mDist);

                                            coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, 0, dblDistanceTo, dblAzimuth, dblDip, holeAttr, continuousHoleAttr,
                                                hole, continuousDesurveyDto, DrillholeTableType.continuous, true, false, DrillholeDesurveyEnum.Tangential, bStructures, bGamma, alpha, beta, gamma, bBottom, dblDistanceTo);
                                        }
                                    }
                                    else
                                    {
                                        bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, continuousHoleAttr, null, null, mDist, null);

                                        if (bCheckDistance)
                                        {
                                            dblPreviousDistance = dblDistanceTo;

                                            dblDistanceTo = Convert.ToDouble(mDist);

                                            dblNewDistTo = dblDistanceTo - dblPreviousDistance;

                                            coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, 0, dblNewDistTo, dblAzimuth, dblDip, holeAttr, continuousHoleAttr, hole,
                                                continuousDesurveyDto, DrillholeTableType.continuous, true, false, DrillholeDesurveyEnum.Tangential, bStructures, bGamma, alpha, beta, gamma, bBottom, dblDistanceTo);
                                        }
                                    }

                                    dblX = coordinate.x;
                                    dblY = coordinate.y;
                                    dblZ = coordinate.z;

                                    surveyCounter++;
                                }

                                gamma = -99;
                                alpha = -99;
                                beta = -99;

                                counter++;
                            }

                            //check to add TD
                            if (bToe)
                            {
                                if (dblLength > dblDistanceTo)
                                {
                                    await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblDistanceTo, dblLength, dblAzimuth, dblDip, holeAttr, continuousHoleAttr, hole, continuousDesurveyDto,
                                        DrillholeTableType.continuous, false, false, DrillholeDesurveyEnum.Tangential, bStructures, bGamma, alpha, beta, gamma, bBottom, dblDistanceTo);
                                }
                            }
                        }
                    }
                }

                return continuousDesurveyDto;
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
            public override async Task<ContinuousDesurveyDto> ContinuousVerticalTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues, 
                bool bToe, bool bCollar)
            {
                ContinuousDesurveyDto continuousDesurveyDto = await DesurveyMethods.ContinuousVerticalTrace(collarTableFields, continuousTableFields,drillholeValues, bToe, bCollar, 
                    DrillholeDesurveyEnum.Tangential, true);

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
                IntervalDesurveyDto intervalDesurveyDto = await DesurveyMethods.IntervalCollarSurveyTrace(collarTableFields, intervalTableFields, drillholeValues, bToe, bCollar, DrillholeDesurveyEnum.Tangential);

               /* IntervalDesurveyDto intervalDesurveyDto = new IntervalDesurveyDto()
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
                intervalElements = intervalElements.OrderBy(o => Convert.ToDouble(o.Element(mFromField).Value));

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
                                            DrillholeTableType.interval, false, true, DrillholeDesurveyEnum.Tangential);
                                    }
                                    //TODO separate dip and azimuth
                                    bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, intervalHoleAttr, null, null, mFrom, mTo);

                                    if (bCheckDistance)
                                    {
                                        dblFrom = Convert.ToDouble(mFrom);
                                        dblTo = Convert.ToDouble(mTo);

                                        coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, dblAzimuth, dblDip, holeAttr, intervalHoleAttr,
                                            hole, intervalDesurveyDto, DrillholeTableType.interval, true, false, DrillholeDesurveyEnum.Tangential);
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
                                            intervalDesurveyDto, DrillholeTableType.interval, true, false, DrillholeDesurveyEnum.Tangential);
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
                                        DrillholeTableType.interval, false, false, DrillholeDesurveyEnum.Tangential);
                                }
                            }
                        }
                    }
                } */

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
            public override async Task<IntervalDesurveyDto> IntervalDownholeTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
            {
                IntervalDesurveyDto intervalDesurveyDto = new IntervalDesurveyDto()
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

                if (dipField == null || azimuthField == null || distanceField == null)
                {
                    throw new SurveyException("Check Dip, Azimuth and Distance fields");
                }

                //TODO - add assay fields 
                var intervalHoleID = intervalTableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var mFromField = intervalTableFields.Where(f => f.columnImportName == DrillholeConstants.distFromName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                var mToField = intervalTableFields.Where(f => f.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
                #endregion

                XElement elements = drillholeValues[0];
                var collarElements = elements.Elements();

                XElement surElements = drillholeValues[1];
                var surveyElements = surElements.Elements();
                surveyElements = surveyElements.OrderBy(o => Convert.ToDouble(o.Element(distanceField).Value));

                XElement intElements = drillholeValues[2];
                var intervalElements = intElements.Elements();
                intervalElements = intervalElements.OrderBy(o => Convert.ToDouble(o.Element(mFromField).Value));

                //CHECK FOR DUPLICATE HOLES AND CHANGE FLAG TO IGNORE ONE OF THEM
                var dupHoles = collarElements.GroupBy(d => d.Element(collarHoleID).Value).Where(group => group.Count() > 1).Select(group => group.Key).ToList();
                for (int i = 0; i < dupHoles.Count; i++)
                {
                    var selectDup = collarElements.Where(h => h.Element(collarHoleID).Value == dupHoles[i]).ToList();

                    bool bFirst = true;
                    foreach (var dup in selectDup)
                    {
                        if (!bFirst)
                        {
                            dup.Attribute("Ignore").Value = "true";
                        }

                        bFirst = false;
                    }
                }

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

                    double dblDip = 0.0, dblAzimuth = 0.0;

                    int surveyCount = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Count(); //check collar exists in assay table
                    if (surveyCount == 0)
                    {
                        dblAzimuth = 0.0;
                        dblDip = -90;
                    }

                    int intervalCount = intervalElements.Where(h => h.Element(intervalHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Count(); //check collar exists in assay table

                    if (intervalCount > 0)
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

                            var intervalHole = intervalElements.Where(h => h.Element(intervalHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").ToList(); //get all samples for hole and ignroe those flagged

                            List<string> depthsFrom = intervalHole.Select(f => f.Element(mFromField).Value).ToList();
                            List<string> depthsTo = intervalHole.Select(f => f.Element(mToField).Value).ToList();

                            string intervalHoleAttr = "";
                            int counter = 0;
                            foreach (var intervalValue in intervalHole) //loop through each sample interval for hole
                            {
                                intervalHoleAttr = intervalValue.Attribute("ID").Value;
                                string mFrom = intervalValue.Element(mFromField).Value;
                                string mTo = intervalValue.Element(mToField).Value;

                                //in case no surveys
                                if (azimuths.Count == 0)
                                {
                                    azimuths.Add("0");

                                    if (dips.Count == 0)
                                        dips.Add("-90.0");

                                    if (distances.Count == 0)
                                        distances.Add(totalDepth);
                                }

                                DownholeSurveys surveys = null;
                                int surveyCounter = 0;

                                surveys = await DesurveyMethods.ReturnDownholeSurveys(azimuths, dips, distances, mFrom, mTo);

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
                                            await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, 0, 0, dblAzimuth, dblDip, holeAttr, intervalHoleAttr, hole, intervalDesurveyDto,
                                                DrillholeTableType.interval, false, true, DrillholeDesurveyEnum.Tangential);
                                        }
                                        //TODO separate dip and azimuth
                                        bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, intervalHoleAttr, null, null, mFrom, mTo);

                                        if (bCheckDistance)
                                        {
                                            dblFrom = surveys.distFrom[s];
                                            dblTo = surveys.distTo[s];

                                            coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, dblAzimuth, dblDip, holeAttr, intervalHoleAttr,
                                                hole, intervalDesurveyDto, DrillholeTableType.interval, true, false, DrillholeDesurveyEnum.Tangential);
                                        }
                                    }
                                    else
                                    {
                                        if (bCheckDistance)
                                        {
                                            dblFrom = surveys.distFrom[s];
                                            dblTo = surveys.distTo[s];

                                            coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, dblAzimuth, dblDip, holeAttr, intervalHoleAttr, hole,
                                                intervalDesurveyDto, DrillholeTableType.interval, true, false, DrillholeDesurveyEnum.Tangential);
                                        }
                                    }

                                    dblX = coordinate.x;
                                    dblY = coordinate.y;
                                    dblZ = coordinate.z;

                                    surveyCounter++;
                                }

                                counter++;
                            }

                            //check to add TD
                            if (bToe)
                            {
                                if (dblLength > dblTo)
                                {
                                    await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblTo, dblLength, dblAzimuth, dblDip, holeAttr, intervalHoleAttr, hole, intervalDesurveyDto,
                                        DrillholeTableType.interval, false, false, DrillholeDesurveyEnum.Tangential);
                                }
                            }
                        }
                    }
                }

                return intervalDesurveyDto;
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
                IntervalDesurveyDto intervalDesurveyDto = await DesurveyMethods.IntervalVerticalTrace(collarTableFields, intervalTableFields, drillholeValues, bToe, bCollar, DrillholeDesurveyEnum.Tangential);

               /* IntervalDesurveyDto intervalDesurveyDto = new IntervalDesurveyDto()
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
                                              DrillholeTableType.interval, false, true, DrillholeDesurveyEnum.Tangential);
                                    }

                                    bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, intervalHoleAttr, null, null, mFrom, mTo);

                                    if (bCheckDistance)
                                    {
                                        //create first sample record
                                        dblFrom = Convert.ToDouble(mFrom);
                                        dblTo = Convert.ToDouble(mTo);

                                        coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, 0.0, -90, holeAttr, intervalHoleAttr, hole, intervalDesurveyDto,
                                              DrillholeTableType.interval, true, false, DrillholeDesurveyEnum.Tangential);
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
                                             DrillholeTableType.interval, true, false, DrillholeDesurveyEnum.Tangential);
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
                                        intervalDesurveyDto, DrillholeTableType.interval, false, false, DrillholeDesurveyEnum.Tangential);
                                }
                            }
                        }
                    }

                }*/

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
                                            hole, surveyDesurveyDto, DrillholeTableType.survey, false, true, DrillholeDesurveyEnum.Tangential);

                                        if (surveyHole.Count > 1)
                                        {
                                            string newDist = surveyHole[i + 1].Element(distField).Value;
                                            bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, surveyHoleAttr, dip, azimuth, mDist, null);

                                            dblNewDistanceTo = Convert.ToDouble(newDist);

                                            coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblDistanceTo, dblNewDistanceTo, dblAzimuth, dblDip, holeAttr, surveyHoleAttr,
                                                hole, surveyDesurveyDto, DrillholeTableType.survey, true, false, DrillholeDesurveyEnum.Tangential);
                                        }
                                    }
                                }
                                else if (i == surveyHole.Count - 1)
                                {
                                    if (dblLength > dblNewDistanceTo)
                                    {

                                        coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblNewDistanceTo, dblLength, dblAzimuth, dblDip, holeAttr, surveyHoleAttr, hole,
                                                surveyDesurveyDto, DrillholeTableType.survey, false, false, DrillholeDesurveyEnum.Tangential);
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
                                                surveyDesurveyDto, DrillholeTableType.survey, true, false, DrillholeDesurveyEnum.Tangential);
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


}
