namespace Fable.Core

open WebSharper
open WebSharper.Core
open WebSharper.Core.AST
module M = Metadata
module I = AST.IgnoreSourcePos
[<JavaScript false>]
[<Sealed>]
type FableImportJSMacro() =
    inherit Macro()

    override __.TranslateCall(c) =
        match c.Method.Entity.Value.MethodName with
        | "import" ->
            match c.Arguments with
            | [I.Value (String export); I.Value (String from)] ->
                if JavaScript.Identifier.IsValid export || export = "*" then
                    c.Compilation.AddJSImport (Some export, from) |> MacroOk
                elif export = "default" then
                    c.Compilation.AddJSImport (None, from) |> MacroOk                
                else
                    MacroError "import export argument must be a valid identifier"
            | _ -> MacroError "import arguments must be constant string"
        | "importDefault" ->
            match c.Arguments with
            | [I.Value (String from)] ->
                c.Compilation.AddJSImport (None, from) |> MacroOk
            | _ -> MacroError "importDefault arguments must be constant string"
        | "importAll" ->
            match c.Arguments with
            | [I.Value (String from)] ->
                c.Compilation.AddJSImport (Some "*", from) |> MacroOk
            | _ -> MacroError "importAll argument must be constant string"
        | _ ->
            failwith "Unrecognized method using ImportJS"
  
[<JavaScript false>]          
[<Macro(typeof<FableImportJSMacro>)>]
type FableImportJsAttribute() =
    inherit System.Attribute()