# Text Adventure — Testing

## Groepsleden
- Jason Halder
- Oğuzhan Güner
- Jef Van Linden

## Projectbeschrijving
Een console-gebaseerd text adventure spel gebouwd in C# (.NET 8). De speler navigeert door kamers, verzamelt items en probeert het spel te winnen. Het project bevat een uitgebreide testaanpak met unit tests, integratietests en Gherkin/BDD tests.

## Hoe starten

### Vereisten
- .NET 8 SDK

### Het spel runnen
\\\ash
dotnet run --project TextAdventure
\\\

### Tests runnen
\\\ash
dotnet test TextAdventure.Tests
dotnet test TextAdventure.specs
\\\

## Wereld
| Kamer | Beschrijving |
|---|---|
| Start | Beginpositie van de speler |
| Dodelijke Gang (west) | Betreden = game over |
| Schatkamer (oost) | Bevat de sleutel |
| De Uitgang (noord) | Winnen met sleutel |
| Kelder (zuid) | Bevat het zwaard |
| Monsterkamer (dieper) | Monster — verslaan met zwaard |

## Commando's
| Commando | Beschrijving |
|---|---|
| help | Toon lijst met commando's |
| look | Toon huidige kamer |
| inventory | Toon je inventory |
| go n/e/s/w | Beweeg naar een kamer |
| take item | Pak een item op |
| fight | Vecht met het monster |
| quit | Stop het spel |

## Testaanpak
- Unit tests: BuildingTests, InventoryTests, ItemTests, RoomTests (25 tests)
- Gherkin/BDD tests: 9 scenario's in het Nederlands

## Klassestructuur
| Klasse | Verantwoordelijkheid |
|---|---|
| Building | Wereldbeheer en game logic |
| Room | Kamer met exits en items |
| Inventory | Items van de speler |
| Item | Object in het spel |
| Direction | Enum voor richtingen |
| GameSetup | Aanmaken van de wereld |
