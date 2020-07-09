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
    public class ContinuousStatistics : IContinuousStatistics
    {
        SummaryContinuousStatisticsDto continuousTableDto = new SummaryContinuousStatisticsDto();
        public async Task<SummaryContinuousStatisticsDto> SummaryStatistics(List<ImportTableField> fields, XElement continuousValues)
        {
            var queryFields = fields.Where(o => o.genericType == false);

            string holeID = "";
            string distID = "";
            string toID = "";

            foreach (var field in queryFields)
            {
                switch (field.columnImportName)
                {
                    case DrillholeConstants.holeIDName:
                        holeID = fields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(f => f.columnHeader).Single();
                        continuousTableDto.tableFieldMapping = fields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(f => f.columnImportAs).Single() + " ==> " +
                            holeID + Environment.NewLine;
                        break;
                    case DrillholeConstants.distName:
                        distID = fields.Where(o => o.columnImportName == DrillholeConstants.distName).Select(f => f.columnHeader).Single();
                        continuousTableDto.tableFieldMapping = continuousTableDto.tableFieldMapping + fields.Where(o => o.columnImportName == DrillholeConstants.distName).Select(f => f.columnImportAs).Single() + " ==> " +
                            distID + Environment.NewLine;
                        break;

                }
            }

            var elements = continuousValues.Elements();

            var holes = elements.GroupBy(x => x.Element(holeID).Value).Where(group => group.Count() > 0).Select(group => group.Key).ToList();

            List<int> ContinCount = new List<int>();
            List<double> ValueLength = new List<double>();

            double previousValue = 0.0;
            double currentValue = 0.0;
            double value = 0.0;


            //min and max counts per hole
            foreach (string hole in holes)
            {
                var distances = elements.Where(h => h.Element(holeID).Value == hole).Select(d => d.Element(distID).Value).ToList();
               // var tos = elements.Where(h => h.Element(holeID).Value == hole).Select(d => d.Element(toID).Value).ToList();

                ContinCount.Add(distances.Count());

                for (int d = 0; d < distances.Count; d++)
                {
                    if (d == 0)
                    {
                        if (Information.IsNumeric(distances[d]))
                        {
                            currentValue = Convert.ToDouble(distances[d]);

                            if (currentValue > 0)
                            {
                                ValueLength.Add(currentValue);
                            }
                        }
                    }
                    else
                    {
                        if (Information.IsNumeric(distances[d - 1]))
                        {
                            if (Information.IsNumeric(distances[d]))
                            {
                                previousValue = Convert.ToDouble(distances[d - 1]);
                                currentValue = Convert.ToDouble(distances[d]);
                                value = currentValue - previousValue;

                                ValueLength.Add(value);
                            }
                        }
                    }
                }
            }

            continuousTableDto.collarCount = holes.Count();
            continuousTableDto.MinValueCount = ContinCount.Min();
            continuousTableDto.MaxValueCount = ContinCount.Max();
            continuousTableDto.AverageValueCount = Math.Round(ContinCount.Average(), 1);

            continuousTableDto.ValueCount = ContinCount.Sum();

            continuousTableDto.MinValueLength = Math.Round(ValueLength.Min(), 1);
            continuousTableDto.MaxValueLength = Math.Round(ValueLength.Max(), 1);
            continuousTableDto.AverageValueLength = Math.Round(ValueLength.Average(), 1);

            continuousTableDto.isValid = true;

            return continuousTableDto;
        }

    }
}
