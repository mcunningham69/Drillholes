using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.Interfaces;
using Drillholes.Domain.DTO;
using Drillholes.Domain.DataObject;
using Drillholes.Domain.Exceptions;
using Drillholes.Domain.Enum;
using AutoMapper;
using System.Xml.Linq;

namespace Drillholes.Domain.Services
{
    public class DesurveyServices
    {
        private readonly IDesurveyDrillhole _drillhole;


        public DesurveyServices(IDesurveyDrillhole drillhole)
        {
            this._drillhole = drillhole;
        }

        public async Task<CollarDesurveyObject> VerticalHole(IMapper mapper, DrillholeDesurveyEnum desurveyType, CollarTableDto collarTableDto, bool bToe)
        {
            var desurvDto = await _drillhole.VerticalHole(desurveyType, collarTableDto, bToe) as CollarDesurveyDto;

            if (desurvDto.IsValid == false)
            {
                throw new CollarException("Issue with desurvey vertical collar data");
            }

            return mapper.Map<CollarDesurveyDto, CollarDesurveyObject>(desurvDto);
        }

        public async Task<CollarDesurveyObject> CollarSurveyHole(IMapper mapper, DrillholeDesurveyEnum desurveyType, CollarTableDto collarTableDto, bool bToe)
        {
            var desurvDto = await _drillhole.CollarSurveyHole(desurveyType, collarTableDto, bToe) as CollarDesurveyDto;

            if (desurvDto.IsValid == false)
            {
                throw new CollarException("Issue with desurvey collar data");
            }

            return mapper.Map<CollarDesurveyDto, CollarDesurveyObject>(desurvDto);
        }

        public async Task<CollarDesurveyObject> DownholeSurveyHole(IMapper mapper, DrillholeDesurveyEnum desurveyType, CollarTableDto collarTableDto, bool bToe, XElement surveyValues)
        {
            var desurvDto = await _drillhole.DownholeSurveyHole(desurveyType, collarTableDto, bToe, surveyValues) as CollarDesurveyDto;

            if (desurvDto.IsValid == false)
            {
                throw new CollarException("Issue with desurvey downhole collar data");
            }

            return mapper.Map<CollarDesurveyDto, CollarDesurveyObject>(desurvDto);
        }

        public async Task<SurveyDesurveyObject> DownholeSurveyHole(IMapper mapper, DrillholeDesurveyEnum desurveyType, SurveyTableDto surveyTableDto, bool bToe, XElement collarValues)
        {
            var desurvDto = await _drillhole.DownholeSurveyHole(desurveyType, surveyTableDto, bToe, collarValues) as SurveyDesurveyDto;

            if (desurvDto.IsValid == false)
            {
                throw new SurveyException("Issue with desurvey downhole survey data");
            }

            return mapper.Map<SurveyDesurveyDto, SurveyDesurveyObject>(desurvDto);


        }

        public async Task<AssayDesurveyObject> VerticalHole(IMapper mapper, DrillholeDesurveyEnum desurveyType, AssayTableDto assayTableDto, bool bToe, XElement collarValues)
        {
            var desurvDto = await _drillhole.VerticalHole(desurveyType, assayTableDto, bToe, collarValues) as AssayDesurveyDto;

            if (desurvDto.IsValid == false)
            {
                throw new AssayException("Issue with desurvey vertical assay data");
            }

            return mapper.Map<AssayDesurveyDto, AssayDesurveyObject>(desurvDto);
        }

        public async Task<AssayDesurveyObject> CollarSurveyHole(IMapper mapper, DrillholeDesurveyEnum desurveyType, AssayTableDto assayTableDto, bool bToe, XElement collarValues)
        {
            var desurvDto = await _drillhole.CollarSurveyHole(desurveyType, assayTableDto, bToe, collarValues) as AssayDesurveyDto;

            if (desurvDto.IsValid == false)
            {
                throw new AssayException("Issue with desurvey assay data");
            }

            return mapper.Map<AssayDesurveyDto, AssayDesurveyObject>(desurvDto);
        }

        public async Task<AssayDesurveyObject> DownholeSurveyHole(IMapper mapper, DrillholeDesurveyEnum desurveyType, AssayTableDto assayTableDto, bool bToe, List<XElement> drillholeValues)
        {
            var desurvDto = await _drillhole.DownholeSurveyHole(desurveyType, assayTableDto, bToe, drillholeValues) as AssayDesurveyDto;

            if (desurvDto.IsValid == false)
            {
                throw new AssayException("Issue with desurvey downhole assay data");
            }

            return mapper.Map<AssayDesurveyDto, AssayDesurveyObject>(desurvDto);
        }

        public async Task<IntervalDesurveyObject> VerticalHole(IMapper mapper, DrillholeDesurveyEnum desurveyType, IntervalTableDto intervalTableDto, bool bToe, XElement collarValues)
        {
            var desurvDto = await _drillhole.VerticalHole(desurveyType, intervalTableDto, bToe, collarValues) as IntervalDesurveyDto;

            if (desurvDto.IsValid == false)
            {
                throw new IntervalException("Issue with desurvey interval assay data");
            }

            return mapper.Map<IntervalDesurveyDto, IntervalDesurveyObject>(desurvDto);
        }

        public async Task<IntervalDesurveyObject> CollarSurveyHole(IMapper mapper, DrillholeDesurveyEnum desurveyType, IntervalTableDto intervalTableDto, bool bToe, XElement collarValues)
        {
            var desurvDto = await _drillhole.CollarSurveyHole(desurveyType, intervalTableDto, bToe, collarValues) as IntervalDesurveyDto;

            if (desurvDto.IsValid == false)
            {
                throw new IntervalException("Issue with desurvey interval data");
            }

            return mapper.Map<IntervalDesurveyDto, IntervalDesurveyObject>(desurvDto);
        }

        public async Task<IntervalDesurveyObject> DownholeSurveyHole(IMapper mapper, DrillholeDesurveyEnum desurveyType, IntervalTableDto intervalTableDto, bool bToe, List<XElement> drillholeValues)
        {
            var desurvDto = await _drillhole.DownholeSurveyHole(desurveyType, intervalTableDto, bToe, drillholeValues) as IntervalDesurveyDto;

            if (desurvDto.IsValid == false)
            {
                throw new IntervalException("Issue with desurvey downhole interval data");
            }

            return mapper.Map<IntervalDesurveyDto, IntervalDesurveyObject>(desurvDto);
        }
    }
}
