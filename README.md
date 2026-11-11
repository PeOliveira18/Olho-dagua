# Olho d'Água — alerta comunitário de alagamentos

> Global Solution 2026.2 · FIAP · Engenharia de Software · CitizenTech

## Ficha da CitizenTech

| Campo | Resposta |
|---|---|
| **Nome** | Olho d'Água |
| **Cidade e bairro** | São Paulo — Jardim Pantanal, São Miguel Paulista (várzea do Rio Tietê) · *placeholder: trocar pelo bairro observado pela equipe* |
| **Cidadão beneficiado** | Morador de área de risco de alagamento — em especial idosos, pessoas com mobilidade reduzida e famílias que moram perto de córregos |
| **Problema, em uma frase** | Quem mora perto de córregos em São Paulo só descobre que a rua vai alagar quando a água já está entrando em casa, sem tempo para proteger pessoas, documentos e bens. |
| **Como funciona** | Sensores de baixo custo medem o nível do córrego e a chuva; quando o nível cruza um limite, o morador inscrito naquela área recebe um alerta no celular com o que fazer e o abrigo mais próximo. Os próprios moradores também reportam alagamentos pelo app, e relatos confirmados viram alerta comunitário. |
| **O que cada parte faz** | **ESP32 (IoT)** mede o nível · **Serviço de Telemetria (Java/SOA)** recebe leituras e detecta limites · **Serviço do Cidadão (C#)** gera alertas, guarda relatos, áreas e abrigos · **App (Expo)** mostra alertas, recebe relatos (funciona offline) e indica abrigos · **ML** prevê chuva forte a partir de dados do INMET · **Linux (SO)** provisiona e monitora os serviços |
| **Link do repositório** | https://github.com/PeOliveira18/Olho-dagua |
| **Indicador de impacto** | Minutos de antecedência entre o alerta e o transbordamento · nº de moradores alertados por evento · % de relatos confirmados pela comunidade |
| **ODS conectados** | **ODS 11** (meta 11.5: reduzir mortes e perdas por desastres) · **ODS 13** (meta 13.1: resiliência a riscos climáticos) · **ODS 10** (a população mais exposta é a de baixa renda em áreas de várzea) |

## Arquitetura

```mermaid
flowchart LR
    ESP[ESP32 no Wokwi<br/>sensor de nível + chuva] -- POST /api/leituras --> TEL
    subgraph SOA [Serviço de Telemetria · Java Spring Boot]
      TEL[API Telemetria] --> PGT[(PostgreSQL<br/>telemetria)]
    end
    TEL -- "nível cruzou limite<br/>POST /api/alertas/sensor" --> CID
    subgraph CS [Serviço do Cidadão · C# .NET]
      CID[API Cidadão] --> PGC[(PostgreSQL<br/>cidadao)]
    end
    APP[App Expo<br/>React Native + TS] -- alertas, abrigos, relatos --> CID
    APP -- nível atual do córrego --> TEL
    APP --- SQL[(SQLite local<br/>fila offline)]
    ML[ML · INMET<br/>previsão de chuva forte] -. alerta de previsão .-> CID
```

**Por que dois serviços:** cada um tem uma responsabilidade clara. A Telemetria só conversa com hardware (muitas leituras, regras de limite). O serviço do Cidadão conversa com pessoas (alertas, relatos, abrigos). Eles se integram por REST: é a parte "orientada a serviços" da entrega de SOA, e deixa as entregas de SOA e C# separadas.

<!-- ESTE-REPO:INICIO -->
## Este repositório: C# Software Development

Entrega da disciplina **C# Software Development**: o **Serviço do Cidadão**, uma Web API em .NET 10 com EF Core e PostgreSQL. É ele que guarda áreas de risco, alertas, relatos dos moradores, abrigos e inscrições, e é o serviço que o app consome.

```
Olho-dagua/
├── README.md                 ← esta Ficha
├── PLANO.md                  ← plano mestre do projeto (cópia oficial)
├── prompts.md                ← registro do uso de IA
├── infra/                    ← docker-compose do PostgreSQL
└── servico-cidadao-dotnet/   ← código da Web API (instruções no README da pasta)
```
<!-- ESTE-REPO:FIM -->

## Repositórios do projeto

| Parte | Disciplina | Repositório |
|---|---|---|
| Serviço do Cidadão (.NET) | C# Software Development | https://github.com/PeOliveira18/Olho-dagua |
| Serviço de Telemetria (Java) | SOA e Web Services | https://github.com/PeOliveira18/GlobalSolution-SOA |
| App do morador (Expo) | Mobile Development & IOT | https://github.com/PeOliveira18/GlobalSolution-Mobile |

## Plano do projeto

O planejamento completo (contratos entre serviços, portas, dados de seed, o que cada disciplina entrega e o cronograma até 19/11) está em **[PLANO.md](PLANO.md)**.

## Links das entregas

| Disciplina | Link |
|---|---|
| Vídeo do app (Mobile) | *a definir* |
| Projeto Wokwi (IoT) | *a definir* |
| Vídeo IoT | *a definir* |
| Vídeo ML | *a definir* |
| Vídeo Linux (OS) | *a definir* |
| Pitch (Testing) | *a definir* |

## Integrantes

| Nome | RM |
|---|---|
| Pedro Oliveira | 99943 |
| Debora Ivanowski | 555694 |
| Diego Cabral | 557817 |
