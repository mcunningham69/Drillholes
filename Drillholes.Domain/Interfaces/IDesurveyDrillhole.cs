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
        Task<CollarDesurveyDto> VerticalHole(DrillholeDesurveyEnum desurveyType, CollarTableDto collarTableDto, bool bToe);

        Task<CollarDesurveyDto> CollarSurveyHole(DrillholeDesurveyEnum desurveyType, CollarTableDto collarTableDto, bool bToe);
        Task<CollarDesurveyDto> DownholeSurveyHole(DrillholeDesurveyEnum desurveyType, CollarTableDto collarTableDto, bool bToe, XElement surveyValues);

        Task<SurveyDesurveyDto> DownholeSurveyHole(DrillholeDesurveyEnum desurveyType, SurveyTableDto surveyTableDto, bool bToe, XElement collarValues);
        Task<AssayDesurveyDto> VerticalHole(DrillholeDesurveyEnum desurveyType, AssayTableDto assayTableDto, bool bToe, XElement collarValues);

        Task<AssayDesurveyDto> CollarSurveyHole(DrillholeDesurveyEnum desurveyType, AssayTableDto assayTableDto, bool bToe, XElement collarValues);
        Task<AssayDesurveyDto> DownholeSurveyHole(DrillholeDesurveyEnum desurveyType, AssayTableDto assayTableDto, bool bToe, List<XElement> drillholeVales);
        Task<IntervalDesurveyDto> VerticalHole(DrillholeDesurveyEnum desurveyType, IntervalTableDto intervalTableDto, bool bToe, XElement collarValues);

        Task<IntervalDesurveyDto> CollarSurveyHole(DrillholeDesurveyEnum desurveyType, IntervalTableDto intervalTableDto, bool bToe, XElement collarValues);
        Task<IntervalDesurveyDto> DownholeSurveyHole(DrillholeDesurveyEnum desurveyType, IntervalTableDto intervalTableDto, bool bToe, List<XElement> drillholeVales);
    }
}
