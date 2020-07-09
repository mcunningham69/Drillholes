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
    public class AssayStatistics : IAssayStatistics
    {
        SummaryAssayStatisticsDto assayTableDto = new SummaryAssayStatisticsDto();
        public async Task<SummaryAssayStatisticsDto> SummaryStatistics(List<ImportTableField> fields, XElement assayValues)
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
                        assayTableDto.tableFieldMapping = fields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(f => f.columnImportAs).Single() + " ==> " +
                            holeID + Environment.NewLine;
                        break;
                    case DrillholeConstants.distFromName:
                        fromID = fields.Where(o => o.columnImportName == DrillholeConstants.distFromName).Select(f => f.columnHeader).Single();
                        assayTableDto.tableFieldMapping = assayTableDto.tableFieldMapping + fields.Where(o => o.columnImportName == DrillholeConstants.distFromName).Select(f => f.columnImportAs).Single() + " ==> " +
                            fromID + Environment.NewLine;
                        break;
                    case DrillholeConstants.distToName:
                        toID = fields.Where(o => o.columnImportName == DrillholeConstants.distToName).Select(f => f.columnHeader).Single();
                        assayTableDto.tableFieldMapping = assayTableDto.tableFieldMapping + fields.Where(o => o.columnImportName == DrillholeConstants.distToName).Select(f => f.columnImportAs).Single() + " ==> " +
                            toID + Environment.NewLine;
                        break;
                }
            }

            var elements = assayValues.Elements();

            var holes = elements.GroupBy(x => x.Element(holeID).Value).Where(group => group.Count() > 0).Select(group => group.Key).ToList();

            List<int> AssayCount = new List<int>();
            List<double> AssayLength = new List<double>();

            //min and max counts per hole
            foreach (string hole in holes)
            {
                var froms = elements.Where(h => h.Element(holeID).Value == hole).Select(d => d.Element(fromID).Value).ToList();
                var tos = elements.Where(h => h.Element(holeID).Value == hole).Select(d => d.Element(toID).Value).ToList();

                AssayCount.Add(froms.Count());

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
                                AssayLength.Add(dblInterval);
                            }
                        }
                    }
                }
            }

            assayTableDto.collarCount = holes.Count();
            assayTableDto.MinAssayCount = AssayCount.Min();
            assayTableDto.MaxAssayCount = AssayCount.Max();
            assayTableDto.AverageAssayCount = Math.Round(AssayCount.Average(), 1);

            assayTableDto.AssayCount = AssayCount.Sum();

            assayTableDto.MinAssayLength = Math.Round(AssayLength.Min(), 1);
            assayTableDto.MaxAssayLength = Math.Round(AssayLength.Max(), 1);
            assayTableDto.AverageAssayLength = Math.Round(AssayLength.Average(), 1);

            assayTableDto.isValid = true;

            return assayTableDto;
        }
    }
}
