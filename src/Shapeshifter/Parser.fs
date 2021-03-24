// https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions
module Parser

open System
open System.IO
open System.Buffers.Binary
open System.Collections.Generic
open Shapes
open ShapeType
open MainFileHeader
open RecordHeader
open ShapefileRecords
open RecordShape


type Parser<'a> = ArraySegment<byte> -> Option<'a * ArraySegment<byte>>


let intBytes : Parser<ArraySegment<byte>> = 
    fun segment ->
        try
            Some(segment.Slice(0, 4), segment.Slice(4))
        with
            | _ -> None


let doubleBytes : Parser<ArraySegment<byte>> =
    fun segment ->
        try
            Some(segment.Slice(0, 8), segment.Slice(8))
        with
            | _ -> None


// TODO - same as above
let bind (f : 'a -> Parser<'b>) (parser : Parser<'a>) : Parser<'b> =
    fun (segment) ->
        segment
        |> parser
        |> Option.bind (fun (item, rest) -> 
            (f item) rest)



let map (f : 'a -> 'b) (parser : Parser<'a>) : Parser<'b> =
    fun arr ->
        arr
        |> parser
        |> Option.map (fun (item, rest) ->
            (f item, rest))


let zero : Parser<'a> = fun (arr : ArraySegment<byte>) -> None


let ret it : Parser<'a> = fun (arr : ArraySegment<byte>) -> Some(it, arr)


let apply (p : Parser<'a -> 'b>) (q : Parser<'a>) : Parser<'b> =
    p |> bind (fun f -> q |> map f)


type ParserBuilder() =
    member this.Bind(parser, f) = bind f parser
    member this.Return(item) = ret item
    member this.Zero() = zero

let shapeParse = ParserBuilder()


// end byref sensitive code
let parseLittleDouble (bytes : ArraySegment<byte>) : double =
    let span = ReadOnlySpan<byte>.op_Implicit(bytes)
    BinaryPrimitives.ReadDoubleLittleEndian span


let parseLittleInt (bytes : ArraySegment<byte>) : int =
    let span = ReadOnlySpan<byte>.op_Implicit(bytes)
    BinaryPrimitives.ReadInt32LittleEndian span


let parseBigInt (bytes : ArraySegment<byte>) : int =
    let span = ReadOnlySpan<byte>.op_Implicit(bytes)
    BinaryPrimitives.ReadInt32BigEndian span
    

let littleDouble : Parser<double> =
    doubleBytes |> map parseLittleDouble


let bigInt : Parser<int> =
    intBytes |> map parseBigInt


let littleInt : Parser<int> =
    intBytes |> map parseLittleInt


let point : Parser<Point> = shapeParse {
    let! x = littleDouble
    let! y = littleDouble

    return { X = x; Y = y }
}


let take count (parser : Parser<'a>) : Parser<list<'a>> = 
    seq { 0 .. (count - 1) }
    |> Seq.fold (fun result _ -> shapeParse {
        let! list = result
        let! next = parser
        return next::list}) (ret [])


let boundingBox : Parser<BoundingBox> =
    littleDouble
    |> take 4
    |> map (fun bounds ->
        match bounds with // hopefully this is safe if we were able to read with take 4
        | [yMax; xMax; yMin; xMin] -> { XMin = xMin; YMin = yMin; XMax = xMax; YMax = yMax })
        //| _ -> None


let mainFileHeaderBounds : Parser<HeaderBounds> = shapeParse {
    let! bounds = littleDouble |> take 8

    return match bounds with
           | [mmax; mmin; zmax; zmin; ymax; xmax; ymin; xmin] ->
               { XMin = xmin
                 YMin = ymin
                 XMax = xmax
                 YMax = ymax
                 ZMin = if zmin = 0.0 then None else Some(zmin)
                 ZMax = if zmax = 0.0 then None else Some(zmax)
                 MMin = if mmin = 0.0 then None else Some(mmin)
                 MMax = if mmax = 0.0 then None else Some(mmax) }
}
        

let mainFileHeader : Parser<MainFileHeader> = shapeParse {
    let! start = bigInt |> take 7
    let! version = littleInt
    let! shapeType = 
        littleInt 
        |> map (ShapeType.resolveShapeType >> (Option.defaultValue ShapeType.Null))
    let! bounds = mainFileHeaderBounds

    return match start with
           | [fileLength; _; _; _; _; _; filecode] ->
               { FileCode = filecode; FileLength = fileLength; Version = version; ShapeType = shapeType; Bounds = bounds }
}


let polygon : Parser<Polygon> = shapeParse {
    let! bounds = boundingBox
    let! partsCount = littleInt
    let! pointsCount = littleInt
    let! parts = littleInt |> take partsCount
    let! points = point |> take pointsCount

    return { BoundingBox = bounds; NumParts = partsCount; NumPoints = pointsCount; Parts = parts; Points = points }
}


let recordHeader : Parser<RecordHeader> = shapeParse {
    let! n = bigInt
    let! contentLength = bigInt
    return { RecordNumber = n; ContentLength = contentLength }
}


// TODO - constrain to only types that can be in Record Contents. Point, Polygon, Polyline, not Double, Int, etc.
let asRecordContent (parser : Parser<'a>) : Parser<RecordContent<'a>> = shapeParse {
    let! shapeType = 
        littleInt 
        |> map (resolveShapeType >> Option.defaultValue ShapeType.Null)
    let! shape = parser

    return { ShapeType = shapeType; Shape = shape }
}


let shapefileRecord (parser : Parser<RecordContent<'a>>) : Parser<ShapefileRecord<'a>> = shapeParse {
    let! header = recordHeader
    let! contents = parser
    return { RecordHeader = header; RecordContent = contents }
}
