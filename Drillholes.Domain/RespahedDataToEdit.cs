using Drillholes.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain
{
    public class ReshapedDataToEdit 
    {
        public bool Ignore { get; set; }
        public string ErrorType { get; set; }
        public List<GroupByHoles> GroupedHoles { get; set; }

        public int Count { get; set; }

        public ReshapedDataToEdit()
        {
            Ignore = false;
            ErrorType = "";
            GroupedHoles = new List<GroupByHoles>();
        }
    }

    public class GroupByHoles
    {
        public bool Ignore { get; set; }
        public string holeid { get; set; }
        public List<GroupByTable> GroupedTables { get; set; }
    }

    public class GroupByTable
    {
        public bool Ignore { get; set; }
        public string TableType { get; set; }
        public List<GroupByTest> GroupedTests { get; set; }
    }

    public class GroupByTest
    {
        public bool Ignore { get; set; }
        public string MainTest { get; set; }

        public List<GroupByTestField> TestFields { get; set; }
    }

    public class GroupByTestField
    {
        public string TestField { get; set; }

        public bool Ignore { get; set; }

        public List<RowsToEdit> TableData { get; set; }
    }

    public class RowsToEdit
    {
        public int id_col { get; set; }
        public int id_sur { get; set; }
        public int id_ass { get; set; }
        public int id_int { get; set; }

        private string _holeid = string.Empty;
        public string holeid
        {
            get
            {
                return _holeid;
            }
            set
            {
                _holeid = value;
            }
        }

        private string _x = string.Empty;
        public string x
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        private string _y = string.Empty;
        public string y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }

        private string _maxDepth = string.Empty;
        public string maxDepth
        {
            get
            {
                return _maxDepth;
            }
            set
            {
                _maxDepth = value;
            }
        }

        private string _z = string.Empty;
        public string z
        {
            get
            {
                return _z;
            }
            set
            {
                _z = value;
            }
        }

        private string _dip = string.Empty;
        public string dip
        {
            get
            {
                return _dip;
            }
            set
            {
                _dip = value;
            }
        }

        private string _azimuth = string.Empty;
        public string azimuth
        {
            get
            {
                return _azimuth;
            }
            set
            {
                _azimuth = value;
            }
        }

        private string _distance = string.Empty;
        public string distance
        {
            get
            {
                return _distance;
            }
            set
            {
                _distance = value;
            }
        }
        private string _distanceFrom = string.Empty;
        public string distanceFrom
        {
            get
            {
                return _distanceFrom;
            }
            set
            {
                _distanceFrom = value;
            }
        }
        private string _distanceTo = string.Empty;
        public string distanceTo
        {
            get
            {
                return _distanceTo;
            }
            set
            {
                _distanceTo = value;
            }
        }

        public bool Ignore { get; set; }
      
        public string Description { get; set; } //bind as tooltip
        public string testType { get; set; }
        public string validationTest { get; set; }
        public DrillholeMessageStatus ErrorType { get; set; }
        public DrillholeTableType TableType { get; set; }
    }
}
