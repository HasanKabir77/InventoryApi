# Inventory API

An ASP.NET Core 8.0 Web API for managing **Products, Categories, and
Authentication** with JWT.

------------------------------------------------------------------------

## Features

-   User **Registration & Login** with JWT Auth
-   **Products CRUD** with image upload, filtering, pagination
-   **Categories CRUD** with product count validation
-   Clean Architecture (Domain, Application, Infrastructure, API layers)
-   **EF Core DB-First** with MS SQL
-   **Swagger** for API documentation

------------------------------------------------------------------------

## Setup Instructions

### 1. Clone Repository

``` bash
git clone https://github.com/HasanKabir77/InventoryApi.git
cd InventoryApi
```

### 2. Configure Database

Edit `appsettings.json`:

``` json
"ConnectionStrings": {
  "DefaultConnection": "Server=[servername];Database=InventoryDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Apply EF Models (DB-First)

Run in terminal:

``` bash
dotnet ef dbcontext scaffold "Server=[servername];Database=InventoryDb;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Infrastructure/Data/Models -f
```

### 4. Run the API

``` bash
dotnet run
```

Server runs at:\
https://localhost:7105

------------------------------------------------------------------------

## Authentication

-   All endpoints except `Auth` require a **JWT token**.\
-   Use `/api/auth/login` to get a token.\
-   In Swagger UI → click **Authorize** → enter:\

```{=html}
<!-- -->
```
    Bearer <your_token_here>

------------------------------------------------------------------------

## API Documentation (Swagger)

Swagger is enabled by default.\
Open in browser:\
<https://localhost:7105/swagger>

### Example Endpoints

-   **Auth**
    -   `POST /api/auth/register`
    -   `POST /api/auth/login`
-   **Products**
    -   `GET /api/products`
    -   `POST /api/products` (with image upload)
    -   `PUT /api/products/{id}`
    -   `DELETE /api/products/{id}`
-   **Categories**
    -   `GET /api/categories`
    -   `POST /api/categories`
    -   `PUT /api/categories/{id}`
    -   `DELETE /api/categories/{id}`

------------------------------------------------------------------------

## License

MIT License
