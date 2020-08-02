using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain.DTO;

namespace Drillholes.Domain.Interfaces
{
    public interface IContinuousValidation
    {
        Task<ValidationContinuousDto> CheckIsEmpty(ValidationMessages ValuesToCheck, XElement continuousValues);

        Task<ValidationContinuousDto> CheckForNumeric(ValidationMessages ValuesToCheck, XElement continuousValues);

        Task<ValidationContinuousDto> CheckForDuplicates(ValidationMessages ValuesToCheck, XElement continuousValues);

        Task<ValidationContinuousDto> CheckMaxDepth(ValidationMessages ValuesToCheck, List<XElement> drillholeValues);

        Task<ValidationContinuousDto> CheckDistance(ValidationMessages ValuesToCheck, List<XElement> drillholeValues);

        Task<ValidationContinuousDto> CheckForMissingCollars(ValidationMessages ValuesToCheck, List<XElement> drillholeValues);
        Task<ValidationContinuousDto> CheckStructuralMeasurements(ValidationMessages ValuesToCheck, XElement continuousValues);

    }
}
