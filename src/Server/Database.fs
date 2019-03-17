
module Database

    open Shared
    open MongoDB.Driver
    open System
    open MongoDB.Bson

    let private getTasks (connstr:string) =
        let dbconn = MongoClient(connstr)
        let db = dbconn.GetDatabase("dingding")
        db.GetCollection<Task>("tasks")

    let createTask (connstr:string) t =
        let tasks = getTasks connstr
        let t = { t with _id = ObjectId.GenerateNewId(DateTime.UtcNow).ToString() }
        tasks.InsertOne (t)
        t

    let retrieveTask (connstr:string) id =
        let tasks = getTasks connstr
        tasks.Find(fun t -> t._id = id).First()

    let updateTask (connstr:string) task =
        let tasks = getTasks connstr
        let r = tasks.ReplaceOne((fun t -> t._id = task._id), task)
        task

    let deleteTask (connstr:string) id =
        let tasks = getTasks connstr
        let r = tasks.DeleteOne(fun t -> t._id = id)
        r.DeletedCount = 1L
