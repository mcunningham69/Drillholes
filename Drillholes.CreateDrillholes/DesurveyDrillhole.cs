using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using Drillholes.Domain.Exceptions;
using Microsoft.VisualBasic;


namespace Drillholes.CreateDrillholes
{
    public class DesurveyDrillhole
    {
        DrillholeDesurveyValues _desurveyHoles;

        public DesurveyDrillhole(DrillholeDesurveyEnum value)
        {
            SetDrillholeType(value);
        }
        public void SetDrillholeType(DrillholeDesurveyEnum value)
        {
             _desurveyHoles = DrillholeDesurveyValues.createDesurvey(value);
        }

        public async Task<CollarDesurveyDto> VerticalTrace(ImportTableFields tableFields, List<XElement> drillholeValues)
        {
            return await _desurveyHoles.CollarVerticalTrace(tableFields, drillholeValues);
        }
        public async Task<CollarDesurveyDto> CollarSurveyTrace(ImportTableFields tableFields, List<XElement> drillholeValues)
        {
            return await _desurveyHoles.CollarSurveyTrace(tableFields, drillholeValues);
        }

        public async Task<SurveyDesurveyDto> SurveyDownholeTrace(ImportTableFields collarTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues)
        {
            return await _desurveyHoles.SurveyDownhole(collarTableFields, surveyTableFields, drillholeValues);
        }

        public async Task<AssayDesurveyDto> AssayVerticalTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            return await _desurveyHoles.AssayVerticalTrace(collarTableFields, assayTableFields, drillholeValues, bToe, bCollar);
        }
        public async Task<AssayDesurveyDto> AssayCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            return await _desurveyHoles.AssayCollarSurveyTrace(collarTableFields, assayTableFields, drillholeValues, bToe, bCollar);
        }
        public async Task<AssayDesurveyDto> AssayDownholeTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            return await _desurveyHoles.AssayDownholeTrace(collarTableFields, assayTableFields, surveyTableFields, drillholeValues, bToe, bCollar);
        }

        public async Task<IntervalDesurveyDto> IntervalVerticalTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            return await _desurveyHoles.IntervalVerticalTrace(collarTableFields, intervalTableFields, drillholeValues, bToe, bCollar);
        }
        public async Task<IntervalDesurveyDto> IntervalCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            return await _desurveyHoles.IntervalCollarSurveyTrace(collarTableFields, intervalTableFields, drillholeValues, bToe, bCollar);
        }
        public async Task<IntervalDesurveyDto> IntervalDownholeTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            return await _desurveyHoles.IntervalDownholeTrace(collarTableFields, intervalTableFields,surveyTableFields, drillholeValues, bToe, bCollar);
        }

        public async Task<ContinuousDesurveyDto> ContinuousVerticalTrace(ImportTableFields collarTableFields, ImportTableFields contTableFields, List<XElement> drillholeValues, bool bToe, 
            bool bCollar)
        {
            return await _desurveyHoles.ContinuousVerticalTrace(collarTableFields, contTableFields, drillholeValues, bToe, bCollar);
        }
        public async Task<ContinuousDesurveyDto> ContinuousCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields contTableFields, List<XElement> drillholeValues, 
            bool bToe, bool bCollar, bool bBottom)
        {
            return await _desurveyHoles.ContinuousCollarSurveyTrace(collarTableFields, contTableFields, drillholeValues, bToe, bCollar, bBottom);
        }
        public async Task<ContinuousDesurveyDto> ContinuousDownholeTrace(ImportTableFields collarTableFields, ImportTableFields contTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, 
            bool bToe, bool bCollar, bool bBottom)
        {
            return await _desurveyHoles.ContinuousDownholeTrace(collarTableFields, contTableFields, surveyTableFields, drillholeValues, bToe, bCollar, bBottom);
        }

    }

    public abstract class DrillholeDesurveyValues
    {
        public static DrillholeDesurveyValues createDesurvey(DrillholeDesurveyEnum value)
        {
            switch (value)
            {
                case DrillholeDesurveyEnum.AverageAngle:
                    {
                        return new AverageAngle();

                    }
                case DrillholeDesurveyEnum.BalancedTangential:
                    {
                        return new BalancedTangential();
                    }
                case DrillholeDesurveyEnum.MinimumCurvature:
                    {
                        return new MinimumCurvature();
                    }
                case DrillholeDesurveyEnum.RadiusCurvature:
                    {
                        return new RadiusCurvature();
                    }
                case DrillholeDesurveyEnum.Tangential:
                    {
                        return new Tangential();
                    }
                default:
                    throw new Exception("Generic error with desurvey method");
            }


        }

        public abstract Task<CollarDesurveyDto> CollarVerticalTrace(ImportTableFields tableFields, List<XElement> drillholeValues);
        public abstract Task<CollarDesurveyDto> CollarSurveyTrace(ImportTableFields tableFields, List<XElement> drillholeValues);

        public abstract Task<SurveyDesurveyDto> SurveyDownhole(ImportTableFields collarTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues);

        public abstract Task<AssayDesurveyDto> AssayVerticalTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar);
        public abstract Task<AssayDesurveyDto> AssayCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar);
        public abstract Task<AssayDesurveyDto> AssayDownholeTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar);

        public abstract Task<IntervalDesurveyDto> IntervalVerticalTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar);
        public abstract Task<IntervalDesurveyDto> IntervalCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar);
        public abstract Task<IntervalDesurveyDto> IntervalDownholeTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar);


        public abstract Task<ContinuousDesurveyDto> ContinuousVerticalTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues,
            bool bToe, bool bCollar);
        public abstract Task<ContinuousDesurveyDto> ContinuousCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues,
            bool bToe, bool bCollar, bool bBottom);
        public abstract Task<ContinuousDesurveyDto> ContinuousDownholeTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, ImportTableFields surveyTableFields,
            List<XElement> drillholeValues, bool bToe, bool bCollar, bool bBottom);



    }

    public class AverageAngle : DrillholeDesurveyValues
    {
        #region Assay
        /// <summary>
        /// Desurvey assay table using downhole surveys from survey table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="assayTableFields"></param>
        /// <param name="surveyTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<AssayDesurveyDto> AssayDownholeTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            AssayDesurveyDto assayDesurveyDto = await DesurveyMethods.AssayDownholeUpdate(collarTableFields, assayTableFields, surveyTableFields, drillholeValues,
                bToe, bCollar, DrillholeDesurveyEnum.AverageAngle);

            return assayDesurveyDto;
        }

        /// <summary>
        /// Desurvey assay table using dip and azimuth from collar table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="assayTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<AssayDesurveyDto> AssayCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            AssayDesurveyDto assayDesurveyDto = await DesurveyMethods.AssayCollarSurveyTrace(collarTableFields, assayTableFields, drillholeValues, bToe, bCollar,
                DrillholeDesurveyEnum.AverageAngle);

            return assayDesurveyDto;
        }

        /// <summary>
        /// Creates vertical intervals for each assay record
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="assayTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<AssayDesurveyDto> AssayVerticalTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            AssayDesurveyDto assayDesurveyDto = await DesurveyMethods.AssayVerticalTrace(collarTableFields, assayTableFields, drillholeValues, bToe, bCollar,
                DrillholeDesurveyEnum.AverageAngle);

            return assayDesurveyDto;
        }
        #endregion

        #region Collar
        /// <summary>
        /// Create two points per collar
        /// </summary>
        /// <param name="tableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <returns></returns>
        public override async Task<CollarDesurveyDto> CollarVerticalTrace(ImportTableFields tableFields, List<XElement> drillholeValues)
        {
            CollarDesurveyDto collarDesurveyDto = await DesurveyMethods.CollarVerticalTrace(tableFields, drillholeValues, DrillholeDesurveyEnum.AverageAngle);



            return collarDesurveyDto;
        }

        /// <summary>
        /// Creates collar and toe based on azimuth and dip
        /// </summary>
        /// <param name="tableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <returns></returns>
        public override async Task<CollarDesurveyDto> CollarSurveyTrace(ImportTableFields tableFields, List<XElement> drillholeValues)
        {
            CollarDesurveyDto collarDesurveyDto = await DesurveyMethods.CollarSurveyTrace(tableFields, drillholeValues, DrillholeDesurveyEnum.AverageAngle);

            return collarDesurveyDto;
        }
        #endregion

        #region Continuous

        /// <summary>
        /// Create a desurveyed continuous table (i.e. distance to field) using dip and azi from collar table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="continuousTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<ContinuousDesurveyDto> ContinuousCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues,
            bool bToe, bool bCollar, bool bBottom)
        {
            ContinuousDesurveyDto continuousDesurveyDto = await DesurveyMethods.ContinuousCollarSurveyTrace(collarTableFields, continuousTableFields, drillholeValues, bToe,
                bCollar, DrillholeDesurveyEnum.AverageAngle, bBottom);

            return continuousDesurveyDto;
        }

        /// <summary>
        /// Desurvey continuous table using downhole surveys from survey table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="continuousTableFields"></param>
        /// <param name="surveyTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<ContinuousDesurveyDto> ContinuousDownholeTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues,
            bool bToe, bool bCollar, bool bBottom)
        {
            ContinuousDesurveyDto continuousDesurveyDto = await DesurveyMethods.ContinuousDownholeTrace(collarTableFields, continuousTableFields, surveyTableFields, drillholeValues, bToe,
                bCollar, bBottom, DrillholeDesurveyEnum.AverageAngle);

            return continuousDesurveyDto;
        }

        /// <summary>
        /// Create a vertical desurvey for continuous table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="continuousTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<ContinuousDesurveyDto> ContinuousVerticalTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues,
            bool bToe, bool bCollar)
        {
            ContinuousDesurveyDto continuousDesurveyDto = await DesurveyMethods.ContinuousVerticalTrace(collarTableFields, continuousTableFields, drillholeValues, bToe, bCollar,
                DrillholeDesurveyEnum.AverageAngle, true);

            return continuousDesurveyDto;
        }
        #endregion

        #region Interval

        /// <summary>
        /// Desurvey interval table using azimuth and dip from collar table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="intervalTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<IntervalDesurveyDto> IntervalCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            IntervalDesurveyDto intervalDesurveyDto = await DesurveyMethods.IntervalCollarSurveyTrace(collarTableFields, intervalTableFields, drillholeValues, bToe, bCollar,
                DrillholeDesurveyEnum.AverageAngle);

            return intervalDesurveyDto;
        }

        /// <summary>
        /// Desurvey interval table using dowhole surveys from survey table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="intervalTableFields"></param>
        /// <param name="surveyTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<IntervalDesurveyDto> IntervalDownholeTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            IntervalDesurveyDto intervalDesurveyDto = await DesurveyMethods.IntervalDownholeTrace(collarTableFields, intervalTableFields, surveyTableFields, drillholeValues,
                bToe, bCollar, DrillholeDesurveyEnum.AverageAngle);

            return intervalDesurveyDto;
        }

        /// <summary>
        /// Create a vertical trace for each interval
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="intervalTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<IntervalDesurveyDto> IntervalVerticalTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            IntervalDesurveyDto intervalDesurveyDto = await DesurveyMethods.IntervalVerticalTrace(collarTableFields, intervalTableFields, drillholeValues, bToe, bCollar,
                DrillholeDesurveyEnum.AverageAngle);

            return intervalDesurveyDto;
        }
        #endregion

        public override async Task<SurveyDesurveyDto> SurveyDownhole(ImportTableFields collarTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues)
        {
            SurveyDesurveyDto surveyDesurveyDto = await DesurveyMethods.SurveyDownholeUpdate(collarTableFields, surveyTableFields, drillholeValues, DrillholeDesurveyEnum.AverageAngle);

            return surveyDesurveyDto;
        }
    }

    public class BalancedTangential : DrillholeDesurveyValues
    {
        #region Assay
        /// <summary>
        /// Desurvey assay table using downhole surveys from survey table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="assayTableFields"></param>
        /// <param name="surveyTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<AssayDesurveyDto> AssayDownholeTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            AssayDesurveyDto assayDesurveyDto = await DesurveyMethods.AssayDownholeUpdate(collarTableFields, assayTableFields, surveyTableFields, drillholeValues,
                bToe, bCollar, DrillholeDesurveyEnum.BalancedTangential);

            return assayDesurveyDto;
        }

        /// <summary>
        /// Desurvey assay table using dip and azimuth from collar table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="assayTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<AssayDesurveyDto> AssayCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            AssayDesurveyDto assayDesurveyDto = await DesurveyMethods.AssayCollarSurveyTrace(collarTableFields, assayTableFields, drillholeValues, bToe, bCollar,
                DrillholeDesurveyEnum.BalancedTangential);

            return assayDesurveyDto;
        }

        /// <summary>
        /// Creates vertical intervals for each assay record
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="assayTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<AssayDesurveyDto> AssayVerticalTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            AssayDesurveyDto assayDesurveyDto = await DesurveyMethods.AssayVerticalTrace(collarTableFields, assayTableFields, drillholeValues, bToe, bCollar,
                DrillholeDesurveyEnum.BalancedTangential);

            return assayDesurveyDto;
        }
        #endregion

        #region Collar
        /// <summary>
        /// Create two points per collar
        /// </summary>
        /// <param name="tableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <returns></returns>
        public override async Task<CollarDesurveyDto> CollarVerticalTrace(ImportTableFields tableFields, List<XElement> drillholeValues)
        {
            CollarDesurveyDto collarDesurveyDto = await DesurveyMethods.CollarVerticalTrace(tableFields, drillholeValues, DrillholeDesurveyEnum.BalancedTangential);



            return collarDesurveyDto;
        }

        /// <summary>
        /// Creates collar and toe based on azimuth and dip
        /// </summary>
        /// <param name="tableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <returns></returns>
        public override async Task<CollarDesurveyDto> CollarSurveyTrace(ImportTableFields tableFields, List<XElement> drillholeValues)
        {
            CollarDesurveyDto collarDesurveyDto = await DesurveyMethods.CollarSurveyTrace(tableFields, drillholeValues, DrillholeDesurveyEnum.BalancedTangential);

            return collarDesurveyDto;
        }
        #endregion

        #region Continuous

        /// <summary>
        /// Create a desurveyed continuous table (i.e. distance to field) using dip and azi from collar table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="continuousTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<ContinuousDesurveyDto> ContinuousCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues,
            bool bToe, bool bCollar, bool bBottom)
        {
            ContinuousDesurveyDto continuousDesurveyDto = await DesurveyMethods.ContinuousCollarSurveyTrace(collarTableFields, continuousTableFields, drillholeValues, bToe,
                bCollar, DrillholeDesurveyEnum.BalancedTangential, bBottom);

            return continuousDesurveyDto;
        }

        /// <summary>
        /// Desurvey continuous table using downhole surveys from survey table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="continuousTableFields"></param>
        /// <param name="surveyTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<ContinuousDesurveyDto> ContinuousDownholeTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues,
            bool bToe, bool bCollar, bool bBottom)
        {
            ContinuousDesurveyDto continuousDesurveyDto = await DesurveyMethods.ContinuousDownholeTrace(collarTableFields, continuousTableFields, surveyTableFields, drillholeValues, bToe,
                bCollar, bBottom, DrillholeDesurveyEnum.BalancedTangential);

            return continuousDesurveyDto;
        }

        /// <summary>
        /// Create a vertical desurvey for continuous table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="continuousTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<ContinuousDesurveyDto> ContinuousVerticalTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues,
            bool bToe, bool bCollar)
        {
            ContinuousDesurveyDto continuousDesurveyDto = await DesurveyMethods.ContinuousVerticalTrace(collarTableFields, continuousTableFields, drillholeValues, bToe, bCollar,
                DrillholeDesurveyEnum.BalancedTangential, true);

            return continuousDesurveyDto;
        }
        #endregion

        #region Interval

        /// <summary>
        /// Desurvey interval table using azimuth and dip from collar table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="intervalTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<IntervalDesurveyDto> IntervalCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            IntervalDesurveyDto intervalDesurveyDto = await DesurveyMethods.IntervalCollarSurveyTrace(collarTableFields, intervalTableFields, drillholeValues, bToe, bCollar,
                DrillholeDesurveyEnum.BalancedTangential);

            return intervalDesurveyDto;
        }

        /// <summary>
        /// Desurvey interval table using dowhole surveys from survey table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="intervalTableFields"></param>
        /// <param name="surveyTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<IntervalDesurveyDto> IntervalDownholeTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            IntervalDesurveyDto intervalDesurveyDto = await DesurveyMethods.IntervalDownholeTrace(collarTableFields, intervalTableFields, surveyTableFields, drillholeValues,
                bToe, bCollar, DrillholeDesurveyEnum.BalancedTangential);

            return intervalDesurveyDto;
        }

        /// <summary>
        /// Create a vertical trace for each interval
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="intervalTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<IntervalDesurveyDto> IntervalVerticalTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            IntervalDesurveyDto intervalDesurveyDto = await DesurveyMethods.IntervalVerticalTrace(collarTableFields, intervalTableFields, drillholeValues, bToe, bCollar,
                DrillholeDesurveyEnum.BalancedTangential);

            return intervalDesurveyDto;
        }
        #endregion

        public override async Task<SurveyDesurveyDto> SurveyDownhole(ImportTableFields collarTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues)
        {
            SurveyDesurveyDto surveyDesurveyDto = await DesurveyMethods.SurveyDownholeUpdate(collarTableFields, surveyTableFields, drillholeValues, DrillholeDesurveyEnum.BalancedTangential);

            return surveyDesurveyDto;
        }
    }

    public class MinimumCurvature : DrillholeDesurveyValues
    {
        #region Assay
        /// <summary>
        /// Desurvey assay table using downhole surveys from survey table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="assayTableFields"></param>
        /// <param name="surveyTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<AssayDesurveyDto> AssayDownholeTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            AssayDesurveyDto assayDesurveyDto = await DesurveyMethods.AssayDownholeUpdate(collarTableFields, assayTableFields, surveyTableFields, drillholeValues,
                bToe, bCollar, DrillholeDesurveyEnum.MinimumCurvature);

            return assayDesurveyDto;
        }

        /// <summary>
        /// Desurvey assay table using dip and azimuth from collar table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="assayTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<AssayDesurveyDto> AssayCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            AssayDesurveyDto assayDesurveyDto = await DesurveyMethods.AssayCollarSurveyTrace(collarTableFields, assayTableFields, drillholeValues, bToe, bCollar,
                DrillholeDesurveyEnum.MinimumCurvature);

            return assayDesurveyDto;
        }

        /// <summary>
        /// Creates vertical intervals for each assay record
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="assayTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<AssayDesurveyDto> AssayVerticalTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            AssayDesurveyDto assayDesurveyDto = await DesurveyMethods.AssayVerticalTrace(collarTableFields, assayTableFields, drillholeValues, bToe, bCollar,
                DrillholeDesurveyEnum.MinimumCurvature);

            return assayDesurveyDto;
        }
        #endregion

        #region Collar
        /// <summary>
        /// Create two points per collar
        /// </summary>
        /// <param name="tableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <returns></returns>
        public override async Task<CollarDesurveyDto> CollarVerticalTrace(ImportTableFields tableFields, List<XElement> drillholeValues)
        {
            CollarDesurveyDto collarDesurveyDto = await DesurveyMethods.CollarVerticalTrace(tableFields, drillholeValues, DrillholeDesurveyEnum.MinimumCurvature);

            return collarDesurveyDto;
        }

        /// <summary>
        /// Creates collar and toe based on azimuth and dip
        /// </summary>
        /// <param name="tableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <returns></returns>
        public override async Task<CollarDesurveyDto> CollarSurveyTrace(ImportTableFields tableFields, List<XElement> drillholeValues)
        {
            CollarDesurveyDto collarDesurveyDto = await DesurveyMethods.CollarSurveyTrace(tableFields, drillholeValues, DrillholeDesurveyEnum.MinimumCurvature);

            return collarDesurveyDto;
        }
        #endregion

        #region Continuous

        /// <summary>
        /// Create a desurveyed continuous table (i.e. distance to field) using dip and azi from collar table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="continuousTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<ContinuousDesurveyDto> ContinuousCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues,
            bool bToe, bool bCollar, bool bBottom)
        {
            ContinuousDesurveyDto continuousDesurveyDto = await DesurveyMethods.ContinuousCollarSurveyTrace(collarTableFields, continuousTableFields, drillholeValues, bToe,
                bCollar, DrillholeDesurveyEnum.MinimumCurvature, bBottom);

            return continuousDesurveyDto;
        }

        /// <summary>
        /// Desurvey continuous table using downhole surveys from survey table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="continuousTableFields"></param>
        /// <param name="surveyTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<ContinuousDesurveyDto> ContinuousDownholeTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues,
            bool bToe, bool bCollar, bool bBottom)
        {
            ContinuousDesurveyDto continuousDesurveyDto = await DesurveyMethods.ContinuousDownholeTrace(collarTableFields, continuousTableFields, surveyTableFields, drillholeValues, bToe,
                bCollar, bBottom, DrillholeDesurveyEnum.MinimumCurvature);

            return continuousDesurveyDto;
        }

        /// <summary>
        /// Create a vertical desurvey for continuous table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="continuousTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<ContinuousDesurveyDto> ContinuousVerticalTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues,
            bool bToe, bool bCollar)
        {
            ContinuousDesurveyDto continuousDesurveyDto = await DesurveyMethods.ContinuousVerticalTrace(collarTableFields, continuousTableFields, drillholeValues, bToe, bCollar,
                DrillholeDesurveyEnum.MinimumCurvature, true);

            return continuousDesurveyDto;
        }
        #endregion

        #region Interval

        /// <summary>
        /// Desurvey interval table using azimuth and dip from collar table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="intervalTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<IntervalDesurveyDto> IntervalCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            IntervalDesurveyDto intervalDesurveyDto = await DesurveyMethods.IntervalCollarSurveyTrace(collarTableFields, intervalTableFields, drillholeValues, bToe, bCollar,
                DrillholeDesurveyEnum.MinimumCurvature);
            return intervalDesurveyDto;
        }

        /// <summary>
        /// Desurvey interval table using dowhole surveys from survey table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="intervalTableFields"></param>
        /// <param name="surveyTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<IntervalDesurveyDto> IntervalDownholeTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            IntervalDesurveyDto intervalDesurveyDto = await DesurveyMethods.IntervalDownholeTrace(collarTableFields, intervalTableFields, surveyTableFields, drillholeValues,
                bToe, bCollar, DrillholeDesurveyEnum.MinimumCurvature);

            return intervalDesurveyDto;
        }

        /// <summary>
        /// Create a vertical trace for each interval
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="intervalTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<IntervalDesurveyDto> IntervalVerticalTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            IntervalDesurveyDto intervalDesurveyDto = await DesurveyMethods.IntervalVerticalTrace(collarTableFields, intervalTableFields, drillholeValues, bToe, bCollar,
                DrillholeDesurveyEnum.MinimumCurvature);

            return intervalDesurveyDto;
        }
        #endregion

        public override async Task<SurveyDesurveyDto> SurveyDownhole(ImportTableFields collarTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues)
        {
            SurveyDesurveyDto surveyDesurveyDto = await DesurveyMethods.SurveyDownholeUpdate(collarTableFields, surveyTableFields, drillholeValues, DrillholeDesurveyEnum.MinimumCurvature);

            return surveyDesurveyDto;
        }
    }

    public class RadiusCurvature : DrillholeDesurveyValues
    {
        #region Assay
        /// <summary>
        /// Desurvey assay table using downhole surveys from survey table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="assayTableFields"></param>
        /// <param name="surveyTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<AssayDesurveyDto> AssayDownholeTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            AssayDesurveyDto assayDesurveyDto = await DesurveyMethods.AssayDownholeUpdate(collarTableFields, assayTableFields, surveyTableFields, drillholeValues,
                bToe, bCollar, DrillholeDesurveyEnum.RadiusCurvature);

            return assayDesurveyDto;
        }

        /// <summary>
        /// Desurvey assay table using dip and azimuth from collar table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="assayTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<AssayDesurveyDto> AssayCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            AssayDesurveyDto assayDesurveyDto = await DesurveyMethods.AssayCollarSurveyTrace(collarTableFields, assayTableFields, drillholeValues, bToe, bCollar,
                DrillholeDesurveyEnum.RadiusCurvature);

            return assayDesurveyDto;
        }

        /// <summary>
        /// Creates vertical intervals for each assay record
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="assayTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<AssayDesurveyDto> AssayVerticalTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues,
            bool bToe, bool bCollar)
        {
            AssayDesurveyDto assayDesurveyDto = await DesurveyMethods.AssayVerticalTrace(collarTableFields, assayTableFields, drillholeValues, bToe, bCollar,
                DrillholeDesurveyEnum.RadiusCurvature);

            return assayDesurveyDto;
        }
        #endregion

        #region Collar
        /// <summary>
        /// Create two points per collar
        /// </summary>
        /// <param name="tableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <returns></returns>
        public override async Task<CollarDesurveyDto> CollarVerticalTrace(ImportTableFields tableFields, List<XElement> drillholeValues)
        {
            CollarDesurveyDto collarDesurveyDto = await DesurveyMethods.CollarVerticalTrace(tableFields, drillholeValues, DrillholeDesurveyEnum.RadiusCurvature);

            return collarDesurveyDto;
        }

        /// <summary>
        /// Creates collar and toe based on azimuth and dip
        /// </summary>
        /// <param name="tableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <returns></returns>
        public override async Task<CollarDesurveyDto> CollarSurveyTrace(ImportTableFields tableFields, List<XElement> drillholeValues)
        {
            CollarDesurveyDto collarDesurveyDto = await DesurveyMethods.CollarSurveyTrace(tableFields, drillholeValues, DrillholeDesurveyEnum.RadiusCurvature);

            return collarDesurveyDto;
        }
        #endregion

        #region Continuous

        /// <summary>
        /// Create a desurveyed continuous table (i.e. distance to field) using dip and azi from collar table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="continuousTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<ContinuousDesurveyDto> ContinuousCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues,
            bool bToe, bool bCollar, bool bBottom)
        {
            ContinuousDesurveyDto continuousDesurveyDto = await DesurveyMethods.ContinuousCollarSurveyTrace(collarTableFields, continuousTableFields, drillholeValues, bToe,
                bCollar, DrillholeDesurveyEnum.RadiusCurvature, bBottom);

            return continuousDesurveyDto;
        }

        /// <summary>
        /// Desurvey continuous table using downhole surveys from survey table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="continuousTableFields"></param>
        /// <param name="surveyTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<ContinuousDesurveyDto> ContinuousDownholeTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues,
            bool bToe, bool bCollar, bool bBottom)
        {
            ContinuousDesurveyDto continuousDesurveyDto = await DesurveyMethods.ContinuousDownholeTrace(collarTableFields, continuousTableFields, surveyTableFields, drillholeValues, bToe,
                bCollar, bBottom, DrillholeDesurveyEnum.RadiusCurvature);

            return continuousDesurveyDto;
        }

        /// <summary>
        /// Create a vertical desurvey for continuous table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="continuousTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<ContinuousDesurveyDto> ContinuousVerticalTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues,
            bool bToe, bool bCollar)
        {
            ContinuousDesurveyDto continuousDesurveyDto = await DesurveyMethods.ContinuousVerticalTrace(collarTableFields, continuousTableFields, drillholeValues, bToe, bCollar,
                DrillholeDesurveyEnum.RadiusCurvature, true);

            return continuousDesurveyDto;
        }
        #endregion

        #region Interval

        /// <summary>
        /// Desurvey interval table using azimuth and dip from collar table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="intervalTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<IntervalDesurveyDto> IntervalCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            IntervalDesurveyDto intervalDesurveyDto = await DesurveyMethods.IntervalCollarSurveyTrace(collarTableFields, intervalTableFields, drillholeValues, bToe, bCollar,
                DrillholeDesurveyEnum.RadiusCurvature);
            return intervalDesurveyDto;
        }

        /// <summary>
        /// Desurvey interval table using dowhole surveys from survey table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="intervalTableFields"></param>
        /// <param name="surveyTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<IntervalDesurveyDto> IntervalDownholeTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            IntervalDesurveyDto intervalDesurveyDto = await DesurveyMethods.IntervalDownholeTrace(collarTableFields, intervalTableFields, surveyTableFields, drillholeValues,
                bToe, bCollar, DrillholeDesurveyEnum.RadiusCurvature);

            return intervalDesurveyDto;
        }

        /// <summary>
        /// Create a vertical trace for each interval
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="intervalTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<IntervalDesurveyDto> IntervalVerticalTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            IntervalDesurveyDto intervalDesurveyDto = await DesurveyMethods.IntervalVerticalTrace(collarTableFields, intervalTableFields, drillholeValues, bToe, bCollar,
                DrillholeDesurveyEnum.RadiusCurvature);

            return intervalDesurveyDto;
        }
        #endregion

        public override async Task<SurveyDesurveyDto> SurveyDownhole(ImportTableFields collarTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues)
        {
            SurveyDesurveyDto surveyDesurveyDto = await DesurveyMethods.SurveyDownholeUpdate(collarTableFields, surveyTableFields, drillholeValues, DrillholeDesurveyEnum.RadiusCurvature);

            return surveyDesurveyDto;
        }
    }

    public class Tangential : DrillholeDesurveyValues
    {
        #region Assay
        /// <summary>
        /// Desurvey assay table using downhole surveys from survey table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="assayTableFields"></param>
        /// <param name="surveyTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<AssayDesurveyDto> AssayDownholeTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            //AssayDesurveyDto assayDesurveyDto = await DesurveyMethods.AssayDownholeUpdate(collarTableFields, assayTableFields, surveyTableFields, drillholeValues,
            //    bToe, bCollar, DrillholeDesurveyEnum.Tangential);

            AssayDesurveyDto assayDesurveyDto = new AssayDesurveyDto()
            {
                desurveyType = DrillholeDesurveyEnum.Tangential,
                surveyType = DrillholeSurveyType.downholesurvey
            };

            #region fieldnames
            //Need holeID, x, y, z, length => reference name in xml
            var collarHoleID = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            var xField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.xName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            var yField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.yName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            var zField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.zName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            var tdField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();

            //Survey table
            var surveyHoleID = surveyTableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            var distanceField = surveyTableFields.Where(f => f.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            var dipField = surveyTableFields.Where(f => f.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            var azimuthField = surveyTableFields.Where(f => f.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();

            if (dipField == null || azimuthField == null || distanceField == null)
            {
                throw new SurveyException("Check Dip, Azimuth and Distance fields. Assays");
            }

            //TODO - add assay fields 
            var assayHoleID = assayTableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            var mFromField = assayTableFields.Where(f => f.columnImportName == DrillholeConstants.distFromName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            var mToField = assayTableFields.Where(f => f.columnImportName == DrillholeConstants.distToName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            #endregion

            XElement elements = drillholeValues[0];
            var collarElements = elements.Elements();

            XElement surElements = drillholeValues[1];
            var surveyElements = surElements.Elements();
            //surveyElements = surveyElements.OrderBy(o => Convert.ToDouble(o.Element(distanceField).Value));

            XElement assElements = drillholeValues[2];
            var assayElements = assElements.Elements();
            assayElements = assayElements.OrderBy(o => Convert.ToDouble(o.Element(mFromField).Value));

            //CHECK FOR DUPLICATE HOLES AND CHANGE FLAG TO IGNORE ONE OF THEM
            var dupHoles = collarElements.GroupBy(d => d.Element(collarHoleID).Value).Where(group => group.Count() > 1).Select(group => group.Key).ToList();

            //Change to ignore in XML for dup hole
            for (int i = 0; i < dupHoles.Count; i++)
            {
                var selectDup = collarElements.Where(h => h.Element(collarHoleID).Value == dupHoles[i]).ToList();

                bool bFirst = true;
                foreach (var dup in selectDup)
                {
                    if (!bFirst)
                    {
                        dup.Attribute("Ignore").Value = "true";
                    }

                    bFirst = false;
                }
            }

            //return collar coordiantes and length for all holes which are not flagged to be ignored
            var showElements = collarElements.Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE");

            foreach (XElement element in showElements)
            {
                //get the values from XML
                string holeAttr = element.Attribute("ID").Value;
                string hole = element.Element(collarHoleID).Value;
                string xCoord = element.Element(xField).Value;
                string yCoord = element.Element(yField).Value;
                string zCoord = element.Element(zField).Value;
                string totalDepth = element.Element(tdField).Value; //FOR NOW< IGNORE

                //Check hole exists in Assay table
                int assayCount = assayElements.Where(h => h.Element(assayHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Count(); //check collar exists in assay table

                if (assayCount > 0) //otherwise no assays for particular hole
                {
                    #region Collar Attributes
                    double dblSurvX1 = 0.0, dblSurvY1 = 0.0, dblSurvZ1 = 0.0, dblLength = 0.0;

                    //sets the sequence from the collar
                    dblSurvX1 = Convert.ToDouble(xCoord);
                    dblSurvY1 = Convert.ToDouble(yCoord);
                    dblSurvZ1 = Convert.ToDouble(zCoord);
                    dblLength = Convert.ToDouble(totalDepth);
                    #endregion

                    var assayHole = assayElements.Where(h => h.Element(assayHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").ToList(); //get all samples for hole and ignroe those flagged
                    List<string> depthsFrom = assayHole.Select(f => f.Element(mFromField).Value).ToList();
                    List<string> depthsTo = assayHole.Select(f => f.Element(mToField).Value).ToList();
                    List<string> assayHoleAttr = assayHole.Select(f => f.Attribute("ID").Value).ToList();

                    #region Validate and populate assay values
                    List<double> dblFromDepths = new List<double>();
                    List<double> dblToDepths = new List<double>();
                    List<int> assayIDs = new List<int>();

                    //convert depths to from string to double
                    for (int a = 0; a < depthsFrom.Count; a++)
                    {
                        if (Information.IsNumeric(assayHoleAttr[a]))
                            assayIDs.Add(Convert.ToInt16(assayHoleAttr[a]));
                        else
                            assayIDs.Add(-99);
                        if (Information.IsNumeric(depthsFrom[a]))
                            dblFromDepths.Add(Convert.ToDouble(depthsFrom[a]));
                        else
                            dblFromDepths.Add(-99.0);
                        if (Information.IsNumeric(depthsTo[a]))
                            dblToDepths.Add(Convert.ToDouble(depthsTo[a]));

                    }
                    #endregion

                    //check if hole exists in survey table
                    int surveyCount = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Count(); //check collar exists in survey table

                   // DownholeSurveys surveys = null;
                    List<double> dblDips = new List<double>();
                    List<double> dblAzimuths = new List<double>();
                    List<double> surveyDistances = new List<double>();
                    List<int> surveyIDs = new List<int>();

                    if (surveyCount == 0)
                    {
                        dblDips.Add(-90.0);
                        dblAzimuths.Add(0.0);
                        surveyDistances.Add(0.0);
                        surveyIDs.Add(-99);
                    }
                    else //(surveyCount > 0) //it means no survey so make vertical. //TODO - ignore if required
                    {
                        #region Validate and Populate lists with survey data
                        // var surveyHole = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").ToList(); 
                        var dips = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Element(dipField).Value).ToList();
                        var azimuths = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Element(azimuthField).Value).ToList();
                        var distances = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Element(distanceField).Value).ToList();
                        var surveyAttrs = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Attribute("ID").Value).ToList();

                        for (int d = 0; d < surveyAttrs.Count; d++)
                        {
                            if (Information.IsNumeric(dips[d]))
                                dblDips.Add(Convert.ToDouble(dips[d]));
                            else
                                dblDips.Add(-90.0);
                            if (Information.IsNumeric(azimuths[d]))
                                dblAzimuths.Add(Convert.ToDouble(azimuths[d]));
                            else
                                dblAzimuths.Add(0.0);
                            if (Information.IsNumeric(distances[d]))
                                surveyDistances.Add(Convert.ToDouble(distances[d]));
                            else
                                surveyDistances.Add(-99.0);
                            if (Information.IsNumeric(surveyAttrs[d]))
                                surveyIDs.Add(Convert.ToInt16(surveyAttrs[d]));
                            else
                                surveyIDs.Add(-99);

                        }
                        #endregion
                    }

                    for (int i = 0; i < surveyDistances.Count; i++)
                    {
                        Coordinate3D surveyCoordinate = new Coordinate3D();

                        List<double> getFromRange = new List<double>();
                        List<double> getToRange = new List<double>();

                        double upperLimit = 0.0, lowerLimit = 0.0, dblCalcDip = 0.0, dblCalcAzi = 0.0, dblCalcDist = 0.0;

                        if (i == 0) //first one
                        {
                            lowerLimit = 0.0;
                            getFromRange = dblFromDepths.Where(d => d <= surveyDistances[i + 1]).ToList();
                            upperLimit = dblToDepths.Where(d => d >= surveyDistances[i + 1]).FirstOrDefault();
                            getToRange = dblToDepths.Where(d => d <= upperLimit).ToList();

                            if (bCollar)
                            {
                                await DesurveyMethods.PopulateNewDesurveyAssay(dblSurvX1, dblSurvY1, dblSurvZ1, 0, 0, Convert.ToInt16(holeAttr), -99, hole, assayDesurveyDto, false, surveyIDs[i]);
                            }

                            surveyCoordinate = await CalculateSurveys.ReturnCoordinateTangential(dblSurvX1, dblSurvY1, dblSurvZ1, surveyDistances[1], dblAzimuths[0], dblDips[0]);

                            List<Coordinate3D> intervalCoordinate = await DesurveyMethods.DownholeIntervalCoordinate(dblSurvX1, dblSurvY1, dblSurvZ1, surveyCoordinate.x, surveyCoordinate.y, surveyCoordinate.z,
                                getToRange, getFromRange, surveyDistances[i + 1]);

                            //overkill for Tangential
                            //dblCalcDip = await ReturnSurveyDipBetweenPoints(dblSurvZ1 - surveyCoordinate.z, surveyDistances[i + 1]);
                            //dblCalcAzi = await ReturnSurveyDirectionBetweenPoints(dblSurvX1, dblSurvY1, surveyCoordinate.x, surveyCoordinate.y);
                            //dblCalcDist = await ReturnSurveyDistanceBetweenPoints(dblSurvX1, dblSurvY1, dblSurvZ1, surveyCoordinate.x, surveyCoordinate.y, surveyCoordinate.z);

                            //for (int log = 0; log < getToRange.Count; log++)
                            //{


                            //}
                        }
                        else if (i == surveyDistances.Count - 1) //last one
                        {
                            if (dblLength < dblToDepths.Max())
                                dblLength = dblToDepths.Max();

                            upperLimit = dblLength;
                            lowerLimit = surveyDistances.Max();
                            getFromRange = dblFromDepths.Where(d => d >= lowerLimit).ToList();
                            getToRange = dblToDepths.Where(d => d <= upperLimit).Where(d => d > lowerLimit).ToList();

                            surveyCoordinate = await CalculateSurveys.ReturnCoordinateTangential(dblSurvX1, dblSurvY1, dblSurvZ1, dblLength - surveyDistances[i], dblAzimuths[i], dblDips[i]); //straight line segments between points

                            List<Coordinate3D> intervalCoordinate = await DesurveyMethods.DownholeIntervalCoordinate(dblSurvX1, dblSurvY1, dblSurvZ1, surveyCoordinate.x, surveyCoordinate.y, surveyCoordinate.z,
                                getToRange, getFromRange, dblLength - surveyDistances[i]);

                        }
                        else
                        {
                            lowerLimit = dblFromDepths.Where(d => d >= surveyDistances[i]).FirstOrDefault();
                            upperLimit = dblToDepths.Where(d => d >= surveyDistances[i + 1]).FirstOrDefault();

                            getFromRange = dblFromDepths.Where(d => d >= lowerLimit).Where(d => d < upperLimit).ToList();
                            getToRange = dblToDepths.Where(d => d <= upperLimit).Where(d => d > lowerLimit).ToList();

                            surveyCoordinate = await CalculateSurveys.ReturnCoordinateTangential(dblSurvX1, dblSurvY1, dblSurvZ1, surveyDistances[i + 1] - surveyDistances[i], dblAzimuths[i], dblDips[i]);

                            List<Coordinate3D> intervalCoordinate = await DesurveyMethods.DownholeIntervalCoordinate(dblSurvX1, dblSurvY1, dblSurvZ1, surveyCoordinate.x, surveyCoordinate.y, surveyCoordinate.z,
                                getToRange, getFromRange, surveyDistances[i + 1] - surveyDistances[i]);

                        }

                        //TODO - point along line.
                        dblSurvX1 = surveyCoordinate.x;
                        dblSurvY1 = surveyCoordinate.y;
                        dblSurvZ1 = surveyCoordinate.z;


                    }


                }

            }

            return assayDesurveyDto;

        }

        /// <summary>
        /// Desurvey assay table using dip and azimuth from collar table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="assayTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<AssayDesurveyDto> AssayCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            AssayDesurveyDto assayDesurveyDto = await DesurveyMethods.AssayCollarSurveyTrace(collarTableFields, assayTableFields, drillholeValues, bToe, bCollar, DrillholeDesurveyEnum.Tangential);

            return assayDesurveyDto;
        }

        /// <summary>
        /// Creates vertical intervals for each assay record
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="assayTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<AssayDesurveyDto> AssayVerticalTrace(ImportTableFields collarTableFields, ImportTableFields assayTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            AssayDesurveyDto assayDesurveyDto = await DesurveyMethods.AssayVerticalTrace(collarTableFields, assayTableFields, drillholeValues, bToe, bCollar, DrillholeDesurveyEnum.Tangential);

            return assayDesurveyDto;
        }
        #endregion

        #region Collar
        /// <summary>
        /// Create two points per collar
        /// </summary>
        /// <param name="tableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <returns></returns>
        public override async Task<CollarDesurveyDto> CollarVerticalTrace(ImportTableFields tableFields, List<XElement> drillholeValues)
        {
            CollarDesurveyDto collarDesurveyDto = await DesurveyMethods.CollarVerticalTrace(tableFields, drillholeValues, DrillholeDesurveyEnum.Tangential);

            return collarDesurveyDto;
        }

        /// <summary>
        /// Creates collar and toe based on azimuth and dip
        /// </summary>
        /// <param name="tableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <returns></returns>
        public override async Task<CollarDesurveyDto> CollarSurveyTrace(ImportTableFields tableFields, List<XElement> drillholeValues)
        {
            CollarDesurveyDto collarDesurveyDto = await DesurveyMethods.CollarSurveyTrace(tableFields, drillholeValues, DrillholeDesurveyEnum.Tangential);

            return collarDesurveyDto;
        }
        #endregion

        #region Continuous

        /// <summary>
        /// Create a desurveyed continuous table (i.e. distance to field) using dip and azi from collar table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="continuousTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<ContinuousDesurveyDto> ContinuousCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues,
            bool bToe, bool bCollar, bool bBottom)
        {
            ContinuousDesurveyDto continuousDesurveyDto = await DesurveyMethods.ContinuousCollarSurveyTrace(collarTableFields, continuousTableFields, drillholeValues, bToe,
                bCollar, DrillholeDesurveyEnum.Tangential, bBottom);

            return continuousDesurveyDto;
        }

        /// <summary>
        /// Desurvey continuous table using downhole surveys from survey table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="continuousTableFields"></param>
        /// <param name="surveyTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<ContinuousDesurveyDto> ContinuousDownholeTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues,
            bool bToe, bool bCollar, bool bBottom)
        {
            ContinuousDesurveyDto continuousDesurveyDto = await DesurveyMethods.ContinuousDownholeTrace(collarTableFields, continuousTableFields, surveyTableFields, drillholeValues, bToe,
                bCollar, bBottom, DrillholeDesurveyEnum.Tangential);

            return continuousDesurveyDto;
        }

        /// <summary>
        /// Create a vertical desurvey for continuous table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="continuousTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<ContinuousDesurveyDto> ContinuousVerticalTrace(ImportTableFields collarTableFields, ImportTableFields continuousTableFields, List<XElement> drillholeValues,
            bool bToe, bool bCollar)
        {
            ContinuousDesurveyDto continuousDesurveyDto = await DesurveyMethods.ContinuousVerticalTrace(collarTableFields, continuousTableFields, drillholeValues, bToe, bCollar,
                DrillholeDesurveyEnum.Tangential, true);

            return continuousDesurveyDto;
        }
        #endregion

        #region Interval

        /// <summary>
        /// Desurvey interval table using azimuth and dip from collar table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="intervalTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<IntervalDesurveyDto> IntervalCollarSurveyTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            IntervalDesurveyDto intervalDesurveyDto = await DesurveyMethods.IntervalCollarSurveyTrace(collarTableFields, intervalTableFields, drillholeValues, bToe, bCollar, DrillholeDesurveyEnum.Tangential);
            return intervalDesurveyDto;
        }

        /// <summary>
        /// Desurvey interval table using dowhole surveys from survey table
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="intervalTableFields"></param>
        /// <param name="surveyTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<IntervalDesurveyDto> IntervalDownholeTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            IntervalDesurveyDto intervalDesurveyDto = await DesurveyMethods.IntervalDownholeTrace(collarTableFields, intervalTableFields, surveyTableFields, drillholeValues,
                bToe, bCollar, DrillholeDesurveyEnum.Tangential);

            return intervalDesurveyDto;
        }

        /// <summary>
        /// Create a vertical trace for each interval
        /// </summary>
        /// <param name="collarTableFields"></param>
        /// <param name="intervalTableFields"></param>
        /// <param name="drillholeValues"></param>
        /// <param name="bToe"></param>
        /// <param name="bCollar"></param>
        /// <returns></returns>
        public override async Task<IntervalDesurveyDto> IntervalVerticalTrace(ImportTableFields collarTableFields, ImportTableFields intervalTableFields, List<XElement> drillholeValues, bool bToe, bool bCollar)
        {
            IntervalDesurveyDto intervalDesurveyDto = await DesurveyMethods.IntervalVerticalTrace(collarTableFields, intervalTableFields, drillholeValues, bToe, bCollar, DrillholeDesurveyEnum.Tangential);

            return intervalDesurveyDto;
        }
        #endregion

        public override async Task<SurveyDesurveyDto> SurveyDownhole(ImportTableFields collarTableFields, ImportTableFields surveyTableFields, List<XElement> drillholeValues)
        {
            //SurveyDesurveyDto surveyDesurveyDto = await DesurveyMethods.SurveyDownholeUpdate(collarTableFields, surveyTableFields, drillholeValues, DrillholeDesurveyEnum.Tangential);
            SurveyDesurveyDto surveyDesurveyDto = new SurveyDesurveyDto()
            {
                desurveyType = DrillholeDesurveyEnum.Tangential,
                surveyType = DrillholeSurveyType.downholesurvey
            };

            #region fieldnames
            //Need holeID, x, y, z, length => reference name in xml
            var collarHoleID = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            var xField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.xName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            var yField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.yName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            var zField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.zName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            var tdField = collarTableFields.Where(f => f.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();

            //TODO - add assay fields 
            var surveyHoleID = surveyTableFields.Where(f => f.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            var distField = surveyTableFields.Where(f => f.columnImportName == DrillholeConstants.distName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            var dipField = surveyTableFields.Where(f => f.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            var azimuthField = surveyTableFields.Where(f => f.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();

            #endregion

            #region Input Data
            XElement elements = drillholeValues[0];
            var collarElements = elements.Elements();

            XElement survElements = drillholeValues[1];
            var surveyElements = survElements.Elements();
            #endregion

            //return collar coordiantes and length for all holes which are not flagged to be ignored
            var showElements = collarElements.Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE");

            foreach (XElement element in showElements)
            {
                //get the values from XML
                string holeAttr = element.Attribute("ID").Value;
                string hole = element.Element(collarHoleID).Value;
                string xCoord = element.Element(xField).Value;
                string yCoord = element.Element(yField).Value;
                string zCoord = element.Element(zField).Value;
                string totalDepth = element.Element(tdField).Value; //FOR NOW< IGNORE

                int survCount = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Count(); //check collar exists in assay table

                List<double> dblDips = new List<double>();
                List<double> dblAzimuths = new List<double>();
                List<double> surveyDistances = new List<double>();
                List<int> surveyIDs = new List<int>();

                double dblX = 0.0, dblY = 0.0, dblZ = 0.0, dblLength = 0.0;

                if (survCount == 0)
                {
                    dblDips.Add(-90.0);
                    dblAzimuths.Add(0.0);
                    surveyDistances.Add(0.0);
                    surveyIDs.Add(-99);
                }
                else
                {
                    
                    dblX = Convert.ToDouble(xCoord);
                    dblY = Convert.ToDouble(yCoord);
                    dblZ = Convert.ToDouble(zCoord);
                    dblLength = Convert.ToDouble(totalDepth);

                    var dips = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Element(dipField).Value).ToList();
                    var azimuths = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Element(azimuthField).Value).ToList();
                    var distances = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Element(distField).Value).ToList();
                    var surveyAttrs = surveyElements.Where(h => h.Element(surveyHoleID).Value == hole).Where(a => a.Attribute("Ignore").Value.ToUpper() == "FALSE").Select(d => d.Attribute("ID").Value).ToList();

                    for (int d = 0; d < surveyAttrs.Count; d++)
                    {
                        if (Information.IsNumeric(dips[d]))
                            dblDips.Add(Convert.ToDouble(dips[d]));
                        else
                            dblDips.Add(-90.0);
                        if (Information.IsNumeric(azimuths[d]))
                            dblAzimuths.Add(Convert.ToDouble(azimuths[d]));
                        else
                            dblAzimuths.Add(0.0);
                        if (Information.IsNumeric(distances[d]))
                            surveyDistances.Add(Convert.ToDouble(distances[d]));
                        else
                            surveyDistances.Add(-99.0);
                        if (Information.IsNumeric(surveyAttrs[d]))
                            surveyIDs.Add(Convert.ToInt16(surveyAttrs[d]));
                        else
                            surveyIDs.Add(-99);
                    }
                }

                for (int i = 0; i < surveyDistances.Count; i++)
                {

                    Coordinate3D coordinate = new Coordinate3D();

                    if (i == 0)
                    {
                      
                        await DesurveyMethods.PopulateNewDesurveySurvey(dblX, dblY, dblZ, -99, -99, Convert.ToInt16(holeAttr), surveyIDs[i],
                            hole, surveyDesurveyDto, false, -99); //Collar
                        
                        coordinate = await CalculateSurveys.ReturnCoordinateTangential(dblX, dblY, dblZ, surveyDistances[1], dblAzimuths[0], dblDips[0]);

                        await DesurveyMethods.PopulateNewDesurveySurvey(coordinate.x, coordinate.y, coordinate.z, dblAzimuths[i], dblDips[i], Convert.ToInt16(holeAttr), surveyIDs[i],
                            hole, surveyDesurveyDto, true, surveyDistances[i+1]);

                    }
                    else if (i == surveyDistances.Count - 1)
                    {
                        if (dblLength > surveyDistances.Max())
                        {

                            coordinate = await CalculateSurveys.ReturnCoordinateTangential(dblX, dblY, dblZ, dblLength - surveyDistances[i], dblAzimuths[i], dblDips[i]);

                            await DesurveyMethods.PopulateNewDesurveySurvey(coordinate.x, coordinate.y, coordinate.z, dblAzimuths[i], dblDips[i], Convert.ToInt16(holeAttr), surveyIDs[i],
                                hole, surveyDesurveyDto, true, dblLength);
                        }
                    }

                    else
                    {
                        coordinate = await CalculateSurveys.ReturnCoordinateTangential(dblX, dblY, dblZ, surveyDistances[i + 1] - surveyDistances[i], dblAzimuths[i], dblDips[i]);

                        await DesurveyMethods.PopulateNewDesurveySurvey(coordinate.x, coordinate.y, coordinate.z, dblAzimuths[i], dblDips[i], Convert.ToInt16(holeAttr), surveyIDs[i],
                                hole, surveyDesurveyDto, true, surveyDistances[i + 1]);


                    }

                    dblX = coordinate.x;
                    dblY = coordinate.y;
                    dblZ = coordinate.z;

                }


            }

            return surveyDesurveyDto;

        }

    }


}
