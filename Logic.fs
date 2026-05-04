module EventDesk.Logic

open System

type Workshop =
    {
        Id: int
        Title: string
        Room: string
        Starts: DateTime
        Seats: int
        Blurb: string
    }

let workshops : Workshop list =
    [
        {
            Id = 1
            Title = "F# domain modelling clinic"
            Room = "Lab-2"
            Starts = DateTime(2026, 5, 12, 14, 0, 0)
            Seats = 24
            Blurb = "Records, DUs, and validation patterns you can reuse in coursework."
        }
        {
            Id = 2
            Title = "Async and UI boundaries"
            Room = "Lab-2"
            Starts = DateTime(2026, 5, 19, 14, 0, 0)
            Seats = 20
            Blurb = "Keep async code predictable before wiring WebSharper RPC."
        }
        {
            Id = 3
            Title = "Readable web forms"
            Room = "Online"
            Starts = DateTime(2026, 5, 26, 10, 0, 0)
            Seats = 40
            Blurb = "Separate validation from rendering; accessible error states."
        }
    ]

let tryWorkshop (id: int) = workshops |> List.tryFind (fun w -> w.Id = id)
