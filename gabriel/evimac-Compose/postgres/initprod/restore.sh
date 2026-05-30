#!/bin/bash
set -e

echo "⏳ Waiting for PostgreSQL..."
until pg_isready -U postgres -d postgres; do
  sleep 1
done

echo "📦 Creating Keycloak database if not exists..."
psql -v ON_ERROR_STOP=1 -U postgres -d postgres <<-'EOSQL'
  SELECT 'CREATE DATABASE keycloak'
  WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'keycloak')\gexec
EOSQL

echo "✔ PostgreSQL restore complete"
