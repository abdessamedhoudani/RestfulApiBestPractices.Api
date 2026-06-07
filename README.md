# RestfulApiBestPractices.Api

API exemple démontrant des bonnes pratiques pour construire une API RESTful légère en .NET.

## Description

Cette solution expose un ensemble d'endpoints CRUD pour gérer des `Product`. Elle met en œuvre :
- API minimaliste (Minimal APIs)
- Validation des DTOs (`CreateProductRequest`, `UpdateProductRequest`, `PatchProductRequest`)
- Pagination pour la liste des produits
- Gestion centralisée des erreurs (Problem Details)
- Documentation OpenAPI et UI interactive

## Caractéristiques principales

- Routes versionnées sous `/api/v1`
- Endpoints : GET (list + pagination), GET by id, POST, PUT, PATCH, DELETE
- Service en mémoire (`ProductService`) pour exemples et tests rapides

## Prérequis

- .NET SDK compatible (le projet cible `net10.0` dans le repo)

Vérifier la version installée :

```
dotnet --version
```

## Exécuter l'application

Depuis le répertoire du projet :

```
dotnet restore
dotnet build
dotnet run
```

L'application démarre sur l'URL par défaut (ex. `http://localhost:5000` ou `https://localhost:7042`). Après démarrage, la spécification OpenAPI est exposée (via `MapOpenApi`) et une UI interactive est disponible (via `MapScalarApiReference`). Ouvrez l'URL racine de l'application ou `/openapi` pour accéder à la documentation.

## Endpoints

Base path : `/api/v1`

- GET `/products`
	- Description : Récupère la liste paginée des produits
	- Query : `page` (int, défaut 1), `pageSize` (int, défaut 20, max 100)
	- Exemple :

```
curl "http://localhost:5000/api/v1/products?page=1&pageSize=10"
```

- GET `/products/{id}`
	- Description : Récupère un produit par `id`
	- Réponses : `200 OK` (produit) ou `404 Not Found`

- POST `/products`
	- Description : Crée un nouveau produit
	- Body (JSON) : `CreateProductRequest`
	- Exemple :

```
curl -X POST http://localhost:5000/api/v1/products \
	-H "Content-Type: application/json" \
	-d '{"name":"Phone","description":"Smartphone","price":499.99,"stock":150,"category":"Electronics"}'
```

- PUT `/products/{id}`
	- Description : Remplace un produit existant (entité complète)
	- Body (JSON) : `UpdateProductRequest`
	- Réponses : `204 No Content` ou `404 Not Found`

- PATCH `/products/{id}`
	- Description : Met à jour partiellement un produit (`PatchProductRequest`)
	- Réponses : `204 No Content` ou `404 Not Found`

- DELETE `/products/{id}`
	- Description : Supprime un produit
	- Réponses : `204 No Content` ou `404 Not Found`

## DTOs (aperçu)

- `CreateProductRequest`
	- `Name` (string, required, max 200)
	- `Description` (string, required)
	- `Price` (decimal, min 0.01)
	- `Stock` (int, min 0)
	- `Category` (string?)

- `UpdateProductRequest` — mêmes propriétés que `CreateProductRequest`
- `PatchProductRequest` — mêmes champs mais optionnels
- `ProductResponse` — représentation renvoyée par l'API (Id, Name, Description, Price, Stock, Category, CreatedAt, UpdatedAt)

## Gestion des erreurs

L'application utilise des réponses au format Problem Details pour standardiser les erreurs HTTP. Les endpoints retournent `404` lorsque la ressource est introuvable et `400` pour les erreurs de validation des DTOs.

## Documentation OpenAPI

La spec OpenAPI est exposée automatiquement. Une UI interactive est fournie pour tester les endpoints depuis le navigateur.

## Développement & tests rapides

- Le service `ProductService` est en mémoire et pré-rempli d'exemples. Idéal pour tests manuels et développement rapide.
- Pour modifier/étendre le comportement métier, implémentez ou remplacez `IProductService`.

## Fichiers clés

- `Program.cs` : configuration des routes et du pipeline
- `Services/ProductService.cs` : implémentation d'exemple en mémoire
- `DTOs/` : modèles de requêtes et réponses
- `Models/` : entités (ex. `Product`)

## Contribution

Les contributions sont bienvenues : ouvrez une issue ou un pull request pour proposer des améliorations.

---

Si vous voulez, je peux :
- ajouter des exemples de tests automatisés
- documenter la spec OpenAPI avec exemples de réponses
- fournir des commandes `docker` pour conteneuriser l'API
