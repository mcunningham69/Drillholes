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
    public class AssayStatisticsView : SurveyStatisticsView
    {
        AssayStatisticsService _assayStatisticsService;
        IAssayStatistics _assayStatistics;

        public AssayStatisticsView(string _tableName, string _tableLocation, string _tableFormat,
            ImportTableFields _importFields, DrillholeSurveyType _survType, XElement _xPreview)
            : base(_tableName, _tableLocation, _tableFormat, _importFields, _survType, _xPreview)
        {

        }

        public async Task<IAssayStatistics> InitialiseStatisticsMapping()
        {
            if (_assayStatistics == null)
                _assayStatistics = new AssayStatistics();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<SummaryAssayStatisticsDto, SummaryAssayStatistics>(); });

            statisticsMapper = config.CreateMapper();

            var source = new SummaryAssayStatisticsDto();

            var dest = statisticsMapper.Map<SummaryAssayStatisticsDto, SummaryAssayStatistics>(source);

            _assayStatisticsService = new AssayStatisticsService(_assayStatistics);

            return _assayStatistics;

        }

        public override async Task<bool> SummaryStatistics()
        {

            if (statisticsMapper == null)
                _assayStatistics = await InitialiseStatisticsMapping();

            ImportTableField holeField = importFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField fromField = importFields.Where(o => o.columnImportName == DrillholeConstants.distFromName).Where(m => m.genericType == false).Single();
            ImportTableField toField = importFields.Where(o => o.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(holeField);
            tempFields.Add(fromField);
            tempFields.Add(toField);

            var summaryStatistics = await _assayStatisticsService.SummaryStatistics(statisticsMapper, tempFields,
                xPreview);

            //summary of field mapping
            tableFields = summaryStatistics.tableFieldMapping;

            TableStatistics.DisplayStatistics.Add(summaryStatistics);

            return true;

        }

    }
}
