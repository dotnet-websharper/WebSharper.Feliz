#r "nuget: Fli"

open Fli
open System.IO

Directory.GetDirectories(".")
|> Array.map (fun x -> x.Substring 2)
|> Array.filter (fun x -> x.StartsWith "WebSharper.Feliz." && not ([|"WebSharper.Feliz.UseElmish"|] |> Array.contains x))
|> Array.iter (fun x ->
    let addCmd str = $"dotnet add package {str}"
    let ws = "WebSharper"
    let pre = "--prerelease"
    let packages = [|
        addCmd $"{ws} {pre}"
        addCmd $"{ws}.FSharp {pre}"
        addCmd $"{ws}.Feliz"
        addCmd $"GensAndHooks"
    |]
    let cmd = $"dotnet sln .. add {x}"
    let nugetcmd = $"""cd {x} && {String.concat " && " packages}"""
    cli {
        Shell BASH
        Command cmd
    }
    |> Command.execute
    |> ignore
)