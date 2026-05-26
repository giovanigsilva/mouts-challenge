import http from 'k6/http';
import { check } from 'k6';

export const options = {
  vus: 1,
  iterations: 5,
  thresholds: {
    http_req_failed: ['rate<0.01'],
    http_req_duration: ['p(95)<500'],
  },
};

const baseUrl = __ENV.BASE_URL || 'http://localhost:8080';

export default function () {
  const live = http.get(`${baseUrl}/health/live`);
  check(live, {
    'live status 200': response => response.status === 200,
  });

  const ready = http.get(`${baseUrl}/health/ready`);
  check(ready, {
    'ready status 200': response => response.status === 200,
  });
}
