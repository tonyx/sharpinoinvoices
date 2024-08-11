module Tests
open Sharpino
open Sharpino.Invoice.Commons
open Sharpino.Invoice
open Sharpino.Invoice.Api

open Expecto
open Sharpino.Invoice
open Sharpino.MemoryStorage
open Sharpino.Invoice.Api.InvoicesApi
open Sharpino.Invoice.Invoice

[<Tests>]
let tests =
    testList "invoiceTests" [
        testCase "add invoices and reference" <| fun _ ->
            let eventStore = MemoryStorage()
            let invoicesApi = InvoicesApi(eventStore)
            let invoice = Invoice.MkNewInvoice()
            let invoiceId = invoice.Id
            let createInvoice = invoicesApi.CreateInvoice invoice
            Expect.isOk createInvoice "should be ok"
            let result = invoicesApi.GetInvoice invoiceId
            Expect.isOk result "should be ok"
            Expect.equal invoice result.OkValue "should be equal"
            Expect.equal invoice.InvoiceState Initial "should be equal"

        testCase "raise an invoice - Ok" <| fun _ ->
            let eventStore = MemoryStorage()
            let invoicesApi = InvoicesApi(eventStore)
            let invoice = Invoice.MkNewInvoice()
            let createInvoice = invoicesApi.CreateInvoice invoice

            let raiseInvoice = 
                invoicesApi.RaiseInvoice (invoice.Id ,
                    { Amount = 100m; 
                    InvoiceNumber = 1; 
                    Payer = "John Doe"; 
                    EmailedTo = Set.empty; 
                    Payments = Set.empty; 
                    AmountPaid = 0m })
            Expect.isOk raiseInvoice "should be ok"

            let result = invoicesApi.GetInvoice invoice.Id
            Expect.isOk result "should be ok"

            let retrievedInvoice = result.OkValue
            Expect.equal retrievedInvoice.InvoiceState (Raised { Amount = 100m; InvoiceNumber = 1; Payer = "John Doe"; EmailedTo = Set.empty; Payments = Set.empty; AmountPaid = 0m }) "should be equal"



    ]


    |> testSequenced
