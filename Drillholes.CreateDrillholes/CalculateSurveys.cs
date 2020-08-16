using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drillholes.CreateDrillholes
{
    public static class CalculateSurveys
    {
        public static async Task<Coordinate3D> ReturnCoordinateTangential(double dblX, double dblY, double dblZ, double dblDistance, double dblAzim, double dblDip)
        {
            Coordinate3D coordinate3D = new Coordinate3D(); ;

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
    }

    public class Coordinate3D
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
    }
}
