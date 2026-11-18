# PLANO MESTRE — Olho d'Água (GS CitizenTech 2026.2)

> **Para quem está lendo (pessoa ou IA em outro chat):** este arquivo é a fonte única de verdade do projeto. Cada matéria tem a sua seção, que pode ser trabalhada sozinha. Os **nomes, portas, IDs e contratos da Parte B são fixos**: não mude sem atualizar este arquivo, senão as partes param de se encaixar.
>
> Nenhuma decisão aqui deve ser rediscutida sem motivo. Se algo não estiver definido, escolha o mais simples e registre a escolha na seção da matéria.

---

## Índice

- **Parte A — Contexto:** o desafio, a solução, regras da GS
- **Parte B — Decisões técnicas compartilhadas:** repositório, portas, dados de seed, contratos
- **Parte C — Uma seção por matéria**
  - C1 · C# Software Development (Serviço do Cidadão)
  - C2 · SOA e Web Services (Serviço de Telemetria, Java)
  - C3 · Mobile Development & IOT (App Expo)
  - C4 · Physical Computing: IOT & IOB (ESP32 / Wokwi)
  - C5 · Inteligência Artificial & Machine Learning
  - C6 · Operating Systems (Linux)
  - C7 · Cybersecurity
  - C8 · Testing, Compliance & QA (Azure Boards + pitch)
- **Parte D — Cronograma e ordem de dependência**
- **Parte E — Prompts prontos para abrir cada chat**
- **Parte F — Roteiro de código: C#, SOA e Mobile** (etapas, checkpoints e prompts)

---

# PARTE A — CONTEXTO

## A1. O desafio (resumo do PDF da FIAP)

- **Tema:** CitizenTech, ou seja, tecnologia criada a partir de um problema real de uma cidade, que entrega benefício concreto a um cidadão.
- **Regra central:** **uma única solução por equipe atravessa todas as disciplinas.** Cada disciplina constrói uma parte, e **as partes conversam entre si**.
- **Prazo:** **19/11/2026, 23h55**, entregue no Teams de cada disciplina. 20/11 é feriado.
- **Equipe:** até 5 alunos da mesma turma. Um integrante envia cada disciplina.
- **Notas:** cada disciplina dá de 0 a 10, separadamente.
- **Pitch:** vídeo de até **4 minutos** no YouTube, enviado na disciplina de **Testing**. Concorre ao prêmio de melhor GS.
- **Combinados obrigatórios:**
  - **README com a Ficha da CitizenTech no topo.**
  - **`prompts.md`** registrando os prompts de IA usados.
  - **Nada de dados pessoais reais**: só dados simulados ou públicos, com a fonte citada.
  - Repositório organizado, com uma pasta para cada parte.
  - Simples primeiro: uma função que funciona vale mais que dez pela metade.
  - Pensar em quem vai usar: letra legível, bom contraste, linguagem simples.
- **ODS:** é obrigatório identificar os ODS e explicar como o projeto contribui.
- **Seis perguntas que a proposta precisa responder:**
  1. Quem é o cidadão?
  2. Qual é o problema?
  3. Como funciona a solução?
  4. Quais tecnologias usa?
  5. Quem participa do ecossistema?
  6. Como medir e ampliar o impacto?

## A2. A solução escolhida

**Nome:** Olho d'Água: alerta comunitário de alagamentos

**Território:** São Paulo, Jardim Pantanal (São Miguel Paulista, zona leste), na várzea do Rio Tietê. Usamos mais duas áreas para dar volume aos dados de demonstração: Jardim Romano e a região do Córrego Ipiranga.

**Cidadão beneficiado:** morador de área de risco de alagamento. O foco são idosos, pessoas com mobilidade reduzida e famílias que moram perto de córregos.

**Problema, em uma frase:** quem mora perto de córregos em São Paulo só descobre que a rua vai alagar quando a água já está entrando em casa, sem tempo para proteger pessoas, documentos e bens.

**Como funciona:**
1. Um sensor de baixo custo (ESP32 com sensor ultrassônico) mede o nível do córrego.
2. O **Serviço de Telemetria** recebe as leituras e detecta quando o nível muda de faixa: NORMAL → ATENCAO → ALERTA → EMERGENCIA.
3. Quando a faixa sobe, a Telemetria avisa o **Serviço do Cidadão**, que cria um alerta para aquela área.
4. O **app** mostra ao morador o nível atual, o alerta com instruções claras e os abrigos mais próximos com vaga.
5. Os moradores também **reportam alagamentos** pelo app. Isso funciona mesmo sem internet: o relato fica salvo no celular e é enviado depois. Quando chegam 3 relatos na mesma área dentro de 1 hora, nasce um **alerta comunitário**.
6. Um modelo de **ML** treinado com dados reais do INMET estima a chance de chuva forte nas próximas horas.

**Indicadores de impacto:**
- Minutos de antecedência entre o alerta e o transbordamento
- Número de moradores alertados por evento
- Percentual de relatos confirmados pela comunidade

**ODS:**
- **ODS 11, meta 11.5:** reduzir mortes, pessoas afetadas e perdas econômicas causadas por desastres, com foco nos mais vulneráveis.
- **ODS 13, meta 13.1:** reforçar a resiliência e a capacidade de adaptação a riscos climáticos.
- **ODS 10:** a população mais exposta a alagamentos é a de baixa renda, em áreas de várzea.

**Ecossistema:** moradores, associação de bairro, Defesa Civil municipal, CGE (Centro de Gerenciamento de Emergências), escolas e igrejas que servem de abrigo, e a subprefeitura.

## A3. Sem entrevista: como cobrir a etapa "Escutar"

A equipe decidiu **não entrevistar ninguém**. O PDF pede conversar com alguém que convive com o problema, então esse é um ponto fraco que precisa ser compensado com **escuta indireta**:

- **Pesquisa documental:** levantar de 3 a 5 reportagens sobre alagamentos no Jardim Pantanal e no Jardim Romano. Há episódios conhecidos, como a cheia de 2009–2010, que durou meses, e eventos mais recentes. Também vale usar boletins do CGE e da Defesa Civil.
  - Para cada fonte, registrar **link, data e 1 ou 2 falas de moradores** citadas na reportagem.
  - Guardar tudo em `docs/pesquisa.md`.
- **Persona:** criar 1 persona baseada nesse material, com nome fictício, idade, rotina, o que faz hoje quando chove e o que atrapalha. Exemplo de perfil: *"Dona Cida, 68 anos, mora a 2 quadras do córrego, tem joelho operado, depende do vizinho para saber se a rua alagou."*
- **Fotos:** usar fotos de bases públicas (imprensa, Prefeitura, Agência Brasil), sempre com o crédito.
- **Argumento para o pitch:** "o problema é recorrente e documentado". Citar números de fontes oficiais, sempre com a fonte.

> ⚠️ **Nunca** inventar números, falas ou fotos e apresentá-los como reais. Quando algo for ilustrativo, escreva **"ilustrativo"** ao lado.

---

# PARTE B — DECISÕES TÉCNICAS COMPARTILHADAS (FIXAS)

## B1. Repositórios: um por matéria

Cada matéria tem o **seu próprio repositório** no GitHub, e é esse link que vai no Teams de cada uma.

| Matéria | Repositório | Pasta local | Código fica em |
|---|---|---|---|
| **C#** (C1) + plano mestre | https://github.com/PeOliveira18/Olho-dagua | `Desktop\olho-d'agua` | `servico-cidadao-dotnet/` |
| **SOA** (C2) | https://github.com/PeOliveira18/GlobalSolution-SOA | `Desktop\GS-SOA` | `servico-telemetria-java/` |
| **Mobile** (C3) | https://github.com/PeOliveira18/GlobalSolution-Mobile | `Desktop\GS-Mobile` | `app-mobile/` |

Os repositórios das outras matérias (IoT, ML, OS, Cyber, QA) são criados por quem fizer cada uma, seguindo o mesmo padrão.

**Todo repositório tem na raiz:**
- `README.md`, com a Ficha da CitizenTech no topo, o que esse repositório entrega e os links dos outros
- `PLANO.md`, uma cópia deste arquivo
- `prompts.md`, com o registro de IA daquela matéria
- `.gitignore` e `.gitattributes`
- Nenhum repositório precisa de Docker para o banco: os dois backends usam o **Oracle 19c da FIAP** (veja a B2)

**Fonte da verdade:** a cópia **mestre** do PLANO.md fica no repositório **Olho-dagua**. Se um contrato da Parte B mudar, atualize a cópia mestre e copie o arquivo para os outros repositórios, senão as partes deixam de se encaixar.

**Cada repositório tem seu próprio chat**, aberto na pasta local dele. Um chat não mexe no repositório de outra matéria.

## B2. Stack e versões

Ferramentas já instaladas na máquina: .NET SDK 10, Node 22, Java 23 com Maven 3.9, Docker e Git.

| Parte | Stack |
|---|---|
| Serviço do Cidadão | **.NET 10**, ASP.NET Core Web API (controllers), EF Core 10 + `Oracle.EntityFrameworkCore` (**Oracle 19c da FIAP**), Scalar (UI do OpenAPI), xUnit |
| Serviço de Telemetria | **Java 21** (compila no JDK 23), Spring Boot na versão estável que o Spring Initializr oferecer, Maven, Spring Data JPA, driver `ojdbc11`, **Flyway** (+ `flyway-database-oracle`), springdoc-openapi **na versão compatível com o Boot escolhido** (Boot 3.x → springdoc 2.x · Boot 4.x → springdoc 3.x), JUnit 5 + MockMvc, H2 em `MODE=Oracle` só nos testes |
| Banco | **Oracle 19c da FIAP** (`oracle.fiap.com.br:1521/ORCL`), nos dois serviços. O usuário é o RM do aluno, e o schema é **compartilhado com projetos de outras matérias** (tabelas como `EH_*` e `Alertas`): cada serviço usa um prefixo próprio e **nunca cria, altera ou apaga tabelas de outro prefixo**. **Serviço do Cidadão:** prefixo `OD_`, credenciais no *user-secrets*. **Serviço de Telemetria:** prefixo `OT_`, histórico do Flyway em `OT_FLYWAY_HISTORY`, credenciais em variáveis de ambiente ou num `.env` local fora do Git |
| App | Expo (versão estável mais recente do `create-expo-app`), React Native + **TypeScript**, **Expo Router**, expo-sqlite, expo-location |
| IoT | ESP32 DevKit no **Wokwi**, Arduino C++, WiFi `Wokwi-GUEST`, HTTPClient + ArduinoJson |
| ML | Python 3.11+, Jupyter, pandas, scikit-learn, xgboost, matplotlib |
| Infra | Ubuntu Server 24.04 em VM, Bash, systemd, Docker Compose |

## B3. Portas e URLs

| Serviço | Porta | URL base local | Documentação |
|---|---|---|---|
| Serviço do Cidadão (.NET) | **5080** | `http://localhost:5080/api` | `http://localhost:5080/scalar` e `/openapi/v1.json` |
| Serviço de Telemetria (Java) | **8081** | `http://localhost:8081/api` | `http://localhost:8081/swagger-ui.html` |
| Oracle 19c (FIAP) | **1521** | `oracle.fiap.com.br:1521/ORCL` · usuário = RM do aluno · senha só em variável de ambiente ou *user-secrets*, nunca no repositório | — |

**Variáveis de ambiente:**
- Cidadão: `ConnectionStrings__Default` (formato `User Id=RM;Password=...;Data Source=oracle.fiap.com.br:1521/ORCL`, de preferência no *user-secrets*), `Integracao__ApiKey`
- Telemetria: `SPRING_DATASOURCE_URL=jdbc:oracle:thin:@oracle.fiap.com.br:1521/ORCL`, `SPRING_DATASOURCE_USERNAME` (o RM), `SPRING_DATASOURCE_PASSWORD`, `CIDADAO_BASE_URL=http://localhost:5080`, `CIDADAO_API_KEY`. Elas também podem ficar em `servico-telemetria-java/.env`, que está no `.gitignore` (modelo em `.env.example`)
- App: `EXPO_PUBLIC_API_CIDADAO`, `EXPO_PUBLIC_API_TELEMETRIA`, `EXPO_PUBLIC_USE_MOCK`

> No celular físico, `localhost` não funciona: use o IP da máquina na rede, por exemplo `http://192.168.0.10:5080/api`.

**Outras regras:**
- As **duas APIs habilitam CORS** para qualquer origem em desenvolvimento, para o Expo Web funcionar.
- A chamada da Telemetria para o Cidadão envia o header **`X-Api-Key`**. O Cidadão rejeita com 401 se a chave estiver errada.
  - **Valor em desenvolvimento:** o mesmo de `Integracao:ApiKey` no `appsettings.Development.json` do Cidadão. Na Telemetria, ele entra só pela variável `CIDADAO_API_KEY`, nunca em arquivo versionado.
- O Serviço do Cidadão já está implementado (repositório Olho-dagua). Além dos endpoints da C1.4, ele tem `GET /api/alertas/{id}`, `POST /api/abrigos/{id}/saida` e `GET /api/notificacoes?dispositivoId=` (histórico dos pushes simulados, útil para mostrar no vídeo que o alerta chegou ao celular).
- **Regra do contrato ②:** ao receber um novo alerta de um sensor, o Cidadão **encerra sozinho** o alerta anterior do mesmo sensor. A Telemetria só precisa chamar o contrato ③ (encerrar) quando o nível volta a NORMAL, usando o `alertaId` do último alerta.
- **JSON:** campos em `camelCase` e **enums como string em MAIÚSCULAS**, por exemplo `"ALERTA"`. No C#, configurar `JsonStringEnumConverter` com essa política de nomes.

## B4. Dados de seed (os mesmos IDs nos dois serviços)

Todos os dados são **simulados**. As coordenadas são aproximadas.

**Áreas de risco** (Serviço do Cidadão):

| id | nome | bairro | lat | lon | raioMetros | nivelRiscoHistorico |
|---|---|---|---|---|---|---|
| 1 | Várzea do Jardim Pantanal | São Miguel Paulista | -23.4985 | -46.4430 | 1200 | 5 |
| 2 | Jardim Romano | Itaim Paulista | -23.4890 | -46.3960 | 1000 | 4 |
| 3 | Córrego Ipiranga | Ipiranga | -23.5870 | -46.6100 | 800 | 3 |

**Sensores** (Serviço de Telemetria). Os limites são em cm de **nível da água**, onde nível = `alturaInstalacaoCm − distânciaMedida`.

| codigo | tipo | areaRiscoId | alturaInstalacaoCm | limiteAtencaoCm | limiteAlertaCm | limiteEmergenciaCm |
|---|---|---|---|---|---|---|
| SNS-PANTANAL-01 | NIVEL | 1 | 400 | 150 | 220 | 300 |
| SNS-ROMANO-01 | NIVEL | 2 | 400 | 150 | 220 | 300 |
| SNS-IPIRANGA-01 | NIVEL | 3 | 350 | 120 | 180 | 250 |
| CHV-PANTANAL-01 | CHUVA | 1 | — | 10 | 30 | 50 (mm/h) |

**Faixas:**
- `NORMAL`: nível abaixo do limite de atenção
- `ATENCAO`: nível ≥ limite de atenção
- `ALERTA`: nível ≥ limite de alerta
- `EMERGENCIA`: nível ≥ limite de emergência

**Abrigos** (Serviço do Cidadão). São **simulados**: usar nomes genéricos, não nomes de escolas reais.
- Área 1: `Abrigo Escola Municipal A (simulado)` (cap 120, acessível, aceita animais) e `Abrigo Centro Comunitário B (simulado)` (cap 60, não acessível)
- Área 2: `Abrigo Ginásio C (simulado)` (cap 200, acessível)
- Área 3: `Abrigo Paróquia D (simulado)` (cap 80, acessível)

## B5. Contratos entre as partes

**① ESP32 → Telemetria:** `POST /api/leituras`

Requisição:
```json
{ "sensorCodigo": "SNS-PANTANAL-01", "valor": 235.0, "unidade": "CM" }
```

- `valor` é o **nível da água já calculado** pelo ESP32.
- `unidade` é `CM` para nível ou `MM_H` para chuva.

Respostas:
- **201:**
  ```json
  { "leituraId": 991, "faixa": "ALERTA", "eventoAberto": true }
  ```
- **404:** sensor não existe
- **400:** dados inválidos

**② Telemetria → Cidadão:** `POST /api/alertas/sensor`, com o header `X-Api-Key`

Requisição:
```json
{
  "areaRiscoId": 1,
  "sensorCodigo": "SNS-PANTANAL-01",
  "nivelCm": 235.0,
  "faixa": "ALERTA",
  "detectadoEm": "2026-11-15T18:42:00-03:00"
}
```

Resposta **201:**
```json
{ "alertaId": 17, "mensagem": "Córrego em ALERTA no Jardim Pantanal (2,35 m). Tire documentos e eletrônicos do chão e prepare-se para sair." }
```

A Telemetria guarda o `alertaId` no `EventoNivel`.

**③ Telemetria → Cidadão:** `PATCH /api/alertas/{alertaId}/encerrar`, com o header `X-Api-Key`

Chamado quando o nível volta a `NORMAL`. Responde **204**.

**④ App → Telemetria:** `GET /api/sensores/{codigo}/status`

```json
{ "sensorCodigo": "SNS-PANTANAL-01", "areaRiscoId": 1, "nivelCm": 235.0, "faixa": "ALERTA", "atualizadoEm": "2026-11-15T18:42:00-03:00" }
```

Também existe `GET /api/sensores?areaRiscoId=1`, para o app descobrir qual sensor pertence à sua área.

**⑤ App → Cidadão:** a lista completa está na seção C1.4.

**Formato de erro, igual nas duas APIs:** padrão ProblemDetails (RFC 7807).
```json
{ "title": "...", "status": 400, "detail": "...", "errors": { "campo": ["mensagem"] } }
```

---

# PARTE C — SEÇÕES POR MATÉRIA

## C1 · C# Software Development: Serviço do Cidadão

**Pasta:** `servico-cidadao-dotnet/`

### C1.1 O que o professor pede
Uma aplicação C#/.NET que faça parte da CitizenTech, cobrindo:
- POO
- Interfaces
- Injeção de Dependência
- EF Core e LINQ
- APIs REST
- Programação assíncrona
- Testes unitários

**Entregáveis:** repositório GitHub com projeto C#, Web API, EF Core, README com instruções de execução e descrição, documentação dos endpoints (Swagger, Scalar ou equivalente) e evidências (prints).

**Rubrica (10 pts):**

| Critério | Pontos |
|---|---|
| POO | 3,0 |
| Web API | 2,5 |
| EF Core + LINQ | 1,5 |
| Async/Await | 1,0 |
| Qualidade do código | 2,0 |

### C1.2 Estrutura da solução
```
servico-cidadao-dotnet/
├── OlhoDagua.Cidadao.sln
├── src/
│   ├── OlhoDagua.Cidadao.Domain/          ← entidades, enums, value objects, regras (sem dependências)
│   ├── OlhoDagua.Cidadao.Application/     ← interfaces de serviço, DTOs, services, validações
│   ├── OlhoDagua.Cidadao.Infrastructure/  ← DbContext, configurações EF, migrations, repositórios, notificadores
│   └── OlhoDagua.Cidadao.Api/             ← controllers, middlewares, Program.cs, DI, Scalar
└── tests/
    └── OlhoDagua.Cidadao.Tests/           ← xUnit (+ FluentAssertions opcional)
```

### C1.3 Domínio (a parte que vale 3 pontos de POO)

**Entidades:**
- **`AreaRisco`**
  - Campos: Id, Nome, Bairro, `Coordenada` Centro, RaioMetros, NivelRiscoHistorico (1–5).
  - Navegações: Alertas, Ocorrencias, Abrigos.
- **`Alerta`** (abstrata)
  - Campos: Id, AreaRiscoId, Severidade (`BAIXA` | `MEDIA` | `ALTA` | `CRITICA`), CriadoEm, EncerradoEm?
  - Propriedade calculada: `Ativo => EncerradoEm == null`.
  - Métodos: `abstract string GerarMensagem(AreaRisco area)`, `abstract int CalcularPrioridade()`, `void Encerrar()`.
  - Subclasses:
    - **`AlertaSensor`**: SensorCodigo, NivelCm, Faixa. A severidade vem da faixa: ATENCAO→MEDIA, ALERTA→ALTA, EMERGENCIA→CRITICA.
    - **`AlertaComunitario`**: QuantidadeRelatos. Severidade ALTA.
    - **`AlertaPrevisao`**: ProbabilidadeChuvaForte (0–1), JanelaHoras. Severidade MEDIA se a probabilidade for ≥ 0,6. Vem do ML e é opcional.
  - Mapear no EF Core com **TPH** (uma tabela só, com discriminador `TipoAlerta`).
- **`Ocorrencia`**
  - Campos: Id, AreaRiscoId, Tipo (`RUA_ALAGADA` | `CORREGO_TRANSBORDANDO` | `IMOVEL_ATINGIDO` | `BUEIRO_ENTUPIDO`), Descricao (máx. 280 caracteres), `Coordenada` Local, Status (`ABERTA` | `CONFIRMADA` | `RESOLVIDA`), Confirmacoes, CriadaEm, IdLocalApp (Guid).
  - Métodos: `Confirmar()` incrementa as confirmações e muda para CONFIRMADA quando chega a ≥ 2; `Resolver()`.
  - **`IdLocalApp` é único.** Se o app reenviar um relato que já existe, a API devolve o mesmo relato (idempotência da sincronização offline).
- **`Abrigo`**
  - Campos: Id, AreaRiscoId, Nome, Endereco, `Coordenada` Local, Capacidade, Ocupacao, Acessivel, AceitaAnimais.
  - Métodos: `bool TemVaga()`, `int VagasLivres`, `void RegistrarEntrada(int pessoas)` (lança exceção de domínio se não houver vaga).
- **`Inscricao`**
  - Campos: Id, DispositivoId (Guid gerado no app; **sem nome, telefone ou CPF**), AreaRiscoId, PrefereFonteGrande, PrefereAltoContraste, CriadaEm.

**Value object:** `Coordenada` (record com Latitude e Longitude, validação de faixa) e o método `double DistanciaMetros(Coordenada outra)` (fórmula de Haversine).

**Encapsulamento:** setters privados, construtores que validam e métodos de comportamento no lugar de setters públicos.

### C1.4 Endpoints

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/areas` | lista as áreas |
| GET | `/api/areas/{id}` | detalhe com nº de alertas ativos e de ocorrências abertas |
| GET | `/api/areas/ranking?dias=7` | **LINQ:** áreas ordenadas pelo nº de ocorrências no período |
| GET | `/api/alertas?areaId=&ativos=true` | **LINQ:** alertas ordenados por `CalcularPrioridade()`, cada um com `mensagem` gerada |
| GET | `/api/alertas/{id}` | detalhe |
| POST | `/api/alertas/sensor` | usado pela Telemetria (exige `X-Api-Key`), contrato ② |
| POST | `/api/alertas/previsao` | usado pelo ML (exige `X-Api-Key`, opcional) |
| PATCH | `/api/alertas/{id}/encerrar` | contrato ③ |
| GET | `/api/ocorrencias?areaId=&status=` | lista |
| POST | `/api/ocorrencias` | cria (idempotente por `idLocalApp`) e **dispara a regra do alerta comunitário** |
| POST | `/api/ocorrencias/{id}/confirmar` | outro morador confirma |
| PATCH | `/api/ocorrencias/{id}/resolver` | marca como resolvida |
| GET | `/api/abrigos?areaId=` | lista |
| GET | `/api/abrigos/proximos?lat=&lon=&acessivel=&limite=5` | **LINQ:** com vaga, ordenados por distância |
| POST | `/api/abrigos/{id}/entrada` | registra ocupação |
| POST | `/api/inscricoes` | cria ou atualiza pelo `dispositivoId` |
| GET | `/api/inscricoes/{dispositivoId}` | consulta |

**Status codes:**
- 200 ou 201 com `Location` no header
- 204 em PATCH
- 400 para validação
- 401 para api key errada
- 404 quando não encontra
- 409 quando o abrigo está sem vaga

### C1.5 Interfaces, DI e regras
- `IRepositorio<T>` genérico, com `ObterPorIdAsync`, `ListarAsync`, `AdicionarAsync` e `SalvarAsync`, mais repositórios específicos quando precisar de consulta própria.
- Serviços: `IAreaService`, `IAlertaService`, `IOcorrenciaService`, `IAbrigoService`, `IInscricaoService`.
- **`INotificador`** com duas implementações, `NotificadorConsole` (escreve no log) e `NotificadorPushSimulado` (guarda em memória a lista de "pushes enviados"). A implementação é escolhida no `appsettings` e registrada na DI. Ao criar qualquer alerta, notificar as inscrições daquela área.
- **`IRegraAlertaComunitario`**: se houver **3 ou mais ocorrências** (ABERTA ou CONFIRMADA) na mesma área nos **últimos 60 minutos** e **nenhum** AlertaComunitario ativo nela, cria um. Precisa ser fácil de testar, com o relógio injetado via `TimeProvider`.
- **Sanitização de PII** na `Descricao` da ocorrência, ligada à seção C7: mascarar CPF e telefone antes de salvar, com um `ISanitizadorPii` simples.
- Toda a cadeia é **async** (controller → service → repositório) e recebe `CancellationToken`.
- Middleware global que transforma exceções de domínio em ProblemDetails.

### C1.6 EF Core
- `CidadaoDbContext` com configurações em classes `IEntityTypeConfiguration<T>`.
- `Coordenada` mapeado como owned type.
- Enums gravados como string.
- **Migrations versionadas** na Infrastructure.
- **Seed** com os dados da B4 via `HasData` ou um seeder no startup.
- `Database.Migrate()` no startup quando estiver em Development.

### C1.7 Testes xUnit (mínimo)
- `AlertaSensor.GerarMensagem` e a `Severidade` para cada faixa
- `Abrigo.RegistrarEntrada` sem vaga lança exceção
- `Ocorrencia.Confirmar` muda o status na 2ª confirmação
- `RegraAlertaComunitario` cria o alerta no 3º relato e **não** duplica no 4º
- `Coordenada.DistanciaMetros` com um valor conhecido
- `SanitizadorPii` mascara CPF e telefone

### C1.8 README da pasta
- Pré-requisitos e como subir: connection string do Oracle no *user-secrets* (`ConnectionStrings:Default`), depois `dotnet run` (as migrations são aplicadas ao subir).
- URL do Scalar.
- Tabela "rubrica → onde está no código".
- Prints do Scalar e de chamadas funcionando.
- Diagrama de classes do domínio (Mermaid `classDiagram`).

### C1.9 Pronto quando
- [ ] `dotnet build` e `dotnet test` passam
- [ ] Migrations criam o banco do zero e o seed aparece
- [ ] Todos os endpoints da C1.4 respondem no Scalar
- [ ] O POST de 3 ocorrências na área 1 gera um AlertaComunitario
- [ ] Os prints estão em `docs/` ou no README

---

## C2 · SOA e Web Services: Serviço de Telemetria (Java Spring Boot)

**Pasta:** `servico-telemetria-java/`

### C2.1 O que o professor pede
A camada de backend da solução, em qualquer linguagem, com estruturação do domínio, conexão com banco de dados, APIs REST e testes automatizados.

**Entregáveis:** repositório GitHub, README com instruções, descrição e **nomes dos integrantes**, documentação dos endpoints (Swagger) e evidências.

**Rubrica (10 pts):**

| Critério | Pontos |
|---|---|
| Estruturação do domínio | 3,0 |
| Conexão com banco | 3,0 |
| **Migrações** | 1,5 |
| **Documentação automática** | 1,5 |
| Testes automatizados | 1,0 |

### C2.2 Estrutura
```
servico-telemetria-java/
├── pom.xml   (groupId br.com.olhodagua, artifactId telemetria)
└── src/main/java/br/com/olhodagua/telemetria/
    ├── domain/          ← entidades JPA, enums, ClassificadorNivel (serviço de domínio)
    ├── application/     ← services (LeituraService, SensorService, EventoService), DTOs (records)
    ├── infrastructure/  ← repositories (Spring Data), CidadaoClient (RestClient), config
    └── web/             ← controllers, @RestControllerAdvice (ProblemDetail)
src/main/resources/
    ├── application.yml
    └── db/migration/    ← V1__sensor.sql, V2__leitura.sql, V3__evento_nivel.sql, V4__seed_sensores.sql, V5__evento_encerramento_notificado.sql
src/test/java/...        ← JUnit 5
```

### C2.3 Domínio
- **`Sensor`**
  - Campos: id, codigo (único), tipo (`NIVEL` | `CHUVA`), latitude, longitude, alturaInstalacaoCm, limiteAtencao, limiteAlerta, limiteEmergencia, status (`ATIVO` | `MANUTENCAO` | `INATIVO`), areaRiscoId.
  - O `areaRiscoId` é referência ao outro serviço, **sem FK**.
- **`Leitura`**: id, sensor (ManyToOne), valor, unidade (`CM` | `MM_H`), faixaCalculada, registradaEm.
- **`EventoNivel`**: id, sensor (ManyToOne), faixa, valorDisparo, iniciadoEm, encerradoEm, alertaCidadaoId (nullable), notificado (boolean), encerramentoNotificado (boolean, coluna criada pela V5).
- **`ClassificadorNivel`** (serviço de domínio puro, sem Spring): `Faixa classificar(Sensor s, double valor)`.

**Regra principal**, em `LeituraService.registrar`:
1. Valida o sensor (precisa existir e estar ATIVO) e salva a leitura com a faixa calculada.
2. Compara com a faixa do **evento aberto** do sensor, ou NORMAL se não houver evento.
3. **A faixa subiu** (ex.: NORMAL→ATENCAO ou ATENCAO→ALERTA): fecha o evento anterior, se houver, abre um novo e chama o `CidadaoClient` (contrato ②), guardando o `alertaId`. O Cidadão encerra sozinho o alerta anterior do mesmo sensor.
4. **Voltou para NORMAL:** fecha o evento e chama o encerramento (contrato ③).
5. **A faixa desceu, mas não para NORMAL:** só atualiza, sem notificar.
6. **O Cidadão está fora do ar:** grava `notificado=false` e **não falha a leitura**, só registra um log de warning. Um `@Scheduled` a cada 1 minuto reenvia os alertas pendentes e, para sensores que já voltaram a NORMAL, os encerramentos pendentes. Isso é resiliência entre serviços e vale contar no vídeo.

### C2.4 Endpoints

| Método | Rota | Descrição |
|---|---|---|
| POST | `/api/leituras` | contrato ① |
| GET | `/api/sensores?areaRiscoId=&tipo=` | lista |
| GET | `/api/sensores/{codigo}` | detalhe |
| POST | `/api/sensores` | cria (com Bean Validation) |
| PUT | `/api/sensores/{codigo}` | atualiza (limites, status) |
| DELETE | `/api/sensores/{codigo}` | desativa (soft delete, muda status para INATIVO) |
| GET | `/api/sensores/{codigo}/status` | contrato ④ |
| GET | `/api/sensores/{codigo}/leituras?desde=&ate=&page=&size=` | histórico paginado |
| GET | `/api/eventos?ativos=true&areaRiscoId=` | eventos |

**Documentação:** springdoc com `@Operation` e `@Schema` nos DTOs, e exemplos de requisição no Swagger.

### C2.5 Banco e migrações
- **Oracle 19c da FIAP**, no schema do aluno, com `spring.jpa.hibernate.ddl-auto=validate`. **Quem cria as tabelas é o Flyway, nunca o Hibernate.**
- Tabelas com prefixo `OT_`: `OT_SENSOR`, `OT_LEITURA` e `OT_EVENTO_NIVEL`. Constraints e índices também começam com `OT_`, porque no Oracle esses nomes não podem se repetir dentro do schema.
- O schema já tem tabelas de outros projetos, então o Flyway usa `baseline-on-migrate` (versão 0), guarda o histórico em `OT_FLYWAY_HISTORY` e fica com `clean-disabled: true`.
- Migrações em SQL do Oracle: `VARCHAR2`, `NUMBER`, `TIMESTAMP`, `GENERATED BY DEFAULT AS IDENTITY`.
- Índice em `OT_LEITURA(SENSOR_ID, REGISTRADA_EM)`.
- A V4 faz o seed dos 4 sensores da B4. A V5 é a **evolução do esquema**: `ALTER TABLE OT_EVENTO_NIVEL ADD ENCERRAMENTO_NOTIFICADO`.

### C2.6 Testes
- **Unitário:** `ClassificadorNivel` em todas as faixas, incluindo os valores exatos de fronteira.
- **Unitário com Mockito:** `LeituraService` abre evento quando a faixa sobe e não chama o Cidadão quando a faixa se mantém.
- **Integração:** `@SpringBootTest` + MockMvc para `POST /api/leituras` (201 e 404). Usar H2 em `MODE=Oracle`, configurado por `@TestPropertySource` (que tem prioridade sobre as variáveis de ambiente), para os testes nunca dependerem do Oracle. No H2 o `validate` fica desligado, porque ele descreve `NUMBER` como `NUMERIC`; quem valida o esquema de verdade é o Oracle, ao subir a aplicação.

### C2.7 Pronto quando
- [ ] `mvn test` passa
- [ ] `mvn spring-boot:run` sobe, e o Flyway cria e popula o banco
- [ ] O Swagger lista todos os endpoints com exemplos
- [ ] Enviar a sequência 100 → 160 → 230 → 310 → 90 cm no SNS-PANTANAL-01 gera 3 alertas no Cidadão e depois encerra
- [ ] O README tem o nome dos integrantes, as instruções e os prints

---

## C3 · Mobile Development & IOT: App Expo

**Pasta:** `app-mobile/`

### C3.1 O que o professor pede
- App funcional em Expo (React Native + TypeScript + Expo Router) que resolva a **jornada principal** da Ficha.
- Precisa **consumir uma API ou mock**, **salvar em banco de dados** e ser **simples e acessível**.

**Entregáveis:** repositório com o código organizado e um README contendo a Ficha, instruções para rodar, **prints das telas** e o link de um **vídeo de até 3 minutos** com o app rodando.

**Critérios:**
- Telas e navegação
- Consumo de dados
- Banco de dados
- Acessibilidade
- Aderência ao problema
- Documentação

### C3.2 Jornada principal
*"Vai alagar na minha rua? O que eu faço? Para onde eu vou? E como eu aviso os vizinhos?"*

### C3.3 Telas (Expo Router)
```
app/
├── _layout.tsx                 ← providers (tema/acessibilidade, banco SQLite)
├── onboarding.tsx              ← 1ª abertura: escolher a área + preferências de acessibilidade → POST /inscricoes
├── (tabs)/
│   ├── _layout.tsx             ← 4 abas com ícone + texto
│   ├── index.tsx               ← INÍCIO
│   ├── reportar.tsx            ← REPORTAR
│   ├── abrigos.tsx             ← ABRIGOS
│   └── perfil.tsx              ← PERFIL
├── alerta/[id].tsx             ← detalhe do alerta com "o que fazer agora" (passo a passo)
└── abrigo/[id].tsx             ← detalhe do abrigo + botão "Como chegar" (abre o app de mapas via Linking)
```

**Início:**
- Cartão grande com o **nível do córrego** da minha área. A faixa aparece com cor **+ ícone + texto**, por exemplo "🟠 ALERTA — 2,35 m", e o horário da última atualização.
- Lista de alertas ativos, com a `mensagem`.
- Botão grande "Reportar alagamento".
- Puxar para atualizar.
- Banner "Sem internet: mostrando dados de 18:42" quando estiver offline.

**Reportar:**
- 4 botões grandes com os tipos de ocorrência, ícone e texto.
- Campo de descrição opcional (máx. 280 caracteres) e localização automática via expo-location, com um fallback para escolher a área.
- Botão "Enviar". Sem rede, salva na fila e mostra "Será enviado quando a internet voltar".
- Abaixo, a lista "Meus relatos" com o status de cada um: pendente, enviado ou confirmado.
- Botão "Confirmar" nos relatos de outras pessoas da área.

**Abrigos:**
- Lista ordenada por distância (`/abrigos/proximos`), com o filtro "Só acessíveis".
- Cada item mostra as vagas livres e a distância.

**Perfil:**
- Trocar de área.
- Fonte grande (escala 1.3×).
- Alto contraste.
- Sobre o projeto e ODS.
- Limpar dados locais.

### C3.4 Dados
- Uma camada `services/api.ts` com `fetch`, timeout e tipos TypeScript espelhando os contratos da B5 e da C1.4.
- **Modo mock:** com `EXPO_PUBLIC_USE_MOCK=true`, `services/mock/*.json` responde com os mesmos formatos. Isso garante que o vídeo funcione sem os backends no ar.
- **SQLite (expo-sqlite)**, com estas tabelas:
  - `preferencias` (chave/valor): área, `dispositivoId`, fonte e contraste
  - `relatos` (id_local, tipo, descricao, lat, lon, criado_em, status_sync: `PENDENTE` | `ENVIADO` | `ERRO`, id_servidor)
  - `cache_status` (area_id, json, atualizado_em)
- **Sincronização:** ao abrir o app, ao voltar a internet (`@react-native-community/netinfo`) e ao puxar para atualizar, enviar os relatos `PENDENTE` com `idLocalApp`. O servidor é idempotente.

### C3.5 Acessibilidade (critério próprio na nota)
- `accessibilityLabel`, `accessibilityRole` e `accessibilityHint` em todos os elementos tocáveis.
- `accessibilityLiveRegion` ou `AccessibilityInfo.announceForAccessibility` quando chegar um novo alerta.
- Alvos de toque de **pelo menos 48×48 dp**. Respeitar o `fontScale` do sistema e, por cima, a preferência "fonte grande".
- Contraste AA. Tema de **alto contraste** (fundo preto, texto branco, faixas com borda).
- **Nunca usar só a cor** para indicar a faixa: sempre cor + ícone + texto.
- Linguagem simples, sem jargão técnico: "nível do córrego", não "telemetria".
- Testar com o TalkBack ou o VoiceOver e mostrar isso no vídeo, se der.

### C3.6 Pronto quando
- [ ] `npx expo start` roda no Expo Go e no web
- [ ] Os modos mock e API real funcionam
- [ ] Um relato feito offline aparece como pendente e é enviado ao reconectar
- [ ] O README tem a Ficha, as instruções, os prints das 4 abas + onboarding + detalhes, e o link do vídeo (≤ 3 min)

---

## C4 · Physical Computing: IOT & IOB (ESP32 no Wokwi)

**Pasta:** `iot/`, com `sketch.ino`, `diagram.json`, `libraries.txt`, `integrantes.txt` e `README.md`.

### C4.1 O que o professor pede
- Um circuito com ESP32 no Wokwi, **integrado à solução**. Não pode ser um projeto paralelo.

**Entregáveis:**
- Link do projeto no Wokwi
- Link do vídeo (até 4 minutos) apresentando a solução, como o circuito se encaixa nela e o circuito funcionando
- Arquivo `.txt` com o nome dos integrantes

### C4.2 Circuito: "estação de nível do córrego"

| Componente | Função | Pinos sugeridos |
|---|---|---|
| ESP32 DevKit v1 | controlador + WiFi | — |
| **HC-SR04** (ultrassônico) | mede a distância até a água. No Wokwi dá para arrastar a distância durante a simulação, o que é ótimo para a demonstração | TRIG 5, ECHO 18 |
| **Potenciômetro** | simula o pluviômetro em mm/h (o Wokwi não tem sensor de chuva) | 34 (ADC) |
| LED verde / amarelo / vermelho | faixa local | 25 / 26 / 27 |
| **Buzzer** | apita em EMERGENCIA | 14 |
| **OLED SSD1306 (I2C)** ou LCD 16x2 I2C | mostra o nível, a faixa e o status do WiFi | SDA 21, SCL 22 |
| Botão | "teste de alerta" manual | 4 |

### C4.3 Lógica
- A cada 5 s:
  1. Mede a distância e calcula `nivel = 400 − distancia` (altura de instalação do SNS-PANTANAL-01).
  2. Calcula a faixa com os **mesmos limites da B4**.
  3. Acende o LED da faixa e atualiza o display.
- **Envio:**
  - Quando a faixa muda, ou a cada 30 s, faz `POST /api/leituras` (contrato ①) via HTTPClient + ArduinoJson.
  - O potenciômetro envia uma leitura do `CHV-PANTANAL-01` em `MM_H`.
- **Funciona mesmo sem servidor:** o alerta local (LEDs e buzzer) não depende da rede. Isso conversa com o requisito "continuar relevante fora de cenários ideais de conectividade".
- **URL do servidor:**
  - O Wokwi só acessa endereços **públicos**. Para a demonstração integrada, expor a Telemetria com `cloudflared tunnel --url http://localhost:8081` ou `ngrok http 8081` e colar a URL em uma constante.
  - Plano B: usar um endpoint público de teste (ex.: webhook.site) só para mostrar o JSON saindo, e explicar a integração no vídeo.

### C4.4 Pronto quando
- [ ] O projeto Wokwi é público, tem o link no README e simula sem erros
- [ ] Mudar a distância no HC-SR04 troca LED, display e buzzer
- [ ] O vídeo mostra a leitura chegando na Telemetria (Swagger ou log) e, se possível, o alerta aparecendo no app

---

## C5 · Inteligência Artificial & Machine Learning

**Pasta:** `ml/`

### C5.1 O que o professor pede
- Transformar o problema em **uma pergunta de ML** e respondê-la com **dados reais e públicos**. **Dados simulados não são aceitos.**
- Escolher um destes modelos: XGBoost, Random Forest, MLP, LSTM ou KMeans + PCA.
- **Testar pelo menos 10 combinações de 3 hiperparâmetros**, com **validação cruzada só no treino**.
- Mostrar as métricas de **treino e teste lado a lado**, com gráficos, discutir **overfitting** e concluir **o que isso significa para o cidadão**.

**Entregáveis:**
- `ml/` com o **notebook executado** (com as saídas), `dados/README.md` (fonte, link e período) e `requirements.txt`
- `RELATORIO_ML.md` (2 a 4 páginas)
- **Vídeo de até 5 minutos** no YouTube, não listado

**Rubrica:**

| Critério | Pontos |
|---|---|
| Problema e dados | 2,0 |
| Escolha do modelo | 1,0 |
| Preparação e divisão | 2,0 |
| Treino com parâmetros | 2,0 |
| Resultados treino/teste | 2,0 |
| Conclusão para o cidadão | 1,0 |

### C5.2 Pergunta de ML
**"Com as condições meteorológicas da última hora, vai cair chuva forte (≥ 10 mm/h) em São Paulo nas próximas 3 horas?"**

É uma classificação binária. Ela alimenta o `AlertaPrevisao` do Serviço do Cidadão.

### C5.3 Dados (reais)
- **INMET, dados históricos:** `https://portal.inmet.gov.br/dadoshistoricos`. Os arquivos são ZIPs anuais com um CSV por estação.
- **Estação A701, São Paulo – Mirante de Santana.** Dados horários. Usar de **2015 a 2025**, cerca de 10 anos, ~90 mil linhas.
- Colunas úteis:
  - precipitação total horária (mm)
  - pressão atmosférica
  - temperatura do ar
  - ponto de orvalho
  - umidade relativa
  - vento (velocidade, rajada, direção)
  - radiação
- **Cuidados:**
  - O CSV usa `;` como separador, vírgula decimal, encoding latin-1 e algumas linhas de cabeçalho antes dos dados.
  - Valores ausentes aparecem como `-9999`.
  - O horário está em UTC.
- Opcional: cruzar com os registros de alagamento do CGE, se a equipe conseguir os dados públicos.

### C5.4 Preparação
- **Features:** variáveis da hora atual e lags de 1, 2 e 3 h; variação de pressão em 3 h; chuva acumulada em 3 h e 6 h; hora do dia e mês codificados com seno e cosseno.
- **Alvo:** `1` se a soma da precipitação nas próximas 3 h for ≥ 10 mm.
- **A classe positiva é rara.** Usar `scale_pos_weight` no XGBoost ou `class_weight` no RF, e avaliar com **F1, recall, precision e PR-AUC**, não com acurácia.
- **Divisão temporal**, sem embaralhar: treino 2015–2022, teste 2023–2025. Validação cruzada com `TimeSeriesSplit` **só no treino**.

### C5.5 Modelo e busca
- Modelo: **XGBoost** (justificativa: dados tabulares, desbalanceamento, interpretabilidade por feature importance). Opcional: comparar com Random Forest.
- Grade com ≥ 10 combinações de `max_depth`, `learning_rate` e `n_estimators`, via `GridSearchCV` ou `RandomizedSearchCV` + `TimeSeriesSplit`, com `scoring="f1"`.
- **Gráficos:**
  - tabela das combinações
  - treino vs teste lado a lado
  - matriz de confusão
  - curva PR
  - importância das features

### C5.6 Conclusão para o cidadão
Traduzir em linguagem simples. Por exemplo: "de cada 10 chuvas fortes, o modelo avisa X com até 3 h de antecedência. Isso dá tempo de tirar móveis do chão." Discutir também o custo dos alarmes falsos, que fazem a população deixar de confiar no alerta.

### C5.7 Pronto quando
- [ ] O notebook roda do início ao fim com as saídas salvas
- [ ] `dados/README.md` tem a fonte, o link, a estação e o período. Se o CSV for grande demais, explicar como baixar
- [ ] O `RELATORIO_ML.md` está pronto e o vídeo está no ar

---

## C6 · Operating Systems: infraestrutura Linux

**Pasta:** `infra/`

### C6.1 O que o professor pede
Um ambiente Linux provisionado **automaticamente por Shell Script** dentro de uma VM, que hospeda uma aplicação web da CitizenTech, com:
- configuração de rede, DNS e hostname
- provisionamento da aplicação
- **monitoramento de CPU, disco e memória com alertas**
- monitoramento rodando **como serviço tolerante a falhas** (systemd)
- **logs de auditoria**
- **teste de falha**

**Entregáveis:** repositório com scripts, configurações, evidências e README, mais um **vídeo de até 7 minutos** mostrando o provisionamento automático e a aplicação rodando.

**Rubrica (10 pts):**

| Critério | Pontos |
|---|---|
| Provisionamento da infra (rede, DNS, IP) | 2 |
| Provisionamento da aplicação | 2 |
| Monitoramento CPU/disco/memória com alertas | 2 |
| Monitoramento como serviço tolerante a falhas | 1 |
| Logs de auditoria | 1 |
| Funcionamento completo | 2 |

### C6.2 O que hospedar
Os dois serviços, via **Docker Compose**. O banco é o Oracle da FIAP, fora da VM: os containers só precisam de acesso à internet e das credenciais em variáveis de ambiente. A "aplicação web" que aparece no navegador é a **página Scalar do Serviço do Cidadão** (`http://olhodagua.local:5080/scalar`) e o Swagger da Telemetria.

### C6.3 Arquivos
```
infra/
├── docker-compose.yml          ← cidadao e telemetria (o banco é o Oracle da FIAP, fora da VM)
├── .env.example
├── Dockerfile.cidadao          ← (ou dentro de cada serviço) multi-stage .NET 10
├── Dockerfile.telemetria       ← multi-stage Maven → JRE 21
└── linux/
    ├── 00_provisionar_tudo.sh  ← orquestra os demais, com log
    ├── 01_infra.sh             ← hostname "olhodagua-srv", IP estático (netplan), /etc/hosts "olhodagua.local", timezone America/Sao_Paulo, ufw (22, 5080, 8081)
    ├── 02_dependencias.sh      ← apt update, docker + compose plugin, git, curl, sysstat
    ├── 03_aplicacao.sh         ← clona/atualiza o repo em /opt/olho-dagua, cria o .env, docker compose up -d --build, healthcheck via curl
    ├── monitor.sh              ← lê CPU/mem/disco; ALERTA quando CPU > 80%, mem > 85%, disco > 90%; também confere se os containers estão de pé
    ├── olhodagua-monitor.service ← systemd: Restart=always, RestartSec=5
    ├── olhodagua-monitor.timer   ← (alternativa) executa a cada 1 min
    ├── olhodagua-app.service     ← systemd que sobe o docker compose no boot
    ├── lib_log.sh              ← função log() → /var/log/olhodagua/auditoria.log com data, usuário, script, ação e resultado
    └── teste_falha.sh          ← (1) kill -9 do monitor → systemd reinicia; (2) docker stop cidadao → monitor detecta e alerta/reinicia; (3) stress de CPU (stress-ng) → alerta
```

### C6.4 Logs
- `/var/log/olhodagua/auditoria.log`: toda execução de script, com quem rodou, quando, o quê e o resultado.
- `/var/log/olhodagua/monitor.log`: as medições.
- `/var/log/olhodagua/alertas.log`: só os alertas.
- Configurar `logrotate`.

### C6.5 Pronto quando
- [ ] Numa VM Ubuntu limpa, `sudo ./00_provisionar_tudo.sh` deixa tudo no ar, sem nenhum passo manual
- [ ] `systemctl status olhodagua-monitor` mostra o serviço ativo e ele volta sozinho depois do `kill -9`
- [ ] O `teste_falha.sh` gera evidências nos logs
- [ ] O vídeo tem até 7 min

---

## C7 · Cybersecurity

**Pasta:** `cyber/`

### C7.1 O que o professor pede

> ⚠️ O PDF fala em "configurar **novas regras no motor de parsing**" e em "**código atualizado**", o que indica que existe uma **ferramenta-base dada em aula** (motor de parsing de PII + dashboard). **Antes de começar, peça ao usuário o código ou material da aula.** O plano abaixo encaixa a missão no nosso projeto, seja qual for a ferramenta.

**Missão técnica:**
1. **Mapeamento de PII:** novas regras no motor de parsing para detectar dados sensíveis (CPF, saúde, senhas).
2. **Taint Analysis:** rastrear o fluxo dos dados coletados e bloquear vazamentos em saídas inseguras.
3. **Tradução via IA:** usar um LLM para transformar falhas técnicas em alertas claros de risco real para o cidadão.

**Entrega (ZIP):**
- código do novo módulo com as regras de PII
- auditoria do protótipo, com evidências do taint analysis funcionando
- vídeo e print do dashboard gerando o indicador **"Risco Cidadão"**

**Rubrica:**

| Critério | Pontos |
|---|---|
| Robustez do motor de parsing PII | 3,0 |
| Eficácia do taint analysis | 3,0 |
| Inovação com IA na remediação | 2,0 |
| Dashboard executivo (Risco Cidadão) | 2,0 |

### C7.2 Onde existe PII no Olho d'Água
O campo livre **`Ocorrencia.Descricao`** é o principal ponto de risco. É onde um morador pode escrever coisas como: "Sou a Maria, CPF 123.456.789-09, tel (11) 98888-7777, minha mãe é **acamada** e **diabética**, mora na Rua X, 120".

Outros pontos:
- localização exata (lat/lon) do relato
- `DispositivoId`
- logs das APIs
- payload enviado ao notificador

### C7.3 Regras de PII (mínimo)
Regex com validação de dígito:
- **CPF** (com e sem máscara, validando o dígito verificador)
- **CNPJ**
- **telefone BR** (fixo e celular, com ou sem DDD/+55)
- **e-mail**
- **CEP**
- **endereço** ("Rua/Av./Travessa … nº")
- **RG**

**Senhas e segredos:**
- padrões como `senha:`, `password=`, `pwd`
- tokens JWT, chaves `sk-...` e `AKIA...`

**Saúde (dicionário):** acamado, cadeirante, diabético, gestante, hemodiálise, oxigênio, CID-10 (`[A-Z]\d{2}(\.\d)?`) e afins.

Cada regra tem id, categoria (`IDENTIFICACAO` | `CONTATO` | `LOCALIZACAO` | `SAUDE` | `CREDENCIAL`), peso de risco e uma **suíte de testes** com casos positivos e negativos (incluindo falsos positivos, como números que parecem CPF mas não são).

### C7.4 Taint analysis
- **Fontes (tainted):** corpo do `POST /api/ocorrencias`, query strings e o payload do ESP32.
- **Sinks inseguros:**
  - logs
  - resposta pública do `GET /api/ocorrencias` (que outros moradores veem)
  - payload do `INotificador`
  - mensagens de erro
- **Sanitizadores:** `ISanitizadorPii` (mascara: `***.***.***-09`, `(11) 9****-7777`) e arredondamento da coordenada pública para ~100 m.
- **Demonstração:** a versão "antes" vaza o CPF no log e no GET. O analisador marca o fluxo fonte→sink e bloqueia ou mascara. A versão "depois" fica limpa. Evidências: prints do relatório e dos logs antes e depois.

### C7.5 IA na remediação
Para cada achado, enviar a um LLM o tipo, o trecho anonimizado e o sink, e pedir:
1. Um **alerta em linguagem simples para o cidadão**, por exemplo: "Seu telefone ia aparecer para todos os vizinhos no mapa. Nós escondemos."
2. Uma **sugestão de correção** para o desenvolvedor.

**Nunca enviar a PII crua ao LLM**: mascarar antes. Registrar os prompts no `prompts.md`.

### C7.6 Dashboard "Risco Cidadão"
- Índice de 0 a 100, calculado pela soma dos pesos dos achados ponderada pelo tipo de sink.
- Gráficos:
  - achados por categoria
  - por sink
  - tendência antes vs depois
  - top regras disparadas
- Pode ser feito no dashboard da ferramenta da aula ou num HTML/Streamlit simples.

### C7.7 Pronto quando
- [ ] O ZIP tem o código, as evidências e o print/vídeo do dashboard
- [ ] Os testes das regras passam
- [ ] O antes e depois está documentado

---

## C8 · Testing, Compliance & QA: Azure Boards + pitch

**Pasta:** `qa/` (prints e PDF final)

### C8.1 O que o professor pede
- No **Azure Boards**, cadastrar as **Features** com descrição e **critérios de aceite**, no padrão ensinado em aula.
- Cadastrar os **Test Cases funcionais**, positivos e negativos, de caixa-preta. Cada um precisa ter objetivo, preparação, inputs, saídas esperadas e passos.
- Incluir o professor como **Admin** no projeto Azure.
- Gerar um **PDF** com o link do projeto e **prints de todas as telas** (lista de Features, detalhe de cada Feature, Test Cases), como contingência.
- **Avaliação:** Features 50% e Test Cases 50%.
- **Pitch:** é nesta disciplina que se envia o **vídeo de até 4 minutos no YouTube**.

### C8.2 Features sugeridas
Escrever a descrição no formato de história de usuário ("Como … quero … para …"). Os critérios de aceite vão em **Dado / Quando / Então**, ou no formato que o professor ensinou.

| ID | Feature |
|---|---|
| F01 | Escolher minha área de risco e inscrever o dispositivo (onboarding) |
| F02 | Ver o nível atual do córrego da minha área |
| F03 | Receber e visualizar alertas ativos com instruções |
| F04 | Reportar alagamento (online) |
| F05 | Reportar alagamento sem internet (fila offline + sincronização) |
| F06 | Confirmar relato de outro morador |
| F07 | Gerar alerta comunitário automaticamente (3 relatos em 60 min) |
| F08 | Consultar abrigos próximos com vaga (filtro acessível) |
| F09 | Receber leituras de sensores e classificar a faixa de nível |
| F10 | Notificar o Serviço do Cidadão quando a faixa sobe (e encerrar quando normaliza) |
| F11 | Configurar acessibilidade (fonte grande / alto contraste) |
| F12 | Proteger dados pessoais nos relatos (mascarar CPF/telefone) |

### C8.3 Test Cases
**Pelo menos 2 por feature:** 1 positivo e 1 negativo.

Exemplos:
- **TC-F04-01 (positivo):**
  - Preparação: app com a área 1 selecionada e internet ativa.
  - Input: tipo RUA_ALAGADA, descrição "Rua tomada até o joelho".
  - Esperado: mensagem "Relato enviado", o relato aparece em "Meus relatos" com status Enviado e `GET /api/ocorrencias?areaId=1` o retorna.
- **TC-F04-02 (negativo):** descrição com 300 caracteres. Esperado: botão Enviar bloqueado e a mensagem "Máximo de 280 caracteres".
- **TC-F09-02 (negativo):** `POST /api/leituras` com `sensorCodigo` inexistente. Esperado: HTTP 404, com a mensagem "Sensor não encontrado".
- **TC-F07-01:** 3 POSTs de ocorrência na área 1 em menos de 60 min. Esperado: 1 AlertaComunitario ativo. O 4º POST não cria um 2º alerta.
- **TC-F08-02:** abrigo com Ocupacao = Capacidade. Esperado: não aparece em `/abrigos/proximos`, e `POST /entrada` retorna 409.
- **TC-F12-01:** descrição com CPF. Esperado: o GET público mostra `***.***.***-09`.

### C8.4 Pitch (até 4 minutos)
| Tempo | Conteúdo |
|---|---|
| 0:00–0:40 | O problema: fotos públicas e reportagem (com crédito) + a persona |
| 0:40–1:00 | A solução em uma frase + diagrama |
| 1:00–3:00 | Demo integrada: arrastar a distância no Wokwi → leitura chega na Telemetria → alerta no app → morador vê o abrigo → relato offline sincroniza |
| 3:00–3:30 | ML: "o modelo avisa X de cada 10 chuvas fortes com 3 h de antecedência" |
| 3:30–4:00 | Impacto, ODS 11/13/10 e próximos passos |

### C8.5 Pronto quando
- [ ] O projeto Azure tem as 12 Features e ≥ 24 Test Cases, e o professor é Admin
- [ ] O PDF tem o link e todos os prints
- [ ] O link do pitch está no YouTube

---

# PARTE D — CRONOGRAMA E DEPENDÊNCIAS

**Ordem de dependência:**
- **C1 (C#) e C2 (Java)** podem começar já: os contratos da B5 estão fixos.
- **C3 (Mobile)** começa já no **modo mock** e liga na API real quando C1 e C2 estiverem prontos.
- **C4 (IoT)** começa já. A integração real precisa da C2 no ar, com túnel.
- **C5 (ML)** é independente: só precisa dos dados do INMET.
- **C6 (Linux)** precisa dos Dockerfiles e do compose, que dependem de C1 e C2 compilando.
- **C7 (Cyber)** depende do material da aula e usa o C1 como alvo.
- **C8 (QA)** pode começar já, a partir deste plano. O pitch fica para o fim.

| Data | Marco |
|---|---|
| Dom 08/11 | Plano fechado ✔ · criar o repositório no GitHub · `docs/pesquisa.md` (escuta indireta) |
| Seg 09 – Qua 11/11 | C1 e C2: domínio, banco, migrações, Swagger · C3: telas em mock · C5: baixar e limpar os dados do INMET · C8: Features no Azure |
| Qui 12 – Sex 13/11 | C1 e C2 completos com testes · C4: circuito no Wokwi · C5: treino e grade · C8: Test Cases |
| Sáb 14 – Dom 15/11 | Integração ESP32 → Telemetria → Cidadão → App · C6: compose + scripts · C7: regras de PII + taint |
| Seg 16 – Ter 17/11 | C6: provisionamento na VM + teste de falha · C7: dashboard · prints de tudo |
| Qua 18/11 | Vídeos: mobile (3 min), IoT (4), ML (5), OS (7), pitch (4) · READMEs finais |
| Qui 19/11 | Revisão e envio em cada Teams **antes das 23h55** |

---

# PARTE E — PROMPTS PRONTOS PARA ABRIR CADA CHAT

Abra o novo chat **na pasta local do repositório da matéria** (tabela da B1) e cole o prompt da matéria:

**C# →**
> Leia o PLANO.md, Partes A e B e a seção C1. Implemente o Serviço do Cidadão em `servico-cidadao-dotnet/` exatamente como especificado, sem mudar os contratos da Parte B. Ao terminar, rode `dotnet build` e `dotnet test` e registre o uso de IA no `prompts.md`.

**SOA →**
> Leia o PLANO.md, Partes A e B e a seção C2. Implemente o Serviço de Telemetria em Java Spring Boot em `servico-telemetria-java/`, seguindo os contratos da Parte B. Rode `mvn test` no final e registre o uso de IA no `prompts.md`.

**Mobile →**
> Leia o PLANO.md, Partes A e B e as seções C3 e C1.4. Crie o app Expo em `app-mobile/` com modo mock primeiro, depois integre com as APIs reais. Priorize acessibilidade. Registre o uso de IA no `prompts.md`.

**IoT →**
> Leia o PLANO.md, Partes A e B e a seção C4. Gere `sketch.ino`, `diagram.json` e `libraries.txt` do Wokwi em `iot/`, usando os limites do sensor SNS-PANTANAL-01.

**ML →**
> Leia o PLANO.md, Parte A e a seção C5. Monte o notebook em `ml/` com dados reais do INMET (estação A701). Não use dados simulados.

**OS →**
> Leia o PLANO.md, Parte B e a seção C6. Crie os Dockerfiles, o `docker-compose.yml` e os scripts Linux em `infra/`.

**Cyber →**
> Leia o PLANO.md, Partes A e B e a seção C7. Antes de codar, me peça o material ou código-base da aula de Cybersecurity.

**QA →**
> Leia o PLANO.md, Partes A e C8. Escreva todas as Features e Test Cases prontos para eu copiar no Azure Boards, e o roteiro do pitch.

---

# PARTE F — ROTEIRO DE CÓDIGO: C#, SOA E MOBILE

As três matérias são do Pedro. As etapas abaixo são feitas **em ordem**. Cada uma é um bloco de trabalho que cabe numa sessão de chat, tem um prompt pronto para colar e termina num **checkpoint verificável**. Só passe para a próxima depois de bater o checkpoint e fazer o commit.

## Trilhas em paralelo

```
            09/11      10/11      11/11      12/11      13/11      14/11      15/11      16/11      17/11      18/11
C#        [CS1][CS2 ][CS3 ][CS4      ][CS5]
SOA                               [J1][J2 ][J3      ][J4 ][J5]
Mobile               [M1][M2     ][M3      ][M4      ][M5 ][M6]
Integração                                                           [I1      ]
Entrega                                                                         [E1: prints, vídeo, READMEs]
```

- **C# vem primeiro**, porque é o serviço que o app e a Telemetria consomem.
- **Mobile começa em mock** no 10/11, em paralelo, sem esperar nenhum backend.
- **A Telemetria** começa quando o C# já tem o endpoint `POST /api/alertas/sensor`.

## Etapa 0 — Base comum (hoje, ~30 min)

1. ✔ Os 3 repositórios (Olho-dagua, GlobalSolution-SOA e GlobalSolution-Mobile) foram criados e receberam o commit inicial.
2. Conferir o acesso ao **Oracle da FIAP** (`oracle.fiap.com.br:1521/ORCL`) com o RM e a senha, por exemplo no SQL Developer. Não é preciso Docker para o banco.
3. Guardar as credenciais fora dos repositórios: no Cidadão, com `dotnet user-secrets`; na Telemetria, nas variáveis `SPRING_DATASOURCE_*` ou num `.env` local, que está no `.gitignore`.
4. Instalar a ferramenta de migrations do EF: `dotnet tool install --global dotnet-ef`.

✅ **Checkpoint:** o login no Oracle da FIAP funciona e as credenciais estão fora do Git.

## Trilha C# — `servico-cidadao-dotnet/` (seção C1)

| Etapa | O que fazer | Checkpoint |
|---|---|---|
| **CS1 · Esqueleto** | Criar a solução (`dotnet new sln`, que no .NET 10 gera `.slnx`), os 4 projetos em `src/` (`classlib` × 3 + `webapi --use-controllers`), o projeto `xunit` em `tests/`, e as referências **Api → Application → Domain** e **Infrastructure → Application/Domain**. Pacotes: `Oracle.EntityFrameworkCore` e `Microsoft.EntityFrameworkCore.Design` (Infrastructure/Api), `Scalar.AspNetCore` (Api). | `dotnet build` passa e `/scalar` abre vazio na porta 5080 |
| **CS2 · Domínio + testes** | `Coordenada`, enums, `AreaRisco`, `Alerta` (abstrata) + `AlertaSensor` / `AlertaComunitario` / `AlertaPrevisao`, `Ocorrencia`, `Abrigo`, `Inscricao`, exceções de domínio. Os testes de domínio da C1.7 entram **junto**. | `dotnet test` passa com ≥ 5 testes de domínio, **sem banco** |
| **CS3 · EF Core** | `CidadaoDbContext`, configurações `IEntityTypeConfiguration`, TPH do `Alerta`, `Coordenada` como owned type, enums como string, seed da B4. Migration `Inicial`. | `dotnet ef database update` cria as tabelas `OD_` no Oracle e o seed aparece |
| **CS4 · Serviços + API** | `IRepositorio<T>`, serviços, `INotificador` (2 implementações), `IRegraAlertaComunitario` com `TimeProvider`, `ISanitizadorPii`, controllers da C1.4, `X-Api-Key`, ProblemDetails, CORS, `JsonStringEnumConverter` em MAIÚSCULAS. Tudo async com `CancellationToken`. Testes da regra comunitária e do sanitizador. | Todos os endpoints respondem no Scalar · 3 POSTs de ocorrência na área 1 criam 1 alerta comunitário |
| **CS5 · Acabamento** | README da pasta (como rodar, tabela rubrica → código, `classDiagram` Mermaid), arquivo `.http` com exemplos de chamadas, prints do Scalar. | Um colega clona o repositório e roda seguindo só o README |

## Trilha SOA — `servico-telemetria-java/` (seção C2)

| Etapa | O que fazer | Checkpoint |
|---|---|---|
| **J1 · Esqueleto** | Gerar o projeto no Spring Initializr (Maven, Java 21, groupId `br.com.olhodagua`, artifactId `telemetria`) com: Web, Data JPA, Oracle Driver, Flyway, Validation e Actuator. Adicionar `flyway-database-oracle` e o springdoc compatível. Criar os pacotes `domain` / `application` / `infrastructure` / `web`. `application.yml` com a porta 8081, `ddl-auto: validate`, histórico do Flyway em `OT_FLYWAY_HISTORY` e credenciais só por variável de ambiente. | `mvn spring-boot:run` sobe e o `/swagger-ui.html` abre |
| **J2 · Migrações** | Migrações V1 a V4 em SQL do Oracle (`OT_SENSOR`, `OT_LEITURA`, `OT_EVENTO_NIVEL` e seed dos 4 sensores da B4) e as entidades JPA batendo com elas. | Ao subir, o Flyway cria e popula as tabelas `OT_` no Oracle, e o `validate` passa |
| **J3 · Domínio + endpoints** | `ClassificadorNivel` (puro, com testes de fronteira), `LeituraService` com a regra da C2.3, controllers da C2.4, DTOs como `record`, Bean Validation, `@RestControllerAdvice` com `ProblemDetail`, exemplos com `@Schema`. | O Swagger cria sensor, registra leitura e mostra o status · `mvn test` passa |
| **J4 · Integração com o C#** | `CidadaoClient` (RestClient com `X-Api-Key`) para os contratos ② e ③, guardando o `alertaId`; `@Scheduled` que reenvia alertas e encerramentos pendentes; migração V5 (evolução do esquema); testes com Mockito. | Com o C# no ar, a sequência 100 → 160 → 230 → 310 → 90 gera 3 alertas e depois o encerramento · com o C# desligado, a leitura não falha e o alerta é reenviado quando ele volta |
| **J5 · Acabamento** | README com o **nome dos integrantes**, instruções, tabela rubrica → código e prints do Swagger. Teste de integração com MockMvc. | `mvn test` passa · README completo |

## Trilha Mobile — `app-mobile/` (seção C3)

| Etapa | O que fazer | Checkpoint |
|---|---|---|
| **M1 · Esqueleto** | `npx create-expo-app@latest app-mobile` (o template padrão já vem com TypeScript e Expo Router), limpar as telas de exemplo, criar a estrutura de rotas da C3.3. `npx expo install expo-sqlite expo-location @react-native-community/netinfo`. | O app abre no Expo Go e no web com as 4 abas vazias |
| **M2 · Tipos + mock + tema** | `types/` espelhando a B5 e a C1.4, `services/api.ts` com o chaveamento `EXPO_PUBLIC_USE_MOCK`, JSONs de mock com os dados da B4, tema com fonte grande e alto contraste em um contexto. | Trocar a flag de mock não quebra nada |
| **M3 · Telas** | Onboarding, Início (cartão de nível com cor + ícone + texto), detalhe do alerta, Reportar, Abrigos + detalhe com "Como chegar", Perfil. Já com `accessibilityLabel` e alvos ≥ 48 dp. | A jornada completa funciona em mock |
| **M4 · SQLite + offline** | Tabelas `preferencias`, `relatos` e `cache_status`; fila de relatos pendentes; sincronização ao abrir, ao reconectar e ao puxar para atualizar; banner "Sem internet". | No modo avião, o relato fica pendente · ao reconectar, é enviado e muda de status |
| **M5 · API real** | Apontar para as APIs (IP da máquina na rede), tratar erros e timeout, revisar a acessibilidade com o TalkBack. | O app mostra dados vindos do C# e da Telemetria |
| **M6 · Acabamento** | README com a Ficha, como rodar, prints de todas as telas e o link do vídeo (≤ 3 min). | README completo e vídeo gravado |

## I1 — Integração ponta a ponta (16–17/11)

1. Subir o banco, o C# e a Telemetria.
2. Rodar uma sequência de leituras com o `.http` ou com curl.
3. Ver o alerta aparecer no app.
4. Fazer um relato offline e ver ele sincronizar.

Gravar essa sequência: é a base do vídeo do Mobile e do pitch.

## Prompts para cada etapa

Abra o chat **na pasta local do repositório daquela matéria** (veja a tabela da B1) e cole o prompt da etapa, trocando o código (CS1, J3, M4…):

> Leia o PLANO.md (Partes B e F e a seção da matéria). Vamos fazer a etapa **CS1** da Parte F. Siga exatamente os nomes, portas e contratos da Parte B. No fim, rode o checkpoint da etapa, me mostre o resultado e registre o uso de IA no `prompts.md`.

**Regras para todos os chats:**
- Uma etapa por vez, e commit a cada checkpoint batido.
- Se precisar mudar um contrato, atualize a Parte B do PLANO.md **no mesmo commit** e avise o Pedro, para ele copiar o PLANO.md para os outros repositórios.
- Você precisa saber explicar cada linha, porque o PDF exige que todos os integrantes expliquem o projeto inteiro.
