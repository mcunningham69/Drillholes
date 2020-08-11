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

        public async Task<AssayDesurveyDto> CreateAssayVerticalHole(DrillholeDesurveyEnum desurveyType, ImportTableFields collarTableFields, ImportTableFields assayTableFields, bool bToe, List<XElement> drillholeVales)
        {

            DesurveyDrillhole assayHole = new DesurveyDrillhole(DrillholeDesurveyEnum.Tangential);
            var assayDesurvey = await assayHole.AssayVerticalTrace(collarTableFields, assayTableFields, drillholeVales);

            if (assayDesurvey.Count > 0)
            {
                assayDesurvey.IsValid = true;
            }

            //need this for xml, so use table fields in the desurvey object class
            assayDesurvey.assayTableFields = assayTableFields;
            assayDesurvey.collarTableFields = collarTableFields;

            return assayDesurvey;
        }

        #region collar

        public async Task<CollarDesurveyDto> CreateCollarSurveyHole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, List<XElement> collarValues)
        {
            CollarDesurveyDto collarDesurvey = new CollarDesurveyDto();

            
            DesurveyDrillhole collarHole = new DesurveyDrillhole(DrillholeDesurveyEnum.Tangential);
            collarDesurvey = await collarHole.CollarSurveyTrace(tableFields, collarValues);


            return collarDesurvey;
        }

        public async Task<CollarDesurveyDto> CreateCollarVerticalHole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, List<XElement> collarValues)
        {

            DesurveyDrillhole collarHole = new DesurveyDrillhole(DrillholeDesurveyEnum.Tangential);
            var collarDesurvey = await collarHole.VerticalTrace(tableFields, collarValues);

            if (collarDesurvey.Count > 0)
            {
                collarDesurvey.IsValid = true;
            }

            //need this for xml, so use table fields in the desurvey object class
            collarDesurvey.collarTableFields = tableFields;

            return collarDesurvey;
        }
        #endregion

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
