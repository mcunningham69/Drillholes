using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain.DTO;

namespace Drillholes.Domain.Interfaces
{
    public interface IIntervalValidation
    {
        Task<ValidationIntervalDto> CheckIsEmpty(ValidationMessages ValuesToCheck, XElement intervalValues);

        Task<ValidationIntervalDto> CheckForNumeric(ValidationMessages ValuesToCheck, XElement intervalValues);

        Task<ValidationIntervalDto> CheckForDuplicates(ValidationMessages ValuesToCheck, XElement intervalValues);

        Task<ValidationIntervalDto> CheckMaxDepth(ValidationMessages ValuesToCheck, List<XElement> drillholeValues);

        Task<ValidationIntervalDto> CheckMissingIntervals(ValidationMessages ValuesToCheck, List<XElement> drillholeValues);

        Task<ValidationIntervalDto> CheckNegativeIntervals(ValidationMessages ValuesToCheck, XElement drillholeValues);
        Task<ValidationIntervalDto> CheckOverlappingIntervals(ValidationMessages ValuesToCheck, XElement drillholeValues);

        Task<ValidationIntervalDto> CheckForMissingCollars(ValidationMessages ValuesToCheck, List<XElement> drillholeValues);

        Task<ValidationIntervalDto> CheckStructuralMeasurements(ValidationMessages ValuesToCheck, XElement intervalValues);
    }
}
