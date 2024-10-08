module Sharpino.Invoice.Commons

open System
open System.Data
open System.Globalization
open MBrace.FsPickler.Json
open MBrace.FsPickler


type Serialization<'F> =
    abstract member Deserialize<'A> : 'F -> Result<'A, string>
    abstract member Serialize<'A> : 'A -> 'F

let jsonPickler = FsPickler.CreateJsonSerializer(indent = false)
let binaryPickle = FsPickler.CreateBinarySerializer()

let jsonPSerializer =
    { new Serialization<string> with
        member this.Deserialize<'A> json =
            try
                jsonPickler.UnPickleOfString<'A> json |> Ok
            with
            | ex -> Error ex.Message
        member this.Serialize<'A> (obj: 'A) =
            jsonPickler.PickleToString obj
    }
    
let binarySerializer = 
    { new Serialization<byte[]> with
        member this.Deserialize<'A> (bytes: byte[]) =
            try
                binaryPickle.UnPickle<'A> bytes |> Ok
            with
            | ex -> Error ex.Message
        member this.Serialize<'A> (obj: 'A) =
            binaryPickle.Pickle obj
    }

// let globalSerializer: Serialization<_> = binarySerializer
let globalSerializer: Serialization<_> = jsonPSerializer

