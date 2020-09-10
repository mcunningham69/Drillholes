using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.CreateDrillholes
{
    public static class CalculateSurveys
    {
        /// <summary>
        /// This method utilises the average of Dip1 and Dip2 as an inclination, the average of azimuth1 and azimuth2 as a
        /// direction, and assumes the entire survey interval to be tangent to the average angle.
        /// </summary>
        /// <param name="dblX"></param>
        /// <param name="dblY"></param>
        /// <param name="dblZ"></param>
        /// <param name="dblAzim"></param>
        /// <param name="dblDip"></param>
        /// <param name="dblDistance"></param>
        /// <returns></returns>
        public static async Task<Coordinate3D> AverageAngleMethod(double dblX, double dblY, double dblZ, List<double> dblAzim, List<double> dblDip, List<double> dblDistance)
        {
            Coordinate3D coordinate3D = new Coordinate3D();

            double dblDipAvg = (dblDip[0] + dblDip[1]) / 2;
            double dblAziAvg = (dblAzim[0] + dblAzim[1]) / 2;

            double dblRadAzimuth = (Math.PI / 180) * dblAziAvg;
            double dblRadDip = (Math.PI / 180) * dblDipAvg;

            double dblMD = dblDistance[1] - dblDistance[0];

            ////formula is incorrect
            //double dblChangeX = dblMD * Math.Sin(dblRadDip) * Math.Sin(dblRadAzimuth);
            //double dblChangeY = dblMD * Math.Sin(dblRadDip) * Math.Cos(dblRadAzimuth);
            //double dblChangeZ = dblMD * Math.Cos(dblRadDip);

            double dX = dblMD * Math.Sin(dblRadAzimuth) * Math.Cos(dblRadDip);
            double dY = dblMD * Math.Cos(dblRadAzimuth) * Math.Cos(dblRadDip);
            double dZ = dblMD * Math.Sin(dblRadDip);

            coordinate3D.x = dblX + dX;
            coordinate3D.y = dblY + dY;
            coordinate3D.z = dblZ + dZ;

            return coordinate3D;
        }

        /// <summary>
        /// Ths method treats half the measured dsitance as being tangent to dip1 and azimuth1
        /// and the remainder as being tangent to dip2 and azimuth2
        /// </summary>
        /// <param name="dblX"></param>
        /// <param name="dblY"></param>
        /// <param name="dblZ"></param>
        /// <param name="dblAzim"></param>
        /// <param name="dblDip"></param>
        /// <param name="dblDistance"></param>
        /// <returns></returns>
        public static async Task<Coordinate3D> BalancedTangentialMethod(double dblX, double dblY, double dblZ, List<double> dblAzim, List<double> dblDip, List<double> dblDistance)
        {
            Coordinate3D coordinate3D = new Coordinate3D();

            double dblDip1 = dblDip[0];
            double dblDip2 = dblDip[1];

            double dblAzi1 = dblAzim[0];
            double dblAzi2 = dblAzim[1];

            double dblRadAzi1 = (Math.PI / 180) * dblAzi1;
            double dblRadDip1 = (Math.PI / 180) * dblDip1;

            double dblRadAzi2 = (Math.PI / 180) * dblAzi2;
            double dblRadDip2 = (Math.PI / 180) * dblDip2;

            double dblMD = dblDistance[1] - dblDistance[0];

            //formula is incorrect
            double dblChangeX = (dblMD / 2) * (Math.Sin(dblRadDip1) * Math.Sin(dblRadAzi1) + Math.Sin(dblRadDip2) * Math.Sin(dblRadAzi2));
            double dblChangeY = (dblMD / 2) * (Math.Sin(dblRadDip1) * Math.Cos(dblRadAzi1) + Math.Sin(dblRadDip2) * Math.Cos(dblRadAzi2));
            double dblChangeZ = (dblMD / 2) * (Math.Cos(dblRadDip2) + Math.Cos(dblRadDip1));

            coordinate3D.x = dblX + dblChangeX;
            coordinate3D.y = dblY + dblChangeY;
            coordinate3D.z = dblZ + dblChangeZ;

            return coordinate3D;
        }

        /// <summary>
        /// This method smooths the two straight-line segments of the Balanced Tangential Method using
        /// the Ratio Factor RF, where RF = 2 over beta * tan (beta over 2)
        /// </summary>
        /// <param name="dblX"></param>
        /// <param name="dblY"></param>
        /// <param name="dblZ"></param>
        /// <param name="dblAzim"></param>
        /// <param name="dblDip"></param>
        /// <param name="dblDistance"></param>
        /// <returns></returns>
        public static async Task<Coordinate3D> MinimumCurvatureMethod(double dblX, double dblY, double dblZ, List<double> dblAzim, List<double> dblDip, List<double> dblDistance)
        {
            Coordinate3D coordinate3D = new Coordinate3D();

            double dblDip1 = dblDip[0];
            double dblDip2 = dblDip[1];

            double dblAzi1 = dblAzim[0];
            double dblAzi2 = dblAzim[1];

            double RF = 0.0;

            double dblRadAzi1 = (Math.PI / 180) * dblAzi1;
            double dblRadDip1 = (Math.PI / 180) * dblDip1;

            double dblRadAzi2 = (Math.PI / 180) * dblAzi2;
            double dblRadDip2 = (Math.PI / 180) * dblDip2;

            //calculate dogleg angle for beta
            double beta = Math.Cos(dblRadDip2 - dblRadDip1) - Math.Sin(dblRadDip1) * (Math.Sin(dblRadDip2) * (1 - Math.Cos(dblRadAzi2 - dblRadAzi1)));

            double betaRad = Math.Acos(beta);

            RF = 2 / betaRad * Math.Tan(betaRad / 2);

            double dblMD = dblDistance[1] - dblDistance[0];

            //formula is incorrect
            double dblChangeX = (dblMD / 2) * ((Math.Sin(dblRadDip1) * Math.Sin(dblRadAzi1) + Math.Sin(dblRadDip2) * Math.Sin(dblRadAzi2)) * RF);
            double dblChangeY = (dblMD / 2) * ((Math.Sin(dblRadDip1) * Math.Cos(dblRadAzi1) + Math.Sin(dblRadDip2) * Math.Cos(dblRadAzi2)) * RF);
            double dblChangeZ = (dblMD / 2) * ((Math.Cos(dblRadDip2) + Math.Cos(dblRadDip1)) * RF);

            coordinate3D.x = dblX + dblChangeX;
            coordinate3D.y = dblY + dblChangeY;
            coordinate3D.z = dblZ + dblChangeZ;

            return coordinate3D;
        }

        public static async Task<Coordinate3D> RadiusCurvatureMethod(double dblX, double dblY, double dblZ, List<double> dblAzim, List<double> dblDip, List<double> dblDistance)
        {
            Coordinate3D coordinate3D = new Coordinate3D();
            double dblDip1 = dblDip[0];
            double dblDip2 = dblDip[1];

            double dblAzi1 = dblAzim[0];
            double dblAzi2 = dblAzim[1];

            double dblRadAzi1 = (Math.PI / 180) * dblAzi1;
            double dblRadDip1 = (Math.PI / 180) * dblDip1;

            double dblRadAzi2 = (Math.PI / 180) * dblAzi2;
            double dblRadDip2 = (Math.PI / 180) * dblDip2;

            double dblMD = dblDistance[1] - dblDistance[0];

            double Denom = (dblDip2 - dblDip1) * (dblAzi2 - dblAzi1);

            double ZDenom = dblDip2 - dblDip1;

            double test = (dblDip2 - dblDip1) * (dblAzi2 - dblAzi1);
            double test2 = (Math.PI / 180) * test;

            double multi = 180 / Math.PI;
            double powerMult = Math.Pow(multi, 2);

            double dblChangeX = dblMD * ((Math.Cos(dblRadDip1) - Math.Cos(dblRadDip2)) * (Math.Cos(dblRadAzi2) - Math.Cos(dblRadAzi1)) / Denom) * powerMult;
            double dblChangeY = dblMD * ((Math.Cos(dblRadDip1) - Math.Cos(dblRadDip2)) * (Math.Sin(dblRadAzi2) - Math.Sin(dblRadAzi1)) / Denom) * powerMult;
            double dblChangeZ = dblMD * (Math.Sin(dblRadDip2) - Math.Sin(dblRadDip1)) / ZDenom * multi;

            coordinate3D.x = dblX + dblChangeX;
            coordinate3D.y = dblY + dblChangeY;
            coordinate3D.z = dblZ + dblChangeZ;

            return coordinate3D;
        }


        public static async Task<Coordinate3D> ReturnCoordinateTangential(double dblX, double dblY, double dblZ, double dblDistance, double dblAzim, double dblDip)
        {
            Coordinate3D coordinate3D = new Coordinate3D(); 

            double dblRadAzimuth = (Math.PI / 180) * dblAzim;
            double dblRadDip = (Math.PI / 180) * dblDip;

            double dX = dblDistance * Math.Sin(dblRadAzimuth) * Math.Cos(dblRadDip);
            double dY = dblDistance * Math.Cos(dblRadAzimuth) * Math.Cos(dblRadDip);
            double dZ = dblDistance * Math.Sin(dblRadDip);

            coordinate3D.x = dblX + dX;
            coordinate3D.y = dblY + dY;
            coordinate3D.z = dblZ + dZ;

            return coordinate3D;
        }

        //Original code
        public static async Task<Coordinate3D> TangentialMethod(double dblX, double dblY, double dblZ, double dblDistance, double dblAzim, double dblDip)
        {
            Coordinate3D coordinate3D = new Coordinate3D();

            dblDip = dblDip * -1;

            //convert to radians
            double dblRadAzimuth = (Math.PI / 180) * dblAzim;
            double dblRadDip = (Math.PI / 180) * dblDip;

            double dblChangeX = dblDistance * Math.Sin(dblRadDip) * Math.Sin(dblRadAzimuth);
            double dblChangeY = dblDistance * Math.Sin(dblRadDip) * Math.Cos(dblRadAzimuth);
            double dblChangeZ = dblDistance * Math.Sin(dblRadDip);

            coordinate3D.x = dblX + dblChangeX;
            coordinate3D.y = dblY + dblChangeY;
            coordinate3D.z = dblZ - dblChangeZ;

            return coordinate3D;
        }
    }

    /// <summary>
    /// Plunge is positive for downhole
    /// Results give in dip and dip direction (for trike in right-hand rule, subtract 90 degrees
    /// Code based on Excel formula from SRK worksheet (Peter Williams, 1998)
    /// Letters after code line is corresponding field in Excel 'Calculator' worksheet
    /// </summary>



    public class Coordinate3D
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
    }

    public class DownholeSurveys
    {
        public List<double> azimuth { get; set; }
        public List<double> dip { get; set; }
        public List<double>distance { get; set; }
        public List<double> distFrom { get; set; }
        public List<double> distTo { get; set; }

        public DownholeSurveys()
        {
            azimuth = new List<double>();
            dip = new List<double>();
            distance = new List<double>();
            distFrom = new List<double>();
            distTo = new List<double>();
        }
    }

    public static class AlphaBetaGamma
    {
        /// <summary>
        /// Calculates dip from downhole alpha beta measurements
        /// </summary>
        /// <param name="azimuth"></param>
        /// <param name="dip"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="TopCore"></param>
        /// <returns></returns>
        public static async Task<double> CalculateDip(double azimuth, double dip, double alpha, double beta, bool bottomCore)
        {
            if (dip < 0)
                dip = dip * -1;

            #region Calculation of rotation axis, rotator, and matrix
            //Rotation axis
            double Ex = Math.Cos((azimuth + 90) / 180 * Math.PI); //Z
            double Ey = Math.Sin((azimuth + 90) / 180 * Math.PI); //AA

            //Calculations of rotator (in radians)
            double rotarAngle = (90.0 - dip) / 180 * Math.PI; //AB

            //Rotation matrix
            double R31 = (Ey * -1) * (Math.Sin(rotarAngle));//AI
            double R32 = Ex * Math.Sin(rotarAngle);
            double R33 = Math.Cos(rotarAngle); //AK
            #endregion

            //Pole to first plane (Sa)
            double plunge = alpha / 180 * Math.PI; //AM => Pole to Sa
            double trend;

            //false if reference orientation line is 'top'
            if (!bottomCore) //AN
            {
                trend = (azimuth + beta + 180) / 180 * Math.PI;
            }
            else
            {
                trend = (azimuth + beta) / 180 * Math.PI;
            }

            //Pole to Sa' after rotation
            double Az = R31 * Math.Cos(plunge) * Math.Cos(trend) + R32 * Math.Cos(plunge) * Math.Sin(trend) + R33 * Math.Sin(plunge); //AQ

            //Pole to Sa'
            double plunge2 = Math.Abs(Math.Asin(Az)); //AR

            // dip/plunge
            double calculated_DIP = 90 - plunge2 / Math.PI * 180;

            return calculated_DIP;
        }

        /// <summary>
        /// Returns calculated dip direction from alpha beta measurments taken from oriented core
        /// </summary>
        /// <param name="azimuth"></param>
        /// <param name="dip"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="TopCore"></param>
        /// <returns></returns>
        public static async Task<double> CalculateDipDirection(double azimuth, double dip, double alpha, double beta, bool bottomCore)
        {
            if (dip < 0)
                dip = dip * -1;

            #region Calculation of rotation axis, rotator, and matrix
            //Rotation axis
            double Ex = Math.Cos((azimuth + 90) / 180 * Math.PI); //Z
            double Ey = Math.Sin((azimuth + 90) / 180 * Math.PI); //AA

            //Calculations of rotator (in radians)
            double rotarAngle = (90.0 - dip) / 180 * Math.PI; //AB

            //Rotation matrix
            double R11 = Math.Pow(Ex, 2) * (1 - Math.Cos(rotarAngle)) + Math.Cos(rotarAngle); //AC
            double R12 = Ex * Ey * (1 - Math.Cos(rotarAngle)); //AD
            double R13 = Ey * Math.Sin(rotarAngle); //AE

            double R21 = Ey * Ex * (1 - Math.Cos(rotarAngle));
            double R22 = Math.Pow(Ey, 2) * (1 - Math.Cos(rotarAngle)) + Math.Cos(rotarAngle);
            double R23 = (Ex * -1) * Math.Sin(rotarAngle);

            double R31 = (Ey * -1) * (Math.Sin(rotarAngle));//AI
            double R32 = Ex * Math.Sin(rotarAngle);
            double R33 = Math.Cos(rotarAngle);
            #endregion

            //Pole to first plane (Sa)
            double plunge = alpha / 180 * Math.PI; //AM => Pole to Sa
            double trend;

            //true if reference orientation line on more is 'top', false if 'bottom'
            if (!bottomCore) //AN
                trend = (azimuth + beta + 180) / 180 * Math.PI;
            else
                trend = (azimuth + beta) / 180 * Math.PI;

            #region pole to Sa' after rotation
            double Ax = R11 * Math.Cos(plunge) * Math.Cos(trend) + R12 * Math.Cos(plunge) * Math.Sin(trend) + R13 * Math.Sin(plunge);
            double Ay = R21 * Math.Cos(plunge) * Math.Cos(trend) + R22 * Math.Cos(plunge) * Math.Sin(trend) + R23 * Math.Sin(plunge);
            double Az = R31 * Math.Cos(plunge) * Math.Cos(trend) + R32 * Math.Cos(plunge) * Math.Sin(trend) + R33 * Math.Sin(plunge);
            #endregion

            double trend1;
            double trend2;

            //Pole to Sa' - trend-1 in radians
            if (Math.Asin(Az) < 0)
                trend1 = Math.Atan2(Ay, Ax) + Math.PI; //AS
            else
                trend1 = Math.Atan2(Ay, Ax);

            //Pole to Sa' - trend-2 in degrees
            if (trend1 < 0)
                trend2 = trend1 + 2 * Math.PI;  //AT
            else
                trend2 = trend1;

            //Calculated dip direction from alpha-beta
            double CalculatedDipDIR;
            if (trend2 < Math.PI) //L
                CalculatedDipDIR = (trend2 + Math.PI) / Math.PI * 180;
            else
                CalculatedDipDIR = (trend2 - Math.PI) / Math.PI * 180;

            return CalculatedDipDIR;
        }

        /// <summary>
        /// Returns plunge and dip of lineation on plane - alpha-beta-gamma
        /// </summary>
        /// <param name="azimuth"></param>
        /// <param name="dip"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="gamma"></param>
        /// <param name="TopCore"></param>
        /// <returns></returns>
        public static async Task<CalculatedPlungeAndTrend> CalculateGammaValues(double azimuth, double dip, double alpha, double beta, double gamma)
        {
            //only calculate if gamma measured
            if (gamma == 0.0)
                return null;

            //use positive values
            if (dip < 0)
                dip = dip * -1;

            #region Calculation of rotation axis, rotator, and matrix
            //Rotation axis
            double Ex = Math.Cos((azimuth + 90) / 180 * Math.PI); //Z
            double Ey = Math.Sin((azimuth + 90) / 180 * Math.PI); //AA

            //Calculations of rotator (in radians)
            double rotarAngle = (90.0 - dip) / 180 * Math.PI; //AB

            //Rotator / rotation matrix
            double R11 = Math.Pow(Ex, 2) * (1 - Math.Cos(rotarAngle)) + Math.Cos(rotarAngle); //AC
            double R12 = Ex * Ey * (1 - Math.Cos(rotarAngle)); //AD
            double R13 = Ey * Math.Sin(rotarAngle); //AE

            double R21 = Ey * Ex * (1 - Math.Cos(rotarAngle)); //AF
            double R22 = Math.Pow(Ey, 2) * (1 - Math.Cos(rotarAngle)) + Math.Cos(rotarAngle); //AG
            double R23 = (Ex * -1) * Math.Sin(rotarAngle); //AH

            double R31 = (Ey * -1) * (Math.Sin(rotarAngle));//AI
            double R32 = Ex * Math.Sin(rotarAngle);//AJ
            double R33 = Math.Cos(rotarAngle); //AK
            #endregion

            //caluclations of lineation on plane Sa (La)
            double dipPlane_rad = (90.0 - alpha) / 180 * Math.PI; //AV//radians
            double azimuth_of_plane = (azimuth + 180 + beta) / 180 * Math.PI; //AW

            //pitch of lineation
            double lin_pitch;
            if (gamma < 90.0) //AX
                lin_pitch = (89.9 - gamma) / 180 * Math.PI;
            else if (gamma < 180)
                lin_pitch = (179.9 - gamma) / 180 * Math.PI;
            else if (gamma < 270)
                lin_pitch = (269.9 - gamma) / 180 * Math.PI;
            else
                lin_pitch = (449.9 - gamma) / 180 * Math.PI;

            //calculations of lineation on plane Sa (La) - La before rotation
            double plunge_before_rotation = Math.Asin(Math.Sin(dipPlane_rad) * Math.Sin(lin_pitch)); //AY

            //calculations of lineation on plane Sa (La) - La before rotation
            double trend_before_rotation; //AZ
            if (lin_pitch > Math.PI / 2)
            {
                trend_before_rotation = azimuth_of_plane - Math.Acos(Math.Tan(plunge_before_rotation) / Math.Tan(dipPlane_rad));
            }
            else
                trend_before_rotation = azimuth_of_plane + Math.Acos(Math.Tan(plunge_before_rotation) / Math.Tan(dipPlane_rad));

            //La after rotation
            double La_x = R11 * Math.Cos(plunge_before_rotation) * Math.Cos(trend_before_rotation) + R12 * Math.Cos(plunge_before_rotation) * Math.Sin(trend_before_rotation) + R13 * Math.Sin(trend_before_rotation); //BA
            double La_y = R21 * Math.Cos(plunge_before_rotation) * Math.Cos(trend_before_rotation) + R22 * Math.Cos(plunge_before_rotation) * Math.Sin(trend_before_rotation) + R23 * Math.Sin(plunge_before_rotation); //BB
            double La_z = R31 * Math.Cos(plunge_before_rotation) * Math.Cos(trend_before_rotation) + R32 * Math.Cos(plunge_before_rotation) * Math.Sin(trend_before_rotation) + R33 * Math.Sin(plunge_before_rotation); //BC

            //La' (i.e. La prime)
            double La_prime_trend; //BE
            if (Math.Sin(La_z) < 0)
                La_prime_trend = Math.Atan2(La_y, La_x) + Math.PI;
            else
                La_prime_trend = Math.Atan2(La_y, La_x);

            double la_prime_plunge = Math.Abs(Math.Asin(La_z)); //BD

            //Calculated plunge of lineation on La plane
            double linPlunge = la_prime_plunge / Math.PI * 180; //R

            //Calculated trend of lineation on LA plane
            double linTrend; //S
            if (La_prime_trend < 0)
                linTrend = (La_prime_trend + 2 * Math.PI) / Math.PI * 180;
            else if (La_prime_trend > 2 * Math.PI)
                linTrend = (La_prime_trend - 2 * Math.PI) / Math.PI * 180;
            else
                linTrend = La_prime_trend / Math.PI * 180;

            //helper class for storing calcualted trend and plunge of lineation
            CalculatedPlungeAndTrend returnValues = new CalculatedPlungeAndTrend()
            {
                lineation_trend = linTrend,
                lineation_plunge = linPlunge
            };

            return returnValues;
        }

    }
    /// <summary>
    /// Stores property values for calculated plunge and trend from gamma downhole
    /// </summary>
    public class CalculatedPlungeAndTrend
    {
        public double lineation_plunge { get; set; }
        public double lineation_trend { get; set; }

    }
}
