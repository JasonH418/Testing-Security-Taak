# Text Adventure — Testing

## Groepsleden
- Jason Halder
- Oguzhan Guner
- Jef Van Linden

## Projectbeschrijving
Een console-gebaseerd text adventure spel gebouwd in C# (.NET 8). De speler navigeert door kamers, verzamelt items en probeert het spel te winnen. Het project bevat een uitgebreide testaanpak met unit tests, integratietests en Gherkin/BDD tests.

## Hoe starten

### Vereisten
- .NET 8 SDK
- Visual Studio 2022

### Opstarten
1. Open de solution in Visual Studio
2. Stel **Multiple Startup Projects** in: zet zowel `TextAdventure` als `TextAdventureAPI` op **Start**
3. Druk op **F5**

## De wereld
Het spel bestaat uit 6 kamers:
- **Start** - beginpositie (midden)
- **Dodelijke Gang** (west) - betreden = game over
- **Schatkamer** (oost) - bevat een sleutel en versleutelde inhoud
- **De Uitgang** (noord) - enkel toegankelijk met sleutel, win
- **Kelder** (zuid) - bevat een zwaard
- **Monsterkamer** (dieper) - monster moet verslagen worden voor je weg kan

## Commando's
| Commando | Beschrijving |
|---|---|
| help | Toon lijst met commando's |
| look | Toon kamer, items en uitgangen |
| inventory | Toon inventory |
| go n/e/s/w | Beweeg naar een richting |
| take [item] | Pak een item op |
| fight | Vecht met het monster |
| unlock | Ontsleutel een versleutelde kamer |
| quit | Stop het spel |

## Testaanpak

### Unit tests
Alle klassen worden afzonderlijk getest:

- **ItemTests** - naam en beschrijving van items
- **InventoryTests** - toevoegen, opzoeken (case-insensitive), weergeven
- **RoomTests** - uitgangen toevoegen, items pakken (case-insensitive, verwijderen na pakken)
- **BuildingTests** - bewegen, game over, winnen, monster, noclip

### Integratietests
Testen dat klassen correct samenwerken via `GameIntegrationTests`:

- Sleutel oppakken -> deur openen
- Zwaard oppakken -> monster verslaan
- Monster verslaan -> veilig weggaan
- Zonder sleutel -> deur blijft dicht
- Admin noclip -> deur zonder sleutel passeren
- Volledig spelverloop van start tot winnen

### Gherkin/BDD tests
End-to-end scenario's via `GameFlow.feature`:

- Winnend pad doorlopen
- Verliezen door verkeerde kamer
- Monster verslaan