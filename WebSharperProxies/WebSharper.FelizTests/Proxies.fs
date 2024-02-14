[<AutoOpen>]
module internal Proxies

open WebSharper
open WebSharper.JavaScript

[<Proxy("Promise, Fable.Promise")>]
module PromiseProxy =
    let map (a   : 'T1 -> 'T2) (pr :Fable.Core.JS.Promise<'T1>): Fable.Core.JS.Promise<'T2> =
        (As<Promise<'T1>> pr).Then(a)
        |> As
    
    let sleep (msec:int) =
        Promise(fun x -> JS.SetTimeout (fst x)  msec |> ignore)
        |> As<Fable.Core.JS.Promise<unit>>

[<Proxy("Fable.Core.JS, Fable.Core")>]
module JSProxy =
    let [<Inline>] clearInterval (handle:int):unit =
        WebSharper.JavaScript.JS.ClearInterval (As handle)
    let [<Inline>] setInterval (f: unit -> unit, msec: int): int =
        WebSharper.JavaScript.JS.SetInterval f msec |> As

[<Proxy("Fable.Core.JsInterop, Fable.Core")>]
module JsInteropProxy =
    [<Inline>]
    let inline (!^) (x: ^t1) : ^t2 =
        ((^t1 or ^t2): (static member op_ErasedCast: ^t1 -> ^t2) x)

    let importDynamic<'T> (path:string):Fable.Core.JS.Promise<'T> =
        WebSharper.JavaScript.JS.ImportDynamic<'T> path |> As
[<Proxy(typeof<Async>)>]
type AsyncProxy =
    [<Inline>]
    static member AwaitPromise(promise: Fable.Core.JS.Promise<'T>) : Async<'T> = (promise |> As) |> Promise.AsAsync

[<Proxy(typeof<System.Environment>)>]
type EnvProxy =
    static member NewLine = "\n" // hack