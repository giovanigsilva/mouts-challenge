import { createBrowserRouter, Navigate } from 'react-router-dom'

import { BootstrapPage } from '@/app/BootstrapPage'
import { ProtectedLayout } from '@/shared/components/layout/ProtectedLayout'
import { LoginPage } from '@/features/auth/pages/LoginPage'

export const router = createBrowserRouter([
  {
    path: '/',
    element: <Navigate to="/dashboard" replace />,
  },
  {
    path: '/login',
    element: <LoginPage />,
  },
  {
    element: <ProtectedLayout />,
    children: [
      {
        path: '/dashboard',
        element: <BootstrapPage />,
      },
      {
        path: '/sales',
        element: <BootstrapPage />,
      },
      {
        path: '/sales/new',
        element: <BootstrapPage />,
      },
      {
        path: '/users/new',
        element: <BootstrapPage />,
      },
      {
        path: '/health',
        element: <BootstrapPage />,
      },
    ],
  },
])
