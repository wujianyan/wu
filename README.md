# EventDesk

Public demo: [wu1.tryasp.net](http://wu1.tryasp.net/)

Workshop signup: ASP.NET Core 8 + F#. `Program.fs`: forwarded headers; `PORT` only off IIS.

---

**Run**

```bash
dotnet build
dotnet run
```

**Local:** `http://127.0.0.1:5030`

## MonsterASP: `\wwwroot` rule

MonsterASP: **Folder `\wwwroot` is website root and all application files what can be accessible through web browser must be located in this directory.**

Upload the **full** `dotnet publish -c Release -o publish` output into `\wwwroot` (so `web.config` and DLLs sit in that folder too). Browser-facing static files from publish live in the **nested** `wwwroot` subfolder; routes like `/` are still served from the same site root. Do not upload only Git `wwwroot/` without publish binaries.

**Routes:** `GET /`, `GET /desk`, `GET|POST /register/{id}`

**Sources:** `EventDesk.fsproj`, `web.config`, `Program.fs`, `App.fs`, `Logic.fs`, `Html.fs`

CI: `.github/workflows/ci.yml`. Ignore `bin/`, `obj/`, `build/`, `_pubcheck/`.