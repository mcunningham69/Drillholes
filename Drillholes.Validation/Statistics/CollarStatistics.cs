using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using Drillholes.Domain.Interfaces;
using Microsoft.VisualBasic;


namespace Drillholes.Validation.Statistics
{
    public class CollarStatistics : ICollarStatistics
    {
        private CollarTableDto collarTableDto;
        public async Task<CollarTableDto> SummaryStatistics(List<ImportTableField> fields, XElement collarValues, DrillholeSurveyType surveyType)
        {
            var queryFields = fields.Where(o => o.genericType == false);

            string holeID = "";
            string xID = "";
            string yID = "";
            string zID = "";
            string maxID = "";
            string dip = "";
            string azimuth = "";

            foreach (var field in queryFields)
            {
                switch (field.columnImportName)
                {
                    case DrillholeConstants.holeIDName:
                        holeID = fields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(f => f.columnHeader).Single();
                        collarTableDto.tableField = fields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(f => f.columnImportAs).Single() + " ==> " +
                            holeID + Environment.NewLine;
                        break;
                    case DrillholeConstants.xName:
                        xID = fields.Where(o => o.columnImportName == DrillholeConstants.xName).Select(f => f.columnHeader).Single();
                        collarTableDto.tableField = collarTableDto.tableField + fields.Where(o => o.columnImportName == DrillholeConstants.xName).Select(f => f.columnImportAs).Single() + " ==> " +
                            xID + Environment.NewLine;
                        break;
                    case DrillholeConstants.yName:
                        yID = fields.Where(o => o.columnImportName == DrillholeConstants.yName).Select(f => f.columnHeader).Single();
                        collarTableDto.tableField = collarTableDto.tableField + fields.Where(o => o.columnImportName == DrillholeConstants.yName).Select(f => f.columnImportAs).Single() + " ==> " +
                            yID + Environment.NewLine;
                        break;
                    case DrillholeConstants.zName:
                        zID = fields.Where(o => o.columnImportName == DrillholeConstants.zName).Select(f => f.columnHeader).Single();
                        collarTableDto.tableField = collarTableDto.tableField + fields.Where(o => o.columnImportName == DrillholeConstants.zName).Select(f => f.columnImportAs).Single() + " ==> " +
                            zID + Environment.NewLine;
                        break;
                    case DrillholeConstants.maxName:
                        maxID = fields.Where(o => o.columnImportName == DrillholeConstants.maxName).Select(f => f.columnHeader).Single();
                        collarTableDto.tableField = collarTableDto.tableField + fields.Where(o => o.columnImportName == DrillholeConstants.maxName).Select(f => f.columnImportAs).Single() + " ==> " +
                           maxID + Environment.NewLine;
                        break;
                    case DrillholeConstants.dipName:
                        dip = fields.Where(o => o.columnImportName == DrillholeConstants.dipName).Select(f => f.columnHeader).Single();
                        collarTableDto.tableField = collarTableDto.tableField + fields.Where(o => o.columnImportName == DrillholeConstants.dipName).Select(f => f.columnImportAs).Single() + " ==> " +
                           dip + Environment.NewLine;
                        break;
                    case DrillholeConstants.azimuthName:
                        azimuth = fields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Select(f => f.columnHeader).Single();
                        collarTableDto.tableField = collarTableDto.tableField + fields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Select(f => f.columnImportAs).Single() + " ==> " +
                           azimuth + Environment.NewLine;
                        break;
                }
            }

            var elements = collarValues.Elements();

            collarTableDto.SummaryStats = new SummaryCollarStatistics();
            collarTableDto.SummaryStats.collarCount = elements.Count();

            var eastings = elements.Select(z => z.Element(xID).Value).ToList();
            var northings = elements.Select(z => z.Element(yID).Value).ToList();
            var elevations = elements.Select(z => z.Element(zID).Value).ToList();
            var lengths = elements.Select(z => z.Element(maxID).Value).ToList();

            List<string> dips = null;
            List<string> azimuths = null;

            if (surveyType == DrillholeSurveyType.collarsurvey)
            {
                if (dip != "" && azimuth != "")
                {
                    dips = elements.Select(z => z.Element(dip).Value).ToList();
                    azimuths = elements.Select(z => z.Element(azimuth).Value).ToList();
                }
            }

            List<double> X = new List<double>();
            List<double> Y = new List<double>();
            List<double> Z = new List<double>();
            List<double> l = new List<double>();
            List<double> d = new List<double>();
            List<double> a = new List<double>();

            for (int i = 0; i < eastings.Count; i++)
            {
                if (Information.IsNumeric(eastings[i]))
                    X.Add(Convert.ToDouble(eastings[i]));
                if (Information.IsNumeric(northings[i]))
                    Y.Add(Convert.ToDouble(northings[i]));
                if (Information.IsNumeric(elevations[i]))
                    Z.Add(Convert.ToDouble(elevations[i]));
                if (Information.IsNumeric(lengths[i]))
                    l.Add(Convert.ToDouble(lengths[i]));

                if (surveyType == DrillholeSurveyType.collarsurvey)
                {
                    if (dips != null && azimuths != null)
                    {
                        if (Information.IsNumeric(dips[i]))
                            d.Add(Convert.ToDouble(dips[i]));
                        if (Information.IsNumeric(azimuths[i]))
                            a.Add(Convert.ToDouble(azimuths[i]));
                    }
                }

            }

            if (l.Count > 0)
            {
                collarTableDto.SummaryStats.MinimumLength = Math.Round(l.Min(), 1);
                collarTableDto.SummaryStats.MaximumLength = Math.Round(l.Max(), 1);
                collarTableDto.SummaryStats.AverageLength = Math.Round(l.Average(), 1);
                collarTableDto.SummaryStats.TotalLength = Math.Round(l.Sum(), 1);
            }
            if (X.Count > 0 && Y.Count > 0 && Z.Count > 0)
            {
                collarTableDto.SummaryStats.MinimumX = Math.Round(X.Min(), 1);
                collarTableDto.SummaryStats.MinimumY = Math.Round(Y.Min(), 1);
                collarTableDto.SummaryStats.MinimumZ = Math.Round(Z.Min(), 1);

                collarTableDto.SummaryStats.MaximumX = Math.Round(X.Max(), 1);
                collarTableDto.SummaryStats.MaximumY = Math.Round(Y.Max(), 1);
                collarTableDto.SummaryStats.MaximumZ = Math.Round(Z.Max(), 1);

                collarTableDto.SummaryStats.ExtentX = Math.Round(X.Max() - X.Min(), 1);
                collarTableDto.SummaryStats.ExtentY = Math.Round(Y.Max() - Y.Min(), 1);
                collarTableDto.SummaryStats.ExtentZ = Math.Round(Z.Max() - Z.Min(), 1);

                collarTableDto.SummaryStats.CalculateArea();
            }

            if (d.Count > 0 && a.Count > 0)
            {
                collarTableDto.SummaryStats.MinimumDip = Math.Round(d.Min(), 1).ToString();
                collarTableDto.SummaryStats.AverageDip = Math.Round(d.Average(), 1).ToString();
                collarTableDto.SummaryStats.MaximumDip = Math.Round(d.Max(), 1).ToString();
                collarTableDto.SummaryStats.MinimumAzi = Math.Round(a.Min(), 1).ToString();
                collarTableDto.SummaryStats.AverageAzi = Math.Round(a.Average(), 1).ToString();
                collarTableDto.SummaryStats.MaximumAzi = Math.Round(a.Max(), 1).ToString();
            }
            else
            {
                collarTableDto.SummaryStats.MinimumDip = "n/a";
                collarTableDto.SummaryStats.AverageDip = "n/a";
                collarTableDto.SummaryStats.MaximumDip = "n/a";
                collarTableDto.SummaryStats.MinimumAzi = "n/a";
                collarTableDto.SummaryStats.AverageAzi = "n/a";
                collarTableDto.SummaryStats.MaximumAzi = "n/a";
            }

            return collarTableDto;
        }
    }
}
