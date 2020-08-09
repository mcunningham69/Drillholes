using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain;

namespace Drillholes.CreateDrillholes
{
    public class CreateHolesByType : IDesurveyDrillhole
    {
        public Task<AssayDesurveyDto> CreateAssayDownhole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, List<XElement> drillholeVales)
        {
            throw new NotImplementedException();
        }

        public Task<AssayDesurveyDto> CreateAssaySurveyHole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, List<XElement> drillholeVales)
        {
            throw new NotImplementedException();
        }

        public Task<AssayDesurveyDto> CreateAssayVerticalHole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, List<XElement> drillholeVales)
        {
            throw new NotImplementedException();
        }

        public Task<CollarDesurveyDto> CreateCollarSurveyHole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, XElement collarValues)
        {
            throw new NotImplementedException();
        }

        public Task<CollarDesurveyDto> CreateCollarVerticalHole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, XElement collarValues)
        {
            throw new NotImplementedException();
        }

        public Task<ContinuousDesurveyDto> CreateContinuousDownhole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, List<XElement> drillholeVales)
        {
            throw new NotImplementedException();
        }

        public Task<ContinuousDesurveyDto> CreateContinuousSurveyHole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, List<XElement> drillholeVales)
        {
            throw new NotImplementedException();
        }

        public Task<ContinuousDesurveyDto> CreateContinuousVerticalHole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, List<XElement> drillholeVales)
        {
            throw new NotImplementedException();
        }

        public Task<IntervalDesurveyDto> CreateIntervalDownhole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, List<XElement> drillholeVales)
        {
            throw new NotImplementedException();
        }

        public Task<IntervalDesurveyDto> CreateIntervalSurveyHole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, List<XElement> drillholeVales)
        {
            throw new NotImplementedException();
        }

        public Task<IntervalDesurveyDto> CreateIntervalVerticalHole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, List<XElement> drillholeVales)
        {
            throw new NotImplementedException();
        }

        public Task<SurveyDesurveyDto> CreateSurveyDownhole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, List<XElement> surveyValues)
        {
            throw new NotImplementedException();
        }
    }
}
