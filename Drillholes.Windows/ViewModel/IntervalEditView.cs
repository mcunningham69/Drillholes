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
    public class IntervalEditView
    {
        public IntervalEditServices _editService;
        public IIntervalEdit _editValues;

        public ImportTableFields importIntervalFields { get; set; }
        public XElement xmlIntervalData { get; set; }

        public IMapper mapper = null;

        public IntervalEditView(XElement _xmlIntervalData, ImportTableFields _intervalFields)
        {
            xmlIntervalData = _xmlIntervalData;
            importIntervalFields = _intervalFields;

            _editValues = new IntervalDataEdits();

            _editService = new IntervalEditServices(_editValues);
        }

        public virtual void InitialiseMapping()
        {
            if (_editValues == null)
                _editValues = new IntervalDataEdits();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<IntervalTableDto, IntervalTableObject>(); });

            mapper = config.CreateMapper();

            var source = new IntervalTableDto();

            var dest = mapper.Map<IntervalTableDto, IntervalTableObject>(source);

            _editService = new IntervalEditServices(_editValues);

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
