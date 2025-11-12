import React, { useState, useEffect } from 'react';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { Card, Button, Badge, Row, Col, Spinner } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { firmaApi } from '../../api/apiClient';
import { useAuth } from '../../context/AuthContext';

const FirmaDetails = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const { hasRole } = useAuth();
  const [firma, setFirma] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadFirma();
  }, [id]);

  const loadFirma = async () => {
    try {
      const response = await firmaApi.getById(id);
      setFirma(response.data.data);
    } catch (error) {
      toast.error('Firma bilgileri yüklenirken hata oluştu.');
      navigate('/firma');
    } finally {
      setLoading(false);
    }
  };

  const handleOnayla = async () => {
    if (!window.confirm('Bu firmayı onaylamak istediğinizden emin misiniz?')) {
      return;
    }

    try {
      await firmaApi.onayla(id);
      toast.success('Firma başarıyla onaylandı!');
      loadFirma();
    } catch (error) {
      toast.error('Firma onaylanırken hata oluştu.');
    }
  };

  const getFirmaTipiText = (tip) => {
    switch (tip) {
      case 1: return <Badge bg="primary">Müşteri</Badge>;
      case 2: return <Badge bg="success">Tedarikçi</Badge>;
      case 3: return <Badge bg="info">Her İkisi</Badge>;
      default: return '-';
    }
  };

  const getDurumText = (durum) => {
    switch (durum) {
      case 1: return <Badge bg="warning">Beklemede</Badge>;
      case 2: return <Badge bg="success">Onaylandı</Badge>;
      case 3: return <Badge bg="danger">Reddedildi</Badge>;
      case 4: return <Badge bg="secondary">Pasif</Badge>;
      default: return '-';
    }
  };

  if (loading) {
    return (
      <div className="d-flex justify-content-center align-items-center" style={{ minHeight: '400px' }}>
        <Spinner animation="border" variant="primary" />
      </div>
    );
  }

  if (!firma) {
    return (
      <div className="alert alert-danger">
        Firma bulunamadı.
      </div>
    );
  }

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>
          <i className="fas fa-building me-2"></i>
          Firma Detayı
        </h2>
        <div>
          <Button 
            variant="secondary" 
            onClick={() => navigate('/firma')}
            className="me-2"
          >
            <i className="fas fa-arrow-left me-2"></i>
            Geri
          </Button>
          {hasRole('Admin') && (
            <Link to={`/firma/edit/${id}`} className="btn btn-primary">
              <i className="fas fa-edit me-2"></i>
              Düzenle
            </Link>
          )}
        </div>
      </div>

      <Card className="shadow mb-4">
        <Card.Header className="bg-primary text-white">
          <h5 className="mb-0">
            <i className="fas fa-info-circle me-2"></i>
            Genel Bilgiler
          </h5>
        </Card.Header>
        <Card.Body>
          <Row>
            <Col md={6} className="mb-3">
              <strong>Firma Adı:</strong>
              <p className="mb-0">{firma.ad}</p>
            </Col>
            <Col md={6} className="mb-3">
              <strong>Vergi No:</strong>
              <p className="mb-0">{firma.vergiNo || '-'}</p>
            </Col>
            <Col md={6} className="mb-3">
              <strong>Email:</strong>
              <p className="mb-0">{firma.email || '-'}</p>
            </Col>
            <Col md={6} className="mb-3">
              <strong>Telefon:</strong>
              <p className="mb-0">{firma.telefon || '-'}</p>
            </Col>
            <Col md={6} className="mb-3">
              <strong>Web Sitesi:</strong>
              <p className="mb-0">
                {firma.webSitesi ? (
                  <a href={firma.webSitesi} target="_blank" rel="noopener noreferrer">
                    {firma.webSitesi}
                  </a>
                ) : '-'}
              </p>
            </Col>
            <Col md={6} className="mb-3">
              <strong>Firma Tipi:</strong>
              <p className="mb-0">{getFirmaTipiText(firma.firmaTipi)}</p>
            </Col>
            <Col md={12} className="mb-3">
              <strong>Adres:</strong>
              <p className="mb-0">{firma.adres || '-'}</p>
            </Col>
          </Row>
        </Card.Body>
      </Card>

      <Card className="shadow">
        <Card.Header className="bg-info text-white">
          <h5 className="mb-0">
            <i className="fas fa-chart-line me-2"></i>
            Durum Bilgileri
          </h5>
        </Card.Header>
        <Card.Body>
          <Row>
            <Col md={6} className="mb-3">
              <strong>Durum:</strong>
              <p className="mb-0">{getDurumText(firma.durum)}</p>
            </Col>
            <Col md={6} className="mb-3">
              <strong>Oluşturma Tarihi:</strong>
              <p className="mb-0">
                {firma.createdDate 
                  ? new Date(firma.createdDate).toLocaleString('tr-TR')
                  : '-'}
              </p>
            </Col>
            {firma.onayTarihi && (
              <Col md={6} className="mb-3">
                <strong>Onay Tarihi:</strong>
                <p className="mb-0">
                  {new Date(firma.onayTarihi).toLocaleString('tr-TR')}
                </p>
              </Col>
            )}
          </Row>

          {(hasRole('Admin') || hasRole('Musteri')) && firma.durum === 1 && (
            <div className="mt-3">
              <Button 
                variant="success" 
                onClick={handleOnayla}
              >
                <i className="fas fa-check me-2"></i>
                Firmayı Onayla
              </Button>
            </div>
          )}
        </Card.Body>
      </Card>
    </div>
  );
};

export default FirmaDetails;
