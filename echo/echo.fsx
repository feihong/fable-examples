(**
 - title: Echo sample - getting started with ajax
 - tagline: Simple ajax demonstration with fable-arch
 - app-style: width:800px; margin:20px auto 50px auto;
 - intro: This is a sample showing how to use ajax calls.
*)

#r "node_modules/fable-core/Fable.Core.dll"
#r "node_modules/fable-powerpack/Fable.PowerPack.dll"
#load "node_modules/fable-arch/Fable.Arch.Html.fs"
#load "node_modules/fable-arch/Fable.Arch.App.fs"
#load "node_modules/fable-arch/Fable.Arch.Virtualdom.fs"

open Fable.Core
open Fable.Core.JsInterop
open Fable.Import.Browser

open Fable.Arch
open Fable.Arch.App
open Fable.Arch.Html
open Fable.PowerPack
open Fable.PowerPack.Fetch


/// Helpers for working with input element from VirtualDom
let inline onInput x = onEvent "oninput" (fun evt -> x (unbox evt?target?value))

let ajax cb (data: string) =
  let url = sprintf "/upper/?text=%s" data
  //fetch url []
  //|> Promise.bind (fun res -> res.text())
  //|> Promise.map cb
  //|> ignore
  promise {
    let! res = fetch url []
    return! res.text()
  }
  |> Promise.map cb
  |> ignore


/// DU used to discriminate the State of the request
type Status =
  | None
  | Pending
  | Done

// Model
type Model =
  { InputValue: string
    ServerResponse: string
    Status: Status
  }

  static member Init =
    { InputValue = "It's zumba time!"
      ServerResponse = ""
      Status = None
    }

type Actions =
  | NoOp
  | ChangeInput of string
  | SendEcho
  | ServerResponse of string
  | SetStatus of Status

// Update
let update model action =
  let model' =
    match action with
    // On input change with update the model value
    | ChangeInput str ->
      { model with InputValue = str }
    // When we got back a server response, we update the value of the server and set the state to Done
    | ServerResponse str ->
      { model with
          ServerResponse = str
          Status = Done }
    // Set the status in the model
    | SetStatus newStatus ->
      { model with Status = newStatus }
    | _ -> model

  // Code used to support delayed code like ajax for example
  let delayedCall h =
    match action with
    | SendEcho ->
      // Send a fake ajax
      // First is a callback to execute when done
      // Second is the data to send
      ajax (ServerResponse >> h) model.InputValue
      // We just sent an ajax request so make the state pending in the model
      h (SetStatus Pending)
    | _ -> ()

  // We return the model, and a list of Actions to execute
  model', delayedCall |> toActionList

// View
let view model =
  // Choose what we want to display on output
  let infoText =
    match model.Status with
    | None -> text ""
    | Pending -> text "Waiting Server response"
    | Done -> text (sprintf "The server response is: %s" model.ServerResponse)

  div [] [
    label [] [text "Enter a sentence: "]
    br []
    form [] [
      input [
        onInput ChangeInput
        attribute "autofocus" ""
        property "value" model.InputValue
      ]
      div [] [
        button [
          onMouseClick (fun _ -> SendEcho)
          classy "btn btn-default"
        ] [ text "Uppercase by server" ]
      ]
    ]
    p [] [ infoText ]
  ]

/// Create our application
createApp Model.Init view update Virtualdom.createRender
|> withStartNodeSelector "#app"
|> start
