namespace Shared
open System

type Counter = { Value : int }


type Customer = {
    Name : string
    OrganizationNumber : string
}

type Task = {
    Specification : string
    Target : DateTime
    Completed : DateTime option
}
