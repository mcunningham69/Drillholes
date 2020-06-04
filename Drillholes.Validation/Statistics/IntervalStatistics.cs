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
    public class IntervalStatistics : IIntervalStatistics
    {
        IntervalTableDto intervalTableDto = new IntervalTableDto();
        public async Task<IntervalTableDto> SummaryStatistics(List<ImportTableField> fields, XElement intervalValues)
        {
            var queryFields = fields.Where(o => o.genericType == false);

            string holeID = "";
            string fromID = "";
            string toID = "";

            foreach (var field in queryFields)
            {
                switch (field.columnImportName)
                {
                    case DrillholeConstants.holeIDName:
                        holeID = fields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(f => f.columnHeader).Single();
                        intervalTableDto.tableField = fields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(f => f.columnImportAs).Single() + " ==> " +
                            holeID + Environment.NewLine;
                        break;
                    case DrillholeConstants.distFromName:
                        fromID = fields.Where(o => o.columnImportName == DrillholeConstants.distFromName).Select(f => f.columnHeader).Single();
                        intervalTableDto.tableField = intervalTableDto.tableField + fields.Where(o => o.columnImportName == DrillholeConstants.distFromName).Select(f => f.columnImportAs).Single() + " ==> " +
                            fromID + Environment.NewLine;
                        break;
                    case DrillholeConstants.distToName:
                        toID = fields.Where(o => o.columnImportName == DrillholeConstants.distToName).Select(f => f.columnHeader).Single();
                        intervalTableDto.tableField = intervalTableDto.tableField + fields.Where(o => o.columnImportName == DrillholeConstants.distToName).Select(f => f.columnImportAs).Single() + " ==> " +
                            toID + Environment.NewLine;
                        break;
                }
            }

            var elements = intervalValues.Elements();

            intervalTableDto.SummaryStats = new SummaryIntervalStatistics();
            intervalTableDto.SummaryStats.IntervalCount = elements.Count();
            var holes = elements.GroupBy(x => x.Element(holeID).Value).Where(group => group.Count() > 0).Select(group => group.Key).ToList();

            List<int> IntervalCount = new List<int>();
            List<double> IntervalLength = new List<double>();

            //min and max counts per hole
            foreach (string hole in holes)
            {
                var froms = elements.Where(h => h.Element(holeID).Value == hole).Select(d => d.Element(fromID).Value).ToList();
                var tos = elements.Where(h => h.Element(holeID).Value == hole).Select(d => d.Element(toID).Value).ToList();

                IntervalCount.Add(froms.Count());

                for (int r = 0; r < froms.Count; r++)
                {
                    if (Information.IsNumeric(froms[r]))
                    {
                        if (Information.IsNumeric(tos[r]))
                        {
                            double dblFrom = Convert.ToDouble(froms[r]);
                            double dblTo = Convert.ToDouble(tos[r]);

                            double dblInterval = dblTo - dblFrom;

                            if (dblInterval > 0)
                            {
                                IntervalLength.Add(dblInterval);
                            }
                        }
                    }
                }
            }

            intervalTableDto.SummaryStats.collarCount = IntervalCount.Count();
            intervalTableDto.SummaryStats.MinIntervalCount = IntervalCount.Min();
            intervalTableDto.SummaryStats.MaxIntervalCount = IntervalCount.Max();
            intervalTableDto.SummaryStats.AverageIntervalCount = Math.Round(IntervalCount.Average(), 1);

            intervalTableDto.SummaryStats.IntervalCount = IntervalCount.Sum();

            intervalTableDto.SummaryStats.MinIntervalLength = Math.Round(IntervalLength.Min(), 1);
            intervalTableDto.SummaryStats.MaxIntervalLength = Math.Round(IntervalLength.Max(), 1);
            intervalTableDto.SummaryStats.AverageIntervalLength = Math.Round(IntervalLength.Average(), 1);

            intervalTableDto.isValid = true;

            return intervalTableDto;
        }
    }
}
