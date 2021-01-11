using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GPS;

public class Example : MonoBehaviour
{
    private GeoTrans geaTrans;

    public double defaultCCTVPositionEast;
    public double defaultCCTVPositionNorth;
    private GeoPoint northWest;
    public double eastDiff;
    public double northDiff;

    // Start is called before the first frame update
    void Start()
    {
        geaTrans = new GeoTrans();
        northWest = new GeoPoint(defaultCCTVPositionEast, defaultCCTVPositionNorth);
        Debug.Log(northWest.east + " " + northWest.north);

        GeoPoint targetPos = GetTargetPosition(northWest, eastDiff, northDiff);
        Debug.Log(targetPos.east + " " + targetPos.north);
    }

    public GeoPoint GetTargetPosition(GeoPoint _CCTVPosition, double _eastDiff, double _northDiff)
    {
        GeoPoint _CCTV_UTMK = geaTrans.Convert(GeoTrans.GEO, GeoTrans.UTMK, _CCTVPosition);

        _CCTV_UTMK.east += _eastDiff;
        _CCTV_UTMK.north += _northDiff;

        _CCTV_UTMK = geaTrans.Convert(GeoTrans.UTMK, GeoTrans.GEO, _CCTV_UTMK);

        return _CCTV_UTMK;
    }
}
