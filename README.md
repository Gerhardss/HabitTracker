# Ieradumu pārvaldnieks HabitTracker
Lietotne, lai atzīmētu uzdevumus.

## Projekta struktūra
- **HabitTracker.Domain** — domēna modeļi (`Habit`, `HabitRecord`), repozitorija interfeiss (`IHabitRepository`) un sākotnējo testa datu ģenerators (`TestDataGenerator`).
- **HabitTracker.Infrastructure** — datu piekļuves slānis ar Entity Framework Core (`AppDbContext`), SQL Server repozitorija realizāciju (`EfHabitRepository`) un datubāzes migrācijām (`Migrations/`).
- **HabitTracker.Api** — ASP.NET Core Web API serveris, kas nodrošina REST galapunktus (`/api/habits`) un sazinās ar SQL Server datubāzi.
- **HabitTracker.Web** — Blazor tīmekļa lietotne, kas sazinās ar Web API caur HTTP (`WebHabitService`).
- **HabitTracker.Mobile** — .NET MAUI klienta lietotne (Windows / Android), kas sazinās ar Web API caur HTTP (`ApiHabitService`) un atbalsta bezsaistes (*offline*) režīmu.
- **HabitTracker.ConsoleApp** — sākotnējā konsoles lietotne testēšanai.

## Datubāze 4.md
- **Datubāze:** Microsoft SQL Server (LocalDB — `HabitTrackerDb`).
- **Datu integritāte:** Tabulām `Habits` un `HabitRecords` ir izveidota 1:N relācija (`ON DELETE CASCADE`), kā arī unikāls saliktais indekss (`HabitID` + `CompletionDate`), kas nepieļauj dubultus izpildes ierakstus vienam ieradumam vienā dienā.
- **Automātiska migrācija un testa dati:** Palaižot API serveri, automātiski izpildās `db.Database.Migrate()`, kas izveido datubāzi un tabulas pēc migrāciju failiem. Ja datubāze ir tukša, tajā automātiski tiek ielādēti sākotnējie testa dati no `TestDataGenerator`.

## Tīmekļa lietotne un sinhronizācija 5.md
- **Tīmekļa lietotne (`HabitTracker.Web`):** Izveidota Blazor lietotne, kas izmanto kopīgo `HabitTracker.Api` un SQL Server datubāzi (bez tiešas piekļuves datubāzei).
- **Funkcionalitāte:**
  - Aktīvo ieradumu saraksta apskate ar šodienas izpildes statusu un progresa skaitītāju (*Šodien izpildīti: X no Y*).
  - Jauna ieraduma izveide ar formas validāciju, esoša ieraduma labošana un arhivēšana.
  - Konkrēta ieraduma detaļu un pilnas izpildes vēstures (pa datumiem) apskate.
  - Ieraduma atzīmēšana kā izpildīta šodien, ielādes indikatori, apstiprinājuma paziņojumi un kļūdu apstrāde, ja API nav pieejams.
- **Koplietošana un bezsaistes režīms (`HabitTracker.Mobile`):** Abas lietotnes (`HabitTracker.Web` un `HabitTracker.Mobile`) strādā ar vienu un to pašu API, tāpēc veiktās izmaiņas pēc datu atjaunošanas uzreiz ir redzamas abās platformās. Mobilajai lietotnei papildus pievienota manuāla datu atjaunošana, šodienas progresa skaitītājs un bezsaistes (*offline*) kešatmiņa (`habits_cache.json`) ar darbību rindu (`pending_actions.json`), kas automātiski sinhronizē izmaiņas, kad savienojums ar API atjaunojas.

## Projekta palaišana
1. Atver risinājumu `HabitTracker.sln` programmā Visual Studio.
2. **Solution Explorer** logā ar peles labo pogu uzklikšķini uz `Solution 'HabitTracker'` -> **Properties** -> **Multiple startup projects** un iestati **Action = Start** gan `HabitTracker.Api` (pirmais sarakstā), gan `HabitTracker.Web` un/vai `HabitTracker.Mobile`.
3. Nospied **Start (F5)**:
   - Vispirms startēsies `HabitTracker.Api` (klausās uz `http://localhost:5294`), izveidos SQL Server datubāzi un ielādēs testa datus.
   - Uzreiz pēc tam atvērsies `HabitTracker.Web` (pārlūkprogrammā) un/vai `HabitTracker.Mobile` lietotne un ielādēs aktīvos ieradumus no datubāzes.
  

//Sagatavots kopā ar gemini
