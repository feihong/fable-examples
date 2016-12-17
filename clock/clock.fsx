(**
A simple clock - producer demo
Source: https://github.com/fable-compiler/fable-arch/tree/master/samples/clock
*)

// Load Fable.Core and fable-arch files and bindings to JS global objects
#r "node_modules/fable-core/Fable.Core.dll"
#load "node_modules/fable-arch/Fable.Arch.Html.fs"
#load "node_modules/fable-arch/Fable.Arch.App.fs"
#load "node_modules/fable-arch/Fable.Arch.Virtualdom.fs"

open System
open Fable.Core
open Fable.Import
open Fable.Import.Browser
open Fable.Arch
open Fable.Arch.App
open Fable.Arch.Html


type Action =
    | Tick of DateTime

/// A really simple type to Store our ModelChanged
type Model =
    { Time: string      // Time: HH:mm:ss
      Date: string }    // Date: YYYY/MM/DD

    /// Static member giving back an init Model
    static member init = { Time = "00:00:00"; Date = "1970/01/01" }

/// Handle all the update of our Application
let update model action =
    match action with
    /// Tick are push by the producer
    | Tick datetime ->
        { model with
            Time = String.Format("{0:HH:mm:ss}", datetime)
            Date = String.Format("{0:yyyy-MM-dd}", datetime) }, []

/// Our application view
let view model =
    div
        []
        [ text model.Date
          br []
          text model.Time]

/// Producer used to send the current Time every second
let tickProducer push =
    window.setInterval((fun _ ->
        push(Tick DateTime.Now)
        null
    ),
        1000) |> ignore
    // Force the first to push to have immediate effect
    // If we don't do that there is one second before the first push
    // and the view is rendered with the Model.init values
    push(Tick DateTime.Now)

/// Create and run our application
createApp Model.init view update (Virtualdom.createRender)
|> withStartNodeSelector "#app"
|> withProducer tickProducer    // Attach our producer to the app
|> start
