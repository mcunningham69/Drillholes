using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.Enum;
using System.IO;
using System.Xml.Linq;
using Drillholes.Domain;

namespace Drillholes.FileDialog
{
    public class FileExportFactory
    {
        ExportDesurveyResults _exportTo;

        public FileExportFactory(DrillholeImportFormat exportMode)
        {
            SetExportType(exportMode);
        }

        public void SetExportType(DrillholeImportFormat exportMode)
        {
            _exportTo = ExportDesurveyResults.exportType(exportMode);
        }

        public async Task<bool> ExportCollarTable(string outputName, string drillholeTableFile, string drillholeFields, string drillholeInputData, bool bAttributes)
        {
            return await _exportTo.ExportCollarTable(outputName, drillholeTableFile, drillholeFields, drillholeInputData, bAttributes);

        }
        public async Task<bool> ExportSurveyTable(string outputName, string drillholeTableFile, string drillholeCollarFields, string drillholeSurveyFields, string drillholeInputData, bool bAttributes)
        {
            return await _exportTo.ExportSurveyTable(outputName, drillholeTableFile, drillholeCollarFields, drillholeSurveyFields, drillholeInputData, bAttributes);

        }
        public async Task<bool> ExportAssayTable(string outputName, string drillholeTableFile, string drillholeCollarFields, string drillholeAssayFields, string drillholeInputData, bool bAttributes)
        {
            return await _exportTo.ExportAssayTable(outputName, drillholeTableFile, drillholeCollarFields, drillholeAssayFields, drillholeInputData, bAttributes);

        }
        public async Task<bool> ExportIntervalTable(string outputName, string drillholeTableFile, string drillholeCollarFields, string drillholeIntervalFields, string drillholeInputData, bool bAttributes)
        {
            return await _exportTo.ExportIntervalTable(outputName, drillholeTableFile, drillholeCollarFields, drillholeIntervalFields, drillholeInputData, bAttributes);

        }
        public async Task<bool> ExportContinuousTable(string outputName, string drillholeTableFile, string drillholeCollarFields, string drillholeContinuousFields, 
            string drillholeInputData, bool bAttributes, string defaultValue, bool bVertical)
        {
            return await _exportTo.ExportContinuousTable(outputName, drillholeTableFile, drillholeCollarFields, drillholeContinuousFields, drillholeInputData, bAttributes, defaultValue, bVertical);

        }

    }

    public abstract class ExportDesurveyResults
    {
        public static ExportDesurveyResults exportType(DrillholeImportFormat exportFormat)
        {
            switch (exportFormat)
            {
                case DrillholeImportFormat.text_csv:
                    return new TextCsvFormat();
                case DrillholeImportFormat.text_txt:
                    return null;
            }

            return null;

        }

        public abstract Task<bool> ExportCollarTable(string outputName, string drillholeTableFile, string drillholeFields, string drillholeInputData, bool bAttributes);
        public abstract Task<bool> ExportSurveyTable(string outputName, string drillholeTableFile, string drillholeCollarFields, string drillholeSurveyFields, string drillholeInputData, bool bAttributes);
        public abstract Task<bool> ExportAssayTable(string outputName, string drillholeTableFile, string drillholeCollarFields, string drillholeAssayFields, string drillholeInputData, bool bAttributes);
        public abstract Task<bool> ExportIntervalTable(string outputName, string drillholeTableFile, string drillholeCollarFields, string drillholeIntervalFields, string drillholeInputData, bool bAttributes);
        public abstract Task<bool> ExportContinuousTable(string outputName, string drillholeTableFile, string drillholeCollarFields, string drillholeContinuousFields, 
            string drillholeInputData, bool bAttributes, string defaultValue, bool bVertical);

    }

    public class TextCsvFormat : ExportDesurveyResults
    {
        public override async Task<bool> ExportAssayTable(string outputName, string drillholeTableFile, string drillholeCollarFields, string drillholeAssayFields, 
            string drillholeInputData, bool bAttributes)
        {
            //Load XML tables from project directory or temp folder
            XDocument xmlResults = XDocument.Load(drillholeTableFile);
            XDocument xmlCollarFields = XDocument.Load(drillholeCollarFields);
            XDocument xmlAssayFields = XDocument.Load(drillholeAssayFields);
            XDocument xmlInput = XDocument.Load(drillholeInputData);

            //Get each row as an XElement
            var desurvElements = xmlResults.Descendants(DrillholeConstants.drillholeDesurv).Descendants("TableType").Elements();
            var inputElements = xmlInput.Descendants(DrillholeConstants.drillholeData).Descendants("TableType").Descendants("Assays").Elements();

            #region Field names
            IEnumerable<XElement> mandatoryFields = null;
            IEnumerable<XElement> mandatoryAssayFields = null;

            IEnumerable<XElement> optionalFields = null;

            //Get each row as an XElement for mandatory fields
            mandatoryFields = xmlCollarFields.Descendants(DrillholeConstants.drillholeFields).Descendants("TableType").Elements().Where(g => g.Element("GroupName").Value == "Mandatory Fields");

            //get field names
            string bhid = mandatoryFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.holeID).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string x = mandatoryFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.x).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string y = mandatoryFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.y).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string z = mandatoryFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.z).Select(v => v.Attribute("Name").Value).FirstOrDefault();

            mandatoryAssayFields = xmlAssayFields.Descendants(DrillholeConstants.drillholeFields).Descendants("TableType").Elements().Where(g => g.Element("GroupName").Value == "Mandatory Fields");
            string assayHole = mandatoryAssayFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.holeID).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string from = mandatoryAssayFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.distFrom).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string to = mandatoryAssayFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.distTo).Select(v => v.Attribute("Name").Value).FirstOrDefault();


            List<string> header = new List<string>(); //need these for CSV header
            List<string> attributes = new List<string>(); //use to search input data based on option imported fields

            List<RowsForExport> rowsForExport = new List<RowsForExport>(); //helper class for writing results to CSV

            header.Add(bhid);
            header.Add(x);
            header.Add(y);
            header.Add(z);
            header.Add(from);
            header.Add(to);
            header.Add("Interval");

            if (bAttributes)
            {
                optionalFields = xmlAssayFields.Descendants(DrillholeConstants.drillholeFields).Descendants("TableType").Elements().Where(g => g.Element("GroupName").Value == "Other Fields")
                .Where(i => i.Element("ColumnImportAs").Value != "Not Imported"); //get optional fields that have been selected for importing

                foreach (var field in optionalFields)
                {
                    header.Add(field.Attribute("Name").Value);
                    attributes.Add(field.Attribute("Name").Value);
                }
            }

            header.Add("Comment"); //add a comment about coordinate type
            #endregion

            List<string> attributeValues = new List<string>();

            string holeID = "";
            string assayID = "";

            foreach (var desurvey in desurvElements)
            {
                holeID = desurvey.Attribute("Name").Value; //get name of hole

                var queryCoord = desurvey.Elements(); //break down coordinate type per hole, i.e. collar and toe

                string coordType = "";

                foreach (var coord in queryCoord)
                {
                    RowsForExport attributeRow = new RowsForExport();
                    assayID = coord.Element("AssayID").Value; //use ID for joining desurvey with input data

                    if (coord.Attribute("Type").Value == "Collar")
                    {
                        coordType = "Collar";
                    }
                    else if (coord.Attribute("Type").Value == "Toe")
                    {
                        coordType = "Toe";
                    }
                    else if (coord.Attribute("Type").Value == "Interval")
                        coordType = "Interval";

                    //add to the string list in the RowsForExport class
                    attributeRow.attributes.Add(holeID);
                    attributeRow.attributes.Add(coord.Element(x).Value);
                    attributeRow.attributes.Add(coord.Element(y).Value);
                    attributeRow.attributes.Add(coord.Element(z).Value);
                    attributeRow.attributes.Add(coord.Element(from).Value);
                    attributeRow.attributes.Add(coord.Element(to).Value);
                    double length = Convert.ToDouble(coord.Element(to).Value) - Convert.ToDouble(coord.Element(from).Value);
                    attributeRow.attributes.Add(length.ToString());


                    if (bAttributes)
                    {
                        var inputTest = inputElements.Where(i => i.Attribute("ID").Value == assayID); //search by optional field name in the input data table

                        foreach (var attribute in attributes)
                        {
                            attributeRow.attributes.Add(inputTest.Select(a => a.Element(attribute).Value).SingleOrDefault()); //add to the list
                        }
                    }

                    attributeRow.attributes.Add(coordType); //add type of coordiante

                    rowsForExport.Add(attributeRow); //add the class to a list of same class

                }
            }

            await CsvExport.ToCSV(outputName, header, rowsForExport); //export to CSV

            return true;
        }

        public override async Task<bool> ExportCollarTable(string outputName, string drillholeTableFile, string drillholeFields, string drillholeInputData, bool bAttributes)
        {
            //Load XML tables from project directory or temp folder
            XDocument xmlResults = XDocument.Load(drillholeTableFile);
            XDocument xmlFields = XDocument.Load(drillholeFields);
            XDocument xmlInput = XDocument.Load(drillholeInputData);

            //Get each row as an XElement
            var desurvElements = xmlResults.Descendants(DrillholeConstants.drillholeDesurv).Descendants("TableType").Elements();
            var inputElements = xmlInput.Descendants(DrillholeConstants.drillholeData).Descendants("TableType").Descendants("Collars").Elements();

            #region Field names
            IEnumerable<XElement> mandatoryFields = null;
            IEnumerable<XElement> optionalFields = null;

            //Get each row as an XElement for mandatory fields
            mandatoryFields = xmlFields.Descendants(DrillholeConstants.drillholeFields).Descendants("TableType").Elements().Where(g => g.Element("GroupName").Value == "Mandatory Fields");            

            //get field names
            string bhid = mandatoryFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.holeID).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string x = mandatoryFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.x).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string y = mandatoryFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.y).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string z = mandatoryFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.z).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string td = mandatoryFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.maxDepth).Select(v => v.Attribute("Name").Value).FirstOrDefault(); ;

            List<string> header = new List<string>(); //need these for CSV header
            List<string> attributes = new List<string>(); //use to search input data based on option imported fields

            List<RowsForExport> rowsForExport = new List<RowsForExport>(); //helper class for writing results to CSV

            header.Add(bhid);
            header.Add(x);
            header.Add(y);
            header.Add(z);
            header.Add(td);

            if (bAttributes)
            {
                optionalFields = xmlFields.Descendants(DrillholeConstants.drillholeFields).Descendants("TableType").Elements().Where(g => g.Element("GroupName").Value == "Other Fields")
                .Where(i => i.Element("ColumnImportAs").Value != "Not Imported"); //get optional fields that have been selected for importing

                foreach (var field in optionalFields)
                {
                    header.Add(field.Attribute("Name").Value);
                    attributes.Add(field.Attribute("Name").Value);
                }
            }

            header.Add("Comment"); //add a comment about coordinate type
            #endregion

            List<string> attributeValues = new List<string>();

            string holeID = "";
            string collarID = "";

            foreach (var desurvey in desurvElements)
            {
                collarID = desurvey.Attribute("ID").Value; //use ID for joining desurvey with input data
                holeID = desurvey.Attribute("Name").Value; //get name of hole

                var queryCoord = desurvey.Elements(); //break down coordinate type per hole, i.e. collar and toe

                string coordType = "";

                foreach (var coord in queryCoord)
                {
                    RowsForExport attributeRow = new RowsForExport();

                    if (coord.Attribute("Type").Value == "Collar")
                    {
                        coordType = "Collar";
                    }
                    else if (coord.Attribute("Type").Value == "Toe")
                    {
                        coordType = "Toe";
                    }

                    //add to the string list in the RowsForExport class
                    attributeRow.attributes.Add(holeID);
                    attributeRow.attributes.Add(coord.Element(x).Value);
                    attributeRow.attributes.Add(coord.Element(y).Value);
                    attributeRow.attributes.Add(coord.Element(z).Value);
                    attributeRow.attributes.Add(coord.Element(td).Value);

                    if (bAttributes)
                    {
                        var inputTest = inputElements.Where(i => i.Attribute("ID").Value == collarID); //search by optional field name in the input data table

                        foreach (var attribute in attributes)
                        {
                            attributeRow.attributes.Add(inputTest.Select(a => a.Element(attribute).Value).SingleOrDefault()); //add to the list
                        }
                    }

                    attributeRow.attributes.Add(coordType); //add type of coordiante

                    rowsForExport.Add(attributeRow); //add the class to a list of same class

                }
            }

            await CsvExport.ToCSV(outputName, header, rowsForExport); //export to CSV

            return true;
        }

        public override async Task<bool> ExportContinuousTable(string outputName, string drillholeTableFile, string drillholeCollarFields, string drillholeContinuousFields, 
            string drillholeInputData, bool bAttributes, string defaultValue, bool bVertical)
        {

            XDocument xmlResults = XDocument.Load(drillholeTableFile);
            XDocument xmlCollarFields = XDocument.Load(drillholeCollarFields);
            XDocument xmlContinuousFields = XDocument.Load(drillholeContinuousFields);
            XDocument xmlInput = XDocument.Load(drillholeInputData);

            //Get each row as an XElement
            var desurvElements = xmlResults.Descendants(DrillholeConstants.drillholeDesurv).Descendants("TableType").Elements();
            var inputElements = xmlInput.Descendants(DrillholeConstants.drillholeData).Descendants("TableType").Descendants("Continuous").Elements();

            #region Field names
            IEnumerable<XElement> mandatoryFields = null;
            IEnumerable<XElement> mandatoryContFields = null;

            IEnumerable<XElement> optionalFields = null;

            //Get each row as an XElement for mandatory fields
            mandatoryFields = xmlCollarFields.Descendants(DrillholeConstants.drillholeFields).Descendants("TableType").Elements().Where(g => g.Element("GroupName").Value == "Mandatory Fields");

            //get field names
            string bhid = mandatoryFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.holeID).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string x = mandatoryFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.x).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string y = mandatoryFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.y).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string z = mandatoryFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.z).Select(v => v.Attribute("Name").Value).FirstOrDefault();

            mandatoryContFields = xmlContinuousFields.Descendants(DrillholeConstants.drillholeFields).Descendants("TableType").Elements().Where(g => g.Element("GroupName").Value == "Mandatory Fields");
            string surveyHole = mandatoryContFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.holeID).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string distance = mandatoryContFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.distName).Select(v => v.Attribute("Name").Value).FirstOrDefault();

            List<string> header = new List<string>(); //need these for CSV header
            List<string> attributes = new List<string>(); //use to search input data based on option imported fields
            List<string> structures = new List<string>(); //add here for calculated fields from alpha, beta

            List<RowsForExport> rowsForExport = new List<RowsForExport>(); //helper class for writing results to CSV

            header.Add(bhid);
            header.Add(x);
            header.Add(y);
            header.Add(z);
            header.Add(distance);

            if (bAttributes)
            {
                optionalFields = xmlContinuousFields.Descendants(DrillholeConstants.drillholeFields).Descendants("TableType").Elements().Where(g => g.Element("GroupName").Value == "Other Fields")
                .Where(i => i.Element("ColumnImportAs").Value != "Not Imported"); //get optional fields that have been selected for importing

                foreach (var field in optionalFields)
                {
                    if (bVertical)
                    {
                        header.Add(field.Attribute("Name").Value);
                        attributes.Add(field.Attribute("Name").Value);
                    }
                    else
                    {
                        if (field.Attribute("Name").Value != "Beta")
                        {
                            if (field.Attribute("Name").Value == "Alpha")
                            {
                                structures.Add(DrillholeConstants.CalculatedDip);
                                structures.Add(DrillholeConstants.CalculatedAzim);
                            }
                            else if (field.Attribute("Name").Value == "Gamma")
                            {
                                structures.Add(DrillholeConstants.CalculatedPlunge);
                                structures.Add(DrillholeConstants.CalculatedTrend);
                            }
                            else
                            {
                                header.Add(field.Attribute("Name").Value);
                                attributes.Add(field.Attribute("Name").Value);
                            }
                        }
                    }
                }
            }

            //put the calculated fields at the end but before Comment
            if (structures.Count > 0)
            {
                foreach(string structure in structures)
                    header.Add(structure);

            }

            header.Add("Comment"); //add a comment about coordinate type
            #endregion

            List<string> attributeValues = new List<string>();

            string holeID = "";
            string continuousID = "";

            foreach (var desurvey in desurvElements)
            {
                holeID = desurvey.Attribute("Name").Value; //get name of hole

                var queryCoord = desurvey.Elements(); //break down coordinate type per hole, i.e. collar and toe

                string coordType = "";

                foreach (var coord in queryCoord)
                {
                    RowsForExport attributeRow = new RowsForExport();
                    continuousID = coord.Element("ContinuousID").Value; //use ID for joining desurvey with input data

                    if (coord.Attribute("Type").Value == "Collar")
                    {
                        coordType = "Collar";
                    }
                    else if (coord.Attribute("Type").Value == "Toe")
                    {
                        coordType = "Toe";
                    }
                    else if (coord.Attribute("Type").Value == "Distance")
                        coordType = "Distance";

                    //add to the string list in the RowsForExport class
                    attributeRow.attributes.Add(holeID);
                    attributeRow.attributes.Add(coord.Element(x).Value);
                    attributeRow.attributes.Add(coord.Element(y).Value);
                    attributeRow.attributes.Add(coord.Element(z).Value);
                    attributeRow.attributes.Add(coord.Element(distance).Value);

                    if (bAttributes)
                    {
                        foreach(var value in inputElements)
                        {
                            if (value.Attribute("ID").Value == continuousID)
                            {
                                var inputTest = value.Nodes();

                                foreach (var attribute in attributes)
                                {
                                    foreach(XElement node in inputTest)
                                    {
                                        XElement changeNode = node as XElement;

                                        if (changeNode.Name == attribute)
                                        {
                                            attributeRow.attributes.Add(changeNode.Value);
                                            break;
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }

                    if (structures.Count > 0)
                    {
                        foreach(XElement hole in desurvElements)
                        {
                            if (hole.Attribute("Name").Value == holeID)
                            {
                                var inputStructures = hole.Nodes();

                                foreach (var structure in structures)
                                {
                                    foreach (XElement node in inputStructures)
                                    {
                                        if (node.Attribute("Type").Value == coordType)
                                        {
                                            XElement changeNode = node as XElement;

                                            if (changeNode.Element("ContinuousID").Value == continuousID)
                                            {
                                                var values = changeNode.Elements().ToList();

                                                foreach (var val in values)
                                                {
                                                    if (val.Name == structure)
                                                    {
                                                        if (val.Value == "-99")
                                                            attributeRow.attributes.Add(defaultValue);
                                                        else
                                                            attributeRow.attributes.Add(val.Value);
                                                        break;
                                                    }
                                                }

                                                break;
                                            }

                                        }
                                    }
                                }
                                break;


                            }
                        }

                       
                    }

                    attributeRow.attributes.Add(coordType); //add type of coordiante

                    rowsForExport.Add(attributeRow); //add the class to a list of same class

                }
            }

            await CsvExport.ToCSV(outputName, header, rowsForExport); //export to CSV

            return true;
        }

        public override async Task<bool> ExportIntervalTable(string outputName, string drillholeTableFile, string drillholeCollarFields, string drillholeIntervalFields,
            string drillholeInputData, bool bAttributes)
        {
            //Load XML tables from project directory or temp folder
            XDocument xmlResults = XDocument.Load(drillholeTableFile);
            XDocument xmlCollarFields = XDocument.Load(drillholeCollarFields);
            XDocument xmlIntervalFields = XDocument.Load(drillholeIntervalFields);
            XDocument xmlInput = XDocument.Load(drillholeInputData);

            //Get each row as an XElement
            var desurvElements = xmlResults.Descendants(DrillholeConstants.drillholeDesurv).Descendants("TableType").Elements();
            var inputElements = xmlInput.Descendants(DrillholeConstants.drillholeData).Descendants("TableType").Descendants("Intervals").Elements();

            #region Field names
            IEnumerable<XElement> mandatoryFields = null;
            IEnumerable<XElement> mandatoryIntervalFields = null;

            IEnumerable<XElement> optionalFields = null;

            //Get each row as an XElement for mandatory fields
            mandatoryFields = xmlCollarFields.Descendants(DrillholeConstants.drillholeFields).Descendants("TableType").Elements().Where(g => g.Element("GroupName").Value == "Mandatory Fields");

            //get field names
            string bhid = mandatoryFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.holeID).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string x = mandatoryFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.x).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string y = mandatoryFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.y).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string z = mandatoryFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.z).Select(v => v.Attribute("Name").Value).FirstOrDefault();

            mandatoryIntervalFields = xmlIntervalFields.Descendants(DrillholeConstants.drillholeFields).Descendants("TableType").Elements().Where(g => g.Element("GroupName").Value == "Mandatory Fields");
            string assayHole = mandatoryIntervalFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.holeID).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string from = mandatoryIntervalFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.distFrom).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string to = mandatoryIntervalFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.distTo).Select(v => v.Attribute("Name").Value).FirstOrDefault();


            List<string> header = new List<string>(); //need these for CSV header
            List<string> attributes = new List<string>(); //use to search input data based on option imported fields

            List<RowsForExport> rowsForExport = new List<RowsForExport>(); //helper class for writing results to CSV

            header.Add(bhid);
            header.Add(x);
            header.Add(y);
            header.Add(z);
            header.Add(from);
            header.Add(to);
            header.Add("Interval");

            if (bAttributes)
            {
                optionalFields = xmlIntervalFields.Descendants(DrillholeConstants.drillholeFields).Descendants("TableType").Elements().Where(g => g.Element("GroupName").Value == "Other Fields")
                .Where(i => i.Element("ColumnImportAs").Value != "Not Imported"); //get optional fields that have been selected for importing

                foreach (var field in optionalFields)
                {
                    header.Add(field.Attribute("Name").Value);
                    attributes.Add(field.Attribute("Name").Value);
                }
            }

            header.Add("Comment"); //add a comment about coordinate type
            #endregion

            List<string> attributeValues = new List<string>();

            string holeID = "";
            string intervalID = "";

            foreach (var desurvey in desurvElements)
            {
                holeID = desurvey.Attribute("Name").Value; //get name of hole

                var queryCoord = desurvey.Elements(); //break down coordinate type per hole, i.e. collar and toe

                string coordType = "";

                foreach (var coord in queryCoord)
                {
                    RowsForExport attributeRow = new RowsForExport();
                    intervalID = coord.Element("IntervalID").Value; //use ID for joining desurvey with input data

                    if (coord.Attribute("Type").Value == "Collar")
                    {
                        coordType = "Collar";
                    }
                    else if (coord.Attribute("Type").Value == "Toe")
                    {
                        coordType = "Toe";
                    }
                    else if (coord.Attribute("Type").Value == "Interval")
                        coordType = "Interval";

                    //add to the string list in the RowsForExport class
                    attributeRow.attributes.Add(holeID);
                    attributeRow.attributes.Add(coord.Element(x).Value);
                    attributeRow.attributes.Add(coord.Element(y).Value);
                    attributeRow.attributes.Add(coord.Element(z).Value);
                    attributeRow.attributes.Add(coord.Element(from).Value);
                    attributeRow.attributes.Add(coord.Element(to).Value);
                    double length = Convert.ToDouble(coord.Element(to).Value) - Convert.ToDouble(coord.Element(from).Value);
                    attributeRow.attributes.Add(length.ToString());


                    if (bAttributes)
                    {
                        var inputTest = inputElements.Where(i => i.Attribute("ID").Value == intervalID); //search by optional field name in the input data table

                        foreach (var attribute in attributes)
                        {
                            attributeRow.attributes.Add(inputTest.Select(a => a.Element(attribute).Value).SingleOrDefault()); //add to the list
                        }
                    }

                    attributeRow.attributes.Add(coordType); //add type of coordiante

                    rowsForExport.Add(attributeRow); //add the class to a list of same class

                }
            }

            await CsvExport.ToCSV(outputName, header, rowsForExport); //export to CSV

            return true;
        }

        public override async Task<bool> ExportSurveyTable(string outputName, string drillholeTableFile, string drillholeCollarFields, string drillholeSurveyFields, string drillholeInputData, bool bAttributes)
        {

            XDocument xmlResults = XDocument.Load(drillholeTableFile);
            XDocument xmlCollarFields = XDocument.Load(drillholeCollarFields);
            XDocument xmlSurveyFields = XDocument.Load(drillholeSurveyFields);
            XDocument xmlInput = XDocument.Load(drillholeInputData);

            //Get each row as an XElement
            var desurvElements = xmlResults.Descendants(DrillholeConstants.drillholeDesurv).Descendants("TableType").Elements();
            var inputElements = xmlInput.Descendants(DrillholeConstants.drillholeData).Descendants("TableType").Descendants("Surveys").Elements();

            #region Field names
            IEnumerable<XElement> mandatoryFields = null;
            IEnumerable<XElement> mandatorySurveyFields = null;

            IEnumerable<XElement> optionalFields = null;

            //Get each row as an XElement for mandatory fields
            mandatoryFields = xmlCollarFields.Descendants(DrillholeConstants.drillholeFields).Descendants("TableType").Elements().Where(g => g.Element("GroupName").Value == "Mandatory Fields");

            //get field names
            string bhid = mandatoryFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.holeID).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string x = mandatoryFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.x).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string y = mandatoryFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.y).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string z = mandatoryFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.z).Select(v => v.Attribute("Name").Value).FirstOrDefault();

            mandatorySurveyFields = xmlSurveyFields.Descendants(DrillholeConstants.drillholeFields).Descendants("TableType").Elements().Where(g => g.Element("GroupName").Value == "Mandatory Fields");
            string surveyHole = mandatorySurveyFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.holeID).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string distance = mandatorySurveyFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.distName).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string dip = mandatorySurveyFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.dipName).Select(v => v.Attribute("Name").Value).FirstOrDefault();
            string azimuth = mandatorySurveyFields.Where(f => f.Element("ColumnImportAs").Value == DrillholeConstants.azimuthName).Select(v => v.Attribute("Name").Value).FirstOrDefault();

            List<string> header = new List<string>(); //need these for CSV header
            List<string> attributes = new List<string>(); //use to search input data based on option imported fields

            List<RowsForExport> rowsForExport = new List<RowsForExport>(); //helper class for writing results to CSV

            header.Add(bhid);
            header.Add(x);
            header.Add(y);
            header.Add(z);
            header.Add(distance);
            header.Add(dip);
            header.Add(azimuth);

            if (bAttributes)
            {
                optionalFields = xmlSurveyFields.Descendants(DrillholeConstants.drillholeFields).Descendants("TableType").Elements().Where(g => g.Element("GroupName").Value == "Other Fields")
                .Where(i => i.Element("ColumnImportAs").Value != "Not Imported"); //get optional fields that have been selected for importing

                foreach (var field in optionalFields)
                {
                    header.Add(field.Attribute("Name").Value);
                    attributes.Add(field.Attribute("Name").Value);
                }
            }

            header.Add("Comment"); //add a comment about coordinate type
            #endregion

            List<string> attributeValues = new List<string>();

            string holeID = "";
            string surveyID = "";

            foreach (var desurvey in desurvElements)
            {
                holeID = desurvey.Attribute("Name").Value; //get name of hole

                var queryCoord = desurvey.Elements(); //break down coordinate type per hole, i.e. collar and toe

                string coordType = "";

                foreach (var coord in queryCoord)
                {
                    RowsForExport attributeRow = new RowsForExport();
                    surveyID = coord.Element("SurveyID").Value; //use ID for joining desurvey with input data

                    if (coord.Attribute("Type").Value == "Collar")
                    {
                        coordType = "Collar";
                    }
                    else if (coord.Attribute("Type").Value == "Toe")
                    {
                        coordType = "Toe";
                    }
                    else if (coord.Attribute("Type").Value == "Survey")
                        coordType = "Survey";

                    //add to the string list in the RowsForExport class
                    attributeRow.attributes.Add(holeID);
                    attributeRow.attributes.Add(coord.Element(x).Value);
                    attributeRow.attributes.Add(coord.Element(y).Value);
                    attributeRow.attributes.Add(coord.Element(z).Value);
                    attributeRow.attributes.Add(coord.Element(distance).Value);
                    attributeRow.attributes.Add(coord.Element(dip).Value);
                    attributeRow.attributes.Add(coord.Element(azimuth).Value);

                    if (bAttributes)
                    {
                        var inputTest = inputElements.Where(i => i.Attribute("ID").Value == surveyID); //search by optional field name in the input data table

                        foreach (var attribute in attributes)
                        {
                            attributeRow.attributes.Add(inputTest.Select(a => a.Element(attribute).Value).SingleOrDefault()); //add to the list
                        }
                    }

                    attributeRow.attributes.Add(coordType); //add type of coordiante

                    rowsForExport.Add(attributeRow); //add the class to a list of same class

                }
            }

            await CsvExport.ToCSV(outputName, header, rowsForExport); //export to CSV

            return true;
        }
    }

    public class RowsForExport
    {
        public List<string> attributes { get; set; }

        public RowsForExport()
        {
            attributes = new List<string>();
        }
    }

    public static class CsvExport
    {
        public static async Task<bool> ToCSV(string fileName, List<string> header, List<RowsForExport>rows)
        {
            StreamWriter sw = new StreamWriter(fileName, false);

            //write the fieldnames to header
            foreach (string field in header)
            {
                sw.Write(field);
                sw.Write(",");
            }

            sw.Write(sw.NewLine); //new line

            foreach(var row in rows)
            {
                foreach(var value in row.attributes) //same values as columns
                {
                    sw.Write(value);
                    sw.Write(",");
                }

                sw.Write(sw.NewLine); //new line for each row
            }
            
            sw.Close();

            return true;
        }
    }

   
}
