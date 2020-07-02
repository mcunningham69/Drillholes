using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using Drillholes.Domain;
using Drillholes.Domain.DataObject;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain.Services;
using Drillholes.FixErrors;

namespace Drillholes.Windows.ViewModel
{
    public class SurveyEditView
    {
        public SurveyEditServices _editService;
        public ISurveyEdit _editValues;

        public ImportTableFields importSurveyFields { get; set; }
        public XElement xmlSurveyData { get; set; }

        public IMapper mapper = null;

        public SurveyEditView(XElement _xmlSurveyData, ImportTableFields _surveyFields)
        {
            xmlSurveyData = _xmlSurveyData;
            importSurveyFields = _surveyFields;

            _editValues = new SurveyDataEdits();

            _editService = new SurveyEditServices(_editValues);
        }

        public virtual void InitialiseMapping()
        {
            if (_editValues == null)
                _editValues = new SurveyDataEdits();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<SurveyTableDto, SurveyTableObject>(); });

            mapper = config.CreateMapper();

            var source = new SurveyTableDto();

            var dest = mapper.Map<SurveyTableDto, SurveyTableObject>(source);

            _editService = new SurveyEditServices(_editValues);

        }

        public async Task<bool> SaveEdits(List<RowsToEdit> rows, bool bIgnore)
        {
            if (mapper == null)
                InitialiseMapping();

            List<ImportTableField> editFields = new List<ImportTableField>();

            return true;
        }
    }
}
