using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.Domain.DataObject
{
    public class SummaryCollarStatistics : SummaryStatistics
    {

        public int collarCount { get; set; }
        public double MinimumLength { get; set; }
        public double MaximumLength { get; set; }
        public double AverageLength { get; set; }
        public double TotalLength { get; set; }
        public double MinimumX { get; set; }
        public double MinimumY { get; set; }
        public double MinimumZ { get; set; }
        public double MaximumX { get; set; }
        public double MaximumY { get; set; }
        public double MaximumZ { get; set; }
        public string MinimumDip { get; set; }
        public string AverageDip { get; set; }
        public string MaximumDip { get; set; }
        public string MinimumAzi { get; set; }
        public string AverageAzi { get; set; }
        public string MaximumAzi { get; set; }

        public double ExtentX { get; set; }
        public double ExtentY { get; set; }
        public double ExtentZ { get; set; }
        public double VolumeOfExtent { get; set; }
        public double AreaOfExtent { get; set; }

        public SummaryCollarStatistics()
        {
            collarCount = 0;
            MinimumLength = 0.0;
            MaximumLength = 0.0;
            AverageLength = 0.0;
            TotalLength = 0.0;
            MinimumX = 0.0;
            MinimumY = 0.0;
            MinimumZ = 0.0;
            MaximumX = 0.0;
            MaximumY = 0.0;
            MaximumZ = 0.0;
            ExtentX = 0.0;
            ExtentY = 0.0;
            ExtentZ = 0.0;
            VolumeOfExtent = 0.0;
            AreaOfExtent = 0.0;

            isValid = true;
        }

        public virtual void CalculateArea()
        {
            if (ExtentX != 0)
            {
                if (ExtentY != 0)
                {
                    AreaOfExtent = Math.Round(ExtentX * ExtentY, 1);
                }
                if (ExtentZ != 0)
                    VolumeOfExtent = Math.Round(AreaOfExtent * ExtentZ, 1);
            }
        }


    }

}
