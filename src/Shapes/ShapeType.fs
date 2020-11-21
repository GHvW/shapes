
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

let resolveShapeType (n : int) : ShapeType =
    match n with
    | 0 -> Null
    | 1 -> Point
    | 3 -> PolyLine
    | 5 -> Polygon
    | 8 -> MultiPoint
    | 11 -> PointZ
    | 13 -> PolyLineZ
    | 15 -> PolygonZ
    | 18 -> MultiPointZ
    | 21 -> PointM
    | 23 -> PolyLineM
    | 25 -> PolygonM
    | 28 -> MultiPointM
    | 31 -> MultiPatch
    | _ -> Null // TODO - do something about this later?