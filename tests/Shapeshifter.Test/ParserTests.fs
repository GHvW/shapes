module ParserTests

open System
open Expecto
open Parser

[<Tests>]
let tests =
    testList "parse tests" [
        testCase "parses big endian int" <| fun _ ->
            let bytes = ArraySegment([| 0b00000000uy; 0b00000000uy; 0b00100011uy; 0b00101000uy |])

            let (result, _) = Option.get (Parser.bigInt bytes)

            Expect.equal 9000 result "successful parse of 9000"
    ]