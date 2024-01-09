namespace Feliz.Styles

open Fable.Core

#if !JAVASCRIPT
[<Erase>]
#endif
type gridColumn =
    static member inline span(value: string) : IGridSpan = unbox("span " + value)
    static member inline span(value: string, count: int) : IGridSpan = unbox("span " + value + " " + (unbox<string> count))
    static member inline span(value: int) : IGridSpan = unbox("span " + (unbox<string> value))

#if !JAVASCRIPT
[<Erase>]
#endif
type gridRow =
    static member inline span(value: string) : IGridSpan = unbox("span " + value)
    static member inline span(value: string, count: int) : IGridSpan = unbox("span " + value + " " + (unbox<string> count))
    static member inline span(value: int) : IGridSpan = unbox("span " + (unbox<string> value))

#if !JAVASCRIPT
[<Erase>]
#endif
type grid =
    static member inline namedLine(value: string) : IGridTemplateItem = unbox ("[" + value + "]")
    static member inline namedLines(value: string[]) : IGridTemplateItem = unbox ("[" + (String.concat " " value) + "]")
    static member inline namedLines(value: string list) : IGridTemplateItem = unbox ("[" + (String.concat " " value) + "]")
    static member inline templateWidth(value: ICssUnit) : IGridTemplateItem = unbox value
    static member inline templateWidth(value: int) : IGridTemplateItem = unbox ((unbox<string>value) + "px")
    static member inline templateWidth(value: float) : IGridTemplateItem = unbox ((unbox<string>value) + "px")
