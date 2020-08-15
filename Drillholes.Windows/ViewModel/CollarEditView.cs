﻿using Drillholes.Domain.Interfaces;
using Drillholes.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Drillholes.Domain.Enum;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Controls;
using Drillholes.Domain;
using System.Xml.Linq;
using Drillholes.FixErrors;
using Drillholes.Domain.DTO;
using Drillholes.Domain.DataObject;

namespace Drillholes.Windows.ViewModel
{
    public class CollarEditView
    {
        public CollarEditServices _editService;
        public ICollarEdit _editValues;

        public XmlService _xmlService;
        public IDrillholeXML _xml;


        public virtual DrillholeSurveyType surveyType { get; set; }

        public ImportTableFields importCollarFields { get; set; }
        public XElement xmlCollarData { get; set; }

        public IMapper mapper = null;

        public string fullPathnameData { get; set; }
        public string rootNameData = "DrillholeData";
        public bool savedSession { get; set; }
        public string sessionName { get; set; }
        public string projectLocation { get; set; }

        public CollarEditView(DrillholeSurveyType _surveyType, XElement _xmlCollarData, ImportTableFields _collarFields)
        {
            surveyType = _surveyType;
            xmlCollarData = _xmlCollarData;
            importCollarFields = _collarFields;

            _editValues = new CollarDataEdits();

            _editService = new CollarEditServices(_editValues);
        }

        public CollarEditView(DrillholeSurveyType _surveyType, XElement _xmlCollarData, ImportTableFields _collarFields, DrillholeTableType _tableType, 
            bool _savedSession, string _sessionName, string _projectLocation)
        {
            surveyType = _surveyType;
            xmlCollarData = _xmlCollarData;
            importCollarFields = _collarFields;

            _editValues = new CollarDataEdits();

            _editService = new CollarEditServices(_editValues);

            savedSession = _savedSession;
            sessionName = _sessionName;
            projectLocation = _projectLocation;

            XmlSetUP(_tableType.ToString());
        }


        public async void XmlSetUP(string tableType)
        {
            //create XML temp table
            if (_xml == null)
                _xml = new Drillholes.XML.XmlController();

            if (_xmlService == null)
                _xmlService = new XmlService(_xml);

            if (!savedSession)
            {
                fullPathnameData = await XmlDefaultPath.GetFullPathAndFilename(rootNameData, tableType);
            }
            else
            {
                fullPathnameData = await XmlDefaultPath.GetProjectPathAndFilename(rootNameData, tableType, sessionName, projectLocation);
            }

        }

        public virtual void InitialiseMapping()
        {
            if (_editValues == null)
                _editValues = new CollarDataEdits();

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<CollarTableDto, CollarTableObject>(); });

            mapper = config.CreateMapper();

            var source = new CollarTableDto();

            var dest = mapper.Map<CollarTableDto, CollarTableObject>(source);

            _editService = new CollarEditServices(_editValues);

        }

        public async Task<XElement> SaveEdits(List<RowsToEdit> rows)
        {
            if (mapper == null)
                InitialiseMapping();

            List<ImportTableField> editFields = new List<ImportTableField>();
            
            ImportTableField holeField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Single();
            ImportTableField xField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.xName).Where(m => m.genericType == false).Single();
            ImportTableField yField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.yName).Where(m => m.genericType == false).Single();
            ImportTableField zField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.zName).Where(m => m.genericType == false).Single();
            ImportTableField maxField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Single();

            editFields.Add(holeField);
            editFields.Add(xField);
            editFields.Add(yField);
            editFields.Add(zField);
            editFields.Add(maxField);

            if (surveyType == DrillholeSurveyType.collarsurvey)
            {
                ImportTableField aziField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false).Single();
                ImportTableField dipField = importCollarFields.Where(o => o.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false).Single();

                editFields.Add(aziField);
                editFields.Add(dipField);
            }

            var _edits = await _editService.UpdateValues(mapper, rows, xmlCollarData, editFields);

            await _xmlService.DrillholeData(fullPathnameData, _edits.xPreview, DrillholeTableType.collar, DrillholeConstants._Collar + "s", rootNameData);

            if (savedSession)
                _xmlService.DrillholeData(projectLocation + "\\" + sessionName + ".dh", fullPathnameData, DrillholeConstants.drillholeProject, DrillholeTableType.collar);


            return _edits.xPreview;
        }


    }
}
