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
    }

    public class TextCsvFormat : ExportDesurveyResults
    {
        public override async Task<bool> ExportCollarTable(string outputName, string drillholeTableFile, string drillholeFields, string drillholeInputData, bool bAttributes)
        {
            XDocument xmlResults = XDocument.Load(drillholeTableFile);
            XDocument xmlFields = XDocument.Load(drillholeFields);
            XDocument xmlInput = XDocument.Load(drillholeInputData);

            IEnumerable<XElement> mandatoryFields = null;
            IEnumerable<XElement> optionalFields = null;

            var desurvElements = xmlResults.Descendants(DrillholeConstants.drillholeDesurv).Descendants("TableType").Elements();
            mandatoryFields = xmlFields.Descendants(DrillholeConstants.drillholeFields).Descendants("TableType").Elements().Where(g => g.Element("GroupName").Value == "Mandatory Fields");
            var inputElements = xmlInput.Descendants(DrillholeConstants.drillholeData).Descendants("TableType").Elements();


            if (bAttributes)
            {
                optionalFields = xmlFields.Descendants(DrillholeConstants.drillholeFields).Descendants("TableType").Elements().Where(g => g.Element("GroupName").Value == "Other Fields");
            }



            //  var DesurvElements = desurvElements.Where(f => f.Attribute("Value").Value == "collar").ToList();
            //  var FieldElements = fieldElements.Select(f => f.Element("TableType")).Nodes().ToList();
            //  var InputElements = inputElements.Select(f => f.Element("collar")).Nodes().ToList();


            //Mandatory fieldnames
            //   var collar = fieldElements.Select(x => x.Element("ColumnHeader")).ToList();





            return true;
        }
    }
}
