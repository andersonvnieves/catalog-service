import http from 'k6/http';
import { check, sleep } from 'k6';
import { SharedArray } from 'k6/data';

// Carrega a massa de dados
const games = new SharedArray('games data', function () {
  return JSON.parse(open('./jogos.json'));
});

export const options = {
  insecureSkipTLSVerify: true,
  
  scenarios: {
    ingest_games: {
      executor: 'shared-iterations',
      vus: 100, 
      iterations: games.length, 
      maxDuration: '320s',
    },
  },
};

export default function () {
  const game = games[__ITER];
  const url = 'https://localhost:30443/api/game';
  const payload = JSON.stringify(game);  
  const token = __ENV.TOKEN;
  
  const params = {
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    },
  };

  const res = http.post(url, payload, params);

  check(res, {
    'status é 200 ou 201 (Created)': (r) => r.status === 200 || r.status === 201,
  });

  sleep(1); 
}