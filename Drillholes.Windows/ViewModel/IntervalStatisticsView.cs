using Drillholes.Domain;
using Drillholes.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain.Services;
using Drillholes.Domain.DTO;
using Drillholes.Validation.Statistics;
using Drillholes.Domain.DataObject;

namespace Drillholes.Windows.ViewModel
{
    public class IntervalStatisticsView : SurveyStatisticsView
    {
        IntervalStatisticsService _intervalStatisticsService;
        IIntervalStatistics _intervalStatistics;

        public IntervalStatisticsView(string _tableName, string _tableLocation, string _tableFormat,
            ImportTableFields _importFields, DrillholeSurveyType _survType, XElement _xPreview)
            : base(_tableName, _tableLocation, _tableFormat, _importFields, _survType, _xPreview)
        { }

        public async Task<IIntervalStatistics> InitialiseStatisticsMapping()
        {
            if (_intervalStatistics == null)
                _intervalStatistics = new IntervalStatistics();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<SummaryIntervalStatisticsDto, SummaryIntervalStatistics>(); });

            statisticsMapper = config.CreateMapper();

            var source = new SummaryIntervalStatisticsDto();

            var dest = statisticsMapper.Map<SummaryIntervalStatisticsDto, SummaryIntervalStatistics>(source);

            _intervalStatisticsService = new IntervalStatisticsService(_intervalStatistics);

            return _intervalStatistics;

        }

        public override async Task<bool> SummaryStatistics()
        {

            if (statisticsMapper == null)
                _intervalStatistics = await InitialiseStatisticsMapping();

            ImportTableField holeField = importFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField fromField = importFields.Where(o => o.columnImportName == DrillholeConstants.distFromName).Where(m => m.genericType == false).Single();
            ImportTableField toField = importFields.Where(o => o.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(holeField);
            tempFields.Add(fromField);
            tempFields.Add(toField);

            var summaryStatistics = await _intervalStatisticsService.SummaryStatistics(statisticsMapper, tempFields,
                xPreview);

            //summary of field mapping
            tableFields = summaryStatistics.tableFieldMapping;

            TableStatistics.DisplayStatistics.Add(summaryStatistics);

            return true;

        }
    }
}
