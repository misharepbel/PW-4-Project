# TeaShop Services

This solution contains several microservices including CatalogService and CartService. Kafka is used to broadcast product cache updates from the catalog to consumers.

## TODO

- Implement `InvoiceService`
- Create `PaymentService` or handle payments within `OrderService`
- Write unit and integration tests
- Send receipts via email after successful payments  
      *(consider using `NotificationService` or implement separately in each service)*
- Add ability for users to edit their account details
- Implement password reset functionality

## Running with Docker

Docker Compose includes SQL Server, Redis and a Kafka broker running in KRaft mode (no Zookeeper). Use the following command to start all services:

Create a `.env` file (see `.env.example`) with the required `SERVICE_CONNECTIONSTRING`,
Kafka settings and JWT keys, then run:

```bash
docker-compose up --build
```

Kafka will be available at `kafka:9092` for the services.
ApiGateway will be available at `localhost:8080/swagger`

## Running Tests

From the repository root run:

```bash
dotnet test CatalogService/CatalogService.UnitTests/CatalogService.UnitTests.csproj
dotnet test CatalogService/CatalogService.IntegrationTests/CatalogService.IntegrationTests.csproj
```

The commands restore packages and build the test projects automatically.

## Product Cache Events

CatalogService publishes a `ProductCacheEvent` with a list of all products whenever the application starts or when products are created, updated or deleted. CartService listens to this topic and keeps an in-memory cache which is used to validate product IDs before items can be added to a cart.

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

#### Unauthorized
- `GET /WeatherForecast` - Example endpoint
- `GET /WeatherForecast/TestOrderServiceResponse` - Proxy call to OrderService

## Example Flow

1. User registers -> `UserRegisteredEvent` -> welcome email
2. User adds items to cart -> `ProductCacheEvent` keeps cache updated
3. User checks out -> `CartCheckedOutEvent` on **cart-checked-out** -> OrderService creates the order and publishes `OrderCreatedEvent`
4. `OrderCreatedEvent` -> CartService clears the cart
5. Payment succeeds -> `OrderPaidEvent` -> order status updated and receipt email sent
