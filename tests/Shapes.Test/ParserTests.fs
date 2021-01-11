module ParserTests

open System
open Expecto
open Parser

[<Tests>]
let tests =
    testList "parse tests" [
        testCase "parses big endian int" <| fun _ ->
            let b1 = ArraySegment([|0b0000uy; 0b0001uy; 0b0010uy; 0b1100uy|])
            // let b2 = ArraySegment([|0b0000uy; 0b0000uy; 0b1001uy; 0b0110uy|])

            let result = run intBytes b1

            Expect.equal 300 (fst result) "successful parse of 300"
    ]