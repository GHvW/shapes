// https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions
module Parser

open System
open System.IO
open System.Buffers.Binary
open Shapes
open Endian


// use BinaryPrimitives class, https://docs.microsoft.com/en-us/dotnet/api/system.buffers.binary.binaryprimitives?view=net-5.0
// Main file header, first 4 bytes = 9994. can find endianness based on that

// TODO - these might not work, come back to this
// type ReaderParser<'a> =  BinaryReader -> Option<'a * BinaryReader>

// using implicit operators from F#
// let inline readonlyspan (x : ^a) : ReadOnlySpan<'a> = 
//     (^a : (static member op_Implicit : ^a -> ReadOnlySpan<'a>) x)

// type ArrayParser<'a> = (int * ArraySegment<byte>) -> Option<'a * (int * ArraySegment<byte>)>
// type Parser<'a> = (int * ReadOnlySpan<byte>) -> Option<'a * (int * ReadOnlySpan<byte>)
// type Parser<'a> = ReadOnlySpan<byte> -> Option<'a * ReadOnlySpan<byte>>
type Parser<'a> = ArraySegment<byte> -> Option<'a * ArraySegment<byte>>

// type Parser<'a> =
//     | Reader of ReaderParser<'a>
//     | Array of ArrayParser<'a>


// type Input =
//     | BinaryReader of BinaryReader
//     | ByteArray of int * ArraySegment<byte>


type ReadBytes =
    | Int
    | Double


let item readType : Parser<ArraySegment<byte>> =
    fun (segment) ->
        // let (index, span) = state
        match readType with
        | Int ->
            try
                Some(segment.Slice(0, 4), segment.Slice(4))
            with
                | _ -> None
        | Double ->
            try
                Some(segment.Slice(0, 8), segment.Slice(8))
            with
                | _ -> None


let intBytes : Parser<ArraySegment<byte>> = item Int


let doubleBytes : Parser<ArraySegment<byte>> = item Double


// TODO - same as above
let bind (f : 'a -> Parser<'b>) (parser : Parser<'a>) : Parser<'b> =
    fun (segment) ->
        segment
        |> parser
        |> Option.bind (fun (item, reader') -> 
            (f item) reader')



let map (f : 'a -> 'b) (parser : Parser<'a>) : Parser<'b> =
    fun arr ->
        arr
        |> parser
        |> Option.map (fun (item, reader') ->
            (f item, reader'))


let zero : Parser<'a> = fun (arr : ArraySegment<byte>) -> None


let ret it : Parser<'a> = fun (arr : ArraySegment<byte>) -> Some(it, arr)


let apply (p : Parser<'a -> 'b>) (q : Parser<'a>) : Parser<'b> =
    p |> bind (fun f -> q |> map f)


type ParserBuilder() =
    member this.Bind(parser, f) = bind f parser
    member this.Return(item) = ret item
    member this.Zero() = zero


let shapeParse = ParserBuilder()


// I don't know how to not copy paste these. byref errors
let convertInt endian (bytes : ArraySegment<byte>) : int =
    let span = ReadOnlySpan<byte>.op_Implicit(bytes)

    match endian with
    | Little -> BinaryPrimitives.ReadInt32LittleEndian span
    | Big -> BinaryPrimitives.ReadInt32BigEndian span


let convertDouble endian (bytes : ArraySegment<byte>) : double =
    let span = ReadOnlySpan<byte>.op_Implicit(bytes)

    match endian with
    | Little -> BinaryPrimitives.ReadDoubleLittleEndian span
    | Big -> BinaryPrimitives.ReadDoubleBigEndian span
// end byref sensitive code


let pInt (converter : ArraySegment<byte> -> int) : Parser<int> =
    intBytes
    |> map converter


let pDouble (converter : ArraySegment<byte> -> double) : Parser<double> = 
    doubleBytes 
    |> map converter


let point readDouble : Parser<Point> = shapeParse {
    let! x = readDouble
    let! y = readDouble
    return { X = x; Y = y }
}


let boundingBox readDouble : Parser<BoundingBox> = shapeParse {
    let! xMin = readDouble
    let! yMin = readDouble
    let! xMax = readDouble
    let! yMax = readDouble

    return { 
        XMin = xMin; 
        YMin = yMin; 
        XMax = xMax; 
        YMax = yMax 
    }
}


// let both p q =
//     p
//     |> bind (fun thep ->
//         q
//         |> bind (fun theq -> ret [thep; theq]))
let both p q = shapeParse {
    let! first = p
    let! second = q
    return [first; second]
}


let take count (parser : Parser<'a>) : Parser<List<'a>> = 
    seq { 0 .. count }
    |> Seq.map (fun _ -> parser)
    |> Seq.fold (fun result p -> shapeParse {
        let! list = result
        let! next = p
        return next::list}) zero

// bind takes?  

let run (p : Parser<'a>) (input : ArraySegment<byte>) = p input
    