using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Windows
{
    public static class XmlDefaultPath
    {
        public static string GetFullPathAndFilename(string _rootName)
        {
            string rootName = _rootName;
            string fullPathAndName = "";

            string xmlPath = Environment.GetEnvironmentVariable("home");

            if (xmlPath == null)
                xmlPath = Environment.GetEnvironmentVariable("temp");

            xmlPath = xmlPath = xmlPath + "\\";
            string searchName = "_" + rootName + ".xml";

            //Order by XML type abd date, with most recent at top
            var xmlFiles = new DirectoryInfo(xmlPath).GetFiles().Where(x => x.Extension == ".xml").OrderBy(t => t.CreationTime).Reverse();

            if (xmlFiles == null)
                return null;

            foreach (var file in xmlFiles)
            {
                string name = file.Name; //get the name of file

                int length = searchName.Length;  //length of seachName

                if (length < name.Length) //filename has to be longer than searchname
                {

                    if (name.Contains(searchName)) //if name contains searchName then load XML file
                    {
                        fullPathAndName = file.FullName;
                        break;
                    }
                    else
                    {
                        fullPathAndName = xmlPath + DateTime.Now.ToFileTime().ToString() + searchName;
                    }
                }
            }

            return fullPathAndName;
        }
    }
}
