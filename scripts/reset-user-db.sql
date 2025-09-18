-- Drop and recreate user_db database
DROP DATABASE IF EXISTS user_db;
CREATE DATABASE user_db OWNER datingapp;

-- Grant permissions to datingapp user
GRANT ALL PRIVILEGES ON DATABASE user_db TO datingapp;

-- Connect to user_db and enable PostGIS
\c user_db;
CREATE EXTENSION IF NOT EXISTS postgis;