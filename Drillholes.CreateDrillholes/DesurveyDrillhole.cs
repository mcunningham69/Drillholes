using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.Enum;


namespace Drillholes.CreateDrillholes
{
    public class DesurveyDrillhole
    {

        public DesurveyDrillhole(DrillholeDesurveyEnum value)
        {
            SetDrillholeType(value);
        }
        public void SetDrillholeType(DrillholeDesurveyEnum value)
        {
            DrillholeDesurveyValues _desurvey = DrillholeDesurveyValues.createDesurvey(value);
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
                            return new BalancedTangential();
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
        }

        public class AverageAngle : DrillholeDesurveyValues
        {

        }

        public class BalancedTangential : DrillholeDesurveyValues
        {

        }

        public class MinimumCurvature : DrillholeDesurveyValues
        {

        }

        public class RadiusCurvature : DrillholeDesurveyValues
        {

        }

        public class Tangential : DrillholeDesurveyValues
        {

        }
    }
}
