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
        #region Assay
        public async Task<AssayDesurveyDto> CreateAssayDownhole(DrillholeDesurveyEnum desurveyType, ImportTableFields collarTableFields, ImportTableFields assayTableFields,
            ImportTableFields surveyTableFields, bool bToe, bool bCollar, List<XElement> drillholeVales)
        {
            DesurveyDrillhole assayHole = new DesurveyDrillhole(desurveyType);
            var assayDesurvey = await assayHole.AssayDownholeTrace(collarTableFields, assayTableFields, surveyTableFields, drillholeVales, bToe, bCollar);

            if (assayDesurvey.Count > 0)
            {
                assayDesurvey.IsValid = true;
            }

            //need this for xml, so use table fields in the desurvey object class
            assayDesurvey.assayTableFields = assayTableFields;
            assayDesurvey.collarTableFields = collarTableFields;
            assayDesurvey.surveyTableFields = surveyTableFields;

            return assayDesurvey;
        }

        public async Task<AssayDesurveyDto> CreateAssaySurveyHole(DrillholeDesurveyEnum desurveyType, ImportTableFields collarTableFields, ImportTableFields assayTableFields, bool bToe, bool bCollar, List<XElement> drillholeVales)
        {
            DesurveyDrillhole assayHole = new DesurveyDrillhole(desurveyType);
            var assayDesurvey = await assayHole.AssayCollarSurveyTrace(collarTableFields, assayTableFields, drillholeVales, bToe, bCollar);

            if (assayDesurvey.Count > 0)
            {
                assayDesurvey.IsValid = true;
            }

            //need this for xml, so use table fields in the desurvey object class
            assayDesurvey.assayTableFields = assayTableFields;
            assayDesurvey.collarTableFields = collarTableFields;

            return assayDesurvey;
        }

        public async Task<AssayDesurveyDto> CreateAssayVerticalHole(DrillholeDesurveyEnum desurveyType, ImportTableFields collarTableFields, ImportTableFields assayTableFields, bool bToe, bool bCollar, List<XElement> drillholeVales)
        {
            DesurveyDrillhole assayHole = new DesurveyDrillhole(DrillholeDesurveyEnum.Tangential);
            var assayDesurvey = await assayHole.AssayVerticalTrace(collarTableFields, assayTableFields, drillholeVales, bToe, bCollar);

            if (assayDesurvey.Count > 0)
            {
                assayDesurvey.IsValid = true;
            }

            //need this for xml, so use table fields in the desurvey object class
            assayDesurvey.assayTableFields = assayTableFields;
            assayDesurvey.collarTableFields = collarTableFields;

            return assayDesurvey;
        }
        #endregion

        #region collar

        public async Task<CollarDesurveyDto> CreateCollarSurveyHole(DrillholeDesurveyEnum desurveyType, ImportTableFields tableFields, bool bToe, List<XElement> collarValues)
        {
            CollarDesurveyDto collarDesurvey = new CollarDesurveyDto();
            
            DesurveyDrillhole collarHole = new DesurveyDrillhole(DrillholeDesurveyEnum.Tangential);
            collarDesurvey = await collarHole.CollarSurveyTrace(tableFields, collarValues);

            if (collarDesurvey.Count > 0)
            {
                collarDesurvey.IsValid = true;
            }

            //need this for xml, so use table fields in the desurvey object class
            collarDesurvey.collarTableFields = tableFields;

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

        #region Continuous
        public async Task<ContinuousDesurveyDto> CreateContinuousDownhole(DrillholeDesurveyEnum desurveyType, ImportTableFields collarTableFields, ImportTableFields contTableFields, ImportTableFields surveyTableFields, 
            bool bToe, bool bCollar, List<XElement> drillholeVales, bool bBottom)
        {
            DesurveyDrillhole continuousHole = new DesurveyDrillhole(desurveyType);
            var continuousDesurvey = await continuousHole.ContinuousDownholeTrace(collarTableFields, contTableFields, surveyTableFields, drillholeVales, bToe, bCollar, bBottom);

            if (continuousDesurvey.Count > 0)
            {
                continuousDesurvey.IsValid = true;
            }

            //need this for xml, so use table fields in the desurvey object class
            continuousDesurvey.continuousTableFields = contTableFields;
            continuousDesurvey.collarTableFields = collarTableFields;
            continuousDesurvey.surveyTableFields = surveyTableFields;

            return continuousDesurvey;
        }

        public async Task<ContinuousDesurveyDto> CreateContinuousSurveyHole(DrillholeDesurveyEnum desurveyType, ImportTableFields collarTableFields, ImportTableFields contTableFields, bool bToe, bool bCollar, List<XElement> drillholeVales, bool bBottom)
        {
            DesurveyDrillhole continuousHole = new DesurveyDrillhole(desurveyType);
            var continuousDesurvey = await continuousHole.ContinuousCollarSurveyTrace(collarTableFields, contTableFields, drillholeVales, bToe, bCollar, bBottom);

            if (continuousDesurvey.Count > 0)
            {
                continuousDesurvey.IsValid = true;
            }

            //need this for xml, so use table fields in the desurvey object class
            continuousDesurvey.continuousTableFields = contTableFields;
            continuousDesurvey.collarTableFields = collarTableFields;

            return continuousDesurvey;
        }

        public async Task<ContinuousDesurveyDto> CreateContinuousVerticalHole(DrillholeDesurveyEnum desurveyType, ImportTableFields collarTableFields, ImportTableFields continuousTableFields, bool bToe, bool bCollar, List<XElement> drillholeVales)
        {
            DesurveyDrillhole continuousHole = new DesurveyDrillhole(DrillholeDesurveyEnum.Tangential);
            var continuousDesurvey = await continuousHole.ContinuousVerticalTrace(collarTableFields, continuousTableFields, drillholeVales, bToe, bCollar);

            if (continuousDesurvey.Count > 0)
            {
                continuousDesurvey.IsValid = true;
            }

            //need this for xml, so use table fields in the desurvey object class
            continuousDesurvey.continuousTableFields = continuousTableFields;
            continuousDesurvey.collarTableFields = collarTableFields;

            return continuousDesurvey;
        }
        #endregion

        #region Interval
        public async Task<IntervalDesurveyDto> CreateIntervalDownhole(DrillholeDesurveyEnum desurveyType, ImportTableFields collarTableFields, ImportTableFields intervalTableFields, 
            ImportTableFields surveyTableFields, bool bToe, bool bCollar, List<XElement> drillholeVales)
        {
            DesurveyDrillhole intervalHole = new DesurveyDrillhole(desurveyType);
            var intervalDesurvey = await intervalHole.IntervalDownholeTrace(collarTableFields, intervalTableFields, surveyTableFields, drillholeVales, bToe, bCollar);

            if (intervalDesurvey.Count > 0)
            {
                intervalDesurvey.IsValid = true;
            }

            //need this for xml, so use table fields in the desurvey object class
            intervalDesurvey.intervalTableFields = intervalTableFields;
            intervalDesurvey.collarTableFields = collarTableFields;
            intervalDesurvey.surveyTableFields = surveyTableFields;

            return intervalDesurvey;
        }

        public async Task<IntervalDesurveyDto> CreateIntervalSurveyHole(DrillholeDesurveyEnum desurveyType, ImportTableFields collarTableFields, ImportTableFields intervalTableFields, bool bToe, bool bCollar, List<XElement> drillholeVales)
        {
            DesurveyDrillhole intervalHole = new DesurveyDrillhole(desurveyType);
            var intervalDesurvey = await intervalHole.IntervalCollarSurveyTrace(collarTableFields, intervalTableFields, drillholeVales, bToe, bCollar);

            if (intervalDesurvey.Count > 0)
            {
                intervalDesurvey.IsValid = true;
            }

            //need this for xml, so use table fields in the desurvey object class
            intervalDesurvey.intervalTableFields = intervalTableFields;
            intervalDesurvey.collarTableFields = collarTableFields;

            return intervalDesurvey;
        }

        public async Task<IntervalDesurveyDto> CreateIntervalVerticalHole(DrillholeDesurveyEnum desurveyType, ImportTableFields collarTableFields, ImportTableFields intervalTableFields, bool bToe, bool bCollar, List<XElement> drillholeVales)
        {
            DesurveyDrillhole intervalHole = new DesurveyDrillhole(DrillholeDesurveyEnum.Tangential);
            var intervalDesurvey = await intervalHole.IntervalVerticalTrace(collarTableFields, intervalTableFields, drillholeVales, bToe, bCollar);

            if (intervalDesurvey.Count > 0)
            {
                intervalDesurvey.IsValid = true;
            }

            //need this for xml, so use table fields in the desurvey object class
            intervalDesurvey.intervalTableFields = intervalTableFields;
            intervalDesurvey.collarTableFields = collarTableFields;

            return intervalDesurvey;
        }
        #endregion

        public async Task<SurveyDesurveyDto> CreateSurveyDownhole(DrillholeDesurveyEnum desurveyType, ImportTableFields collarTableFields, ImportTableFields surveyTableFields, bool bToe, bool bCollar, List<XElement> surveyValues)
        {
            DesurveyDrillhole surveyHole = new DesurveyDrillhole(desurveyType);
            var surveyDesurvey = await surveyHole.SurveyDownholeTrace(collarTableFields, surveyTableFields, surveyValues);

            if (surveyDesurvey.Count > 0)
            {
                surveyDesurvey.IsValid = true;
            }

            //need this for xml, so use table fields in the desurvey object class
            surveyDesurvey.surveyTableFields = surveyTableFields;
            surveyDesurvey.collarTableFields = collarTableFields;

            return surveyDesurvey;
        }
    }
}
