full:
	services
	infrastructure
	telemetry

services:
	docker-compose up -d user-service wish-service --build

infrastructure:
	docker-compose up -d postgres neo4j rabbit

telemetry:
	docker-compose up -d node_exporter prometheus grafana
