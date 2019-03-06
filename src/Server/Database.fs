
module Database

    open Shared
    open MongoDB.Driver
    open System

    let createTask (connstr:string) task =
        let dbconn = MongoClient(connstr)
        let db = dbconn.GetDatabase("dingding")
        let customerCol = db.GetCollection<Task>("tasks")
        customerCol.InsertOne(task)
        "Task created OK"

