open Microsoft.AspNetCore.Builder

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)
    builder.WebHost.UseUrls("http://127.0.0.1:5030") |> ignore
    let app = builder.Build()
    app.UseStaticFiles() |> ignore
    EventDesk.App.mapRoutes app
    app.Run()
    0
