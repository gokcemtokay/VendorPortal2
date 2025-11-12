import React from 'react';
import { Outlet, Link, useNavigate } from 'react-router-dom';
import { Navbar, Nav, Container, NavDropdown } from 'react-bootstrap';
import { useAuth } from '../../context/AuthContext';

const Layout = () => {
  const { user, logout, hasRole } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="min-vh-100 d-flex flex-column">
      <Navbar bg="dark" variant="dark" expand="lg" className="shadow-sm">
        <Container fluid>
          <Navbar.Brand as={Link} to="/">
            <i className="fas fa-industry me-2"></i>
            Vendor Portal
          </Navbar.Brand>
          <Navbar.Toggle aria-controls="basic-navbar-nav" />
          <Navbar.Collapse id="basic-navbar-nav">
            <Nav className="me-auto">
              <Nav.Link as={Link} to="/">
                <i className="fas fa-home me-1"></i>
                Dashboard
              </Nav.Link>
              
              {(hasRole('Admin') || hasRole('Musteri')) && (
                <Nav.Link as={Link} to="/siparis?view=satin-alma">
                  <i className="fas fa-shopping-cart me-1"></i>
                  Satın Alma Siparişleri
                </Nav.Link>
              )}

              {(hasRole('Admin') || hasRole('Tedarikci')) && (
                <Nav.Link as={Link} to="/siparis?view=satis">
                  <i className="fas fa-box me-1"></i>
                  Satış Siparişleri
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
                  <i className="fas fa-box me-2"></i>
                  Malzemeler
                </NavDropdown.Item>
              </NavDropdown>
            </Nav>

            <Nav>
              <NavDropdown 
                title={
                  <>
                    <i className="fas fa-user me-1"></i>
                    {user?.name || user?.email}
                  </>
                } 
                id="user-dropdown"
                align="end"
              >
                <NavDropdown.Item>
                  <i className="fas fa-user-cog me-2"></i>
                  Profil
                </NavDropdown.Item>
                <NavDropdown.Item>
                  <i className="fas fa-cog me-2"></i>
                  Ayarlar
                </NavDropdown.Item>
                <NavDropdown.Divider />
                <NavDropdown.Item onClick={handleLogout}>
                  <i className="fas fa-sign-out-alt me-2"></i>
                  Çıkış Yap
                </NavDropdown.Item>
              </NavDropdown>
            </Nav>
          </Navbar.Collapse>
        </Container>
      </Navbar>

      <Container fluid className="flex-grow-1 py-4">
        <Outlet />
      </Container>

      <footer className="bg-light border-top py-3 mt-auto">
        <Container>
          <div className="text-center text-muted">
            &copy; {new Date().getFullYear()} Vendor Portal 2025 - Tüm hakları saklıdır.
          </div>
        </Container>
      </footer>
    </div>
  );
};

export default Layout;
