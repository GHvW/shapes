// https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions
module Parser

open System
open System.IO


// TODO - these might not work, come back to this
type ReaderParser<'a> =  BinaryReader -> Option<'a * BinaryReader>


type ArrayParser<'a> = (int * ArraySegment<byte>) -> Option<'a * (int * ArraySegment<byte>)>


// type Parser<'a> =
//     | Reader of ReaderParser<'a>
//     | Array of ArrayParser<'a>


type Input =
    | BinaryReader of BinaryReader
    | ByteArray of int * ArraySegment<byte>


type ReadBytes =
    | Int
    | Double


let readerItem readBytes : ReaderParser<byte[]> = 
    fun (reader : BinaryReader) -> 
        match readBytes with
        | Int -> 
            try
                Some(reader.ReadBytes(4), reader)
            with
                | _ -> None
        | Double -> 
            try
                Some(reader.ReadBytes(8), reader)
            with
                | _ -> None


let arrayItem readBytes : ArrayParser<ArraySegment<byte>> =
    fun (state : int * ArraySegment<byte>) ->
        let (index, arr) = state
        match readBytes with
        | Int -> 
            try
                Some(arr.Slice(index, 4), (index + 4, arr))
            with
                | _ -> None
        | Double -> 
            try
                Some(arr.Slice(index, 8), (index + 8, arr))
            with
                | _ -> None


let readerInt : ReaderParser<byte[]> = readerItem Int


let readerDouble : ReaderParser<byte[]> = readerItem Double


let arrayInt = arrayItem Int


let arrayDouble = arrayItem Double



// let takeIntReader (reader : BinaryReader) : byte[] * BinaryReader = 
//     (reader.ReadBytes(4), reader)

// let takeIntArray (arr : byte[]) : byte[] * byte[] =
//     (Array.get 4 arr, arr)


let bindRead (parser : ReaderParser<'a>) (f : 'a -> ReaderParser<'b>) : ReaderParser<'b> =
    fun (reader : BinaryReader) ->
        reader
        |> parser
        |> Option.bind (fun (item, reader') -> 
            (f item) reader')

// let bindArray (parser : ArrayParser<'a>) (f : 'a -> ArrayParser<'b>) : ArrayParser<'b> =


// let takeDouble (reader : BinaryReader) : byte[] =
//     reader.ReadBytes(8)



// change to immutable?
let reverseBytes (bytes : byte[]) : byte[] =
    Array.Reverse(bytes)
    bytes


let littleEndianInt (bytes : byte[]) : int =
    BitConverter.ToInt32(bytes, 0)


let littleEndianDouble (bytes: byte[]) : double =
    BitConverter.ToDouble(bytes, 0)


let bigEndianInt (bytes : byte[]) : int =
    BitConverter.ToInt32(reverseBytes bytes, 0)


let bigEndianDouble (bytes : byte[]) : double =
    BitConverter.ToDouble(reverseBytes bytes, 0)

