
module MainFileHeader

open ShapeType
// 100 bytes long total

type HeaderBounds =
    { XMin : double 
      YMin : double
      XMax : double
      YMax : double
      ZMin : Option<double>
      ZMax : Option<double>
      MMin : Option<double>
      MMax : Option<double> }


type MainFileHeader = 
    { FileCode : int
      FileLength : int
      Version : int 
      ShapeType : ShapeType 
      Bounds : HeaderBounds }
