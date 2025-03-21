#r "nuget: FAKE.Core"
#r "nuget: Fake.Core.Target"
#r "nuget: Fake.IO.FileSystem"
#r "nuget: Fake.Tools.Git"
#r "nuget: Fake.DotNet.Cli"
#r "nuget: Fake.DotNet.AssemblyInfoFile"
#r "nuget: Fake.DotNet.Paket"

open Fake.Core
System.Environment.GetCommandLineArgs()
|> Array.skip 2 // skip fsi.exe; build.fsx
|> Array.toList
|> Fake.Core.Context.FakeExecutionContext.Create false __SOURCE_FILE__
|> Fake.Core.Context.RuntimeContext.Fake
|> Fake.Core.Context.setExecutionContext

#load "paket-files/wsbuild/github.com/dotnet-websharper/build-script/WebSharper.Fake.fsx"
open WebSharper.Fake
open Fake.DotNet
open Fake.Core
open Fake.Core.TargetOperators

let GetSemVerOf pkgName =
    let lockFile = Paket.LockFile.LoadFrom "./paket.lock"
    lockFile
        .GetGroup(Paket.Domain.GroupName "WebSharper")
        .GetPackage(Paket.Domain.PackageName pkgName)
        .Version
    |> Some

let LazyVersionFrom packageName =
    fun () -> GetSemVerOf packageName |> ComputeVersion

let WithProjects projects args =
    { args with BuildAction = Projects projects }

Target.create "WS-Feliz-Update" <| fun _ ->
    let depsFile = Paket.DependenciesFile.ReadFromFile "./paket.dependencies"
    let mainGroup = depsFile.GetGroup (Paket.Domain.GroupName "WebSharper")
    let needsUpdate =
        mainGroup.Packages
        |> Seq.exists (fun { Name = pkg } ->
            pkg.Name.Contains "WebSharper")
    if needsUpdate then
        let res =
            DotNet.exec id "paket"
                (sprintf "update -g %s" mainGroup.Name.Name)
        if not res.OK then failwith "dotnet paket update failed"
    for g, _ in depsFile.Groups |> Map.toSeq do
        if g.Name.ToLower().StartsWith("test") then
            let res =
                DotNet.exec id "paket"
                    (sprintf "update -g %s" g.Name)
            if not res.OK then failwith "dotnet paket update failed"


Target.create "PrePackaging" <| fun _ ->
    let files =
        [
            // "Feliz.CompilerPlugins"
            "Feliz"
            "Feliz.Delay"
            // "Feliz.Kawaii"
            // "Feliz.Listeners"
            "Feliz.Markdown"
            "Feliz.PigeonMaps"
            "Feliz.Popover"
            // "Feliz.Recharts"
            "Feliz.RoughViz"
            // "Feliz.SelectSearch"
            "Feliz.Svelte"
            // "Feliz.SvelteComponent"
            "Feliz.UseDeferred"
            "Feliz.UseElmish"
            "Feliz.UseMediaQuery"
        ]

    let template = """type file
id WebSharper.%TEMP%
authors IntelliFactory
projectUrl https://websharper.com/
repositoryType git
repositoryUrl https://github.com/dotnet-websharper/WebSharper.Feliz/
licenseUrl https://github.com/dotnet-websharper/WebSharper.Feliz/blob/master/LICENSE.md
iconUrl https://github.com/dotnet-websharper/core/raw/websharper50/tools/WebSharper.png
description
    WebSharper Proxy for %TEMP%
tags
    WebSharper Fable FSharp CSharp JavaScript WebAPI Feliz %TEMP%
dependencies
    framework: netstandard2.0
        WebSharper ~> LOCKEDVERSION:[3]
        %TEMP% == %VERSION%
files
    ../WebSharperProxies/WebSharper.%TEMP%/bin/Release/netstandard2.0/WebSharper.%TEMP%.dll ==> lib/netstandard2.0

references
    WebSharper.%TEMP%.dll

"""

    let ftemplate = """type file
id WebSharper.Feliz
authors IntelliFactory
projectUrl https://websharper.com/
repositoryType git
repositoryUrl https://github.com/dotnet-websharper/WebSharper.Feliz/
licenseUrl https://github.com/dotnet-websharper/WebSharper.Feliz/blob/master/LICENSE.md
iconUrl https://github.com/dotnet-websharper/core/raw/websharper50/tools/WebSharper.png
description
    WebSharper Proxy for Feliz
tags
    WebSharper Fable FSharp CSharp JavaScript WebAPI Feliz
dependencies
    framework: netstandard2.0
        WebSharper ~> LOCKEDVERSION:[3]
        Feliz == %VERSION%
files
    ../WebSharperProxies/WebSharper.Feliz/bin/Release/netstandard2.0/WebSharper.Feliz.dll ==> lib/netstandard2.0
    ../WebSharperProxies/WebSharper.Feliz.CompilerPlugins/bin/Release/netstandard2.0/WebSharper.Feliz.CompilerPlugins.dll ==> lib/netstandard2.0

references
    WebSharper.Feliz.dll
    WebSharper.Feliz.CompilerPlugins.dll

"""

    let content x v = template.Replace("%TEMP%", x).Replace("%VERSION%", v)
    let fcontent x v = template.Replace("%TEMP%", x).Replace("%VERSION%", v)

    let getVersionNumber x =
        let lines = System.IO.File.ReadAllLines(sprintf "%s/%s.fsproj" x x)
        lines
        |> Array.tryPick (fun x ->
            let line = x.Trim()
            if line.StartsWith "<Version>" then
                line.Replace("<Version>","").Replace("</Version>", "") |> Some
            else
                None
        )

    files
    |> List.iter (fun x ->
        if x = "Feliz" then
            let versionNumber = getVersionNumber x |> Option.map (fun x -> printfn "%s" x; x) |> Option.defaultValue "1.0.0"
            System.IO.Directory.CreateDirectory "nuget" |> ignore
            System.IO.File.WriteAllText(sprintf "nuget/WebSharper.%s.paket.template" x, fcontent x versionNumber)
        else
            let versionNumber = getVersionNumber x |> Option.map (fun x -> printfn "%s" x; x) |> Option.defaultValue "1.0.0"
            System.IO.Directory.CreateDirectory "nuget" |> ignore
            System.IO.File.WriteAllText(sprintf "nuget/WebSharper.%s.paket.template" x, content x versionNumber)
    )

let targets =
    LazyVersionFrom "WebSharper" |> WSTargets.Default
    |> fun args ->
        { args with
            Attributes = [
                AssemblyInfo.Company "IntelliFactory"
                AssemblyInfo.Copyright "(c) IntelliFactory 2023"
                AssemblyInfo.Title "https://github.com/dotnet-websharper/WebSharper.Feliz"
                AssemblyInfo.Product "WebSharper.Feliz"
            ]
        }
    |> WithProjects [
        "WebSharperProxies/WebSharper.sln"
    ]
    |> MakeTargets

"PrePackaging" ==> "WS-Package"
"WS-Clean" ==> "WS-Feliz-Update" ==> "WS-Update"

targets
|> RunTargets