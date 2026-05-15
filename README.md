# Product API Evaluation Task

This repository contains a small .NET 9 Web API created for a technical interview task.

The API uses Minimal APIs, an in-memory product repository, Swagger UI, and xUnit v3 tests.

## Requirements Covered

- Product model with:
  - Id
  - Name
  - Price
  - CategoryId

- Category model with:
  - Id
  - Name
  - Description

- Product filtering endpoint
- Product update endpoint
- Repository abstraction through `IProductRepository`
- Dependency injection
- Input validation
- Swagger UI
- Unit and endpoint tests

## Project Structure

```text
PoC
├── PoC
│   ├── Endpoints
│   │   └── ProductEndpoints.cs
│   ├── Http
│   │   ├── Products.Valid.http
│   │   ├── Products.Invalid.http
│   │   └── Swagger.http
│   └── Program.cs
│
├── PoC.Core
│   ├── Contracts
│   │   ├── ProductFilter.cs
│   │   └── UpdateProductRequest.cs
│   ├── Model
│   │   ├── Category.cs
│   │   └── Product.cs
│   └── Repository
│       ├── IProductRepository.cs
│       └── InMemoryRepository.cs
│
└── PoC.Tests
    ├── Endpoints
    │   └── ProductEndpointsTests.cs
    └── Repository
        └── InMemoryRepositoryTests.cs
```

## Running the API

From the solution root:

```bash
dotnet restore
dotnet build
dotnet run --project PoC
```

The HTTPS launch profile uses:

```text
https://localhost:7266
```

## Swagger

Swagger UI:

```text
https://localhost:7266/swagger
```

Swagger JSON:

```text
https://localhost:7266/swagger/v1/swagger.json
```

## Endpoints

### Get filtered products

```http
GET /api/products
```

Optional query parameters:

```text
name
categoryId
minPrice
maxPrice
```

Examples:

```http
GET /api/products
GET /api/products?name=lap
GET /api/products?categoryId=1
GET /api/products?minPrice=10&maxPrice=100
```

### Update product

```http
PATCH /api/products/{id}
```

Example request body:

```json
{
  "name": "Updated Laptop",
  "price": 999.99
}
```

Both fields are optional, but at least one must be provided.

Valid examples:

```json
{
  "name": "Mechanical Keyboard"
}
```

```json
{
  "price": 34.99
}
```

## Validation

The API handles invalid input gracefully.

Examples of invalid input:

- Negative minimum price
- Negative maximum price
- Minimum price greater than maximum price
- Empty update request
- Empty product name
- Negative product price
- Updating a non-existing product

## Architecture Notes

The API project does not directly access the data store.

Instead, the endpoint handlers depend on:

```csharp
IProductRepository
```

The current implementation is:

```csharp
InMemoryRepository
```

This makes the data access implementation replaceable without changing the endpoint code.

The API uses Minimal APIs to keep the solution simple and focused for the evaluation task.

## Tests

The solution uses xUnit v3.

The tests cover:

- Filtering by name
- Filtering by category
- Filtering by price range
- Updating product name and price
- Updating only name
- Updating only price
- Missing product update
- Endpoint behavior with a fake repository
- Endpoint validation responses

## Manual HTTP Tests

Manual request files are available in:

```text
PoC/Http
```

Files:

```text
Products.Valid.http
Products.Invalid.http
Swagger.http
```

These can be used from Visual Studio or another HTTP client that supports `.http` files.

## Technologies

- .NET 9
- ASP.NET Core Minimal APIs
- Swashbuckle Swagger UI
- xUnit v3
- Central Package Management
