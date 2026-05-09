module EventDesk.App

open System
open System.Collections.Generic
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Primitives
open EventDesk.Logic
open EventDesk.Html

let private gate = obj ()
let private regs = ResizeArray<Registration> ()

let private writeHtml (ctx: HttpContext) (html: string) =
    ctx.Response.ContentType <- "text/html; charset=utf-8"
    ctx.Response.WriteAsync(html)

let private textOrEmpty (t: string | null) =
    match t with
    | null -> ""
    | x -> x

let private routeText (o: obj | null) =
    match o with
    | null -> ""
    | v -> textOrEmpty (v.ToString())

let private formGet (form: IFormCollection) (name: string) =
    let mutable s = StringValues.Empty

    if form.TryGetValue(name, &s) then
        textOrEmpty (s.ToString())
    else
        ""

let private routeId (ctx: HttpContext) =
    match ctx.Request.RouteValues.TryGetValue("id") with
    | true, v ->
        let t = routeText v

        match System.Int32.TryParse(t) with
        | true, n -> n
        | _ -> 0
    | _ -> 0

let mapRoutes (app: WebApplication) =
    app.MapGet(
        "/",
        RequestDelegate(fun ctx ->
            let ids =
                lock gate (fun () -> regs |> Seq.toList)

            writeHtml ctx (listWorkshops ids))
    )
    |> ignore

    app.MapGet(
        "/desk",
        RequestDelegate(fun ctx ->
            let rows =
                lock gate (fun () -> regs |> Seq.toList)

            writeHtml ctx (registry rows))
    )
    |> ignore

    app.MapGet(
        "/register/{id:int}",
        RequestDelegate(fun ctx ->
            let id = routeId ctx

            match tryWorkshop id with
            | Some w -> writeHtml ctx (registerForm w None)
            | None -> writeHtml ctx (missing ()))
    )
    |> ignore

    app.MapPost(
        "/register/{id:int}",
        RequestDelegate(fun ctx ->
            task {
                let id = routeId ctx

                match tryWorkshop id with
                | None -> return! writeHtml ctx (missing ())
                | Some w ->
                    let! form = ctx.Request.ReadFormAsync()
                    let name = formGet form "name"
                    let email = formGet form "email"
                    let diet = formGet form "diet"

                    if String.IsNullOrWhiteSpace(name) || String.IsNullOrWhiteSpace(email) then
                        return! writeHtml ctx (registerForm w (Some "Name and email are required."))
                    elif not (email.Contains("@")) then
                        return! writeHtml ctx (registerForm w (Some "Email looks invalid."))
                    else
                        let taken =
                            lock gate (fun () -> regs |> Seq.filter (fun r -> r.WorkshopId = id) |> Seq.length)

                        if taken >= w.Seats then
                            return! writeHtml ctx (registerForm w (Some "This session is full."))
                        else
                            let row =
                                {
                                    WorkshopId = id
                                    Name = name.Trim()
                                    Email = email.Trim()
                                    Diet = diet.Trim()
                                    AtUtc = DateTime.UtcNow
                                }

                            lock gate (fun () -> regs.Add(row))
                            return! writeHtml ctx (thankYou name w.Title)
            })
    )
    |> ignore
