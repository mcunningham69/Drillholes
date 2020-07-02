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
    public class AssayEditView
    {
        public AssayEditServices _editService;
        public IAssayEdit _editValues;

        public ImportTableFields importAssayFields { get; set; }
        public XElement xmlAssayData { get; set; }

        public IMapper mapper = null;

        public AssayEditView(XElement _xmlAssayData, ImportTableFields _assayFields)
        {
            xmlAssayData = _xmlAssayData;
            importAssayFields = _assayFields;

            _editValues = new AssayDataEdits();

            _editService = new AssayEditServices(_editValues);
        }

        public virtual void InitialiseMapping()
        {
            if (_editValues == null)
                _editValues = new AssayDataEdits();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<AssayTableDto, AssayTableObject>(); });

            mapper = config.CreateMapper();

            var source = new AssayTableDto();

            var dest = mapper.Map<AssayTableDto, AssayTableObject>(source);

            _editService = new AssayEditServices(_editValues);

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
