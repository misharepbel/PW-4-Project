# TeaShop Microservices

## TeaShop Services
- **ApiGateway** - YARP reverse proxy exposing a unified Swagger UI and routing requests to backend services.
- **CatalogService** - CRUD operations for products and categories. Publishes catalog updates via Kafka.
- **CartService** - Stores shopping carts in Redis and validates products from the catalog cache. Emits checkout events.
- **OrderService** - Persists orders in SQL Server and updates order statuses when payments complete.
- **PaymentService** - Processes payments and publishes `OrderPaid` events.
- **UserService** - Handles user registration, login, password reset and issues JWT tokens. Enforces roles.
- **NotificationService** - Sends emails for registration, password reset and payment receipts.

## Tech Stack
- **.NET 8 / ASP.NET Core** – microservice APIs.
- **Entity Framework Core & SQL Server** – relational storage for orders, users and catalog.
- **Redis** – cart storage.
- **Apache Kafka** – asynchronous communication between services.
- **MediatR** – simplifies request/response handling inside services.
- **YARP Reverse Proxy** – gateway routing and aggregated Swagger docs.
- **Docker & Docker Compose** – containerised deployment.

## Running with Docker
1. Copy `.env.example` to `.env` and fill in connection strings, Kafka topics and JWT keys.
2. Build and launch all services:
   ```bash
   docker-compose up --build
   ```
3. Open `http://localhost:8080/swagger` to access the gateway and select a service cluster.

Stop the stack with `docker-compose down`.

## Running Tests
From the repository root run:
```bash
dotnet test CatalogService/CatalogService.UnitTests/CatalogService.UnitTests.csproj
dotnet test CatalogService/CatalogService.IntegrationTests/CatalogService.IntegrationTests.csproj
```
The commands restore packages and build the test projects automatically.

## Requirements and Implementation
- [x] **Grade 3.0** – Customer details retrieval, Product CRUD, Category CRUD, Cart CRUD, SINGLE SERVICE - Initially provided by a monolithic app combining User, Catalog and Cart features.
- [x] **Grade 3.5** – All of Grade 3.0 plus unit and integration tests - CatalogService contains dedicated test suites.
- [x] **Grade 4.0** – All of Grade 3.5 plus JWT with registration, login, password reset, account editing, user roles, cart processing and email receipts - Implemented across UserService, CartService, OrderService and NotificationService.
- [x] **Grade 4.5** – All of Grade 4.0 deployed as microservices (at least three services) - ApiGateway and six domain services communicate via Kafka.
- [x] **Grade 5.0** – All of Grade 4.5 packaged as Docker images and using technologies like Kafka or MediatR - Docker Compose builds the stack and services use Kafka and MediatR for messaging.
## Product Cache Events
CatalogService publishes a `ProductCacheEvent` containing all products whenever the catalog changes or on startup. CartService subscribes to this topic and keeps an in-memory cache to validate product IDs before items are added to a cart.

## API Endpoints
Below is a short description of the available endpoints in each service and how they are secured.

### CatalogService
#### Admin only
- `POST /Categories` - Create a category
- `POST /Products` - Create a product
- `PUT /Products/{id}` - Update a product
- `DELETE /Products/{id}` - Delete a product

#### Admin and user
- _None_

#### Unauthorized
- `GET /Categories` - List all categories
- `GET /Products` - List all products
- `GET /Products/{id}` - Get product by id
- `GET /` - Health check

### CartService
#### Admin only
- `GET /admin` - Get all carts
- `GET /admin/{userId}` - Get cart by user id
- `POST /admin/{userId}/additem` - Add item to user's cart
- `DELETE /admin/{userId}/removeitem/{productId}` - Remove item from user's cart
- `DELETE /admin/{userId}` - Clear user's cart

#### Admin and user
- `GET /cached` - Show cached products
- `GET /mycart` - Get current user's cart
- `POST /item` - Add item to cart
- `DELETE /item/{productId}` - Remove item from cart
- `DELETE /mycart` - Clear current cart
- `POST /checkout` - Checkout cart

#### Unauthorized
- `GET /` - Health check

### OrderService
#### Admin only
- `GET /get` - Get all orders
- `PUT /status/{id}` - Update order status
- `DELETE /delete/{id}` - Delete order

#### Admin and user
- `GET /get/{id}` - Get order by id (only if owner or admin)
- `GET /my` - Get current user's orders

#### Unauthorized
- `GET /` - Health check

### UserService
#### Admin only
- `GET /admin-test` - Example endpoint for admins
- `GET /{id}` - Get user by id
- `GET /all` - List all users

#### Admin and user
- `GET /me` - Get current user's details

#### Unauthorized
- `GET /` - Health check
- `POST /register` - Register a new user
- `POST /login` - Log in and obtain JWT
- `POST /password-reset` - Request password reset
- `POST /reset-password` - Reset password with token

### ApiGateway
The gateway exposes `GET /swagger` for API discovery and forwards all other requests to backend services.

## Detailed Event Flow
1. User registration → `UserRegisteredEvent` → NotificationService sends a welcome email.
2. Password reset → `PasswordResetEvent` → NotificationService delivers a reset link.
3. Catalog modifications → `ProductCacheEvent` → CartService refreshes its product cache.
4. Cart checkout → `CartCheckedOutEvent` → OrderService creates an order and CartService clears the cart.
5. Payment → `OrderPaidEvent` → OrderService marks the order paid and NotificationService emails the receipt.
