
module MainFileHeader

open ShapeType
// 100 bytes long total

type Header = 
    { FileCode : int
      FileLength : int
      Version : int 
      ShapeType : ShapeType
      XMin : double 
      YMin : double
      XMax : double
      YMax : double
      ZMin : double
      ZMax : double
      MMin : double
      MMax : double }