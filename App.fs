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
let private regs = ResizeArray<int> ()

let private writeHtml (ctx: HttpContext) (html: string) =
    ctx.Response.ContentType <- "text/html; charset=utf-8"
    ctx.Response.WriteAsync(html)

let private formGet (form: IFormCollection) (name: string) =
    let mutable s = StringValues.Empty

    if form.TryGetValue(name, &s) then
        s.ToString()
    else
        ""

let private routeId (ctx: HttpContext) =
    match ctx.Request.RouteValues.TryGetValue("id") with
    | true, v ->
        match System.Int32.TryParse(string v) with
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
                    let _diet = formGet form "diet"

                    if String.IsNullOrWhiteSpace(name) || String.IsNullOrWhiteSpace(email) then
                        return! writeHtml ctx (registerForm w (Some "Name and email are required."))
                    elif not (email.Contains("@")) then
                        return! writeHtml ctx (registerForm w (Some "Email looks invalid."))
                    else
                        let taken =
                            lock gate (fun () -> regs |> Seq.filter ((=) id) |> Seq.length)

                        if taken >= w.Seats then
                            return! writeHtml ctx (registerForm w (Some "This session is full."))
                        else
                            lock gate (fun () -> regs.Add(id))
                            return! writeHtml ctx (thankYou name w.Title)
            })
    )
    |> ignore
