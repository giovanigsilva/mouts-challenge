import http from 'k6/http';
import { check } from 'k6';

export const options = {
  vus: 5,
  duration: '30s',
  thresholds: {
    http_req_failed: ['rate<0.01'],
    http_req_duration: ['p(95)<500'],
  },
};

const baseUrl = __ENV.BASE_URL || 'http://localhost:8080';
const token = __ENV.TOKEN || '';
const saleId = __ENV.SALE_ID || '';

function headers() {
  return token ? { Authorization: `Bearer ${token}` } : {};
}

export default function () {
  const list = http.get(`${baseUrl}/api/sales?page=1&pageSize=20`, { headers: headers() });
  check(list, {
    'list status 200 or auth expected': response => response.status === 200 || response.status === 401 || response.status === 403,
  });

  if (saleId) {
    const detail = http.get(`${baseUrl}/api/sales/${saleId}`, { headers: headers() });
    check(detail, {
      'detail status 200 or auth expected': response => response.status === 200 || response.status === 401 || response.status === 403 || response.status === 404,
    });
  }
}
