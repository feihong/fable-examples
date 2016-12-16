(*
This is a simple "hello world" application.
*)

#r "node_modules/fable-core/Fable.Core.dll"
#load "node_modules/fable-arch/Fable.Arch.Html.fs"
#load "node_modules/fable-arch/Fable.Arch.App.fs"
#load "node_modules/fable-arch/Fable.Arch.Virtualdom.fs"

open Fable.Core
open Fable.Core.JsInterop

open Fable.Arch
open Fable.Arch.App
open Fable.Arch.Html

// Model
type Model = { name: string }

let initModel = { name = "" }

type Actions =
    | ChangeInput of string

// Update
let update model msg =
    match msg with
    | ChangeInput str -> { model with name = str }

// On input event, send the input value to the given function.
let inline onInput' fn = onInput (fun evt -> unbox evt?target?value |> fn)

// View
let view model =
  let greeting =
    if model.name = ""
    then ""
    else sprintf "Hello %s!" model.name
  div
    []
    [
      label [] [text "Enter name: "]
      input [onInput' ChangeInput]
      br []
      span [classy "greeting"] [text greeting]
    ]

// Using createSimpleApp instead of createApp since our
// update function doesn't generate any actions. See
// some of the other more advanced examples for how to
// use createApp. In addition to the application functions
// we also need to specify which renderer to use.
createSimpleApp initModel view update Virtualdom.createRender
|> withStartNodeSelector "#hello"
|> start
