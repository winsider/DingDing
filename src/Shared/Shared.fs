namespace Shared
open System

type Counter = { Value : int }


type Contact = {
    FistName : string
    LastName : string
    Email : string
    Mobile : string
}

type Organization = {
    Name : string
    Website : string
    Phone : string
    Email : string
    OrganizationNumber : string
}

type Lead = {
    Name : string
}

type Task = {
    _id : string
    Specification : string
    Target : DateTime
    Completed : DateTime option
}
