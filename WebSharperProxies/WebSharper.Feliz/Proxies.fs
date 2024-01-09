namespace WebSharper.Feliz

open WebSharper
open WebSharper.JavaScript
open WebSharper.JavaScript.Interop


[<InternalProxy("Fable.Core, Fable.Core")>]
module private CoreProxies =

    let [<Inline>] jsNative<'a> = Unchecked.defaultof<'a>
[<InternalProxy("Fable.Core.JsInterop, Fable.Core");AutoOpen>]
module private JsInteropProxies =
    [<Inline>] 
    let inline (!!) x = unbox x

    let asd = JavaScript.Dom.Event
    [<Inline>]
    let inline createObj<'b when 'b :> seq<string*obj>> (fields: 'b) = New fields


    let importDefault<'T0> path = JS.ImportDefault<'T0> path

    let import<'T0> selector path = JS.Import<'T0>(selector, path)

    [<InternalProxy(typeof<Fable.React.IRefValue<_>>)>]
    type IRefValue<'T> = 
        abstract current: 'T with get,set
        
    [<Inline "$target == null">]
    let isNullOrUndefined (target:obj) : bool = target = null
