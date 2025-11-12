import React, { useState, useEffect } from 'react';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { Card, Button, Badge, Row, Col, Table, Spinner } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { ihaleApi } from '../../api/apiClient';
import { useAuth } from '../../context/AuthContext';

const IhaleDetails = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const { hasRole } = useAuth();
  const [ihale, setIhale] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadIhale();
  }, [id]);

  const loadIhale = async () => {
    try {
      const response = await ihaleApi.getById(id);
      setIhale(response.data.data);
    } catch (error) {
      toast.error('İhale bilgileri yüklenirken hata oluştu.');
      navigate('/ihale');
    } finally {
      setLoading(false);
    }
  };

  const handleYayinla = async () => {
    if (!window.confirm('Bu ihaleyi yayınlamak istediğinizden emin misiniz?')) {
      return;
    }

    try {
      await ihaleApi.yayinla(id);
      toast.success('İhale başarıyla yayınlandı!');
      loadIhale();
    } catch (error) {
      toast.error('İhale yayınlanırken hata oluştu.');
    }
  };

  const handleIptal = async () => {
    const iptalNedeni = window.prompt('İptal nedenini giriniz:');
    if (!iptalNedeni) return;

    try {
      await ihaleApi.iptalEt(id, iptalNedeni);
      toast.success('İhale iptal edildi.');
      loadIhale();
    } catch (error) {
      toast.error('İhale iptal edilirken hata oluştu.');
    }
  };

  const getIhaleTuruText = (tur) => {
    switch (tur) {
      case 1: return <Badge bg="info">Herkese Açık</Badge>;
      case 2: return <Badge bg="warning">Davetli</Badge>;
      case 3: return <Badge bg="secondary">Kapalı</Badge>;
      default: return '-';
    }
  };

  const getDurumText = (durum) => {
    switch (durum) {
      case 1: return <Badge bg="secondary">Taslak</Badge>;
      case 2: return <Badge bg="success">Yayında</Badge>;
      case 3: return <Badge bg="primary">Tamamlandı</Badge>;
      case 4: return <Badge bg="danger">İptal</Badge>;
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

  if (!ihale) {
    return (
      <div className="alert alert-danger">
        İhale bulunamadı.
      </div>
    );
  }

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>
          <i className="fas fa-gavel me-2"></i>
          İhale Detayı
        </h2>
        <div>
          <Button 
            variant="secondary" 
            onClick={() => navigate('/ihale')}
            className="me-2"
          >
            <i className="fas fa-arrow-left me-2"></i>
            Geri
          </Button>
          
          {ihale.durum === 2 && (
            <Link 
              to={`/ihale/teklifler/${id}`}
              className="btn btn-primary me-2"
            >
              <i className="fas fa-file-invoice-dollar me-2"></i>
              Teklifleri Gör
            </Link>
          )}

          {ihale.durum === 1 && (hasRole('Admin') || hasRole('Musteri')) && (
            <Button 
              variant="success"
              onClick={handleYayinla}
              className="me-2"
            >
              <i className="fas fa-rocket me-2"></i>
              Yayınla
            </Button>
          )}

          {ihale.durum === 2 && (hasRole('Admin') || hasRole('Musteri')) && (
            <Button 
              variant="danger"
              onClick={handleIptal}
            >
              <i className="fas fa-ban me-2"></i>
              İptal Et
            </Button>
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
              <strong>İhale Adı:</strong>
              <p className="mb-0">{ihale.ihaleAdi}</p>
            </Col>
            <Col md={6} className="mb-3">
              <strong>Referans No:</strong>
              <p className="mb-0">{ihale.referansNo}</p>
            </Col>
            <Col md={6} className="mb-3">
              <strong>Müşteri Firma:</strong>
              <p className="mb-0">{ihale.musteriFirma?.ad || '-'}</p>
            </Col>
            <Col md={6} className="mb-3">
              <strong>İhale Türü:</strong>
              <p className="mb-0">{getIhaleTuruText(ihale.ihaleTuru)}</p>
            </Col>
            <Col md={6} className="mb-3">
              <strong>Durum:</strong>
              <p className="mb-0">{getDurumText(ihale.durum)}</p>
            </Col>
            <Col md={6} className="mb-3">
              <strong>Para Birimi:</strong>
              <p className="mb-0">
                <Badge bg="secondary">{ihale.paraBirimi}</Badge>
              </p>
            </Col>
            <Col md={6} className="mb-3">
              <strong>Teklif Başlangıç:</strong>
              <p className="mb-0">
                {ihale.teklifBaslangicTarihi 
                  ? new Date(ihale.teklifBaslangicTarihi).toLocaleString('tr-TR')
                  : '-'}
              </p>
            </Col>
            <Col md={6} className="mb-3">
              <strong>Teklif Bitiş:</strong>
              <p className="mb-0">
                {ihale.teklifBitisTarihi 
                  ? new Date(ihale.teklifBitisTarihi).toLocaleString('tr-TR')
                  : '-'}
              </p>
            </Col>
            <Col md={12} className="mb-3">
              <strong>Açıklama:</strong>
              <p className="mb-0">{ihale.aciklama || '-'}</p>
            </Col>
          </Row>
        </Card.Body>
      </Card>

      {ihale.kalemler && ihale.kalemler.length > 0 && (
        <Card className="shadow">
          <Card.Header className="bg-info text-white">
            <h5 className="mb-0">
              <i className="fas fa-list me-2"></i>
              İhale Kalemleri ({ihale.kalemler.length})
            </h5>
          </Card.Header>
          <Card.Body>
            <Table hover>
              <thead>
                <tr>
                  <th>Kalem No</th>
                  <th>Malzeme</th>
                  <th>Miktar</th>
                  <th>Birim</th>
                  <th>Açıklama</th>
                </tr>
              </thead>
              <tbody>
                {ihale.kalemler.map((kalem, index) => (
                  <tr key={kalem.id}>
                    <td>{index + 1}</td>
                    <td>{kalem.malzeme?.ad || kalem.malzemeKodu}</td>
                    <td>{kalem.miktar}</td>
                    <td>{kalem.birim}</td>
                    <td>{kalem.aciklama || '-'}</td>
                  </tr>
                ))}
              </tbody>
            </Table>
          </Card.Body>
        </Card>
      )}
    </div>
  );
};

export default IhaleDetails;
