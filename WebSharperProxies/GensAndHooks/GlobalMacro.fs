namespace Fable.Core

open WebSharper

type GlobalMacro() =
    inherit Core.Macro()

    override this.TranslateCall(c) =
        Core.AST.Expression.GlobalAccess
            {
                Module = Core.AST.Module.StandardLibrary
                Address = [c.Method.Entity.Value.MethodName]
            } |> Core.MacroOk

[<Macro(typeof<GlobalMacro>)>]
type GlobalAttribute() = inherit System.Attribute()