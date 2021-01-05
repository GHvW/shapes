module ShapeInit

open System
open Parser
open Endian
open System.Buffers.Binary


let determineEndiannes (bytes : ArraySegment<byte>) : Result<SystemEndianness, string> =
    let span = ReadOnlySpan<byte>.op_Implicit(bytes)
    let fileCodeBig = BinaryPrimitives.ReadInt32BigEndian(span)
    let fileCodeLittle = BinaryPrimitives.ReadInt32LittleEndian(span)

    if fileCodeBig = 9994 then // 9994 is the file code for a shape file
        Ok(Big)
    else if fileCodeLittle = 9994 then
        Ok(Little)
    else
        Error("file is not a shapefile")
    

