namespace Feliz

open WebSharper
open WebSharper.Core

[<JavaScript(false)>]
type HookGenerator() = 
    inherit Generator() 
    override this.Generate (gen: Generated) = 
        match gen.Member with 
        | GeneratedMethod (typeDef, method) ->
            match gen.CompiledMember with 
            | Metadata.CompiledMember.Func(name, fromInstance) when not <| name.StartsWith "use" -> 
                GeneratorCompiledMemberChange(Metadata.CompiledMember.Func($"use{name}", fromInstance), GeneratedAST gen.Expression) 
            | _ -> 
                GeneratedAST gen.Expression 
        | GeneratedImplementation _ -> GeneratedAST gen.Expression 
        | _ -> GeneratedAST gen.Expression 

[<JavaScript(false)>]
[<Generated(typeof<HookGenerator>)>] 
type HookAttribute() = 
    inherit System.Attribute()