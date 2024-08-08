module Sharpino.Invoice.Events
open Sharpino.Invoice.Invoice
open Sharpino.Core
open Sharpino
open Commons

type InvoiceEvent =
    | InvoiceRaised of InvoiceState
    | InvoiceEmailed of EmailReceipt
    | PaymentReceived of  Payment
    | InvoiceFinalized of InvoiceState
        interface Event<Invoice>  with
            member this.Process invoice =
                match this with
                | InvoiceRaised data -> 
                    invoice.RaiseInvoice data
                | InvoiceEmailed data ->
                    invoice.EmailReceipt data
                | PaymentReceived data ->
                    invoice.ReceivePayment data
                | InvoiceFinalized data ->
                    invoice.FinalizeInvoice data
        member this.Serialize = 
            globalSerializer.Serialize this
        static member Deserialize x = 
            globalSerializer.Deserialize<InvoiceEvent> x