
namespace Sharpino.Invoices
open Sharpino.Invoice.Commons
open System 
open FSharpPlus
open FsToolkit.ErrorHandling
open Sharpino.Core
open Sharpino

module Invoices =
    type Invoices =
        {
            InvoicesRefs: List<Guid>
        }
        with 
            member this.AddInvoiceRef id =
                { this with InvoicesRefs = id :: this.InvoicesRefs } |> Ok
            member this.RemoveInvoiceRef id =
                { this with InvoicesRefs = this.InvoicesRefs |> List.filter (fun x -> x <> id) } |> Ok

            static member Zero = 
                {
                    InvoicesRefs = []
                }
            static member StorageName =
                "_invoices"
            static member Version =
                "_01"
            static member SnapshotsInterval =
                15
            static member Deserialize x =
                globalSerializer.Deserialize<Invoices> x
            member this.Serialize  =
                this
                |> globalSerializer.Serialize 