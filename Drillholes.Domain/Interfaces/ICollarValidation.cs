using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;

namespace Drillholes.Domain.Interfaces
{
    public interface ICollarValidation
    {
        Task<ValidationCollarDto> CheckIsEmpty(ValidationMessages ValuesToCheck, XElement collarValues);

        Task<ValidationCollarDto> CheckForNumeric(ValidationMessages ValuesToCheck, XElement collarValues);

        Task<ValidationCollarDto> CheckForDuplicates(ValidationMessages ValuesToCheck, XElement collarValues);

        Task<ValidationCollarDto> CheckMaxDepth(ValidationMessages _ValuesToCheck, XElement collarValues);

        Task<ValidationCollarDto> CheckRange(ValidationMessages _ValuesToCheck, XElement collarValues);

        Task<ValidationCollarDto> CheckForZeroCoordinate(ValidationMessages _ValuesToCheck, XElement collarValues);

    }
}
