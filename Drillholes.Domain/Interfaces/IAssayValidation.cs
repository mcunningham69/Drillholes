using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain.DTO;

namespace Drillholes.Domain.Interfaces
{
    public interface IAssayValidation
    {
        Task<ValidationAssayDto> CheckIsEmpty(ValidationMessages ValuesToCheck, XElement assayValues);

        Task<ValidationAssayDto> CheckForNumeric(ValidationMessages ValuesToCheck, XElement assayValues);

        Task<ValidationAssayDto> CheckForDuplicates(ValidationMessages ValuesToCheck, XElement assayValues);

        Task<ValidationAssayDto> CheckMaxDepth(ValidationMessages ValuesToCheck, List<XElement> drillholeValues);

        Task<ValidationAssayDto> CheckMissingIntervals(ValidationMessages ValuesToCheck, List<XElement> drillholeValues);

        Task<ValidationAssayDto> CheckNegativeIntervals(ValidationMessages ValuesToCheck, XElement drillholeValues);
        Task<ValidationAssayDto> CheckOverlappingIntervals(ValidationMessages ValuesToCheck, XElement drillholeValues);

        Task<ValidationAssayDto> CheckForMissingCollars(ValidationMessages ValuesToCheck, List<XElement> drillholeValues);

        Task<ValidationAssayDto> CheckForZeroGradeValues(ValidationMessages ValuesToCheck, XElement assayValues);


    }
}
