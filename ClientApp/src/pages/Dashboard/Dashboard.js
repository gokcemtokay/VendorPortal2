import React, { useState, useEffect } from 'react';
import { Card, Row, Col } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { firmaApi, siparisApi, ihaleApi } from '../../api/apiClient';

const Dashboard = () => {
  const { user } = useAuth();
  const [stats, setStats] = useState({
    totalFirmalar: 0,
    totalSiparisler: 0,
    totalIhaleler: 0,
    bekleyenSiparisler: 0
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    try {
      // Bu örnekte basit sayılar gösteriyoruz
      // Gerçek uygulamada API'den dashboard istatistikleri çekilir
      setStats({
        totalFirmalar: 246,
        totalSiparisler: 1250,
        totalIhaleler: 15,
        bekleyenSiparisler: 32
      });
    } catch (error) {
      console.error('Dashboard verileri yüklenirken hata:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="d-flex justify-content-center align-items-center" style={{ minHeight: '400px' }}>
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Yükleniyor...</span>
        </div>
      </div>
    );
  }

  return (
    <div>
      <div className="mb-4">
        <h1 className="display-4">
          <i className="fas fa-tachometer-alt me-3"></i>
          Hoş Geldiniz, {user?.name || user?.email}!
        </h1>
        <p className="lead text-muted">Vendor Portal - Tedarik Zinciri Yönetim Sistemi</p>
        <hr />
      </div>

      <Row className="g-4 mb-4">
        <Col md={4}>
          <Card className="border-primary shadow-sm h-100">
            <Card.Body>
              <div className="d-flex justify-content-between align-items-center">
                <div>
                  <h5 className="card-title text-primary">Toplam Firma</h5>
                  <h2 className="display-4">{stats.totalFirmalar}</h2>
                  <p className="text-muted mb-0">180 Adet Tedarikçi</p>
                </div>
                <div>
                  <i className="fas fa-building fa-3x text-primary"></i>
                </div>
              </div>
            </Card.Body>
            <Card.Footer className="bg-primary text-white">
              <Link to="/firma" className="text-white text-decoration-none">
                Detayları Gör <i className="fas fa-arrow-right"></i>
              </Link>
            </Card.Footer>
          </Card>
        </Col>

        <Col md={4}>
          <Card className="border-success shadow-sm h-100">
            <Card.Body>
              <div className="d-flex justify-content-between align-items-center">
                <div>
                  <h5 className="card-title text-success">Bekleyen Tedarikçi</h5>
                  <h2 className="display-4">8</h2>
                  <p className="text-muted mb-0">Başvurusu</p>
                </div>
                <div>
                  <i className="fas fa-check-circle fa-3x text-success"></i>
                </div>
              </div>
            </Card.Body>
            <Card.Footer className="bg-success text-white">
              <Link to="/firma" className="text-white text-decoration-none">
                Onayları İncele <i className="fas fa-arrow-right"></i>
              </Link>
            </Card.Footer>
          </Card>
        </Col>

        <Col md={4}>
          <Card className="border-info shadow-sm h-100">
            <Card.Body>
              <div className="d-flex justify-content-between align-items-center">
                <div>
                  <h5 className="card-title text-info">Aktif İhale</h5>
                  <h2 className="display-4">{stats.totalIhaleler}</h2>
                  <p className="text-muted mb-0">3 Çok Yakında Biliyor</p>
                </div>
                <div>
                  <i className="fas fa-gavel fa-3x text-info"></i>
                </div>
              </div>
            </Card.Body>
            <Card.Footer className="bg-info text-white">
              <Link to="/ihale" className="text-white text-decoration-none">
                İhaleleri Gör <i className="fas fa-arrow-right"></i>
              </Link>
            </Card.Footer>
          </Card>
        </Col>
      </Row>

      <Row className="g-4 mb-4">
        <Col md={4}>
          <Card className="shadow-sm">
            <Card.Header className="bg-secondary text-white">
              <h5 className="mb-0"><i className="fas fa-clipboard-list me-2"></i>Onay Bekleyen Siparişler</h5>
            </Card.Header>
            <Card.Body>
              <h2 className="display-4">{stats.bekleyenSiparisler}</h2>
              <p className="text-muted">Tüm Firmalar</p>
            </Card.Body>
          </Card>
        </Col>

        <Col md={4}>
          <Card className="shadow-sm">
            <Card.Header className="bg-warning text-dark">
              <h5 className="mb-0"><i className="fas fa-chart-line me-2"></i>Toplam Sipariş Kalemi</h5>
            </Card.Header>
            <Card.Body>
              <h2 className="display-4">{stats.totalSiparisler}</h2>
              <p className="text-muted">Bu Ay</p>
            </Card.Body>
          </Card>
        </Col>

        <Col md={4}>
          <Card className="shadow-sm">
            <Card.Header className="bg-danger text-white">
              <h5 className="mb-0"><i className="fas fa-calendar-alt me-2"></i>Yaklaşan Teslimatlar</h5>
            </Card.Header>
            <Card.Body>
              <h2 className="display-4">7</h2>
              <p className="text-muted">Önümüzdeki Hafta</p>
            </Card.Body>
          </Card>
        </Col>
      </Row>

      <Row>
        <Col>
          <Card className="shadow-sm">
            <Card.Header className="bg-dark text-white">
              <h5 className="mb-0"><i className="fas fa-info-circle me-2"></i>Hızlı Erişim</h5>
            </Card.Header>
            <Card.Body>
              <Row className="g-3">
                <Col md={3}>
                  <Link to="/siparis?view=satin-alma" className="btn btn-outline-primary w-100 py-3">
                    <i className="fas fa-shopping-cart fa-2x d-block mb-2"></i>
                    Satın Alma Siparişleri
                  </Link>
                </Col>
                <Col md={3}>
                  <Link to="/siparis?view=satis" className="btn btn-outline-success w-100 py-3">
                    <i className="fas fa-box fa-2x d-block mb-2"></i>
                    Satış Siparişleri
                  </Link>
                </Col>
                <Col md={3}>
                  <Link to="/ihale" className="btn btn-outline-info w-100 py-3">
                    <i className="fas fa-gavel fa-2x d-block mb-2"></i>
                    İhale Yönetimi
                  </Link>
                </Col>
                <Col md={3}>
                  <Link to="/malzeme" className="btn btn-outline-warning w-100 py-3">
                    <i className="fas fa-box fa-2x d-block mb-2"></i>
                    Malzeme Yönetimi
                  </Link>
                </Col>
              </Row>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </div>
  );
};

export default Dashboard;
