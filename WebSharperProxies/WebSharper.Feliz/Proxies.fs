[<AutoOpen>]
module Helpers

open WebSharper
open WebSharper.JavaScript
open WebSharper.JavaScript.Interop

[<InternalProxy(typeof<System.Environment>)>]
type internal EnvironmentProxyHack =
    static member NewLine = "\n" // forgive me
    
// [<Proxy(typeof<System.Threading.CancellationTokenSource>)>]
// type internal CtsProxy() =
//     
//     member this.Dispose() =
//         (As<System.IDisposable> this).Dispose()
        
// [<Proxy(typeof<System.DateTime>)>]
// type internal DateTimeProxy =
//     
//     [<Inline "new Date($y,$mo-1,$d,$h,$m,$s).getTime()"; Pure>]
//     new(a:int,b:int,c:int,d:int,e:int,f:int) = {}
//     member this.ToString(format:string) =
//         (this |> As<Date>).ToISOString()


// [<Proxy("Fable.React, Fable.ReactDom.Types")>]
module internal ReactProxies =
    [<Proxy("Fable.React.ReactBindings, Fable.React.Types")>]
    module ReactBindingsProxy =
        let [<Fable.Core.Global>] React = 
            Unchecked.defaultof<Fable.React.IReactExports>
        
    [<Proxy(typeof<Fable.React.ReactElement>)>]
    type ReactElementProxy = React.React.Element

    [<Proxy(typeof<Fable.React.IRefValue<_>>)>]
    type IRefValue<'T> = 
        abstract current: 'T with get,set
        
    [<Proxy(typeof<Fable.React.IContext<_>>)>]
    type IContextProxy<'T> = interface end
    
    [<Proxy(typeof<Fable.React.ReactElementType<_>>)>]
    type ElTypeProxy<'T> = WebSharper.React.React.ElementType<'T>
        
    [<Proxy(typeof<Fable.React.IReactExports>)>]
    type ReactExportsProxy =
        [<Import("createContext","react");Inline "createContext($defaultValue)">]
        abstract createContext<'T> : defaultValue:'T -> Fable.React.IContext<'T>
        // default this.createContext<'T> (defaultValue:'T) : Fable.React.IContext<'T> = WebSharper.React.React.CreateContext defaultValue |> As
        
        [<Import("createElement","react");Inline "createElement($comp,$props,$children)">]
        abstract createElement : comp: obj * props: obj * children: Fable.React.ReactElement seq -> Fable.React.ReactElement
        // default this.createElement<'T>(comp:obj,props:obj, children: Fable.React.ReactElement seq) : Fable.React.ReactElement =
        //     React.React.CreateElement(comp, props, children |> As) |> As
        
        [<Import("Fragment","react")>]
        abstract Fragment : Fable.React.ReactElementType<obj> with get
        // default this.Fragment with get () : Fable.React.ReactElementType<obj> = WebSharper.React.React.Fragment |> As
        
        

