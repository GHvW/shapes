
module RecordHeader

// Big endian byte order
type RecordHeader =
    { RecordNumber : int
      ContentLength : int }