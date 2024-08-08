namespace Sharpino.Invoice.Api
open Sharpino
open Sharpino.Core
open Sharpino.CommandHandler
open Sharpino.Invoice.Invoice
open Sharpino.Invoice.Events
open Sharpino.Invoice.Commands
open Sharpino.Invoices.Invoices
open System
open Sharpino.Storage
open Sharpino.Invoices.Events.Events

module InvoicesApi =
    open Sharpino.Invoices.Commands
    open Sharpino.Core

    let doNothingBroker: IEventBroker<_> =
        {
            notify = None
            notifyAggregate = None
        }

    type InvoicesApi(eventStore: IEventStore<string>) =
        let invoicesStateViewer = getStorageFreshStateViewer<Invoices, InvoicesEvents, string> eventStore
        let invoiceStateViewer = getAggregateStorageFreshStateViewer<Invoice, InvoiceEvent, string> eventStore

        member this.CreateInvoice(invoice: Invoice) = 
            result {
                let addInvoices: Command<Invoices, InvoicesEvents> = InvoicesCommand.AddInvoiceRef invoice.Id
                let result =
                    runInitAndCommand eventStore doNothingBroker invoice addInvoices 
                return! result
            }

        member this.GetInvoice(id: Guid) = 
            result {
                let! (_, invoices) = invoicesStateViewer()

                let! invoiceIsReference =
                    invoices.InvoicesRefs |> List.contains id
                    |> Result.ofBool "Invoice not found"

                let! (_, invoice) = invoiceStateViewer id
                return invoice
            }

        member this.RaiseInvoice(id: Guid, data: InvoiceState) = 
            result {
                let! (_, invoices) = invoicesStateViewer()

                let! invoiceIsReferenced =
                    invoices.InvoicesRefs |> List.contains id
                    |> Result.ofBool "Invoice not found"

                let raiseInvoice: AggregateCommand<Invoice, InvoiceEvent> = InvoiceCommand.RaiseInvoice data
                
                return! 
                    raiseInvoice
                    |> 
                    runAggregateCommand id eventStore doNothingBroker 
            }

            
