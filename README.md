# SmartGas API

RESTful API for the SmartGas web application.

This backend provides the services required by the SmartGas Web Application, including authentication, profile management, monitored zones, IoT sensors, sensor readings, incidents, alerts, notifications, subscriptions, settings, emergency contacts, dashboard data, and external weather information.

## Technologies

- ASP.NET Core
- C#
- Entity Framework Core
- PostgreSQL
- Swagger / OpenAPI
- Docker
- Render

## Main features

- User registration and authentication
- Profile and account settings management
- Emergency contact management
- Monitored zones management
- Sensor management
- IoT sensor readings
- Automatic incident creation from dangerous readings
- Alerts and notifications
- Dashboard summary
- Plans and subscriptions
- Subscription plan changes
- External weather integration
- Internationalized API messages
- Swagger documentation

## Local requirements

- .NET SDK 10.0
- Docker Desktop
- PostgreSQL container or PostgreSQL local database

## Local database

The project uses PostgreSQL.

Recommended local database configuration:

```txt
Database: smartgas_db
Username: smartgas_user
Password: smartgas_password
Port: 5432
