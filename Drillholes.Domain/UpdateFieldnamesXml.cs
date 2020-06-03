using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain.Enum;

namespace Drillholes.Domain
{
    public static class UpdateFieldnamesXml
    {
        public static void UpdateFieldnamesXML(DrillholeTableType TableType, ImportTableFields mappedFields, string collarKey,
            string otherKey, string tableName)
        {
            if (mappedFields == null)
                return;

            //load preferences from XML
            XDocument fileName = ReturnXML();

            if (fileName == null)
                fileName = CreateXML();

            List<string> strFieldname = new List<string>();
            List<string> strFieldvalue = new List<string>();
            int counter = 0;

            string strTableType = "";

            if (TableType == DrillholeTableType.collar)
            {
                strTableType = DrillholeConstants._Collar;
                UpdateTableName(strTableType, fileName, tableName);

                UpdateFieldName(strTableType, "Constraint", collarKey, fileName);

                foreach (ImportTableField _field in mappedFields)
                {
                    if (_field.groupName == "Mandatory Fields")
                    {
                        strFieldname.Add(_field.columnImportName);
                        strFieldvalue.Add(_field.columnHeader);
                        counter++;
                    }
                    else if (_field.columnImportName == "Azimuth" || _field.columnImportName == "Dip")
                    {
                        strFieldname.Add(_field.columnImportName.ToLower());
                        strFieldvalue.Add(_field.columnHeader);
                        counter++;
                    }

                    if (counter == 7)
                        break;
                }
            }
            else if (TableType == DrillholeTableType.survey)
            {
                strTableType = DrillholeConstants._Survey;
                UpdateTableName(strTableType, fileName, tableName);

                UpdateFieldName(strTableType, "Constraint", otherKey, fileName);
                UpdateFieldName(strTableType, "Foreign", collarKey, fileName);

                foreach (ImportTableField _field in mappedFields)
                {
                    if (_field.groupName == "Mandatory Fields")
                    {
                        strFieldname.Add(_field.columnImportName);
                        strFieldvalue.Add(_field.columnHeader);
                        counter++;
                    }

                    if (counter == 4)
                        break;
                }

            }
            else if (TableType == DrillholeTableType.assay)
            {
                strTableType = DrillholeConstants._Assay;
                UpdateTableName(strTableType, fileName, tableName);

                UpdateFieldName(strTableType, "Constraint", otherKey, fileName);
                UpdateFieldName(strTableType, "Foreign", collarKey, fileName);

                foreach (ImportTableField _field in mappedFields)
                {
                    if (_field.groupName == "Mandatory Fields")
                    {
                        strFieldname.Add(_field.columnImportName);
                        strFieldvalue.Add(_field.columnHeader);
                        counter++;
                    }


                    if (counter == 3)
                        break;
                }

            }
            else if (TableType == DrillholeTableType.interval)
            {
                strTableType = DrillholeConstants._Interval;
                UpdateTableName(strTableType, fileName, tableName);

                UpdateFieldName(strTableType, "Constraint", otherKey, fileName);
                UpdateFieldName(strTableType, "Foreign", collarKey, fileName);

                foreach (ImportTableField _field in mappedFields)
                {
                    if (_field.groupName == "Mandatory Fields")
                    {
                        strFieldname.Add(_field.columnImportName);
                        strFieldvalue.Add(_field.columnHeader);
                        counter++;
                    }

                    if (counter == 3)
                        break;
                }
            }

            UpdateFieldNames(strTableType, strFieldname, strFieldvalue, fileName);

        }

        public static void UpdateFieldnameInXml(string strTableName, string strName, string strValue)
        {
            //load preferences from XML
            XDocument fileName = ReturnXML();

            if (fileName == null)
                fileName = CreateXML();

            UpdateFieldName(strTableName, strName, strValue, fileName);

        }

        private static void UpdateFieldName(string strTableName, string strFieldName, string strFieldValue, XDocument xmlFile)
        {

            XElement mElement = xmlFile.Root.Descendants(strTableName).Select(e => e.Element(strFieldName)).FirstOrDefault();

            if (strFieldValue != null)
            {
                mElement.Value = strFieldValue;

                SaveXML(xmlFile, GetFullPathAndFilename());
            }
        }

        private static void UpdateTableName(string strTableName, XDocument xmlFile, string tableName)
        {
            XAttribute mAttribute = xmlFile.Root.Descendants(strTableName).Select(e => e.Attribute("Name")).FirstOrDefault();

            if (mAttribute != null)
            {
                mAttribute.Value = tableName;

                SaveXML(xmlFile, GetFullPathAndFilename());
            }
        }

        private static void UpdateFieldNames(string strTableName, List<string> strFieldName, List<string> strFieldValue, XDocument xmlFile)
        {
            for (int i = 0; i < strFieldName.Count; i++)
            {
                string _strFieldName = strFieldName[i];
                string _strFieldValue = strFieldValue[i];

                XElement mElement = xmlFile.Root.Descendants(strTableName).Select(e => e.Element(_strFieldName)).FirstOrDefault();

                if (mElement != null)
                    mElement.Value = _strFieldValue;
            }


            SaveXML(xmlFile, GetFullPathAndFilename());
        }
        private static XDocument CreateXML()
        {
            string rootName = "Drillhole";
            string dateTime = DateTime.Now.ToFileTime().ToString();
            string xmlPath = Environment.GetEnvironmentVariable("home");

            if (xmlPath == null)
                xmlPath = Environment.GetEnvironmentVariable("temp");

            //if (xmlPath != "")
            xmlPath = xmlPath + "\\";
            //TODO Exception


            string fileName = dateTime + "_" + rootName + ".xml";
            string fullPathAndName = xmlPath + fileName;

            XDocument xmlFile = new XDocument(
                new XDeclaration("1.0", null, null),
                new XProcessingInstruction("order", "alpha ascending"),
                new XElement(rootName, new XAttribute("Modified", DateTime.Now)));

            xmlFile.Root.Add(InitiateDrillholeParameters());
            xmlFile.Root.Add(InitiatePreferences());
            xmlFile.Root.Add(InitiateCollarParameters());
            xmlFile.Root.Add(InitiateSurveyParameters());
            xmlFile.Root.Add(InitiateAssayParameters());
            xmlFile.Root.Add(InitiateIntervalParameters());

            SaveXML(xmlFile, fullPathAndName);

            return xmlFile;

        }

        private static XElement InitiatePreferences()
        {

            XElement nodeVals = new XElement("Preferences");
            nodeVals.Add(new XElement("Save", "false"));
            nodeVals.Add(new XElement("NoSamps", "true"));
            nodeVals.Add(new XElement("MissSurv", "true"));
            nodeVals.Add(new XElement("DipNeg", "true"));
            nodeVals.Add(new XElement("Vertical", "false"));
            nodeVals.Add(new XElement("CollSurv", "true"));
            nodeVals.Add(new XElement("Desurvey", "false"));
            nodeVals.Add(new XElement("CreateCol", "true"));
            nodeVals.Add(new XElement("CreateToe", "false"));
            nodeVals.Add(new XElement("Save", "false"));

            return nodeVals;

        }

        private static XElement InitiateDrillholeParameters()
        {
            XElement nodeVals = new XElement("Parameters");
            nodeVals.Add(new XElement("Database", ""));
            nodeVals.Add(new XElement("CollarTable", ""));
            nodeVals.Add(new XElement("SurveyTable", ""));
            nodeVals.Add(new XElement("AssayTable", ""));
            nodeVals.Add(new XElement("IntervalTable", ""));
            nodeVals.Add(new XElement("TableType", ""));

            return nodeVals;

        }

        private static XElement InitiateCollarParameters()
        {
            XElement nodeVals = new XElement("Collar", new XAttribute("Name", ""));
            nodeVals.Add(new XElement(DrillholeConstants.holeIDName, ""));
            nodeVals.Add(new XElement(DrillholeConstants.xName, ""));
            nodeVals.Add(new XElement(DrillholeConstants.yName, ""));
            nodeVals.Add(new XElement(DrillholeConstants.zName, ""));
            nodeVals.Add(new XElement(DrillholeConstants.maxName, ""));
            nodeVals.Add(new XElement(DrillholeConstants.dipName, ""));
            nodeVals.Add(new XElement(DrillholeConstants.azimuthName, ""));
            nodeVals.Add(new XElement("Constraint", ""));

            return nodeVals;
        }

        private static XElement InitiateSurveyParameters()
        {
            XElement nodeVals = new XElement("Survey", new XAttribute("Name", ""));
            nodeVals.Add(new XElement(DrillholeConstants.holeIDName, ""));
            nodeVals.Add(new XElement(DrillholeConstants.distName, ""));
            nodeVals.Add(new XElement(DrillholeConstants.dipName, ""));
            nodeVals.Add(new XElement(DrillholeConstants.azimuthName, ""));
            nodeVals.Add(new XElement("Constraint", ""));
            nodeVals.Add(new XElement("Foreign", ""));

            return nodeVals;
        }

        private static XElement InitiateAssayParameters()
        {
            XElement nodeVals = new XElement("Assay", new XAttribute("Name", ""));
            nodeVals.Add(new XElement(DrillholeConstants.holeIDName, ""));
            nodeVals.Add(new XElement(DrillholeConstants.distFromName, ""));
            nodeVals.Add(new XElement(DrillholeConstants.distToName, ""));
            nodeVals.Add(new XElement("Constraint", ""));
            nodeVals.Add(new XElement("Foreign", ""));

            return nodeVals;
        }

        private static XElement InitiateIntervalParameters()
        {
            XElement nodeVals = new XElement("Interval", new XAttribute("Name", ""));
            nodeVals.Add(new XElement(DrillholeConstants.holeIDName, ""));
            nodeVals.Add(new XElement(DrillholeConstants.distFromName, ""));
            nodeVals.Add(new XElement(DrillholeConstants.distToName, ""));
            nodeVals.Add(new XElement("Constraint", ""));
            nodeVals.Add(new XElement("Foreign", ""));

            return nodeVals;
        }
        private static void SaveXML(XDocument xmlFile, string fullPathAndName)
        {
            xmlFile.Save(fullPathAndName);
        }

        private static XDocument ReturnXML()
        {
            string fullPathAndName = GetFullPathAndFilename();

            if (fullPathAndName != "")
                return XDocument.Load(fullPathAndName);
            else

                return null;
        }

        private static string GetFullPathAndFilename()
        {
            string rootName = "Drillhole";
            string fullPathAndName = "";

            string xmlPath = Environment.GetEnvironmentVariable("home");

            if (xmlPath == null)
                xmlPath = Environment.GetEnvironmentVariable("temp");

            xmlPath = xmlPath = xmlPath + "\\";
            string searchName = "_" + rootName + ".xml";

            //Order by XML type abd date, with most recent at top
            var xmlFiles = new DirectoryInfo(xmlPath).GetFiles().Where(x => x.Extension == ".xml").OrderBy(t => t.CreationTime).Reverse();

            if (xmlFiles == null)
                return null;

            foreach (var file in xmlFiles)
            {
                string name = file.Name; //get the name of file

                int length = searchName.Length;  //length of seachName

                if (length < name.Length) //filename has to be longer than searchname
                {

                    if (name.Contains(searchName)) //if name contains searchName then load XML file
                    {
                        fullPathAndName = file.FullName;
                        break;
                    }
                }
            }

            return fullPathAndName;
        }


    }
}
