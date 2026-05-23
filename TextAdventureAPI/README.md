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
\\\ash
dotnet run --project TextAdventureAPI
\\\

### Console spel starten
\\\ash
dotnet run --project TextAdventure
\\\

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
- JWT signing key via environment variable JWT_SECRET

## Versleutelde kamers
- room1.enc — geheime wapenkamer
- room2.enc — schatkamer

## Swagger
Beschikbaar op http://localhost:49400/swagger tijdens development.
