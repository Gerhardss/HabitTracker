# Ieradumu pārvaldienks HabitTracker

Lietotne lai atzīmētu uzdevumus

## Projekta struktūra

* **`HabitTracker.Domain`** — domēna modeļi (`Habit`, `HabitRecord`), repozitorija interfeiss (`IHabitRepository`) un sākotnējo testa datu ģenerators (`TestDataGenerator`).
* **`HabitTracker.Infrastructure`** — datu piekļuves slānis ar Entity Framework Core (`AppDbContext`), SQL Server repozitorija realizāciju (`EfHabitRepository`) un datubāzes migrācijām (`Migrations/`).
* **`HabitTracker.Api`** — ASP.NET Core Web API serveris, kas nodrošina REST galapunktus (`/api/habits`) un sazinās ar SQL Server datubāzi.
* **`HabitTracker.Mobile`** — .NET MAUI klienta lietotne (Windows / Android), kas sazinās ar Web API caur HTTP (`ApiHabitService`).
* **`HabitTracker.ConsoleApp`** — sākotnējā konsoles lietotne testēšanai.

## Datubāze 4.md

* **Datubāze:** Microsoft SQL Server (LocalDB — `HabitTrackerDb`).
* **Datu integritāte:** Tabulām `Habits` un `HabitRecords` ir izveidota `1:N` relācija (`ON DELETE CASCADE`), kā arī unikāls saliktais indekss (`HabitID` + `CompletionDate`), kas nepieļauj dubultus izpildes ierakstus vienam ieradumam vienā dienā.
* **Automātiska migrācija un testa dati:** Palaižot API serveri, automātiski izpildās `db.Database.Migrate()`, kas izveido datubāzi un tabulas pēc migrāciju failiem. Ja datubāze ir tukša, tajā automātiski tiek ielādēti sākotnējie testa dati no `TestDataGenerator`.

## Projekta palaišana

1. Atver risinājumu **`HabitTracker.sln`** programmā Visual Studio.
2. **Solution Explorer** logā ar peles labo pogu uzklikšķini uz **`Solution 'HabitTracker'`** -> **Properties** -> **Multiple startup projects** un iestati **Action = Start** gan **`HabitTracker.Api`** (pirmais sarakstā), gan **`HabitTracker.Mobile`** (otrais sarakstā).
3. Nospied **Start (F5)**:
   * Vispirms startēsies `HabitTracker.Api` (klausās uz `http://localhost:5294`), izveidos SQL Server datubāzi un ielādēs testa datus.
   * Uzreiz pēc tam atvērsies `HabitTracker.Mobile` lietotne un ielādēs aktīvos ieradumus no datubāzes.


