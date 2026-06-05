# Atelier API

---

## Stack technique

- **.NET 10** — ASP.NET Core Web API
- **Entity Framework Core 10** + **SQLite** — persistence (base auto-créée et seedée au démarrage)
- **JWT Bearer** — authentification et autorisation par rôle
- **Swashbuckle** — documentation Swagger avec annotations custom
- **MSTest + Moq** — tests unitaires
- **Docker** — conteneurisation
- **Azure Container Apps** — déploiement cloud via GitHub Actions

---

## Lancer le projet en local

### Prérequis

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Démarrage

```bash
git clone <url-du-repo>
cd Atelier/Api/Atelier.Api
dotnet run
```

L'API démarre sur `https://localhost:57120`. La base SQLite (`Tennis.db`) est créée automatiquement et seedée avec les données initiales au premier lancement.

### Via Docker

```bash
docker build -f Api/Atelier.Api/Dockerfile -t atelier-api .
docker run -p 8080:8080 \
  -e Jwt__Key="your_secret_key_here" \
  -e Jwt__Issuer="AtelierIssuer" \
  -e Jwt__Audience="AtelierAudience" \
  -e Jwt__ExpiryMinutes="480" \
  atelier-api
```

---

## Lancer les tests

```bash
cd UnitTests/Atelier.Api.Tests
dotnet test
```

Les tests couvrent les services (`PlayerService`, `StatsService`, `AuthService`, `JwtService`), le calculator (`StatsCalculator`), les middlewares et les helpers.

---

## Authentification

Certains endpoints nécessitent un token JWT. Deux comptes sont disponibles par défaut :

| Utilisateur | Mot de passe | Rôle  |
|-------------|--------------|-------|
| `admin`     | `admin123!`  | Admin |
| `user`      | `user123!`   | User  |

**Obtenir un token :**

```http
POST /api/auth/token
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123!"
}
```

Utilisez ensuite le token retourné dans le header `Authorization: Bearer <token>`.

---

## Endpoints

### Joueurs — `GET /api/v1/players`

Retourne la liste de tous les joueurs, triée par rang croissant, séparée par genre.

```json
{
  "male": [{ "id": 17, "name": "Rafael Nadal" }, ...],
  "female": [{ "id": 102, "name": "Serena Williams" }, ...]
}
```

---

### Joueur — `GET /api/v1/players/{id}`

Retourne les informations complètes d'un joueur par son ID.

```json
{
  "id": 17,
  "firstName": "Rafael",
  "lastName": "Nadal",
  "shortName": "R.NAD",
  "sex": "M",
  "country": { "code": "ESP", "picture": "https://..." },
  "picture": "https://...",
  "data": {
    "rank": 1,
    "points": 1982,
    "weight": 85000,
    "height": 185,
    "age": 33,
    "last": [1, 0, 0, 0, 1]
  }
}
```

---

### Statistiques — `GET /api/v1/stats`

Retourne les statistiques globales.

```json
{
  "bestCountry": "SRB",
  "averageBmi": 23.36,
  "medianHeight": 185.0
}
```

- **bestCountry** : pays avec le meilleur ratio de victoires (moyenne des win rates individuels par pays)
- **averageBmi** : IMC moyen de tous les joueurs (`poids(kg) / taille(m)²`)
- **medianHeight** : médiane des tailles en cm

---

### Créer un joueur — `POST /api/v1/players`

> Requiert un token JWT avec le rôle **Admin**.

```http
Authorization: Bearer <token>
Content-Type: application/json

{
  "firstName": "Carlos",
  "lastName": "Alcaraz",
  "sex": "M",
  "picture": "https://example.com/alcaraz.png",
  "countryCode": "ESP",
  "data": {
    "rank": 1,
    "points": 9000,
    "weight": 75000,
    "height": 185,
    "age": 21,
    "last": [1, 1, 0, 1, 1]
  }
}
```

- `sex` : `"M"` ou `"F"`
- `countryCode` : code ISO 3 lettres — si le pays n'existe pas en base, il est créé automatiquement
- `weight` : en grammes
- `height` : en centimètres
- `last` : tableau de résultats récents (`1` = victoire, `0` = défaite)

Retourne `201 Created` avec les données du joueur créé.

---

### Authentification — `POST /api/auth/token`

Retourne un token JWT. Voir section [Authentification](#authentification).

---

## Déploiement

Le déploiement est automatisé via GitHub Actions (`.github/workflows/ppl-atelier-api.yml`) :

1. Push sur `main`
2. Build et push de l'image Docker vers Azure Container Registry
3. Déploiement sur Azure Container Apps

La clé JWT et les secrets sont injectés via les variables d'environnement Azure, jamais stockés dans le code.
