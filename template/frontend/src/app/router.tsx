import { createBrowserRouter, Navigate } from 'react-router-dom'

import { BootstrapPage } from '@/app/BootstrapPage'

export const router = createBrowserRouter([
  {
    path: '/',
    element: <Navigate to="/dashboard" replace />,
  },
  {
    path: '/dashboard',
    element: <BootstrapPage />,
  },
])
