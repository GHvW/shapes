module RecordShape

open Shapes

type RecordShape =
    | Point of Point
    | Polygon of Polygon


let asRecordShape (shape : 'a) : Option<RecordShape> =
    match shape with
    | Point p -> Some(Point(p))
    | Polygon p -> Some(Polygon(p))
    | _ -> None
