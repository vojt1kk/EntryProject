# Triáž AI gramotnosti ve zdravotnictví

## Postup spuštění (krok za krokem)

```bash
# 1) Klon repository
git clone https://github.com/vojt1kk/EntryProject.git
cd EntryProject

# 2) Spuštění databáze (PostgreSQL 16 v Dockeru)
docker compose up -d
#    Connection string je v EntryProject/appsettings.json (Host=localhost;Port=5432).

# 3) Spuštění aplikace
dotnet run --project EntryProject

#    Při startu se AUTOMATICKY:
#      - aplikují EF Core migrace (vytvoří tabulku triage_results),
#      - spustí sql/02_procedures.sql (sp_insert_triage_result + fn_role_statistics, CREATE OR REPLACE),
#      - při prázdné tabulce nahraje testovací data sql/03_seed.sql (150 řádků).
#    Ruční `dotnet ef database update` NENÍ potřeba.

# 4) Otevřete v prohlížeči adresu z logu, např.:
#    http://localhost:5199
```

Zastavení databáze: `docker compose down` (data zůstanou ve volume) nebo
`docker compose down -v` (smaže i data).

Spuštění testů: `dotnet test` (pokrývají scoring logiku — hraniční hodnoty a přechody úrovní).

---

## Práce s AI

### Které nástroje jsem použil
- **Claude Code** (model Opus/Sonnet) jako hlavní asistent — plánování i implementace.
- Vlastní workflow nad Claude Code: řízená pipeline **assess → deep-interview → konsensuální
  plán (Planner/Architect/Critic) → implementace → verifikace**.

### Jak jsem postupoval (klíčové prompty, pravidla typu CLAUDE.md)
1. **Vyhodnocení rozsahu** a strukturovaný rozhovor, který vyjasnil technická rozhodnutí
   (framework, databáze, způsob práce se schématem) dřív, než padla první řádka kódu.
2. **Kontrola hotového plánu** — na hotový plán jsem pustil 2 AI kontrolory, kteří odhalili
   nedostatky. Pro zajímavost dva konkrétní příklady, co našli: lokálně chybí .NET 10 SDK →
   retarget na .NET 9; a že SQL schéma nemůže být zároveň autorita nad strukturou i něco, co
   appka reálně volá → vyřešeno tak, že schéma řídí EF migrace a genuine jsou jen procedury +
   seed, které appka reálně spouští.
3. **Implementace** — jednotlivé fáze tasku (plánování, kontrola plánu, samotná implementace)
   běžely přes multiagentní pipeline, každá fáze měla svého specializovaného agenta; samotná
   implementace pak v jednom sekvenčním tracku (od nuly, sdílený build) + průběžné buildy.
4. **Kontrola kódu** — po dokončení implementace jsem prošel veškerý kód, abych odhalil
   případné nedostatky. Vzhledem k detailnosti plánovací fáze jsem na žádné nenarazil, jen jsem
   zvažoval pár stylistických doplňků — například důsledné oddělení modelů, enumů a services do
   vlastních souborů/složek. Nakonec jsem to nedělal — vzhledem k velikosti projektu současná
   struktura nebrání přehlednosti.
5. **Verifikace end-to-end** po každé větší změně.

Globální pravidla, kterými se AI řídila (`~/.claude/CLAUDE.md`), a klíčové prompty v rámci
konverzace:
- „Nikdy necommituj ani nenabízej commit — git si řídím sám." → AI do gitu nikdy nezasahovala,
  ani při rozdělování změn do commitů (jen navrhla rozdělení a příkazy, spouštěl jsem je já).
- „Preferuj .NET best-practices; SQL požadavek splň **funkčně**, ne naoko." → schéma řídí
  EF migrace (idiomatický přístup v .NET), SQL procedury a funkce ale aplikace reálně volá
  při každém zápisu výsledku i zobrazení statistik, ne jen jako dekorativní přílohu.
- „Ověřuj aktuální dokumentaci, neřeš zpaměti."

### Co jsem po AI kontroloval
- **Průběh celé aplikace lokálně** — otestoval jsem celý flow (role → test → výsledek →
  statistiky) a ověřil, že hodnoty zobrazené na frontendu sedí s tím, co počítá a ukládá
  backend (skóre, úroveň, doporučení, statistiky dle role).
- **Port databáze** — 5432 dočasně kolidoval s jiným běžícím kontejnerem, přemapováno na 5433
  a po vypnutí toho kontejneru vráceno zpět na standardní 5432.
- **Že procedura je opravdu volaná** — ověřeno přejmenováním procedury: zápis pak selže
  (`42883: procedure ... does not exist`), tzn. cesta nevede přes EF `SaveChanges`.
- **Celou codebase** — prošel jsem celý kód, aby vše sedělo (názvy, provázanost mezi
  vrstvami, žádné pozůstatky po předchozích úpravách).

### Co bych si z postupu uložil pro příště
- Důkladné rozebírání plánu před samotnou implementací — eliminuje velkou část chyb ještě
  předtím, než se vůbec dostanou do kódu.
- Spouštění advisorů s čistým kontextem, kteří se zaměří jen na konkrétní problematické
  části plánu, místo jednoho kontrolora řešícího všechno najednou.
