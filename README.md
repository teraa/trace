# Trace
- The API part of the project is WIP

## Minimal Docker example

```yaml
services:
  trace:
    container_name: trace-app
    image: ghcr.io/teraa/trace:master
    environment:
      - Db__ConnectionString=Host=trace-db;Port=5432;Database=trace;Username=postgres;Password=example;Include Error Detail=true;Command Timeout=60
      - PubSub__Token=x
    depends_on:
      - db

  db:
    container_name: trace-db
    image: docker.io/postgres:16
    environment:
      - POSTGRES_PASSWORD=example
    ports:
      - 127.0.0.1:5432:5432
```
