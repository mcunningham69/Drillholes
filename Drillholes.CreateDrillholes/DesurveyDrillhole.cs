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
                AssayDesurveyDto assayDesurveyDto = await DesurveyMethods.AssayDownholeTrace(collarTableFields, assayTableFields, surveyTableFields, drillholeValues,
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
                SurveyDesurveyDto surveyDesurveyDto = await DesurveyMethods.SurveyDownhole(collarTableFields, surveyTableFields, drillholeValues, DrillholeDesurveyEnum.AverageAngle);

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
                AssayDesurveyDto assayDesurveyDto = await DesurveyMethods.AssayDownholeTrace(collarTableFields, assayTableFields, surveyTableFields, drillholeValues,
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
                SurveyDesurveyDto surveyDesurveyDto = await DesurveyMethods.SurveyDownhole(collarTableFields, surveyTableFields, drillholeValues, DrillholeDesurveyEnum.BalancedTangential);

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
                AssayDesurveyDto assayDesurveyDto = await DesurveyMethods.AssayDownholeTrace(collarTableFields, assayTableFields, surveyTableFields, drillholeValues,
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
                SurveyDesurveyDto surveyDesurveyDto = await DesurveyMethods.SurveyDownhole(collarTableFields, surveyTableFields, drillholeValues, DrillholeDesurveyEnum.MinimumCurvature);

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
                AssayDesurveyDto assayDesurveyDto = await DesurveyMethods.AssayDownholeTrace(collarTableFields, assayTableFields, surveyTableFields, drillholeValues,
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
                SurveyDesurveyDto surveyDesurveyDto = await DesurveyMethods.SurveyDownhole(collarTableFields, surveyTableFields, drillholeValues, DrillholeDesurveyEnum.RadiusCurvature);

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
                AssayDesurveyDto assayDesurveyDto = await DesurveyMethods.AssayDownholeTrace(collarTableFields, assayTableFields, surveyTableFields, drillholeValues, 
                    bToe, bCollar, DrillholeDesurveyEnum.Tangential);
               
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
                ContinuousDesurveyDto continuousDesurveyDto = await DesurveyMethods.ContinuousCollarSurveyTrace(collarTableFields,continuousTableFields,  drillholeValues, bToe, 
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
                ContinuousDesurveyDto continuousDesurveyDto = await DesurveyMethods.ContinuousVerticalTrace(collarTableFields, continuousTableFields,drillholeValues, bToe, bCollar, 
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
                SurveyDesurveyDto surveyDesurveyDto = await DesurveyMethods.SurveyDownhole(collarTableFields, surveyTableFields, drillholeValues, DrillholeDesurveyEnum.Tangential); 
                
                return surveyDesurveyDto;
            }

        }
    }


}
