using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain;
using Drillholes.Domain.Enum;
using Drillholes.Domain.DataObject;
using Drillholes.Domain.Services;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain.DTO;
using Drillholes.Validation.Statistics;
using AutoMapper;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace Drillholes.Windows.ViewModel
{
    public class SurveyStatisticsView : CollarStatisticsView
    {
        SurveyStatisticsService _surveyStatisticsService;
        ISurveyStatistics _surveyStatistics;


        public SurveyStatisticsView(string _tableName, string _tableLocation, string _tableFormat,
            ImportTableFields _importFields, DrillholeSurveyType _survType, XElement _xPreview)
            : base(_tableName, _tableLocation, _tableFormat, _importFields, _survType, _xPreview)
        { }

        public async Task<ISurveyStatistics> InitialiseStatisticsMapping()
        {
            if (_surveyStatistics == null)
                _surveyStatistics = new SurveyStatistics();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<SummarySurveyStatisticsDto, SummarySurveyStatistics>(); });

            statisticsMapper = config.CreateMapper();

            var source = new SummarySurveyStatisticsDto();

            var dest = statisticsMapper.Map<SummarySurveyStatisticsDto, SummarySurveyStatistics>(source);

            _surveyStatisticsService = new SurveyStatisticsService(_surveyStatistics);

            return _surveyStatistics;

        }

        public virtual async Task<bool> SummaryStatistics()
        {

            if (statisticsMapper == null)
                _surveyStatistics = await InitialiseStatisticsMapping();

            ImportTableField holeField = importFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField distField = importFields.Where(o => o.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Single();
            ImportTableField dipField = importFields.Where(o => o.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false).Single();
            ImportTableField aziField = importFields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(holeField);
            tempFields.Add(distField);
            tempFields.Add(dipField);
            tempFields.Add(aziField);

            var summaryStatistics = await _surveyStatisticsService.SummaryStatistics(statisticsMapper, tempFields, xPreview);

            tableFields = summaryStatistics.tableFieldMapping;
            TableStatistics.DisplayStatistics.Add(summaryStatistics);

            return true;
        }

    }
}
