
module Endian

open System

type SystemEndianness =
    | Big
    | Little

let getEndianness () : SystemEndianness =
    if BitConverter.IsLittleEndian then
        Little
    else
        Big