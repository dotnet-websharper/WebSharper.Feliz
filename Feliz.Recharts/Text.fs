namespace Feliz.Recharts

open System
open Feliz
open Fable.Core
open Fable.Core.JsInterop

[<Erase>]
type text =
    static member inline scaleToFit (value: bool) = Interop.mkTextAttr "scaleToFit" value
    static member inline angle (value: int) = Interop.mkTextAttr "angle" value
    static member inline width (value: int) = Interop.mkTextAttr "width" value

module text =

    [<Erase>]
    type textAnchor =
        static member inline start = Interop.mkTextAttr "textAnchor" "start"
        static member inline middle = Interop.mkTextAttr "textAnchor" "middle"
        #if JAVASCRIPT
        [<WebSharper.Name("end_")>]
        #endif
        static member inline end' = Interop.mkTextAttr "textAnchor" "end"
        #if JAVASCRIPT
        [<WebSharper.Name("inherit_")>]
        #endif
        static member inline inherit' = Interop.mkTextAttr "textAnchor" "inherit"

    [<Erase>]
    type verticalAnchor =
        static member inline start = Interop.mkTextAttr "verticalAnchor" "start"
        static member inline middle = Interop.mkTextAttr "verticalAnchor" "middle"
        #if JAVASCRIPT
        [<WebSharper.Name("end_")>]
        #endif
        static member inline end' = Interop.mkTextAttr "verticalAnchor" "end"