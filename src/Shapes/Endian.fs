
module Endian

open System

type SystemEndianness =
    | Big
    | Little

let determineEndianness n : SystemEndianness =
    if n = 9994 then
        Little
    else
        Big