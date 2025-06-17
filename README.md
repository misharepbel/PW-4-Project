# TeaShop Services

This solution contains several microservices including CatalogService and CartService. Kafka is used to broadcast product cache updates from the catalog to consumers.

## Running with Docker

Docker Compose includes SQL Server, Redis and a Kafka broker running in KRaft mode (no Zookeeper). Use the following command to start all services:

Create a `.env` file (see `.env.example`) with the required `CONNECTIONSTRING`,
Kafka settings and JWT keys, then run:

```bash
docker-compose up --build
```

Kafka will be available at `kafka:9092` for the services.

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

