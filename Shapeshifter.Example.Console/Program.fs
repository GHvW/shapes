// Learn more about F# at http://fsharp.org

open System
open System.IO
open Parser

[<EntryPoint>]
let main argv =
    printfn "Hello Shapeshifter!"
    //let path = @"C:\Users\ghvw\projects\csharp\compass\shapefile\supercool.shp";
    let path = @"C:\Users\ghvw\projects\rust\clay\examples\defaultish\defautish.shp";
    
    let bytes = ArraySegment<byte>.op_Implicit(File.ReadAllBytes(path))
    
    printfn "There are %A bytes in the shapefile" bytes.Count
    
    let (main, rest) = (mainFileHeader bytes) |> Option.get
    printfn "main header is %A" main

    let readAll (parser : Parser<'a>) : ArraySegment<byte> -> list<'a> =
        let rec inner agg parse b =
            match parse b with
            | None -> agg
            | Some (data, rest_) -> inner (data::agg) parse rest_

        fun arr ->
            inner [] parser arr
            
    let result = readAll (polygon |> asRecordContent |> shapefileRecord) rest

    printfn "result: %A" result

    0 // return an integer exit code
