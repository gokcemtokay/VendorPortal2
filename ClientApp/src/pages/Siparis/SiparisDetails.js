import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { Card, Button, Badge, Row, Col, Table, Spinner } from 'react-bootstrap';
import { toast } from 'react-toastify';
import { siparisApi } from '../../api/apiClient';

const SiparisDetails = () => {
  const navigate = useNavigate();
  const { id } = useParams();
  const [siparis, setSiparis] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadSiparis();
  }, [id]);

  const loadSiparis = async () => {
    try {
      const response = await siparisApi.getSiparis(id);
      setSiparis(response.data.data);
    } catch (error) {
      toast.error('Sipariş bilgileri yüklenirken hata oluştu.');
      navigate('/siparis');
    } finally {
      setLoading(false);
    }
  };

  const getSiparisTuruText = (tur) => {
    switch (tur) {
      case 1: return <Badge bg="primary">Satın Alma</Badge>;
      case 2: return <Badge bg="success">Satış</Badge>;
      default: return '-';
    }
  };

  const getDurumText = (durum) => {
    switch (durum) {
      case 1: return <Badge bg="secondary">Oluşturuldu</Badge>;
      case 2: return <Badge bg="warning">Bekliyor</Badge>;
      case 3: return <Badge bg="success">Onaylandı</Badge>;
      case 4: return <Badge bg="info">İşleniyor</Badge>;
      case 5: return <Badge bg="primary">Tamamlandı</Badge>;
      case 6: return <Badge bg="danger">İptal</Badge>;
      default: return '-';
    }
  };

  const getKalemDurumText = (durum) => {
    switch (durum) {
      case 1: return <Badge bg="warning" size="sm">Bekliyor</Badge>;
      case 2: return <Badge bg="success" size="sm">Onaylandı</Badge>;
      case 3: return <Badge bg="danger" size="sm">Reddedildi</Badge>;
      case 4: return <Badge bg="info" size="sm">Revize</Badge>;
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

  if (!siparis) {
    return (
      <div className="alert alert-danger">
        Sipariş bulunamadı.
      </div>
    );
  }

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2>
          <i className="fas fa-shopping-cart me-2"></i>
          Sipariş Detayı
        </h2>
        <Button 
          variant="secondary" 
          onClick={() => navigate('/siparis')}
        >
          <i className="fas fa-arrow-left me-2"></i>
          Geri
        </Button>
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
              <strong>Sipariş No:</strong>
              <p className="mb-0">{siparis.referansNo}</p>
            </Col>
            <Col md={6} className="mb-3">
              <strong>Sipariş Tarihi:</strong>
              <p className="mb-0">
                {siparis.siparisTarihi 
                  ? new Date(siparis.siparisTarihi).toLocaleString('tr-TR')
                  : '-'}
              </p>
            </Col>
            <Col md={6} className="mb-3">
              <strong>Müşteri Firma:</strong>
              <p className="mb-0">{siparis.musteriFirma?.ad || '-'}</p>
            </Col>
            <Col md={6} className="mb-3">
              <strong>Tedarikçi Firma:</strong>
              <p className="mb-0">{siparis.tedarikciFirma?.ad || '-'}</p>
            </Col>
            <Col md={6} className="mb-3">
              <strong>Sipariş Türü:</strong>
              <p className="mb-0">{getSiparisTuruText(siparis.siparisTuru)}</p>
            </Col>
            <Col md={6} className="mb-3">
              <strong>Durum:</strong>
              <p className="mb-0">{getDurumText(siparis.durum)}</p>
            </Col>
            <Col md={6} className="mb-3">
              <strong>Para Birimi:</strong>
              <p className="mb-0">
                <Badge bg="secondary">{siparis.paraBirimi}</Badge>
              </p>
            </Col>
            <Col md={6} className="mb-3">
              <strong>Toplam Tutar:</strong>
              <p className="mb-0">
                <strong className="text-success">
                  {siparis.toplamTutar 
                    ? siparis.toplamTutar.toLocaleString('tr-TR', {
                        minimumFractionDigits: 2,
                        maximumFractionDigits: 2
                      })
                    : '0,00'} {siparis.paraBirimi}
                </strong>
              </p>
            </Col>
            {siparis.hedefTeslimatTarihi && (
              <Col md={6} className="mb-3">
                <strong>Hedef Teslimat:</strong>
                <p className="mb-0">
                  {new Date(siparis.hedefTeslimatTarihi).toLocaleDateString('tr-TR')}
                </p>
              </Col>
            )}
            {siparis.teslimatAdresi && (
              <Col md={12} className="mb-3">
                <strong>Teslimat Adresi:</strong>
                <p className="mb-0">{siparis.teslimatAdresi}</p>
              </Col>
            )}
            {siparis.aciklama && (
              <Col md={12} className="mb-3">
                <strong>Açıklama:</strong>
                <p className="mb-0">{siparis.aciklama}</p>
              </Col>
            )}
          </Row>
        </Card.Body>
      </Card>

      {siparis.kalemler && siparis.kalemler.length > 0 && (
        <Card className="shadow">
          <Card.Header className="bg-info text-white">
            <h5 className="mb-0">
              <i className="fas fa-list me-2"></i>
              Sipariş Kalemleri ({siparis.kalemler.length})
            </h5>
          </Card.Header>
          <Card.Body>
            <div className="table-responsive">
              <Table hover>
                <thead>
                  <tr>
                    <th>#</th>
                    <th>Malzeme Kodu</th>
                    <th>Malzeme Adı</th>
                    <th>Miktar</th>
                    <th>Birim</th>
                    <th>Birim Fiyat</th>
                    <th>Toplam Fiyat</th>
                    <th>Durum</th>
                  </tr>
                </thead>
                <tbody>
                  {siparis.kalemler.map((kalem, index) => (
                    <tr key={kalem.id}>
                      <td>{index + 1}</td>
                      <td>{kalem.malzemeKodu}</td>
                      <td>{kalem.malzemeAdi}</td>
                      <td>{kalem.miktar}</td>
                      <td>{kalem.birim}</td>
                      <td>
                        {kalem.birimFiyat 
                          ? kalem.birimFiyat.toLocaleString('tr-TR', { minimumFractionDigits: 2 })
                          : '0,00'}
                      </td>
                      <td>
                        <strong>
                          {kalem.toplamFiyat 
                            ? kalem.toplamFiyat.toLocaleString('tr-TR', { minimumFractionDigits: 2 })
                            : '0,00'}
                        </strong>
                      </td>
                      <td>{getKalemDurumText(kalem.durum)}</td>
                    </tr>
                  ))}
                </tbody>
                <tfoot className="table-light">
                  <tr>
                    <td colSpan="6" className="text-end">
                      <strong>GENEL TOPLAM:</strong>
                    </td>
                    <td colSpan="2">
                      <strong className="text-success">
                        {siparis.toplamTutar 
                          ? siparis.toplamTutar.toLocaleString('tr-TR', {
                              minimumFractionDigits: 2,
                              maximumFractionDigits: 2
                            })
                          : '0,00'} {siparis.paraBirimi}
                      </strong>
                    </td>
                  </tr>
                </tfoot>
              </Table>
            </div>
          </Card.Body>
        </Card>
      )}

      {siparis.onayGecmisleri && siparis.onayGecmisleri.length > 0 && (
        <Card className="shadow mt-4">
          <Card.Header className="bg-secondary text-white">
            <h5 className="mb-0">
              <i className="fas fa-history me-2"></i>
              Sipariş Geçmişi
            </h5>
          </Card.Header>
          <Card.Body>
            <div className="timeline">
              {siparis.onayGecmisleri.map((gecmis, index) => (
                <div key={gecmis.id} className="timeline-item mb-3">
                  <div className="d-flex">
                    <div className="me-3">
                      <i className="fas fa-circle text-primary"></i>
                    </div>
                    <div className="flex-grow-1">
                      <p className="mb-1">
                        <strong>{gecmis.aciklama}</strong>
                      </p>
                      <small className="text-muted">
                        {gecmis.islemTarihi 
                          ? new Date(gecmis.islemTarihi).toLocaleString('tr-TR')
                          : '-'}
                      </small>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </Card.Body>
        </Card>
      )}
    </div>
  );
};

export default SiparisDetails;
