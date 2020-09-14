using Drillholes.Domain;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using Drillholes.Domain.Exceptions;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Drillholes.CreateDrillholes
{
    public static class DesurveyMethods
    {
        #region vertical
        public static async Task<CollarDesurveyDto> CollarVerticalTrace(ImportTableFields tableFields, List<XElement> drillholeValues, DrillholeDesurveyEnum desurveyEnum)
        {
            CollarDesurveyDto collarDesurveyDto = new CollarDesurveyDto()
            {
                desurveyType = desurveyEnum,
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

            List<double> dblDip = new List<double>();
            dblDip.Add(-90.0);

            List<double> dblAzimuth = new List<double>();
            dblAzimuth.Add(0.0);

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

                double dblX = 0.0, dblY = 0.0, dblZ = 0.0, dblLength = 0.0; //Vertical so only z will change

                if (bCheck)
                {
                    dblX = Convert.ToDouble(xCoord);
                    dblY = Convert.ToDouble(yCoord);
                    dblZ = Convert.ToDouble(zCoord);
                    dblLength = Convert.ToDouble(totalDepth);

                    await PopulateDesurveyObject(dblX, dblY, dblZ, 0, 0, dblAzimuth, dblDip, holeAttr, hole, collarDesurveyDto, true, true, desurveyEnum);

                    //store toe
                    await PopulateDesurveyObject(dblX, dblY, dblZ, 0, dblLength, dblAzimuth, dblDip, holeAttr, hole, collarDesurveyDto, false, false, desurveyEnum);
                }
            }

            return collarDesurveyDto;
        }
        public static async Task<AssayDesurveyDto> AssayVerticalTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar, DrillholeDesurveyEnum desurveyEnum)
        {
            //initialise object and set up lists
            AssayDesurveyDto assayDesurveyDto = new AssayDesurveyDto()
            {
                desurveyType = desurveyEnum,
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
                    bool bCheck = await ValidateValues(xCoord, yCoord, zCoord, totalDepth, holeAttr, null, null, null, null);

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
                                    await PopulateDesurveyObject(dblX, dblY, dblZ, 0, 0, 0.0, -90, holeAttr, assayHoleAttr, hole, assayDesurveyDto,
                                           DrillholeTableType.assay, false, true, desurveyEnum);

                                }

                                bCheckDistance = await ValidateValues(null, null, null, null, assayHoleAttr, null, null, mFrom, mTo);

                                if (bCheckDistance)
                                {
                                    //create first sample record
                                    dblFrom = Convert.ToDouble(mFrom);
                                    dblTo = Convert.ToDouble(mTo);

                                    coordinate = await PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, 0.0, -90, holeAttr, assayHoleAttr,
                                        hole, assayDesurveyDto, DrillholeTableType.assay, true, false, desurveyEnum);

                                }
                            }
                            else
                            {
                                bCheckDistance = await ValidateValues(null, null, null, null, assayHoleAttr, null, null, mFrom, mTo);

                                if (bCheckDistance)
                                {
                                    dblFrom = Convert.ToDouble(mFrom);
                                    dblTo = Convert.ToDouble(mTo);

                                    coordinate = await PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, 0.0, -90, holeAttr, assayHoleAttr, hole, assayDesurveyDto,
                                          DrillholeTableType.assay, true, false, desurveyEnum);
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
                                    DrillholeTableType.assay, false, false, desurveyEnum);
                            }


                        }
                    }
                }

            }

            return assayDesurveyDto;
        }

        public static async Task<IntervalDesurveyDto> IntervalVerticalTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar, DrillholeDesurveyEnum desurveyEnum)
        {
            IntervalDesurveyDto intervalDesurveyDto = new IntervalDesurveyDto()
            {
                desurveyType = desurveyEnum,
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
                    bool bCheck = await ValidateValues(xCoord, yCoord, zCoord, totalDepth, holeAttr, null, null, null, null);

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
                                    await PopulateDesurveyObject(dblX, dblY, dblZ, 0, dblLength, 0.0, -90, holeAttr, intervalHoleAttr, hole, intervalDesurveyDto,
                                          DrillholeTableType.interval, false, true, desurveyEnum);
                                }

                                bCheckDistance = await ValidateValues(null, null, null, null, intervalHoleAttr, null, null, mFrom, mTo);

                                if (bCheckDistance)
                                {
                                    //create first sample record
                                    dblFrom = Convert.ToDouble(mFrom);
                                    dblTo = Convert.ToDouble(mTo);

                                    coordinate = await PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, 0.0, -90, holeAttr, intervalHoleAttr, hole, intervalDesurveyDto,
                                          DrillholeTableType.interval, true, false, desurveyEnum);
                                }
                            }
                            else
                            {
                                bCheckDistance = await ValidateValues(null, null, null, null, intervalHoleAttr, null, null, mFrom, mTo);

                                if (bCheckDistance)
                                {
                                    dblFrom = Convert.ToDouble(mFrom);
                                    dblTo = Convert.ToDouble(mTo);

                                    coordinate = await PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, 0.0, -90, holeAttr, intervalHoleAttr, hole, intervalDesurveyDto,
                                         DrillholeTableType.interval, true, false, desurveyEnum);
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
                                await PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblLength, 0.0, -90.0, holeAttr, intervalHoleAttr, hole,
                                    intervalDesurveyDto, DrillholeTableType.interval, false, false, desurveyEnum);
                            }
                        }
                    }
                }

            }

            return intervalDesurveyDto;
        }

        public static async Task<ContinuousDesurveyDto> ContinuousVerticalTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar,
            DrillholeDesurveyEnum desurveyEnum, bool bBottom)
        {
            ContinuousDesurveyDto continuousDesurveyDto = new ContinuousDesurveyDto()
            {
                desurveyType = desurveyEnum,
                surveyType = DrillholeSurveyType.vertical
            };

            List<double> dblAzimuth = new List<double>();
            List<double> dblDip = new List<double>();
        //  List<double> dblSurveyDistance = new List<double>(); //survey distance

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

            //structural measurement fields
            //string alphaField = "";
            //string betaField = "";
            //string gammaField = "";

            //bool bStructures = false;
            //bool bGamma = false;

            ////check for structure fields
            //bStructures = await CheckForAlphaBeta(continuousTableFields);

            //if (bStructures)
            //{
            //    alphaField = continuousTableFields.Where(f => f.columnImportName == "Alpha").Select(f => f.columnHeader).SingleOrDefault();
            //    betaField = continuousTableFields.Where(f => f.columnImportName == "Beta").Select(f => f.columnHeader).SingleOrDefault();

            //    bGamma = await CheckForGamma(continuousTableFields);

            //    if (bGamma)
            //        gammaField = continuousTableFields.Where(f => f.columnImportName == "Gamma").Select(f => f.columnHeader).SingleOrDefault();
            //}
            #endregion

            XElement elements = drillholeValues[0];
            var collarElements = elements.Elements();

            XElement contElements = drillholeValues[2];
            var continuousElements = contElements.Elements();
            continuousElements = continuousElements.Where(v => v.Element(contHoleID).Value != "-").OrderBy(o => Convert.ToDouble(o.Element(distanceField).Value));

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
                    bool bCheck = await ValidateValues(xCoord, yCoord, zCoord, totalDepth, holeAttr, null, null, null, null);

                    if (bCheck)
                    {
                        double dblX = 0.0, dblY = 0.0, dblZ = 0.0, dblDistTo = 0.0, dblLength = 0.0,
                            dblPreviousDistance = 0.0; //Vertical so only z will change

                        dblDip.Add(-90.0);
                        dblAzimuth.Add(0.0);
                       // dblSurveyDistance.Add(0.0);

                        double alpha = -99.0, beta = -99.0, gamma = -99.0;

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
                            string distanceTo = contValue.Element(distanceField).Value; //distance to

                            //if (alphaField != "")
                            //{
                            //    string sAlpha = contValue.Element(alphaField).Value;
                            //    string sBeta = contValue.Element(betaField).Value;

                            //    if (Information.IsNumeric(sAlpha))
                            //    {
                            //        alpha = Convert.ToDouble(sAlpha);
                            //        if (Information.IsNumeric(sBeta))
                            //        {
                            //            beta = Convert.ToDouble(sBeta);

                            //            if (gammaField != "")
                            //            {
                            //                string sGamma = contValue.Element(gammaField).Value; ;

                            //                if (Information.IsNumeric(sGamma))
                            //                    gamma = Convert.ToDouble(sGamma);
                            //            }
                            //        }
                            //    }


                            //}

                            bool bCheckDistance = true;

                            if (counter == 0) //always set as condition above shows collar coordiantes are valid
                            {
                                if (bCollar)
                                {
                                    await PopulateDesurveyObject(dblX, dblY, dblZ, 0, 0, dblAzimuth, dblDip, holeAttr, contHoleAttr, hole, continuousDesurveyDto,
                                          DrillholeTableType.continuous, false, true, DrillholeDesurveyEnum.Tangential, false, false, alpha, beta, gamma, bBottom);
                                }

                                bCheckDistance = await ValidateValues(null, null, null, null, contHoleAttr, null, null, distanceTo, null);


                                if (bCheckDistance)
                                {
                                    dblDistTo = Convert.ToDouble(distanceTo);

                                    //dblFrom is actually Distance to so it is in the dblTo position
                                    coordinate = await PopulateDesurveyObject(dblX, dblY, dblZ, 0, dblDistTo, dblAzimuth, dblDip, holeAttr, contHoleAttr, hole, continuousDesurveyDto,
                                          DrillholeTableType.continuous, true, false, desurveyEnum, false, false, alpha, beta, gamma, bBottom);

                                    dblPreviousDistance = dblDistTo;
                                }
                            }
                            else
                            {
                                bCheckDistance = await ValidateValues(null, null, null, null, contHoleAttr, null, null, distanceTo, null);


                                if (bCheckDistance)
                                {
                                    dblDistTo = Convert.ToDouble(distanceTo);

                                    coordinate = await PopulateDesurveyObject(dblX, dblY, dblZ, dblPreviousDistance, dblDistTo, dblAzimuth, dblDip, holeAttr, contHoleAttr, hole, continuousDesurveyDto,
                                          DrillholeTableType.continuous, true, false, desurveyEnum, false, false, alpha, beta, gamma, bBottom);

                                    dblPreviousDistance = dblDistTo;
                                }

                            }

                            dblZ = coordinate.z;

                            counter++;
                        }

                        if (bToe)
                        {
                            if (dblLength > dblDistTo)
                            {
                                await PopulateDesurveyObject(dblX, dblY, dblZ, dblDistTo, dblLength, dblAzimuth, dblDip, holeAttr, contHoleAttr, hole,
                                    continuousDesurveyDto, DrillholeTableType.continuous, false, false, desurveyEnum, false, false, alpha, beta, gamma, bBottom);
                            }

                        }
                    }

                }
            }

            return continuousDesurveyDto;
        }
        #endregion

        #region Collar Survey
        public static async Task<CollarDesurveyDto> CollarSurveyTrace(ImportTableFields tableFields, List<XElement> drillholeValues, DrillholeDesurveyEnum desurveyEnum)
        {
            CollarDesurveyDto collarDesurveyDto = new CollarDesurveyDto()
            {
                desurveyType = desurveyEnum,
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

                List<double> dblDip = new List<double>();
                List<double> dblAzimuth = new List<double>();

                bool bCheck = false;
                if (hole != "")
                {
                    //TODO if dip and azimuth don't make sense then change to vertical
                    bCheck = await ValidateValues(xCoord, yCoord, zCoord, totalDepth, colID, dip, azimuth, null, null);
                }

                Coordinate3D coordinate = new Coordinate3D();
                double dblX = 0.0, dblY = 0.0, dblZ = 0.0, dblDistance = 0.0;

                if (bCheck)
                {
                    dblX = Convert.ToDouble(xCoord);
                    dblY = Convert.ToDouble(yCoord);
                    dblZ = Convert.ToDouble(zCoord);
                    dblDistance = Convert.ToDouble(totalDepth);
                    dblDip.Add(Convert.ToDouble(dip));
                    dblAzimuth.Add(Convert.ToDouble(azimuth));

                    await PopulateDesurveyObject(dblX, dblY, dblZ, 0, 0, dblAzimuth, dblDip, colID, hole, collarDesurveyDto, true, true, desurveyEnum);

                    //store toe
                    await PopulateDesurveyObject(dblX, dblY, dblZ, 0.0, dblDistance, dblAzimuth, dblDip, colID, hole, collarDesurveyDto, false, false, desurveyEnum);

                    collarDesurveyDto.Count++;

                }
            }

            return collarDesurveyDto;
        }

        public static async Task<AssayDesurveyDto> AssayCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar, DrillholeDesurveyEnum desurveyEnum)
        {
            AssayDesurveyDto assayDesurveyDto = new AssayDesurveyDto()
            {
                desurveyType = desurveyEnum,
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
                                        DrillholeTableType.assay, false, true, desurveyEnum);
                                }
                                //TODO separate dip and azimuth
                                bCheckDistance = await ValidateValues(null, null, null, null, assayHoleAttr, null, null, mFrom, mTo);

                                if (bCheckDistance)
                                {
                                    dblFrom = Convert.ToDouble(mFrom);
                                    dblTo = Convert.ToDouble(mTo);

                                    coordinate = await PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, dblAzimuth, dblDip, holeAttr, assayHoleAttr,
                                        hole, assayDesurveyDto, DrillholeTableType.assay, true, false, desurveyEnum);
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
                                        assayDesurveyDto, DrillholeTableType.assay, true, false, desurveyEnum);
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
                                    DrillholeTableType.assay, false, false, desurveyEnum);
                            }
                        }
                    }
                }
            }

            return assayDesurveyDto;
        }

        public static async Task<IntervalDesurveyDto> IntervalCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar, DrillholeDesurveyEnum desurveyEnum)
        {
            IntervalDesurveyDto intervalDesurveyDto = new IntervalDesurveyDto()
            {
                desurveyType = desurveyEnum,
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
                                    await PopulateDesurveyObject(dblX, dblY, dblZ, 0, 0, dblAzimuth, dblDip, holeAttr, intervalHoleAttr, hole, intervalDesurveyDto,
                                        DrillholeTableType.interval, false, true, desurveyEnum);
                                }
                                //TODO separate dip and azimuth
                                bCheckDistance = await ValidateValues(null, null, null, null, intervalHoleAttr, null, null, mFrom, mTo);

                                if (bCheckDistance)
                                {
                                    dblFrom = Convert.ToDouble(mFrom);
                                    dblTo = Convert.ToDouble(mTo);

                                    coordinate = await PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, dblAzimuth, dblDip, holeAttr, intervalHoleAttr,
                                        hole, intervalDesurveyDto, DrillholeTableType.interval, true, false, desurveyEnum);
                                }
                            }
                            else
                            {
                                bCheckDistance = await ValidateValues(null, null, null, null, intervalHoleAttr, dip, azimuth, mFrom, mTo);

                                if (bCheckDistance)
                                {
                                    dblFrom = Convert.ToDouble(mFrom);
                                    dblTo = Convert.ToDouble(mTo);

                                    coordinate = await PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, dblAzimuth, dblDip, holeAttr, intervalHoleAttr, hole,
                                        intervalDesurveyDto, DrillholeTableType.interval, true, false, desurveyEnum);
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
                                await PopulateDesurveyObject(dblX, dblY, dblZ, dblTo, dblLength, dblAzimuth, dblDip, holeAttr, intervalHoleAttr, hole, intervalDesurveyDto,
                                    DrillholeTableType.interval, false, false, desurveyEnum);
                            }
                        }
                    }
                }
            }

            return intervalDesurveyDto;
        }

        public static async Task<ContinuousDesurveyDto> ContinuousCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar,
            DrillholeDesurveyEnum desurveyEnum, bool bBottom)
        {
            ContinuousDesurveyDto continuousDesurveyDto = new ContinuousDesurveyDto()
            {
                desurveyType = desurveyEnum,
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

            //structural measurement fields
            string alphaField = "";
            string betaField = "";
            string gammaField = "";

            bool bStructures = false;
            bool bGamma = false;

            //check for structure fields
            bStructures = await CheckForAlphaBeta(continuousTableFields);

            if (bStructures)
            {
                alphaField = continuousTableFields.Where(f => f.columnImportName == "Alpha").Select(f => f.columnHeader).SingleOrDefault();
                betaField = continuousTableFields.Where(f => f.columnImportName == "Beta").Select(f => f.columnHeader).SingleOrDefault();

                bGamma = await CheckForGamma(continuousTableFields);

                if (bGamma)
                    gammaField = continuousTableFields.Where(f => f.columnImportName == "Gamma").Select(f => f.columnHeader).SingleOrDefault();
            }
            #endregion

            XElement elements = drillholeValues[0];
            var collarElements = elements.Elements();

            XElement contElements = drillholeValues[2];
            var continuousElements = contElements.Elements();
           // continuousElements = continuousElements.OrderBy(o => Convert.ToDouble(o.Element(distField).Value));
            continuousElements = continuousElements.Where(v => v.Element(continuousHoleID).Value != "-").OrderBy(o => Convert.ToDouble(o.Element(distField).Value));

            //return collar coordiantes and length for all holes which are not flagged to be ignored
            var showElements = collarElements.Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE");

            List<double> dblDip = new List<double>();
            List<double> dblAzimuth = new List<double>();

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

                int contCount = continuousElements.Where(h => h.Element(continuousHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Count(); //check collar exists 
                if (contCount > 0)
                {
                    //check values are number otherwise go to next hole
                    bool bCheck = await ValidateValues(xCoord, yCoord, zCoord, totalDepth, holeAttr, dip, azimuth, null, null);

                    if (bCheck)
                    {
                        double dblX = 0.0, dblY = 0.0, dblZ = 0.0, dblLength = 0.0, dblDistanceTo = 0.0, dblNewDistTo = 0.0, dblPreviousDistance = 0.0;
                        // dblDip = 0.0, dblAzimuth = 0.0, dblNewDistTo = 0.0, dblPreviousDistance = 0.0;

                        double alpha = 0.0, beta = 0.0, gamma = 0.0;

                        dblX = Convert.ToDouble(xCoord);
                        dblY = Convert.ToDouble(yCoord);
                        dblZ = Convert.ToDouble(zCoord);
                        dblLength = Convert.ToDouble(totalDepth);
                        dblAzimuth.Add(Convert.ToDouble(azimuth));
                        dblDip.Add(Convert.ToDouble(dip));

                        var continuousHole = continuousElements.Where(h => h.Element(continuousHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").ToList(); //get all samples for hole and ignroe those flagged

                        string continuousHoleAttr = "";
                        int counter = 0;
                        foreach (var continuousValue in continuousHole) //loop through each sample interval for hole
                        {
                            continuousHoleAttr = continuousValue.Attribute("ID").Value;
                            string mDist = continuousValue.Element(distField).Value;

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

                            bool bCheckDistance = true;

                            Coordinate3D coordinate = new Coordinate3D();

                            if (counter == 0) //always set as condition above shows collar coordiantes are valid
                            {
                                if (bCollar)
                                {
                                    await PopulateDesurveyObject(dblX, dblY, dblZ, 0, 0, dblAzimuth, dblDip, holeAttr, continuousHoleAttr, hole, continuousDesurveyDto,
                                        DrillholeTableType.continuous, false, true, desurveyEnum, bStructures, bGamma, alpha, beta, gamma, bBottom);
                                }

                                //TODO separate dip and azimuth
                                bCheckDistance = await ValidateValues(null, null, null, null, continuousHoleAttr, null, null, mDist, null);

                                if (bCheckDistance)
                                {
                                    dblDistanceTo = Convert.ToDouble(mDist);

                                    coordinate = await PopulateDesurveyObject(dblX, dblY, dblZ, 0, dblDistanceTo, dblAzimuth, dblDip, holeAttr, continuousHoleAttr,
                                        hole, continuousDesurveyDto, DrillholeTableType.continuous, true, false, desurveyEnum, bStructures, bGamma, alpha, beta, gamma, bBottom);

                                    dblPreviousDistance = dblDistanceTo;

                                }
                            }
                            else
                            {
                                bCheckDistance = await ValidateValues(null, null, null, null, continuousHoleAttr, dip, azimuth, mDist, null);

                                if (bCheckDistance)
                                {
                                    dblDistanceTo = Convert.ToDouble(mDist);

                                    coordinate = await PopulateDesurveyObject(dblX, dblY, dblZ, dblPreviousDistance, dblDistanceTo, dblAzimuth, dblDip, holeAttr, continuousHoleAttr, hole,
                                        continuousDesurveyDto, DrillholeTableType.continuous, true, false, desurveyEnum, bStructures, bGamma, alpha, beta, gamma, bBottom);

                                    dblPreviousDistance = dblDistanceTo;
                                }
                            }

                            dblX = coordinate.x;
                            dblY = coordinate.y;
                            dblZ = coordinate.z;

                            //reset values
                            alpha = -99.0;
                            beta = -99.0;
                            gamma = -99.0;

                            counter++;
                        }

                        //check to add TD
                        if (bToe)
                        {
                            if (dblLength > dblDistanceTo)
                            {
                                await PopulateDesurveyObject(dblX, dblY, dblZ, dblDistanceTo, dblLength, dblAzimuth, dblDip, holeAttr, continuousHoleAttr, hole, continuousDesurveyDto,
                                    DrillholeTableType.continuous, false, false, desurveyEnum, bStructures, bGamma, alpha, beta, gamma, bBottom);
                            }
                        }
                    }
                }
            }

            return continuousDesurveyDto;
        }
        #endregion

        #region Downhole

        public static async Task<SurveyDesurveyDto> SurveyDownhole(ImportTableFields collarTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues,
            DrillholeDesurveyEnum desurveyEnum)
        {
            SurveyDesurveyDto surveyDesurveyDto = new SurveyDesurveyDto()
            {
                desurveyType = desurveyEnum,
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
                    bool bCheck = await ValidateValues(xCoord, yCoord, zCoord, totalDepth, holeAttr, null, null, null, null);

                    if (bCheck)
                    {
                        double dblX = 0.0, dblY = 0.0, dblZ = 0.0, dblLength = 0.0, dblNewDistanceTo = 0.0, dblDistanceTo = 0.0;

                        dblX = Convert.ToDouble(xCoord);
                        dblY = Convert.ToDouble(yCoord);
                        dblZ = Convert.ToDouble(zCoord);
                        dblLength = Convert.ToDouble(totalDepth);

                        var surveyHole = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").ToList(); //get all samples for hole and ignroe those flagged

                        string surveyHoleAttr = "";
                        for (int i = 0; i < surveyHole.Count; i++)
                        {
                            //get list of azimuth and dip and survey distances
                            var dips = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Element(dipField).Value).ToList();
                            var azimuths = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Element(azimuthField).Value).ToList();
                            var distances = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Element(distField).Value).ToList();

                            List<double> dblDip = new List<double>();
                            List<double> dblAzimuth = new List<double>();
                            List<double> dblSurveyDistance = new List<double>();
                            //populate lists
                            for (int d = 0; d < dips.Count; d++)
                            {
                                if (Information.IsNumeric(dips[d]))
                                    dblDip.Add(Convert.ToDouble(dips[d]));
                                else
                                    dblDip.Add(-99);

                                if (Information.IsNumeric(azimuths[d]))
                                    dblAzimuth.Add(Convert.ToDouble(azimuths[d]));
                                else
                                    dblAzimuth.Add(-99);

                                if (Information.IsNumeric(distances[d]))
                                    dblSurveyDistance.Add(Convert.ToDouble(distances[d]));
                                else
                                    dblSurveyDistance.Add(-99);
                            }

                            surveyHoleAttr = surveyHole[i].Attribute("ID").Value;
                            string mDist = surveyHole[i].Element(distField).Value;
                            string dip = surveyHole[i].Element(dipField).Value;
                            string azimuth = surveyHole[i].Element(azimuthField).Value;

                            bool bCheckDistance = true;

                            Coordinate3D coordinate = new Coordinate3D();

                            if (i == 0)
                            {
                                bCheckDistance = await ValidateValues(null, null, null, null, surveyHoleAttr, dip, azimuth, mDist, null);

                                if (bCheckDistance)
                                {
                                    dblDistanceTo = Convert.ToDouble(mDist);
                                    
                                    //get collar first
                                    await PopulateDesurveyObject(dblX, dblY, dblZ, 0, 0, dblAzimuth, dblDip, holeAttr, surveyHoleAttr,
                                        hole, surveyDesurveyDto, false, true, desurveyEnum, i);

                                    if (surveyHole.Count > 1)
                                    {
                                        string newDist = surveyHole[i + 1].Element(distField).Value;
                                        bCheckDistance = await ValidateValues(null, null, null, null, surveyHoleAttr, dip, azimuth, mDist, null);

                                        dblNewDistanceTo = Convert.ToDouble(newDist);

                                        coordinate = await PopulateDesurveyObject(dblX, dblY, dblZ, dblDistanceTo, dblNewDistanceTo, dblAzimuth, dblDip, holeAttr, surveyHoleAttr,
                                            hole, surveyDesurveyDto, true, false, desurveyEnum, i);
                                    }
                                }
                            }
                            else if (i == surveyHole.Count - 1)
                            {
                                if (dblLength > dblNewDistanceTo)
                                {

                                    coordinate = await PopulateDesurveyObject(dblX, dblY, dblZ, dblNewDistanceTo, dblLength, dblAzimuth, dblDip, holeAttr, surveyHoleAttr, hole,
                                            surveyDesurveyDto, false, false, desurveyEnum, i);
                                }
                            }

                            else
                            {
                                if (surveyHole.Count > 1)
                                {
                                    string newDist = surveyHole[i + 1].Element(distField).Value;

                                    bCheckDistance = await ValidateValues(null, null, null, null, surveyHoleAttr, dip, azimuth, newDist, null); //TODO add dblDist and new dist

                                    //dblAzimuth = Convert.ToDouble(azimuth);
                                    //dblDip = Convert.ToDouble(dip);

                                    if (bCheckDistance)
                                    {
                                        dblDistanceTo = Convert.ToDouble(surveyHole[i].Element(distField).Value);

                                        dblNewDistanceTo = Convert.ToDouble(newDist);


                                        coordinate = await PopulateDesurveyObject(dblX, dblY, dblZ, dblDistanceTo, dblNewDistanceTo, dblAzimuth, dblDip, holeAttr, surveyHoleAttr, hole,
                                            surveyDesurveyDto, true, false, desurveyEnum, i);
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

        public static async Task<SurveyDesurveyDto> SurveyDownholeUpdate(ImportTableFields collarTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues,
    DrillholeDesurveyEnum desurveyEnum)
        {
            SurveyDesurveyDto surveyDesurveyDto = new SurveyDesurveyDto()
            {
                desurveyType = desurveyEnum,
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

            #region Input Data
            XElement elements = drillholeValues[0];
            var collarElements = elements.Elements();

            XElement survElements = drillholeValues[1];
            var surveyElements = survElements.Elements();
            #endregion

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
                    double dblX = 0.0, dblY = 0.0, dblZ = 0.0, dblLength = 0.0;

                    dblX = Convert.ToDouble(xCoord);
                    dblY = Convert.ToDouble(yCoord);
                    dblZ = Convert.ToDouble(zCoord);
                    dblLength = Convert.ToDouble(totalDepth);

                    var surveyHole = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").ToList(); //get all samples for hole and ignroe those flagged
                                                                                                                                                                             //get list of azimuth and dip and survey distances
                    var dips = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Element(dipField).Value).ToList();
                    var azimuths = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Element(azimuthField).Value).ToList();
                    var distances = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Element(distField).Value).ToList();
                    var surveyAttrs = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Attribute("ID").Value).ToList();

                    for (int i = 0; i < surveyHole.Count; i++)
                    {
                        #region populate dip, azimuth and distance arrays 
                        //only ever need the current and next value for desurvey purposes, i.e. 2 count
                        double[] dblDip = new double[2];
                        double[] dblAzimuth = new double[2];
                        double[] dblSurveyDistance = new double[2];

                        if (Information.IsNumeric(dips[i]))
                            dblDip[0] = Convert.ToDouble(dips[i]);
                        else
                            dblDip[0] = -99;

                        if (Information.IsNumeric(azimuths[i]))
                            dblAzimuth[0] = Convert.ToDouble(azimuths[i]);
                        else
                            dblAzimuth[0] = -99;

                        if (Information.IsNumeric(distances[i]))
                            dblSurveyDistance[0] = Convert.ToDouble(distances[i]);
                        else
                            dblSurveyDistance[0] = -99;

                        if (i < surveyHole.Count - 1)
                        {
                            if (Information.IsNumeric(dips[i + 1]))
                                dblDip[1] = Convert.ToDouble(dips[i + 1]);
                            else
                                dblDip[1] = -99;

                            if (Information.IsNumeric(azimuths[i + 1]))
                                dblAzimuth[1] = Convert.ToDouble(azimuths[i + 1]);
                            else
                                dblAzimuth[1] = -99;

                            if (Information.IsNumeric(distances[i + 1]))
                                dblSurveyDistance[1] = Convert.ToDouble(distances[i + 1]);
                            else
                                dblSurveyDistance[1] = -99;
                        }
                        else //dips and azimuths will averae to same
                        {
                            if (Information.IsNumeric(dips[i]))
                                dblDip[1] = Convert.ToDouble(dips[i]);
                            else
                                dblDip[1] = -99;

                            if (Information.IsNumeric(azimuths[i]))
                                dblAzimuth[1] = Convert.ToDouble(azimuths[i]);
                            else
                                dblAzimuth[1] = -99;

                            dblSurveyDistance[1] = dblLength;

                        }
                        #endregion

                        Coordinate3D coordinate = new Coordinate3D();

                        if (i == 0)
                        {
                            await PopulateDesurveyObject(dblX, dblY, dblZ, 0, dblAzimuth, dblDip, holeAttr, surveyAttrs[i],
                                hole, surveyDesurveyDto, false, true, desurveyEnum, dblSurveyDistance); //Collar


                            coordinate = await PopulateDesurveyObject(dblX, dblY, dblZ, dblLength, dblAzimuth, dblDip, holeAttr, surveyAttrs[i],
                                hole, surveyDesurveyDto, true, false, desurveyEnum, dblSurveyDistance);


                        }
                        else if (i == surveyHole.Count - 1)
                        {
                            if (dblLength > dblSurveyDistance.Max())
                            {

                                coordinate = await PopulateDesurveyObject(dblX, dblY, dblZ, dblLength, dblAzimuth, dblDip, holeAttr, surveyAttrs[i], hole,
                                        surveyDesurveyDto, false, false, desurveyEnum, dblSurveyDistance);
                            }
                        }

                        else
                        {
                            if (surveyHole.Count > 1)
                            {

                                coordinate = await PopulateDesurveyObject(dblX, dblY, dblZ, 0, dblAzimuth, dblDip, holeAttr, surveyAttrs[i], hole,
                                    surveyDesurveyDto, true, false, desurveyEnum, dblSurveyDistance);

                            }
                        }

                        dblX = coordinate.x;
                        dblY = coordinate.y;
                        dblZ = coordinate.z;

                    }

                }
            }

            return surveyDesurveyDto;
        }

        public static async Task<AssayDesurveyDto> AssayDownholeTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar, DrillholeDesurveyEnum desurveyEnum)
        {
            AssayDesurveyDto assayDesurveyDto = new AssayDesurveyDto()
            {
                desurveyType = desurveyEnum,
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
                                            DrillholeTableType.assay, false, true, desurveyEnum);
                                    }
                                    //TODO separate dip and azimuth
                                    bCheckDistance = await DesurveyMethods.ValidateValues(null, null, null, null, assayHoleAttr, null, null, mFrom, mTo);

                                    if (bCheckDistance)
                                    {
                                        dblFrom = surveys.distFrom[s];
                                        dblTo = surveys.distTo[s];

                                        coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, dblAzimuth, dblDip, holeAttr, assayHoleAttr,
                                            hole, assayDesurveyDto, DrillholeTableType.assay, true, false, desurveyEnum);
                                    }
                                }
                                else
                                {
                                    if (bCheckDistance)
                                    {
                                        dblFrom = surveys.distFrom[s];
                                        dblTo = surveys.distTo[s];

                                        coordinate = await DesurveyMethods.PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, dblAzimuth, dblDip, holeAttr, assayHoleAttr, hole,
                                            assayDesurveyDto, DrillholeTableType.assay, true, false, desurveyEnum);
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
                                    DrillholeTableType.assay, false, false, desurveyEnum);
                            }
                        }
                    }
                }
            }

            return assayDesurveyDto;
        }


        public static async Task<IntervalDesurveyDto> IntervalDownholeTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar,
             DrillholeDesurveyEnum desurveyEnum)
        {
            IntervalDesurveyDto intervalDesurveyDto = new IntervalDesurveyDto()
            {
                desurveyType = desurveyEnum,
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
                    bool bCheck = await ValidateValues(xCoord, yCoord, zCoord, totalDepth, holeAttr, null, null, null, null);

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

                            surveys = await ReturnDownholeSurveys(azimuths, dips, distances, mFrom, mTo);

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
                                        await PopulateDesurveyObject(dblX, dblY, dblZ, 0, 0, dblAzimuth, dblDip, holeAttr, intervalHoleAttr, hole, intervalDesurveyDto,
                                            DrillholeTableType.interval, false, true, desurveyEnum);
                                    }
                                    //TODO separate dip and azimuth
                                    bCheckDistance = await ValidateValues(null, null, null, null, intervalHoleAttr, null, null, mFrom, mTo);

                                    if (bCheckDistance)
                                    {
                                        dblFrom = surveys.distFrom[s];
                                        dblTo = surveys.distTo[s];

                                        coordinate = await PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, dblAzimuth, dblDip, holeAttr, intervalHoleAttr,
                                            hole, intervalDesurveyDto, DrillholeTableType.interval, true, false, desurveyEnum);
                                    }
                                }
                                else
                                {
                                    if (bCheckDistance)
                                    {
                                        dblFrom = surveys.distFrom[s];
                                        dblTo = surveys.distTo[s];

                                        coordinate = await PopulateDesurveyObject(dblX, dblY, dblZ, dblFrom, dblTo, dblAzimuth, dblDip, holeAttr, intervalHoleAttr, hole,
                                            intervalDesurveyDto, DrillholeTableType.interval, true, false, desurveyEnum);
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
                                await PopulateDesurveyObject(dblX, dblY, dblZ, dblTo, dblLength, dblAzimuth, dblDip, holeAttr, intervalHoleAttr, hole, intervalDesurveyDto,
                                    DrillholeTableType.interval, false, false, desurveyEnum);
                            }
                        }
                    }
                }
            }

            return intervalDesurveyDto;
        }

        public static async Task<ContinuousDesurveyDto> ContinuousDownholeTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues,
             bool bToe, bool bCollar, bool bBottom, DrillholeDesurveyEnum desurveyEnum)
        {
            ContinuousDesurveyDto continuousDesurveyDto = new ContinuousDesurveyDto()
            {
                desurveyType = desurveyEnum,
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
            var surveyDistanceField = surveyTableFields.Where(f => f.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            var dipField = surveyTableFields.Where(f => f.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            var azimuthField = surveyTableFields.Where(f => f.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();

            if (dipField == null || azimuthField == null || surveyDistanceField == null)
            {
                throw new SurveyException("Check Dip, Azimuth and Distance fields. Continuous");
            }

            List<double> dblAzimuth = new List<double>();
            List<double> dblDip = new List<double>();
            List<double> dblSurveyDistance = new List<double>(); //survey distance

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
            bStructures = await CheckForAlphaBeta(continuousTableFields);

            if (bStructures)
            {
                alphaField = continuousTableFields.Where(f => f.columnImportName == "Alpha").Select(f => f.columnHeader).SingleOrDefault();
                betaField = continuousTableFields.Where(f => f.columnImportName == "Beta").Select(f => f.columnHeader).SingleOrDefault();

                bGamma = await CheckForGamma(continuousTableFields);

                if (bGamma)
                    gammaField = continuousTableFields.Where(f => f.columnImportName == "Gamma").Select(f => f.columnHeader).SingleOrDefault();
            }
            #endregion

            double alpha = -99.0, beta = -99.0, gamma = -99.0;

            XElement elements = drillholeValues[0];
            var collarElements = elements.Elements();

            XElement surElements = drillholeValues[1];
            var surveyElements = surElements.Elements();
            surveyElements = surveyElements.OrderBy(o => Convert.ToDouble(o.Element(surveyDistanceField).Value));

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

                var surveyDistance = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Element(surveyDistanceField).Value).ToList(); 
                var surveyDip = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Element(dipField).Value).ToList();
                var surveyAzimuth = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Element(azimuthField).Value).ToList();

                //Populate lists
                if (surveyDip.Count > 0)
                {
                    for (int i = 0; i < surveyDip.Count; i++)
                    {
                        if (Information.IsNumeric(surveyDistance[i]))
                        {
                            dblSurveyDistance.Add(Convert.ToDouble(surveyDistance[i]));
                        }
                        else
                            dblSurveyDistance.Add(-999);

                        if (Information.IsNumeric(surveyDip[i]))
                        {
                            dblDip.Add(Convert.ToDouble(surveyDip[i]));
                        }
                        else
                            dblDip.Add(-999);

                        if (Information.IsNumeric(surveyAzimuth[i]))
                        {
                            dblAzimuth.Add(Convert.ToDouble(surveyAzimuth[i]));
                        }
                        else
                            dblAzimuth.Add(-999);
                    }
                }
                else
                {
                    dblAzimuth.Add(-99);
                    dblDip.Add(-99);
                    dblSurveyDistance.Add(0.0);
                }


                //get the values from XML
                string holeAttr = element.Attribute("ID").Value;

                string xCoord = element.Element(xField).Value;
                string yCoord = element.Element(yField).Value;
                string zCoord = element.Element(zField).Value;
                string totalDepth = element.Element(tdField).Value; //FOR NOW< IGNORE

              //  double dblDip = 0.0, dblAzimuth = 0.0;

                int surveyCount = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Count(); //check collar exists in assay table
                //if (surveyCount == 0)
                //{
                //    dblAzimuth.Add(0.0);
                //    dblDip = -90;
                //}

                int contCount = continuousElements.Where(h => h.Element(continuousHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Count(); //check collar exists in assay table

                if (contCount > 0)
                {
                    //check values are number otherwise go to next hole
                    bool bCheck = await ValidateValues(xCoord, yCoord, zCoord, totalDepth, holeAttr, null, null, null, null);
                    var surveyDistances = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Element(surveyDistanceField).Value).ToList();
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

                            if (azimuths.Count == 0) //assume vertical
                            {
                                azimuths.Add("0");

                                if (dips.Count == 0)
                                    dips.Add("-90.0");

                                if (surveyDistances.Count == 0)
                                    surveyDistances.Add(totalDepth);
                            }

                            surveys = await ReturnDownholeSurveys(azimuths, dips, surveyDistances, mDist);

                            for (int s = 0; s < surveys.distance.Count; s++)
                            {
                                //dblAzimuth = surveys.azimuth[s];
                                //dblDip = surveys.dip[s];

                                bool bCheckDistance = true;

                                Coordinate3D coordinate = new Coordinate3D();

                                if (counter == 0) //always set as condition above shows collar coordiantes are valid
                                {
                                    if (bCollar)
                                    {
                                        //await PopulateDesurveyObject(dblX, dblY, dblZ, 0, 0, dblAzimuth, dblDip, holeAttr, continuousHoleAttr, hole, continuousDesurveyDto,
                                        //    DrillholeTableType.continuous, false, true, desurveyEnum, bStructures, bGamma, -99.0, -99.0, -99.0, bBottom, 0);
                                    }
                                    //TODO separate dip and azimuth
                                    bCheckDistance = await ValidateValues(null, null, null, null, continuousHoleAttr, null, null, mDist, null);

                                    if (bCheckDistance)
                                    {
                                        dblDistanceTo = Convert.ToDouble(mDist);

                                        //coordinate = await PopulateDesurveyObject(dblX, dblY, dblZ, 0, dblDistanceTo, dblAzimuth, dblDip, holeAttr, continuousHoleAttr,
                                        //    hole, continuousDesurveyDto, DrillholeTableType.continuous, true, false, desurveyEnum, bStructures, bGamma, alpha, beta, gamma, bBottom, dblDistanceTo);
                                    }
                                }
                                else
                                {
                                    bCheckDistance = await ValidateValues(null, null, null, null, continuousHoleAttr, null, null, mDist, null);

                                    if (bCheckDistance)
                                    {
                                        dblPreviousDistance = dblDistanceTo;

                                        dblDistanceTo = Convert.ToDouble(mDist);

                                        dblNewDistTo = dblDistanceTo - dblPreviousDistance;

                                        //coordinate = await PopulateDesurveyObject(dblX, dblY, dblZ, 0, dblNewDistTo, dblAzimuth, dblDip, holeAttr, continuousHoleAttr, hole,
                                        //    continuousDesurveyDto, DrillholeTableType.continuous, true, false, desurveyEnum, bStructures, bGamma, alpha, beta, gamma, bBottom, dblDistanceTo);
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
                                //await PopulateDesurveyObject(dblX, dblY, dblZ, dblDistanceTo, dblLength, dblAzimuth, dblDip, holeAttr, continuousHoleAttr, hole, continuousDesurveyDto,
                                //    DrillholeTableType.continuous, false, false, desurveyEnum, bStructures, bGamma, alpha, beta, gamma, bBottom, dblDistanceTo);
                            }
                        }
                    }
                }
            }

            return continuousDesurveyDto;
        }
        #endregion


        //Collar desurvey
        private static async Task<Coordinate3D> PopulateDesurveyObject(double dblX, double dblY, double dblZ, double dblFrom, double dblTo, List<double> dblAzimuth, List<double> dblDip,
string holeAttr, string hole, CollarDesurveyDto collarDesurveyDto, bool isSample, bool bCollar, DrillholeDesurveyEnum drillholeDesurvey)
        {
            Coordinate3D coordinate = null;

            //CollarDesurveyDto collarDesurveyDto = desurveyDto as CollarDesurveyDto;

            collarDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
            collarDesurveyDto.bhid.Add(hole);
            collarDesurveyDto.dip.Add(dblDip[0]);
            collarDesurveyDto.azimuth.Add(dblAzimuth[0]);
            collarDesurveyDto.length.Add(Convert.ToDouble(dblTo - dblFrom));
            collarDesurveyDto.Count++;
            collarDesurveyDto.isCollar.Add(isSample);

            if (!bCollar)
            {
                if (drillholeDesurvey == DrillholeDesurveyEnum.Tangential)
                    coordinate = await CalculateSurveys.ReturnCoordinateTangential(dblX, dblY, dblZ, dblTo - dblFrom, dblAzimuth[0], dblDip[0]);
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

            return coordinate;
        }

        //Survey desurvey
        private static async Task<Coordinate3D> PopulateDesurveyObject(double dblX, double dblY, double dblZ, double dblFrom, double dblTo, List<double> dblAzimuth, List<double> dblDip,
string holeAttr, string sampleAttr, string hole, SurveyDesurveyDto surveyDesurveyDto, bool isSample, bool bCollar, DrillholeDesurveyEnum drillholeDesurvey, int index)
        {
            Coordinate3D coordinate = null;

            surveyDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
            surveyDesurveyDto.survId.Add(Convert.ToInt16(sampleAttr));
            surveyDesurveyDto.bhid.Add(hole);

            surveyDesurveyDto.Count++;
            surveyDesurveyDto.isSurvey.Add(isSample);

            if (!bCollar)
            {
                if (drillholeDesurvey == DrillholeDesurveyEnum.Tangential)
                {
                    surveyDesurveyDto.dip.Add(dblDip[index]);
                    surveyDesurveyDto.azimuth.Add(dblAzimuth[index]);

                    coordinate = await CalculateSurveys.TangentialMethod(dblX, dblY, dblZ, dblTo - dblFrom, dblAzimuth[index], dblDip[index]);
                }

                surveyDesurveyDto.x.Add(Convert.ToDouble(coordinate.x));
                surveyDesurveyDto.y.Add(Convert.ToDouble(coordinate.y));
                surveyDesurveyDto.z.Add(Convert.ToDouble(coordinate.z));
                surveyDesurveyDto.distSurvFrom.Add(dblTo - dblFrom);
            }
            else
            {
                surveyDesurveyDto.distSurvFrom.Add(dblTo);
                surveyDesurveyDto.x.Add(dblX);
                surveyDesurveyDto.y.Add(dblY);
                surveyDesurveyDto.z.Add(dblZ);
            }

            return coordinate;
        }

        private static async Task<Coordinate3D> PopulateDesurveyObject(double dblX, double dblY, double dblZ, double dblTD, double[] dblAzimuth, double[] dblDip,
string holeAttr, string sampleAttr, string hole, SurveyDesurveyDto surveyDesurveyDto, bool isSample, bool bCollar, DrillholeDesurveyEnum drillholeDesurvey, double[] surveyDistance)
        {
            Coordinate3D coordinate = null;

            surveyDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
            surveyDesurveyDto.survId.Add(Convert.ToInt16(sampleAttr));
            surveyDesurveyDto.bhid.Add(hole);

            surveyDesurveyDto.Count++;
            surveyDesurveyDto.isSurvey.Add(isSample);

            if (!bCollar)
            {
                double dblMD = surveyDistance[1] - surveyDistance[0];
                surveyDesurveyDto.dip.Add(dblDip[0]);
                surveyDesurveyDto.azimuth.Add(dblAzimuth[0]);

                if (drillholeDesurvey == DrillholeDesurveyEnum.Tangential)
                {
                    

                    coordinate = await CalculateSurveys.ReturnCoordinateTangential(dblX, dblY, dblZ, dblMD, dblAzimuth[0], dblDip[0]);
                }
                else if (drillholeDesurvey == DrillholeDesurveyEnum.AverageAngle)
               {
                   

                    coordinate = await CalculateSurveys.AverageAngleMethod(dblX, dblY, dblZ, dblDip, dblAzimuth, dblMD);
                }

                surveyDesurveyDto.x.Add(Convert.ToDouble(coordinate.x));
                surveyDesurveyDto.y.Add(Convert.ToDouble(coordinate.y));
                surveyDesurveyDto.z.Add(Convert.ToDouble(coordinate.z));
                surveyDesurveyDto.distSurvFrom.Add(surveyDistance[0]);
            }
            else
            {
                surveyDesurveyDto.distSurvFrom.Add(-99);
                surveyDesurveyDto.azimuth.Add(-99);
                surveyDesurveyDto.dip.Add(-99);
                surveyDesurveyDto.x.Add(dblX);
                surveyDesurveyDto.y.Add(dblY);
                surveyDesurveyDto.z.Add(dblZ);
            }

            return coordinate;
        }
        private static async Task<DownholeSurveys> ReturnDownholeSurveys(List<string> azimuths, List<string> dips, List<string> distances,
    string mFrom, string mTo)
        {
            double dblFrom = Convert.ToDouble(mFrom);
            double dblTo = Convert.ToDouble(mTo);

            //populate lists
            List<double> dblAzimuths = new List<double>();
            List<double> dblDips = new List<double>();
            List<double> dblDistances = new List<double>();

            int zeroCheck = 0;
            bool bZero = false;

            //populate numberical lists
            for (int i = 0; i < dips.Count; i++)
            {
                if (Information.IsNumeric(distances[i]))
                {
                    if (zeroCheck == 0)
                    {
                        if (distances[i] != "0")
                        {
                            dblDistances.Add(0);
                            bZero = true;
                        }
                    }
                    dblDistances.Add(Convert.ToDouble(distances[i]));
                }
                else
                {
                    dblDistances.Add(-99);

                }

                if (Information.IsNumeric(dips[i]))
                {
                    if (zeroCheck == 0 && bZero)
                        dblDips.Add(-90.0);

                    dblDips.Add(Convert.ToDouble(dips[i]));

                }
                else
                    dblDips.Add(-90);

                if (Information.IsNumeric(azimuths[i]))
                {
                    if (zeroCheck == 0 && bZero)
                        dblAzimuths.Add(0.0);

                    dblAzimuths.Add(Convert.ToDouble(azimuths[i]));

                }
                else
                    dblAzimuths.Add(0.0);

                zeroCheck++;
            }

            DownholeSurveys downholeSurveys = new DownholeSurveys();

            double maxInterval = dblDistances.Max();

            int fromIndex = await DepthIntervalIndex(dblFrom, dblDistances);
            int toIndex = await DepthIntervalIndex(dblTo, dblDistances);

            int indexDiff = toIndex - fromIndex;

            if (indexDiff == 0) //depths occur within same survey interval
            {
                downholeSurveys.distFrom.Add(dblFrom);
                downholeSurveys.distTo.Add(dblTo);
                downholeSurveys.distance.Add(dblTo - dblFrom);
                downholeSurveys.dip.Add(dblDips[fromIndex]);
                downholeSurveys.azimuth.Add(dblAzimuths[fromIndex]);
            }
            else if (indexDiff == 1) //straddles adjacent survey interval
            {
                downholeSurveys.distFrom.Add(dblFrom);
                downholeSurveys.distTo.Add(dblDistances[toIndex]);
                downholeSurveys.distance.Add(dblDistances[toIndex] - dblFrom);
                downholeSurveys.dip.Add(dblDips[fromIndex]);
                downholeSurveys.azimuth.Add(dblAzimuths[fromIndex]);

                downholeSurveys.distFrom.Add(dblDistances[toIndex]);
                downholeSurveys.distTo.Add(dblTo);
                downholeSurveys.distance.Add(dblTo - dblDistances[toIndex]);
                downholeSurveys.dip.Add(dblDips[toIndex]);
                downholeSurveys.azimuth.Add(dblAzimuths[toIndex]);
            }
            else if (indexDiff > 1) //straddles many survey intervals
            {

                for (int i = fromIndex; i < (indexDiff + fromIndex); i++)
                {
                    if (i == 0)
                    {
                        downholeSurveys.distFrom.Add(dblFrom);
                        downholeSurveys.distTo.Add(dblDistances[i]);
                        downholeSurveys.distance.Add(dblDistances[i] - dblFrom);
                        downholeSurveys.dip.Add(dblDips[fromIndex]);
                        downholeSurveys.azimuth.Add(dblAzimuths[fromIndex]);
                    }
                    else if (i < (indexDiff + fromIndex) - 1)
                    {
                        downholeSurveys.distFrom.Add(dblDistances[i]);
                        downholeSurveys.distTo.Add(dblDistances[i + 1]);
                        downholeSurveys.distance.Add(dblDistances[i + 1] - dblDistances[i]);
                        downholeSurveys.dip.Add(dblDips[i]);
                        downholeSurveys.azimuth.Add(dblAzimuths[i]);
                    }

                }
            }

            return downholeSurveys;
        }

        private static async Task<DownholeSurveys> ReturnDownholeSurveys(List<string> azimuths, List<string> dips, List<string> distances,
        string distanceTo)
        {
            double dblTo = Convert.ToDouble(distanceTo);

            //populate lists
            List<double> dblAzimuths = new List<double>();
            List<double> dblDips = new List<double>();
            List<double> dblDistances = new List<double>();

            int zeroCheck = 0;
            bool bZero = false;

            //populate numberical lists
            for (int i = 0; i < dips.Count; i++)
            {
                if (Information.IsNumeric(distances[i]))
                {
                    if (zeroCheck == 0)
                    {
                        if (distances[i] != "0")
                        {
                            dblDistances.Add(0);
                            bZero = true;
                        }
                    }
                    dblDistances.Add(Convert.ToDouble(distances[i]));
                }
                else
                {
                    dblDistances.Add(-99);

                }

                if (Information.IsNumeric(dips[i]))
                {
                    if (zeroCheck == 0 && bZero)
                        dblDips.Add(-90.0);

                    dblDips.Add(Convert.ToDouble(dips[i]));

                }
                else
                    dblDips.Add(-90);

                if (Information.IsNumeric(azimuths[i]))
                {
                    if (zeroCheck == 0 && bZero)
                        dblAzimuths.Add(0.0);

                    dblAzimuths.Add(Convert.ToDouble(azimuths[i]));

                }
                else
                    dblAzimuths.Add(0.0);

                zeroCheck++;
            }

            DownholeSurveys downholeSurveys = new DownholeSurveys();

            double maxInterval = dblDistances.Max();

            int toIndex = await DepthIntervalIndex(dblTo, dblDistances);

            if (toIndex == 0) //depths occur within same survey interval
            {
                downholeSurveys.distFrom.Add(0);
                downholeSurveys.distTo.Add(dblTo);
                downholeSurveys.distance.Add(dblTo - 0);
                downholeSurveys.dip.Add(dblDips[0]);
                downholeSurveys.azimuth.Add(dblAzimuths[0]);
            }
            else
            {
                downholeSurveys.distFrom.Add(dblDistances[toIndex]);
                downholeSurveys.distTo.Add(dblTo);
                downholeSurveys.distance.Add(dblTo - dblDistances[toIndex]);
                downholeSurveys.dip.Add(dblDips[toIndex]);
                downholeSurveys.azimuth.Add(dblAzimuths[toIndex]);

            }

            return downholeSurveys;
        }

        #region Structural measurements
        private static async Task<bool> CheckForAlphaBeta(ImportTableFields continuousTableFields)
        {
            var alphaBeta = continuousTableFields.Where(f => f.columnHeader == "Alpha" || f.columnHeader == "Beta").Where(m => m.columnImportAs != DrillholeConstants.notImported).Count();

            if (alphaBeta > 1)
                return true;
            else
                return false;
        }

        private static async Task<bool> CheckForGamma(ImportTableFields continuousTableFields)
        {
            var gamma = continuousTableFields.Where(f => f.columnHeader == "Gamma").Where(m => m.columnImportAs != DrillholeConstants.notImported).Count();

            if (gamma == 1)
                return true;
            else
                return false;
        }
        #endregion

        private static async Task<int> DepthIntervalIndex(double dblValue, List<double> distances)
        {
            int count = 0;

            for (int i = 0; i < distances.Count; i++)
            {
                double dblDistance = distances[i];
                double dblDistanceNext = 0.0;

                if (i < distances.Count - 1)
                    dblDistanceNext = distances[i + 1];
                else if (i == distances.Count - 1)
                {
                    return i;
                }

                if (dblValue == dblDistance)
                    return i;
                else if (dblValue > dblDistance && dblValue < dblDistanceNext)
                    return i;
            }


            return count;
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
        /// Populates the desurvey class with values - NOT continuous table 
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
        private static async Task<Coordinate3D> PopulateDesurveyObject(double dblX, double dblY, double dblZ, double dblFrom, double dblTo, double dblAzimuth, double dblDip,
string holeAttr, string sampleAttr, string hole, object desurveyDto, DrillholeTableType tableType, bool isSample, bool bCollar, DrillholeDesurveyEnum drillholeDesurvey)
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
                    if (drillholeDesurvey == DrillholeDesurveyEnum.Tangential)
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
                    if (drillholeDesurvey == DrillholeDesurveyEnum.Tangential)
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
            else if (tableType == DrillholeTableType.survey)
            {
                SurveyDesurveyDto surveyDesurveyDto = desurveyDto as SurveyDesurveyDto;

                surveyDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
                surveyDesurveyDto.survId.Add(Convert.ToInt16(sampleAttr));
                surveyDesurveyDto.bhid.Add(hole);
                surveyDesurveyDto.dip.Add(dblDip);
                surveyDesurveyDto.azimuth.Add(dblAzimuth);
                surveyDesurveyDto.Count++;
                surveyDesurveyDto.isSurvey.Add(isSample);

                if (!bCollar)
                {
                    if (drillholeDesurvey == DrillholeDesurveyEnum.Tangential)
                        coordinate = await CalculateSurveys.TangentialMethod(dblX, dblY, dblZ, dblTo - dblFrom, dblAzimuth, dblDip);


                    surveyDesurveyDto.x.Add(Convert.ToDouble(coordinate.x));
                    surveyDesurveyDto.y.Add(Convert.ToDouble(coordinate.y));
                    surveyDesurveyDto.z.Add(Convert.ToDouble(coordinate.z));
                    surveyDesurveyDto.distSurvFrom.Add(dblTo - dblFrom);

                }
                else
                {
                    surveyDesurveyDto.distSurvFrom.Add(dblTo);
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
                    if (drillholeDesurvey == DrillholeDesurveyEnum.Tangential)
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

        //Continuous table only
        private static async Task<Coordinate3D> PopulateDesurveyObject(double dblX, double dblY, double dblZ, double dblFrom, double dblTo, List<double> dblAzimuth, List<double> dblDip,
string holeAttr, string sampleAttr, string hole, object desurveyDto, DrillholeTableType tableType, bool isSample, bool bCollar, DrillholeDesurveyEnum drillholeDesurvey,
bool bStructure, bool bGamma, double alpha, double beta, double gamma, bool bBottom)
        {
            Coordinate3D coordinate = null;

            ContinuousDesurveyDto continuousDesurveyDto = desurveyDto as ContinuousDesurveyDto;

            continuousDesurveyDto.colId.Add(Convert.ToInt16(holeAttr));
            continuousDesurveyDto.contId.Add(Convert.ToInt16(sampleAttr));
            continuousDesurveyDto.bhid.Add(hole);
            continuousDesurveyDto.Count++;
            continuousDesurveyDto.isContinuous.Add(isSample);

            if (!bCollar)
            {
                if (drillholeDesurvey == DrillholeDesurveyEnum.Tangential)
                {
                    continuousDesurveyDto.dip.Add(dblDip[0]);
                    continuousDesurveyDto.azimuth.Add(dblAzimuth[0]);
                    continuousDesurveyDto.distFrom.Add(dblFrom);
                    continuousDesurveyDto.distTo.Add(dblTo);

                    coordinate = await CalculateSurveys.ReturnCoordinateTangential(dblX, dblY, dblZ, dblTo - dblFrom, dblAzimuth[0], dblDip[0]); //dblDistance was dblTo - dblFrom
                }
                else if (drillholeDesurvey == DrillholeDesurveyEnum.AverageAngle)
                {

                  //  coordinate = await CalculateSurveys.AverageAngleMethod(dblX, dblY, dblZ, dblAzimuth, dblDip, dblTo - dblFrom);
                }
                else if (drillholeDesurvey == DrillholeDesurveyEnum.BalancedTangential)
                {
                    coordinate = await CalculateSurveys.BalancedTangentialMethod(dblX, dblY, dblZ, dblAzimuth, dblDip, dblTo - dblFrom);

                }
                else if (drillholeDesurvey == DrillholeDesurveyEnum.MinimumCurvature)
                {
                    coordinate = await CalculateSurveys.MinimumCurvatureMethod(dblX, dblY, dblZ, dblAzimuth, dblDip, dblTo - dblFrom);

                }
                else if (drillholeDesurvey == DrillholeDesurveyEnum.RadiusCurvature)
                {
                    coordinate = await CalculateSurveys.RadiusCurvatureMethod(dblX, dblY, dblZ, dblAzimuth, dblDip, dblTo - dblFrom);

                }

                continuousDesurveyDto.x.Add(Convert.ToDouble(coordinate.x));
                continuousDesurveyDto.y.Add(Convert.ToDouble(coordinate.y));
                continuousDesurveyDto.z.Add(Convert.ToDouble(coordinate.z));

                //if calculated values then add to object
                if (bStructure)
                {
                    if (alpha != -99 && beta != -99)
                    {
                        double calcDip = await AlphaBetaGamma.CalculateDip(dblAzimuth[0], dblDip[0], alpha, beta, bBottom);
                        double calcAzi = await AlphaBetaGamma.CalculateDipDirection(dblAzimuth[0], dblDip[0], alpha, beta, bBottom);


                        continuousDesurveyDto.CalculatedDip.Add(calcDip);
                        continuousDesurveyDto.CalculatedAzimuth.Add(calcAzi);

                        if (bGamma && gamma != -99)
                        {
                            CalculatedPlungeAndTrend lineation = new CalculatedPlungeAndTrend();
                            lineation = await AlphaBetaGamma.CalculateGammaValues(dblAzimuth[0], dblDip[0], alpha, beta, gamma);

                            if (lineation != null)
                            {
                                continuousDesurveyDto.CalculatedPlunge.Add(lineation.lineation_plunge);
                                continuousDesurveyDto.CalculatedTrend.Add(lineation.lineation_trend);
                            }
                            else
                            {
                                continuousDesurveyDto.CalculatedPlunge.Add(-99);
                                continuousDesurveyDto.CalculatedTrend.Add(-99);
                            }
                        }
                        else
                        {
                            continuousDesurveyDto.CalculatedPlunge.Add(-99);
                            continuousDesurveyDto.CalculatedTrend.Add(-99);
                        }
                    }
                    else
                    {
                        continuousDesurveyDto.CalculatedDip.Add(-99);
                        continuousDesurveyDto.CalculatedAzimuth.Add(-99);
                        continuousDesurveyDto.CalculatedPlunge.Add(-99);
                        continuousDesurveyDto.CalculatedTrend.Add(-99);
                    }
                }

            }
            else
            {
                continuousDesurveyDto.distFrom.Add(-99);
                continuousDesurveyDto.distTo.Add(-99);
                continuousDesurveyDto.x.Add(dblX);
                continuousDesurveyDto.y.Add(dblY);
                continuousDesurveyDto.z.Add(dblZ);
                continuousDesurveyDto.dip.Add(-99);
                continuousDesurveyDto.azimuth.Add(-99);

                continuousDesurveyDto.CalculatedDip.Add(-99);
                continuousDesurveyDto.CalculatedAzimuth.Add(-99);
                continuousDesurveyDto.CalculatedPlunge.Add(-99);
                continuousDesurveyDto.CalculatedTrend.Add(-99);
            }

            return coordinate;
        }
    }
}
