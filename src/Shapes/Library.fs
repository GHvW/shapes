// everything is an int or a double
// file header data and record contents in main file are little endian (Intel / PC) byte order
// everything else is big endian (Sun / Motorola) byte order
namespace Shapes

module Say =
    let hello name =
        printfn "Hello %s" name
