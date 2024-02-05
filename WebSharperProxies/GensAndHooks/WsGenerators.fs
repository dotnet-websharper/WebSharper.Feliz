namespace Fable.Core

open WebSharper
open WebSharper.Core

[<JavaScript(false)>]
type NoneGenerator() =
    inherit Generator()

    override this.Generate(gen: Generated): GeneratorResult =
        GeneratedAST gen.Expression

[<JavaScript(false)>]
[<Generated(typeof<NoneGenerator>)>]
type EraseAttribute() =
    inherit System.Attribute()

[<JavaScript(false)>]
[<Generated(typeof<NoneGenerator>)>]
type StringEnumAttribute() =
    inherit System.Attribute()