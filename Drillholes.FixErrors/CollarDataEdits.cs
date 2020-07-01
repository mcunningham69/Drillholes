using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Drillholes.Domain;
using Drillholes.Domain.DTO;
using Drillholes.Domain.Interfaces;

namespace Drillholes.FixErrors
{
    public class CollarDataEdits : ICollarEdit
    {
        private CollarTableDto collarTableDto;

        public async Task<CollarTableDto> UpdateValues(List<RowsToEdit> RowsToEdit, XElement collarValues, List<ImportTableField> editFields, bool bIgnore)
        {
            collarTableDto = new CollarTableDto();
            collarTableDto.xPreview = collarValues; //update XML

            //get fieldnames
            string holeIDName = editFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            string xName = editFields.Where(o => o.columnImportName == DrillholeConstants.xName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            string yName = editFields.Where(o => o.columnImportName == DrillholeConstants.yName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            string zName = editFields.Where(o => o.columnImportName == DrillholeConstants.zName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            string maxName = editFields.Where(o => o.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            string aziName = editFields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            string dipName = editFields.Where(o => o.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();

            //get elments in XML
            var collarElements = collarValues.Elements();

            //Loop through each row of data using the ID field
            foreach (var row in RowsToEdit)
            {
                int record = row.id_col;

                var updateQuery = from r in collarElements
                                  where r.Attribute("ID").Value == record.ToString()
                                  select r;

                //update the XML
                foreach(var query in updateQuery)
                {
                    query.Attribute("Ignore").SetValue(bIgnore.ToString());
                    query.Element(holeIDName).SetValue(row.holeid);
                    query.Element(xName).SetValue(row.x);
                    query.Element(yName).SetValue(row.y);
                    query.Element(zName).SetValue(row.z);
                    query.Element(maxName).SetValue(row.maxDepth);

                    //check whether or not to update azimuth and dip fields (Collar Survey option)
                    if (aziName != null && dipName != null)
                    {
                        query.Element(aziName).SetValue(row.azimuth);
                        query.Element(dipName).SetValue(row.dip);
                    }

                }

            }

            return collarTableDto;
        }
    }
}
