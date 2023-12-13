namespace WebSharper.Feliz

open WebSharper
open WebSharper.JavaScript
open WebSharper.JavaScript.Interop

[<Proxy("Fable.Core.JsInterop, Fable.Core")>]
module private JsInteropProxies =
    [<Inline>] 
    let inline (!!) x = unbox x
    
    [<Inline>]
    let inline createObj<'b when 'b :> seq<string*obj>> (fields: 'b) = New fields

    [<Inline>]
    let inline importDefault<'T0> path = JS.ImportDefault<'T0> path
    [<Inline>]
    let inline import<'T0> selector path = JS.Import<'T0>(selector, path)

    [<Proxy(typeof<Fable.React.IRefValue<_>>)>]
    type IRefValue<'T> = 
        abstract current: 'T with get,set

[<Proxy("Fable.Core.Util, Fable.Core")>]
module private FableUtilProxies =
    let inline jsNative<'T> = X<'T>