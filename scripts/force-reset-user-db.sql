-- Terminate all connections to user_db
SELECT pg_terminate_backend(pid) 
FROM pg_stat_activity 
WHERE datname = 'user_db' AND pid != pg_backend_pid();

-- Drop and recreate user_db database
DROP DATABASE IF EXISTS user_db;
CREATE DATABASE user_db OWNER datingapp;

-- Grant permissions to datingapp user
GRANT ALL PRIVILEGES ON DATABASE user_db TO datingapp;