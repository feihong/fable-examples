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
    | ToggleTimeDisplay

type Model =
    { Datetime: DateTime
      ShowMilitary: bool
    }
    // Static member giving back an init model
    static member init = { Datetime = DateTime.MinValue; ShowMilitary = false }

let update model action =
    match action with
    // Tick are push by the producer
    | Tick datetime ->
      { model with Datetime = datetime }, []
    | ToggleTimeDisplay ->
      { model with ShowMilitary = not model.ShowMilitary }, []

let view model =
  let timeFormat =
    if model.ShowMilitary
    then "HH:mm:ss"
    // Unfortunately, "tt" does not show AM/PM.
    else "hh:mm:ss " + (if model.Datetime.Hour >= 12 then "PM" else "AM")

  div [] [
    text <| model.Datetime.ToString("yyyy-MM-dd")
    br []
    text <| model.Datetime.ToString(timeFormat)
    div [] [
      button [
        classy "btn btn-primary"
        onMouseClick (fun _ -> ToggleTimeDisplay)
      ] [text <| if model.ShowMilitary then "Regular time" else "Military time"]
    ]
  ]

// Producer used to send the current DateTime every second
let tickProducer push =
    //window.setInterval((fun _ ->
    //    Tick DateTime.Now |> push
    //), 1000) |> ignore
    let timer = new System.Timers.Timer(1000.0)
    timer.AutoReset <- true
    timer.Elapsed.Add (fun _ ->
      Tick DateTime.Now |> push)

    // Force the first to push to have immediate effect
    // If we don't do that there is one second before the first push
    // and the view is rendered with the Model.init values
    Tick DateTime.Now |> push

// Create and run our application
createApp Model.init view update (Virtualdom.createRender)
|> withStartNodeSelector "#app"
|> withProducer tickProducer    // attach our producer to the app
|> start
