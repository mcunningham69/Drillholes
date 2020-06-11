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
    public class CollarStatisticsView
    {
        CollarStatisticsService _collarStatisticsService;
        ICollarStatistics _collarStatistics;

        public IMapper statisticsMapper = null;

        public DrillholeSurveyType surveyType { get; set; }

        public ImportTableFields importFields { get; set; }

        public XElement xPreview { get; set; }
        public string tableName { get; set; }
        public string tableLocation { get; set; }
        public string tableFormat { get; set; }

        public string tableFields { get; set; }

        public DisplaySummaryStatistics TableStatistics = new DisplaySummaryStatistics();
        public ObservableCollection<SummaryStatistics> ShowStatistics { get { return TableStatistics.DisplayStatistics; } }

        public CollarStatisticsView(string _tableName, string _tableLocation, string _tableFormat,
            ImportTableFields _importFields, DrillholeSurveyType _survType, XElement _xPreview)
        {
            tableName = _tableName;
            tableLocation = _tableLocation;
            tableFormat = _tableFormat;
            tableFields = "";
            importFields = _importFields;
            surveyType = _survType;
            xPreview = _xPreview;
        }

        public async Task<ICollarStatistics> InitialiseStatisticsMapping()
        {
            if (_collarStatistics == null)
                _collarStatistics = new CollarStatistics();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<SummaryCollarStatisticsDto, SummaryCollarStatistics>(); });

            statisticsMapper = config.CreateMapper();

            var source = new SummaryCollarStatisticsDto();

            var dest = statisticsMapper.Map<SummaryCollarStatisticsDto, SummaryCollarStatistics>(source);

            _collarStatisticsService = new CollarStatisticsService(_collarStatistics);

            return _collarStatistics;

        }

        public virtual async Task<bool> SummaryStatistics()
        {

            if (statisticsMapper == null)
                _collarStatistics = await InitialiseStatisticsMapping();

            ImportTableField holeField = importFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField xField = importFields.Where(o => o.columnImportName == DrillholeConstants.xName).Where(m => m.genericType == false).Single();
            ImportTableField yField = importFields.Where(o => o.columnImportName == DrillholeConstants.yName).Where(m => m.genericType == false).Single();
            ImportTableField zField = importFields.Where(o => o.columnImportName == DrillholeConstants.zName).Where(m => m.genericType == false).Single();
            ImportTableField maxField = importFields.Where(o => o.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Single();

            List<ImportTableField> tempFields = new List<ImportTableField>();
            tempFields.Add(holeField);
            tempFields.Add(xField);
            tempFields.Add(yField);
            tempFields.Add(zField);
            tempFields.Add(maxField);

            if (surveyType == DrillholeSurveyType.collarsurvey)
            {
                ImportTableField dipField = importFields.Where(o => o.columnImportName == DrillholeConstants.dipName).FirstOrDefault();
                ImportTableField aziField = importFields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).FirstOrDefault();

                tempFields.Add(dipField);
                tempFields.Add(aziField);
            }

            var summaryStatistics = await _collarStatisticsService.SummaryStatistics(statisticsMapper, tempFields,
                xPreview, surveyType);

            summaryStatistics.CalculateArea();

            //summary of field mapping
            tableFields = summaryStatistics.tableFieldMapping;

            TableStatistics.DisplayStatistics.Add(summaryStatistics);

            return true;

        }
    }
}
