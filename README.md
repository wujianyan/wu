# EventDesk

Public demo: [wu1.tryasp.net](http://wu1.tryasp.net/)

Workshop signup demo in **ASP.NET Core 8 + F#** (no WebSharper). Fixed workshop list in `Logic.fs`, HTML in `Html.fs`, routes in `App.fs`. Registrations stay in memory for the running process. Static files: `wwwroot/css/app.css`. `Program.fs` adds **forwarded headers** (Alpha-style) and binds `PORT` only when **not** on IIS.

---

**Run** (from repository root):

```bash
dotnet build
dotnet run
```

**Local URL:** `http://127.0.0.1:5030` (`Properties/launchSettings.json`). On generic hosts, `PORT` maps to `http://0.0.0.0:{PORT}`; on IIS (MonsterASP), ANCM owns the URL.

**MonsterASP deploy:** from this folder run `dotnet publish -c Release -o publish`, then upload the **full** `publish/` contents (including `web.config` and all DLLs) to the site **Website root** via [WebFTP](https://webftp.monsterasp.net) or the [control panel](https://admin.monsterasp.net/app/site/site/info?guid=4cb633a5-57c5-497d-98e6-389b3b976d89). Do not upload only the Git `wwwroot/` tree.

**Routes**

| Method | Path | Purpose |
|--------|------|---------|
| GET | `/` | Workshop list |
| GET | `/desk` | Registration table |
| GET | `/register/{id}` | Form for workshop `id` |
| POST | `/register/{id}` | Submit registration |

**Sources:** `EventDesk.fsproj`, `Program.fs`, `App.fs`, `Logic.fs`, `Html.fs`.

CI: `.github/workflows/ci.yml`. Ignore build output under `bin/`, `obj/`, optional `build/`.