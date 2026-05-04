module EventDesk.Html

open System.Net
open EventDesk.Logic

let esc (s: string) = WebUtility.HtmlEncode(s)

let layout (title: string) (body: string) =
    sprintf
        """<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>%s</title>
  <link rel="stylesheet" href="/css/app.css" />
</head>
<body>
  <header class="head">
    <div class="brand"><a href="/">CampusMeet</a><span>workshop desk</span></div>
    <nav><a href="/">Sessions</a></nav>
  </header>
  <main class="wrap">%s</main>
  <footer class="foot">Registrations are stored in memory on this demo host.</footer>
</body>
</html>"""
        (esc title)
        body

let listWorkshops (regs: int list) =
    let regCount wid = regs |> List.filter ((=) wid) |> List.length

    let cards =
        workshops
        |> List.map (fun w ->
            let taken = regCount w.Id
            let left = max (w.Seats - taken) 0

            sprintf
                """<article class="card">
  <div class="when">%s</div>
  <h2>%s</h2>
  <p class="room">%s · %i seats · <strong>%i</strong> left</p>
  <p class="blurb">%s</p>
  <a class="cta" href="/register/%i">Register</a>
</article>"""
                (w.Starts.ToString("yyyy-MM-dd HH:mm"))
                (esc w.Title)
                (esc w.Room)
                w.Seats
                left
                (esc w.Blurb)
                w.Id)

        |> String.concat ""

    layout
        "Workshops"
        (sprintf
            """<section class="hero"><h1>Hands-on slots fill quickly</h1>
<p>Pick a session, leave contact details, and optional dietary note for catering.</p></section>
<section class="deck">%s</section>"""
            cards)

let registerForm (w: Workshop) (err: string option) =
    let banner =
        match err with
        | None -> ""
        | Some m -> sprintf """<p class="err">%s</p>""" (esc m)

    layout
        ("Register · " + w.Title)
        (sprintf
            """%s
<form method="post" action="/register/%i" class="form">
  <h1>%s</h1>
  <p class="muted">%s · starts %s</p>
  <label>Full name <input name="name" required maxlength="120" autocomplete="name" /></label>
  <label>Email <input type="email" name="email" required maxlength="120" autocomplete="email" /></label>
  <label>Diet / allergies <input name="diet" maxlength="200" placeholder="Vegetarian, none, …" /></label>
  <button type="submit">Confirm seat</button>
</form>
<p><a href="/">← All sessions</a></p>"""
            banner
            w.Id
            (esc w.Title)
            (esc w.Room)
            (w.Starts.ToString("yyyy-MM-dd HH:mm")))

let thankYou (name: string) (title: string) =
    layout
        "Registered"
        (sprintf
            """<section class="card ok">
  <h1>You are on the list</h1>
  <p>%s — we saved you for <strong>%s</strong>. Check your inbox for a confirmation stub from this demo server.</p>
  <a class="cta ghost" href="/">Back to sessions</a>
</section>"""
            (esc name)
            (esc title))

let missing () =
    layout
        "Not found"
        """<section class="card"><h1>Unknown workshop</h1><p><a href="/">Return</a></p></section>"""
