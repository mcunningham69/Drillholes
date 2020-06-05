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
    public class SurveyStatistics : ISurveyStatistics
    {
        private SummarySurveyStatisticsDto surveyTableDto = new SummarySurveyStatisticsDto();

        public async Task<SummarySurveyStatisticsDto> SummaryStatistics(List<ImportTableField> fields, XElement surveyValues)
        {
            var queryFields = fields.Where(o => o.genericType == false);

            string holeID = "";
            string distID = "";
            string dipID = "";
            string aziID = "";

            foreach (var field in queryFields)
            {
                switch (field.columnImportName)
                {
                    case DrillholeConstants.holeIDName:
                        holeID = fields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(f => f.columnHeader).Single();
                        surveyTableDto.tableFieldMapping = fields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Select(f => f.columnImportAs).Single() + " ==> " +
                            holeID + Environment.NewLine;
                        break;
                    case DrillholeConstants.distName:
                        distID = fields.Where(o => o.columnImportName == DrillholeConstants.distName).Select(f => f.columnHeader).Single();
                        surveyTableDto.tableFieldMapping = surveyTableDto.tableFieldMapping + fields.Where(o => o.columnImportName == DrillholeConstants.distName).Select(f => f.columnImportAs).Single() + " ==> " +
                            distID + Environment.NewLine;
                        break;
                    case DrillholeConstants.dipName:
                        dipID = fields.Where(o => o.columnImportName == DrillholeConstants.dipName).Select(f => f.columnHeader).Single();
                        surveyTableDto.tableFieldMapping = surveyTableDto.tableFieldMapping + fields.Where(o => o.columnImportName == DrillholeConstants.dipName).Select(f => f.columnImportAs).Single() + " ==> " +
                            dipID + Environment.NewLine;
                        break;
                    case DrillholeConstants.azimuthName:
                        aziID = fields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Select(f => f.columnHeader).Single();
                        surveyTableDto.tableFieldMapping = surveyTableDto.tableFieldMapping + fields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Select(f => f.columnImportAs).Single() + " ==> " +
                            aziID + Environment.NewLine;
                        break;

                }
            }

            var elements = surveyValues.Elements();

            surveyTableDto.surveyCount = elements.Count();

            var holes = elements.GroupBy(x => x.Element(holeID).Value).Where(group => group.Count() > 0).Select(group => group.Key).ToList();

            surveyTableDto.collarCount = holes.Count();

            List<int> SurvCount = new List<int>();
            List<double> _survLength = new List<double>();
            List<double> _dip = new List<double>();
            List<double> _direction = new List<double>();

            //min and max counts per hole
            foreach (string hole in holes)
            {
                var distances = elements.Where(h => h.Element(holeID).Value == hole).Select(d => d.Element(distID).Value).ToList();
                var dips = elements.Where(h => h.Element(holeID).Value == hole).Select(d => d.Element(dipID).Value).ToList();
                var directions = elements.Where(h => h.Element(holeID).Value == hole).Select(d => d.Element(aziID).Value).ToList();

                SurvCount.Add(distances.Count());

                int distCount = 0;
                double previousValue = 0.0;
                double currentValue = 0.0;
                double value = 0.0;

                #region Distance
                for (int d = 0; d < distances.Count; d++)
                {
                    if (d == 0)
                    {
                        if (Information.IsNumeric(distances[d]))
                        {
                            currentValue = Convert.ToDouble(distances[d]);

                            if (currentValue > 0)
                            {
                                _survLength.Add(currentValue);
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

                                _survLength.Add(value);
                            }
                        }
                    }
                }
                #endregion

                #region Dip
                foreach (var dip in dips)
                {
                    if (Information.IsNumeric(dip))
                    {
                        double dblDip = Convert.ToDouble(dip);

                        if (dblDip < 0)
                            dblDip = dblDip * -1;

                        _dip.Add(Convert.ToDouble(dblDip));
                    }
                }
                #endregion

                #region Direction
                foreach (var direction in directions)
                {
                    if (Information.IsNumeric(direction))
                    {
                        _direction.Add(Convert.ToDouble(direction));
                    }
                }
                #endregion

            }

            surveyTableDto.MinSurveyLength = _survLength.Min();
            surveyTableDto.MaxSurveyLength = _survLength.Max();
            surveyTableDto.AverageSurveyLength = _survLength.Average();

            surveyTableDto.MinSurveyCount = SurvCount.Min();
            surveyTableDto.MaxSurveyCount = SurvCount.Max();
            surveyTableDto.AverageSurveyCount = SurvCount.Average();

            surveyTableDto.MinimumDip = _dip.Min();
            surveyTableDto.MaximumDip = _dip.Max();
            surveyTableDto.AverageDip = _dip.Average();

            surveyTableDto.MinDipDir = _direction.Min();
            surveyTableDto.MaxDipDir = _direction.Max();
            surveyTableDto.AverageDipDir = _direction.Average();

            surveyTableDto.isValid = true;

            return surveyTableDto;
        }

       
    }
}
