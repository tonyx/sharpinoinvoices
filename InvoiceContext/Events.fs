
namespace Sharpino.Invoices.Events
open Sharpino.Invoice.Commons
open Sharpino.Invoices.Invoices
open System 
open FSharpPlus
open FsToolkit.ErrorHandling
open Sharpino.Core
open Sharpino

module Events =
    type InvoicesEvents =
        | InvoiceAdded of Guid
        | InvoiceRemoved of Guid
            interface Event<Invoices>  with
                member this.Process invoices =
                    match this with
                    | InvoiceAdded id -> 
                        invoices.AddInvoiceRef id
                    | InvoiceRemoved data ->
                        invoices.RemoveInvoiceRef data
            member this.Serialize = 
                globalSerializer.Serialize this
            static member Deserialize x = 
                globalSerializer.Deserialize<InvoicesEvents> x