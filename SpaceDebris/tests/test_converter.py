import pytest
from converter import dms_to_dd, llh_to_ecef, ecef_distance

def test_northern_hemisphere():
    assert dms_to_dd("11°37'44N") == 11.628889

def test_western_hemisphere():
    assert dms_to_dd("145°50'21W") == -145.839167

def test_southern_hemisphere():
    assert dms_to_dd("12°30'00S") == -12.5

def test_west_zero_minutes_seconds():
    assert dms_to_dd("74°00'00W") == -74.0

def test_eastern_hemisphere():
    assert dms_to_dd("120°45'30E") == 120.758333

def test_near_equator():
    assert dms_to_dd("00°00'01N") == 0.000278

def test_invalid_format():
    with pytest.raises(ValueError):
        dms_to_dd("invalid")

def test_llh_to_ecef_direct_coordinates():
    x, y, z = llh_to_ecef(11.6288890, -145.8391670, 1.79559)
    assert x == -5171510.07
    assert y == -3509389.05
    assert z == 1277581.60

def test_llh_to_ecef_with_dms_conversion():
    lat = dms_to_dd("90°3'28N")
    lon = dms_to_dd("143°42'18W")
    x, y, z = llh_to_ecef(lat, lon, 1.24593)
    assert x == 5202.37
    assert y == 3820.82
    assert z == 6357994.99

def test_ecef_distance_zero():
    """Test distance between identical points is zero."""
    point = (100.0, 200.0, 300.0)
    assert ecef_distance(point, point) == 0.0

def test_ecef_distance_simple():
    """Test distance with simple coordinate values."""
    point1 = (0.0, 0.0, 0.0)
    point2 = (3.0, 4.0, 0.0)
    assert ecef_distance(point1, point2) == 5.0

def test_ecef_distance_real_coordinates():
    """Test distance between actual ECEF coordinates."""
    # Use coordinates from previous test cases
    point1 = (-6625344.32, -4495961.62, 1639159.98)
    point2 = (6214.0, 4563.8, 7602678.43)
    expected = 9989787.14
    assert round(ecef_distance(point1, point2), 2) == expected
