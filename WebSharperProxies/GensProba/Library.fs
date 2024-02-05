namespace GensProba

open WebSharper
open Fable.Core
open Fable.Core.JsInterop
[<JavaScript>]
module Client =

    [<SPAEntryPoint>]
    let Main () =
        let probaObj =
            seq {
                "a" ==> 5
                "b" ==> 8
            }
            |> JsInterop.createObj
        ()