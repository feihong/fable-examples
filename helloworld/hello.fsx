(*
This is a simple "hello world" application.

Source: https://github.com/fable-compiler/fable-arch/tree/master/samples/helloworld
*)

#r "node_modules/fable-core/Fable.Core.dll"
#load "node_modules/fable-arch/Fable.Arch.Html.fs"
#load "node_modules/fable-arch/Fable.Arch.App.fs"
#load "node_modules/fable-arch/Fable.Arch.Virtualdom.fs"

open System

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
  let name = model.name.Trim()
  let greeting = if name = "" then "" else sprintf "Hello %s!" name
  let reverse =
    if name = ""
    then ""
    else name.ToCharArray() |> Array.rev |> String |> sprintf "Reverse: %s"
  let len = if name = "" then "" else sprintf "Length: %d" name.Length
  div
    []
    [
      label [] [text "Enter name: "]
      input [
        onInput' ChangeInput
        attribute "autofocus" ""
      ]
      div [classy "greeting"] [text greeting]
      div [classy "reverse"] [text reverse]
      div [classy "length"] [text len]
    ]

// Using createSimpleApp instead of createApp since our
// update function doesn't generate any actions. See
// some of the other more advanced examples for how to
// use createApp. In addition to the application functions
// we also need to specify which renderer to use.
createSimpleApp initModel view update Virtualdom.createRender
|> withStartNodeSelector "#app"
|> start
