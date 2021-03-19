module ShapefileRecords

open ShapeType
open RecordHeader

type RecordContent<'a> =
    { ShapeType: ShapeType
      Shape: 'a }


type ShapefileRecord<'a> =
    { RecordHeader: RecordHeader
      RecordContent: RecordContent<'a> }
