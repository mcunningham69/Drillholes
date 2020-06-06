using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain.DTO;

namespace Drillholes.Domain.Interfaces
{
    public interface ISurveyValidation
    {
        Task<ValidationSurveyDto> CheckIsEmpty(ValidationMessages ValuesToCheck, XElement surveyValues);

        Task<ValidationSurveyDto> CheckForNumeric(ValidationMessages ValuesToCheck, XElement surveyValues);

        Task<ValidationSurveyDto> CheckForDuplicates(ValidationMessages ValuesToCheck, XElement surveyValues);

        Task<ValidationSurveyDto> CheckMaxDepth(ValidationMessages ValuesToCheck, List<XElement> drillholeValues);

        Task<ValidationSurveyDto> CheckRange(ValidationMessages ValuesToCheck, XElement surveyValues);

        Task<ValidationSurveyDto> CheckSurveyDistance(ValidationMessages ValuesToCheck, List<XElement> drillholeValues);

        Task<ValidationSurveyDto> CheckForMissingCollars(ValidationMessages ValuesToCheck, List<XElement> drillholeValues);

    }
}
