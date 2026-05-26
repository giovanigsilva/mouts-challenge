import http from 'k6/http';
import { check } from 'k6';

export const options = {
  vus: 2,
  duration: '20s',
  thresholds: {
    http_req_failed: ['rate<0.05'],
    http_req_duration: ['p(95)<800'],
  },
};

const baseUrl = __ENV.BASE_URL || 'http://localhost:8080';
const token = __ENV.TOKEN || '';

export default function () {
  if (!token) {
    const response = http.get(`${baseUrl}/health/live`);
    check(response, { 'token ausente executa smoke': r => r.status === 200 });
    return;
  }

  const now = Date.now();
  const payload = JSON.stringify({
    saleNumber: `K6-${__VU}-${__ITER}-${now}`,
    saleDate: new Date().toISOString(),
    customerExternalId: '5c9d7b1e-2a63-4e69-9c55-4c0e8142f8c1',
    customerName: 'Cliente k6',
    branchExternalId: '7a2b2c71-6c2e-4f54-8a7e-32159a4d53e2',
    branchName: 'Loja k6',
    items: [
      {
        productExternalId: '33a8b4f9-4a6e-49c9-91df-ec7b40b3b1a1',
        productName: 'Produto k6',
        quantity: 4,
        unitPrice: 50.0,
      },
    ],
  });

  const response = http.post(`${baseUrl}/api/sales`, payload, {
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
  });

  check(response, {
    'create status 201': r => r.status === 201,
  });
}
