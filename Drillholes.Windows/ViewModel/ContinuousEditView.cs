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
    public class ContinuousEditView
    {
        public ContinuousEditServices _editService;
        public IContinuousEdit _editValues;

        public ImportTableFields importContinuousFields { get; set; }
        public XElement xmlContinuousData { get; set; }

        public IMapper mapper = null;

        public ContinuousEditView(XElement _xmlContinuousData, ImportTableFields _continuousFields)
        {
            xmlContinuousData = _xmlContinuousData;
            importContinuousFields = _continuousFields;

            _editValues = new ContinuousDataEdits();

            _editService = new ContinuousEditServices(_editValues);
        }

        public virtual void InitialiseMapping()
        {
            if (_editValues == null)
                _editValues = new ContinuousDataEdits();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<ContinuousTableDto, ContinuousTableObject>(); });

            mapper = config.CreateMapper();

            var source = new ContinuousTableDto();

            var dest = mapper.Map<ContinuousTableDto, ContinuousTableObject>(source);

            _editService = new ContinuousEditServices(_editValues);

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
