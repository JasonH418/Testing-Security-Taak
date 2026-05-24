# Text Adventure — Security

## Groepsleden
- Jason Halder
- Oğuzhan Güner
- Jef Van Linden

## Projectbeschrijving
Uitbreiding van het Text Adventure spel met security features: gebruikersbeheer via een Minimal API, JWT authenticatie, wachtwoordhashing, X.509/CMS encryptie en secure coding.

## Hoe starten

### Vereisten
- .NET 8 SDK
- Windows (voor X.509 certificaatstore)

### API starten
```bash
dotnet run --project TextAdventureAPI
```

### Console spel starten
```bash
dotnet run --project TextAdventure
```

## API Endpoints
| Endpoint | Methode | Beschrijving |
|---|---|---|
| /api/auth/register | POST | Gebruiker registreren |
| /api/auth/login | POST | Inloggen en JWT ophalen |
| /api/auth/me | GET | Huidige gebruiker opvragen |
| /api/encryption/encrypt | POST | Tekst versleutelen |
| /api/encryption/decrypt | POST | Tekst ontsleutelen |
| /api/rooms/{id}/keyshare | GET | Keyshare ophalen |
| /api/rooms/unlock | POST | Kamer ontsleutelen |

## Security Features
- Wachtwoorden gehasht met SHA-256
- Lockout na 3 mislukte loginpogingen
- JWT tokens voor beveiligde endpoints
- Rollen: Player en Admin
- Admin heeft noclip (kan door vergrendelde deuren)
- X.509/CMS encryptie voor kamers (.enc bestanden)
- JWT signing key via appsettings.json (niet hardcoded)

## Versleutelde kamers
De `.enc` bestanden worden gegenereerd met het `GenerateEnc` project. Dit project gebruikt het X.509 certificaat van de huidige machine om de kamers te versleutelen.

### .enc bestanden genereren
```bash
dotnet run --project GenerateEnc
```
Dit maakt `room1.enc` en `room2.enc` aan in de root van het project. Deze bestanden zijn machine-specifiek — ze kunnen alleen gedecrypteerd worden op de machine waarop ze gegenereerd zijn.

- room1.enc — geheime wapenkamer (passphrase: dragonslayer)
- room2.enc — schatkamer (passphrase: shadowkey)

## Swagger
Beschikbaar op https://localhost:49399/swagger tijdens development.