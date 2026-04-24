# EventCart

EventCart is a distributed, event-driven e-commerce simulation designed to explore microservices architecture using Kafka.

The project focuses on solving real-world challenges such as eventual consistency, asynchronous communication, and failure handling across distributed services.

## 🚀 Purpose

This project is built to demonstrate how independent services can collaborate through events instead of direct communication, ensuring scalability and resilience.

It simulates a simplified commerce flow where:

* Orders are created
* Stock is reserved
* Payments are processed
* Failures trigger compensating actions

## 🧠 Key Concepts

* Microservices architecture
* Event-driven communication
* Apache Kafka messaging
* Eventual consistency
* Idempotency
* Partitioning and ordering
* Failure handling and compensation

## 🧩 Services

### Order Service

Responsible for:

* Creating orders
* Publishing order events
* Tracking order state

### Inventory Service

Responsible for:

* Reserving stock
* Restoring stock on failure
* Reacting to order and payment events

### Payment Service

Responsible for:

* Processing payments (simulated)
* Publishing payment results (approved/failed)

## 🔄 Event Flow

### Happy Path

1. OrderCreated
2. StockReserved
3. PaymentApproved

### Failure Flow

1. OrderCreated
2. StockReserved
3. PaymentFailed
4. StockRestored

## 🛠️ Tech Stack

* .NET (ASP.NET Core Web API)
* Apache Kafka
* Docker (for local infrastructure)

## 📦 Project Structure

```
event-cart/
  services/
    order-service/
    inventory-service/
    payment-service/
  shared/
    contracts/
  docker/
```

## ⚠️ Important Notes

* Each service is designed to be independent
* Communication is done via events (Kafka)
* No shared database between services
* Consistency is eventual, not immediate

## 🎯 Goals

This project is not meant to be production-ready, but rather a learning environment to deeply understand distributed systems behavior.

## 📌 Future Improvements

* Saga pattern implementation
* Outbox pattern
* Observability (logs, tracing)
* Dead Letter Queue (DLQ)
* Retry strategies

## 🧪 Status

Work in progress – evolving step by step to introduce complexity gradually.

---

Feel free to explore, modify, and break things — that's part of the learning process.
