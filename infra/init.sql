-- Executado só na primeira vez que o volume do Postgres é criado.
-- As tabelas NÃO são criadas aqui: quem cria é o EF Core (cidadao) e o Flyway (telemetria).
CREATE DATABASE cidadao;
CREATE DATABASE telemetria;
