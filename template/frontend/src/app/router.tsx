import { createBrowserRouter, Navigate } from 'react-router-dom'

import { ProtectedLayout } from '@/shared/components/layout/ProtectedLayout'
import { LoginPage } from '@/features/auth/pages/LoginPage'
import { DashboardPage } from '@/features/dashboard/pages/DashboardPage'
import { HealthPage } from '@/features/health/pages/HealthPage'
import { SaleCreatePage } from '@/features/sales/pages/SaleCreatePage'
import { SaleDetailsPage } from '@/features/sales/pages/SaleDetailsPage'
import { SaleEditPage } from '@/features/sales/pages/SaleEditPage'
import { SalesListPage } from '@/features/sales/pages/SalesListPage'
import { CreateUserPage } from '@/features/users/pages/CreateUserPage'

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
        element: <DashboardPage />,
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
        element: <CreateUserPage />,
      },
      {
        path: '/health',
        element: <HealthPage />,
      },
    ],
  },
])
