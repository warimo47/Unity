using System;

namespace GPS
{
    public class GeoTrans
    {
        public static int GEO = 0;
        public static int KATEC = 1;
        public static int TM = 2;
        public static int GRS80 = 3;
        public static int UTMK = 4;

        private readonly double[] m_Ind = new double[5];
        private readonly double[] m_Es = new double[5];
        private readonly double[] m_Esp = new double[5];
        private readonly double[] src_m = new double[5];
        private readonly double[] dst_m = new double[5];

        private readonly double EPSLN = 0.0000000001;
        private readonly double[] m_arMajor = new double[5];
        private readonly double[] m_arMinor = new double[5];

        private readonly double[] m_arScaleFactor = new double[5];
        private readonly double[] m_arLonCenter = new double[5];
        private readonly double[] m_arLatCenter = new double[5];
        private readonly double[] m_arFalseNorthing = new double[5];
        private readonly double[] m_arFalseEasting = new double[5];

        private readonly double[] datum_params = new double[3];

        public GeoTrans()
        {
            m_arScaleFactor[GEO] = 1;
            m_arLonCenter[GEO] = 0.0;
            m_arLatCenter[GEO] = 0.0;
            m_arFalseNorthing[GEO] = 0.0;
            m_arFalseEasting[GEO] = 0.0;
            m_arMajor[GEO] = 6378137.0;
            m_arMinor[GEO] = 6356752.3142;

            m_arScaleFactor[KATEC] = 0.9999;
            m_arLonCenter[KATEC] = 2.23402144255274; // 128
            m_arLatCenter[KATEC] = 0.663225115757845;
            m_arFalseNorthing[KATEC] = 600000.0;
            m_arFalseEasting[KATEC] = 400000.0;
            m_arMajor[KATEC] = 6377397.155;
            m_arMinor[KATEC] = 6356078.9633422494;

            m_arScaleFactor[TM] = 1;
            m_arLonCenter[TM] = 2.21656815003280; // 127
            m_arLatCenter[TM] = 0.663225115757845;
            m_arFalseNorthing[TM] = 600000.0;
            m_arFalseEasting[TM] = 200000.0;
            m_arMajor[TM] = 6377397.155;
            m_arMinor[TM] = 6356078.9633422494;

            m_arScaleFactor[GRS80] = 1.0;//0.9999;
            m_arLonCenter[GRS80] = 2.21656815003280; // 127
                                                     //m_arLonCenter[GRS80] = 2.21661859489671; // 127.+10.485 minute
            m_arLatCenter[GRS80] = 0.663225115757845;
            m_arFalseNorthing[GRS80] = 600000.0;
            m_arFalseEasting[GRS80] = 200000.0;
            m_arMajor[GRS80] = 6378137;
            m_arMinor[GRS80] = 6356752.3142;

            m_arScaleFactor[UTMK] = 0.9996;//0.9999;
                                           //m_arLonCenter[UTMK] = 2.22534523630815; // 127.502890
            m_arLonCenter[UTMK] = 2.22529479629277; // 127.5
            m_arLatCenter[UTMK] = 0.663225115757845;
            m_arFalseNorthing[UTMK] = 2000000.0;
            m_arFalseEasting[UTMK] = 1000000.0;
            m_arMajor[UTMK] = 6378137;
            m_arMinor[UTMK] = 6356752.3141403558;

            datum_params[0] = -146.43;
            datum_params[1] = 507.89;
            datum_params[2] = 681.46;

            double tmp = m_arMinor[GEO] / m_arMajor[GEO];
            m_Es[GEO] = 1.0 - tmp * tmp;
            m_Esp[GEO] = m_Es[GEO] / (1.0 - m_Es[GEO]);

            if (m_Es[GEO] < 0.00001)
            {
                m_Ind[GEO] = 1.0;
            }
            else
            {
                m_Ind[GEO] = 0.0;
            }

            tmp = m_arMinor[KATEC] / m_arMajor[KATEC];
            m_Es[KATEC] = 1.0 - tmp * tmp;
            m_Esp[KATEC] = m_Es[KATEC] / (1.0 - m_Es[KATEC]);

            if (m_Es[KATEC] < 0.00001)
            {
                m_Ind[KATEC] = 1.0;
            }
            else
            {
                m_Ind[KATEC] = 0.0;
            }

            tmp = m_arMinor[TM] / m_arMajor[TM];
            m_Es[TM] = 1.0 - tmp * tmp;
            m_Esp[TM] = m_Es[TM] / (1.0 - m_Es[TM]);

            if (m_Es[TM] < 0.00001)
            {
                m_Ind[TM] = 1.0;
            }
            else
            {
                m_Ind[TM] = 0.0;
            }

            tmp = m_arMinor[UTMK] / m_arMajor[UTMK];
            m_Es[UTMK] = 1.0 - tmp * tmp;
            m_Esp[UTMK] = m_Es[UTMK] / (1.0 - m_Es[UTMK]);

            if (m_Es[UTMK] < 0.00001)
            {
                m_Ind[UTMK] = 1.0;
            }
            else
            {
                m_Ind[UTMK] = 0.0;
            }

            tmp = m_arMinor[GRS80] / m_arMajor[GRS80];
            m_Es[GRS80] = 1.0 - tmp * tmp;
            m_Esp[GRS80] = m_Es[GRS80] / (1.0 - m_Es[GRS80]);

            if (m_Es[GRS80] < 0.00001)
            {
                m_Ind[GRS80] = 1.0;
            }
            else
            {
                m_Ind[GRS80] = 0.0;
            }

            src_m[GEO] = m_arMajor[GEO] * Mlfn(E0fn(m_Es[GEO]), E1fn(m_Es[GEO]), E2fn(m_Es[GEO]), E3fn(m_Es[GEO]), m_arLatCenter[GEO]);
            dst_m[GEO] = m_arMajor[GEO] * Mlfn(E0fn(m_Es[GEO]), E1fn(m_Es[GEO]), E2fn(m_Es[GEO]), E3fn(m_Es[GEO]), m_arLatCenter[GEO]);
            src_m[KATEC] = m_arMajor[KATEC] * Mlfn(E0fn(m_Es[KATEC]), E1fn(m_Es[KATEC]), E2fn(m_Es[KATEC]), E3fn(m_Es[KATEC]), m_arLatCenter[KATEC]);
            dst_m[KATEC] = m_arMajor[KATEC] * Mlfn(E0fn(m_Es[KATEC]), E1fn(m_Es[KATEC]), E2fn(m_Es[KATEC]), E3fn(m_Es[KATEC]), m_arLatCenter[KATEC]);
            src_m[TM] = m_arMajor[TM] * Mlfn(E0fn(m_Es[TM]), E1fn(m_Es[TM]), E2fn(m_Es[TM]), E3fn(m_Es[TM]), m_arLatCenter[TM]);
            dst_m[TM] = m_arMajor[TM] * Mlfn(E0fn(m_Es[TM]), E1fn(m_Es[TM]), E2fn(m_Es[TM]), E3fn(m_Es[TM]), m_arLatCenter[TM]);
            src_m[GRS80] = m_arMajor[GRS80] * Mlfn(E0fn(m_Es[GRS80]), E1fn(m_Es[GRS80]), E2fn(m_Es[GRS80]), E3fn(m_Es[GRS80]), m_arLatCenter[GRS80]);
            dst_m[GRS80] = m_arMajor[GRS80] * Mlfn(E0fn(m_Es[GRS80]), E1fn(m_Es[GRS80]), E2fn(m_Es[GRS80]), E3fn(m_Es[GRS80]), m_arLatCenter[GRS80]);
            src_m[UTMK] = m_arMajor[UTMK] * Mlfn(E0fn(m_Es[UTMK]), E1fn(m_Es[UTMK]), E2fn(m_Es[UTMK]), E3fn(m_Es[UTMK]), m_arLatCenter[UTMK]);
            dst_m[UTMK] = m_arMajor[UTMK] * Mlfn(E0fn(m_Es[UTMK]), E1fn(m_Es[UTMK]), E2fn(m_Es[UTMK]), E3fn(m_Es[UTMK]), m_arLatCenter[UTMK]);
        }

        private double D2R(double degree)
        {
            return degree * Math.PI / 180.0;
        }

        private double R2D(double radian)
        {
            return radian * 180.0 / Math.PI;
        }

        private double E0fn(double x)
        {
            return 1.0 - 0.25 * x * (1.0 + x / 16.0 * (3.0 + 1.25 * x));
        }

        private double E1fn(double x)
        {
            return 0.375 * x * (1.0 + 0.25 * x * (1.0 + 0.46875 * x));
        }

        private double E2fn(double x)
        {
            return 0.05859375 * x * x * (1.0 + 0.75 * x);
        }

        private double E3fn(double x)
        {
            return x * x * x * (35.0 / 3072.0);
        }

        private double Mlfn(double e0, double e1, double e2, double e3, double phi)
        {
            return e0 * phi - e1 * Math.Sin(2.0 * phi) + e2 * Math.Sin(4.0 * phi) - e3 * Math.Sin(6.0 * phi);
        }

        private double ASinz(double value)
        {
            if (Math.Abs(value) > 1.0)
            {
                value = (value > 0 ? 1 : -1);
            }

            return Math.Asin(value);
        }

        public GeoPoint Convert(int srctype, int dsttype, GeoPoint in_pt)
        {
            GeoPoint tmpPt = new GeoPoint(0, 0);
            GeoPoint out_pt = new GeoPoint(0, 0);

            if (srctype == GEO)
            {
                tmpPt.east = D2R(in_pt.east);
                tmpPt.north = D2R(in_pt.north);
                // tmpPt.Z = D2R(in_pt.Z);
            }
            else
            {
                Tm2geo(srctype, in_pt, tmpPt);
            }

            if (dsttype == GEO)
            {
                out_pt.east = R2D(tmpPt.east);
                out_pt.north = R2D(tmpPt.north);
            }
            else
            {
                Geo2tm(dsttype, tmpPt, out_pt);
                //out_pt.x = Math.round(out_pt.x);
                //out_pt.y = Math.round(out_pt.y);
            }

            return out_pt;
        }

        public void Geo2tm(int dsttype, GeoPoint in_pt, GeoPoint out_pt)
        {
            Transform(GEO, dsttype, in_pt);
            double delta_lon = in_pt.east - m_arLonCenter[dsttype];
            double Sin_phi = Math.Sin(in_pt.north);
            double Cos_phi = Math.Cos(in_pt.north);
            double al = Cos_phi * delta_lon;
            double als = al * al;
            double c = m_Esp[dsttype] * Cos_phi * Cos_phi;
            double tq = Math.Tan(in_pt.north);
            double t = tq * tq;
            double con = 1.0 - m_Es[dsttype] * Sin_phi * Sin_phi;
            double n = m_arMajor[dsttype] / Math.Sqrt(con);
            double ml = m_arMajor[dsttype] * Mlfn(E0fn(m_Es[dsttype]), E1fn(m_Es[dsttype]), E2fn(m_Es[dsttype]), E3fn(m_Es[dsttype]), in_pt.north);

            out_pt.east = m_arScaleFactor[dsttype] * n * al * (1.0 + als / 6.0 * (1.0 - t + c + als / 20.0 * (5.0 - 18.0 * t + t * t + 72.0 * c - 58.0 * m_Esp[dsttype]))) + m_arFalseEasting[dsttype];
            out_pt.north = m_arScaleFactor[dsttype] * (ml - dst_m[dsttype] + n * tq * (als * (0.5 + als / 24.0 * (5.0 - t + 9.0 * c + 4.0 * c * c + als / 30.0 * (61.0 - 58.0 * t + t * t + 600.0 * c - 330.0 * m_Esp[dsttype]))))) + m_arFalseNorthing[dsttype];
        }

        public void Tm2geo(int srctype, GeoPoint in_pt, GeoPoint out_pt)
        {
            GeoPoint tmpPt = new GeoPoint(in_pt.east, in_pt.north);
            int max_iter = 6;

            if (m_Ind[srctype] != 0)
            {
                double f = Math.Exp(in_pt.east / (m_arMajor[srctype] * m_arScaleFactor[srctype]));
                double g = 0.5 * (f - 1.0 / f);
                double temp = m_arLatCenter[srctype] + tmpPt.north / (m_arMajor[srctype] * m_arScaleFactor[srctype]);
                double h = Math.Cos(temp);
                double conn = Math.Sqrt((1.0 - h * h) / (1.0 + g * g));
                out_pt.north = ASinz(conn);

                if (temp < 0)
                {
                    out_pt.north *= -1;
                }

                if ((g == 0) && (h == 0))
                {
                    out_pt.east = m_arLonCenter[srctype];
                }
                else
                {
                    out_pt.east = Math.Atan(g / h) + m_arLonCenter[srctype];
                }
            }

            tmpPt.east -= m_arFalseEasting[srctype];
            tmpPt.north -= m_arFalseNorthing[srctype];

            double con = (src_m[srctype] + tmpPt.north / m_arScaleFactor[srctype]) / m_arMajor[srctype];
            double phi = con;

            int i = 0;

            while (true)
            {
                double delta_Phi = ((con + E1fn(m_Es[srctype]) * Math.Sin(2.0 * phi) - E2fn(m_Es[srctype]) * Math.Sin(4.0 * phi) + E3fn(m_Es[srctype]) * Math.Sin(6.0 * phi)) / E0fn(m_Es[srctype])) - phi;
                phi += delta_Phi;

                if (Math.Abs(delta_Phi) <= EPSLN)
                {
                    break;
                }

                if (i >= max_iter)
                {
                    //Log.d("무한대 에러");
                    //System.out.println("무한대 에러");
                    break;
                }

                i++;
            }

            if (Math.Abs(phi) < (Math.PI / 2))
            {
                double Sin_phi = Math.Sin(phi);
                double Cos_phi = Math.Cos(phi);
                double tan_phi = Math.Tan(phi);
                double c = m_Esp[srctype] * Cos_phi * Cos_phi;
                double cs = c * c;
                double t = tan_phi * tan_phi;
                double ts = t * t;
                double cont = 1.0 - m_Es[srctype] * Sin_phi * Sin_phi;
                double n = m_arMajor[srctype] / Math.Sqrt(cont);
                double r = n * (1.0 - m_Es[srctype]) / cont;
                double d = tmpPt.east / (n * m_arScaleFactor[srctype]);
                double ds = d * d;
                out_pt.north = phi - (n * tan_phi * ds / r) * (0.5 - ds / 24.0 * (5.0 + 3.0 * t + 10.0 * c - 4.0 * cs - 9.0 * m_Esp[srctype] - ds / 30.0 * (61.0 + 90.0 * t + 298.0 * c + 45.0 * ts - 252.0 * m_Esp[srctype] - 3.0 * cs)));
                out_pt.east = m_arLonCenter[srctype] + (d * (1.0 - ds / 6.0 * (1.0 + 2.0 * t + c - ds / 20.0 * (5.0 - 2.0 * c + 28.0 * t - 3.0 * cs + 8.0 * m_Esp[srctype] + 24.0 * ts))) / Cos_phi);
            }
            else
            {
                out_pt.north = Math.PI * 0.5 * Math.Sin(tmpPt.north);
                out_pt.east = m_arLonCenter[srctype];
            }
            Transform(srctype, GEO, out_pt);
        }

        public double GetDistancebyGeo(GeoPoint pt1, GeoPoint pt2)
        {
            double lat1 = D2R(pt1.north);
            double lon1 = D2R(pt1.east);
            double lat2 = D2R(pt2.north);
            double lon2 = D2R(pt2.east);

            double longitude = lon2 - lon1;
            double latitude = lat2 - lat1;

            double a = Math.Pow(Math.Sin(latitude / 2.0), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(longitude / 2.0), 2);
            return 6376.5 * 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
        }

        public double GetDistancebyKatec(GeoPoint pt1, GeoPoint pt2)
        {
            pt1 = Convert(KATEC, GEO, pt1);
            pt2 = Convert(KATEC, GEO, pt2);

            return GetDistancebyGeo(pt1, pt2);
        }

        public double GetDistancebyTm(GeoPoint pt1, GeoPoint pt2)
        {
            pt1 = Convert(TM, GEO, pt1);
            pt2 = Convert(TM, GEO, pt2);

            return GetDistancebyGeo(pt1, pt2);
        }

        public double GetDistancebyUTMK(GeoPoint pt1, GeoPoint pt2)
        {
            pt1 = Convert(UTMK, GEO, pt1);
            pt2 = Convert(UTMK, GEO, pt2);

            return GetDistancebyGeo(pt1, pt2);
        }

        public double GetDistancebyGrs80(GeoPoint pt1, GeoPoint pt2)
        {
            pt1 = Convert(GRS80, GEO, pt1);
            pt2 = Convert(GRS80, GEO, pt2);

            return GetDistancebyGeo(pt1, pt2);
        }

        //public long getTimebyMin(double distance)
        //{
        //    return (long)(Math.Ceiling(getTimebySec(distance) / 60));
        //}

        /*
        Author:       Richard Greenwood rich@greenwoodmap.com
        License:      LGPL as per: http://www.gnu.org/copyleft/lesser.html
        */

        /**
         * convert between geodetic coordinates (longitude, latitude, height)
         * and gecentric coordinates (X, Y, Z)
         * ported from Proj 4.9.9 geocent.c
        */

        // following constants from geocent.c
        private readonly double HALF_PI = 0.5 * Math.PI;
        private readonly double Cos_67P5 = 0.38268343236508977;  /* Cosine of 67.5 degrees */
        private readonly double AD_C = 1.0026000;
        /* Toms region 1 constant */

        private void Transform(int srctype, int dsttype, GeoPoint point)
        {
            if (srctype == dsttype)
            {
                return;
            }

            if ((srctype != 0 && srctype != GRS80 && srctype != UTMK) || (dsttype != 0 && dsttype != GRS80 && dsttype != UTMK))
            {
                // Convert to geocentric coordinates.
                Geodetic_to_geocentric(srctype, point);

                // Convert between datums
                if (srctype != 0 && srctype != GRS80 && srctype != UTMK)
                {
                    Geocentric_to_wgs84(point);
                }

                if (dsttype != 0 && dsttype != GRS80 && dsttype != UTMK)
                {
                    Geocentric_from_wgs84(point);
                }

                // Convert back to geodetic coordinates
                Geocentric_to_geodetic(dsttype, point);
            }
        }

        private bool Geodetic_to_geocentric(int type, GeoPoint p)
        {
            // The function Convert_Geodetic_To_Geocentric converts geodetic coordinates
            // (latitude, longitude, and height) to geocentric coordinates (X, Y, Z),
            // according to the current ellipsoid parameters.

            //    Latitude  : Geodetic latitude in radians                     (input)
            //    Longitude : Geodetic longitude in radians                    (input)
            //    Height    : Geodetic height, in meters                       (input)
            //    X         : Calculated Geocentric X coordinate, in meters    (output)
            //    Y         : Calculated Geocentric Y coordinate, in meters    (output)
            //    Z         : Calculated Geocentric Z coordinate, in meters    (output)

            double Longitude = p.east;
            double Latitude = p.north;
            double Height = p.height;
            double X;  // output
            double Y;
            double Z;

            double Rn;            //  Earth radius at location
            double Sin_Lat;       //  Math.Sin(Latitude)
            double Sin2_Lat;      //  Square of Math.Sin(Latitude)
            double Cos_Lat;       //  Math.Cos(Latitude)

            // Don't blow up if Latitude is just a little out of the value
            // range as it may just be a rounding issue.  Also removed longitude
            // test, it should be wrapped by Math.Cos() and Math.Sin().  NFW for PROJ.4, Sep/2001.
            if (Latitude < -HALF_PI && Latitude > -1.001 * HALF_PI)
            {
                Latitude = -HALF_PI;
            }
            else if (Latitude > HALF_PI && Latitude < 1.001 * HALF_PI)
            {
                Latitude = HALF_PI;
            }
            else if ((Latitude < -HALF_PI) || (Latitude > HALF_PI))
            { // Latitude out of range
                return true;
            }

            // no errors
            if (Longitude > Math.PI)
            {
                Longitude -= (2 * Math.PI);
            }

            Sin_Lat = Math.Sin(Latitude);
            Cos_Lat = Math.Cos(Latitude);
            Sin2_Lat = Sin_Lat * Sin_Lat;
            Rn = m_arMajor[type] / (Math.Sqrt(1.0e0 - m_Es[type] * Sin2_Lat));
            X = (Rn + Height) * Cos_Lat * Math.Cos(Longitude);
            Y = (Rn + Height) * Cos_Lat * Math.Sin(Longitude);
            Z = ((Rn * (1 - m_Es[type])) + Height) * Sin_Lat;

            p.east = X;
            p.north = Y;
            p.height = Z;
            return false;
        } // cs_geodetic_to_geocentric()

        /** Convert_Geocentric_To_Geodetic
         * The method used here is derived from 'An Improved Algorithm for
         * Geocentric to Geodetic Coordinate Conversion', by Ralph Toms, Feb 1996
         */
        private void Geocentric_to_geodetic(int type, GeoPoint p)
        {

            double X = p.east;
            double Y = p.north;
            double Z = p.height;
            double Longitude;
            double Latitude = 0;
            double Height;

            double W;        /* distance from Z axis */
            double W2;       /* square of distance from Z axis */
            double T0;       /* initial estimate of vertical component */
            double T1;       /* corrected estimate of vertical component */
            double S0;       /* initial estimate of horizontal component */
            double S1;       /* corrected estimate of horizontal component */
            double Sin_B0;   /* Math.Sin(B0), B0 is estimate of Bowring aux doubleiable */
            double Sin3_B0;  /* cube of Math.Sin(B0) */
            double Cos_B0;   /* Math.Cos(B0) */
            double Sin_p1;   /* Math.Sin(phi1), phi1 is estimated latitude */
            double Cos_p1;   /* Math.Cos(phi1) */
            double Rn;       /* Earth radius at location */
            double Sum;      /* numerator of Math.Cos(phi1) */
            bool At_Pole;  /* indicates location is in polar region */

            At_Pole = false;
            if (X != 0.0)
            {
                Longitude = Math.Atan2(Y, X);
            }
            else
            {
                if (Y > 0)
                {
                    Longitude = HALF_PI;
                }
                else if (Y < 0)
                {
                    Longitude = -HALF_PI;
                }
                else
                {
                    At_Pole = true;
                    Longitude = 0.0;
                    if (Z > 0.0)
                    {  /* north pole */
                        Latitude = HALF_PI;
                    }
                    else if (Z < 0.0)
                    {  /* south pole */
                        Latitude = -HALF_PI;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            W2 = X * X + Y * Y;
            W = Math.Sqrt(W2);
            T0 = Z * AD_C;
            S0 = Math.Sqrt(T0 * T0 + W2);
            Sin_B0 = T0 / S0;
            Cos_B0 = W / S0;
            Sin3_B0 = Sin_B0 * Sin_B0 * Sin_B0;
            T1 = Z + m_arMinor[type] * m_Esp[type] * Sin3_B0;
            Sum = W - m_arMajor[type] * m_Es[type] * Cos_B0 * Cos_B0 * Cos_B0;
            S1 = Math.Sqrt(T1 * T1 + Sum * Sum);
            Sin_p1 = T1 / S1;
            Cos_p1 = Sum / S1;
            Rn = m_arMajor[type] / Math.Sqrt(1.0 - m_Es[type] * Sin_p1 * Sin_p1);
            if (Cos_p1 >= Cos_67P5)
            {
                Height = W / Cos_p1 - Rn;
            }
            else if (Cos_p1 <= -Cos_67P5)
            {
                Height = W / -Cos_p1 - Rn;
            }
            else
            {
                Height = Z / Sin_p1 + Rn * (m_Es[type] - 1.0);
            }
            if (At_Pole == false)
            {
                Latitude = Math.Atan(Sin_p1 / Cos_p1);
            }

            p.east = Longitude;
            p.north = Latitude;
            p.height = Height;
            return;
        } // geocentric_to_geodetic()



        /****************************************************************/
        // geocentic_to_wgs84(defn, p )
        //  defn = coordinate system definition,
        //  p = point to transform in geocentric coordinates (x,y,z)
        private void Geocentric_to_wgs84(GeoPoint p)
        {

            //if( defn.datum_type == PJD_3PARAM )
            {
                // if( x[io] == HUGE_VAL )
                //    continue;
                p.east += datum_params[0];
                p.north += datum_params[1];
                p.height += datum_params[2];
            }
        } // geocentric_to_wgs84

        /****************************************************************/
        // geocentic_from_wgs84()
        //  coordinate system definition,
        //  point to transform in geocentric coordinates (x,y,z)
        private void Geocentric_from_wgs84(GeoPoint p)
        {

            //if( defn.datum_type == PJD_3PARAM ) 
            {
                //if( x[io] == HUGE_VAL )
                //    continue;
                p.east -= datum_params[0];
                p.north -= datum_params[1];
                p.height -= datum_params[2];
            }
        }
    }
}
