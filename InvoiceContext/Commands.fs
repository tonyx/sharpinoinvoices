
module Sharpino.Invoices.Commands
open Sharpino.Invoices.Invoices
open Sharpino.Core
open FsToolkit.ErrorHandling
open System
open Sharpino.Invoices.Events.Events

type InvoicesCommand =
    | AddInvoiceRef of Guid
    | RemoveInvoieRef of Guid
        interface Command<Invoices, InvoicesEvents>  with
            member this.Execute invoice =
                match this with
                | AddInvoiceRef id -> 
                    invoice.AddInvoiceRef id
                    |> Result.map (fun x -> (x , [InvoiceAdded id]))
                | RemoveInvoieRef data ->
                    invoice.RemoveInvoiceRef data
                    |> Result.map (fun x -> (x , [InvoiceRemoved data]))
            member this.Undoer = None