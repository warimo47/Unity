namespace GPS
{
    public class GeoPoint
    {
        public double east { get; set; }
        public double north { get; set; }
        public double height { get; set; }

        public GeoPoint(double _east = 0.0, double _north = 0.0)
        {
            east = _east;
            north = _north;
        }

        public GeoPoint(double _east = 0.0, double _north = 0.0, double _height = 0.0)
        {
            east = _east;
            north = _north;
            height = _height;
        }
    }
}
