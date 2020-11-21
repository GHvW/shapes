// https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/computation-expressions
open System
open System.IO

module Parser =

    let takeInt (reader : BinaryReader) : byte[] = 
        reader.ReadBytes(4)


    let takeDouble (reader : BinaryReader) : byte[] =
        reader.ReadBytes(8)


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