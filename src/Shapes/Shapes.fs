
module Shapes

type Point = 
    { X : double 
      Y : double }

type MultiPoint =
    { BoundingBox : double[] // size 4 array, [Xmin, Ymin, Xmax, Ymax]
      NumPoints : int
      Points : Point[] } // size NumPoints array

type PolyLine =
    { BoundingBox : double[] // size 4 array, [Xmin, Ymin, Xmax, Ymax]
      NumParts : int
      NumPoints : int
      Parts : int[] // size NumParts array
      Points : Point[] } // size NumPoints array


type Polygon =
    { BoundingBox : double[] // size 4 array, [Xmin, Ymin, Xmax, Ymax]
      NumParts : int
      NumPoints : int
      Parts : int[] // size NumParts array
      Points : Point[] } // size NumPoints array

// compose with Point instead?
type PointM =
    { X : double
      Y : double
      Measure : double }

// compose?
type MultiPointM =
    { BoundingBox : double[]
      NumPoints : int
      Points : Point[]
      BoundingMeasureRange : double[]
      Measures : double[] }

// compose?
type PolyLineM =
    { BoundingBox : double[] // size 4 array, [Xmin, Ymin, Xmax, Ymax]
      NumParts : int
      NumPoints : int
      Parts : int[] // size NumParts array
      Points : Point[] 
      BoundingMeasureRange : double[]
      Measures : double[] } // size NumPoints array

// compose?
type PolygonM =
    { BoundingBox : double[] // size 4 array, [Xmin, Ymin, Xmax, Ymax]
      NumParts : int
      NumPoints : int
      Parts : int[] // size NumParts array
      Points : Point[] 
      BoundingMeasureRange : double[]
      Measures : double[] } // size NumPoints array

type PointZ =
    { X : double
      Y : double
      Z : double
      Measure : double }


type MultiPointZ =
    { BoundingBox : double[]
      NumPoints : int
      Points : Point[]
      BoundingZRange : double[]
      ZValues : double[]
      BoundingMeasureRange : double[]
      Measures : double[] }

type PolyLineZ =
    { BoundingBox : double[] // size 4 array, [Xmin, Ymin, Xmax, Ymax]
      NumParts : int
      NumPoints : int
      Parts : int[] // size NumParts array
      Points : Point[] 
      BoundingZRange : double[]
      ZValues : double[]
      BoundingMeasureRange : double[]
      Measures : double[] } // size NumPoints array


type PolygonZ =
    { BoundingBox : double[] // size 4 array, [Xmin, Ymin, Xmax, Ymax]
      NumParts : int
      NumPoints : int
      Parts : int[] // size NumParts array
      Points : Point[] 
      BoundingZRange : double[]
      ZValues : double[]
      BoundingMeasureRange : double[]
      Measures : double[] } // size NumPoints array


type MultiPatchPartType =
    | TriangleStrip
    | TriangleFan
    | OuterRing
    | InnerRing
    | FirstRing
    | Ring

let resolveMultiPatchPartType (n : int) : Option<MultiPatchPartType> =
    match n with
    | 0 -> Some(TriangleStrip)
    | 1 -> Some(TriangleFan)
    | 2 -> Some(OuterRing)
    | 3 -> Some(FirstRing)
    | 5 -> Some(Ring)
    | _ -> None

type MultiPatch = 
    { BoundingBox : double[]
      NumParts : int
      NumPoints : int
      Parts : int[]
      PartTypes : int[] 
      Points : Point[]
      BoundingZRange : double[]
      ZValues : double[]
      BoundingMeasureRange : double[]
      Measures : double[] }