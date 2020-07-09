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
using Drillholes.Domain.DataObject;
using Drillholes.Validation.Statistics;


namespace Drillholes.Windows.ViewModel
{
    public class ContinuousStatisticsView : IntervalStatisticsView
    {
        ContinuousStatisticsService _continuousStatisticsService;
        IContinuousStatistics _continuousStatistics;

        public ContinuousStatisticsView(string _tableName, string _tableLocation, string _tableFormat,
            ImportTableFields _importFields, DrillholeSurveyType _survType, XElement _xPreview)
            : base(_tableName, _tableLocation, _tableFormat, _importFields, _survType, _xPreview)
        {

        }

        public async Task<IContinuousStatistics> InitialiseStatisticsMapping()
        {
            if (_continuousStatistics == null)
                _continuousStatistics = new ContinuousStatistics();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<SummaryContinuousStatisticsDto, SummaryContinuousStatistics>(); });

            statisticsMapper = config.CreateMapper();

            var source = new SummaryContinuousStatisticsDto();

            var dest = statisticsMapper.Map<SummaryContinuousStatisticsDto, SummaryContinuousStatistics>(source);

            _continuousStatisticsService = new ContinuousStatisticsService(_continuousStatistics);

            return _continuousStatistics;

        }

        public override async Task<bool> SummaryStatistics()
        {

            if (statisticsMapper == null)
                _continuousStatistics = await InitialiseStatisticsMapping();

            ImportTableField holeField = importFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField distField = importFields.Where(o => o.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Single();
          //  ImportTableField toField = importFields.Where(o => o.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(holeField);
            tempFields.Add(distField);
          //  tempFields.Add(toField);

            var summaryStatistics = await _continuousStatisticsService.SummaryStatistics(statisticsMapper, tempFields,
                xPreview);

            //summary of field mapping
            tableFields = summaryStatistics.tableFieldMapping;

            TableStatistics.DisplayStatistics.Add(summaryStatistics);

            return true;

        }

    }
}
