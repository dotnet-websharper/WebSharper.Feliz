namespace Feliz

open WebSharper
open WebSharper.Core

[<AutoOpen>]
module internal WsHelpers =
    
    let (|IsRecord|_|) expr =
        let rec exprEval expression =
            match expression with
            | AST.Expression.Let (_, _, expression)
            | AST.Expression.ExprSourcePos (_, expression) ->
                exprEval expression
            | AST.Expression.NewRecord (t, f) -> Some (t, f, expression)
            | AST.Expression.ExprSourcePos (_, AST.Expression.ExprSourcePos _)
            | _ -> None
        exprEval expr

[<JavaScript(false)>]
type ReactComponentGenerator(?import:string, ?from:string) =
    inherit Generator()
    new() = ReactComponentGenerator(?import = None,?from = None)
    override this.Generate (gen: Generated) =
        let expr = gen.Expression
        let inline changedWithImport expr =
            GeneratorCompiledMemberChange(gen.CompiledMember, expr)

        let inline (|ReflectionLoad|) (t:AST.Type): System.Type =
            AST.Reflection.LoadType t
        let inline (|IsReflectionRecord|_|) (t: System.Type):System.Type option =
            if FSharp.Reflection.FSharpType.IsRecord t then Some t else None
        
        let inline (|IsMetaRecord|_|) (ct:AST.Concrete<AST.TypeDefinition>):Metadata.FSharpRecordFieldInfo list option =
            match gen.Compilation.GetCustomTypeInfo ct.Entity with
            | Metadata.FSharpRecordInfo recordFields -> Some recordFields
            | _ -> None
        match gen.Member with
        | GeneratedMethod (typeDef, method) ->
            let membArgs = method.Value.Parameters
            match expr with
            | AST.Expression.Function(parameters, idOption, typeOption, statement) ->
                let newProp = AST.Id.New(?name = Some "props")
                let paramsToUse = parameters
                gen.Compilation.AddWarning(None, sprintf "GENERATOR paramNames %s: %A" method.Value.MethodName paramsToUse)
                gen.Compilation.AddMetadataEntry
                    (
                        Metadata.MetadataEntry.CompositeEntry
                            [
                                Metadata.MetadataEntry.StringEntry "ReactComponentMacro"
                                Metadata.MetadataEntry.TypeDefinitionEntry typeDef
                                Metadata.MetadataEntry.MethodEntry method
                            ],
                        (paramsToUse |> List.map (fun ast -> ast.Name.Value) |> String.concat "," |> Metadata.MetadataEntry.StringEntry)
                    )
                let newExpr = 
                    match membArgs with
                    | [AST.Type.ConcreteType (IsMetaRecord _)] | [ReflectionLoad (IsReflectionRecord _)] ->
                        gen.Expression |> GeneratedAST
                    | _ ->
                        let newStatement =
                            List.foldBack (fun (astid: AST.Id) body ->
                                let get =
                                    AST.Expression.ItemGet
                                        (AST.Expression.Var newProp,
                                            AST.Expression.Value (AST.Literal.String astid.Name.Value),
                                            AST.Purity.NonPure) // $props.x
                                AST.Expression.Let(astid, get, body)
                            ) paramsToUse (AST.Expression.StatementExpr (statement, None))
                        AST.Expression.Function([newProp], idOption, typeOption, AST.Statement.ExprStatement newStatement) |> GeneratedAST
                gen.Compilation.AddWarning(None, sprintf "%A" gen.CompiledMember)
                match gen.CompiledMember with
                | Metadata.CompiledMember.Macro (_, _, d) ->
                    match d with
                    | Some (Metadata.CompiledMember.Func(name, fromInstance)) when method.Value.MethodName = name ->
                        match membArgs with
                        | [AST.Type.VoidType] | [] ->
                            newExpr
                        | [AST.Type.TupleType(elems,_)] ->
                            newExpr
                        | _ ->
                            newExpr
                    | _ -> GeneratedAST gen.Expression
                | q -> 
                    GeneratedAST gen.Expression
            | notCall -> GeneratorError("This attribute should only be used on methods.")
        | notGenMethod -> 
            GeneratedAST gen.Expression
    
[<JavaScript(false)>]
type ReactComponentMacro() =
    inherit Macro()

    override this.TranslateCall(mc) =
        
        
        let inline (|ReflectionLoad|) (t:AST.Concrete<AST.TypeDefinition>): System.Type =
            AST.Reflection.LoadTypeDefinition t.Entity
        let inline (|IsReflectionRecord|_|) (t: System.Type):System.Type option =
            if FSharp.Reflection.FSharpType.IsRecord t then Some t else None
        
        
        let inline (|RecordFieldList|_|) (ct:AST.Concrete<AST.TypeDefinition>):Metadata.FSharpRecordFieldInfo list option =
            match mc.Compilation.GetCustomTypeInfo ct.Entity with
            | Metadata.FSharpRecordInfo recordFields -> Some recordFields
            | _ -> None
        
        [|
            mc.Compilation.GetMethodAttributes(mc.DefiningType.Entity,mc.Method.Entity)
            mc.Compilation.GetTypeAttributes(mc.DefiningType.Entity)
        |]
        |> Array.choose id
        |> Array.collect Array.ofList
        |> Array.distinct
        |> Array.iter (fun (td,ps) ->
            match td.Value.FullName.EndsWith "ReactComponentAttribute", ps with
            | true, [|Metadata.ParameterObject.String name;Metadata.ParameterObject.String import;Metadata.ParameterObject.String from|] ->
                // TODO
                ()
            | _ -> ()
            
        )
        let paramNames =
            mc.Compilation.GetMetadataEntries
                (Metadata.MetadataEntry.CompositeEntry
                    [
                        Metadata.MetadataEntry.StringEntry "ReactComponentMacro"
                        Metadata.MetadataEntry.TypeDefinitionEntry mc.DefiningType.Entity
                        Metadata.MetadataEntry.MethodEntry mc.Method.Entity
                    ]
                )
            |> List.head
            |> fun (Metadata.MetadataEntry.StringEntry x) ->
                x.Split([|','|]) |> List.ofArray
        mc.Compilation.AddWarning(None, $"MACRO params: %A{paramNames}")
        match mc.Method.Generics |> List.tryFind (fun g -> g.IsParameter) with
        | Some t ->
            MacroResult.MacroNeedsResolvedTypeArg t
        | _ ->
            match mc.Arguments with
            
            | [AST.Expression.NewTuple (items, _)] ->
                mc.Compilation.AddWarning(None, $"MACRO tuple arg: %A{items}")
                let objExpr =
                    List.zip paramNames items
                    |> List.map (fun (name, arg) -> name, AST.MemberKind.Simple, arg) |> AST.Expression.Object
                AST.Expression.Call(mc.This, mc.DefiningType, mc.Method, [objExpr])
                |> MacroOk
            | [arg] ->
                match arg with
                | IsRecord (ReflectionLoad (IsReflectionRecord recordType), fieldExprs, recordExpression) ->
                    let fieldList =
                        recordType
                        |> FSharp.Reflection.FSharpType.GetRecordFields
                        |> List.ofArray
                    mc.Compilation.AddWarning(None, $"%A{fieldList}")
                    match List.zip fieldList fieldExprs |> List.tryFind (fun (fieldInfo, _) -> fieldInfo.Name = "Key") with
                    | Some (_, keyVal) ->
                        let astid = AST.Id.New("props2")
                        AST.Expression.Let(astid, arg, 
                            AST.Expression.Sequential [
                                AST.Expression.ItemSet(AST.Expression.Var astid, AST.Expression.Value (AST.Literal.String "key"),
                                                       AST.Expression.ItemGet (AST.Expression.Var astid, AST.Expression.Value (AST.Literal.String("Key")), AST.NonPure))
                                AST.Expression.Call(mc.This, mc.DefiningType, mc.Method, [AST.Expression.Var astid])
                            ]
                        )
                        |> MacroOk
                    | _ ->
                        MacroFallback
                | IsRecord (RecordFieldList fieldList, fieldExprs, recordExpr) ->
                    match List.zip fieldList fieldExprs |> List.tryFind (fun (fieldInfo, asd) -> fieldInfo.Name = "Key") with
                    | Some (_, keyVal) ->
                        let astid = AST.Id.New("props2")
                        AST.Expression.Let(astid, arg, 
                            AST.Expression.Sequential [
                                AST.Expression.ItemSet(AST.Expression.Var astid, AST.Expression.Value (AST.Literal.String "key"), keyVal)
                                AST.Expression.Call(mc.This, mc.DefiningType, mc.Method, [AST.Expression.Var astid])
                            ]
                        )
                        |> MacroOk
                    | _ ->
                        MacroFallback
                | _ ->
                    MacroFallback
            | args ->
                if paramNames.Length <> args.Length then // of if length 1, is this an AST.Expression.Object
                    MacroFallback
                else
                    mc.Compilation.AddWarning(None, $"MACRO regular args: %A{args}")
                    let obj =
                        List.zip paramNames args
                        |> List.map (fun (name, arg) -> name, AST.MemberKind.Simple, arg) |> AST.Expression.Object
                    AST.Expression.Call(mc.This, mc.DefiningType, mc.Method, [obj])
                    |> MacroOk

[<JavaScript(false)>]
[<Macro(typeof<ReactComponentMacro>)>]
[<Generated(typeof<ReactComponentGenerator>)>]
type ReactComponentAttribute() =
    inherit System.Attribute()