module Sharpino.Invoice.Commands
open Sharpino.Invoice.Invoice
open Sharpino.Invoice.Events
open Sharpino.Core
open FsToolkit.ErrorHandling

type InvoiceCommand =
    | RaiseInvoice of InvoiceState
    | EmailReceipt of EmailReceipt
    | ReceivePayment of Payment
    | FinalizeInvoice of InvoiceState
        interface AggregateCommand<Invoice, InvoiceEvent>  with
            member this.Execute invoice =
                match this with
                | RaiseInvoice data -> 
                    invoice.RaiseInvoice data
                    |> Result.map (fun x -> (x , [InvoiceRaised data]))
                | EmailReceipt data ->
                    invoice.EmailReceipt data
                    |> Result.map (fun x -> (x , [InvoiceEmailed data]))
                | ReceivePayment data ->
                    invoice.ReceivePayment data
                    |> Result.map (fun x -> (x , [PaymentReceived data]))
                | FinalizeInvoice data ->
                    invoice.FinalizeInvoice data
                    |> Result.map (fun x -> (x , [InvoiceFinalized data]))
            member this.Undoer = None