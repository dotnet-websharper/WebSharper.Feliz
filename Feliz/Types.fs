namespace Feliz

/// Describes an element property
type IReactProperty = interface end

/// Describes style attribute
type IStyleAttribute = interface end

/// Describes an SVG attribute
type ISvgAttribute = interface end 

// #if JAVASCRIPT
// type ReactElement = WebSharper.React.React.Element
// type IRefValue<'T> = 
//     abstract member current: 'T with get, set
// #else
type ReactElement = Fable.React.ReactElement
type IRefValue<'T> = Fable.React.IRefValue<'T>
// #endif
// module JS =
//     
//     type Promise<'a> =
//         #if JAVASCRIPT
//         WebSharper.JavaScript.Promise<'a>
//         #else
//         Fable.Core.JS.Promise<'a>
//         #endif