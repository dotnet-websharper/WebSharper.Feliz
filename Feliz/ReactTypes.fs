module Feliz.ReactApi

#if JAVASCRIPT
type private ParamListAttribute = System.ParamArrayAttribute // TODO: something better
#endif
open Fable.React
open Fable.Core

open Browser.Types
open Feliz

open System

#if JAVASCRIPT
[<WebSharper.Stub>]
#endif
type ReactChildren =
    // #if JAVASCRIPT
    // [<WebSharper.Inline "$0.toArray($1)">]
    // #endif
    abstract toArray: ReactElement -> ReactElement seq
    abstract toArray: ReactElement seq -> ReactElement seq

#if JAVASCRIPT
[<WebSharper.Stub>]
#endif
type IReactApi =
    abstract Children: ReactChildren
    abstract createContext: defaultValue: 'a -> IContext<'a>
    abstract createElement: comp: obj * props: obj -> Feliz.ReactElement
    abstract createElement: comp: obj * props: obj * [<ParamList>] children: Feliz.ReactElement seq -> Feliz.ReactElement
    abstract forwardRef: render: Func<'props,IRefValue<'t>,Feliz.ReactElement> -> ('props -> IRefValue<'t> -> Feliz.ReactElement)
    #if JAVASCRIPT
    [<WebSharper.Inline("$0.lazy($1)")>]
    #else
    [<Emit("$0.lazy($1)")>]
    #endif
    abstract lazy': import: (unit -> JS.Promise<'t>) -> 't
    abstract memo: render: ('props -> ReactElement) * areEqual: ('props -> 'props -> bool) -> ('props -> ReactElement)
    abstract StrictMode: obj
    abstract Suspense: obj
    abstract useCallback: callbackFunction: ('a -> 'b) -> dependencies: obj array -> ('a -> 'b)
    abstract useContext: ctx: IContext<'a> -> 'a
    abstract useEffect: obj * 't array -> unit
    abstract useEffect: obj -> unit
    abstract useEffect: (unit -> unit) -> unit
    abstract useImperativeHandle<'t> : ref: Feliz.IRefValue<'t> -> createHandle: (unit -> 't) -> dependencies: obj array -> unit
    #if JAVASCRIPT
    [<WebSharper.Inline("$this.useImperativeHandle($ref,$createHandle)")>]
    #else
    [<Emit("$0.useImperativeHandle($1, $2)")>]
    #endif
    abstract useImperativeHandleNoDeps<'t> : ref: Feliz.IRefValue<'t> -> createHandle: (unit -> 't) -> unit
    abstract useMemo: createFunction: (unit -> 'a) -> dependencies: obj array -> 'a
    abstract useReducer: ('state -> 'msg -> 'state) -> 'state -> ('state * ('msg -> unit))
    #if JAVASCRIPT
    [<WebSharper.Inline("$this.useRef($initial)")>]
    #else
    [<Emit "$0.useRef($1)">]
    #endif
    abstract useRefInternal<'t> : initial: 't -> Feliz.IRefValue<'t>
    abstract useState<'t,'u> : initial:'t -> ('u * ('u -> unit))

type IReactRoot =
    /// Renders the provided React element into the DOM in the supplied container.
    abstract render: ReactElement -> unit
    /// Removes a mounted React component from the DOM and cleans up its event handlers and state.
    abstract unmount : unit -> unit