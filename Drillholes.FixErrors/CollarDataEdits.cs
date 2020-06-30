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
            collarTableDto.xPreview = collarValues;

            //get fieldnames
            string holeIDName = editFields.Where(o => o.columnImportName == DrillholeConstants.holeIDName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            string xName = editFields.Where(o => o.columnImportName == DrillholeConstants.xName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            string yName = editFields.Where(o => o.columnImportName == DrillholeConstants.yName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            string zName = editFields.Where(o => o.columnImportName == DrillholeConstants.zName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            string maxName = editFields.Where(o => o.columnImportName == DrillholeConstants.maxName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            string aziName = editFields.Where(o => o.columnImportName == DrillholeConstants.azimuthName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();
            string dipName = editFields.Where(o => o.columnImportName == DrillholeConstants.dipName).Where(m => m.genericType == false).Select(f => f.columnHeader).SingleOrDefault();

            int record = 0;

            //string hole = "";
            //string x = "";
            //string y = "";
            //string z = "";
            //string depth = "";
            //string azimuth = "";
            //string dip = "";
            //string ignore = "";

            var collarElements = collarValues.Elements();

            foreach (var row in RowsToEdit)
            {
                record = row.id_col;

                collarElements.Where(a => a.Attribute("ID").Value == record.ToString()).Select(h => new { V = h.Element(holeIDName).Value = row.holeid});
                collarElements.Where(a => a.Attribute("ID").Value == record.ToString()).Select(h => new { V = h.Element(xName).Value = row.x });
                collarElements.Where(a => a.Attribute("ID").Value == record.ToString()).Select(h => new { V = h.Element(yName).Value = row.y });
                collarElements.Where(a => a.Attribute("ID").Value == record.ToString()).Select(h => new { V = h.Element(zName).Value = row.z });
                collarElements.Where(a => a.Attribute("ID").Value == record.ToString()).Select(h => new { V = h.Element(maxName).Value = row.maxDepth });
                collarElements.Where(a => a.Attribute("ID").Value == record.ToString()).Select(h => new { V = h.Attribute("Ignore").Value = bIgnore.ToString() });

                collarValues.Descendants("Collars").Where(a => a.Attribute("ID").Value == record.ToString()).Select(h => new { V = h.Element(xName).Value = row.x });
                collarValues.Descendants("Collars").Where(a => a.Attribute("ID").Value == record.ToString()).Select(h =>  h.Element(xName).Value = row.x );
                collarValues.Descendants().Where(a => a.Attribute("ID").Value == record.ToString()).Select(h => h.Element(xName).Value = row.x);
                collarElements.Where(a => a.Attribute("ID").Value == record.ToString()).Select(h => h.Element(xName).Value = row.x );



                var updateQuery = from r in collarElements
                                  where r.Attribute("ID").Value == record.ToString()
                                  select r;

                foreach(var query in updateQuery)
                {
                    query.Element(xName).SetValue(row.x);
                }

                //hole = collarElements.Where(a => a.Attribute("ID").Value == record.ToString()).Select(h => h.Element(holeIDName).Value).SingleOrDefault();
                //x = collarElements.Where(a => a.Attribute("ID").Value == record.ToString()).Select(h => h.Element(xName).Value).SingleOrDefault();
                //y = collarElements.Where(a => a.Attribute("ID").Value == record.ToString()).Select(h => h.Element(yName).Value).SingleOrDefault();
                //z = collarElements.Where(a => a.Attribute("ID").Value == record.ToString()).Select(h => h.Element(zName).Value).SingleOrDefault();
                //depth = collarElements.Where(a => a.Attribute("ID").Value == record.ToString()).Select(h => h.Element(maxName).Value).SingleOrDefault();
                //ignore = collarElements.Where(a => a.Attribute("ID").Value == record.ToString()).Select(h => h.Attribute("Ignore").Value).SingleOrDefault();

                //ignore = bIgnore.ToString();
                //hole = row.holeid;
                //x = row.x;
                //y = row.y;
                //z = row.z;
                //depth = row.maxDepth;



                //if (aziName != null && dipName != null)
                //{
                //    //azimuth = collarElements.Where(a => a.Attribute("ID").Value == record.ToString()).Select(h => h.Element(aziName).Value).SingleOrDefault();
                //    //dip = collarElements.Where(a => a.Attribute("ID").Value == record.ToString()).Select(h => h.Element(dipName).Value).SingleOrDefault();

                //    collarElements.Where(a => a.Attribute("ID").Value == record.ToString()).Select(h => new { V = h.Element(aziName).Value = row.azimuth });
                //    collarElements.Where(a => a.Attribute("ID").Value == record.ToString()).Select(h => new { V = h.Element(dipName).Value = row.dip });

                //    //azimuth = row.azimuth;
                //    //dip = row.dip;
                //}
            }

            return collarTableDto;
        }
    }
}
