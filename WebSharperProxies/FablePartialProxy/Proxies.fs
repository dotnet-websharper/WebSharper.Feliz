module internal FableProxies

open WebSharper
open WebSharper.JavaScript
    
[<Proxy("Fable.Core.Util, Fable.Core")>]
module internal UtilProxy =
    let [<Inline>] jsNative<'a> = Unchecked.defaultof<'a>


[<Proxy("Fable.Core.JsInterop, Fable.Core")>]
module internal JsInteropProxies =
    let (==>) (a:string,o:obj) : (string*obj) =
        a => o // TODO: check
    
    //! TODO: jsOptions
    //! Create a literal JS object from a mutator lambda. Normally used when
    //! the options interface has too many fields to be represented with a Pojo record.
    //! E.g. `jsOptions<MyOpt> (fun o -> o.foo <- 5)` in JS becomes `{ foo: 5 }`


    let op_Dynamic<'T>(a:obj, b:obj) : 'T =
        Pervasives.(?) a (string b)
    let op_DynamicAssignment(a:obj,b:obj,c:obj) : unit =
        a?b <- c
    let toPlainJsObj (o: 'T):obj = JS.New o
    [<Inline>] 
    let (!!) x = unbox x

    [<Inline>]
    let createObj<'b when 'b :> seq<string*obj>> (fields: 'b) = New fields

    [<Fable.Core.FableImportJs;Inline>]
    let importDefault<'T0> path : 'T0 = Fable.Core.Util.jsNative

    [<Fable.Core.FableImportJs;Inline>]
    let import<'T> (selector:string) (path:string) : 'T = Fable.Core.Util.jsNative

    [<Inline "$target == null">]
    let isNullOrUndefined (target:obj) : bool = target = null

[<Proxy("Fable.Core.JS, Fable.Core")>]
module internal JSProxy =
    let [<Inline "undefined">] undefined<'T> = X<'T>

    let [<Fable.Core.Global>] console : Fable.Core.JS.Console = Fable.Core.Util.jsNative

    [<Inline>]
    let inline  (!!) x = unbox x
    let (==>) (key: string) v = key,v :> obj

    [<Inline>]
    let inline createObj<'b when 'b :> seq<string*obj>> (fields: 'b) = New fields

[<Proxy(typeof<Fable.Core.U2<_,_>>)>]
type internal U2Proxy<'a,'b> = JavaScript.Union<'a,'b>
[<Proxy(typeof<Fable.Core.U3<_,_,_>>)>]
type internal U3Proxy<'a,'b,'c> = JavaScript.Union<'a,'b,'c>

[<Proxy(typeof<Fable.Core.U4<_,_,_,_>>)>]
type internal U4Proxy<'a,'b,'c,'d> = JavaScript.Union<'a,'b,'c,'d>

[<Proxy(typeof<Fable.Core.JS.Console>)>]
type internal ConsoleProxy = JavaScript.Console