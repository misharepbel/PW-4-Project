# TeaShop Services

This solution contains several microservices including CatalogService and CartService. Kafka is used to broadcast product cache updates from the catalog to consumers.

## Running with Docker

Docker Compose includes SQL Server, Redis and a Kafka broker running in KRaft mode (no Zookeeper). Use the following command to start all services:

```bash
docker-compose up --build
```

Kafka will be available at `kafka:9092` for the services.

## Product Cache Events

CatalogService publishes a `ProductCacheEvent` with a list of all products whenever the application starts or when products are created, updated or deleted. CartService listens to this topic and keeps an in-memory cache which is used to validate product IDs before items can be added to a cart.

