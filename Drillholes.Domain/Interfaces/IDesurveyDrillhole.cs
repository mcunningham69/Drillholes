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
    public interface IDesurveyDrillhole
    {
        Task<CollarDesurveyDto> CreateCollarVerticalHole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, XElement collarValues);
        Task<CollarDesurveyDto> CreateCollarSurveyHole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, XElement collarValues);

        Task<SurveyDesurveyDto> CreateSurveyDownhole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, List<XElement> drillholeVales);

        Task<AssayDesurveyDto> CreateAssaySurveyHole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, List<XElement> drillholeVales);
        Task<AssayDesurveyDto> CreateAssayDownhole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, List<XElement> drillholeVales);
        Task<AssayDesurveyDto> CreateAssayVerticalHole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, List<XElement> drillholeVales);

        Task<IntervalDesurveyDto> CreateIntervalSurveyHole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, List<XElement> drillholeVales);
        Task<IntervalDesurveyDto> CreateIntervalDownhole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, List<XElement> drillholeVales);
        Task<IntervalDesurveyDto> CreateIntervalVerticalHole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, List<XElement> drillholeVales);

        Task<ContinuousDesurveyDto> CreateContinuousSurveyHole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, List<XElement> drillholeVales);
        Task<ContinuousDesurveyDto> CreateContinuousDownhole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, List<XElement> drillholeVales);
        Task<ContinuousDesurveyDto> CreateContinuousVerticalHole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, List<XElement> drillholeVales);
    }
}
