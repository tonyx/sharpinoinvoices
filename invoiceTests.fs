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
    ]
    |> testSequenced
