using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drillholes.Domain.DataObject;

namespace Drillholes.Domain
{
    //public class SummaryCollarStatistics
    //{
    //    public bool isValid { get; set; }
    //    public int collarCount { get; set; }
    //    public double MinimumLength { get; set; }
    //    public double MaximumLength { get; set; }
    //    public double AverageLength { get; set; }
    //    public double TotalLength { get; set; }
    //    public double MinimumX { get; set; }
    //    public double MinimumY { get; set; }
    //    public double MinimumZ { get; set; }
    //    public double MaximumX { get; set; }
    //    public double MaximumY { get; set; }
    //    public double MaximumZ { get; set; }
    //    public string MinimumDip { get; set; }
    //    public string AverageDip { get; set; }
    //    public string MaximumDip { get; set; }
    //    public string MinimumAzi { get; set; }
    //    public string AverageAzi { get; set; }
    //    public string MaximumAzi { get; set; }

    //    public double ExtentX { get; set; }
    //    public double ExtentY { get; set; }
    //    public double ExtentZ { get; set; }
    //    public double VolumeOfExtent { get; set; }
    //    public double AreaOfExtent { get; set; }

    //    public SummaryCollarStatistics()
    //    {
    //        collarCount = 0;
    //        MinimumLength = 0.0;
    //        MaximumLength = 0.0;
    //        AverageLength = 0.0;
    //        TotalLength = 0.0;
    //        MinimumX = 0.0;
    //        MinimumY = 0.0;
    //        MinimumZ = 0.0;
    //        MaximumX = 0.0;
    //        MaximumY = 0.0;
    //        MaximumZ = 0.0;
    //        ExtentX = 0.0;
    //        ExtentY = 0.0;
    //        ExtentZ = 0.0;
    //        VolumeOfExtent = 0.0;
    //        AreaOfExtent = 0.0;

    //        isValid = true;
    //    }
    //    public virtual void CalculateArea()
    //    {
    //        if (ExtentX != 0)
    //        {
    //            if (ExtentY != 0)
    //            {
    //                AreaOfExtent = Math.Round(ExtentX * ExtentY, 1);
    //            }
    //            if (ExtentZ != 0)
    //                VolumeOfExtent = Math.Round(AreaOfExtent * ExtentZ, 1);
    //        }
    //    }

    ////}
    //public class SummarySurveyStatistics : SummaryCollarStatistics
    //{
    //    public int surveyCount { get; set; }
    //    public int MinSurveyCount { get; set; }
    //    public int MaxSurveyCount { get; set; }
    //    public double AverageSurveyCount { get; set; }
    //    public double MinSurveyLength { get; set; }
    //    public double MaxSurveyLength { get; set; }
    //    public double AverageSurveyLength { get; set; }
    //    public new double MinimumDip { get; set; }
    //    public new double AverageDip { get; set; }
    //    public new double MaximumDip { get; set; }
    //    public double MinDipDir { get; set; }
    //    public double MaxDipDir { get; set; }
    //    public double AverageDipDir { get; set; }

    //    public SummarySurveyStatistics() : base()
    //    {
    //        collarCount = 0;
    //        surveyCount = 0;
    //        MinSurveyCount = 0;
    //        MaxSurveyCount = 0;
    //        AverageSurveyCount = 0;
    //        MinSurveyLength = 0.0;
    //        MaxSurveyLength = 0.0;
    //        AverageSurveyLength = 0.0;
    //        MinDipDir = 0.0;
    //        MaxDipDir = 0.0;
    //        AverageDipDir = 0.0;
    //    }
    //}

    //public class SummaryAssayStatistics : SummarySurveyStatistics
    //{
    //    public int AssayCount { get; set; }
    //    public int MinAssayCount { get; set; }
    //    public int MaxAssayCount { get; set; }
    //    public double AverageAssayCount { get; set; }
    //    public double MinAssayLength { get; set; }
    //    public double MaxAssayLength { get; set; }
    //    public double AverageAssayLength { get; set; }

    //    public SummaryAssayStatistics() : base()
    //    {

    //        AssayCount = 0;
    //        MinAssayCount = 0;
    //        MaxAssayCount = 0;
    //        AverageAssayCount = 0.0;
    //        MinAssayLength = 0.0;
    //        MaxAssayLength = 0.0;
    //        AverageAssayLength = 0.0;

    //        //TODO - add count for most (1) frequent occurring, and (2) length weighted

    //    }

    //}

    //public class SummaryIntervalStatistics : SummarySurveyStatistics
    //{
    //    public int IntervalCount { get; set; }
    //    public int MinIntervalCount { get; set; }
    //    public int MaxIntervalCount { get; set; }
    //    public double AverageIntervalCount { get; set; }
    //    public double MinIntervalLength { get; set; }
    //    public double MaxIntervalLength { get; set; }
    //    public double AverageIntervalLength { get; set; }
    //    public SummaryIntervalStatistics() : base()
    //    {
    //        IntervalCount = 0;
    //        MinIntervalCount = 0;
    //        MaxIntervalCount = 0;
    //        AverageIntervalCount = 0.0;
    //        MinIntervalLength = 0.0;
    //        MaxIntervalLength = 0.0;
    //        AverageIntervalLength = 0.0;

    //        //TODO - add count for most (1) frequent occurring, and (2) length weighted

    //    }
    //}
}
