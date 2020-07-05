using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Enum;
using Drillholes.Domain.Interfaces;

namespace Drillholes.CreateDrillholes
{
    public class CreateHolesByType : IDesurveyDrillhole
    {
        public Task<CollarDesurveyDto> CollarSurveyHole(DrillholeDesurveyEnum desurveyType, CollarTableDto collarTableDto, bool bToe)
        {
            throw new NotImplementedException();
        }

        public Task<AssayDesurveyDto> CollarSurveyHole(DrillholeDesurveyEnum desurveyType, AssayTableDto assayTableDto, bool bToe, XElement collarValues)
        {
            throw new NotImplementedException();
        }

        public Task<IntervalDesurveyDto> CollarSurveyHole(DrillholeDesurveyEnum desurveyType, IntervalTableDto intervalTableDto, bool bToe, XElement collarValues)
        {
            throw new NotImplementedException();
        }

        public Task<CollarDesurveyDto> DownholeSurveyHole(DrillholeDesurveyEnum desurveyType, CollarTableDto collarTableDto, bool bToe, XElement surveyValues)
        {
            throw new NotImplementedException();
        }

        public Task<SurveyDesurveyDto> DownholeSurveyHole(DrillholeDesurveyEnum desurveyType, SurveyTableDto surveyTableDto, bool bToe, XElement collarValues)
        {
            throw new NotImplementedException();
        }

        public Task<AssayDesurveyDto> DownholeSurveyHole(DrillholeDesurveyEnum desurveyType, AssayTableDto assayTableDto, bool bToe, List<XElement> drillholeVales)
        {
            throw new NotImplementedException();
        }

        public Task<IntervalDesurveyDto> DownholeSurveyHole(DrillholeDesurveyEnum desurveyType, IntervalTableDto intervalTableDto, bool bToe, List<XElement> drillholeVales)
        {
            throw new NotImplementedException();
        }

        public Task<CollarDesurveyDto> VerticalHole(DrillholeDesurveyEnum desurveyType, CollarTableDto collarTableDto, bool bToe)
        {
            throw new NotImplementedException();
        }

        public Task<AssayDesurveyDto> VerticalHole(DrillholeDesurveyEnum desurveyType, AssayTableDto assayTableDto, bool bToe, XElement collarValues)
        {
            throw new NotImplementedException();
        }

        public Task<IntervalDesurveyDto> VerticalHole(DrillholeDesurveyEnum desurveyType, IntervalTableDto intervalTableDto, bool bToe, XElement collarValues)
        {
            throw new NotImplementedException();
        }
    }
}
