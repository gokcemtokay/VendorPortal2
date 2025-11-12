import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import { useAuth } from './context/AuthContext';

// Layout
import Layout from './components/Layout/Layout';
import PrivateRoute from './components/Auth/PrivateRoute';

// Pages
import Login from './pages/Auth/Login';
import Dashboard from './pages/Dashboard/Dashboard';

// Firma
import FirmaList from './pages/Firma/FirmaList';
import FirmaCreate from './pages/Firma/FirmaCreate';
import FirmaEdit from './pages/Firma/FirmaEdit';
import FirmaDetails from './pages/Firma/FirmaDetails';

// Malzeme
import MalzemeList from './pages/Malzeme/MalzemeList';
import MalzemeCreate from './pages/Malzeme/MalzemeCreate';
import MalzemeEdit from './pages/Malzeme/MalzemeEdit';

// İhale
import IhaleList from './pages/Ihale/IhaleList';
import IhaleCreate from './pages/Ihale/IhaleCreate';
import IhaleDetails from './pages/Ihale/IhaleDetails';
import IhaleTeklifler from './pages/Ihale/IhaleTeklifler';

// Sipariş
import SiparisList from './pages/Siparis/SiparisList';
import SiparisDetails from './pages/Siparis/SiparisDetails';

// User Management
import UserManagement from './pages/User/UserManagement';
import UserCreate from './pages/User/UserCreate';
import UserEdit from './pages/User/UserEdit';

function App() {
    const { isAuthenticated } = useAuth();

    return (
        <>
            <ToastContainer
                position="top-right"
                autoClose={3000}
                hideProgressBar={false}
                newestOnTop={true}
                closeOnClick
                rtl={false}
                pauseOnFocusLoss
                draggable
                pauseOnHover
                theme="light"
            />

            <Routes>
                {/* Login Route */}
                <Route path="/login" element={
                    isAuthenticated ? <Navigate to="/" replace /> : <Login />
                } />

                {/* Protected Routes */}
                <Route element={<Layout />}>
                    <Route path="/" element={
                        <PrivateRoute>
                            <Dashboard />
                        </PrivateRoute>
                    } />

                    {/* Firma Routes */}
                    <Route path="/firma" element={
                        <PrivateRoute>
                            <FirmaList />
                        </PrivateRoute>
                    } />
                    <Route path="/firma/create" element={
                        <PrivateRoute roles={['Admin', 'Musteri']}>
                            <FirmaCreate />
                        </PrivateRoute>
                    } />
                    <Route path="/firma/edit/:id" element={
                        <PrivateRoute>
                            <FirmaEdit />
                        </PrivateRoute>
                    } />
                    <Route path="/firma/details/:id" element={
                        <PrivateRoute>
                            <FirmaDetails />
                        </PrivateRoute>
                    } />

                    {/* Malzeme Routes */}
                    <Route path="/malzeme" element={
                        <PrivateRoute>
                            <MalzemeList />
                        </PrivateRoute>
                    } />
                    <Route path="/malzeme/create" element={
                        <PrivateRoute>
                            <MalzemeCreate />
                        </PrivateRoute>
                    } />
                    <Route path="/malzeme/edit/:id" element={
                        <PrivateRoute>
                            <MalzemeEdit />
                        </PrivateRoute>
                    } />

                    {/* İhale Routes */}
                    <Route path="/ihale" element={
                        <PrivateRoute>
                            <IhaleList />
                        </PrivateRoute>
                    } />
                    <Route path="/ihale/create" element={
                        <PrivateRoute roles={['Admin', 'Musteri']}>
                            <IhaleCreate />
                        </PrivateRoute>
                    } />
                    <Route path="/ihale/details/:id" element={
                        <PrivateRoute>
                            <IhaleDetails />
                        </PrivateRoute>
                    } />
                    <Route path="/ihale/teklifler/:id" element={
                        <PrivateRoute>
                            <IhaleTeklifler />
                        </PrivateRoute>
                    } />

                    {/* Sipariş Routes */}
                    <Route path="/siparis" element={
                        <PrivateRoute>
                            <SiparisList />
                        </PrivateRoute>
                    } />
                    <Route path="/siparis/details/:id" element={
                        <PrivateRoute>
                            <SiparisDetails />
                        </PrivateRoute>
                    } />

                    {/* User Management Routes - Sadece Admin */}
                    <Route path="/users" element={
                        <PrivateRoute roles={['Admin']}>
                            <UserManagement />
                        </PrivateRoute>
                    } />
                    <Route path="/users/create" element={
                        <PrivateRoute roles={['Admin']}>
                            <UserCreate />
                        </PrivateRoute>
                    } />
                    <Route path="/users/edit/:id" element={
                        <PrivateRoute roles={['Admin']}>
                            <UserEdit />
                        </PrivateRoute>
                    } />
                </Route>

                {/* 404 Redirect */}
                <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
        </>
    );
}

export default App;