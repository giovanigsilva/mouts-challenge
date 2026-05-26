import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  vus: 5,
  duration: '45s',
  thresholds: {
    http_req_failed: ['rate<0.05'],
    http_req_duration: ['p(95)<800'],
  },
};

const baseUrl = __ENV.BASE_URL || 'http://localhost:8080';
const token = __ENV.TOKEN || '';

function authHeaders() {
  return token ? { Authorization: `Bearer ${token}` } : {};
}

export default function () {
  const live = http.get(`${baseUrl}/health/live`);
  check(live, { 'live status 200': r => r.status === 200 });

  const list = http.get(`${baseUrl}/api/sales?page=1&pageSize=10`, { headers: authHeaders() });
  check(list, {
    'list ok or auth expected': r => r.status === 200 || r.status === 401 || r.status === 403,
  });

  if (token && __ITER % 5 === 0) {
    const payload = JSON.stringify({
      saleNumber: `K6-MIX-${__VU}-${__ITER}-${Date.now()}`,
      saleDate: new Date().toISOString(),
      customerExternalId: '5c9d7b1e-2a63-4e69-9c55-4c0e8142f8c1',
      customerName: 'Cliente k6 misto',
      branchExternalId: '7a2b2c71-6c2e-4f54-8a7e-32159a4d53e2',
      branchName: 'Loja k6',
      items: [
        {
          productExternalId: '33a8b4f9-4a6e-49c9-91df-ec7b40b3b1a1',
          productName: 'Produto k6',
          quantity: 10,
          unitPrice: 25.0,
        },
      ],
    });

    const create = http.post(`${baseUrl}/api/sales`, payload, {
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    });

    check(create, { 'create status 201': r => r.status === 201 });
  }

  sleep(1);
}
