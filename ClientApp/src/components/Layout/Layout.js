import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Navbar, Nav, Container, NavDropdown } from 'react-bootstrap';
import { useAuth } from '../../context/AuthContext';
import { useFirma } from '../../context/FirmaContext';
import NotificationDropdown from '../Notifications/NotificationDropdown';
import FirmaSwitcher from '../FirmaSwitcher/FirmaSwitcher';
import './Layout.css';

const Layout = ({ children }) => {
    const { user, logout, hasRole } = useAuth();
    const { aktiveFirma } = useFirma();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    return (
        <div className="app-container">
            <Navbar bg="white" expand="lg" className="navbar-custom shadow-sm">
                <Container fluid>
                    <Navbar.Brand as={Link} to="/" className="brand-logo">
                        <i className="fas fa-cube me-2"></i>
                        <span className="brand-text">VendorPortal</span>
                    </Navbar.Brand>

                    <Navbar.Toggle aria-controls="basic-navbar-nav" />

                    <Navbar.Collapse id="basic-navbar-nav">
                        <Nav className="me-auto">
                            <Nav.Link as={Link} to="/">
                                <i className="fas fa-home me-1"></i> Dashboard
                            </Nav.Link>

                            <Nav.Link as={Link} to="/firma">
                                <i className="fas fa-building me-1"></i> Firmalar
                            </Nav.Link>

                            <Nav.Link as={Link} to="/malzeme">
                                <i className="fas fa-box me-1"></i> Malzemeler
                            </Nav.Link>

                            <Nav.Link as={Link} to="/ihale">
                                <i className="fas fa-gavel me-1"></i> İhaleler
                            </Nav.Link>

                            <Nav.Link as={Link} to="/siparis">
                                <i className="fas fa-shopping-cart me-1"></i> Siparişler
                            </Nav.Link>

                            {hasRole('Admin') && (
                                <Nav.Link as={Link} to="/user-management">
                                    <i className="fas fa-users me-1"></i> Kullanıcılar
                                </Nav.Link>
                            )}
                        </Nav>

                        <Nav className="align-items-center">
                            {/* Firma Switcher */}
                            <FirmaSwitcher />

                            {/* Notifications */}
                            <NotificationDropdown />

                            {/* User Menu */}
                            <NavDropdown
                                title={
                                    <span className="user-menu-trigger">
                                        <i className="fas fa-user-circle me-1"></i>
                                        {user?.name || user?.email}
                                    </span>
                                }
                                id="user-dropdown"
                                align="end"
                                className="user-dropdown"
                            >
                                <NavDropdown.Item as={Link} to="/profile">
                                    <i className="fas fa-user me-2"></i> Profil
                                </NavDropdown.Item>
                                <NavDropdown.Item as={Link} to="/settings">
                                    <i className="fas fa-cog me-2"></i> Ayarlar
                                </NavDropdown.Item>
                                <NavDropdown.Divider />
                                <NavDropdown.Item onClick={handleLogout}>
                                    <i className="fas fa-sign-out-alt me-2"></i> Çıkış Yap
                                </NavDropdown.Item>
                            </NavDropdown>
                        </Nav>
                    </Navbar.Collapse>
                </Container>
            </Navbar>

            {/* Aktif Firma Bilgisi */}
            {aktiveFirma && (
                <div className="firma-info-bar">
                    <Container fluid>
                        <div className="firma-info-content">
                            <i className="fas fa-briefcase me-2"></i>
                            <strong>Aktif Firma:</strong>
                            <span className="ms-2">{aktiveFirma.firmaAd}</span>
                        </div>
                    </Container>
                </div>
            )}

            <main className="main-content">
                <Container fluid className="py-4">
                    {children}
                </Container>
            </main>

            <footer className="footer mt-auto py-3 bg-light">
                <Container fluid>
                    <span className="text-muted">© 2025 VendorPortal. Tüm hakları saklıdır.</span>
                </Container>
            </footer>
        </div>
    );
};

export default Layout;