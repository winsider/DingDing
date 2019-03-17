open System
open System.IO
open System.Threading.Tasks

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open FSharp.Control.Tasks.V2
open Giraffe
open Saturn
open Saturn.ControllerHelpers
open Shared
open Database
open Microsoft.WindowsAzure.Storage

let tryGetEnv = System.Environment.GetEnvironmentVariable >> function null | "" -> None | x -> Some x

let publicPath = tryGetEnv "public_path" |> Option.defaultValue "../Client/public" |> Path.GetFullPath
let storageAccount = tryGetEnv "STORAGE_CONNECTIONSTRING" |> Option.defaultValue "UseDevelopmentStorage=true" |> CloudStorageAccount.Parse
let connectionString = tryGetEnv "DB_CONNECTIONSTRING" |> Option.defaultValue "mongodb://localhost:27017"
let port = "SERVER_PORT" |> tryGetEnv |> Option.map uint16 |> Option.defaultValue 8085us

let getInitCounter() : Task<Counter> = task { return { Value = 26 } }

let postTask ctx = task {
    let! t = Controller.getJson<Task> ctx
    return createTask connectionString t
}

let putTask ctx id = task {
    let! t = Controller.getJson<Task> ctx
    // todo: check id is correct
    return updateTask connectionString t
}

let deleteTask ctx id = task {
    return deleteTask connectionString id
}

let getTask ctx id = task {
    return retrieveTask connectionString id
}

let taskController = controller {
    show getTask
    create postTask
    update putTask
    delete deleteTask
}

let webApp = router {
    get "/api/init" (fun next ctx ->
        task {
            let! counter = getInitCounter()
            return! json counter next ctx
        })
    forward "/api/task" taskController
}

let configureSerialization (services:IServiceCollection) =
    services.AddSingleton<Giraffe.Serialization.Json.IJsonSerializer>(Thoth.Json.Giraffe.ThothSerializer())

let configureAzure (services:IServiceCollection) =
    tryGetEnv "APPINSIGHTS_INSTRUMENTATIONKEY"
    |> Option.map services.AddApplicationInsightsTelemetry
    |> Option.defaultValue services

let app = application {
    url ("http://0.0.0.0:" + port.ToString() + "/")
    use_router webApp
    memory_cache
    use_static publicPath
    service_config configureSerialization
    service_config configureAzure
    use_gzip
}

run app
