#r "nuget: Fli"
open System.IO
open Fli

Directory.GetDirectories(".")
|> Array.map (fun x -> x.Substring(2))
|> Array.filter(fun x -> x.StartsWith("Feliz.") && not ([|"Feliz.CompilerPlugins";"Feliz.UseElmish"|] |> Array.contains x))
|> Array.iter (fun x ->
    let newProjName = $"WebSharper.{x}"
    cli {
        Shell BASH
        Command $"cd WebSharperProxies && dotnet new websharper-prx -lang F# -o {newProjName}"
        // WorkingDirectory $"""WebSharperProxies\{newProjName}"""
        // Command "ls"
    }
    |> Command.execute
    |> ignore
)