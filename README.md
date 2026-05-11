# EventDesk

Public demo: [wu1.tryasp.net](http://wu1.tryasp.net/)

Workshop signup demo in **ASP.NET Core 8 + F#** (no WebSharper). Fixed workshop list in `Logic.fs`, HTML in `Html.fs`, routes in `App.fs`. Registrations stay in memory for the running process. Static files: `wwwroot/css/app.css`.

---

**Run** (from repository root):

```bash
dotnet build
dotnet run
```

**Local URL:** `http://127.0.0.1:5030` (`Properties/launchSettings.json`). With `PORT` set, `Program.fs` uses `http://0.0.0.0:{PORT}`.

**Routes**

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/` | Workshop list |
| GET | `/desk` | Registration table |
| GET | `/register/{id}` | Form for workshop `id` |
| POST | `/register/{id}` | Submit registration |

**Sources:** `EventDesk.fsproj`, `Program.fs`, `App.fs`, `Logic.fs`, `Html.fs`.

CI: `.github/workflows/ci.yml`. Ignore build output under `bin/`, `obj/`, optional `build/`.