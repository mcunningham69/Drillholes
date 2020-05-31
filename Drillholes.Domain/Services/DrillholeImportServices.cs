using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using Drillholes.Domain.Exceptions;
using Drillholes.Domain.DataObject;
using AutoMapper;

namespace Drillholes.Domain.Services
{
    public class DrillholeImportServices
    {
        private readonly IDrillholeTables drillholeTables;

        public DrillholeImportServices(IDrillholeTables _drillholeTables)
        {
            this.drillholeTables = _drillholeTables;
        }
        

        public DrillholeTable SelectTables(DrillholeTableType value, IMapper mapper)
        {
            var drillholeImportDto = drillholeTables.SelectTables(value);

            if (drillholeImportDto.isValid == false)
            {

                throw new ImportClassException("Problem with importing table " + value.ToString());
            }

            var drillholeImport = mapper.Map<DrillholeImportDto, DrillholeTable>(drillholeImportDto);

            return drillholeImport;
        }

        public DrillholeTable SelectIntervalTables(DrillholeTableType value, IMapper mapper)
        {
            var drillholeImportDto = drillholeTables.SelectIntervalTables(value);

            if (drillholeImportDto.isValid == false)
            {
                throw new ImportClassException("Problem with importing table " + value.ToString());
            }

            var drillholeImport = mapper.Map<DrillholeImportDto, DrillholeTable>(drillholeImportDto);

            return drillholeImport;
        }

    }

}
