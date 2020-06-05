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
        // private CollarTableDto collarTableDto;
        private SummaryCollarStatisticsDto summaryStats;

        public async Task<SummaryCollarStatisticsDto> SummaryStatistics(List<ImportTableField> fields, XElement collarValues, DrillholeSurveyType surveyType)
        {
            summaryStats = new SummaryCollarStatisticsDto();

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
                        summaryStats.tableFieldMapping = fields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(f => f.columnImportAs).Single() + " ==> " +
                            holeID + Environment.NewLine;
                        break;
                    case DrillholeConstants.xName:
                        xID = fields.Where(o => o.columnImportName == DrillholeConstants.xName).Select(f => f.columnHeader).Single();
                        summaryStats.tableFieldMapping = summaryStats.tableFieldMapping + fields.Where(o => o.columnImportName == DrillholeConstants.xName).Select(f => f.columnImportAs).Single() + " ==> " +
                            xID + Environment.NewLine;
                        break;
                    case DrillholeConstants.yName:
                        yID = fields.Where(o => o.columnImportName == DrillholeConstants.yName).Select(f => f.columnHeader).Single();
                        summaryStats.tableFieldMapping = summaryStats.tableFieldMapping + fields.Where(o => o.columnImportName == DrillholeConstants.yName).Select(f => f.columnImportAs).Single() + " ==> " +
                            yID + Environment.NewLine;
                        break;
                    case DrillholeConstants.zName:
                        zID = fields.Where(o => o.columnImportName == DrillholeConstants.zName).Select(f => f.columnHeader).Single();
                        summaryStats.tableFieldMapping = summaryStats.tableFieldMapping + fields.Where(o => o.columnImportName == DrillholeConstants.zName).Select(f => f.columnImportAs).Single() + " ==> " +
                            zID + Environment.NewLine;
                        break;
                    case DrillholeConstants.maxName:
                        maxID = fields.Where(o => o.columnImportName == DrillholeConstants.maxName).Select(f => f.columnHeader).Single();
                        summaryStats.tableFieldMapping = summaryStats.tableFieldMapping + fields.Where(o => o.columnImportName == DrillholeConstants.maxName).Select(f => f.columnImportAs).Single() + " ==> " +
                           maxID + Environment.NewLine;
                        break;
                    case DrillholeConstants.dipName:
                        dip = fields.Where(o => o.columnImportName == DrillholeConstants.dipName).Select(f => f.columnHeader).Single();
                        summaryStats.tableFieldMapping = summaryStats.tableFieldMapping + fields.Where(o => o.columnImportName == DrillholeConstants.dipName).Select(f => f.columnImportAs).Single() + " ==> " +
                           dip + Environment.NewLine;
                        break;
                    case DrillholeConstants.azimuthName:
                        azimuth = fields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Select(f => f.columnHeader).Single();
                        summaryStats.tableFieldMapping = summaryStats.tableFieldMapping + fields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Select(f => f.columnImportAs).Single() + " ==> " +
                           azimuth + Environment.NewLine;
                        break;
                }
            }

            var elements = collarValues.Elements();

          // SummaryStats = new SummaryCollarStatistics();
            summaryStats.collarCount = elements.Count();

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
                summaryStats.MinimumLength = Math.Round(l.Min(), 1);
                summaryStats.MaximumLength = Math.Round(l.Max(), 1);
                summaryStats.AverageLength = Math.Round(l.Average(), 1);
                summaryStats.TotalLength = Math.Round(l.Sum(), 1);
            }
            if (X.Count > 0 && Y.Count > 0 && Z.Count > 0)
            {
                summaryStats.MinimumX = Math.Round(X.Min(), 1);
                summaryStats.MinimumY = Math.Round(Y.Min(), 1);
                summaryStats.MinimumZ = Math.Round(Z.Min(), 1);

                summaryStats.MaximumX = Math.Round(X.Max(), 1);
                summaryStats.MaximumY = Math.Round(Y.Max(), 1);
                summaryStats.MaximumZ = Math.Round(Z.Max(), 1);

                summaryStats.ExtentX = Math.Round(X.Max() - X.Min(), 1);
                summaryStats.ExtentY = Math.Round(Y.Max() - Y.Min(), 1);
                summaryStats.ExtentZ = Math.Round(Z.Max() - Z.Min(), 1);

               // summaryStats.CalculateArea();
            }

            if (d.Count > 0 && a.Count > 0)
            {
                summaryStats.MinimumDip = Math.Round(d.Min(), 1).ToString();
                summaryStats.AverageDip = Math.Round(d.Average(), 1).ToString();
                summaryStats.MaximumDip = Math.Round(d.Max(), 1).ToString();
                summaryStats.MinimumAzi = Math.Round(a.Min(), 1).ToString();
                summaryStats.AverageAzi = Math.Round(a.Average(), 1).ToString();
                summaryStats.MaximumAzi = Math.Round(a.Max(), 1).ToString();
            }
            else
            {
                summaryStats.MinimumDip = "n/a";
                summaryStats.AverageDip = "n/a";
                summaryStats.MaximumDip = "n/a";
                summaryStats.MinimumAzi = "n/a";
                summaryStats.AverageAzi = "n/a";
                summaryStats.MaximumAzi = "n/a";
            }

            summaryStats.isValid = true;

            return summaryStats;
        }
    }
}
