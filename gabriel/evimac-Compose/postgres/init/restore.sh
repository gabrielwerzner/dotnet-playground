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

echo "🔄 Restoring PostgreSQL Data backup..."
pg_restore -U postgres -d postgres /backup/evimacRelBackup.backup

echo "🔄 Restoring PostgreSQL ConfigDB backup..."
psql -v ON_ERROR_STOP=1 -U postgres -d postgres <<-'EOSQL'
  SELECT 'CREATE DATABASE "MaC_Config"'
  WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'MaC_Config')\gexec
EOSQL
pg_restore -U postgres -d MaC_Config /backup/MaC_Config.backup

echo "🔄 Restoring PostgreSQL MetaDB backup..."
psql -v ON_ERROR_STOP=1 -U postgres -d postgres <<-'EOSQL'
  SELECT 'CREATE DATABASE "MaC_Meta"'
  WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'MaC_Meta')\gexec
EOSQL
pg_restore -U postgres -d MaC_Meta /backup/MaC_Meta.backup

echo "✔ PostgreSQL restore complete"


