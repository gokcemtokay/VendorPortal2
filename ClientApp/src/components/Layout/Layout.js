import React, { useState } from 'react';
import { Outlet, Link, useNavigate } from 'react-router-dom';
import { Navbar, Nav, Container, NavDropdown, Badge, Dropdown } from 'react-bootstrap';
import { useAuth } from '../../context/AuthContext';
import { useFirma } from '../../context/FirmaContext';

const Layout = () => {
    const { user, logout, hasRole } = useAuth();
    const { currentFirma, firmalar = [], selectFirma, loading } = useFirma();
    const navigate = useNavigate();
    const [unreadNotifications] = useState(3); // Backend'den gelecek

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    const handleFirmaChange = async (firmaId) => {
        try {
            await selectFirma(firmaId);
        } catch (error) {
            console.error('Firma değiştirilirken hata:', error);
        }
    };

    // Kullanıcının baş harflerini al
    const getUserInitials = () => {
        if (user?.name) {
            return user.name.split(' ').map(n => n[0]).join('').toUpperCase().substring(0, 2);
        }
        return user?.email?.[0]?.toUpperCase() || 'U';
    };

    return (
        <div className="min-vh-100 d-flex flex-column">
            <Navbar bg="dark" variant="dark" expand="lg" className="shadow-sm">
                <Container fluid>
                    {/* Logo */}
                    <Navbar.Brand as={Link} to="/" className="fw-bold">
                        <i className="fas fa-industry me-2"></i>
                        Vendor Portal
                    </Navbar.Brand>

                    <Navbar.Toggle aria-controls="basic-navbar-nav" />

                    <Navbar.Collapse id="basic-navbar-nav">
                        {/* Sol menü */}
                        <Nav className="me-auto">
                            <Nav.Link as={Link} to="/">
                                <i className="fas fa-home me-1"></i>
                                Dashboard
                            </Nav.Link>

                            {(hasRole('Admin') || hasRole('Musteri')) && (
                                <Nav.Link as={Link} to="/siparis?view=satin-alma">
                                    <i className="fas fa-shopping-cart me-1"></i>
                                    Satın Alma
                                </Nav.Link>
                            )}

                            {(hasRole('Admin') || hasRole('Tedarikci')) && (
                                <Nav.Link as={Link} to="/siparis?view=satis">
                                    <i className="fas fa-box me-1"></i>
                                    Satış
                                </Nav.Link>
                            )}

                            <Nav.Link as={Link} to="/ihale">
                                <i className="fas fa-gavel me-1"></i>
                                İhale Yönetimi
                            </Nav.Link>

                            <NavDropdown title={<><i className="fas fa-cog me-1"></i>Yönetim</>} id="yonetim-dropdown">
                                <NavDropdown.Item as={Link} to="/firma">
                                    <i className="fas fa-building me-2"></i>
                                    Firmalar
                                </NavDropdown.Item>
                                <NavDropdown.Item as={Link} to="/malzeme">
                                    <i className="fas fa-cube me-2"></i>
                                    Malzemeler
                                </NavDropdown.Item>
                                {hasRole('Admin') && (
                                    <>
                                        <NavDropdown.Divider />
                                        <NavDropdown.Item as={Link} to="/users">
                                            <i className="fas fa-users me-2"></i>
                                            Kullanıcı Yönetimi
                                        </NavDropdown.Item>
                                    </>
                                )}
                            </NavDropdown>
                        </Nav>

                        {/* Sağ menü - JetSRM tarzı */}
                        <Nav className="align-items-center">
                            {/* Firma Değiştirme Dropdown */}
                            {!loading && firmalar && firmalar.length > 0 && (
                                <Dropdown className="me-3">
                                    <Dropdown.Toggle
                                        variant="outline-light"
                                        size="sm"
                                        id="firma-dropdown"
                                        className="d-flex align-items-center"
                                    >
                                        <i className="fas fa-building me-2"></i>
                                        <span className="d-none d-md-inline">
                                            {currentFirma?.ad || 'Firma Seç'}
                                        </span>
                                    </Dropdown.Toggle>

                                    <Dropdown.Menu align="end">
                                        <Dropdown.Header>
                                            <i className="fas fa-building me-2"></i>
                                            Aktif Firma
                                        </Dropdown.Header>
                                        <Dropdown.Divider />
                                        {firmalar.map((firma) => (
                                            <Dropdown.Item
                                                key={firma.id}
                                                active={currentFirma?.id === firma.id}
                                                onClick={() => handleFirmaChange(firma.id)}
                                            >
                                                {firma.ad}
                                                {currentFirma?.id === firma.id && (
                                                    <i className="fas fa-check ms-2 text-success"></i>
                                                )}
                                            </Dropdown.Item>
                                        ))}
                                    </Dropdown.Menu>
                                </Dropdown>
                            )}

                            {/* Bildirimler */}
                            <Dropdown className="me-3">
                                <Dropdown.Toggle
                                    variant="link"
                                    id="notifications-dropdown"
                                    className="text-white position-relative p-2"
                                    style={{ textDecoration: 'none' }}
                                >
                                    <i className="fas fa-bell fs-5"></i>
                                    {unreadNotifications > 0 && (
                                        <Badge
                                            bg="danger"
                                            pill
                                            className="position-absolute top-0 start-100 translate-middle"
                                            style={{ fontSize: '0.65rem' }}
                                        >
                                            {unreadNotifications}
                                        </Badge>
                                    )}
                                </Dropdown.Toggle>

                                <Dropdown.Menu align="end" style={{ minWidth: '300px' }}>
                                    <Dropdown.Header className="d-flex justify-content-between align-items-center">
                                        <span>
                                            <i className="fas fa-bell me-2"></i>
                                            Bildirimler
                                        </span>
                                        {unreadNotifications > 0 && (
                                            <Badge bg="danger" pill>{unreadNotifications}</Badge>
                                        )}
                                    </Dropdown.Header>
                                    <Dropdown.Divider />

                                    {/* Örnek bildirimler */}
                                    <Dropdown.Item className="py-2">
                                        <div className="d-flex">
                                            <div className="flex-shrink-0">
                                                <i className="fas fa-info-circle text-primary me-2"></i>
                                            </div>
                                            <div>
                                                <div className="fw-bold small">Yeni sipariş oluşturuldu</div>
                                                <div className="text-muted small">5 dakika önce</div>
                                            </div>
                                        </div>
                                    </Dropdown.Item>

                                    <Dropdown.Item className="py-2">
                                        <div className="d-flex">
                                            <div className="flex-shrink-0">
                                                <i className="fas fa-check-circle text-success me-2"></i>
                                            </div>
                                            <div>
                                                <div className="fw-bold small">Sipariş onaylandı</div>
                                                <div className="text-muted small">2 saat önce</div>
                                            </div>
                                        </div>
                                    </Dropdown.Item>

                                    <Dropdown.Divider />
                                    <Dropdown.Item as={Link} to="/notifications" className="text-center small text-primary">
                                        Tüm bildirimleri gör
                                    </Dropdown.Item>
                                </Dropdown.Menu>
                            </Dropdown>

                            {/* Kullanıcı Menüsü */}
                            <NavDropdown
                                title={
                                    <span className="d-flex align-items-center">
                                        {/* Avatar */}
                                        <div
                                            className="rounded-circle bg-primary text-white d-flex align-items-center justify-content-center me-2"
                                            style={{ width: '32px', height: '32px', fontSize: '0.875rem', fontWeight: 'bold' }}
                                        >
                                            {getUserInitials()}
                                        </div>
                                        {/* Kullanıcı bilgileri */}
                                        <div className="d-none d-lg-flex flex-column align-items-start text-white">
                                            <span style={{ fontSize: '0.875rem', lineHeight: '1.2' }}>
                                                {user?.name || user?.email?.split('@')[0]}
                                            </span>
                                            {user?.rol && (
                                                <Badge
                                                    bg={user.rol === 'Admin' ? 'danger' : user.rol === 'Musteri' ? 'primary' : 'success'}
                                                    style={{ fontSize: '0.65rem' }}
                                                >
                                                    {user.rol}
                                                </Badge>
                                            )}
                                        </div>
                                    </span>
                                }
                                id="user-dropdown"
                                align="end"
                                className="no-caret"
                            >
                                <Dropdown.Header>
                                    <div className="text-center">
                                        <div
                                            className="rounded-circle bg-primary text-white d-inline-flex align-items-center justify-content-center mb-2"
                                            style={{ width: '48px', height: '48px', fontSize: '1.25rem', fontWeight: 'bold' }}
                                        >
                                            {getUserInitials()}
                                        </div>
                                        <div className="fw-bold">{user?.name || 'Kullanıcı'}</div>
                                        <div className="text-muted small">{user?.email}</div>
                                    </div>
                                </Dropdown.Header>
                                <Dropdown.Divider />

                                <Dropdown.Item as={Link} to="/profile">
                                    <i className="fas fa-user me-2"></i>
                                    Profil Ayarları
                                </Dropdown.Item>

                                <Dropdown.Item as={Link} to="/settings">
                                    <i className="fas fa-cog me-2"></i>
                                    Hesap Ayarları
                                </Dropdown.Item>

                                {hasRole('Admin') && (
                                    <Dropdown.Item as={Link} to="/users">
                                        <i className="fas fa-users-cog me-2"></i>
                                        Kullanıcı Yönetimi
                                    </Dropdown.Item>
                                )}

                                <Dropdown.Divider />

                                <Dropdown.Item onClick={handleLogout} className="text-danger">
                                    <i className="fas fa-sign-out-alt me-2"></i>
                                    Çıkış Yap
                                </Dropdown.Item>
                            </NavDropdown>
                        </Nav>
                    </Navbar.Collapse>
                </Container>
            </Navbar>

            {/* Ana içerik */}
            <Container fluid className="flex-grow-1 py-4">
                <Outlet />
            </Container>

            {/* Footer */}
            <footer className="bg-light border-top py-3 mt-auto">
                <Container fluid>
                    <div className="row align-items-center">
                        <div className="col-md-6 text-center text-md-start">
                            <small className="text-muted">
                                &copy; {new Date().getFullYear()} Vendor Portal - Tüm hakları saklıdır.
                            </small>
                        </div>
                        <div className="col-md-6 text-center text-md-end">
                            <small className="text-muted">
                                {currentFirma && (
                                    <>
                                        <i className="fas fa-building me-1"></i>
                                        {currentFirma.ad} |
                                    </>
                                )}
                                {' '}Sürüm 2.0
                            </small>
                        </div>
                    </div>
                </Container>
            </footer>

            <style jsx>{`
        .no-caret .dropdown-toggle::after {
          display: none;
        }
      `}</style>
        </div>
    );
};

export default Layout;