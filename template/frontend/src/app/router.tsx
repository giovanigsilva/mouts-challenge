import { createBrowserRouter, Navigate } from 'react-router-dom'

import { BootstrapPage } from '@/app/BootstrapPage'
import { ProtectedLayout } from '@/shared/components/layout/ProtectedLayout'
import { LoginPage } from '@/features/auth/pages/LoginPage'
import { SaleCreatePage } from '@/features/sales/pages/SaleCreatePage'
import { SaleDetailsPage } from '@/features/sales/pages/SaleDetailsPage'
import { SaleEditPage } from '@/features/sales/pages/SaleEditPage'
import { SalesListPage } from '@/features/sales/pages/SalesListPage'

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
        element: <SalesListPage />,
      },
      {
        path: '/sales/new',
        element: <SaleCreatePage />,
      },
      {
        path: '/sales/:id',
        element: <SaleDetailsPage />,
      },
      {
        path: '/sales/:id/edit',
        element: <SaleEditPage />,
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
