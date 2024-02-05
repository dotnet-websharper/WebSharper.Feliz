#r "nuget: Fli"

open Fli
open System.IO

Directory.GetDirectories(".")
|> Array.map (fun x -> x.Substring 2)
|> Array.filter (fun x -> x.StartsWith "WebSharper.Feliz.")
|> Array.iter (fun x ->
    let addCmd str = $"dotnet add package {str}"
    let ws = "WebSharper"
    let pre = "--prerelease"
    let packages = [|
        addCmd $"GensAndHooks"
        addCmd "FablePartialProxy"
    |]
    let nugetcmd = $"""cd {x} && {String.concat " && " packages}"""
    cli {
        Shell BASH
        Command nugetcmd
    }
    |> Command.execute
    |> ignore
)