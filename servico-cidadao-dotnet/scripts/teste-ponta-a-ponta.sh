#!/usr/bin/env bash
B=http://localhost:5080/api; K='X-Api-Key: olhodagua-dev-chave'; J='Content-Type: application/json'
D=11111111-2222-3333-4444-555555555555
SUF=$(printf '%06d' $((RANDOM * 30 % 1000000)))

echo "== inscricao (201 ou 200)"; curl -s -o /dev/null -w '[%{http_code}]\n' -H "$J" -d "{\"dispositivoId\":\"$D\",\"areaRiscoId\":1,\"prefereFonteGrande\":true,\"prefereAltoContraste\":false}" $B/inscricoes
echo "== alerta sensor SEM chave (401)"; curl -s -o /dev/null -w '[%{http_code}]\n' -H "$J" -d '{"areaRiscoId":1,"sensorCodigo":"SNS-PANTANAL-01","nivelCm":235,"faixa":"ALERTA"}' $B/alertas/sensor
echo "== alerta sensor ATENCAO (201)"; curl -s -w ' [%{http_code}]\n' -H "$J" -H "$K" -d '{"areaRiscoId":1,"sensorCodigo":"SNS-PANTANAL-01","nivelCm":160,"faixa":"ATENCAO","detectadoEm":"2026-11-15T18:30:00-03:00"}' $B/alertas/sensor
echo "== alerta sensor ALERTA (201, encerra o ATENCAO)"; R=$(curl -s -H "$J" -H "$K" -d '{"areaRiscoId":1,"sensorCodigo":"SNS-PANTANAL-01","nivelCm":235,"faixa":"ALERTA","detectadoEm":"2026-11-15T18:42:00-03:00"}' $B/alertas/sensor); echo "$R"
ALERTA_ID=$(echo "$R" | grep -o '"alertaId":[0-9]*' | cut -d: -f2)
echo "== alertas SENSOR ativos na area 1 (deve haver só 1)"; curl -s "$B/alertas?areaId=1" | grep -o '"tipo":"SENSOR"' | wc -l
echo "== 3 relatos (1o com CPF e telefone)"
for n in 1 2 3; do curl -s -o /tmp/oc$n.json -w "[%{http_code}] " -H "$J" -d "{\"tipo\":\"RUA_ALAGADA\",\"descricao\":\"Rua tomada. Sou Maria CPF 123.456.789-09 tel (11) 98888-7777\",\"latitude\":-23.49876,\"longitude\":-46.44251,\"idLocalApp\":\"aaaaaaaa-0000-0000-0000-${SUF}00000$n\"}" $B/ocorrencias; done; echo; cat /tmp/oc1.json; echo
echo "== reenvio idempotente (200)"; curl -s -o /dev/null -w '[%{http_code}]\n' -H "$J" -d "{\"tipo\":\"RUA_ALAGADA\",\"latitude\":-23.4990,\"longitude\":-46.4425,\"idLocalApp\":\"aaaaaaaa-0000-0000-0000-${SUF}000001\"}" $B/ocorrencias
echo "== alertas ativos da area 1 (ordem de prioridade)"; curl -s "$B/alertas?areaId=1" | grep -o '"tipo":"[A-Z]*","areaRiscoId":1,"areaNome":"[^"]*","severidade":"[A-Z]*","mensagem":"[^"]*","prioridade":[0-9]*'
echo "== notificacoes enviadas ao dispositivo"; curl -s "$B/notificacoes?dispositivoId=$D&limite=5" | grep -o '"mensagem":"[^"]*"'
echo "== encerrar alerta $ALERTA_ID (204, e 204 de novo)"; curl -s -o /dev/null -w '[%{http_code}] ' -X PATCH -H "$K" $B/alertas/$ALERTA_ID/encerrar; curl -s -o /dev/null -w '[%{http_code}]\n' -X PATCH -H "$K" $B/alertas/$ALERTA_ID/encerrar
echo "== ranking"; curl -s "$B/areas/ranking?dias=7"; echo
echo "== detalhe area 1"; curl -s "$B/areas/1"; echo
echo "== abrigo: entrada de 59 no abrigo 2 (200) e mais 5 (409)"; curl -s -o /dev/null -w '[%{http_code}] ' -H "$J" -d '{"pessoas":59}' $B/abrigos/2/entrada; curl -s -w ' [%{http_code}]\n' -H "$J" -d '{"pessoas":5}' $B/abrigos/2/entrada
echo "== erros: 404, fora da area (400), validacao (400)"
curl -s -o /dev/null -w '[%{http_code}] ' $B/alertas/9999
curl -s -w ' [%{http_code}]\n' -H "$J" -d '{"tipo":"RUA_ALAGADA","latitude":10,"longitude":10,"idLocalApp":"bbbbbbbb-0000-0000-0000-000000000001"}' $B/ocorrencias
curl -s -w ' [%{http_code}]\n' -H "$J" -d '{"latitude":-23.49}' $B/ocorrencias
