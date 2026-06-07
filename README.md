# Synapse

A real-time, AI-powered note-taking system that transforms user notes into structured summaries using a distributed, event-driven architecture with background workers, message queues, and live UI updates.


![HomePage](./docs/screenshots/homepage.png)

Event-driven note management system with AI-powered summarization, built using .NET and Azure.

## What This Project Demonstrates

Synapse is designed as a production-style distributed system showcasing:

- Event-driven backend architecture
- Asynchronous background processing (worker service)
- Real-time frontend updates (SignalR)
- Message queue decoupling (Azure Service Bus)
- Secure authentication (JWT)
- Full Dockerized local environment

---

## Key Highlights

- AI-powered note summarization and title generation
- Real-time UI updates via SignalR
- Event-driven architecture using Azure Service Bus
- Background worker for async processing
- JWT authentication with protected API routes
- Fully Dockerized (one-command startup)
- Clean separation between API, worker, and frontend

---

## Architecture
```User action triggers an event-driven pipeline:
	User creates note
│
▼
Backend API (.NET)
(validates + publishes message)
│
▼
Azure Service Bus Queue
│
▼
Worker Service (.NET)
(processes AI summarization)
│
▼
SignalR Hub
(pushes real-time update)
│
▼
Frontend updates instantly
```
## Tech Stack

### Backend
- ASP.NET Core Web API
- SignalR
- Entity Framework Core
- Azure Service Bus
- JWT Authentication

### Frontend
- React
- TypeScript
- Tailwind CSS
- React Router

### Infrastructure
- Docker 
- Azure SQL


## ScreenShots(TBD)
-Dashboard
-Login/Register
-Detail Page

		
## Getting Started (Docker - Recommended)
This project is fully containerized and should be run using Docker Compose.

---

### 1. Prerequisites

Make sure you have installed:

- Docker Desktop
- Docker Compose

---

### 2. Environment Variables

Create a `.env` file in the project root (`/src`):
	ServiceBus__ConnectionStrings=your_connection_string
	ServiceBus__QueueName=notesqueue

	DeepSeek__ApiKey=your_api_key
	DeepSeek__Model=deepseek-v4-flash
	DeepSeek__BaseUrl=https://api.deepseek.com

	Jwt__Key=your_jwt_secret

---

### 3. Build & Run

From the project root:

	docker compose up --build -d



---

### 4. Access the App

Frontend:
http://localhost:3000

Backend API:
http://localhost:8080

Swagger:
http://localhost:8080/swagger

---

### 5. Stop Services
	docker compose down


---

## Alternative (Manual Development Mode)

### Backend API

```bash
cd src/backend/Synapse.API
dotnet run

### Worker
```bash
cd src/backend/Synapse.Worker
dotnet run

### FrontEnd
```bash
cd src/frontend/synapse-web
npm install
npm run dev



## Future Roadmaps:
	- note tags auto generated
	- search function
	- Retry function
	- Refresh token authentication
	- Markdown support
	- file uploads
	- ...-> team collaboration tool

## Design Points: 
### Why use a worker service?

AI summarization can be time-consuming. Offloading processing to a worker prevents blocking API requests and improves scalability.

### Why SignalR?

SignalR enables real-time frontend updates when background processing completes.

### Why Azure Service Bus?

Using a message queue decouples API requests from background processing workloads.
