
module ShapeType

type ShapeType =
    | Null
    | Point
    | PolyLine
    | Polygon
    | MultiPoint
    | PointZ
    | PolyLineZ
    | PolygonZ
    | MultiPointZ
    | PointM
    | PolyLineM
    | PolygonM
    | MultiPointM
    | MultiPatch

let resolveShapeType (n : int) : Option<ShapeType> =
    match n with
    | 0 -> Some(Null)
    | 1 -> Some(Point)
    | 3 -> Some(PolyLine)
    | 5 -> Some(Polygon)
    | 8 -> Some(MultiPoint)
    | 11 -> Some(PointZ)
    | 13 -> Some(PolyLineZ)
    | 15 -> Some(PolygonZ)
    | 18 -> Some(MultiPointZ)
    | 21 -> Some(PointM)
    | 23 -> Some(PolyLineM)
    | 25 -> Some(PolygonM)
    | 28 -> Some(MultiPointM)
    | 31 -> Some(MultiPatch)
    | _ -> None 