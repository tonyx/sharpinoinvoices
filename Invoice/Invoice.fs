namespace Sharpino.Invoice
open Sharpino.Invoice.Commons
open System 
open FsToolkit.ErrorHandling
open Sharpino.Core
open Sharpino

module Invoice =
    type InvoiceState =
       { Amount: decimal
         InvoiceNumber: int
         Payer: string
         EmailedTo: Set<string>
         Payments: Set<string>
         AmountPaid: decimal }

     type Payment = { PaymentId: string; Amount: decimal }

     type EmailReceipt =
       { IdempotencyKey: string
         Recipient: string
         SentAt: System.DateTimeOffset }

    type State = 
    | Initial
    | Raised of InvoiceState
    | Finalized of InvoiceState

    type Invoice =
        { 
            Id: Guid
            InvoiceState : State
        } 
        with 
            static member MkNewInvoice() = 
                { 
                    Id = Guid.NewGuid(); 
                    InvoiceState = Initial 
                }
            member this.RaiseInvoice (data: InvoiceState) = 
                result 
                    {
                        let! stateMustBeInitial =
                            this.InvoiceState = Initial
                            |> Result.ofBool "Invoice must be in Initial state to raise it"
                        return 
                            { Id = this.Id; InvoiceState = 
                                Raised 
                                    {
                                        Amount = data.Amount
                                        InvoiceNumber = data.InvoiceNumber
                                        Payer = data.Payer
                                        EmailedTo = Set.empty
                                        Payments = Set.empty
                                        AmountPaid = 0m
                                    }
                            }
                    }
            member this.EmailReceipt (email: EmailReceipt) = 
                result 
                    {
                        let! stateMustBeRaised =
                            match this.InvoiceState with
                            | Raised invoice -> Ok invoice
                            | _ -> Error "Invoice must be in Raised state to email receipt"
                        return 
                            { Id = this.Id; InvoiceState = 
                                Raised 
                                    {
                                        stateMustBeRaised with
                                            EmailedTo = stateMustBeRaised.EmailedTo.Add email.Recipient
                                    }
                            }
                    }
            member this.ReceivePayment (payment: Payment) = 
                result 
                    {
                        let! stateMustBeRaised =
                            match this.InvoiceState with
                            | Raised invoice -> Ok invoice
                            | _ -> Error "Invoice must be in Raised state to receive payment"
                        return 
                            { Id = this.Id; InvoiceState = 
                                Raised 
                                    {
                                        stateMustBeRaised with
                                            Payments = stateMustBeRaised.Payments.Add payment.PaymentId
                                            AmountPaid = stateMustBeRaised.AmountPaid + payment.Amount
                                    }
                            }
                    }

            member this.FinalizeInvoice (invoice: InvoiceState) = 
                result 
                    {
                        let! stateMustBeRaised =
                            this.InvoiceState = Raised invoice
                            |> Result.ofBool "Invoice must be in Raised state to finalize it"
                        return 
                            { Id = this.Id; InvoiceState = Finalized invoice }
                    }        
            static member Deserialize(x: string) =
                globalSerializer.Deserialize<Invoice> x

            static member StorageName = "_invoice"
            static member Version = "_01"
            static member SnapshotsInterval = 15

            member this.Serialize =
                globalSerializer.Serialize this 

            interface Aggregate<string> with
                member this.Id = this.Id
                member this.Serialize = this.Serialize